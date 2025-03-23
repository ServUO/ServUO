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
using Server.Gumps;

using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Schedules
{
	public class SheduleTimeListGump : ListGump<TimeSpan>
	{
		public static string HelpText = "Schedule Times: List specific times for this schedule.\n" +
										"These times determine what time of day the schedule will tick.";

		public Schedule Schedule { get; set; }

		public bool UseConfirmDialog { get; set; }

		public SheduleTimeListGump(Mobile user, Schedule schedule, Gump parent = null, bool useConfirm = true)
			: base(user, parent, emptyText: "There are no times to display.", title: "Schedule Times")
		{
			Schedule = schedule;
			UseConfirmDialog = useConfirm;

			ForceRecompile = true;
			CanMove = false;
			CanResize = false;

			Columns = 3;
			EntriesPerPage = 24;
		}

		protected override void Compile()
		{
			var i = Title.IndexOf('(');

			if (i > 0)
			{
				Title = Title.Substring(0, i);
			}

			Title += String.Concat(' ', '(', Schedule.Now.ToSimpleString("X"), ')');

			base.Compile();
		}

		protected override string GetLabelText(int index, int pageIndex, TimeSpan entry)
		{
			return entry.ToSimpleString("h:m");
		}

		protected override void CompileMenuOptions(MenuGumpOptions list)
		{
			list.AppendEntry("Delete All", OnDeleteAll, HighlightHue);
			list.AppendEntry("Add Time", OnAddTime, HighlightHue);
			list.AppendEntry("Use Preset", OnPresetsMenu, HighlightHue);
			list.AppendEntry("Help", ShowHelp);

			base.CompileMenuOptions(list);
		}

		protected virtual void ShowHelp(GumpButton button)
		{
			if (User != null && !User.Deleted)
			{
				Send(new NoticeDialogGump(User, this, title: "Help", html: HelpText));
			}
		}

		protected virtual void OnDeleteAll(GumpButton button)
		{
			if (UseConfirmDialog)
			{
				new ConfirmDialogGump(User, this)
				{
					Title = "Delete All Times?",
					Html = "All times in the schedule will be deleted, erasing all data associated with them.\n" +
						   "This action can not be reversed.\n\nDo you want to continue?",
					AcceptHandler = subButton =>
					{
						Schedule.Info.Times.Clear();
						Schedule.InvalidateNextTick();

						Refresh(true);
					},
					CancelHandler = Refresh
				}.Send();
			}
			else
			{
				Schedule.Info.Times.Clear();
				Schedule.InvalidateNextTick();

				Refresh(true);
			}
		}

		protected virtual void OnAddTime(GumpButton button)
		{
			var nowTime = Schedule.Now.TimeOfDay;

			new InputDialogGump(User, this)
			{
				Title = "Add Schedule Time",
				Html = "Enter the time of day " + String.Concat('(', nowTime.ToSimpleString("X"), ')') +
					   " to add to this schedule.\nFormat: HH:MM\nExample: " +
					   String.Format("{0:D2}:{1:D2}", nowTime.Hours, nowTime.Minutes) +
					   "\n\nYou can also load a preset list of times, but be aware that presets will " +
					   "overwrite any custom entries you have created.",
				Callback = (b, text) =>
				{

					ParseTime(text, out var hh, out var mm);

					if (hh == -1 || mm == -1)
					{
						OnAddTime(button);
						return;
					}

					Schedule.Info.Times.Add(new TimeSpan(0, hh, mm, 0, 0));
					Schedule.InvalidateNextTick();

					OnAddTime(button);
				},
				CancelHandler = Refresh
			}.Send();
		}

		protected virtual void OnPresetsMenu(GumpButton button)
		{
			var entries = new[]
			{
				new ListGumpEntry("Noon", b => UsePreset(b, ScheduleTimes.Noon), HighlightHue),
				new ListGumpEntry("Midnight", b => UsePreset(b, ScheduleTimes.Midnight), HighlightHue),
				new ListGumpEntry("Every Hour", b => UsePreset(b, ScheduleTimes.EveryHour), HighlightHue),
				new ListGumpEntry("Every 30 Minutes", b => UsePreset(b, ScheduleTimes.EveryHalfHour), HighlightHue),
				new ListGumpEntry("Every 15 Minutes", b => UsePreset(b, ScheduleTimes.EveryQuarterHour), HighlightHue),
				new ListGumpEntry("Every 10 Minutes", b => UsePreset(b, ScheduleTimes.EveryTenMinutes), HighlightHue),
				new ListGumpEntry("Every 5 Minutes", b => UsePreset(b, ScheduleTimes.EveryFiveMinutes), HighlightHue),
				new ListGumpEntry("Every Minute", b => UsePreset(b, ScheduleTimes.EveryMinute), HighlightHue),
				new ListGumpEntry("Four Twenty", b => UsePreset(b, ScheduleTimes.FourTwenty), HighlightHue)
			};

			Send(new MenuGump(User, this, entries, button));
		}

		protected virtual void UsePreset(GumpButton button, ScheduleTimes times)
		{
			Schedule.Info.Times.Clear();
			Schedule.Info.Times.Add(times);
			Schedule.InvalidateNextTick();

			Refresh(true);
		}

		protected virtual void ParseTime(string text, out int hh, out int mm)
		{
			var parts = text.Split(':');

			if (parts.Length >= 2)
			{
				if (!Int32.TryParse(parts[0], out hh))
				{
					hh = -1;
				}

				if (!Int32.TryParse(parts[1], out mm))
				{
					mm = -1;
				}

				hh = (hh < 0) ? 0 : (hh > 23) ? 23 : hh;
				mm = (mm < 0) ? 0 : (mm > 59) ? 59 : mm;
			}
			else
			{
				hh = -1;
				mm = -1;
			}
		}

		protected override void CompileList(List<TimeSpan> list)
		{
			list.Clear();
			list.AddRange(Schedule.Info.Times);

			base.CompileList(list);
		}

		public override string GetSearchKeyFor(TimeSpan key)
		{
			return key.ToSimpleString("h:m");
		}

		protected override void SelectEntry(GumpButton button, TimeSpan entry)
		{
			base.SelectEntry(button, entry);

			Send(new ScheduleTimeListEntryGump(User, Schedule, Refresh(), button, entry, UseConfirmDialog));
		}
	}
}