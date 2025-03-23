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

namespace VitaNex.Schedules
{
	[Flags]
	public enum ScheduleDays : short
	{
		None = 0x000,
		Sunday = 0x001,
		Monday = 0x002,
		Tuesday = 0x004,
		Wednesday = 0x008,
		Thursday = 0x010,
		Friday = 0x020,
		Saturday = 0x040,

		All = Int16.MaxValue
	}
}