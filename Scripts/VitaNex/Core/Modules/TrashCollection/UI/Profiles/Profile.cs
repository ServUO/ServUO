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

using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Modules.TrashCollection
{
	public class TrashCollectionProfileGump : HtmlPanelGump<TrashProfile>
	{
		public bool UseConfirmDialog { get; set; }

		public TrashCollectionProfileGump(Mobile user, TrashProfile profile, Gump parent = null, bool useConfirm = true)
			: base(user, parent, emptyText: "No profile selected.", title: "Trash Collection Profile", selected: profile)
		{
			UseConfirmDialog = useConfirm;
			HtmlColor = Color.SkyBlue;
		}

		protected override void Compile()
		{
			base.Compile();

			if (Selected == null || Selected.Deleted)
			{
				Selected = TrashCollection.EnsureProfile(User, true);
			}

			Html = String.Format("<basefont color=#{0:X6}>", HtmlColor.ToRgb());
			Html += Selected.ToHtmlString(User);
		}

		protected override void CompileMenuOptions(MenuGumpOptions list)
		{
			if (Selected == null || Selected.Deleted)
			{
				Selected = TrashCollection.EnsureProfile(User, true);
			}

			if (User.AccessLevel >= TrashCollection.Access)
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
				Selected = TrashCollection.EnsureProfile(User);
			}

			Send(new TrashCollectionProfileHistoryGump(User, Selected, Hide(true), UseConfirmDialog, DateTime.UtcNow));
		}

		protected virtual void OnClearHistory(GumpButton button)
		{
			if (Selected == null || Selected.Deleted)
			{
				Selected = TrashCollection.EnsureProfile(User);
			}

			if (UseConfirmDialog)
			{
				Send(
					new ConfirmDialogGump(User, Refresh())
					{
						Title = "Clear Profile History?",
						Html = "All data associated with the profile history will be lost.\n" +
							   "This action can not be reversed!\nDo you want to continue?",
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
				Selected = TrashCollection.EnsureProfile(User);
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

			var sb = TrashCollectionGumpUtility.GetHelpText(User);
			Send(
				new HtmlPanelGump<StringBuilder>(User, Refresh())
				{
					Selected = sb,
					Html = sb.ToString(),
					Title = "Trash Collection Help",
					HtmlColor = Color.SkyBlue
				});
		}
	}
}