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
using Server.Items;

using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
using VitaNex.Targets;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class PvPDoorsUI : ListGump<BaseDoor>
	{
		public PvPBattle Battle { get; set; }

		public bool UseConfirmDialog { get; set; }

		public PvPDoorsUI(Mobile user, PvPBattle battle, Gump parent = null, bool useConfirm = true)
			: base(user, parent, emptyText: "There are no doors to display.", title: "PvP Battle Doors")
		{
			Battle = battle;
			UseConfirmDialog = useConfirm;

			ForceRecompile = true;
		}

		protected override void CompileMenuOptions(MenuGumpOptions list)
		{
			list.Clear();

			if (User.AccessLevel >= AutoPvP.Access)
			{
				list.AppendEntry(
					new ListGumpEntry(
						"Remove All",
						b =>
						{
							if (UseConfirmDialog)
							{
								Send(
									new ConfirmDialogGump(
										User,
										this,
										title: "Remove All Doors?",
										html:
										"All doors in the list will be removed (not deleted).\nThis action can not be reversed.\n\nDo you want to continue?",
										onAccept: OnConfirmRemoveAllDoors));
							}
							else
							{
								OnConfirmRemoveAllDoors(b);
							}
						},
						HighlightHue));

				list.AppendEntry(new ListGumpEntry("Add Door", b => SelectAddDoor(), HighlightHue));

				list.AppendEntry(
					new ListGumpEntry(
						"Open All",
						b => List.ForEach(
							door =>
							{
								if (door == null || door.Deleted || door.Open)
								{
									return;
								}

								door.Open = true;

								if (!door.UseChainedFunctionality)
								{
									return;
								}

								door.GetChain()
									.ForEach(
										chained =>
										{
											if (chained == null || chained.Deleted || chained.Open)
											{
												return;
											}

											chained.Open = true;
										});
							}),
						HighlightHue));

				list.AppendEntry(
					new ListGumpEntry(
						"Close All",
						b => List.ForEach(
							door =>
							{
								if (door == null || door.Deleted || !door.Open)
								{
									return;
								}

								door.Open = false;

								if (!door.UseChainedFunctionality)
								{
									return;
								}

								door.GetChain()
									.ForEach(
										chained =>
										{
											if (chained != null && !chained.Deleted && chained.Open)
											{
												chained.Open = false;
											}
										});
							}),
						ErrorHue));

				list.AppendEntry(
					new ListGumpEntry(
						"Unlock All",
						b => List.ForEach(
							door =>
							{
								if (door == null || door.Deleted || !door.Locked)
								{
									return;
								}

								door.Locked = false;

								if (!door.UseChainedFunctionality)
								{
									return;
								}

								door.GetChain()
									.ForEach(
										chained =>
										{
											if (chained != null && !chained.Deleted && chained.Locked)
											{
												chained.Locked = false;
											}
										});
							}),
						HighlightHue));

				list.AppendEntry(
					new ListGumpEntry(
						"Lock All",
						b => List.ForEach(
							door =>
							{
								if (door == null || door.Deleted || door.Locked)
								{
									return;
								}

								door.Locked = true;

								if (!door.UseChainedFunctionality)
								{
									return;
								}

								door.GetChain()
									.ForEach(
										chained =>
										{
											if (chained != null && !chained.Deleted && !chained.Locked)
											{
												chained.Locked = true;
											}
										});
							}),
						ErrorHue));
			}

			base.CompileMenuOptions(list);
		}

		protected virtual void SelectAddDoor()
		{
			Minimize();
			User.Target = new ItemSelectTarget<BaseDoor>(OnDoorAdd, OnDoorAddCancel);
		}

		protected virtual void OnDoorAdd(Mobile m, BaseDoor door)
		{
			if (!Battle.Doors.Contains(door))
			{
				Battle.Doors.Add(door);
				m.SendMessage("The door has been added to the list.");
				SelectAddDoor();
			}
			else if (Minimized)
			{
				Maximize();
			}

			Refresh(true);
		}

		protected virtual void OnDoorAddCancel(Mobile m)
		{
			if (Minimized)
			{
				Maximize();
			}

			Refresh(true);
		}

		protected virtual void OnConfirmRemoveAllDoors(GumpButton button)
		{
			foreach (var team in List)
			{
				team.Delete();
			}

			Refresh(true);
		}

		protected override void CompileList(List<BaseDoor> list)
		{
			list.Clear();
			list.AddRange(Battle.Doors);
			base.CompileList(list);
		}

		public override string GetSearchKeyFor(BaseDoor key)
		{
			return key != null ? (key.Name ?? key.Serial.ToString()) : base.GetSearchKeyFor(null);
		}

		protected override void SelectEntry(GumpButton button, BaseDoor entry)
		{
			base.SelectEntry(button, entry);

			if (button == null || entry == null || entry.Deleted)
			{
				return;
			}

			if (User.AccessLevel < AutoPvP.Access)
			{
				Refresh(true);
				return;
			}

			var list = new MenuGumpOptions();

			list.AppendEntry(
				new ListGumpEntry(
					"Remove",
					b =>
					{
						Battle.Doors.Remove(entry);
						Refresh(true);
					},
					HighlightHue));

			if (entry.Open)
			{
				list.AppendEntry(
					new ListGumpEntry(
						"Close",
						b =>
						{
							if (entry.Deleted || !entry.Open)
							{
								return;
							}

							entry.Open = false;

							if (entry.UseChainedFunctionality)
							{
								entry.GetChain()
									 .ForEach(
										 chained =>
										 {
											 if (chained != null && !chained.Deleted && chained.Open)
											 {
												 chained.Open = false;
											 }
										 });
							}

							Refresh(true);
						},
						ErrorHue));
			}
			else
			{
				list.AppendEntry(
					new ListGumpEntry(
						"Open",
						b =>
						{
							if (entry.Deleted || entry.Open)
							{
								return;
							}

							entry.Open = true;

							if (entry.UseChainedFunctionality)
							{
								entry.GetChain()
									 .ForEach(
										 chained =>
										 {
											 if (chained != null && !chained.Deleted && !chained.Open)
											 {
												 chained.Open = true;
											 }
										 });
							}

							Refresh(true);
						},
						HighlightHue));
			}

			if (entry.Open)
			{
				list.AppendEntry(
					new ListGumpEntry(
						"Lock",
						b =>
						{
							if (entry.Deleted || entry.Locked)
							{
								return;
							}

							entry.Locked = true;

							if (entry.UseChainedFunctionality)
							{
								entry.GetChain()
									 .ForEach(
										 chained =>
										 {
											 if (chained != null && !chained.Deleted && !chained.Locked)
											 {
												 chained.Locked = true;
											 }
										 });
							}

							Refresh(true);
						},
						ErrorHue));
			}
			else
			{
				list.AppendEntry(
					new ListGumpEntry(
						"Unlock",
						b =>
						{
							if (entry.Deleted || !entry.Locked)
							{
								return;
							}

							entry.Locked = false;

							if (entry.UseChainedFunctionality)
							{
								entry.GetChain()
									 .ForEach(
										 chained =>
										 {
											 if (chained != null && !chained.Deleted && chained.Locked)
											 {
												 chained.Locked = false;
											 }
										 });
							}

							Refresh(true);
						},
						HighlightHue));
			}

			Send(new MenuGump(User, Refresh(), list, button));
		}

		protected override void CompileEntryLayout(
			SuperGumpLayout layout,
			int length,
			int index,
			int pIndex,
			int yOffset,
			BaseDoor entry)
		{
			base.CompileEntryLayout(layout, length, index, pIndex, yOffset, entry);

			layout.Replace(
				"label/list/entry/" + index,
				() =>
				{
					AddLabelCropped(65, 2 + yOffset, 150, 20, GetLabelHue(index, pIndex, entry), GetLabelText(index, pIndex, entry));
					AddLabelCropped(
						205,
						2 + yOffset,
						170,
						20,
						GetStateLabelHue(index, pIndex, entry),
						GetStateLabelText(index, pIndex, entry));
				});
		}

		protected override int GetLabelHue(int index, int pageIndex, BaseDoor entry)
		{
			return entry != null
				? ((entry.Hue > 0 && entry.Hue < 3000) ? entry.Hue : TextHue)
				: base.GetLabelHue(index, pageIndex, null);
		}

		protected override string GetLabelText(int index, int pageIndex, BaseDoor entry)
		{
			return entry != null && !entry.Deleted
				? (entry.Name ?? entry.Serial.ToString())
				: base.GetLabelText(index, pageIndex, entry);
		}

		protected virtual string GetStateLabelText(int index, int pageIndex, BaseDoor entry)
		{
			return entry != null ? (entry.Open ? "Open" : "Closed") : String.Empty;
		}

		protected virtual int GetStateLabelHue(int index, int pageIndex, BaseDoor entry)
		{
			return entry != null ? (entry.Open ? HighlightHue : ErrorHue) : TextHue;
		}
	}
}