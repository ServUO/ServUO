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
using System.Drawing;

using Server;
using Server.Gumps;

using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Schedules
{
	public class ScheduleOverviewGump : HtmlPanelGump<Schedule>
	{
		public bool UseConfirmDialog { get; set; }

		public ScheduleOverviewGump(Mobile user, Schedule schedule, Gump parent = null, bool useConfirm = true)
			: base(user, parent, emptyText: "Schedule Unavailable", title: "Schedule Overview", selected: schedule)
		{
			UseConfirmDialog = useConfirm;

			HtmlColor = Color.GreenYellow;
			ForceRecompile = true;
			AutoRefresh = true;
		}

		protected override void Compile()
		{
			base.Compile();

			if (Selected != null)
			{
				Html = Selected.ToHtmlString();
			}
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			layout.Replace(
				"label/header/title",
				() =>
				{
					var title = String.IsNullOrEmpty(Title) ? "Schedule Overview" : Title;

					AddLabelCropped(90, 15, Width - 235, 20, GetTitleHue(), title);
				});

			layout.Replace(
				"label/header/subtitle",
				() =>
				{
					var time = Selected.Now.ToSimpleString("t@h:m@ X");

					AddLabelCropped(90 + (Width - 235), 15, 100, 20, HighlightHue, time);
				});
		}

		protected override void CompileMenuOptions(MenuGumpOptions list)
		{
			list.Clear();

			if (Selected != null && User.AccessLevel >= Schedules.Access)
			{
				if (!Selected.Enabled)
				{
					list.AppendEntry(
						"Enable",
						b =>
						{
							Selected.Enabled = true;
							Refresh(true);
						},
						HighlightHue);
				}
				else
				{
					list.AppendEntry(
						"Disable",
						b =>
						{
							Selected.Enabled = false;
							Refresh(true);
						},
						HighlightHue);
				}

				if (Selected.IsLocal)
				{
					list.AppendEntry(
						"Use Universal Time",
						b =>
						{
							Selected.IsLocal = false;
							Refresh(true);
						},
						HighlightHue);
				}
				else
				{
					list.AppendEntry(
						"Use Local Time",
						b =>
						{
							Selected.IsLocal = true;
							Refresh(true);
						},
						HighlightHue);
				}

				list.AppendEntry("Edit Months", b => Send(new ScheduleMonthsMenuGump(User, Selected, Refresh(), b)), HighlightHue);
				list.AppendEntry("Edit Days", b => Send(new ScheduleDaysMenuGump(User, Selected, Refresh(), b)), HighlightHue);
				list.AppendEntry("Edit Times", b => Send(new SheduleTimeListGump(User, Selected, Hide(true))), HighlightHue);

				list.AppendEntry(
					"Clear Schedule",
					b =>
					{
						if (UseConfirmDialog)
						{
							new ConfirmDialogGump(User, this)
							{
								Title = "Clear Schedule?",
								Html = "The schedule will be cleared, erasing all data associated with its entries.\n" +
									   "This action can not be reversed.\n\nDo you want to continue?",
								AcceptHandler = OnConfirmClearSchedule,
								CancelHandler = Refresh
							}.Send();
						}
						else
						{
							OnConfirmClearSchedule(b);
						}
					},
					HighlightHue);
			}

			base.CompileMenuOptions(list);
		}

		protected virtual void OnConfirmClearSchedule(GumpButton button)
		{
			if (Selected == null)
			{
				Close();
				return;
			}

			Selected.Info.Clear();
			Selected.InvalidateNextTick();

			Refresh(true);
		}
	}
}