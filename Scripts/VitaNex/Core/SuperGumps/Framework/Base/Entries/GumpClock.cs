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
using System.Linq;

using Server;
using Server.Gumps;
using Server.Network;
#endregion

namespace VitaNex.SuperGumps
{
	public class GumpClock : SuperGumpEntry, IGumpEntryVector
	{
		private const string _Format1A = "{{ gumppic {0} {1} {2} }}";
		private const string _Format1B = "{{ gumppic {0} {1} {2} hue={3} }}";
		private const string _Format2 = "{{ htmlgump {0} {1} {2} {3} {4} 0 0 }}";

		private static readonly byte[] _Layout1A = Gump.StringToBuffer("gumppic");
		private static readonly byte[] _Layout1B = Gump.StringToBuffer(" }{ gumppic");
		private static readonly byte[] _Layout1Hue = Gump.StringToBuffer(" hue=");
		private static readonly byte[] _Layout2A = Gump.StringToBuffer("htmlgump");
		private static readonly byte[] _Layout2B = Gump.StringToBuffer(" }{ htmlgump");

		private int _X;
		private int _Y;

		private DateTime _Time;

		private bool _Background;
		private int _BackgroundHue;

		private bool _Face;
		private int _FaceHue;

		private bool _Numerals;

		private bool _Numbers;
		private Color _NumbersColor;

		private bool _Hours;
		private Color _HoursColor;

		private bool _Minutes;
		private Color _MinutesColor;

		private bool _Seconds;
		private Color _SecondsColor;

		public int X { get => _X; set => Delta(ref _X, value); }
		public int Y { get => _Y; set => Delta(ref _Y, value); }

		public DateTime Time { get => _Time; set => Delta(ref _Time, value); }

		public bool Background { get => _Background; set => Delta(ref _Background, value); }
		public int BackgroundHue { get => _BackgroundHue; set => Delta(ref _BackgroundHue, value); }

		public bool Face { get => _Face; set => Delta(ref _Face, value); }
		public int FaceHue { get => _FaceHue; set => Delta(ref _FaceHue, value); }

		public bool Numerals { get => _Numerals; set => Delta(ref _Numerals, value); }

		public bool Numbers { get => _Numbers; set => Delta(ref _Numbers, value); }
		public Color NumbersColor { get => _NumbersColor; set => Delta(ref _NumbersColor, value); }

		public bool Hours { get => _Hours; set => Delta(ref _Hours, value); }
		public Color HoursColor { get => _HoursColor; set => Delta(ref _HoursColor, value); }

		public bool Minutes { get => _Minutes; set => Delta(ref _Minutes, value); }
		public Color MinutesColor { get => _MinutesColor; set => Delta(ref _MinutesColor, value); }

		public bool Seconds { get => _Seconds; set => Delta(ref _Seconds, value); }
		public Color SecondsColor { get => _SecondsColor; set => Delta(ref _SecondsColor, value); }

		public virtual int Width { get => 80; set { } }
		public virtual int Height { get => 80; set { } }

		public GumpClock(
			int x,
			int y,
			DateTime time,
			bool background = true,
			int backgroundHue = 900,
			bool face = true,
			int faceHue = 900,
			bool numerals = false,
			bool numbers = true,
			Color? numbersColor = null,
			bool hours = true,
			Color? hoursColor = null,
			bool minutes = true,
			Color? minutesColor = null,
			bool seconds = true,
			Color? secondsColor = null)
		{
			_X = x;
			_Y = y;
			_Time = time;
			_Background = background;
			_BackgroundHue = backgroundHue;
			_Face = face;
			_FaceHue = faceHue;
			_Numerals = numerals;
			_Numbers = numbers;
			_NumbersColor = numbersColor ?? Color.Gold;
			_Hours = hours;
			_HoursColor = hoursColor ?? Color.Gainsboro;
			_Minutes = minutes;
			_MinutesColor = minutesColor ?? Color.Gainsboro;
			_Seconds = seconds;
			_SecondsColor = secondsColor ?? Color.Red;
		}

		public override string Compile()
		{
			var compiled = String.Empty;

			if (_Background)
			{
				if (_BackgroundHue <= 0)
				{
					compiled += String.Format(_Format1A, _X, _Y, 1417);
				}
				else
				{
					compiled += String.Format(_Format1B, _X, _Y, 1417, FixHue(_BackgroundHue));
				}
			}

			if (_Face)
			{
				if (_FaceHue <= 0)
				{
					compiled += String.Format(_Format1A, _X + 33, _Y + 33, 1210);
				}
				else
				{
					compiled += String.Format(_Format1B, _X + 33, _Y + 33, 1210, FixHue(_FaceHue));
				}
			}

			if (_Numbers)
			{
				for (var number = 1; number <= 12; number++)
				{
					compiled += Compile(number, _NumbersColor);
				}
			}

			var center = new Point2D(_X + 40, _Y + 40);

			if (_Hours)
			{
				var ha = 2.0f * Math.PI * (_Time.Hour + _Time.Minute / 60.0f) / 12.0f;
				var hl = center.GetLine2D(center.Clone2D((int)(40 * Math.Sin(ha) / 1.5f), (int)(-40 * Math.Cos(ha) / 1.5f)));

				compiled += Compile(hl, _Minutes && _Seconds ? 2 : 1, _HoursColor);
			}

			if (_Minutes)
			{
				var ma = 2.0f * Math.PI * (_Time.Minute + _Time.Second / 60.0f) / 60.0f;
				var ml = center.GetLine2D(center.Clone2D((int)(40 * Math.Sin(ma)), (int)(-40 * Math.Cos(ma))));

				compiled += Compile(ml, _Seconds ? 2 : 1, _MinutesColor);
			}

			if (_Seconds)
			{
				var sa = 2.0f * Math.PI * _Time.Second / 60.0f;
				var sl = center.GetLine2D(center.Clone2D((int)(40 * Math.Sin(sa)), (int)(-40 * Math.Cos(sa))));

				compiled += Compile(sl, 1, _SecondsColor);
			}

			if (String.IsNullOrWhiteSpace(compiled))
			{
				compiled = String.Format(_Format2, _X, _Y, Width, Height, Parent.Intern(" ".WrapUOHtmlBG(Color.Transparent)));
			}

			return compiled;
		}

		public virtual string Compile(int number, Color color)
		{
			string n;

			if (_Numerals)
			{
				n = (Numeral)number;
			}
			else
			{
				n = number.ToString();
			}

			var x = (_X + 30) + (int)(-1 * (40 * Math.Cos((Math.PI / 180.0f) * (number * 30 + 90))));
			var y = (_Y + 30) + (int)(-1 * (40 * Math.Sin((Math.PI / 180.0f) * (number * 30 + 90))));
			var text = Parent.Intern(n.WrapUOHtmlBig().WrapUOHtmlCenter().WrapUOHtmlColor(color, false));

			return String.Format(_Format2, x, y, 20, 40, text);
		}

		public virtual string Compile(Point2D[] line, int size, Color color)
		{
			var offset = size / 3;
			var bgColor = Parent.Intern(" ".WrapUOHtmlBG(color));

			return String.Join(
				String.Empty,
				line.Select(p => String.Format(_Format2, p.X - offset, p.Y - offset, size, size, bgColor)));
		}

		public override void AppendTo(IGumpWriter disp)
		{
			var first = true;

			if (_Background)
			{
				disp.AppendLayout(_Layout1A);
				disp.AppendLayout(_X);
				disp.AppendLayout(_Y);
				disp.AppendLayout(1417);

				if (_BackgroundHue > 0)
				{
					disp.AppendLayout(_Layout1Hue);
					disp.AppendLayoutNS(FixHue(_BackgroundHue));
				}

				first = false;
			}

			if (_Face)
			{
				disp.AppendLayout(first ? _Layout1A : _Layout1B);
				disp.AppendLayout(_X + 33);
				disp.AppendLayout(_Y + 33);
				disp.AppendLayout(1210);

				if (_FaceHue > 0)
				{
					disp.AppendLayout(_Layout1Hue);
					disp.AppendLayoutNS(FixHue(_FaceHue));
				}

				first = false;
			}

			if (_Numbers)
			{
				for (var number = 1; number <= 12; number++)
				{
					AppendTo(disp, ref first, number, _NumbersColor);
				}
			}

			var center = new Point2D(_X + 40, _Y + 40);

			if (_Hours)
			{
				var ha = 2.0f * Math.PI * (_Time.Hour + _Time.Minute / 60.0f) / 12.0f;
				var hl = center.GetLine2D(center.Clone2D((int)(40 * Math.Sin(ha) / 1.5f), (int)(-40 * Math.Cos(ha) / 1.5f)));

				AppendTo(disp, ref first, hl, _Minutes && _Seconds ? 2 : 1, _HoursColor);
			}

			if (_Minutes)
			{
				var ma = 2.0f * Math.PI * (_Time.Minute + _Time.Second / 60.0f) / 60.0f;
				var ml = center.GetLine2D(center.Clone2D((int)(40 * Math.Sin(ma)), (int)(-40 * Math.Cos(ma))));

				AppendTo(disp, ref first, ml, _Seconds ? 2 : 1, _MinutesColor);
			}

			if (_Seconds)
			{
				var sa = 2.0f * Math.PI * _Time.Second / 60.0f;
				var sl = center.GetLine2D(center.Clone2D((int)(40 * Math.Sin(sa)), (int)(-40 * Math.Cos(sa))));

				AppendTo(disp, ref first, sl, 1, _SecondsColor);
			}

			if (first)
			{
				disp.AppendLayout(_Layout2A);
				disp.AppendLayout(_X);
				disp.AppendLayout(_Y);
				disp.AppendLayout(Width);
				disp.AppendLayout(Height);
				disp.AppendLayout(Parent.Intern(" ".WrapUOHtmlBG(Color.Transparent)));
				disp.AppendLayout(false);
				disp.AppendLayout(false);
			}
		}

		public virtual void AppendTo(IGumpWriter disp, ref bool first, int number, Color color)
		{
			string n;

			if (_Numerals)
			{
				n = (Numeral)number;
			}
			else
			{
				n = number.ToString();
			}

			var x = (_X + 30) + (int)(-1 * (40 * Math.Cos((Math.PI / 180.0f) * (number * 30 + 90))));
			var y = (_Y + 30) + (int)(-1 * (40 * Math.Sin((Math.PI / 180.0f) * (number * 30 + 90))));
			var text = Parent.Intern(n.WrapUOHtmlBig().WrapUOHtmlCenter().WrapUOHtmlColor(color, false));

			disp.AppendLayout(first ? _Layout2A : _Layout2B);
			disp.AppendLayout(x);
			disp.AppendLayout(y);
			disp.AppendLayout(20);
			disp.AppendLayout(40);
			disp.AppendLayout(text);
			disp.AppendLayout(false);
			disp.AppendLayout(false);

			first = false;
		}

		public virtual void AppendTo(IGumpWriter disp, ref bool first, Point2D[] line, int size, Color color)
		{
			var offset = size / 3;
			var bgColor = Parent.Intern(" ".WrapUOHtmlBG(color));

			foreach (var p in line)
			{
				disp.AppendLayout(first ? _Layout2A : _Layout2B);
				disp.AppendLayout(p.X - offset);
				disp.AppendLayout(p.Y - offset);
				disp.AppendLayout(size);
				disp.AppendLayout(size);
				disp.AppendLayout(bgColor);
				disp.AppendLayout(false);
				disp.AppendLayout(false);

				first = false;
			}
		}
	}
}
