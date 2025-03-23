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
	public enum ScheduleMonths : short
	{
		None = 0x000,
		January = 0x001,
		February = 0x002,
		March = 0x004,
		April = 0x008,
		May = 0x010,
		June = 0x020,
		July = 0x040,
		August = 0x080,
		September = 0x100,
		October = 0x200,
		November = 0x400,
		December = 0x800,

		All = ~None
	}
}