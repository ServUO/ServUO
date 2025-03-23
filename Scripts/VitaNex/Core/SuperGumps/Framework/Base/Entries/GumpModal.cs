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

using Server.Gumps;
using Server.Network;
#endregion

namespace VitaNex.SuperGumps
{
	public class GumpModal : SuperGumpEntry, IGumpEntryVector
	{
		private const string _Format1 = "{{ gumppictiled {0} {1} {2} {3} {4} }}";
		private const string _Format2 = "{{ checkertrans {0} {1} {2} {3} }}";

		private static readonly byte[] _Layout1A = Gump.StringToBuffer("gumppictiled");
		private static readonly byte[] _Layout1B = Gump.StringToBuffer(" }{ gumppictiled");
		private static readonly byte[] _Layout2A = Gump.StringToBuffer("checkertrans");
		private static readonly byte[] _Layout2B = Gump.StringToBuffer(" }{ checkertrans");

		private int _X, _Y;
		private int _Width, _Height;
		private int _GumpID;

		public int X { get => _X; set => Delta(ref _X, value); }
		public int Y { get => _Y; set => Delta(ref _Y, value); }

		public int Width { get => _Width; set => Delta(ref _Width, value); }
		public int Height { get => _Height; set => Delta(ref _Height, value); }

		public int GumpID { get => _GumpID; set => Delta(ref _GumpID, value); }

		public override bool IgnoreModalOffset => true;

		public GumpModal(int x, int y, int width, int height, int gumpID)
		{
			_X = x;
			_Y = y;

			_Width = width;
			_Height = height;

			_GumpID = gumpID;
		}

		public override string Compile()
		{
			if (_Width <= 1024 && _Height <= 786)
			{
				return Compile(_X, _Y, _Width, _Height);
			}

			var compiled = String.Empty;

			var xx = _X;
			var ww = _Width;

			while (ww > 0)
			{
				var yy = _Y;
				var hh = _Height;

				var mw = Math.Min(1024, ww);

				while (hh > 0)
				{
					var mh = Math.Min(786, hh);

					compiled += Compile(xx, yy, mw, mh);

					yy += mh;
					hh -= mh;
				}

				xx += mw;
				ww -= mw;
			}

			return compiled;
		}

		public virtual string Compile(int x, int y, int w, int h)
		{
			var compiled = String.Empty;

			if (_GumpID >= 0)
			{
				compiled += String.Format(_Format1, x, y, w, h, _GumpID);
			}

			compiled += String.Format(_Format2, x, y, w, h);

			return compiled;
		}

		public override void AppendTo(IGumpWriter disp)
		{
			if (_Width <= 1024 && _Height <= 786)
			{
				AppendTo(disp, _X, _Y, _Width, _Height);
				return;
			}

			var xx = _X;
			var ww = _Width;

			while (ww > 0)
			{
				var yy = _Y;
				var hh = _Height;

				var mw = Math.Min(1024, ww);

				while (hh > 0)
				{
					var mh = Math.Min(786, hh);

					AppendTo(disp, xx, yy, mw, mh);

					yy += mh;
					hh -= mh;
				}

				xx += mw;
				ww -= mw;
			}
		}

		public virtual void AppendTo(IGumpWriter disp, int x, int y, int w, int h)
		{
			var first = _X == x && _Y == y;

			if (_GumpID >= 0)
			{
				disp.AppendLayout(first ? _Layout1A : _Layout1B);
				disp.AppendLayout(x);
				disp.AppendLayout(y);
				disp.AppendLayout(w);
				disp.AppendLayout(h);
				disp.AppendLayout(_GumpID);

				disp.AppendLayout(_Layout2B);
				disp.AppendLayout(x);
				disp.AppendLayout(y);
				disp.AppendLayout(w);
				disp.AppendLayout(h);
			}
			else
			{
				disp.AppendLayout(first ? _Layout2A : _Layout2B);
				disp.AppendLayout(x);
				disp.AppendLayout(y);
				disp.AppendLayout(w);
				disp.AppendLayout(h);
			}
		}
	}
}