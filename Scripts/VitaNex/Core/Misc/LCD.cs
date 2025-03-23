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
using System.Globalization;
using System.Linq;
#endregion

namespace VitaNex
{
	[Flags]
	public enum LCDLines : byte
	{
		None = 0x00,
		Top = 0x01,
		TopLeft = 0x02,
		TopRight = 0x04,
		Middle = 0x08,
		BottomLeft = 0x10,
		BottomRight = 0x20,
		Bottom = 0x40,

		Number0 = Top | TopLeft | TopRight | BottomLeft | BottomRight | Bottom,
		Number1 = TopLeft | BottomLeft,
		Number2 = Top | TopRight | Middle | BottomLeft | Bottom,
		Number3 = Top | TopRight | Middle | BottomRight | Bottom,
		Number4 = TopLeft | TopRight | Middle | BottomRight,
		Number5 = Top | TopLeft | Middle | BottomRight | Bottom,
		Number6 = Top | TopLeft | Middle | BottomLeft | BottomRight | Bottom,
		Number7 = Top | TopRight | BottomRight,
		Number8 = Top | TopLeft | TopRight | Middle | BottomLeft | BottomRight | Bottom,
		Number9 = Top | TopLeft | TopRight | Middle | BottomRight
	}

	public static class LCD
	{
		private static readonly LCDLines[] _NumericMatrix = LCDLines.None.EnumerateValues<LCDLines>(false).Skip(7).ToArray();

		public static LCDLines[] NumericMatrix => _NumericMatrix;

		public static bool TryParse(int val, out LCDLines[] matrix)
		{
			var s = val.ToString(CultureInfo.InvariantCulture);
			matrix = new LCDLines[s.Length];

			var success = false;

			for (var i = 0; i < s.Length; i++)
			{
				success = Int32.TryParse(s[i].ToString(CultureInfo.InvariantCulture), out val) && TryParse(val, out matrix[i]);

				if (success)
				{
					continue;
				}

				matrix = new LCDLines[0];
				break;
			}

			return success;
		}

		public static bool TryParse(int val, out LCDLines matrix)
		{
			if (val < 0 || val > 9)
			{
				matrix = LCDLines.None;
				return false;
			}

			matrix = _NumericMatrix[val];
			return true;
		}

		public static bool HasLines(int val, LCDLines lines)
		{
			if (val < 0 || val > 9)
			{
				return false;
			}

			if (TryParse(val, out LCDLines matrix))
			{
				return matrix.HasFlag(lines);
			}

			return false;
		}
	}
}