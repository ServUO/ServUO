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
	public class GumpRectangle : SuperGumpEntry, IGumpEntryVector
	{
		private const string _Format1 = @"{{ htmlgump {0} {1} {2} {3} {4} 0 0 }}";

		private static readonly byte[] _Layout1A = Gump.StringToBuffer("htmlgump");
		private static readonly byte[] _Layout1B = Gump.StringToBuffer(" }{ htmlgump");

		private int _X, _Y;
		private int _Width, _Height;
		private int _BorderSize;
		private Color _FillColor, _BorderColor;

		public int X { get => _X; set => Delta(ref _X, value); }
		public int Y { get => _Y; set => Delta(ref _Y, value); }

		public int Width { get => _Width; set => Delta(ref _Width, value); }
		public int Height { get => _Height; set => Delta(ref _Height, value); }

		public Color FillColor { get => _FillColor; set => Delta(ref _FillColor, value); }
		public Color BorderColor { get => _BorderColor; set => Delta(ref _BorderColor, value); }

		public int BorderSize { get => _BorderSize; set => Delta(ref _BorderSize, value); }

		public GumpRectangle(Rectangle bounds, Color color)
			: this(bounds.X, bounds.Y, bounds.Width, bounds.Height, color)
		{ }

		public GumpRectangle(Rectangle bounds, Color color, bool filled)
			: this(bounds.X, bounds.Y, bounds.Width, bounds.Height, color, filled)
		{ }

		public GumpRectangle(Rectangle bounds, Color color, int borderSize)
			: this(bounds.X, bounds.Y, bounds.Width, bounds.Height, color, borderSize)
		{ }

		public GumpRectangle(Rectangle bounds, Color color, Color border, int borderSize)
			: this(bounds.X, bounds.Y, bounds.Width, bounds.Height, color, border, borderSize)
		{ }

		public GumpRectangle(Rectangle2D bounds, Color color)
			: this(bounds.X, bounds.Y, bounds.Width, bounds.Height, color)
		{ }

		public GumpRectangle(Rectangle2D bounds, Color color, bool filled)
			: this(bounds.X, bounds.Y, bounds.Width, bounds.Height, color, filled)
		{ }

		public GumpRectangle(Rectangle2D bounds, Color color, int borderSize)
			: this(bounds.X, bounds.Y, bounds.Width, bounds.Height, color, borderSize)
		{ }

		public GumpRectangle(Rectangle2D bounds, Color color, Color border, int borderSize)
			: this(bounds.X, bounds.Y, bounds.Width, bounds.Height, color, border, borderSize)
		{ }

		public GumpRectangle(int x, int y, int w, int h, Color color)
			: this(x, y, w, h, color, false)
		{ }

		public GumpRectangle(int x, int y, int w, int h, Color color, bool filled)
			: this(x, y, w, h, color, filled ? 0 : 1)
		{ }

		public GumpRectangle(int x, int y, int w, int h, Color color, int borderSize)
			: this(x, y, w, h, borderSize <= 0 ? color : Color.Empty, borderSize <= 0 ? Color.Empty : color, borderSize)
		{ }

		public GumpRectangle(int x, int y, int w, int h, Color color, Color border, int borderSize)
		{
			_X = x;
			_Y = y;

			_Width = w;
			_Height = h;

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

			var b = Math.Max(0, _BorderSize);

			if (_BorderColor.IsEmpty || _BorderColor == Color.Transparent || _BorderColor == _FillColor || b <= 0)
			{
				b = 0;
			}
			else
			{
				compiled += Compile(_X, _Y, _Width, b, _BorderColor);
				compiled += Compile(_X + (_Width - b), _Y, b, _Height, _BorderColor);
				compiled += Compile(_X, _Y + (_Height - b), _Width, b, _BorderColor);
				compiled += Compile(_X, _Y, b, _Height, _BorderColor);
			}

			compiled += Compile(_X + b, _Y + b, _Width - (b * 2), _Height - (b * 2), _FillColor);

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

			var first = true;

			var b = Math.Max(0, _BorderSize);

			if (_BorderColor.IsEmpty || _BorderColor == Color.Transparent || _BorderColor == _FillColor || b <= 0)
			{
				b = 0;
			}
			else
			{
				AppendTo(disp, ref first, _X, _Y, _Width, b, _BorderColor);
				AppendTo(disp, ref first, _X + (_Width - b), _Y, b, _Height, _BorderColor);
				AppendTo(disp, ref first, _X, _Y + (_Height - b), _Width, b, _BorderColor);
				AppendTo(disp, ref first, _X, _Y, b, _Height, _BorderColor);
			}

			AppendTo(disp, ref first, _X + b, _Y + b, _Width - (b * 2), _Height - (b * 2), _FillColor);
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