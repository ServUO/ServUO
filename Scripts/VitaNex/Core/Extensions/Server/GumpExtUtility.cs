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

using VitaNex.SuperGumps;
#endregion

namespace Server.Gumps
{
	public static class GumpExtUtility
	{
		public static bool TryGetX(this GumpEntry e, out int x)
		{
			if (e is IGumpEntryPoint)
			{
				x = ((IGumpEntryPoint)e).X;
				return true;
			}

			return e.GetPropertyValue("X", out x);
		}

		public static bool TrySetX(this GumpEntry e, int x)
		{
			if (e is IGumpEntryPoint)
			{
				((IGumpEntryPoint)e).X = x;
				return true;
			}

			return e.SetPropertyValue("X", x);
		}

		public static bool TryOffsetX(this GumpEntry e, int x)
		{

			if (TryGetX(e, out var ox))
			{
				return TrySetX(e, ox + x);
			}

			return false;
		}

		public static bool TryGetY(this GumpEntry e, out int y)
		{
			if (e is IGumpEntryPoint)
			{
				y = ((IGumpEntryPoint)e).Y;
				return true;
			}

			return e.GetPropertyValue("Y", out y);
		}

		public static bool TrySetY(this GumpEntry e, int y)
		{
			if (e is IGumpEntryPoint)
			{
				((IGumpEntryPoint)e).Y = y;
				return true;
			}

			return e.SetPropertyValue("Y", y);
		}

		public static bool TryOffsetY(this GumpEntry e, int y)
		{

			if (TryGetY(e, out var oy))
			{
				return TrySetY(e, oy + y);
			}

			return false;
		}

		public static bool TryGetWidth(this GumpEntry e, out int width)
		{
			if (e is IGumpEntrySize)
			{
				width = ((IGumpEntrySize)e).Width;
				return true;
			}

			return e.GetPropertyValue("Width", out width);
		}

		public static bool TrySetWidth(this GumpEntry e, int width)
		{
			if (e is IGumpEntrySize)
			{
				((IGumpEntrySize)e).Width = width;
				return true;
			}

			return e.SetPropertyValue("Width", width);
		}

		public static bool TryOffsetWidth(this GumpEntry e, int width)
		{

			if (TryGetWidth(e, out var ow))
			{
				return TrySetWidth(e, ow + width);
			}

			return false;
		}

		public static bool TryGetHeight(this GumpEntry e, out int height)
		{
			if (e is IGumpEntrySize)
			{
				height = ((IGumpEntrySize)e).Height;
				return true;
			}

			return e.GetPropertyValue("Height", out height);
		}

		public static bool TrySetHeight(this GumpEntry e, int height)
		{
			if (e is IGumpEntrySize)
			{
				((IGumpEntrySize)e).Height = height;
				return true;
			}

			return e.SetPropertyValue("Height", height);
		}

		public static bool TryOffsetHeight(this GumpEntry e, int height)
		{

			if (TryGetHeight(e, out var oh))
			{
				return TrySetHeight(e, oh + height);
			}

			return false;
		}

		public static bool TryGetPosition(this GumpEntry e, out int x, out int y)
		{
			if (e is IGumpEntryPoint)
			{
				x = ((IGumpEntryPoint)e).X;
				y = ((IGumpEntryPoint)e).Y;
				return true;
			}

			return e.GetPropertyValue("X", out x) & e.GetPropertyValue("Y", out y);
		}

		public static bool TrySetPosition(this GumpEntry e, int x, int y)
		{
			if (e is IGumpEntryPoint)
			{
				((IGumpEntryPoint)e).X = x;
				((IGumpEntryPoint)e).Y = y;
				return true;
			}

			return e.SetPropertyValue("X", x) & e.SetPropertyValue("Y", y);
		}

		public static bool TryOffsetPosition(this GumpEntry e, int x, int y)
		{

			if (TryGetPosition(e, out var ox, out var oy))
			{
				return TrySetPosition(e, ox + x, oy + y);
			}

			return false;
		}

		public static bool TryGetSize(this GumpEntry e, out int width, out int height)
		{
			if (e is IGumpEntrySize)
			{
				width = ((IGumpEntrySize)e).Width;
				height = ((IGumpEntrySize)e).Height;
				return true;
			}

			return e.GetPropertyValue("Width", out width) & e.GetPropertyValue("Height", out height);
		}

		public static bool TrySetSize(this GumpEntry e, int width, int height)
		{
			if (e is IGumpEntrySize)
			{
				((IGumpEntrySize)e).Width = width;
				((IGumpEntrySize)e).Height = height;
				return true;
			}

			return e.SetPropertyValue("Width", width) & e.SetPropertyValue("Height", height);
		}

		public static bool TryOffsetSize(this GumpEntry e, int width, int height)
		{

			if (TryGetSize(e, out var ow, out var oh))
			{
				return TrySetSize(e, ow + width, oh + height);
			}

			return false;
		}

		public static bool TryGetBounds(this GumpEntry e, out int x, out int y, out int width, out int height)
		{
			if (e is IGumpEntryVector)
			{
				x = ((IGumpEntryVector)e).X;
				y = ((IGumpEntryVector)e).Y;
				width = ((IGumpEntryVector)e).Width;
				height = ((IGumpEntryVector)e).Height;
				return true;
			}

			return e.GetPropertyValue("X", out x) & e.GetPropertyValue("Y", out y) & //
				   e.GetPropertyValue("Width", out width) & e.GetPropertyValue("Height", out height);
		}

		public static bool TrySetBounds(this GumpEntry e, int x, int y, int width, int height)
		{
			if (e is IGumpEntryVector)
			{
				((IGumpEntryVector)e).X = x;
				((IGumpEntryVector)e).Y = y;
				((IGumpEntryVector)e).Width = width;
				((IGumpEntryVector)e).Height = height;
				return true;
			}

			return e.SetPropertyValue("X", x) & e.SetPropertyValue("Y", y) & //
				   e.SetPropertyValue("Width", width) & e.SetPropertyValue("Height", height);
		}

		public static bool TryOffsetBounds(this GumpEntry e, int x, int y, int width, int height)
		{

			if (TryGetBounds(e, out var ox, out var oy, out var ow, out var oh))
			{
				return TrySetBounds(e, ox + x, oy + y, ow + width, oh + height);
			}

			return false;
		}

		public static Rectangle2D GetBounds(this Gump g)
		{
			int x = g.X, y = g.Y, w = 0, h = 0;

			if (g is SuperGump)
			{
				var sg = (SuperGump)g;

				x += sg.XOffset;
				y += sg.YOffset;

				if (sg.Modal)
				{
					x += sg.ModalXOffset;
					y += sg.ModalYOffset;
				}

				w = sg.OuterWidth;
				h = sg.OuterHeight;
			}
			else
			{
				foreach (var e in g.Entries)
				{
					e.TryGetPosition(out var ex, out var ey);

					e.TryGetSize(out var ew, out var eh);

					w = Math.Max(ex + ew, w);
					h = Math.Max(ey + eh, h);
				}
			}

			return new Rectangle2D(x, y, w, h);
		}
	}
}