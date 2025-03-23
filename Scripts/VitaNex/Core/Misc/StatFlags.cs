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

using Server;
#endregion

namespace VitaNex
{
	[Flags]
	public enum StatFlags : byte
	{
		None = 0x0,

		Str = 0x1,
		Dex = 0x2,
		Int = 0x4,

		Hits = 0x8,
		Stam = 0x10,
		Mana = 0x20,

		All = Byte.MaxValue
	}

	public static class StatsFlagsExtension
	{
		public static bool TryConvert(this StatFlags flags, out StatType stats)
		{
			stats = StatType.All;

			var f = (byte)flags;

			if (f < 0x1 || f > 0x7)
			{
				return false;
			}

			stats = (StatType)flags;
			return true;
		}

		public static string GetName(this StatFlags flags)
		{
			return GetName(flags, ", ");
		}

		public static string GetName(this StatFlags flags, string separator)
		{
			if (flags == StatFlags.None)
			{
				return "None";
			}

			return String.Join(separator, flags.EnumerateValues<StatFlags>(true).Not(s => s == StatFlags.None));
		}
	}
}