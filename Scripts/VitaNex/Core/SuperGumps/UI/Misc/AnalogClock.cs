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
#endregion

namespace VitaNex.SuperGumps.UI
{
	public class AnalogClock : SuperGump
	{
		private static readonly Numeral[] _RomanNumerals = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

		public static void Initialize()
		{
			CommandUtility.Register("Clock", AccessLevel.Player, e => DisplayTo(e.Mobile));
		}

		public static void DisplayTo(Mobile user)
		{
			var roman = EnumerateInstances<AnalogClock>(user).Any(g => g != null && !g.IsDisposed && g.RomanNumerals);

			DisplayTo(user, !roman);
		}

		public static void DisplayTo(Mobile user, bool roman)
		{
			if (user != null)
			{
				new AnalogClock(user)
				{
					RomanNumerals = roman
				}.Send();
			}
		}

		public static int DefaultRadius = 40;

		public static Color DefaultNumeralsColor = Color.Gold;
		public static Color DefaultHourHandColor = Color.Gainsboro;
		public static Color DefaultMinuteHandColor = Color.Gainsboro;
		public static Color DefaultSecondHandColor = Color.OrangeRed;

		private TimeSpan? _LastTime;

		public TimeSpan Time { get; set; }

		public int Radius { get; set; }

		public bool RomanNumerals { get; set; }

		public bool DisplayNumerals { get; set; }
		public bool DisplayHourHand { get; set; }
		public bool DisplayMinuteHand { get; set; }
		public bool DisplaySecondHand { get; set; }

		public Color ColorNumerals { get; set; }
		public Color ColorHourHand { get; set; }
		public Color ColorMinuteHand { get; set; }
		public Color ColorSecondHand { get; set; }

		public bool RealTime { get; set; }

		public AnalogClock(
			Mobile user,
			Gump parent = null,
			int? x = null,
			int? y = null,
			int radius = -1,
			TimeSpan? time = null)
			: base(user, parent, x, y)
		{
			Radius = radius <= 0 ? DefaultRadius : radius;

			RomanNumerals = false;

			DisplayNumerals = true;
			DisplayHourHand = true;
			DisplayMinuteHand = true;
			DisplaySecondHand = true;

			ColorNumerals = DefaultNumeralsColor;
			ColorHourHand = DefaultHourHandColor;
			ColorMinuteHand = DefaultMinuteHandColor;
			ColorSecondHand = DefaultSecondHandColor;

			if (time != null)
			{
				Time = time.Value;
				RealTime = false;
			}
			else
			{
				Time = DateTime.Now.TimeOfDay;
				RealTime = true;
			}

			ForceRecompile = true;

			AutoRefresh = RealTime;
		}

		protected virtual void ComputeRefreshRate()
		{
			if (DisplaySecondHand)
			{
				AutoRefreshRate = TimeSpan.FromSeconds(1.0);
			}
			else if (DisplayMinuteHand)
			{
				AutoRefreshRate = TimeSpan.FromMinutes(1.0);
			}
			else if (DisplayHourHand)
			{
				AutoRefreshRate = TimeSpan.FromHours(1.0);
			}
			else
			{
				AutoRefresh = false;
			}
		}

		protected override bool OnBeforeSend()
		{
			ComputeRefreshRate();

			return base.OnBeforeSend();
		}

		protected override void OnAutoRefresh()
		{
			if (RealTime)
			{
				Time = DateTime.Now.TimeOfDay;
			}
			else if (_LastTime == Time)
			{
				_LastTime = (Time += DateTime.UtcNow - LastAutoRefresh);
			}

			ComputeRefreshRate();

			base.OnAutoRefresh();
		}

		protected virtual void GetBounds(out int x, out int y, out int w, out int h)
		{
			x = y = 15;
			w = h = Radius * 2;
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			var sup = SupportsUltimaStore;
			var ec = IsEnhancedClient;
			var bgID = ec ? 83 : sup ? 40000 : 2620;

			GetBounds(out var x, out var y, out var w, out var h);

			var c = new Point2D(x + (w / 2), y + (h / 2));

			layout.Add("clock/bg", () => AddBackground(x - 15, y - 15, w + 30, h + 30, bgID));

			if (DisplayNumerals)
			{
				CompileNumerals(layout, c);
			}

			if (DisplayHourHand)
			{
				CompileHourHand(layout, c);
			}

			if (DisplayMinuteHand)
			{
				CompileMinuteHand(layout, c);
			}

			if (DisplaySecondHand)
			{
				CompileSecondHand(layout, c);
			}
		}

		protected virtual void CompileHourHand(SuperGumpLayout layout, Point2D center)
		{
			layout.Add(
				"clock/hand/hour",
				() =>
				{
					var ha = 2.0f * Math.PI * (Time.Hours + Time.Minutes / 60.0f) / 12.0f;
					var hhp = center.Clone2D((int)(Radius * Math.Sin(ha) / 1.5f), (int)(-Radius * Math.Cos(ha) / 1.5f));

					AddLine(center, hhp, ColorHourHand, 3);
				});
		}

		protected virtual void CompileMinuteHand(SuperGumpLayout layout, Point2D center)
		{
			layout.Add(
				"clock/hand/minute",
				() =>
				{
					var ma = 2.0f * Math.PI * (Time.Minutes + Time.Seconds / 60.0f) / 60.0f;
					var mhp = center.Clone2D((int)(Radius * Math.Sin(ma)), (int)(-Radius * Math.Cos(ma)));

					AddLine(center, mhp, ColorMinuteHand, 3);
				});
		}

		protected virtual void CompileSecondHand(SuperGumpLayout layout, Point2D center)
		{
			layout.Add(
				"clock/hand/second",
				() =>
				{
					var sa = 2.0f * Math.PI * Time.Seconds / 60.0f;
					var shp = center.Clone2D((int)(Radius * Math.Sin(sa)), (int)(-Radius * Math.Cos(sa)));

					AddLine(center, shp, ColorSecondHand, 1);
				});
		}

		protected virtual void CompileNumerals(SuperGumpLayout layout, Point2D center)
		{
			for (var i = 1; i <= 12; i++)
			{
				CompileNumeral(layout, center, i);
			}
		}

		protected virtual void CompileNumeral(SuperGumpLayout layout, Point2D center, int num)
		{
			layout.Add(
				"clock/numeral/" + num,
				() =>
				{
					var x = center.X - (RomanNumerals ? 20 : 10);
					x += (int)(-1 * (Radius * Math.Cos((Math.PI / 180.0f) * (num * 30 + 90))));

					var y = center.Y - 10;
					y += (int)(-1 * (Radius * Math.Sin((Math.PI / 180.0f) * (num * 30 + 90))));

					var n = GetNumeralString(num).WrapUOHtmlCenter().WrapUOHtmlColor(ColorNumerals);

					AddHtml(x, y, RomanNumerals ? 40 : 20, 40, n, false, false);
				});
		}

		protected virtual string GetNumeralString(int num)
		{
			return (RomanNumerals ? GetRomanNumeral(num) : num.ToString()).WrapUOHtmlBold();
		}

		protected virtual string GetRomanNumeral(int num)
		{
			return _RomanNumerals[num - 1];
		}
	}
}