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
using Server;
using Server.Gumps;

using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Schedules
{
	public class ScheduleMonthsMenuGump : MenuGump
	{
		public Schedule Schedule { get; set; }

		public bool UseConfirmDialog { get; set; }

		public ScheduleMonthsMenuGump(
			Mobile user,
			Schedule schedule,
			Gump parent = null,
			GumpButton clicked = null,
			bool useConfirm = true)
			: base(user, parent, clicked: clicked)
		{
			Schedule = schedule;
			UseConfirmDialog = useConfirm;

			CanMove = false;
			CanResize = false;
		}

		protected override void CompileOptions(MenuGumpOptions list)
		{
			list.AppendEntry("None", b => SetMonth(b, ScheduleMonths.None));
			list.AppendEntry("All", b => SetMonth(b, ScheduleMonths.All));
			list.AppendEntry("January", b => SetMonth(b, ScheduleMonths.January));
			list.AppendEntry("February", b => SetMonth(b, ScheduleMonths.February));
			list.AppendEntry("March", b => SetMonth(b, ScheduleMonths.March));
			list.AppendEntry("April", b => SetMonth(b, ScheduleMonths.April));
			list.AppendEntry("May", b => SetMonth(b, ScheduleMonths.May));
			list.AppendEntry("June", b => SetMonth(b, ScheduleMonths.June));
			list.AppendEntry("July", b => SetMonth(b, ScheduleMonths.July));
			list.AppendEntry("August", b => SetMonth(b, ScheduleMonths.August));
			list.AppendEntry("September", b => SetMonth(b, ScheduleMonths.September));
			list.AppendEntry("October", b => SetMonth(b, ScheduleMonths.October));
			list.AppendEntry("November", b => SetMonth(b, ScheduleMonths.November));
			list.AppendEntry("December", b => SetMonth(b, ScheduleMonths.December));

			base.CompileOptions(list);

			list.Replace("Cancel", "Done", Cancel);
		}

		protected override int GetLabelHue(int index, int pageIndex, ListGumpEntry entry)
		{
			if (Schedule == null)
			{
				return ErrorHue;
			}

			switch (entry.Label)
			{
				case "January":
					return Schedule.Info.HasMonth(ScheduleMonths.January) ? HighlightHue : ErrorHue;
				case "February":
					return Schedule.Info.HasMonth(ScheduleMonths.February) ? HighlightHue : ErrorHue;
				case "March":
					return Schedule.Info.HasMonth(ScheduleMonths.March) ? HighlightHue : ErrorHue;
				case "April":
					return Schedule.Info.HasMonth(ScheduleMonths.April) ? HighlightHue : ErrorHue;
				case "May":
					return Schedule.Info.HasMonth(ScheduleMonths.May) ? HighlightHue : ErrorHue;
				case "June":
					return Schedule.Info.HasMonth(ScheduleMonths.June) ? HighlightHue : ErrorHue;
				case "July":
					return Schedule.Info.HasMonth(ScheduleMonths.July) ? HighlightHue : ErrorHue;
				case "August":
					return Schedule.Info.HasMonth(ScheduleMonths.August) ? HighlightHue : ErrorHue;
				case "September":
					return Schedule.Info.HasMonth(ScheduleMonths.September) ? HighlightHue : ErrorHue;
				case "October":
					return Schedule.Info.HasMonth(ScheduleMonths.October) ? HighlightHue : ErrorHue;
				case "November":
					return Schedule.Info.HasMonth(ScheduleMonths.November) ? HighlightHue : ErrorHue;
				case "December":
					return Schedule.Info.HasMonth(ScheduleMonths.December) ? HighlightHue : ErrorHue;
			}

			return base.GetLabelHue(index, pageIndex, entry);
		}

		protected virtual void SetMonth(GumpButton button, ScheduleMonths month)
		{
			if (Schedule == null)
			{
				Close();
				return;
			}

			switch (month)
			{
				case ScheduleMonths.None:
					Schedule.Info.Months = ScheduleMonths.None;
					break;
				case ScheduleMonths.All:
					Schedule.Info.Months = ScheduleMonths.All;
					break;
				default:
					Schedule.Info.Months ^= month;
					break;
			}

			Schedule.InvalidateNextTick();

			Refresh(true);
		}
	}
}