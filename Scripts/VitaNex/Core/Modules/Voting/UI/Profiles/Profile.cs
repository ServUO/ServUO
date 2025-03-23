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
using System.Text;

using Server;
using Server.Gumps;
using Server.Mobiles;

using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Modules.Voting
{
	public class VoteProfileGump : HtmlPanelGump<VoteProfile>
	{
		public VoteProfileGump(Mobile user, VoteProfile profile, Gump parent = null, bool useConfirm = true)
			: base(user, parent, emptyText: "No profile selected.", title: "Vote Profile", selected: profile)
		{
			UseConfirmDialog = useConfirm;
			HtmlColor = Color.SkyBlue;
		}

		public bool UseConfirmDialog { get; set; }

		protected override void Compile()
		{
			base.Compile();

			if (Selected == null || Selected.Deleted)
			{
				Selected = Voting.EnsureProfile(User as PlayerMobile, true);
			}

			Html = String.Format("<basefont color=#{0:X6}>", HtmlColor.ToRgb());

			if (Selected != null)
			{
				Html += Selected.ToHtmlString(User);
			}
		}

		protected override void CompileMenuOptions(MenuGumpOptions list)
		{
			if (Selected == null || Selected.Deleted)
			{
				Selected = Voting.EnsureProfile(User as PlayerMobile, true);
			}

			if (Selected == null)
			{
				base.CompileMenuOptions(list);
				return;
			}

			if (User.AccessLevel >= Voting.Access)
			{
				list.AppendEntry(new ListGumpEntry("Clear History", OnClearHistory, HighlightHue));
				list.AppendEntry(new ListGumpEntry("Delete Profile", OnDeleteProfile, HighlightHue));
			}

			list.AppendEntry(new ListGumpEntry("View History", OnViewHistory, HighlightHue));

			list.AppendEntry(new ListGumpEntry("Help", ShowHelp));

			base.CompileMenuOptions(list);
		}

		private void OnViewHistory(GumpButton button)
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

			Send(new VoteProfileHistoryGump(User, Selected, Hide(true), UseConfirmDialog, DateTime.UtcNow));
		}

		protected virtual void OnClearHistory(GumpButton button)
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
						Html =
							"All data associated with the profile history will be lost.\nThis action can not be reversed!\nDo you want to continue?",
						AcceptHandler = OnConfirmClearHistory
					});
			}
			else
			{
				Selected.ClearHistory();
				Refresh(true);
			}
		}

		protected virtual void OnDeleteProfile(GumpButton button)
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
						Html =
							"All data associated with this profile will be deleted.\nThis action can not be reversed!\nDo you want to continue?",
						AcceptHandler = ConfirmDeleteProfile
					});
			}
			else
			{
				Selected.Delete();
				Close();
			}
		}

		protected virtual void OnConfirmClearHistory(GumpButton button)
		{
			if (Selected != null && !Selected.Deleted)
			{
				Selected.ClearHistory();
			}

			Refresh(true);
		}

		protected virtual void ConfirmDeleteProfile(GumpButton button)
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