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
	public class PvPProfilesSortUI : MenuGump
	{
		public PvPProfilesUI ListGump { get; set; }

		public PvPProfilesSortUI(Mobile user, PvPProfilesUI listGump, Gump parent = null, GumpButton clicked = null)
			: base(user, parent, clicked: clicked)
		{
			ListGump = listGump;
		}

		protected override void CompileOptions(MenuGumpOptions list)
		{
			if (ListGump != null)
			{
				if (ListGump.SortOrder != PvPProfileRankOrder.None)
				{
					list.AppendEntry(
						"No Sorting",
						button =>
						{
							ListGump.SortOrder = PvPProfileRankOrder.None;
							ListGump.Refresh(true);
						});
				}

				if (ListGump.SortOrder != PvPProfileRankOrder.Kills)
				{
					list.AppendEntry(
						"Kills",
						button =>
						{
							ListGump.SortOrder = PvPProfileRankOrder.Kills;
							ListGump.Refresh(true);
						});
				}

				if (ListGump.SortOrder != PvPProfileRankOrder.Points)
				{
					list.AppendEntry(
						"Points",
						button =>
						{
							ListGump.SortOrder = PvPProfileRankOrder.Points;
							ListGump.Refresh(true);
						});
				}

				if (ListGump.SortOrder != PvPProfileRankOrder.Wins)
				{
					list.AppendEntry(
						"Wins",
						button =>
						{
							ListGump.SortOrder = PvPProfileRankOrder.Wins;
							ListGump.Refresh(true);
						});
				}
			}

			base.CompileOptions(list);
		}
	}
}