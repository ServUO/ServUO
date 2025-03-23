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

using Server.Gumps;
using Server.Mobiles;

using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
#endregion

namespace Server
{
	public class FilterGump : TreeGump
	{
		private PlayerMobile _Owner;

		public PlayerMobile Owner => _Owner == User ? _Owner : (_Owner = User as PlayerMobile);

		public bool UseOwnFilter
		{
			get => Owner != null && Owner.UseOwnFilter;
			set
			{
				if (Owner != null)
				{
					Owner.UseOwnFilter = value;
				}
			}
		}

		public IFilter OwnerFilter => Owner != null ? Owner.GetFilter(MainFilter.GetType()) : null;

		public IFilter Filter => UseOwnFilter ? OwnerFilter ?? MainFilter : MainFilter;

		public IFilter MainFilter { get; private set; }

		public Action<Mobile, IFilter> ApplyHandler { get; set; }

		public FilterGump(Mobile user, Gump parent, IFilter filter, Action<Mobile, IFilter> onApply)
			: base(user, parent)
		{
			MainFilter = filter;
			ApplyHandler = onApply;

			Width = 800;
			Height = 500;
		}

		protected override void Compile()
		{
			Title = "Filtering: " + Filter.Name;

			base.Compile();
		}

		protected override bool OnBeforeSend()
		{
			if (MainFilter == null)
			{
				return false;
			}

			return base.OnBeforeSend();
		}

		protected override void OnClosed(bool all)
		{
			base.OnClosed(all);

			if (ApplyHandler != null)
			{
				ApplyHandler(User, Filter);
			}
		}

		protected override void CompileNodes(Dictionary<TreeGumpNode, Action<Rectangle, int, TreeGumpNode>> list)
		{
			base.CompileNodes(list);

			foreach (var c in Filter.Options.Categories)
			{
				list[c] = AddFilterOptions;
			}
		}

		protected virtual void AddFilterOptions(Rectangle b, int i, TreeGumpNode n)
		{
			var cols = (int)(b.Width / (b.Width / 3.0));
			var rows = (int)(b.Height / 30.0);

			var cellW = (int)(b.Width / (double)cols);
			var cellH = (int)(b.Height / (double)rows);

			var opts = Filter.Options.Where(o => Insensitive.Equals(o.Category, n.Name)).ToList();

			if (opts.Count == 0)
			{
				return;
			}

			i = -1;

			int c, x, r, y;

			for (r = 0; r < rows && opts.InBounds(i + 1); r++)
			{
				y = b.Y + (r * cellH);

				for (c = 0; c < cols && opts.InBounds(i + 1); c++)
				{
					x = b.X + (c * cellW);

					var o = opts[++i];

					if (o.IsEmpty)
					{
						if (c == 0)
						{
							--c;
							continue;
						}

						break;
					}

					AddOptionCell(x, y, cellW, cellH, o);
				}
			}

			opts.Free(true);
		}

		protected virtual void AddOptionCell(int x, int y, int w, int h, FilterOption o)
		{
			y += (h / 2) - 10;

			if (o.IsSelected(Filter))
			{
				AddImage(x + 5, y, 9904);
				AddHtml(x + 35, y + 2, w - 40, 40, FormatText(Color.LawnGreen, o.Name), false, false);
			}
			else
			{
				AddButton(x + 5, y, 9903, 9905, btn => Refresh(o.Select(Filter)));
				AddHtml(x + 35, y + 2, w - 40, 40, FormatText(Color.White, o.Name), false, false);
			}
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			layout.Replace("panel/right/overlay", () => AddImageTiled(265, 68, Width - 290, Height - 50, 2624));

			layout.Add(
				"cpanel",
				() =>
				{
					var x = 0;
					var y = Height + 43;
					var w = Width;

					AddBackground(x, y, w, 150, 9260);

					AddBackground(x + 15, y + 15, 234, 120, 9270);
					AddImageTiled(x + 25, y + 25, 214, 100, 1280);

					var use = String.Format("Use {0} Filter", UseOwnFilter ? "Main" : "My");

					AddButton(
						x + 25,
						y + 25,
						4006,
						4007,
						b =>
						{
							UseOwnFilter = !UseOwnFilter;
							Refresh(true);
						}); // Use [My|Book] Filter
					AddHtml(x + 60, y + 27, 164, 40, FormatText(Color.Goldenrod, use), false, false);

					AddButton(
						x + 25,
						y + 50,
						4021,
						4022,
						b =>
						{
							Filter.Clear();
							Refresh(true);
						}); //Clear
					AddHtml(x + 60, y + 52, 164, 40, FormatText(Color.OrangeRed, "Clear"), false, false);

					AddButton(x + 25, y + 75, 4024, 4025, Close); //Apply
					AddHtml(x + 60, y + 77, 164, 40, FormatText(Color.Gold, "Apply"), false, false);

					x += 239;

					AddBackground(x + 15, y + 15, w - 270, 120, 9270);
					AddImageTiled(x + 25, y + 25, w - 290, 100, 1280);

					AddHtml(x + 30, y + 25, w - 295, 100, GetFilteringText(), false, true);
				});
		}

		protected virtual string FormatOption(FilterOption o)
		{
			return FormatText(
				Color.PaleGoldenrod,
				"{0}: {1}",
				FormatText(Color.PaleGoldenrod, o.Category),
				FormatText(Color.LawnGreen, o.Name));
		}

		protected virtual string GetFilteringText()
		{
			return String.Join("\n", Filter.Options.Where(o => o.IsSelected(Filter)).Select(FormatOption));
		}

		protected string FormatText(Color c, string text, params object[] args)
		{
			return String.Format(text, args).WrapUOHtmlColor(c, false);
		}
	}
}
