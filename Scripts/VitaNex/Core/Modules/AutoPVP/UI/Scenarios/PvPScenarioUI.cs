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
using Server;
using Server.Gumps;

using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class PvPScenarioUI : HtmlPanelGump<PvPScenario>
	{
		public bool UseConfirmDialog { get; set; }

		public PvPScenarioUI(Mobile user, PvPScenario scenario, Gump parent = null, bool useConfirm = true)
			: base(user, parent, emptyText: "No scenario selected.", title: "PvP Scenario Overview", selected: scenario)
		{
			UseConfirmDialog = useConfirm;
		}

		protected override void Compile()
		{
			base.Compile();

			if (Selected != null)
			{
				Html = Selected.ToHtmlString(User);
			}
		}

		protected override void CompileMenuOptions(MenuGumpOptions list)
		{
			if (Selected != null && User.AccessLevel >= AutoPvP.Access)
			{
				list.AppendEntry("Create Battle", OnCreate, HighlightHue);
			}

			base.CompileMenuOptions(list);
		}

		private void OnCreate(GumpButton b)
		{
			if (UseConfirmDialog)
			{
				new ConfirmDialogGump(User, Hide(true))
				{
					Title = "Create New Battle?",
					Html = "This action will create a new battle from the selected scenario.\nDo you want to continue?",
					AcceptHandler = OnConfirmCreateBattle,
					CancelHandler = Refresh
				}.Send();
			}
			else
			{
				OnConfirmCreateBattle(b);
			}
		}

		protected virtual void OnConfirmCreateBattle(GumpButton button)
		{
			if (Selected == null)
			{
				Close();
				return;
			}

			var battle = AutoPvP.CreateBattle(Selected);

			Close(true);

			PvPBattlesUI.DisplayTo(User, battle, false);
		}
	}
}