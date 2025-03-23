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
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

using Server;
using Server.Gumps;
#endregion

namespace VitaNex.SuperGumps.UI
{
	public class ListGump<T> : SuperGumpList<T>
	{
		public const string DefaultTitle = "List View";
		public const string DefaultEmptyText = "No entries to display.";

		protected bool WasModal { get; set; }

		public virtual string Title { get; set; }

		public virtual string EmptyText { get; set; }

		public virtual bool Minimized { get; set; }

		public virtual T Selected { get; set; }

		public virtual bool CanSearch { get; set; }
		public virtual string SearchText { get; set; }
		public virtual List<T> SearchResults { get; set; }

		public virtual MenuGumpOptions Options { get; set; }

		public MenuGump Menu { get; private set; }

		public override int EntryCount => IsSearching() ? SearchResults.Count : base.EntryCount;

		public int EntryHeight { get; set; }

		public int Columns { get; set; }
		public int Width { get; set; }

		public ListGump(
			Mobile user,
			Gump parent = null,
			int? x = null,
			int? y = null,
			IEnumerable<T> list = null,
			string emptyText = null,
			string title = null,
			IEnumerable<ListGumpEntry> opts = null)
			: base(user, parent, x, y, list)
		{
			EmptyText = emptyText ?? DefaultEmptyText;
			Title = title ?? DefaultTitle;

			Minimized = false;

			CanMove = false;
			CanSearch = true;

			EntryHeight = 30;

			Width = 400;
			Columns = 1;

			Options = new MenuGumpOptions(opts);
		}

		public override void AssignCollections()
		{
			if (SearchResults == null)
			{
				SearchResults = new List<T>(0x20);
			}

			base.AssignCollections();
		}

		public virtual bool IsSearching()
		{
			return CanSearch && !String.IsNullOrWhiteSpace(SearchText);
		}

		protected override void Compile()
		{
			base.Compile();

			if (Options == null)
			{
				Options = new MenuGumpOptions();
			}
			else
			{
				Options.Clear();
			}

			CompileMenuOptions(Options);
		}

		protected override void CompileList(List<T> list)
		{
			SearchResults.Clear();

			if (IsSearching())
			{
				SearchResults.AddRange(
					list.Where(
						o => o != null && Regex.IsMatch(GetSearchKeyFor(o), Regex.Escape(SearchText), RegexOptions.IgnoreCase)));

				if (Sorted)
				{
					SearchResults.Sort(SortCompare);
				}
			}

			base.CompileList(list);
		}

		public virtual string GetSearchKeyFor(T key)
		{
			return key != null ? key.ToString() : "NULL";
		}

		protected virtual void OnClearSearch(GumpButton button)
		{
			SearchText = null;
			Refresh(true);
		}

		protected virtual void OnNewSearch(GumpButton button)
		{
			new InputDialogGump(User, this)
			{
				Title = "Search",
				Html = "Search " + Title + ".\nRegular Expressions are supported.",
				Limit = 100,
				Callback = (subBtn, input) =>
				{
					SearchText = input;
					Page = 0;
					Refresh(true);
				}
			}.Send();
		}

		protected virtual void CompileMenuOptions(MenuGumpOptions list)
		{
			if (Minimized)
			{
				list.Replace("Minimize", "Maximize", Maximize);
			}
			else
			{
				list.Replace("Maximize", "Minimize", Minimize);
			}

			if (CanSearch)
			{
				if (IsSearching())
				{
					list.Replace("New Search", "Clear Search", OnClearSearch);
				}
				else
				{
					list.Replace("Clear Search", "New Search", OnNewSearch);
				}
			}

			list.AppendEntry("Refresh", b => Refresh(b));

			if (CanClose)
			{
				list.AppendEntry("Exit", b => Close(b));
			}
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			var sup = SupportsUltimaStore;
			var ec = IsEnhancedClient;
			var bgID = ec ? 83 : sup ? 40000 : 9270;
			var fillID = ec ? 87 : sup ? 40004 : 2624;

			layout.Add(
				"background/header/base",
				() =>
				{
					AddBackground(0, 0, Width + 20, 50, bgID);
					AddImageTiled(10, 10, Width, 30, fillID);
					//AddAlphaRegion(10, 10, Width, 30);
				});

			layout.Add(
				"button/header/options",
				() =>
				{
					AddButton(15, 15, 2008, 2007, ShowOptionMenu);
					AddTooltip(1015326);
				});

			layout.Add(
				"button/header/minimize",
				() =>
				{
					if (Minimized)
					{
						AddButton(Width - 10, 20, 10740, 10742, Maximize);
						AddTooltip(3002086);
					}
					else
					{
						AddButton(Width - 10, 20, 10741, 10742, Minimize);
						AddTooltip(3002085);
					}
				});

			layout.Add(
				"label/header/title",
				() =>
				{
					var title = String.IsNullOrWhiteSpace(Title) ? DefaultTitle : Title;

					AddLabelCropped(90, 15, Width - 90, 20, GetTitleHue(), title);
				});

			if (Minimized)
			{
				return;
			}

			layout.Add("imagetiled/body/spacer", () => AddImageTiled(0, 50, Width + 20, 10, fillID));

			var range = GetListRange();

			if (range.Count > 0)
			{
				var height = (int)Math.Ceiling(range.Count / Math.Max(1.0, Columns)) * EntryHeight;

				layout.Add(
					"background/body/base",
					() =>
					{
						AddBackground(0, 55, Width + 20, height + 20, bgID);
						AddImageTiled(10, 65, Width, height, fillID);
						//AddAlphaRegion(10, 65, Width, height);
					});

				CompileEntryLayout(layout, range);

				range.Clear();
			}
			else
			{
				layout.Add(
					"background/body/base",
					() =>
					{
						AddBackground(0, 55, Width + 20, 50, bgID);
						AddImageTiled(10, 65, Width, 30, fillID);
						//AddAlphaRegion(10, 65, Width, 30); 
					});

				layout.Add(
					"label/list/empty",
					() =>
					{
						var label = String.IsNullOrEmpty(EmptyText) ? DefaultEmptyText : EmptyText;

						label = label.WrapUOHtmlCenter();

						AddHtml(15, 72, Width - 10, 20, label, Color.IndianRed, Color.Empty);
					});
			}

			layout.Add(
				"widget/body/scrollbar",
				() => AddScrollbarHM(10, 46, Width, PageCount, Page, PreviousPage, NextPage));
		}

		protected virtual void CompileEntryLayout(SuperGumpLayout layout, Dictionary<int, T> range)
		{
			var i = 0;
			var h = EntryHeight;

			foreach (var kv in range)
			{
				CompileEntryLayout(layout, range.Count, kv.Key, i, 70 + ((i++ / Columns) * h), kv.Value);
			}
		}

		protected virtual void CompileEntryLayout(
			SuperGumpLayout layout,
			int length,
			int index,
			int pIndex,
			int yOffset,
			T entry)
		{
			var wOffset = (Width - 10) / Columns;
			var xOffset = (pIndex % Columns) * wOffset;

			layout.Add(
				"button/list/select/" + index,
				() => AddButton(xOffset + 15, yOffset, 4006, 4007, b => SelectEntry(b, entry)));

			layout.Add(
				"label/list/entry/" + index,
				() =>
				{
					var offset = 15 + GetImageSize(4006).Width + 5;

					var hue = GetLabelHue(index, pIndex, entry);
					var text = GetLabelText(index, pIndex, entry);

					AddLabelCropped(xOffset + offset, yOffset + 2, wOffset - offset, EntryHeight - 2, hue, text);
				});
		}

		public override IEnumerable<T> EnumerateListRange(int index, int length)
		{
			if (!IsSearching())
			{
				foreach (var o in base.EnumerateListRange(index, length))
				{
					yield return o;
				}

				yield break;
			}

			index = Math.Max(0, Math.Min(EntryCount, index));
			length = Math.Max(0, Math.Min(EntryCount - index, length));

			while (--length >= 0 && SearchResults.InBounds(index))
			{
				yield return SearchResults[index++];
			}
		}

		public override Dictionary<int, T> GetListRange(int index, int length)
		{
			if (!IsSearching())
			{
				return base.GetListRange(index, length);
			}

			index = Math.Max(0, Math.Min(EntryCount, index));
			length = Math.Max(0, Math.Min(EntryCount - index, length));

			var d = new Dictionary<int, T>(length);

			while (--length >= 0 && SearchResults.InBounds(index))
			{
				d[index] = SearchResults[index++];
			}

			return d;
		}

		protected virtual void Minimize(GumpButton entry = null)
		{
			Minimized = true;

			if (Modal)
			{
				WasModal = true;
			}

			Modal = false;

			Refresh(true);
		}

		protected virtual void Maximize(GumpButton entry = null)
		{
			Minimized = false;

			if (WasModal)
			{
				Modal = true;
			}

			WasModal = false;

			Refresh(true);
		}

		protected virtual int GetTitleHue()
		{
			return HighlightHue;
		}

		protected virtual string GetLabelText(int index, int pageIndex, T entry)
		{
			return entry != null ? entry.ToString() : "NULL";
		}

		protected virtual int GetLabelHue(int index, int pageIndex, T entry)
		{
			return entry != null ? TextHue : ErrorHue;
		}

		protected virtual void SelectEntry(GumpButton button, T entry)
		{
			Selected = entry;
		}

		protected virtual void ShowOptionMenu(GumpButton button)
		{
			if (User != null && !User.Deleted && Options != null && Options.Count > 0)
			{
				Send(new MenuGump(User, Refresh(), Options, button));
			}
		}
	}

	public class ListGump<T, U> : ListGump<KeyValuePair<T, U>>
	{
		public ListGump(
			Mobile user,
			Gump parent = null,
			int? x = null,
			int? y = null,
			IEnumerable<KeyValuePair<T, U>> list = null,
			string emptyText = null,
			string title = null,
			IEnumerable<ListGumpEntry> opts = null)
			: base(user, parent, x, y, list, emptyText, title, opts)
		{ }
	}
}
