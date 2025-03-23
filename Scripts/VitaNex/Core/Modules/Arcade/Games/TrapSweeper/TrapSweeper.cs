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
#endregion

namespace VitaNex.Modules.Games
{
	public sealed class TrapSweeper : Game<TrapSweeperEngine>
	{
		private readonly IconDefinition _Icon = IconDefinition.FromGump(24013, 1258);

		public override IconDefinition Icon => _Icon;

		public override string Name => "Trap Sweeper";

		public override string Desc => "Sweep the castle for traps!";

		public override string Help => _Help;

		private static readonly string _Help = String.Concat(
			"<BIG>Rules & Basics</BIG>",
			"<BR>",
			"<BR><BIG>Objective</BIG>",
			"<BR>Find the empty tiles while avoiding the traps.",
			"<BR>The faster you clear the floor, the better your score.",
			"<BR>",
			"<BR><BIG>Floors</BIG>",
			"<BR>There are three floors to choose from, each more difficult than the last.",
			"<BR>",
			"<BR><B>* Rookie:</B> Easy mode, small floor.",
			"<BR><B>* Guard:</B> Normal mode, medium floor.",
			"<BR><B>* Knight:</B> Expert mode, large floor.",
			"<BR><B>* Random:</B> Generates a random floor.",
			"<BR>",
			"<BR><BIG>Rules</BIG>",
			"<BR>Reveal a trap, you die.",
			"<BR>Reveal an empty tile, you continue sweeping.",
			"<BR>Reveal a bonus tile, you continue and collect a reward when you win.",
			"<BR>Reveal a number tile, you continue and it tells you how many traps lay hidden in the eight surrounding tiles.",
			"<BR><I>These numbers will help you decide which tiles are safe to reveal.</I>",
			"<BR>",
			"<BR><BIG>Tips</BIG>",
			"<BR>Mark the traps!",
			"<BR>If you think a tile hides a trap, select Mark and click it, this will highlight the tile.",
			"<BR>You can unhighlight a tile by selecting Mark and clicking it again.");
	}
}