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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

using Server;
using Server.Gumps;

using Ultima;
#endregion

namespace VitaNex.SuperGumps.UI
{
	public class HueSelector : SuperGump, IEnumerable<int>
	{
		public static int DefaultIcon = 4650;

		private readonly Grid<int> _HueGrid = new Grid<int>
		{
			DefaultValue = -1
		};

		private int[] _Hues = new int[0];

		public virtual int ScrollX { get; set; }
		public virtual int ScrollY { get; set; }

		public virtual int ScrollWidth { get; set; }
		public virtual int ScrollHeight { get; set; }

		public virtual Action<int> AcceptCallback { get; set; }
		public virtual Action<int> CancelCallback { get; set; }

		public virtual int this[int idx] => idx >= 0 && idx < _Hues.Length ? _Hues[idx] : -1;

		public virtual int this[int x, int y] => x >= 0 && x < _HueGrid.Width && y >= 0 && y < _HueGrid.Height ? _HueGrid[x, y] : -1;

		public virtual int[] Hues { get => _Hues; set => SetHues(value); }

		public virtual int Selected { get; set; }

		public virtual string Title { get; set; }

		public virtual int PreviewIcon { get; set; }

		public HueSelector(
			Mobile user,
			Gump parent = null,
			int? x = null,
			int? y = null,
			string title = "Color Chart",
			int[] hues = null,
			int selected = -1,
			Action<int> accept = null,
			Action<int> cancel = null)
			: base(user, parent, x, y)
		{
			CanDispose = false;

			ScrollX = ScrollY = 0;
			ScrollWidth = ScrollHeight = 5;

			Selected = selected;
			AcceptCallback = accept;
			CancelCallback = cancel;

			Title = title ?? "Color Chart";

			PreviewIcon = DefaultIcon;

			SetHues(hues);
		}

		public void SetHues(params int[] hues)
		{
			hues = hues ?? new int[0];

			if (hues.Length == 0)
			{
				_Hues = hues;
			}
			else
			{
				var list = new List<int>(hues);

				list.Prune();

				_Hues = list.FreeToArray(true);
			}

			_Hues.Sort();

			var size = (int)Math.Ceiling(Math.Sqrt(_Hues.Length));

			_HueGrid.DefaultValue = -1;
			_HueGrid.Resize(size, size);

			int i = 0, gx, gy;

			for (gy = 0; gy < size; gy++)
			{
				for (gx = 0; gx < size; gx++)
				{
					_HueGrid.SetContent(gx, gy, i < _Hues.Length ? _Hues[i] : -1);

					++i;
				}
			}

			if (IsOpen)
			{
				Refresh(true);
			}
		}

		protected override void Compile()
		{
			if (_Hues.Length <= 0)
			{
				Selected = -1;
			}
			else if (!_Hues.Contains(Selected))
			{
				Selected = _Hues[0];
			}

			base.Compile();
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			var sup = SupportsUltimaStore;
			var ec = IsEnhancedClient;
			var pad = ec || sup ? 15 : 10;
			var bgID = ec ? 83 : sup ? 40000 : 2620;

			var w = 44 + (44 * ScrollWidth);
			var h = 44 + (44 * ScrollHeight);

			w = Math.Max(176, w);
			h = Math.Max(176, h);

			w += pad * 2;
			h += pad * 2;

			/* Layout:
			 *  ___________
			 * [___________|<]
			 * |  |O O O O |^|
			 * |  |O O O O | |
			 * |  |O O O O | |
			 * |  |________|v|
			 * |__|<______>|>]
			 */

			var width = 135 + w;
			var height = 70 + h;

			layout.Add(
				"bg",
				() =>
				{
					AddBackground(0, 0, width, height, bgID);
					AddBackground(0, 35, 100, height - 35, bgID);
					AddBackground(100, 35, width - 135, height - 70, bgID);
					AddImageTiled(100 + pad, 35 + pad, width - (135 + (pad * 2)), height - (70 + (pad * 2)), bgID + 4);
				});

			layout.Add(
				"title",
				() =>
				{
					var title = Title;

					title = title.WrapUOHtmlBig();
					title = title.WrapUOHtmlColor(Color.Gold, false);

					AddHtml(pad, pad, width - (pad * 2), 40, title, false, false);
				});

			layout.Add(
				"preview",
				() =>
				{
					var label = GetHueLabel(Selected);

					label = label.WrapUOHtmlBig();
					label = label.WrapUOHtmlCenter();
					label = label.WrapUOHtmlColor(Color.Gold, false);

					AddHtml(pad, 35 + pad, 100 - (pad * 2), 40, label, false, false);

					if (PreviewIcon >= 0)
					{
						var s = ArtExtUtility.GetImageSize(PreviewIcon);

						if (Selected > 0)
						{
							AddItem((100 - s.Width) / 2, 35 + pad + 40, PreviewIcon, Selected);
						}
						else
						{
							AddItem((100 - s.Width) / 2, 35 + pad + 40, PreviewIcon);
						}
					}
				});

			CompileEntries(layout, 100 + pad, 35 + pad);

			layout.Add(
				"scrollY",
				() =>
				{
					var value = Math.Max(0, (_HueGrid.Height + 1) - ScrollHeight);

					AddScrollbarV(
						width - (16 + pad),
						35 + pad,
						value,
						ScrollY,
						b => ScrollUp(),
						b => ScrollDown(),
						new Rectangle(0, 20, 16, height - (110 + (pad * 2))),
						new Rectangle(0, 0, 16, 16),
						new Rectangle(0, 20 + (height - (110 + (pad * 2))) + 4, 16, 16),
						Tuple.Create(9354, 9304),
						Tuple.Create(5604, 5600, 5604),
						Tuple.Create(5606, 5602, 5606));
				});

			layout.Add(
				"scrollX",
				() =>
				{
					var value = Math.Max(0, (_HueGrid.Width + 1) - ScrollWidth);

					AddScrollbarH(
						100 + pad,
						height - (16 + pad),
						value,
						ScrollX,
						b => ScrollLeft(),
						b => ScrollRight(),
						new Rectangle(20, 0, width - (175 + (pad * 2)), 16),
						new Rectangle(0, 0, 16, 16),
						new Rectangle(20 + (width - (175 + (pad * 2))) + 4, 0, 16, 16),
						Tuple.Create(9354, 9304),
						Tuple.Create(5607, 5603, 5607),
						Tuple.Create(5605, 5601, 5605));
				});

			layout.Add(
				"cancel",
				() =>
				{
					AddButton(width - (15 + pad), pad, 11410, 11411, OnCancel);
					AddTooltip(1006045);
				});

			layout.Add(
				"accept",
				() =>
				{
					AddButton(width - (15 + pad), height - (15 + pad), 11400, 11401, OnAccept);
					AddTooltip(1006044);
				});
		}

		protected virtual void CompileEntries(SuperGumpLayout layout, int x, int y)
		{
			var cells = _HueGrid.SelectCells(ScrollX, ScrollY, ScrollWidth, ScrollHeight);

			int i = 0, gx, gy, xx, yy;

			for (gy = 0, yy = y; gy < ScrollHeight; gy++, yy += 44)
			{
				for (gx = 0, xx = x; gx < ScrollWidth; gx++, xx += 44)
				{
					CompileEntry(layout, xx, yy, gx, gy, i++, cells.InBounds(gx, gy) ? cells[gx][gy] : -1);
				}
			}
		}

		protected virtual void CompileEntry(SuperGumpLayout layout, int x, int y, int gx, int gy, int idx, int hue)
		{
			if (hue <= -1)
			{
				return;
			}

			var sup = SupportsUltimaStore;
			var ec = IsEnhancedClient;
			var fillID = ec ? 87 : sup ? 40004 : 2624;

			layout.Add(
				"entry/" + idx,
				() =>
				{
					const int itemID = 4011;

					var s = Selected == hue;

					AddButton(x, y, 2240, 2240, b => SelectEntry(gx, gy, idx, hue));
					AddImageTiled(x, y, 44, 44, fillID);

					var o = ArtExtUtility.GetImageOffset(itemID);

					if (s)
					{
						AddItem(x + o.X, y + o.Y + 5, itemID, 2050);
						AddItem(x + o.X, y + o.Y + 2, itemID, 2999);
					}
					else if (sup)
					{
						AddItem(x + o.X, y + o.Y + 5, itemID, 2999);
					}

					if (hue > 0)
					{
						AddItem(x + o.X, y + o.Y, itemID, hue);
					}
					else
					{
						AddItem(x + o.X, y + o.Y, itemID);
					}
				});
		}

		protected virtual void SelectEntry(int gx, int gy, int idx, int hue)
		{
			Selected = hue;

			OnSelected(gx, gy, idx, hue);
		}

		protected virtual void OnSelected(int gx, int gy, int idx, int hue)
		{
			Refresh(true);
		}

		public virtual void OnCancel(GumpButton b)
		{
			if (CancelCallback != null)
			{
				CancelCallback(Selected);
			}
		}

		public virtual void OnAccept(GumpButton b)
		{
			if (AcceptCallback != null)
			{
				AcceptCallback(Selected);
			}
		}

		public virtual void ScrollLeft()
		{
			ScrollX--;

			Refresh(true);
		}

		public virtual void ScrollRight()
		{
			ScrollX++;

			Refresh(true);
		}

		public virtual void ScrollUp()
		{
			ScrollY--;

			Refresh(true);
		}

		public virtual void ScrollDown()
		{
			ScrollY++;

			Refresh(true);
		}

		public virtual string GetHueLabel(int hue)
		{
			return hue <= 0 ? (Utility.RandomBool() ? "Cillit Bang" : "Industrial Bleach") : "N°. " + hue;
		}

		public virtual IEnumerator<int> GetEnumerator()
		{
			return _Hues.GetEnumerator<int>();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}