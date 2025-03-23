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
#endregion

namespace VitaNex.SuperGumps.UI
{
	public enum ProgressBarFlow
	{
		Up = Direction.Up,
		UpRight = Direction.North,
		Right = Direction.Right,
		DownRight = Direction.East,
		Down = Direction.Down,
		DownLeft = Direction.South,
		Left = Direction.Left,
		UpLeft = Direction.West
	}

	public class ProgressBarGump : SuperGump
	{
		public static int DefaultWidth = 210;
		public static int DefaultHeight = 25;

		public static int DefaultPadding = 5;

		public static int DefaultBackgroundID = 9400;
		public static int DefaultForegroundID = 1464;

		public static string DefaultText = "Progress";

		public static ProgressBarFlow DefaultFlow = ProgressBarFlow.Right;

		private double? _InitValue, _InitMaxValue;

		private double _Value;
		private double _MaxValue;

		public int Width { get; set; }
		public int Height { get; set; }
		public int Padding { get; set; }

		public int BackgroundID { get; set; }
		public int ForegroundID { get; set; }

		public Color TextColor { get; set; }
		public string Text { get; set; }

		public bool DisplayPercent { get; set; }
		public ProgressBarFlow Flow { get; set; }

		public Action<ProgressBarGump, double> ValueChanged { get; set; }

		public double MaxValue
		{
			get => _MaxValue;
			set
			{
				_MaxValue = value;

				if (_InitMaxValue == null)
				{
					_InitMaxValue = _MaxValue;
				}
			}
		}

		public double Value
		{
			get => _Value;
			set
			{
				if (_Value == value)
				{
					return;
				}

				var oldValue = _Value;

				InternalValue = Math.Min(_MaxValue, value);

				OnValueChanged(oldValue);
			}
		}

		public double InternalValue
		{
			get => Value;
			set
			{
				if (_Value == value)
				{
					return;
				}

				_Value = Math.Min(_MaxValue, value);

				if (_InitValue == null)
				{
					_InitValue = _Value;
				}
			}
		}

		public double PercentComplete
		{
			get
			{
				if (_Value == _MaxValue)
				{
					return 1.0;
				}

				if (_MaxValue != 0)
				{
					return _Value / _MaxValue;
				}

				return 1.0;
			}
		}

		public bool Completed => PercentComplete >= 1.0;

		public ProgressBarGump(
			Mobile user,
			string text,
			double max,
			double value = 0,
			bool displayPercent = true,
			ProgressBarFlow? flow = null,
			Action<ProgressBarGump, double> valueChanged = null)
			: this(user, null, null, null, null, null, text, max, value, displayPercent, flow, valueChanged)
		{ }

		public ProgressBarGump(
			Mobile user,
			Gump parent = null,
			int? x = null,
			int? y = null,
			int? w = null,
			int? h = null,
			string text = null,
			double max = 100,
			double value = 0,
			bool displayPercent = true,
			ProgressBarFlow? flow = null,
			Action<ProgressBarGump, double> valueChanged = null)
			: base(user, parent, x, y)
		{
			Width = w ?? DefaultWidth;
			Height = h ?? DefaultHeight;

			BackgroundID = DefaultBackgroundID;
			ForegroundID = DefaultForegroundID;

			Padding = DefaultPadding;

			TextColor = DefaultHtmlColor;
			Text = text ?? DefaultText;

			DisplayPercent = displayPercent;
			Flow = flow ?? DefaultFlow;
			ValueChanged = valueChanged;

			_MaxValue = max;
			_Value = value;

			ForceRecompile = true;
		}

		public virtual void Reset()
		{
			_Value = _InitValue ?? 0;
			_MaxValue = _InitMaxValue ?? 100;

			_InitValue = _InitMaxValue = null;
		}

		public virtual string FormatText(bool html = false)
		{
			var text = String.Format("{0} {1}", Text, DisplayPercent ? PercentComplete.ToString("0.##%") : String.Empty);

			if (html && !String.IsNullOrWhiteSpace(text))
			{
				text = text.WrapUOHtmlColor(TextColor, false).WrapUOHtmlTag("center");
			}

			return text;
		}

		protected override void Compile()
		{
			Width = Math.Max(10 + Padding, Math.Min(1024, Width));
			Height = Math.Max(10 + Padding, Math.Min(768, Height));

			if (Padding < 0)
			{
				Padding = DefaultPadding;
			}

			base.Compile();
		}

		protected virtual void OnValueChanged(double oldValue)
		{
			if (ValueChanged != null)
			{
				ValueChanged(this, oldValue);
			}

			Refresh(true);
		}

		protected bool FlowOffset(ref int x, ref int y, ref int w, ref int h)
		{
			double xo = x, yo = y, wo = w, ho = h;

			switch (Flow)
			{
				case ProgressBarFlow.Up:
				{
					ho *= PercentComplete;
					yo = (y + h) - ho;
				}
				break;
				case ProgressBarFlow.UpRight:
				{
					wo *= PercentComplete;
					ho *= PercentComplete;
					yo = (y + h) - ho;
				}
				break;
				case ProgressBarFlow.Right:
				{
					wo *= PercentComplete;
				}
				break;
				case ProgressBarFlow.DownRight:
				{
					wo *= PercentComplete;
					ho *= PercentComplete;
				}
				break;
				case ProgressBarFlow.Down:
				{
					ho *= PercentComplete;
				}
				break;
				case ProgressBarFlow.DownLeft:
				{
					wo *= PercentComplete;
					ho *= PercentComplete;
					xo = (x + w) - wo;
				}
				break;
				case ProgressBarFlow.Left:
				{
					wo *= PercentComplete;
					xo = (x + w) - wo;
				}
				break;
				case ProgressBarFlow.UpLeft:
				{
					wo *= PercentComplete;
					ho *= PercentComplete;
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

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			var xyPadding = Padding;
			var whPadding = xyPadding * 2;

			layout.Add(
				"background/body/base",
				() =>
				{
					AddBackground(0, 0, Width, Height, BackgroundID);

					if (Width > whPadding && Height > whPadding)
					{
						AddAlphaRegion(xyPadding, xyPadding, Width - whPadding, Height - whPadding);
					}

					//AddTooltip(Completed ? 1049071 : 1049070);
				});

			layout.Add(
				"imagetiled/body/visual",
				() =>
				{
					if (Width <= whPadding || Height <= whPadding)
					{
						return;
					}

					int x = xyPadding, y = xyPadding, w = Width - whPadding, h = Height - whPadding;

					if (FlowOffset(ref x, ref y, ref w, ref h))
					{
						AddImageTiled(x, y, w, h, ForegroundID);
					}
				});

			layout.Add(
				"html/body/text",
				() =>
				{
					if (Width <= whPadding || Height <= whPadding)
					{
						return;
					}

					var text = FormatText(true);

					if (!String.IsNullOrWhiteSpace(text))
					{
						AddHtml(xyPadding, xyPadding, Width - whPadding, Math.Max(40, Height - whPadding), text, false, false);
					}
				});
		}
	}
}