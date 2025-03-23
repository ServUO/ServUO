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

using Server;
using Server.Gumps;
#endregion

namespace VitaNex.SuperGumps.UI
{
	public class PanelGump<T> : SuperGump
	{
		private int _Width;
		private int _Height;

		protected bool WasModal { get; set; }

		public virtual string Title { get; set; }
		public virtual string EmptyText { get; set; }

		public virtual bool Minimized { get; set; }

		public virtual T Selected { get; set; }

		public virtual MenuGumpOptions Options { get; set; }

		public virtual int Width { get => _Width; set => _Width = Math.Max(250, Math.Min(1024, value)); }
		public virtual int Height { get => _Height; set => _Height = Math.Max(250, Math.Min(786, value)); }

		public PanelGump(
			Mobile user,
			Gump parent = null,
			int? x = null,
			int? y = null,
			int width = 420,
			int height = 420,
			string emptyText = null,
			string title = null,
			IEnumerable<ListGumpEntry> opts = null,
			T selected = default(T))
			: base(user, parent, x, y)
		{
			Width = width;
			Height = height;
			Selected = selected;
			EmptyText = emptyText ?? "No entry to display.";
			Title = title ?? "Panel View";
			Minimized = false;
			CanMove = false;

			if (opts != null)
			{
				Options = new MenuGumpOptions(opts);
			}
		}

		protected override void Compile()
		{
			base.Compile();

			if (Options == null)
			{
				Options = new MenuGumpOptions();
			}

			CompileMenuOptions(Options);
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			var sup = SupportsUltimaStore;
			var ec = IsEnhancedClient;
			var bgID = ec ? 83 : sup ? 40000 : 9270;

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
				"button/header/options",
				() =>
				{
					AddButton(15, 15, 2008, 2007, ShowOptionMenu);
					AddTooltip(1015326);
				});

			layout.Add(
				"button/header/minimize",
				() =>
				{
					if (Minimized)
					{
						AddButton(Width - 30, 20, 10740, 10742, Maximize);
						AddTooltip(3002086);
					}
					else
					{
						AddButton(Width - 30, 20, 10741, 10742, Minimize);
						AddTooltip(3002085);
					}
				});

			layout.Add(
				"label/header/title",
				() => AddLabelCropped(90, 15, Width - 135, 20, GetTitleHue(), String.IsNullOrEmpty(Title) ? "Panel View" : Title));

			if (Minimized)
			{
				return;
			}

			layout.Add(
				"background/body/base",
				() =>
				{
					AddBackground(0, 50, Width, Height, bgID);

					if (!ec)
					{
						AddImageTiled(10, 60, Width - 20, Height - 20, 2624);
						//AddAlphaRegion(10, 60, Width - 20, Height - 20);
					}
				});

			if (Selected == null)
			{
				var text = String.IsNullOrEmpty(EmptyText) ? "No entry to display." : EmptyText;

				layout.Add("label/list/empty", () => AddLabelCropped(15, 67, Width - 30, 20, ErrorHue, text));
			}
		}

		protected virtual void Minimize(GumpButton entry = null)
		{
			Minimized = true;

			if (Modal)
			{
				WasModal = true;
			}

			Modal = false;

			Refresh(true);
		}

		protected virtual void Maximize(GumpButton entry = null)
		{
			Minimized = false;

			if (WasModal)
			{
				Modal = true;
			}

			WasModal = false;

			Refresh(true);
		}

		protected virtual int GetTitleHue()
		{
			return TextHue;
		}

		protected virtual void ShowOptionMenu(GumpButton button)
		{
			if (User != null && !User.Deleted && Options != null && Options.Count >= 0)
			{
				Send(new MenuGump(User, Refresh(), Options, button));
			}
		}

		protected virtual void CompileMenuOptions(MenuGumpOptions list)
		{
			if (Minimized)
			{
				list.Replace("Minimize", new ListGumpEntry("Maximize", Maximize));
			}
			else
			{
				list.Replace("Maximize", new ListGumpEntry("Minimize", Minimize));
			}

			list.AppendEntry(new ListGumpEntry("Refresh", button => Refresh(true)));

			if (CanClose)
			{
				list.AppendEntry(new ListGumpEntry("Exit", button => Close()));
			}

			list.AppendEntry(new ListGumpEntry("Cancel", button => { }));
		}
	}
}