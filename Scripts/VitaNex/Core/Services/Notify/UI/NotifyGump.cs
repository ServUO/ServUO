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

using Server;
using Server.Gumps;

using VitaNex.SuperGumps;
using VitaNex.Text;
#endregion

namespace VitaNex.Notify
{
	public abstract class NotifyGump : SuperGump
	{
		private static void InitSettings(NotifySettings o)
		{
			o.Name = "Misc Notifications";
			o.Desc = "Any notifications that do not fall into a more specific category.";
		}

		public static event Action<NotifyGump> OnNotify;

		public static TimeSpan DefaultAnimDuration = TimeSpan.FromMilliseconds(500.0);
		public static TimeSpan DefaultPauseDuration = TimeSpan.FromSeconds(5.0);

		public static Size SizeMin = new Size(100, 60);
		public static Size SizeMax = new Size(400, 200);

		public enum AnimState
		{
			Show,
			Hide,
			Pause
		}

		public TimeSpan AnimDuration { get; set; }
		public TimeSpan PauseDuration { get; set; }

		public string Html { get; set; }
		public Color HtmlColor { get; set; }
		public int HtmlIndent { get; set; }

		public int BorderID { get; set; }
		public int BorderSize { get; set; }
		public bool BorderAlpha { get; set; }

		public int BackgroundID { get; set; }
		public bool BackgroundAlpha { get; set; }

		public int WidthMax { get; set; }
		public int HeightMax { get; set; }

		public bool AutoClose { get; set; }

		public int Frame { get; private set; }
		public AnimState State { get; private set; }

		public int FrameCount => (int)Math.Ceiling(Math.Max(100.0, AnimDuration.TotalMilliseconds) / 100.0);
		public int FrameHeight { get; private set; }
		public int FrameWidth { get; private set; }

		public Size HtmlSize { get; private set; }
		public Size OptionsSize { get; private set; }

		public List<NotifyGumpOption> Options { get; private set; }

		public int OptionsCols { get; private set; }
		public int OptionsRows { get; private set; }

		public override bool InitPolling => true;

		public NotifyGump(Mobile user, string html)
			: this(user, html, null)
		{ }

		public NotifyGump(Mobile user, string html, IEnumerable<NotifyGumpOption> options)
			: base(user, null, 0, 140)
		{
			Options = options.Ensure().ToList();

			AnimDuration = DefaultAnimDuration;
			PauseDuration = DefaultPauseDuration;

			Html = html ?? String.Empty;
			HtmlColor = Color.White;
			HtmlIndent = 10;

			BorderSize = 4;
			BorderID = 9204;
			BorderAlpha = false;

			BackgroundID = 2624;
			BackgroundAlpha = true;

			AutoClose = true;

			Frame = 0;
			State = AnimState.Pause;

			CanMove = false;
			CanResize = false;

			CloseSound = -1;

			ForceRecompile = true;
			AutoRefreshRate = TimeSpan.FromMilliseconds(100.0);
			AutoRefresh = true;

			WidthMax = 250;
			HeightMax = 100;

			InitOptions();
		}

		protected virtual void InitOptions()
		{ }

		public void AddOption(TextDefinition label, Action<GumpButton> callback)
		{
			AddOption(label, callback, Color.Empty);
		}

		public void AddOption(TextDefinition label, Action<GumpButton> callback, Color color)
		{
			AddOption(label, callback, color, Color.Empty);
		}

		public void AddOption(TextDefinition label, Action<GumpButton> callback, Color color, Color fill)
		{
			AddOption(label, callback, color, fill, Color.Empty);
		}

		public void AddOption(TextDefinition label, Action<GumpButton> callback, Color color, Color fill, Color border)
		{
			if (color.IsEmpty)
			{
				color = HtmlColor;
			}

			var opt = Options.Find(o => o.Label.Equals(label));

			if (opt != null)
			{
				opt.Callback = callback;
				opt.LabelColor = color;
				opt.FillColor = fill;
				opt.BorderColor = border;
			}
			else
			{
				Options.Add(new NotifyGumpOption(label, callback, color, fill, border));
			}
		}

		protected virtual Size GetSizeMin()
		{
			return SizeMin;
		}

		protected virtual Size GetSizeMax()
		{
			return SizeMax;
		}

		protected override void Compile()
		{
			base.Compile();

			var sMin = GetSizeMin();
			var sMax = GetSizeMax();

			WidthMax = Math.Max(sMin.Width, Math.Min(sMax.Width, WidthMax));
			HeightMax = Math.Max(sMin.Height, Math.Min(sMax.Height, HeightMax));

			HtmlIndent = Math.Max(0, HtmlIndent);
			BorderSize = Math.Max(0, BorderSize);

			var wm = WidthMax - (BorderSize * 2);

			var text = Html.ParseBBCode(HtmlColor).StripHtmlBreaks(true).StripHtml();

			var font = UOFont.Unicode[1];

			var s = font.GetSize(text);

			s.Width += 4;
			s.Height += 4;

			if (s.Width > wm)
			{
				s.Height += (int)Math.Ceiling((s.Width - wm) / (double)wm) * (font.LineHeight + font.LineSpacing);
				s.Width = wm;
			}

			if (!Initialized)
			{
				s.Height = Math.Max(sMin.Height, Math.Min(HeightMax, Math.Min(sMax.Height, s.Height)));
			}
			else
			{
				s.Height = Math.Max(sMin.Height, Math.Min(sMax.Height, Math.Min(sMax.Height, s.Height)));
			}

			HtmlSize = s;

			if (Options.Count > 1)
			{
				OptionsCols = wm / Math.Max(30, Math.Min(wm, Options.Max(o => o.Width)));
				OptionsRows = (int)Math.Ceiling(Options.Count / (double)OptionsCols);
			}
			else if (Options.Count > 0)
			{
				OptionsCols = OptionsRows = 1;
			}

			s.Width = wm;
			s.Height = (OptionsRows * 20) + 4;

			OptionsSize = s;

			HeightMax = (BorderSize * 2) + HtmlSize.Height + OptionsSize.Height;

			var f = Frame / (double)FrameCount;

			FrameWidth = (int)Math.Ceiling(WidthMax * f);
			FrameHeight = (int)Math.Ceiling(HeightMax * f);
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			layout.Add(
				"frame",
				() =>
				{
					if (BorderSize > 0 && BorderID >= 0)
					{
						AddImageTiled(0, 0, FrameWidth, FrameHeight, BorderID);

						if (BorderAlpha)
						{
							AddAlphaRegion(0, 0, FrameWidth, FrameHeight);
						}
					}

					var fw = FrameWidth - (BorderSize * 2);
					var fh = FrameHeight - (BorderSize * 2);

					if (fw * fh > 0 && BackgroundID > 0)
					{
						AddImageTiled(BorderSize, BorderSize, fw, fh, BackgroundID);

						if (BackgroundAlpha)
						{
							AddAlphaRegion(BorderSize, BorderSize, fw, fh);
						}
					}

					if (Frame < FrameCount)
					{
						return;
					}

					AddButton(FrameWidth - BorderSize, (FrameHeight / 2) - 8, 22153, 22155, OnSettings);

					var x = BorderSize + 2 + HtmlIndent;
					var y = BorderSize + 2;
					var w = fw - (4 + HtmlIndent);
					var h = fh - (4 + OptionsSize.Height);

					var html = Html.ParseBBCode(HtmlColor).WrapUOHtmlColor(HtmlColor, false);

					AddHtml(x, y, w, h, html, false, (HtmlSize.Height - 4) > h);
				});

			if (Frame >= FrameCount && Options.Count > 0)
			{
				var x = BorderSize + 2;
				var y = BorderSize + HtmlSize.Height + 2;
				var w = OptionsSize.Width - 4;
				var h = OptionsSize.Height - 4;

				CompileOptionsLayout(layout, x, y, w, h, w / OptionsCols);
			}
		}

		protected virtual void CompileOptionsLayout(SuperGumpLayout layout, int x, int y, int w, int h, int bw)
		{
			if (Frame < FrameCount || Options.Count == 0 || OptionsCols * OptionsRows <= 0)
			{
				return;
			}

			layout.Add("opts", () => AddRectangle(x, y, w, h, Color.Black, true));

			var oi = 0;
			var ox = x;
			var oy = y;

			foreach (var ob in Options)
			{
				CompileOptionLayout(layout, oi, ox, oy, bw, 20, ob);

				if (++oi % OptionsCols == 0)
				{
					ox = x;
					oy += 20;
				}
				else
				{
					ox += bw;
				}
			}
		}

		protected virtual void CompileOptionLayout(
			SuperGumpLayout layout,
			int index,
			int x,
			int y,
			int w,
			int h,
			NotifyGumpOption option)
		{
			layout.Add(
				"opts/" + index,
				() =>
				{
					AddHtmlButton(
						x,
						y,
						w,
						h,
						b =>
						{
							if (option.Callback != null)
							{
								option.Callback(b);
							}

							Refresh();
						},
						option.GetString(User),
						option.LabelColor,
						option.FillColor,
						option.BorderColor,
						1);
				});
		}

		protected virtual void OnSettings(GumpButton b)
		{
			Refresh();

			new NotifySettingsGump(User).Send();
		}

		protected override bool CanAutoRefresh()
		{
			return State == AnimState.Pause && Frame > 0 ? AutoClose && base.CanAutoRefresh() : base.CanAutoRefresh();
		}

		protected override void OnAutoRefresh()
		{
			base.OnAutoRefresh();

			AnimateList();

			switch (State)
			{
				case AnimState.Show:
				{
					if (Frame++ >= FrameCount)
					{
						AutoRefreshRate = PauseDuration;
						State = AnimState.Pause;
						Frame = FrameCount;
					}
				}
				break;
				case AnimState.Hide:
				{
					if (Frame-- <= 0)
					{
						AutoRefreshRate = TimeSpan.FromMilliseconds(100.0);
						State = AnimState.Pause;
						Frame = 0;
						Close(true);
					}
				}
				break;
				case AnimState.Pause:
				{
					AutoRefreshRate = TimeSpan.FromMilliseconds(100.0);
					State = Frame <= 0 ? AnimState.Show : AutoClose ? AnimState.Hide : AnimState.Pause;
				}
				break;
			}
		}

		protected override bool OnBeforeSend()
		{
			if (!Initialized)
			{
				if (Notify.IsIgnored(GetType(), User))
				{
					return false;
				}

				if (!Notify.IsAnimated(GetType(), User))
				{
					AnimDuration = TimeSpan.Zero;
				}

				if (OnNotify != null)
				{
					OnNotify(this);
				}
			}

			return base.OnBeforeSend();
		}

		public override void Close(bool all)
		{
			if (all)
			{
				base.Close(true);
			}
			else
			{
				AutoRefreshRate = TimeSpan.FromMilliseconds(100.0);
				AutoClose = true;
			}
		}

		private void AnimateList()
		{
			VitaNexCore.TryCatch(
				() =>
				{
					var p = this;

					foreach (var g in EnumerateInstances<NotifyGump>(User, true)
						.Where(g => g != this && g.IsOpen && !g.IsDisposed && g.Y >= p.Y)
						.OrderBy(g => g.Y))
					{
						g.Y = p.Y + p.FrameHeight;

						p = g;

						if (g.State != AnimState.Pause)
						{
							return;
						}

						var lr = g.LastAutoRefresh;

						g.Refresh(true);
						g.LastAutoRefresh = lr;
					}
				},
				e => e.ToConsole());
		}

		private class Sub0 : NotifyGump
		{
			public Sub0(Mobile user, string html)
				: base(user, html)
			{ }
		}

		private class Sub1 : NotifyGump
		{
			public Sub1(Mobile user, string html)
				: base(user, html)
			{ }
		}

		private class Sub2 : NotifyGump
		{
			public Sub2(Mobile user, string html)
				: base(user, html)
			{ }
		}

		private class Sub3 : NotifyGump
		{
			public Sub3(Mobile user, string html)
				: base(user, html)
			{ }
		}

		private class Sub4 : NotifyGump
		{
			public Sub4(Mobile user, string html)
				: base(user, html)
			{ }
		}

		private class Sub5 : NotifyGump
		{
			public Sub5(Mobile user, string html)
				: base(user, html)
			{ }
		}

		private class Sub6 : NotifyGump
		{
			public Sub6(Mobile user, string html)
				: base(user, html)
			{ }
		}

		private class Sub7 : NotifyGump
		{
			public Sub7(Mobile user, string html)
				: base(user, html)
			{ }
		}

		private class Sub8 : NotifyGump
		{
			public Sub8(Mobile user, string html)
				: base(user, html)
			{ }
		}

		private class Sub9 : NotifyGump
		{
			public Sub9(Mobile user, string html)
				: base(user, html)
			{ }
		}
	}
}