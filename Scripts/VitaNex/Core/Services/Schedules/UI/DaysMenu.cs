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
	public class ScheduleDaysMenuGump : MenuGump
	{
		public Schedule Schedule { get; set; }

		public bool UseConfirmDialog { get; set; }

		public ScheduleDaysMenuGump(
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
			list.AppendEntry(new ListGumpEntry("None", b => SetDay(b, ScheduleDays.None)));
			list.AppendEntry(new ListGumpEntry("All", b => SetDay(b, ScheduleDays.All)));
			list.AppendEntry(new ListGumpEntry("Monday", b => SetDay(b, ScheduleDays.Monday)));
			list.AppendEntry(new ListGumpEntry("Tuesday", b => SetDay(b, ScheduleDays.Tuesday)));
			list.AppendEntry(new ListGumpEntry("Wednesday", b => SetDay(b, ScheduleDays.Wednesday)));
			list.AppendEntry(new ListGumpEntry("Thursday", b => SetDay(b, ScheduleDays.Thursday)));
			list.AppendEntry(new ListGumpEntry("Friday", b => SetDay(b, ScheduleDays.Friday)));
			list.AppendEntry(new ListGumpEntry("Saturday", b => SetDay(b, ScheduleDays.Saturday)));
			list.AppendEntry(new ListGumpEntry("Sunday", b => SetDay(b, ScheduleDays.Sunday)));

			base.CompileOptions(list);

			list.Replace("Cancel", new ListGumpEntry("Done", Cancel));
		}

		protected override int GetLabelHue(int index, int pageIndex, ListGumpEntry entry)
		{
			if (Schedule == null)
			{
				return ErrorHue;
			}

			switch (entry.Label)
			{
				case "Monday":
					return Schedule.Info.HasDay(ScheduleDays.Monday) ? HighlightHue : ErrorHue;
				case "Tuesday":
					return Schedule.Info.HasDay(ScheduleDays.Tuesday) ? HighlightHue : ErrorHue;
				case "Wednesday":
					return Schedule.Info.HasDay(ScheduleDays.Wednesday) ? HighlightHue : ErrorHue;
				case "Thursday":
					return Schedule.Info.HasDay(ScheduleDays.Thursday) ? HighlightHue : ErrorHue;
				case "Friday":
					return Schedule.Info.HasDay(ScheduleDays.Friday) ? HighlightHue : ErrorHue;
				case "Saturday":
					return Schedule.Info.HasDay(ScheduleDays.Saturday) ? HighlightHue : ErrorHue;
				case "Sunday":
					return Schedule.Info.HasDay(ScheduleDays.Sunday) ? HighlightHue : ErrorHue;
			}

			return base.GetLabelHue(index, pageIndex, entry);
		}

		protected virtual void SetDay(GumpButton button, ScheduleDays day)
		{
			if (Schedule == null)
			{
				Close();
				return;
			}

			switch (day)
			{
				case ScheduleDays.None:
					Schedule.Info.Days = ScheduleDays.None;
					break;
				case ScheduleDays.All:
					Schedule.Info.Days = ScheduleDays.All;
					break;
				default:
					Schedule.Info.Days ^= day;
					break;
			}

			Schedule.InvalidateNextTick();

			Refresh(true);
		}
	}
}