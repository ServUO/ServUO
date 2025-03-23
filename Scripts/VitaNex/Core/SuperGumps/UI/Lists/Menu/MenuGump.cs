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
using System.Linq;

using Server;
using Server.Gumps;

using VitaNex.Text;
#endregion

namespace VitaNex.SuperGumps.UI
{
	public class MenuGump : SuperGumpList<ListGumpEntry>
	{
		public int GuessWidth { get; protected set; }

		public int EntryHeight { get; set; }

		public GumpButton Clicked { get; set; }

		public ListGumpEntry Selected { get; set; }

		public MenuGump(Mobile user, Gump parent = null, IEnumerable<ListGumpEntry> list = null, GumpButton clicked = null)
			: base(user, parent, DefaultX, DefaultY, list)
		{
			EntryHeight = 30;

			Clicked = clicked;

			if (Clicked != null)
			{
				if (Clicked.Parent != null)
				{
					X = Clicked.Parent.X + Clicked.X;
					Y = Clicked.Parent.Y + Clicked.Y;

					if (Parent == null)
					{
						Parent = Clicked.Parent;
					}
				}
				else if (Parent != null)
				{
					X = Parent.X;
					Y = Parent.Y;
				}
				else
				{
					X = DefaultX;
					Y = DefaultY;
				}
			}
			else if (Parent != null)
			{
				X = Parent.X;
				Y = Parent.Y;
			}
			else
			{
				X = DefaultX;
				Y = DefaultY;
			}

			ForceRecompile = true;
			CanMove = false;
			CanResize = false;
			Modal = true;
		}

		protected virtual void InvalidateWidth()
		{
			double epp = EntriesPerPage;

			GuessWidth = 100;

			if (epp > 0)
			{
				var font = UOFont.Unicode[1];

				GuessWidth += List.Select((e, i) => font.GetWidth(GetLabelText(i, (int)Math.Ceiling(i + 1 / epp), e))).Highest();
			}
		}

		public void AddOption(string label, Action handler)
		{
			List.Add(new ListGumpEntry(label, handler));
		}

		public void AddOption(string label, Action handler, int hue)
		{
			List.Add(new ListGumpEntry(label, handler, hue));
		}

		public void AddOption(string label, Action<GumpButton> handler)
		{
			List.Add(new ListGumpEntry(label, handler));
		}

		public void AddOption(string label, Action<GumpButton> handler, int hue)
		{
			List.Add(new ListGumpEntry(label, handler, hue));
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			var range = GetListRange();

			var eh = range.Count * EntryHeight;

			var sup = SupportsUltimaStore;
			var ec = IsEnhancedClient;
			var bgID = ec ? 83 : sup ? 40000 : 9270;

			layout.Add(
				"background/body/base",
				() =>
				{
					AddBackground(0, 0, GuessWidth, 30 + eh, bgID);

					if (!sup)
					{
						AddImageTiled(10, 10, GuessWidth - 20, 10 + eh, 2624);
						//AddAlphaRegion(10, 10, GuessWidth - 20, 10 + eh);
					}
				});

			layout.Add(
				"imagetiled/body/vsep/0",
				() =>
				{
					if (!sup || ec)
					{
						AddImageTiled(50, 20, 5, eh, bgID + 5);
					}
				});

			CompileEntryLayout(layout, range);

			layout.Add(
				"widget/body/scrollbar",
				() => AddScrollbarHM(10, 10, GuessWidth - 20, PageCount, Page, PreviousPage, NextPage));
		}

		public virtual void CompileEntryLayout(SuperGumpLayout layout, Dictionary<int, ListGumpEntry> range)
		{
			range.For((i, kv) => CompileEntryLayout(layout, range.Count, kv.Key, i, 25 + (i * EntryHeight), kv.Value));
		}

		public virtual void CompileEntryLayout(
			SuperGumpLayout layout,
			int length,
			int index,
			int pIndex,
			int yOffset,
			ListGumpEntry entry)
		{
			var sup = SupportsUltimaStore;
			var ec = IsEnhancedClient;
			var bgID = ec ? 83 : sup ? 40000 : 9270;

			layout.Add("button/list/select/" + index, () => AddButton(15, yOffset, 4006, 4007, b => SelectEntry(b, entry)));

			layout.Add(
				"label/list/entry/" + index,
				() => AddLabelCropped(
					65,
					2 + yOffset,
					GuessWidth - 75,
					20,
					GetLabelHue(index, pIndex, entry),
					GetLabelText(index, pIndex, entry)));

			if (pIndex < (length - 1))
			{
				layout.Add(
					"imagetiled/body/hsep/" + index,
					() =>
					{
						if (!ec)
						{
							AddImageTiled(10, 25 + yOffset, GuessWidth - 20, 5, bgID + 7);
						}
					});
			}
		}

		protected override sealed void CompileList(List<ListGumpEntry> list)
		{
			var opts = new MenuGumpOptions(list);

			CompileOptions(opts);

			list.Clear();
			list.AddRange(opts);

			InvalidateWidth();
		}

		protected virtual void CompileOptions(MenuGumpOptions list)
		{
			list.Insert(list.Count, new ListGumpEntry("Cancel", Cancel, ErrorHue));
		}

		protected virtual string GetLabelText(int index, int pageIndex, ListGumpEntry entry)
		{
			return entry != null ? entry.Label : "NULL";
		}

		protected virtual int GetLabelHue(int index, int pageIndex, ListGumpEntry entry)
		{
			return entry != null ? entry.Hue : ErrorHue;
		}

		protected virtual void SelectEntry(GumpButton button, ListGumpEntry entry)
		{
			Selected = entry;

			if (entry == null || entry.Handler == null)
			{
				Close();
				return;
			}

			entry.Handler(button);
		}

		protected virtual void Cancel(GumpButton button)
		{
			Close();
		}
	}
}
