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
using System.Collections.Generic;

using Server;
#endregion

namespace VitaNex.Schedules
{
	public static partial class Schedules
	{
		public const AccessLevel Access = AccessLevel.Administrator;

		public static List<Schedule> Registry { get; private set; }

		public static Schedule CreateSchedule(
			string name,
			bool enabled = true,
			bool register = true,
			ScheduleMonths months = ScheduleMonths.None,
			ScheduleDays days = ScheduleDays.None,
			ScheduleTimes times = null,
			Action<Schedule> handler = null)
		{
			return CreateSchedule<Schedule>(name, enabled, register, new ScheduleInfo(months, days, times), handler);
		}

		public static Schedule CreateSchedule(
			string name,
			bool enabled = true,
			bool register = true,
			ScheduleInfo info = null,
			Action<Schedule> handler = null)
		{
			return CreateSchedule<Schedule>(name, enabled, register, info, handler);
		}

		public static TSchedule CreateSchedule<TSchedule>(
			string name,
			bool enabled = true,
			bool register = true,
			ScheduleMonths months = ScheduleMonths.None,
			ScheduleDays days = ScheduleDays.None,
			ScheduleTimes times = null,
			Action<Schedule> handler = null)
			where TSchedule : Schedule
		{
			return CreateSchedule<TSchedule>(name, enabled, register, new ScheduleInfo(months, days, times), handler);
		}

		public static TSchedule CreateSchedule<TSchedule>(
			string name,
			bool enabled = true,
			bool register = true,
			ScheduleInfo info = null,
			Action<Schedule> handler = null)
			where TSchedule : Schedule
		{
			var st = VitaNexCore.TryCatchGet(() => typeof(TSchedule).CreateInstance<TSchedule>(name, enabled, info, handler));

			if (st != null && register)
			{
				st.Register();
			}

			return st;
		}

		public static bool IsRegistered(Schedule schedule)
		{
			return schedule != null && Registry.Contains(schedule);
		}

		public static void Register(Schedule schedule)
		{
			if (schedule != null)
			{
				Registry.Update(schedule);
			}
		}

		public static void Unregister(Schedule schedule)
		{
			if (schedule != null)
			{
				Registry.Remove(schedule);
			}
		}

		public static ScheduleDays ConvertDay(int day)
		{
			switch (day)
			{
				case 0:
					return ScheduleDays.None;
				case 1:
					return ScheduleDays.Sunday;
				case 2:
					return ScheduleDays.Monday;
				case 3:
					return ScheduleDays.Tuesday;
				case 4:
					return ScheduleDays.Wednesday;
				case 5:
					return ScheduleDays.Thursday;
				case 6:
					return ScheduleDays.Friday;
				case 7:
					return ScheduleDays.Saturday;
			}

			return ScheduleDays.All;
		}

		public static int ConvertDay(ScheduleDays day)
		{
			switch (day)
			{
				case ScheduleDays.None:
					return 0;
				case ScheduleDays.Sunday:
					return 1;
				case ScheduleDays.Monday:
					return 2;
				case ScheduleDays.Tuesday:
					return 3;
				case ScheduleDays.Wednesday:
					return 4;
				case ScheduleDays.Thursday:
					return 5;
				case ScheduleDays.Friday:
					return 6;
				case ScheduleDays.Saturday:
					return 7;
			}

			return 0;
		}

		public static ScheduleDays ConvertDay(DayOfWeek day)
		{
			switch (day)
			{
				case DayOfWeek.Sunday:
					return ScheduleDays.Sunday;
				case DayOfWeek.Monday:
					return ScheduleDays.Monday;
				case DayOfWeek.Tuesday:
					return ScheduleDays.Tuesday;
				case DayOfWeek.Wednesday:
					return ScheduleDays.Wednesday;
				case DayOfWeek.Thursday:
					return ScheduleDays.Thursday;
				case DayOfWeek.Friday:
					return ScheduleDays.Friday;
				case DayOfWeek.Saturday:
					return ScheduleDays.Saturday;
			}

			return ScheduleDays.None;
		}

		public static ScheduleMonths ConvertMonth(int month)
		{
			switch (month)
			{
				case 0:
					return ScheduleMonths.None;
				case 1:
					return ScheduleMonths.January;
				case 2:
					return ScheduleMonths.February;
				case 3:
					return ScheduleMonths.March;
				case 4:
					return ScheduleMonths.April;
				case 5:
					return ScheduleMonths.May;
				case 6:
					return ScheduleMonths.June;
				case 7:
					return ScheduleMonths.July;
				case 8:
					return ScheduleMonths.August;
				case 9:
					return ScheduleMonths.September;
				case 10:
					return ScheduleMonths.October;
				case 11:
					return ScheduleMonths.November;
				case 12:
					return ScheduleMonths.December;
			}

			return ScheduleMonths.All;
		}

		public static int ConvertMonth(ScheduleMonths month)
		{
			switch (month)
			{
				case ScheduleMonths.None:
					return 0;
				case ScheduleMonths.January:
					return 1;
				case ScheduleMonths.February:
					return 2;
				case ScheduleMonths.March:
					return 3;
				case ScheduleMonths.April:
					return 4;
				case ScheduleMonths.May:
					return 5;
				case ScheduleMonths.June:
					return 6;
				case ScheduleMonths.July:
					return 7;
				case ScheduleMonths.August:
					return 8;
				case ScheduleMonths.September:
					return 9;
				case ScheduleMonths.October:
					return 10;
				case ScheduleMonths.November:
					return 11;
				case ScheduleMonths.December:
					return 12;
			}

			return 0;
		}
	}
}