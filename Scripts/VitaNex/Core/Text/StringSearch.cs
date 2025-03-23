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

namespace VitaNex.Text
{
	[Flags]
	public enum StringSearchFlags
	{
		None = 0x0,
		Equals = 0x1,
		Contains = 0x2,
		StartsWith = 0x4,
		EndsWith = 0x8
	}

	public static class StringSearch
	{
		public static bool Execute(this StringSearchFlags flags, string haystack, string needle, bool ignoreCase)
		{
			if (flags == StringSearchFlags.None || haystack == null || needle == null)
			{
				return false;
			}

			if (flags.HasFlag(StringSearchFlags.Equals) &&
				(ignoreCase ? Insensitive.Equals(haystack, needle) : haystack.Equals(needle)))
			{
				return true;
			}

			if (flags.HasFlag(StringSearchFlags.Contains) &&
				(ignoreCase ? Insensitive.Contains(haystack, needle) : haystack.Contains(needle)))
			{
				return true;
			}

			if (flags.HasFlag(StringSearchFlags.StartsWith) &&
				(ignoreCase ? Insensitive.StartsWith(haystack, needle) : haystack.StartsWith(needle)))
			{
				return true;
			}

			if (flags.HasFlag(StringSearchFlags.EndsWith) &&
				(ignoreCase ? Insensitive.EndsWith(haystack, needle) : haystack.EndsWith(needle)))
			{
				return true;
			}

			return false;
		}
	}
}