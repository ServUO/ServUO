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

using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Schedules
{
	public class ScheduleListGump : ListGump<Schedule>
	{
		public static string HelpText = "Schedules: Schedules are timers that tick on specific dates at specific times.";

		public bool UseConfirmDialog { get; set; }

		public ScheduleListGump(Mobile user, Gump parent = null, bool useConfirm = true)
			: base(user, parent, emptyText: "There are no schedules to display.", title: "Schedules")
		{
			UseConfirmDialog = useConfirm;

			ForceRecompile = true;
			CanMove = false;
			CanResize = false;

			AutoRefresh = true;
		}

		protected override string GetLabelText(int index, int pageIndex, Schedule entry)
		{
			return entry != null ? entry.Name : base.GetLabelText(index, pageIndex, null);
		}

		protected override int GetLabelHue(int index, int pageIndex, Schedule entry)
		{
			if (entry != null)
			{
				if (!entry.Enabled)
				{
					return ErrorHue;
				}

				if (!entry.Running || entry.NextGlobalTick == null)
				{
					return HighlightHue;
				}
			}

			return base.GetLabelHue(index, pageIndex, null);
		}

		protected override void CompileMenuOptions(MenuGumpOptions list)
		{
			list.AppendEntry(new ListGumpEntry("Help", ShowHelp));

			base.CompileMenuOptions(list);
		}

		protected virtual void ShowHelp(GumpButton button)
		{
			Send(new NoticeDialogGump(User, this, title: "Help", html: HelpText));
		}

		protected override void CompileList(List<Schedule> list)
		{
			list.Clear();
			list.AddRange(Schedules.Registry);

			base.CompileList(list);
		}

		public override string GetSearchKeyFor(Schedule key)
		{
			return key != null ? key.Name : base.GetSearchKeyFor(null);
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			layout.Replace(
				"label/header/title",
				() => AddLabelCropped(90, 15, 185, 20, GetTitleHue(), String.IsNullOrEmpty(Title) ? DefaultTitle : Title));

			layout.Replace(
				"label/header/subtitle",
				() =>
				{
					var loc = DateTime.Now.ToSimpleString("D t@h:m@ X");
					var utc = DateTime.UtcNow.ToSimpleString("D t@h:m@ X");

					AddLabelCropped(275, 15, 100, 20, HighlightHue, String.Format("[{0}] [{1}]", loc, utc));
				});
		}

		protected override void SelectEntry(GumpButton button, Schedule entry)
		{
			base.SelectEntry(button, entry);

			if (button != null && entry != null)
			{
				Send(new ScheduleOverviewGump(User, entry, Hide(true), UseConfirmDialog));
			}
		}
	}
}