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

#if ServUO58
#define ServUOX
#endif

#region References
using System;
using System.Drawing;

using Server;
using Server.Gumps;
#endregion

namespace VitaNex.SuperGumps.UI
{
	public class DialogGump : SuperGump
	{
		public static int Defaultwidth = 400;
		public static int DefaultHeight = 300;

		public static int DefaultIcon = 7000;
		public static string DefaultTitle = "Dialog";

		protected IconDefinition _Icon;

		public virtual Action<GumpButton> AcceptHandler { get; set; }
		public virtual Action<GumpButton> CancelHandler { get; set; }

		public virtual string Title { get; set; }

		public virtual bool HtmlBackground { get; set; }
		public virtual bool HtmlScrollbar { get; set; }

		public virtual Color HtmlColor { get; set; }
		public virtual string Html { get; set; }

		public virtual int Icon { get => _Icon.AssetID; set => _Icon.AssetID = Math.Max(0, value); }
		public virtual int IconHue { get => _Icon.Hue; set => _Icon.Hue = Math.Max(0, value); }

		public virtual bool IconItem
		{
			get => _Icon.IsItemArt;
			set => _Icon.AssetType = value ? IconType.ItemArt : IconType.GumpArt;
		}

		public virtual int IconTooltip { get; set; }

		public virtual int Width { get; set; }
		public virtual int Height { get; set; }

		public DialogGump(
			Mobile user,
			Gump parent = null,
			int? x = null,
			int? y = null,
			string title = null,
			string html = null,
			int icon = -1,
			Action<GumpButton> onAccept = null,
			Action<GumpButton> onCancel = null)
			: base(user, parent, x, y)
		{
			_Icon = IconDefinition.FromGump(icon >= 0 ? icon : DefaultIcon);
			_Icon.ComputeOffset = false;

			Modal = true;
			CanDispose = false;

			HtmlBackground = false;
			HtmlScrollbar = true;

			HtmlColor = DefaultHtmlColor;

			Width = Defaultwidth;
			Height = DefaultHeight;

			Title = title ?? DefaultTitle;
			Html = html;

			AcceptHandler = onAccept;
			CancelHandler = onCancel;
		}

		protected virtual void OnAccept(GumpButton button)
		{
			if (AcceptHandler != null)
			{
				AcceptHandler(button);
			}
		}

		protected virtual void OnCancel(GumpButton button)
		{
			if (CancelHandler != null)
			{
				CancelHandler(button);
			}
		}

		protected override void Compile()
		{
			Width = Math.Max(300, Math.Min(1024, Width));
			Height = Math.Max(200, Math.Min(768, Height));

			base.Compile();
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			var sup = SupportsUltimaStore;
			var ec = IsEnhancedClient;
			var bgID = ec ? 83 : sup ? 40000 : 9270;

			layout.Add(
				"background/body/base",
				() =>
				{
					AddBackground(0, 0, Width, Height, bgID);

					if (!ec)
					{
						AddImageTiled(10, 10, Width - 20, Height - 20, 2624);
						//AddAlphaRegion(10, 10, Width - 20, Height - 20);
					}
				});

			layout.Add(
				"background/header/base",
				() =>
				{
					AddBackground(0, 0, Width, 50, bgID);

					if (!ec)
					{
						AddImageTiled(10, 10, Width - 20, 30, 2624);
						//AddAlphaRegion(10, 10, Width - 20, 30);
					}
				});

			layout.Add(
				"label/header/title",
				() =>
				{
					var title = Title.WrapUOHtmlBig().WrapUOHtmlColor(HtmlColor, false);

					AddHtml(15, 15, Width - 30, 40, title, false, false);
				});

			layout.Add(
				"image/body/icon",
				() =>
				{
					if (_Icon == null || _Icon.IsEmpty)
					{
						return;
					}

					var iw = Math.Max(32, _Icon.Size.Width);
					var ih = Math.Max(32, _Icon.Size.Height);

					if (sup)
					{
						if (_Icon.IsSpellIcon)
						{
							AddBackground(10, 50, iw + 30, ih + 30, 30536);
						}
						else
						{
							AddBackground(15, 55, iw + 20, ih + 20, 40000);
							AddImageTiled(25, 65, iw, ih, 2624);
						}
					}
					else
					{
						AddBackground(15, 55, iw + 20, ih + 20, 9270);
						AddImageTiled(25, 65, iw, ih, 2624);
					}

					_Icon.AddToGump(this, 25, 65);

					if (IconTooltip > 0)
					{
						if (IconTooltip >= 0x40000000)
						{
#if ServUOX
							AddProperties(new Serial(IconTooltip));
#else
							AddProperties(IconTooltip);
#endif
						}
						else
						{
							AddTooltip(IconTooltip);
						}
					}
				});

			layout.Add(
				"html/body/info",
				() =>
				{
					var x = 15;
					var y = 55;
					var w = Width - 30;
					var h = Height - 110;

					if (_Icon != null && !_Icon.IsEmpty)
					{
						var iw = Math.Max(32, _Icon.Size.Width);

						x += iw + 25;
						w -= iw + 25;
					}

					if (SupportsUltimaStore)
					{
						AddBackground(x, y, w, h, 39925);

						x += 20;
						y += 22;
						w -= 25;
						h -= 38;
					}

					AddHtml(x, y, w, h, Html.WrapUOHtmlColor(HtmlColor, false), HtmlBackground, HtmlScrollbar);
				});

			layout.Add(
				"button/body/cancel",
				() =>
				{
					AddButton(Width - 90, Height - 45, 4018, 4019, OnCancel);
					AddTooltip(1006045);
				});

			layout.Add(
				"button/body/accept",
				() =>
				{
					AddButton(Width - 50, Height - 45, 4015, 4016, OnAccept);
					AddTooltip(1006044);
				});
		}

		protected override void OnDispose()
		{
			base.OnDispose();

			_Icon = null;

			AcceptHandler = null;
			CancelHandler = null;
		}
	}
}
