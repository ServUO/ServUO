#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

using Server;
using Server.Accounting;
using Server.Engines.Help;
using Server.Misc;
using Server.Mobiles;

using VitaNex.IO;
using VitaNex.SuperGumps.UI;
using VitaNex.Text;
#endregion

namespace VitaNex.Modules.AntiAdverts
{
	public static partial class AntiAdverts
	{
		public const AccessLevel Access = AccessLevel.Administrator;

		public static AntiAdvertsOptions CMOptions { get; private set; }

		public static FileInfo ReportsFile => IOUtility.EnsureFile(VitaNexCore.SavesDirectory + "/AntiAdverts/Reports.bin");

		public static List<AntiAdvertsReport> Reports { get; private set; }

		public static event Action<AntiAdvertNotifyGump> OnBeforeSendGump;
		public static event Action<AntiAdvertNotifyGump> OnAfterSendGump;

		private static void InvokeOnBeforeSendGump(AntiAdvertNotifyGump g)
		{
			if (OnBeforeSendGump != null)
			{
				OnBeforeSendGump(g);
			}
		}

		private static void InvokeOnAfterSendGump(AntiAdvertNotifyGump g)
		{
			if (OnAfterSendGump != null)
			{
				OnAfterSendGump(g);
			}
		}

		public static bool Detect(string text)
		{

			return Detect(text, out var keyword);
		}

		public static bool Detect(string text, out string keyword)
		{
			// Does the speech contain any forbidden key words?
			foreach (var kw in CMOptions.KeyWords)
			{
				if (CMOptions.SearchMode.Execute(text, kw, CMOptions.SearchCapsIgnore))
				{
					keyword = kw;
					return true;
				}

				if (!kw.Contains(' '))
				{
					continue;
				}

				foreach (var kwr in CMOptions.WhitespaceAliases.Select(wa => kw.Replace(' ', wa))
											 .Where(kwr => CMOptions.SearchMode.Execute(text, kwr, CMOptions.SearchCapsIgnore)))
				{
					keyword = kwr;
					return true;
				}
			}

			keyword = null;
			return false;
		}

		private static void OnSpeech(SpeechEventArgs e)
		{
			if (e == null || !(e.Mobile is PlayerMobile) || e.Mobile.NetState == null || String.IsNullOrWhiteSpace(e.Speech) ||
				e.Mobile.AccessLevel > CMOptions.HandleAccess)
			{
				return;
			}


			if (!Detect(e.Speech, out var detected))
			{
				return;
			}

			var banned = Ban(e.Mobile, e.Speech);
			var jailed = Jail(e.Mobile, e.Speech);

			ToConsole(e.Mobile, e.Speech, jailed, banned);
			ToLog(e.Mobile, e.Speech, jailed, banned);

			SendPage(e.Mobile, e.Speech, jailed, banned);
			SendWarning(e.Mobile, e.Speech, jailed, banned);

			if (CMOptions.Squelch)
			{
				e.Mobile.Squelched = true;
			}

			if (CMOptions.Kick)
			{
				e.Mobile.Say("I've been kicked!");

				if (e.Mobile.NetState != null)
				{
					e.Mobile.NetState.Dispose();
				}
			}

			Reports.Insert(
				0,
				new AntiAdvertsReport(
					DateTime.Now,
					e.Mobile,
					GetDetectedString(false, e.Mobile, e.Speech, jailed, banned),
					e.Speech,
					jailed,
					banned));

			Reports.TrimEndTo(100);
		}

		private static string GetDetectedString(bool lines, Mobile m, string speech, bool jailed, bool banned)
		{
			if (m == null || m.Deleted || !m.Player || String.IsNullOrWhiteSpace(speech))
			{
				return String.Empty;
			}

			return String.Format(
				lines
					? "IP('{0}')\nAccount('{1}')\nChar('{2}')\nJail('{3}')\nBan('{4}')\nQuote(\"{5}\")"
					: "IP('{0}') Account('{1}') Char('{2}') Jail('{3}') Ban('{4}') Quote(\"{5}\")",
				m.NetState.Address,
				m.Account.Username,
				m.RawName,
				jailed ? "yes" : "no",
				banned ? "yes" : "no",
				speech);
		}

		private static void ToConsole(Mobile m, string speech, bool jailed, bool banned)
		{
			if (m != null && !m.Deleted && m.Player && !String.IsNullOrWhiteSpace(speech) && CMOptions.ConsoleWrite)
			{
				Console.WriteLine("[Warning: Advertising Detected]: " + GetDetectedString(false, m, speech, jailed, banned));
			}
		}

		private static void ToLog(Mobile m, string speech, bool jailed, bool banned)
		{
			if (m != null && !m.Deleted && !String.IsNullOrWhiteSpace(speech) && CMOptions.LogEnabled)
			{
				String.Format(
						  "[{0}]: {1}",
						  DateTime.Now.TimeOfDay.ToSimpleString("h:m"),
						  GetDetectedString(false, m, speech, jailed, banned))
					  .Log("DetectedAdvertisers.log");
			}
		}

		private static void SendPage(Mobile m, string speech, bool jailed, bool banned)
		{
			if (m == null || m.Deleted || !m.Player || String.IsNullOrWhiteSpace(speech) || !CMOptions.PageStaff)
			{
				return;
			}

			var entry = PageQueue.GetEntry(m);

			if (entry == null || !entry.Message.StartsWith("[Warning: Advertising Detected]"))
			{
				PageQueue.Enqueue(
					new PageEntry(
						m,
						"[Warning: Advertising Detected]: " + GetDetectedString(true, m, speech, jailed, banned),
						PageType.Account));
			}
		}

		private static void SendWarning(Mobile m, string speech, bool jailed, bool banned)
		{
			if (m == null || m.Deleted || !m.Player || String.IsNullOrWhiteSpace(speech))
			{
				return;
			}

			string message;

			if (CMOptions.NotifyStaff)
			{
				message = "[Warning: Advertising Detected]: " + GetDetectedString(true, m, speech, jailed, banned);

				Notify.Notify.Broadcast<AntiAdvertNotifyGump>(
					message,
					false,
					1.0,
					3.0,
					Color.OrangeRed,
					InvokeOnBeforeSendGump,
					InvokeOnAfterSendGump,
					CMOptions.NotifyAccess);
			}

			if (!CMOptions.NotifyPlayer)
			{
				return;
			}

			var pm = m as PlayerMobile;

			if (pm == null)
			{
				return;
			}

			message = String.Empty;

			message += "A recent check shows that you may be trying to advertise on " + ServerList.ServerName + ".\n";
			message += "Advertising is not allowed.\n";

			if (jailed)
			{
				message += "You have been jailed until a member of staff can review your case.\n";
				message += "If you are found to be innocent, you will be promptly released and free to adventure again.\n";
			}
			else
			{
				message += "A report has been submitted to the staff team for review.\n";
			}

			message += "\nOffending Speech:\n" + speech;

			new NoticeDialogGump(pm)
			{
				Width = 420,
				Height = 420,
				CanMove = false,
				CanDispose = false,
				BlockSpeech = true,
				RandomButtonID = true,
				Title = jailed ? "Jailed! Why?" : "Advertising Reported",
				Html = message
			}.Send();
		}

		private static bool Jail(Mobile m, string speech)
		{
			return m != null && !m.Deleted && m.Player && !String.IsNullOrWhiteSpace(speech) && CMOptions.Jail &&
				   CMOptions.JailPoint.MoveToWorld(m);
		}

		private static bool Ban(Mobile m, string speech)
		{
			if (m == null || m.Deleted || !m.Player || String.IsNullOrWhiteSpace(speech) || !CMOptions.Ban)
			{
				return false;
			}

			var a = m.Account as Account;

			if (a != null)
			{
				a.Banned = true;
				a.SetBanTags(null, DateTime.UtcNow, TimeSpan.MaxValue);

				return true;
			}

			return false;
		}
	}
}