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
	public class GumpProgress : SuperGumpEntry, IGumpEntryVector
	{
		private const string _Format1 = @"{{ htmlgump {0} {1} {2} {3} {4} 0 0 }}";

		private static readonly byte[] _Layout1A = Gump.StringToBuffer("htmlgump");
		private static readonly byte[] _Layout1B = Gump.StringToBuffer(" }{ htmlgump");

		private int _X, _Y;
		private int _Width, _Height;
		private double _Progress;
		private Direction _Direction;
		private Color _BackgroundColor, _ForegroundColor, _BorderColor;
		private int _BorderSize;

		public int X { get => _X; set => Delta(ref _X, value); }
		public int Y { get => _Y; set => Delta(ref _Y, value); }

		public int Width { get => _Width; set => Delta(ref _Width, value); }
		public int Height { get => _Height; set => Delta(ref _Height, value); }

		public double Progress { get => _Progress; set => Delta(ref _Progress, value); }

		public Direction Direction { get => _Direction; set => Delta(ref _Direction, value); }

		public Color BackgroundColor { get => _BackgroundColor; set => Delta(ref _BackgroundColor, value); }
		public Color ForegroundColor { get => _ForegroundColor; set => Delta(ref _ForegroundColor, value); }
		public Color BorderColor { get => _BorderColor; set => Delta(ref _BorderColor, value); }

		public int BorderSize { get => _BorderSize; set => Delta(ref _BorderSize, value); }

		public GumpProgress(
			int x,
			int y,
			int width,
			int height,
			double progress,
			Direction dir = Direction.Right,
			Color? background = null,
			Color? foreground = null,
			Color? border = null,
			int borderSize = 0)
		{
			_X = x;
			_Y = y;
			_Width = width;
			_Height = height;
			_Progress = progress;
			_Direction = dir;
			_BackgroundColor = background ?? Color.OrangeRed;
			_ForegroundColor = foreground ?? Color.LawnGreen;
			_BorderColor = border ?? Color.Black;
			_BorderSize = borderSize;
		}

		public bool FlowOffset(ref int x, ref int y, ref int w, ref int h)
		{
			double xo = x, yo = y, wo = w, ho = h;

			switch (_Direction & Direction.Mask)
			{
				case Direction.Up:
				{
					ho *= _Progress;
					yo = (y + h) - ho;
				}
				break;
				case Direction.North:
				{
					wo *= _Progress;
					ho *= _Progress;
					yo = (y + h) - ho;
				}
				break;
				case Direction.Right:
				{
					wo *= _Progress;
				}
				break;
				case Direction.East:
				{
					wo *= _Progress;
					ho *= _Progress;
				}
				break;
				case Direction.Down:
				{
					ho *= _Progress;
				}
				break;
				case Direction.South:
				{
					wo *= _Progress;
					ho *= _Progress;
					xo = (x + w) - wo;
				}
				break;
				case Direction.Left:
				{
					wo *= _Progress;
					xo = (x + w) - wo;
				}
				break;
				case Direction.West:
				{
					wo *= _Progress;
					ho *= _Progress;
					xo = (x + w) - wo;
					yo = (y + h) - ho;
				}
				break;
			}

			var contained = xo >= x && yo >= y && xo + wo <= x + w && yo + ho <= y + h;

			x = (int)xo;
			y = (int)yo;
			w = (int)wo;
			h = (int)ho;

			return contained;
		}

		public override string Compile()
		{
			if (IsEnhancedClient)
			{
				return String.Empty;
			}

			var compiled = Compile(_X, _Y, _Width, _Height, _BackgroundColor);

			int x = _X, y = _Y, w = _Width, h = _Height;

			if (FlowOffset(ref x, ref y, ref w, ref h))
			{
				compiled += Compile(x, y, w, h, _ForegroundColor);
			}

			compiled += Compile(_X, _Y, _Width, _BorderSize, _BorderColor);
			compiled += Compile(_X + (_Width - _BorderSize), _Y, _BorderSize, _Height, _BorderColor);
			compiled += Compile(_X, _Y + (_Height - _BorderSize), _Width, _BorderSize, _BorderColor);
			compiled += Compile(_X, _Y, _BorderSize, _Height, _BorderColor);

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

			AppendTo(disp, ref first, _X, _Y, _Width, _Height, _BackgroundColor);

			int x = _X, y = _Y, w = _Width, h = _Height;

			if (FlowOffset(ref x, ref y, ref w, ref h))
			{
				AppendTo(disp, ref first, x, y, w, h, _ForegroundColor);
			}

			AppendTo(disp, ref first, _X, _Y, _Width, _BorderSize, _BorderColor);
			AppendTo(disp, ref first, _X + (_Width - _BorderSize), _Y, _BorderSize, _Height, _BorderColor);
			AppendTo(disp, ref first, _X, _Y + (_Height - _BorderSize), _Width, _BorderSize, _BorderColor);
			AppendTo(disp, ref first, _X, _Y, _BorderSize, _Height, _BorderColor);
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