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
using System.Text;

using VitaNex;
using VitaNex.Collections;
#endregion

namespace System
{
	[Flags]
	public enum Months
	{
		None = 0x000,
		January = 0x001,
		Febuary = 0x002,
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

	public enum TimeUnit
	{
		Years,
		Months,
		Weeks,
		Days,
		Hours,
		Minutes,
		Seconds,
		Milliseconds
	}

	public static class ChronExtUtility
	{
		public static bool InRange(this TimeSpan now, TimeSpan start, TimeSpan end)
		{
			if (start <= end)
			{
				return now >= start && now <= end;
			}

			return now >= start || now <= end;
		}

		public static bool InRange(this DateTime now, DateTime start, DateTime end)
		{
			if (now.Year < end.Year)
			{
				return now >= start;
			}

			if (now.Year > start.Year)
			{
				return now <= end;
			}

			return now >= start && now <= end;
		}

		public static double GetTotal(this TimeSpan time, TimeUnit unit)
		{
			var total = (double)time.Ticks;

			switch (unit)
			{
				case TimeUnit.Years:
					total = time.TotalDays / 365.2422;
					break;
				case TimeUnit.Months:
					total = time.TotalDays / 30.43685;
					break;
				case TimeUnit.Weeks:
					total = time.TotalDays / 7.0;
					break;
				case TimeUnit.Days:
					total = time.TotalDays;
					break;
				case TimeUnit.Hours:
					total = time.TotalHours;
					break;
				case TimeUnit.Minutes:
					total = time.TotalMinutes;
					break;
				case TimeUnit.Seconds:
					total = time.TotalSeconds;
					break;
				case TimeUnit.Milliseconds:
					total = time.TotalMilliseconds;
					break;
			}

			return total;
		}

		public static DateTime Interpolate(this DateTime start, DateTime end, double percent)
		{
			return new DateTime((long)(start.Ticks + ((end.Ticks - start.Ticks) * percent)));
		}

		public static TimeSpan Interpolate(this TimeSpan start, TimeSpan end, double percent)
		{
			return new TimeSpan((long)(start.Ticks + ((end.Ticks - start.Ticks) * percent)));
		}

		public static TimeStamp Interpolate(this TimeStamp start, TimeStamp end, double percent)
		{
			return new TimeStamp((long)(start.Ticks + ((end.Ticks - start.Ticks) * percent)));
		}

		public static TimeStamp ToTimeStamp(this DateTime date)
		{
			return new TimeStamp(date);
		}

		public static Months GetMonth(this DateTime date)
		{
			switch (date.Month)
			{
				case 1:
					return Months.January;
				case 2:
					return Months.Febuary;
				case 3:
					return Months.March;
				case 4:
					return Months.April;
				case 5:
					return Months.May;
				case 6:
					return Months.June;
				case 7:
					return Months.July;
				case 8:
					return Months.August;
				case 9:
					return Months.September;
				case 10:
					return Months.October;
				case 11:
					return Months.November;
				case 12:
					return Months.December;
				default:
					return Months.None;
			}
		}

		public static string ToSimpleString(this TimeZoneInfo tzo, bool dst)
		{
			var build = ObjectPool<StringBuilder>.AcquireObject();

			if (tzo.Id == "UTC")
			{
				return tzo.Id;
			}

			string value;

			if (dst)
			{
				value = tzo.DaylightName.Replace("Daylight Time", String.Empty);
			}
			else
			{
				value = tzo.StandardName.Replace("Standard Time", String.Empty);
			}

			foreach (var c in value)
			{
				if (!Char.IsWhiteSpace(c) && Char.IsLetter(c) && Char.IsUpper(c))
				{
					build.Append(c);
				}
			}

			build.Append(dst ? "-DT" : "-ST");

			value = build.ToString();

			ObjectPool.Free(ref build);

			return value;
		}

		public static string ToSimpleString(this DateTime date, string format = "t D d M y")
		{
			var build = ObjectPool<StringBuilder>.AcquireObject();

			build.EnsureCapacity(format.Length * 2);

			var noformat = false;

			for (var i = 0; i < format.Length; i++)
			{
				if (format[i] == '#')
				{
					noformat = !noformat;
					continue;
				}

				if (noformat)
				{
					build.Append(format[i]);
					continue;
				}

				switch (format[i])
				{
					case '\\':
						build.Append((i + 1 < format.Length) ? Convert.ToString(format[++i]) : String.Empty);
						break;
					case 'x':
					case 'z':
					{
						var utc = date.Kind == DateTimeKind.Utc;
						var tzo = utc ? TimeZoneInfo.Utc : TimeZoneInfo.Local;

						build.Append(ToSimpleString(tzo, false));
					}
					break;
					case 'X':
					case 'Z':
					{
						var utc = date.Kind == DateTimeKind.Utc;
						var tzo = utc ? TimeZoneInfo.Utc : TimeZoneInfo.Local;

						build.Append(ToSimpleString(tzo, date.IsDaylightSavingTime()));
					}
					break;
					case 'D':
						build.Append(date.DayOfWeek);
						break;
					case 'd':
						build.Append(date.Day);
						break;
					case 'M':
						build.Append(GetMonth(date));
						break;
					case 'm':
						build.Append(date.Month);
						break;
					case 'y':
						build.Append(date.Year);
						break;
					case 't':
					{
						var tf = String.Empty;

						if (i + 1 < format.Length)
						{
							if (format[i + 1] == '@')
							{
								++i;

								while (++i < format.Length && format[i] != '@')
								{
									tf += format[i];
								}
							}
						}

						build.Append(ToSimpleString(date.TimeOfDay, !String.IsNullOrWhiteSpace(tf) ? tf : "h-m-s"));
					}
					break;
					default:
						build.Append(format[i]);
						break;
				}
			}

			var value = build.ToString();

			ObjectPool.Free(ref build);

			return value;
		}

		public static string ToSimpleString(this TimeSpan time, string format = "h-m-s")
		{
			var build = ObjectPool<StringBuilder>.AcquireObject();

			build.EnsureCapacity(format.Length * 2);

			var noformat = false;
			var nopadding = false;
			var zeroValue = false;
			var zeroEmpty = 0;

			string fFormat, dFormat;
			object append;

			for (var i = 0; i < format.Length; i++)
			{
				if (format[i] == '#')
				{
					noformat = !noformat;
					continue;
				}

				if (noformat)
				{
					build.Append(format[i]);
					continue;
				}

				switch (format[i])
				{
					case '!':
						nopadding = !nopadding;
						continue;
				}

				fFormat = zeroEmpty > 0 ? (nopadding ? "{0:#.#}" : "{0:#.##}") : (nopadding ? "{0:0.#}" : "{0:0.##}");
				dFormat = zeroEmpty > 0 ? (nopadding ? "{0:#}" : "{0:##}") : (nopadding ? "{0:0}" : "{0:00}");

				append = null;

				switch (format[i])
				{
					case '<':
						++zeroEmpty;
						break;
					case '>':
					{
						if (zeroEmpty == 0 || (zeroEmpty > 0 && --zeroEmpty == 0))
						{
							zeroValue = false;
						}
					}
					break;
					case '\\':
					{
						if (i + 1 < format.Length)
						{
							append = format[++i];
						}
					}
					break;
					case 'x':
						append = ToSimpleString(TimeZoneInfo.Utc, false);
						break;
					case 'X':
						append = ToSimpleString(TimeZoneInfo.Utc, DateTime.UtcNow.IsDaylightSavingTime());
						break;
					case 'z':
						append = ToSimpleString(TimeZoneInfo.Local, false);
						break;
					case 'Z':
						append = ToSimpleString(TimeZoneInfo.Local, DateTime.Now.IsDaylightSavingTime());
						break;
					case 'D':
					{
						append = String.Format(fFormat, time.TotalDays);
						zeroValue = String.IsNullOrWhiteSpace((string)append);
					}
					break;
					case 'H':
					{
						append = String.Format(fFormat, time.TotalHours);
						zeroValue = String.IsNullOrWhiteSpace((string)append);
					}
					break;
					case 'M':
					{
						append = String.Format(fFormat, time.TotalMinutes);
						zeroValue = String.IsNullOrWhiteSpace((string)append);
					}
					break;
					case 'S':
					{
						append = String.Format(fFormat, time.TotalSeconds);
						zeroValue = String.IsNullOrWhiteSpace((string)append);
					}
					break;
					case 'd':
					{
						append = String.Format(dFormat, time.Days);
						zeroValue = String.IsNullOrWhiteSpace((string)append);
					}
					break;
					case 'h':
					{
						append = String.Format(dFormat, time.Hours);
						zeroValue = String.IsNullOrWhiteSpace((string)append);
					}
					break;
					case 'm':
					{
						append = String.Format(dFormat, time.Minutes);
						zeroValue = String.IsNullOrWhiteSpace((string)append);
					}
					break;
					case 's':
					{
						append = String.Format(dFormat, time.Seconds);
						zeroValue = String.IsNullOrWhiteSpace((string)append);
					}
					break;
					default:
						append = format[i];
						break;
				}

				if (append != null && (!zeroValue || zeroEmpty <= 0))
				{
					build.Append(append);
				}
			}

			var value = build.ToString();

			ObjectPool.Free(ref build);

			return value;
		}

		public static string ToDirectoryName(this DateTime date, string format = "D d M y")
		{
			return ToSimpleString(date, format);
		}

		public static string ToFileName(this DateTime date, string format = "D d M y")
		{
			return ToSimpleString(date, format);
		}

		public static string ToDirectoryName(this TimeSpan time, string format = "h-m")
		{
			return ToSimpleString(time, format);
		}

		public static string ToFileName(this TimeSpan time, string format = "h-m")
		{
			return ToSimpleString(time, format);
		}
	}
}