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

using Server;
using Server.Gumps;

using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Schedules
{
	public class ScheduleTimeListEntryGump : MenuGump
	{
		public Schedule Schedule { get; set; }

		public TimeSpan Time { get; set; }

		public bool UseConfirmDialog { get; set; }

		public ScheduleTimeListEntryGump(
			Mobile user,
			Schedule schedule,
			Gump parent = null,
			GumpButton clicked = null,
			TimeSpan? time = null,
			bool useConfirm = true)
			: base(user, parent, clicked: clicked)
		{
			Schedule = schedule;
			Time = time ?? TimeSpan.Zero;
			UseConfirmDialog = useConfirm;

			CanMove = false;
			CanResize = false;
		}

		protected override void CompileOptions(MenuGumpOptions list)
		{
			base.CompileOptions(list);

			list.PrependEntry(
				"Delete",
				button =>
				{
					if (UseConfirmDialog)
					{
						new ConfirmDialogGump(User, Refresh())
						{
							Title = "Delete Time?",
							Html = "All data associated with this time will be deleted.\n" +
								   "This action can not be reversed!\nDo you want to continue?",
							AcceptHandler = OnConfirmDelete
						}.Send();
					}
					else
					{
						OnConfirmDelete(button);
					}
				},
				HighlightHue);
		}

		protected virtual void OnConfirmDelete(GumpButton button)
		{
			if (Selected == null)
			{
				Close();
				return;
			}

			Schedule.Info.Times.Remove(Time);
			Schedule.InvalidateNextTick();

			Close();
		}
	}
}