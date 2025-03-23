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
using System.Drawing;

using VitaNex;
#endregion

namespace System
{
	public static class ColorExtUtility
	{
		public static Color ToColor(this KnownColor color)
		{
			return Color.FromKnownColor(color);
		}

		public static Color555 ToColor555(this Color value)
		{
			return value;
		}

		public static ushort ToArgb555(this Color value)
		{
			return ToColor555(value);
		}

		public static int ToRgb(this Color value)
		{
			return value.ToArgb() & 0x00FFFFFF;
		}

		public static Color Interpolate(this Color source, Color target, double percent)
		{
			if (percent <= 0.0)
			{
				return source;
			}

			if (percent >= 1.0)
			{
				return target;
			}

			var r = (int)(source.R + (target.R - source.R) * percent);
			var g = (int)(source.G + (target.G - source.G) * percent);
			var b = (int)(source.B + (target.B - source.B) * percent);

			return Color.FromArgb(255, r, g, b);
		}

		public static Color FixBlackTransparency(this Color source)
		{
			if (source.IsEmpty || source.A <= 0 || source.R >= 0x08 || source.G >= 0x08 || source.B >= 0x08)
			{
				return source;
			}

			var r = source.R;
			var g = source.G;
			var b = source.B;

			if (r != g || r != b)
			{
				var rd = 0x08 - r;
				var gd = 0x08 - g;
				var bd = 0x08 - b;

				if (rd < gd && rd < bd)
				{
					r = 0x08;
				}
				else if (gd < rd && gd < bd)
				{
					g = 0x08;
				}
				else if (bd < rd && bd < gd)
				{
					b = 0x08;
				}
				else
				{
					r = g = b = 0x08;
				}
			}
			else
			{
				r = g = b = 0x08;
			}

			source = Color.FromArgb(source.A, r, g, b);

			return source;
		}
	}
}