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
using Server.Network;

using Ultima;
#endregion

namespace VitaNex.SuperGumps
{
	public class GumpCursor : SuperGumpEntry, IGumpEntryVector
	{
		private static readonly Dictionary<UOCursor, Size> _SizeCache;

		static GumpCursor()
		{
			var cursors = default(UOCursor).EnumerateValues<UOCursor>(false);

			_SizeCache = cursors.ToDictionary(c => c, c => ArtExtUtility.GetImageSize((int)c));

			_SizeCache[UOCursor.None] = Size.Empty;
		}

		private const string _Format0 = "{{ tooltip {0} }}";
		private const string _Format1 = "{{ tilepic {0} {1} {2} }}";
		private const string _Format2 = "{{ tilepichue {0} {1} {2} {3} }}";
		private const string _Format3 = "{{ gumppictiled {0} {1} {2} {3} {4} }}";
		private const string _Format4 = "{{ checkertrans {0} {1} {2} {3} }}";

		private static readonly byte[] _Layout0 = Gump.StringToBuffer("tooltip");
		private static readonly byte[] _Layout1 = Gump.StringToBuffer(" }{ tilepic");
		private static readonly byte[] _Layout2 = Gump.StringToBuffer(" }{ tilepichue");
		private static readonly byte[] _Layout3A = Gump.StringToBuffer("gumppictiled");
		private static readonly byte[] _Layout3B = Gump.StringToBuffer(" }{ gumppictiled");
		private static readonly byte[] _Layout4A = Gump.StringToBuffer("checkertrans");
		private static readonly byte[] _Layout4B = Gump.StringToBuffer(" }{ checkertrans");

		private int _X, _Y;
		private UOCursor _Cursor;
		private int _Hue;
		private int _Tile;
		private int _Width;
		private int _Height;

		public int X { get => _X; set => Delta(ref _X, value); }
		public int Y { get => _Y; set => Delta(ref _Y, value); }

		public UOCursor Cursor { get => _Cursor; set => Delta(ref _Cursor, value); }

		public int Hue { get => _Hue; set => Delta(ref _Hue, value); }

		public int Tile { get => _Tile; set => Delta(ref _Tile, value); }

		public int Width
		{
			get
			{
				if (_Width <= 0)
				{
					return _SizeCache[_Cursor].Width;
				}

				return _Width;
			}
			set => Delta(ref _Width, value);
		}

		public int Height
		{
			get
			{
				if (_Height <= 0)
				{
					return _SizeCache[_Cursor].Height;
				}

				return _Height;
			}
			set => Delta(ref _Height, value);
		}

		public GumpCursor(int x, int y, UOCursor cursor)
			: this(x, y, cursor, 0)
		{ }

		public GumpCursor(int x, int y, UOCursor cursor, int hue)
			: this(x, y, cursor, hue, 0)
		{ }

		public GumpCursor(int x, int y, UOCursor cursor, int hue, int bgID)
		{
			_X = x;
			_Y = y;
			_Cursor = cursor;
			_Hue = hue;
			_Tile = bgID;
		}

		public override string Compile()
		{
			var x = X;
			var y = Y;
			var w = Width;
			var h = Height;
			var t = Tile;
			var c = (int)Cursor;

			if (c == 0 || w * h <= 0)
			{
				return String.Format(_Format0, 0);
			}

			var compiled = String.Empty;

			if (t > 0)
			{
				compiled += String.Format(_Format3, x, y, w, h, t);
			}
			else
			{
				compiled += String.Format(_Format4, x, y, w, h);
			}

			if (Hue <= 0)
			{
				compiled += String.Format(_Format1, x, y, c);
			}
			else
			{
				compiled += String.Format(_Format2, x, y, c, FixHue(Hue, true));
			}

			const int bw = 2;

			if (t > 0)
			{
				compiled += String.Format(_Format3, x, y, w, bw, t); // T
				compiled += String.Format(_Format3, x, y, bw, h, t); // L
				compiled += String.Format(_Format3, x, y + (h - bw), w, bw, t); // B
				compiled += String.Format(_Format3, x + (w - bw), y, bw, h, t); // R
			}
			else
			{
				compiled += String.Format(_Format4, x, y, w, bw); // T
				compiled += String.Format(_Format4, x, y, bw, h); // L
				compiled += String.Format(_Format4, x, y + (h - bw), w, bw); // B
				compiled += String.Format(_Format4, x + (w - bw), y, bw, h); // R
			}

			return compiled;
		}

		public override void AppendTo(IGumpWriter disp)
		{
			var x = X;
			var y = Y;
			var w = Width;
			var h = Height;
			var t = Tile;
			var c = (int)Cursor;

			if (c == 0 || w * h <= 0)
			{
				disp.AppendLayout(_Layout0);
				disp.AppendLayout(0);

				return;
			}

			if (t > 0)
			{
				disp.AppendLayout(_Layout3A);
				disp.AppendLayout(x);
				disp.AppendLayout(y);
				disp.AppendLayout(w);
				disp.AppendLayout(h);
				disp.AppendLayout(t);
			}
			else
			{
				disp.AppendLayout(_Layout4A);
				disp.AppendLayout(x);
				disp.AppendLayout(y);
				disp.AppendLayout(w);
				disp.AppendLayout(h);
			}

			disp.AppendLayout(Hue <= 0 ? _Layout1 : _Layout2);
			disp.AppendLayout(x);
			disp.AppendLayout(y);
			disp.AppendLayout(c);

			if (Hue > 0)
			{
				disp.AppendLayout(FixHue(Hue, true));
			}

			const int bw = 2;

			if (t > 0)
			{
				disp.AppendLayout(_Layout3B);
				disp.AppendLayout(x);
				disp.AppendLayout(y);
				disp.AppendLayout(w);
				disp.AppendLayout(bw);
				disp.AppendLayout(t);

				disp.AppendLayout(_Layout3B);
				disp.AppendLayout(x);
				disp.AppendLayout(y);
				disp.AppendLayout(bw);
				disp.AppendLayout(h);
				disp.AppendLayout(t);

				disp.AppendLayout(_Layout3B);
				disp.AppendLayout(x);
				disp.AppendLayout(y + (h - bw));
				disp.AppendLayout(w);
				disp.AppendLayout(bw);
				disp.AppendLayout(t);

				disp.AppendLayout(_Layout3B);
				disp.AppendLayout(x + (w - bw));
				disp.AppendLayout(y);
				disp.AppendLayout(bw);
				disp.AppendLayout(h);
				disp.AppendLayout(t);
			}
			else
			{
				disp.AppendLayout(_Layout4B);
				disp.AppendLayout(x);
				disp.AppendLayout(y);
				disp.AppendLayout(w);
				disp.AppendLayout(bw);

				disp.AppendLayout(_Layout4B);
				disp.AppendLayout(x);
				disp.AppendLayout(y);
				disp.AppendLayout(bw);
				disp.AppendLayout(h);

				disp.AppendLayout(_Layout4B);
				disp.AppendLayout(x);
				disp.AppendLayout(y + (h - bw));
				disp.AppendLayout(w);
				disp.AppendLayout(bw);

				disp.AppendLayout(_Layout4B);
				disp.AppendLayout(x + (w - bw));
				disp.AppendLayout(y);
				disp.AppendLayout(bw);
				disp.AppendLayout(h);
			}
		}
	}
}