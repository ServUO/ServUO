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

namespace Server
{
	public struct FilterGumpEntry
	{
		public FilterOption Option { get; private set; }
		public bool IsCategory { get; private set; }
		public int Col { get; private set; }
		public int Row { get; private set; }

		public FilterGumpEntry(FilterOption option, bool isCategory, int col, int row)
			: this()
		{
			Option = option;
			IsCategory = isCategory;
			Col = col;
			Row = row;
		}
	}
}