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
	public class GumpImageTime : SuperGumpEntry, IGumpEntryVector
	{
		private const string _Format1A = "{{ gumppic {0} {1} {2} }}";
		private const string _Format1B = "{{ gumppic {0} {1} {2} hue={3} }}";

		private static readonly byte[] _Layout1A = Gump.StringToBuffer("gumppic");
		private static readonly byte[] _Layout1B = Gump.StringToBuffer(" }{ gumppic");
		private static readonly byte[] _Layout1Hue = Gump.StringToBuffer(" hue=");

		private int _X, _Y;
		private TimeSpan _Value;
		private int _Hue;
		private Axis _Centering;

		public int X { get => _X; set => Delta(ref _X, value); }
		public int Y { get => _Y; set => Delta(ref _Y, value); }

		public TimeSpan Value { get => _Value; set => Delta(ref _Value, value); }

		public int Hue { get => _Hue; set => Delta(ref _Hue, value); }

		public Axis Centering { get => _Centering; set => Delta(ref _Centering, value); }

		public virtual int Width { get => _Value < TimeSpan.Zero ? 195 : 175; set { } }
		public virtual int Height { get => 28; set { } }

		public GumpImageTime(int x, int y, TimeSpan value, int hue = 0, Axis centering = Axis.None)
		{
			_X = x;
			_Y = y;
			_Value = value;
			_Hue = hue;
			_Centering = centering;
		}

		public virtual string GetString()
		{
			return String.Format(
				"{0}{1:D2}:{2:D2}:{3:D2}:{4:D2}",
				_Value < TimeSpan.Zero ? "-" : String.Empty,
				Math.Abs(_Value.Days),
				Math.Abs(_Value.Hours),
				Math.Abs(_Value.Minutes),
				Math.Abs(_Value.Seconds));
		}

		public override string Compile()
		{
			var x = _X;
			var y = _Y;

			if (_Centering.HasFlag(Axis.Horizontal))
			{
				x -= Width / 2;
			}

			if (_Centering.HasFlag(Axis.Vertical))
			{
				y -= Height / 2;
			}

			var compiled = String.Empty;
			var val = GetString();

			for (int i = 0, s = 0; i < val.Length; i++)
			{
				switch (val[i])
				{
					case '-':
					{
						if (_Hue <= 0)
						{
							compiled += String.Format(_Format1A, x + s, y + 12, 1433);
						}
						else
						{
							compiled += String.Format(_Format1B, x + s, y + 12, 1433, FixHue(_Hue));
						}

						s += 20;
					}
					continue;
					case ':':
					{
						if (_Hue <= 0)
						{
							compiled += String.Format(_Format1A, x + s, y + 7, 1433);
							compiled += String.Format(_Format1A, x + s, y + 17, 1433);
						}
						else
						{
							compiled += String.Format(_Format1B, x + s, y + 7, 1433, FixHue(_Hue));
							compiled += String.Format(_Format1B, x + s, y + 17, 1433, FixHue(_Hue));
						}

						s += 5;
					}
					continue;
					default:
					{
						if (_Hue <= 0)
						{
							compiled += String.Format(_Format1A, x + s, y, 1423 + Byte.Parse(val.Substring(i, 1)));
						}
						else
						{
							compiled += String.Format(_Format1B, x + s, y, 1423 + Byte.Parse(val.Substring(i, 1)), FixHue(_Hue));
						}

						s += 20;
					}
					continue;
				}
			}

			return compiled;
		}

		public override void AppendTo(IGumpWriter disp)
		{
			var x = _X;
			var y = _Y;

			if (_Centering.HasFlag(Axis.Horizontal))
			{
				x -= Width / 2;
			}

			if (_Centering.HasFlag(Axis.Vertical))
			{
				y -= Height / 2;
			}

			var val = GetString();

			for (int i = 0, s = 0; i < val.Length; i++)
			{
				switch (val[i])
				{
					case '-':
					{
						disp.AppendLayout(_Layout1A);
						disp.AppendLayout(x + s);
						disp.AppendLayout(y + 12);
						disp.AppendLayout(1433);

						if (_Hue > 0)
						{
							disp.AppendLayout(_Layout1Hue);
							disp.AppendLayoutNS(FixHue(_Hue));
						}

						s += 20;
					}
					continue;
					case ':':
					{
						disp.AppendLayout(_Layout1B);
						disp.AppendLayout(x + s);
						disp.AppendLayout(y + 7);
						disp.AppendLayout(1433);

						if (_Hue > 0)
						{
							disp.AppendLayout(_Layout1Hue);
							disp.AppendLayoutNS(FixHue(_Hue));
						}

						disp.AppendLayout(_Layout1B);
						disp.AppendLayout(x + s);
						disp.AppendLayout(y + 17);
						disp.AppendLayout(1433);

						if (_Hue > 0)
						{
							disp.AppendLayout(_Layout1Hue);
							disp.AppendLayoutNS(FixHue(_Hue));
						}

						s += 5;
					}
					continue;
					default:
					{
						disp.AppendLayout(i == 0 ? _Layout1A : _Layout1B);
						disp.AppendLayout(x + s);
						disp.AppendLayout(y);
						disp.AppendLayout(1423 + Byte.Parse(val.Substring(i, 1)));

						if (_Hue > 0)
						{
							disp.AppendLayout(_Layout1Hue);
							disp.AppendLayoutNS(FixHue(_Hue));
						}

						s += 20;
					}
					continue;
				}
			}
		}
	}
}