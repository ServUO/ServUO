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

namespace System
{
	public static class NumericExtUtility
	{
		private static string GetOrdinalSuffix(double value)
		{
			var ones = (int)(value % 10);
			var tens = (int)Math.Floor(value / 10.0) % 10;

			string suff;

			if (tens == 1)
			{
				suff = "th";
			}
			else
			{
				switch (ones)
				{
					case 1:
						suff = "st";
						break;
					case 2:
						suff = "nd";
						break;
					case 3:
						suff = "rd";
						break;
					default:
						suff = "th";
						break;
				}
			}

			return suff;
		}

		public static string ToOrdinalString(this decimal value, string format = "#,0")
		{
			return value.ToString(format) + GetOrdinalSuffix((double)value);
		}

		public static string ToOrdinalString(this double value, string format = "#,0")
		{
			return value.ToString(format) + GetOrdinalSuffix(value);
		}

		public static string ToOrdinalString(this float value, string format = "#,0")
		{
			return value.ToString(format) + GetOrdinalSuffix(value);
		}

		public static string ToOrdinalString(this sbyte value, string format = "#,0")
		{
			return value.ToString(format) + GetOrdinalSuffix(value);
		}

		public static string ToOrdinalString(this byte value, string format = "#,0")
		{
			return value.ToString(format) + GetOrdinalSuffix(value);
		}

		public static string ToOrdinalString(this short value, string format = "#,0")
		{
			return value.ToString(format) + GetOrdinalSuffix(value);
		}

		public static string ToOrdinalString(this ushort value, string format = "#,0")
		{
			return value.ToString(format) + GetOrdinalSuffix(value);
		}

		public static string ToOrdinalString(this int value, string format = "#,0")
		{
			return value.ToString(format) + GetOrdinalSuffix(value);
		}

		public static string ToOrdinalString(this uint value, string format = "#,0")
		{
			return value.ToString(format) + GetOrdinalSuffix(value);
		}

		public static string ToOrdinalString(this long value, string format = "#,0")
		{
			return value.ToString(format) + GetOrdinalSuffix(value);
		}

		public static string ToOrdinalString(this ulong value, string format = "#,0")
		{
			return value.ToString(format) + GetOrdinalSuffix(value);
		}

		public static decimal Overflow(this decimal value, decimal min, decimal max)
		{
			if (min > max)
			{
				var swapMin = Math.Min(min, max);
				var swapMax = Math.Max(min, max);

				min = swapMin;
				max = swapMax;
			}

			if (value < min)
			{
				while (value < min)
				{
					value = max - (min - value);
				}
			}
			else if (value > max)
			{
				while (value > max)
				{
					value = min + (value - max);
				}
			}

			return value;
		}

		public static double Overflow(this double value, double min, double max)
		{
			if (min > max)
			{
				var swapMin = Math.Min(min, max);
				var swapMax = Math.Max(min, max);

				min = swapMin;
				max = swapMax;
			}

			if (value < min)
			{
				while (value < min)
				{
					value = max - (min - value);
				}
			}
			else if (value > max)
			{
				while (value > max)
				{
					value = min + (value - max);
				}
			}

			return value;
		}

		public static float Overflow(this float value, float min, float max)
		{
			if (min > max)
			{
				var swapMin = Math.Min(min, max);
				var swapMax = Math.Max(min, max);

				min = swapMin;
				max = swapMax;
			}

			if (value < min)
			{
				while (value < min)
				{
					value = max - (min - value);
				}
			}
			else if (value > max)
			{
				while (value > max)
				{
					value = min + (value - max);
				}
			}

			return value;
		}

		public static sbyte Overflow(this sbyte value, sbyte min, sbyte max)
		{
			if (min > max)
			{
				var swapMin = Math.Min(min, max);
				var swapMax = Math.Max(min, max);

				min = swapMin;
				max = swapMax;
			}

			if (value < min)
			{
				while (value < min)
				{
					value = (sbyte)(max - (min - value));
				}
			}
			else if (value > max)
			{
				while (value > max)
				{
					value = (sbyte)(min + (value - max));
				}
			}

			return value;
		}

		public static byte Overflow(this byte value, byte min, byte max)
		{
			if (min > max)
			{
				var swapMin = Math.Min(min, max);
				var swapMax = Math.Max(min, max);

				min = swapMin;
				max = swapMax;
			}

			if (value < min)
			{
				while (value < min)
				{
					value = (byte)(max - (min - value));
				}
			}
			else if (value > max)
			{
				while (value > max)
				{
					value = (byte)(min + (value - max));
				}
			}

			return value;
		}

		public static short Overflow(this short value, short min, short max)
		{
			if (min > max)
			{
				var swapMin = Math.Min(min, max);
				var swapMax = Math.Max(min, max);

				min = swapMin;
				max = swapMax;
			}

			if (value < min)
			{
				while (value < min)
				{
					value = (short)(max - (min - value));
				}
			}
			else if (value > max)
			{
				while (value > max)
				{
					value = (short)(min + (value - max));
				}
			}

			return value;
		}

		public static ushort Overflow(this ushort value, ushort min, ushort max)
		{
			if (min > max)
			{
				var swapMin = Math.Min(min, max);
				var swapMax = Math.Max(min, max);

				min = swapMin;
				max = swapMax;
			}

			if (value < min)
			{
				while (value < min)
				{
					value = (ushort)(max - (min - value));
				}
			}
			else if (value > max)
			{
				while (value > max)
				{
					value = (ushort)(min + (value - max));
				}
			}

			return value;
		}

		public static int Overflow(this int value, int min, int max)
		{
			if (min > max)
			{
				var swapMin = Math.Min(min, max);
				var swapMax = Math.Max(min, max);

				min = swapMin;
				max = swapMax;
			}

			if (value < min)
			{
				while (value < min)
				{
					value = max - (min - value);
				}
			}
			else if (value > max)
			{
				while (value > max)
				{
					value = min + (value - max);
				}
			}

			return value;
		}

		public static uint Overflow(this uint value, uint min, uint max)
		{
			if (min > max)
			{
				var swapMin = Math.Min(min, max);
				var swapMax = Math.Max(min, max);

				min = swapMin;
				max = swapMax;
			}

			if (value < min)
			{
				while (value < min)
				{
					value = max - (min - value);
				}
			}
			else if (value > max)
			{
				while (value > max)
				{
					value = min + (value - max);
				}
			}

			return value;
		}

		public static long Overflow(this long value, long min, long max)
		{
			if (min > max)
			{
				var swapMin = Math.Min(min, max);
				var swapMax = Math.Max(min, max);

				min = swapMin;
				max = swapMax;
			}

			if (value < min)
			{
				while (value < min)
				{
					value = max - (min - value);
				}
			}
			else if (value > max)
			{
				while (value > max)
				{
					value = min + (value - max);
				}
			}

			return value;
		}

		public static ulong Overflow(this ulong value, ulong min, ulong max)
		{
			if (min > max)
			{
				var swapMin = Math.Min(min, max);
				var swapMax = Math.Max(min, max);

				min = swapMin;
				max = swapMax;
			}

			if (value < min)
			{
				while (value < min)
				{
					value = max - (min - value);
				}
			}
			else if (value > max)
			{
				while (value > max)
				{
					value = min + (value - max);
				}
			}

			return value;
		}
	}
}