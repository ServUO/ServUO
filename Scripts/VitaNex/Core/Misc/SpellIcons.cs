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

using Server;
using Server.Gumps;

using VitaNex.SuperGumps;
#endregion

namespace VitaNex
{
	public class SpellIconUI : SuperGump
	{
		private int _Display, _Displaying;

		public bool DisplaySmall
		{
			get => _Display == 0 || (_Display & 0x1) != 0;
			set
			{
				if (value)
				{
					_Display |= 0x1;
				}
				else
				{
					_Display &= ~0x1;
				}
			}
		}

		public bool DisplayLarge
		{
			get => _Display == 0 || (_Display & 0x2) != 0;
			set
			{
				if (value)
				{
					_Display |= 0x2;
				}
				else
				{
					_Display &= ~0x2;
				}
			}
		}

		public int Icon { get; set; }

		public Action<int> SelectHandler { get; set; }

		public bool CloseOnSelect { get; set; }

		public int BackgroundID { get; set; }

		public SpellIconUI(Mobile user, Gump parent = null, int? icon = null, Action<int> onSelect = null)
			: base(user, parent)
		{
			Icon = icon ?? -1;
			SelectHandler = onSelect;

			DisplaySmall = true;
			DisplayLarge = true;

			CanClose = true;
			CanDispose = true;
			CanMove = true;
			CanResize = false;

			BackgroundID = SupportsUltimaStore ? 40000 : 9270;
		}

		protected override void Compile()
		{
			if (_Displaying == 0x0)
			{
				if (SpellIcons.IsSmallIcon(Icon) && DisplaySmall)
				{
					_Displaying = 0x1;
				}
				else if (SpellIcons.IsLargeIcon(Icon) && DisplayLarge)
				{
					_Displaying = 0x2;
				}
				else
				{
					_Displaying = DisplaySmall ? 0x1 : DisplayLarge ? 0x2 : 0x0;
				}
			}

			if (_Displaying == 0x1 && !DisplaySmall)
			{
				_Displaying = DisplayLarge ? 0x2 : 0x0;
			}

			if (_Displaying == 0x2 && !DisplayLarge)
			{
				_Displaying = DisplaySmall ? 0x1 : 0x0;
			}

			base.Compile();
		}

		protected override bool OnBeforeSend()
		{
			return (_Displaying == 0x1 || _Displaying == 0x2) && base.OnBeforeSend();
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			int[] icons;
			int iw, ih;

			switch (_Displaying)
			{
				case 0x1:
				{
					icons = SpellIcons.SmallIcons;
					iw = SpellIcons.SmallSize.Width;
					ih = SpellIcons.SmallSize.Height;
				}
				break;

				case 0x2:
				{
					icons = SpellIcons.LargeIcons;
					iw = SpellIcons.LargeSize.Width;
					ih = SpellIcons.LargeSize.Height;
				}
				break;

				default: return;
			}

			var sqrt = (int)Math.Ceiling(Math.Sqrt(icons.Length));

			var w = 20 + (sqrt * iw);
			var h = 50 + (sqrt * ih);

			layout.Add("bg", () => AddBackground(0, 0, w, h, BackgroundID));

			layout.Add("header", () =>
			{
				var title = "Icon Selection";
				title = title.WrapUOHtmlBig();
				title = title.WrapUOHtmlColor(Color.Gold, false);

				AddHtml(15, 12, w - 130, 40, title, false, false);

				if (!DisplayLarge || !DisplaySmall)
				{
					return;
				}

				var x = 15 + (w - 30);

				if (DisplayLarge)
				{
					x -= 50;

					var c = _Displaying == 0x2 ? Color.Gold : Color.White;

					AddHtmlButton(x, 10, 50, 30, b =>
					{
						_Displaying = 0x2;
						Refresh(true);
					}, "Large".WrapUOHtmlCenter(), c, Color.Empty, c, 1);
				}

				if (DisplaySmall)
				{
					x -= 50;

					var c = _Displaying == 0x1 ? Color.Gold : Color.White;

					AddHtmlButton(x, 10, 50, 30, b =>
					{
						_Displaying = 0x1;
						Refresh(true);
					}, "Small".WrapUOHtmlCenter(), c, Color.Empty, c, 1);
				}
			});

			layout.Add("icons", () =>
			{
				int xx, yy, x, y, i = 0;

				for (yy = 0; yy < sqrt; yy++)
				{
					y = 40 + (yy * ih);

					for (xx = 0; xx < sqrt; xx++)
					{
						x = 10 + (xx * iw);

						var index = i++;

						if (!icons.InBounds(index))
						{
							continue;
						}

						var icon = icons[index];

						AddButton(x, y, icon, icon, b => SelectIcon(icon));

						if (icon == Icon)
						{
							AddRectangle(x, y, iw, ih, Color.LawnGreen, 2);
						}
					}
				}
			});
		}

		protected virtual void SelectIcon(int icon)
		{
			Icon = icon;

			if (SelectHandler != null)
			{
				SelectHandler(icon);
			}

			if (!CloseOnSelect)
			{
				Refresh(true);
			}
			else
			{
				Close();
			}
		}
	}

	public static class SpellIcons
	{
		public static readonly Size SmallSize = new Size(44, 44);
		public static readonly Size LargeSize = new Size(70, 70);

		private static readonly int[][] _Icons = new int[3][];

		public static int[] SmallIcons => _Icons[0];
		public static int[] LargeIcons => _Icons[1];
		public static int[] ItemIcons => _Icons[2];

		static SpellIcons()
		{
			var small = new HashSet<int>();
			var large = new HashSet<int>();
			var items = new HashSet<int>();

			Register(small, 2237);
			Register(small, 2240, 2305);
			Register(small, 2373, 2378);
			Register(large, 7000, 7064);
			Register(small, 20480, 20496);
			Register(small, 20736, 20745);
			Register(small, 20992, 21022);
			Register(large, 21248, 21255);
			Register(small, 21256, 21257);
			Register(small, 21280, 21287);
			Register(large, 21504, 21510);
			Register(small, 21536, 21542);
			Register(large, 21632, 21642);
			Register(small, 23000, 23015);
			Register(small, 24000, 24030);
			Register(small, 30103);
			Register(small, 30106);
			Register(small, 30109);
			Register(small, 30114);
			Register(small, 39819, 39860);
			Register(items, 8320, 8383, true);

            _Icons[0] = small.FreeToArray(true);
            _Icons[1] = large.FreeToArray(true);
            _Icons[2] = items.FreeToArray(true);
        }

		private static void Register(HashSet<int> list, int from, int to, bool items = false)
		{
			var c = to - from;

			if (c <= 0)
			{
				return;
			}

			for (var id = from; id <= to; id++)
			{
				Register(list, id, items);
			}
		}

		private static void Register(HashSet<int> list, int id, bool item = false)
		{
			var size = item ? Ultima.ArtExtUtility.GetImageSize(id) : Ultima.GumpsExtUtility.GetImageSize(id);

			if (size == SmallSize || size == LargeSize)
			{
				list.Add(id);
			}
#if DEBUG
			else
			{
				Utility.PushColor(ConsoleColor.Yellow);
				Console.WriteLine($"Warning: Invalid spell icon 0x{id:X4} ({id}) W{size.Width} H{size.Height}");
				Utility.PopColor();
			}
#endif
		}

		public static bool IsIcon(int id)
		{
			return IsItemIcon(id) || IsGumpIcon(id);
		}

        public static bool IsItemIcon(int id)
        {
            return ItemIcons?.Contains(id) ?? false;
        }

		public static bool IsGumpIcon(int id)
		{
			return IsSmallIcon(id) || IsLargeIcon(id);
		}

		public static bool IsSmallIcon(int id)
		{
			return SmallIcons.Contains(id);
		}

		public static bool IsLargeIcon(int id)
		{
			return LargeIcons.Contains(id);
		}

		public static int RandomItemIcon()
		{
			return ItemIcons.GetRandom();
		}

		public static int RandomGumpIcon()
		{
			return Utility.RandomBool() ? RandomSmallIcon() : RandomLargeIcon();
		}

		public static int RandomSmallIcon()
		{
			return SmallIcons.GetRandom();
		}

		public static int RandomLargeIcon()
		{
			return LargeIcons.GetRandom();
		}
	}
}
