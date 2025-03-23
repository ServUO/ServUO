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
using System.Drawing;
using System.Linq;
using System.Text;

using Server;
using Server.Gumps;
using Server.Mobiles;

using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Modules.Voting
{
	public sealed class VoteProfileHistoryGump : HtmlPanelGump<VoteProfile>
	{
		public DateTime? HistoryDate { get; set; }
		public bool UseConfirmDialog { get; set; }
		public bool TechnicalView { get; set; }

		public VoteProfileHistoryGump(
			Mobile user,
			VoteProfile profile,
			Gump parent = null,
			bool useConfirm = true,
			DateTime? when = null)
			: base(user, parent, emptyText: "No profile selected.", title: "Vote Profile History", selected: profile)
		{
			HtmlColor = Color.SkyBlue;
			UseConfirmDialog = useConfirm;
			HistoryDate = when;
		}

		protected override void Compile()
		{
			base.Compile();

			if (Selected == null)
			{
				return;
			}

			Html = String.Format("<basefont color=#{0:X6}>", HtmlColor.ToRgb());

			if (HistoryDate != null && HistoryDate.Value <= DateTime.UtcNow)
			{
				Html += String.Format(
					"Viewing History for {0} on {1}\n\n",
					Selected.Owner.RawName,
					HistoryDate.Value.ToSimpleString(Voting.CMOptions.DateFormat));

				Html += "<big>Showing up to 500 entries...</big>\n\n";

				Html += String.Join("\n", Selected.GetHistory(HistoryDate.Value, 500).Select(e => e.ToHtmlString(TechnicalView)));
			}
			else
			{
				Html += String.Format("Viewing Recent History for {0}\n\n", Selected.Owner.RawName);

				Html += "<big>Showing up to 500 entries...</big>\n\n";

				Html += String.Join("\n", Selected.GetHistory(500).Select(e => e.ToHtmlString(TechnicalView)));
			}
		}

		protected override void CompileMenuOptions(MenuGumpOptions list)
		{
			if (Selected == null)
			{
				return;
			}

			if (User.AccessLevel >= Voting.Access)
			{
				list.AppendEntry(new ListGumpEntry("Clear History", ClearHistory, HighlightHue));
				list.AppendEntry(new ListGumpEntry("Delete Profile", DeleteProfile, HighlightHue));

				if (TechnicalView)
				{
					list.Replace(
						"Technical View",
						new ListGumpEntry(
							"Standard View",
							b =>
							{
								TechnicalView = false;
								Refresh(true);
							},
							HighlightHue));
				}
				else
				{
					list.Replace(
						"Standard View",
						new ListGumpEntry(
							"Technical View",
							b =>
							{
								TechnicalView = true;
								Refresh(true);
							},
							HighlightHue));
				}
			}

			list.AppendEntry(
				new ListGumpEntry(
					"Show Recent",
					b =>
					{
						HistoryDate = null;
						Refresh(true);
					}));

			list.AppendEntry(
				new ListGumpEntry(
					"Show Today",
					b =>
					{
						HistoryDate = DateTime.UtcNow;
						Refresh(true);
					}));

			list.AppendEntry(
				new ListGumpEntry(
					"Select Date",
					b => Send(
						new InputDateDialogGump(User, Refresh())
						{
							Title = "Select Date",
							Html = "Type the date you wish to view history for.",
							InputDate = HistoryDate,
							MaxDate = DateTime.UtcNow,
							CallbackDate = (b1, d) =>
							{
								if (d.HasValue)
								{
									HistoryDate = d.Value;
								}
								else
								{
									User.SendMessage(0x22, "No valid date was given.");
								}

								Refresh(true);
							}
						})));

			list.AppendEntry(new ListGumpEntry("Help", ShowHelp));

			base.CompileMenuOptions(list);
		}

		private void ClearHistory(GumpButton button)
		{
			if (Selected == null || Selected.Deleted)
			{
				Selected = Voting.EnsureProfile(User as PlayerMobile);
			}

			if (Selected == null)
			{
				Refresh(true);
				return;
			}

			if (UseConfirmDialog)
			{
				Send(
					new ConfirmDialogGump(User, Refresh())
					{
						Title = "Clear Profile History?",
						Html = "All data associated with the profile history will be lost.\n" +
							   "This action can not be reversed!\nDo you want to continue?",
						AcceptHandler = ConfirmClearHistory
					});
			}
			else
			{
				Selected.ClearHistory();
				Refresh(true);
			}
		}

		private void DeleteProfile(GumpButton button)
		{
			if (Selected == null || Selected.Deleted)
			{
				Selected = Voting.EnsureProfile(User as PlayerMobile);
			}

			if (Selected == null)
			{
				Refresh(true);
				return;
			}

			if (UseConfirmDialog)
			{
				Send(
					new ConfirmDialogGump(User, Refresh())
					{
						Title = "Delete Profile?",
						Html = "All data associated with this profile will be deleted.\n" +
							   "This action can not be reversed!\nDo you want to continue?",
						AcceptHandler = ConfirmDeleteProfile
					});
			}
			else
			{
				Selected.Delete();
				Close();
			}
		}

		private void ConfirmClearHistory(GumpButton button)
		{
			if (Selected != null && !Selected.Deleted)
			{
				Selected.ClearHistory();
			}

			Refresh(true);
		}

		private void ConfirmDeleteProfile(GumpButton button)
		{
			if (Selected != null && !Selected.Deleted)
			{
				Selected.Delete();
			}

			Close();
		}

		private void ShowHelp(GumpButton button)
		{
			if (User == null || User.Deleted)
			{
				return;
			}

			var sb = VoteGumpUtility.GetHelpText(User);
			Send(
				new HtmlPanelGump<StringBuilder>(User, Hide(true))
				{
					Selected = sb,
					Html = sb.ToString(),
					Title = "Voting Help",
					HtmlColor = Color.SkyBlue
				});
		}
	}
}