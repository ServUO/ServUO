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

using Server;
using Server.Gumps;

using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class PvPScenariosUI : ListGump<PvPScenario>
	{
		public bool UseConfirmDialog { get; set; }

		public PvPScenariosUI(Mobile user, Gump parent = null, bool useConfirm = true)
			: base(user, parent, emptyText: "There are no scenarios to display.", title: "PvP Scenarios")
		{
			UseConfirmDialog = useConfirm;

			ForceRecompile = true;

			Width = 600;

			Columns = 3;
			EntriesPerPage = 12;
		}

		public override string GetSearchKeyFor(PvPScenario key)
		{
			return key != null ? key.Name : base.GetSearchKeyFor(null);
		}

		protected override string GetLabelText(int index, int pageIndex, PvPScenario entry)
		{
			return entry != null
				? String.Format("{0} ({1})", entry.Name, entry.TypeOf.Name)
				: base.GetLabelText(index, pageIndex, null);
		}

		protected override void CompileList(List<PvPScenario> list)
		{
			list.Clear();
			list.AddRange(AutoPvP.Scenarios);

			base.CompileList(list);
		}

		protected override void SelectEntry(GumpButton button, PvPScenario entry)
		{
			base.SelectEntry(button, entry);

			if (button != null && entry != null)
			{
				new PvPScenarioUI(User, entry, Hide(true), UseConfirmDialog).Send();
			}
		}
	}
}