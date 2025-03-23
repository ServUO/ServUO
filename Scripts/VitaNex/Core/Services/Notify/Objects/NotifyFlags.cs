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

namespace VitaNex.Notify
{
	[Flags]
	public enum NotifyFlags : ulong
	{
		None = 0x0,

		AutoClose = 0x1,
		Ignore = 0x2,
		TextOnly = 0x4,
		Animate = 0x8
	}
}