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
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using Server;
using Server.Commands;
using Server.Network;

using VitaNex.IO;
using VitaNex.Text;
#endregion

namespace VitaNex.Notify
{
	public static partial class Notify
	{
		public const AccessLevel Access = AccessLevel.Administrator;

		public static CoreServiceOptions CSOptions { get; private set; }

		public static Type[] GumpTypes { get; private set; }

		public static Dictionary<Type, Type[]> NestedTypes { get; private set; }

		public static Dictionary<Type, Type> SettingsMap { get; private set; }

		public static BinaryDataStore<Type, NotifySettings> Settings { get; private set; }

		public static event Action<NotifyGump> OnGlobalMessage;
		public static event Action<NotifyGump> OnLocalMessage;

		public static event Action<string> OnBroadcast;

		[Usage("Notify <text | html | bbc>"), Description(
			 "Send a global notification gump to all online clients, " + //
			 "containing a message parsed from HTML, BBS or plain text.")]
		private static void HandleNotify(CommandEventArgs e)
		{
			if (e.Mobile.AccessLevel >= AccessLevel.Seer && !String.IsNullOrWhiteSpace(e.ArgString) && ValidateCommand(e))
			{
				Broadcast(e.Mobile, e.ArgString);
				return;
			}

			new NotifySettingsGump(e.Mobile).Send();
		}

		[Usage("NotifyAC <text | html | bbc>"), Description(
			 "Send a global notification gump to all online clients, " + //
			 "containing a message parsed from HTML, BBS or plain text, " + //
			 "which auto-closes after 10 seconds.")]
		private static void HandleNotifyAC(CommandEventArgs e)
		{
			if (ValidateCommand(e))
			{
				Broadcast(e.Mobile, e.ArgString, true);
			}
		}

		[Usage("NotifyNA <text | html | bbc>"), Description(
			 "Send a global notification gump to all online clients, " + //
			 "containing a message parsed from HTML, BBS or plain text, " + //
			 "which has no animation delay.")]
		private static void HandleNotifyNA(CommandEventArgs e)
		{
			if (ValidateCommand(e))
			{
				Broadcast(e.Mobile, e.ArgString, false, false);
			}
		}

		[Usage("NotifyACNA <text | html | bbc>"), Description(
			 "Send a global notification gump to all online clients, " + //
			 "containing a message parsed from HTML, BBS or plain text, " + //
			 "which auto-closes after 10 seconds and has no animation delay.")]
		private static void HandleNotifyACNA(CommandEventArgs e)
		{
			if (ValidateCommand(e))
			{
				Broadcast(e.Mobile, e.ArgString, true, false);
			}
		}

		private static bool ValidateCommand(CommandEventArgs e)
		{
			if (e == null || e.Mobile == null)
			{
				return false;
			}

			if (String.IsNullOrWhiteSpace(e.ArgString) ||
				String.IsNullOrWhiteSpace(Regex.Replace(e.ArgString, @"<[^>]*>", String.Empty)))
			{
				e.Mobile.SendMessage(0x22, "Html/BBC message must be at least 1 character and not all white-space after parsing.");
				e.Mobile.SendMessage(0x22, "Usage: {0}{1} <text | html | bbc>", CommandSystem.Prefix, e.Command);
				return false;
			}

			return true;
		}

		public static NotifySettings EnsureSettings<TGump>()
			where TGump : NotifyGump
		{
			return EnsureSettings(typeof(TGump));
		}

		public static NotifySettings EnsureSettings(Type t)
		{
			if (t == null || !t.IsEqualOrChildOf<NotifyGump>())
			{
				return null;
			}

			if (SettingsMap.TryGetValue(t, out var st) && st != null)
			{
				var o = Settings.GetValue(st);

				if (o != null)
				{
					return o;
				}
			}

			const BindingFlags f = BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.NonPublic;

			var m = t.GetMethod("InitSettings", f);

			if (m == null)
			{
				foreach (var p in t.EnumerateHierarchy())
				{
					m = p.GetMethod("InitSettings", f);

					if (m != null)
					{
						st = p;
						break;
					}
				}
			}

			if (st == null)
			{
				st = t;
			}

			var init = false;

			if (!Settings.TryGetValue(st, out var settings) || settings == null)
			{
				Settings[st] = settings = new NotifySettings(st);
				init = true;
			}

			SettingsMap[t] = st;

			if (init && m != null)
			{
				m.Invoke(null, new object[] { settings });
			}

			return settings;
		}

		public static bool IsAutoClose<TGump>(Mobile pm)
			where TGump : NotifyGump
		{
			return IsAutoClose(typeof(TGump), pm);
		}

		public static bool IsAutoClose(Type t, Mobile pm)
		{
			var settings = EnsureSettings(t);

			return settings != null && settings.IsAutoClose(pm);
		}

		public static bool IsIgnored<TGump>(Mobile pm)
			where TGump : NotifyGump
		{
			return IsIgnored(typeof(TGump), pm);
		}

		public static bool IsIgnored(Type t, Mobile pm)
		{
			var settings = EnsureSettings(t);

			return settings != null && settings.IsIgnored(pm);
		}

		public static bool IsAnimated<TGump>(Mobile pm)
			where TGump : NotifyGump
		{
			return IsAnimated(typeof(TGump), pm);
		}

		public static bool IsAnimated(Type t, Mobile pm)
		{
			var settings = EnsureSettings(t);

			return settings == null || settings.IsAnimated(pm);
		}

		public static bool IsTextOnly<TGump>(Mobile pm)
			where TGump : NotifyGump
		{
			return IsTextOnly(typeof(TGump), pm);
		}

		public static bool IsTextOnly(Type t, Mobile pm)
		{
			var settings = EnsureSettings(t);

			return settings != null && settings.IsTextOnly(pm);
		}

		public static void AlterTime<TGump>(Mobile pm, ref double value)
			where TGump : NotifyGump
		{
			AlterTime(typeof(TGump), pm, ref value);
		}

		public static void AlterTime(Type t, Mobile pm, ref double value)
		{
			var settings = EnsureSettings(t);

			if (settings != null)
			{
				settings.AlterTime(pm, ref value);
			}
		}

		public static void Broadcast(
			Mobile m,
			string message,
			bool autoClose = false,
			bool animate = true,
			AccessLevel level = AccessLevel.Player)
		{
			if (m != null && !m.Deleted)
			{
				message = String.Format("[{0}] {1}:\n{2}", DateTime.Now.ToSimpleString("t@h:m@"), m.RawName, message);
			}

			Broadcast(message, autoClose, animate ? 1.0 : 0.0, 10.0, null, null, null, level);
		}

		public static void Broadcast(
			string html,
			bool autoClose = true,
			double delay = 1.0,
			double pause = 5.0,
			Color? color = null,
			Action<WorldNotifyGump> beforeSend = null,
			Action<WorldNotifyGump> afterSend = null,
			AccessLevel level = AccessLevel.Player)
		{
			Broadcast<WorldNotifyGump>(html, autoClose, delay, pause, color, beforeSend, afterSend, level);
		}

		public static void Broadcast<TGump>(
			string html,
			bool autoClose = true,
			double delay = 1.0,
			double pause = 5.0,
			Color? color = null,
			Action<TGump> beforeSend = null,
			Action<TGump> afterSend = null,
			AccessLevel level = AccessLevel.Player)
			where TGump : NotifyGump
		{
			var c = NetState.Instances.Count;

			while (--c >= 0)
			{
				if (!NetState.Instances.InBounds(c))
				{
					continue;
				}

				var ns = NetState.Instances[c];

				if (ns != null && ns.Running && ns.Mobile != null && ns.Mobile.AccessLevel >= level)
				{
					VitaNexCore.TryCatch(
						m => Send(false, m, html, autoClose, delay, pause, color, beforeSend, afterSend, level),
						ns.Mobile);
				}
			}

			if (level < AccessLevel.Counselor && OnBroadcast != null)
			{
				OnBroadcast(html.ParseBBCode());
			}
		}

		public static void Send(
			Mobile m,
			string html,
			bool autoClose = true,
			double delay = 1.0,
			double pause = 3.0,
			Color? color = null,
			Action<NotifyGump> beforeSend = null,
			Action<NotifyGump> afterSend = null,
			AccessLevel level = AccessLevel.Player)
		{
			Send(true, m, html, autoClose, delay, pause, color, beforeSend, afterSend, level);
		}

		public static void Send<TGump>(
			Mobile m,
			string html,
			bool autoClose = true,
			double delay = 1.0,
			double pause = 3.0,
			Color? color = null,
			Action<TGump> beforeSend = null,
			Action<TGump> afterSend = null,
			AccessLevel level = AccessLevel.Player)
			where TGump : NotifyGump
		{
			Send(true, m, html, autoClose, delay, pause, color, beforeSend, afterSend, level);
		}

		private static void Send<TGump>(
			bool local,
			Mobile m,
			string html,
			bool autoClose = true,
			double delay = 1.0,
			double pause = 3.0,
			Color? color = null,
			Action<TGump> beforeSend = null,
			Action<TGump> afterSend = null,
			AccessLevel level = AccessLevel.Player)
			where TGump : NotifyGump
		{
			if (!m.IsOnline() || m.AccessLevel < level)
			{
				return;
			}

			var t = typeof(TGump);

			if (t.IsAbstract || m.HasGump(t))
			{
				if (!NestedTypes.TryGetValue(t, out var subs) || subs == null)
				{
					NestedTypes[t] = subs = t.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic) //
											 .Where(st => st.IsChildOf<NotifyGump>())
											 .ToArray();
				}

				var sub = subs.FirstOrDefault(st => !m.HasGump(st));

				if (sub != null)
				{
					t = sub;
				}
			}

			if (IsIgnored(t, m))
			{
				return;
			}

			if (!t.IsAbstract && !IsTextOnly(t, m))
			{
				if (!autoClose && IsAutoClose(t, m))
				{
					autoClose = true;
				}

				if (delay > 0.0 && !IsAnimated(t, m))
				{
					delay = 0.0;
				}

				if (delay > 0.0)
				{
					AlterTime(t, m, ref delay);
				}

				if (pause > 3.0)
				{
					AlterTime(t, m, ref pause);

					pause = Math.Max(3.0, pause);
				}

				var ng = t.CreateInstanceSafe<TGump>(m, html);

				if (ng != null)
				{
					ng.AutoClose = autoClose;
					ng.AnimDuration = TimeSpan.FromSeconds(Math.Max(0, delay));
					ng.PauseDuration = TimeSpan.FromSeconds(Math.Max(0, pause));
					ng.HtmlColor = color ?? Color.White;

					if (ng.IsDisposed)
					{
						return;
					}

					if (local && OnLocalMessage != null)
					{
						OnLocalMessage(ng);
					}
					else if (!local && OnGlobalMessage != null)
					{
						OnGlobalMessage(ng);
					}

					if (beforeSend != null)
					{
						beforeSend(ng);
					}

					if (ng.IsDisposed)
					{
						return;
					}

					ng.Send();

					if (ng.IsDisposed)
					{
						return;
					}

					if (afterSend != null)
					{
						afterSend(ng);
					}

					return;
				}
			}

			html = html.StripHtmlBreaks(true);
			html = html.Replace("\n", "  ");
			html = html.StripHtml(false);
			html = html.StripTabs();
			html = html.StripCRLF();

			m.SendMessage(html);
		}
	}
}