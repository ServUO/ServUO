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
using System.Drawing;

using Server;
using Server.Gumps;
using Server.Network;
#endregion

namespace VitaNex.SuperGumps
{
	public class GumpCross : SuperGumpEntry, IGumpEntryVector
	{
		private const string _Format1 = @"{{ htmlgump {0} {1} {2} {3} {4} 0 0 }}";

		private static readonly byte[] _Layout1A = Gump.StringToBuffer("htmlgump");
		private static readonly byte[] _Layout1B = Gump.StringToBuffer(" }{ htmlgump");

		private int _X, _Y;
		private int _Width, _Height;
		private int _Size, _BorderSize;
		private Color _FillColor, _BorderColor;

		public int X { get => _X; set => Delta(ref _X, value); }
		public int Y { get => _Y; set => Delta(ref _Y, value); }

		public int Width { get => _Width; set => Delta(ref _Width, value); }
		public int Height { get => _Height; set => Delta(ref _Height, value); }

		public int Size { get => _Size; set => Delta(ref _Size, value); }
		public int BorderSize { get => _BorderSize; set => Delta(ref _BorderSize, value); }

		public Color FillColor { get => _FillColor; set => Delta(ref _FillColor, value); }
		public Color BorderColor { get => _BorderColor; set => Delta(ref _BorderColor, value); }

		public GumpCross(Rectangle bounds, int size, Color color)
			: this(bounds.X, bounds.Y, bounds.Width, bounds.Height, size, color)
		{ }

		public GumpCross(Rectangle bounds, int size, Color color, bool filled)
			: this(bounds.X, bounds.Y, bounds.Width, bounds.Height, size, color, filled)
		{ }

		public GumpCross(Rectangle bounds, int size, Color color, int borderSize)
			: this(bounds.X, bounds.Y, bounds.Width, bounds.Height, size, color, borderSize)
		{ }

		public GumpCross(Rectangle bounds, int size, Color color, Color border, int borderSize)
			: this(bounds.X, bounds.Y, bounds.Width, bounds.Height, size, color, border, borderSize)
		{ }

		public GumpCross(Rectangle2D bounds, int size, Color color)
			: this(bounds.X, bounds.Y, bounds.Width, bounds.Height, size, color)
		{ }

		public GumpCross(Rectangle2D bounds, int size, Color color, bool filled)
			: this(bounds.X, bounds.Y, bounds.Width, bounds.Height, size, color, filled)
		{ }

		public GumpCross(Rectangle2D bounds, int size, Color color, int borderSize)
			: this(bounds.X, bounds.Y, bounds.Width, bounds.Height, size, color, borderSize)
		{ }

		public GumpCross(Rectangle2D bounds, int size, Color color, Color border, int borderSize)
			: this(bounds.X, bounds.Y, bounds.Width, bounds.Height, size, color, border, borderSize)
		{ }

		public GumpCross(int x, int y, int w, int h, int size, Color color)
			: this(x, y, w, h, size, color, false)
		{ }

		public GumpCross(int x, int y, int w, int h, int size, Color color, bool filled)
			: this(x, y, w, h, size, color, filled ? 0 : 1)
		{ }

		public GumpCross(int x, int y, int w, int h, int size, Color color, int borderSize)
			: this(x, y, w, h, size, borderSize <= 0 ? color : Color.Empty, borderSize <= 0 ? Color.Empty : color, borderSize)
		{ }

		public GumpCross(int x, int y, int w, int h, int size, Color color, Color border, int borderSize)
		{
			_X = x;
			_Y = y;

			_Width = w;
			_Height = h;

			_Size = size;

			_FillColor = color;
			_BorderColor = border;

			_BorderSize = borderSize;
		}

		public override string Compile()
		{
			if (IsEnhancedClient)
			{
				return String.Empty;
			}

			var compiled = String.Empty;

			var x1 = _X;
			var y1 = _Y + ((_Height - _Size) / 2);
			var w1 = _Width;
			var h1 = _Size;

			var x2 = _X + ((_Width - _Size) / 2);
			var y2 = _Y;
			var w2 = _Size;
			var h2 = _Height;

			if (!_BorderColor.IsEmpty && _BorderColor != Color.Transparent && _BorderColor != _FillColor && _BorderSize > 0)
			{
				compiled += Compile(x1, y1, w1, h1, _BorderColor);
				compiled += Compile(x2, y2, w2, h2, _BorderColor);

				x1 += _BorderSize;
				y1 += _BorderSize;
				w1 -= _BorderSize * 2;
				h1 -= _BorderSize * 2;

				x2 += _BorderSize;
				y2 += _BorderSize;
				w2 -= _BorderSize * 2;
				h2 -= _BorderSize * 2;
			}

			compiled += Compile(x1, y1, w1, h1, _FillColor);
			compiled += Compile(x2, y2, w2, h2, _FillColor);

			return compiled;
		}

		public virtual string Compile(int x, int y, int w, int h, Color color)
		{
			var text = " ";

			if (!color.IsEmpty && color != Color.Transparent)
			{
				text = text.WrapUOHtmlBG(color);
			}

			return String.Format(_Format1, x, y, w, h, Parent.Intern(text));
		}

		public override void AppendTo(IGumpWriter disp)
		{
			if (IsEnhancedClient)
			{
				AppendEmptyLayout(disp);
				return;
			}

			var x1 = _X;
			var y1 = _Y + ((_Height - _Size) / 2);
			var w1 = _Width;
			var h1 = _Size;

			var x2 = _X + ((_Width - _Size) / 2);
			var y2 = _Y;
			var w2 = _Size;
			var h2 = _Height;

			var first = true;

			if (!_BorderColor.IsEmpty && _BorderColor != Color.Transparent && _BorderColor != _FillColor && _BorderSize > 0)
			{
				AppendTo(disp, ref first, x1, y1, w1, h1, _BorderColor);
				AppendTo(disp, ref first, x2, y2, w2, h2, _BorderColor);

				x1 += _BorderSize;
				y1 += _BorderSize;
				w1 -= _BorderSize * 2;
				h1 -= _BorderSize * 2;

				x2 += _BorderSize;
				y2 += _BorderSize;
				w2 -= _BorderSize * 2;
				h2 -= _BorderSize * 2;
			}

			AppendTo(disp, ref first, x1, y1, w1, h1, _FillColor);
			AppendTo(disp, ref first, x2, y2, w2, h2, _FillColor);
		}

		public virtual void AppendTo(IGumpWriter disp, ref bool first, int x, int y, int w, int h, Color color)
		{
			var text = " ";

			if (!color.IsEmpty && color != Color.Transparent)
			{
				text = text.WrapUOHtmlBG(color);
			}

			disp.AppendLayout(first ? _Layout1A : _Layout1B);
			disp.AppendLayout(x);
			disp.AppendLayout(y);
			disp.AppendLayout(w);
			disp.AppendLayout(h);
			disp.AppendLayout(Parent.Intern(text));
			disp.AppendLayout(false);
			disp.AppendLayout(false);

			first = false;
		}
	}
}