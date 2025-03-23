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
using System.Text;

using Server;
using Server.Gumps;

using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Modules.Games
{
	public sealed class ArcadeUI : SuperGumpList<IGame>
	{
		public ArcadeProfile Profile { get; private set; }

		public int Width { get; set; }
		public int Height { get; set; }

		public int Margin { get; set; }
		public int Padding { get; set; }

		public int MenuSize { get; set; }
		public int IconSize { get; set; }

		public int Rows { get; private set; }
		public int Cols { get; private set; }

		public IGame SelectedGame { get; private set; }

		public ArcadeUI(Mobile user)
			: base(user)
		{
			Profile = Arcade.EnsureProfile(User);

			Sorted = true;

			EnsureDefaults();
		}

		public void EnsureDefaults()
		{
			Width = 800;
			Height = 600;

			Margin = 30;
			Padding = 15;

			MenuSize = 250;
			IconSize = 120;

			InvalidateGrid();
		}

		public void InvalidateGrid()
		{
			Width = Math.Max(300, Math.Min(1024, Width));
			Height = Math.Max(300, Math.Min(768, Height));

			MenuSize = Math.Max(200, Math.Min(400, MenuSize));
			IconSize = Math.Max(100, Math.Min(250, IconSize));

			Rows = (int)Math.Floor(((Width - MenuSize) - (Padding * 2)) / (double)IconSize);
			Cols = (int)Math.Floor((Height - (Padding * 2)) / (double)IconSize);

			EntriesPerPage = Rows * Cols;
		}

		protected override void Compile()
		{
			if (Profile == null || Profile.Owner != User)
			{
				Profile = Arcade.EnsureProfile(User);
			}

			InvalidateGrid();

			base.Compile();
		}

		protected override void CompileList(List<IGame> list)
		{
			list.Clear();
			list.AddRange(Arcade.Games.Values);

			base.CompileList(list);
		}

		public override int SortCompare(IGame a, IGame b)
		{
			var res = 0;

			if (a.CompareNull(b, ref res))
			{
				return res;
			}

			return Insensitive.Compare(a.Name, b.Name);
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			layout.Add(
				"body",
				() =>
				{
					AddBackground(0, 0, Width + (Margin * 2), Height + (Margin * 2), 2600);
					AddBackground(Margin, Margin, MenuSize, Height, 9250);
					AddBackground(Margin + MenuSize, Margin, Width - MenuSize, Height, 9250);
				});

			layout.Add(
				"title",
				() =>
				{
					var x = Margin + MenuSize + Padding;
					var y = Margin + Padding;
					var w = Width - (MenuSize + (Padding * 2));

					var title = "Games Arcade";

					title = title.WrapUOHtmlBig().WrapUOHtmlCenter().WrapUOHtmlColor(Color.Gold, false);

					AddHtml(x, y, w, 40, title, false, false);
				});

			layout.Add(
				"menu",
				() =>
				{
					var x = Margin + Padding;
					var y = Margin + Padding;
					var w = MenuSize - (Padding * 2);
					var h = Height - (Padding * 2);

					CompileMenuLayout(x, y, w, h);
				});

			var games = GetListRange();

			if (games != null)
			{
				var width = Cols * IconSize;
				var height = Rows * IconSize;

				var initX = Margin + MenuSize;
				var initY = Margin + Padding;

				if (width < Width - MenuSize)
				{
					initX += ((Width - MenuSize) - width) / 2;
				}

				if (height < Height)
				{
					initY += (Height - height) / 2;
				}

				var xOffset = initX;
				var yOffset = initY;

				var index = 0;

				var col = 0;
				var row = 0;

				foreach (var game in games.Values)
				{
					CompileGameLayout(layout, index++, xOffset, yOffset, game);

					xOffset += IconSize;

					if (++col % Cols == 0)
					{
						xOffset = initX;
						yOffset += IconSize;

						if (++row % Rows == 0)
						{
							break;
						}
					}
				}

				if (col < Cols || row < Rows)
				{
					while (index < EntriesPerPage)
					{
						CompileGameLayout(layout, index++, xOffset, yOffset, null);

						xOffset += IconSize;

						if (++col % Cols == 0)
						{
							xOffset = initX;
							yOffset += IconSize;

							if (++row % Rows == 0)
							{
								break;
							}
						}
					}
				}
			}
		}

		private void CompileGameLayout(SuperGumpLayout layout, int index, int x, int y, IGame game)
		{
			layout.Add("games/" + index, () => CompileGameLayout(x, y, IconSize, IconSize, game));
		}

		private void CompileGameLayout(int x, int y, int w, int h, IGame game)
		{
			if (game != null)
			{
				AddTileButton(x + Margin, y + Margin, w - (Margin * 2), h - (Margin * 2), b => SelectGame(game));
			}

			AddBackground(x, y, w, h, 2600);

			if (game == null)
			{
				return;
			}

			y += Margin;
			h -= Margin;

			if (game.Icon != null && !game.Icon.IsEmpty)
			{
				var s = game.Icon.Size;

				if (game.Enabled)
				{
					game.Icon.AddToGump(this, x + ((w - s.Width) / 2), y);
				}
				else
				{
					game.Icon.AddToGump(this, x + ((w - s.Width) / 2), y, 900);
				}

				y += s.Height + 5;
				h -= s.Height + 5;
			}

			var text = game.Name.WrapUOHtmlBig().WrapUOHtmlCenter().WrapUOHtmlColor(Color.Gold, false);

			AddHtml(x, y, w, h, text, false, false);
		}

		private void CompileMenuLayout(int x, int y, int w, int h)
		{
			if (SelectedGame == null)
			{
				return;
			}

			AddHtml(x, y, w, h - 95, GetDescription(SelectedGame), false, false);

			y += h - 85;

			var help = "Help";

			help = help.WrapUOHtmlCenter();

			if (SupportsUltimaStore)
			{
				help = help.WrapUOHtmlColor(Color.Gold, false);

				AddButton(x + ((w - 126) / 2), y, 40019, 40029, b => SelectHelp(SelectedGame));
				AddHtml(x + ((w - 126) / 2), y + 2, 126, 40, help, false, false);
			}
			else
			{
				help = help.WrapUOHtmlBig();

				AddHtmlButton(x + ((w - 126) / 2), y, 126, 25, b => SelectHelp(SelectedGame), help, Color.White, Color.Empty);
			}

			y += 25;

			var play = "Play";

			play = play.WrapUOHtmlCenter();

			if (SupportsUltimaStore)
			{
				if (SelectedGame.Enabled || User.AccessLevel >= Arcade.Access)
				{
					play = play.WrapUOHtmlColor(Color.Gold, false);

					AddButton(x + ((w - 126) / 2), y, 40019, 40029, b => PlayGame(SelectedGame));
					AddHtml(x + ((w - 126) / 2), y + 2, 126, 40, play, false, false);
				}
				else
				{
					play = play.WrapUOHtmlColor(Color.Gray, false);

					AddImage(x + ((w - 126) / 2), y, 40019, 900);
					AddHtml(x + ((w - 126) / 2), y + 2, 126, 40, play, false, false);
				}
			}
			else
			{
				if (SelectedGame.Enabled)
				{
					play = play.WrapUOHtmlBig();

					AddHtmlButton(x + ((w - 126) / 2), y, 126, 25, b => PlayGame(SelectedGame), play, Color.White, Color.Black);
				}
				else
				{
					play = play.WrapUOHtmlColor(Color.Gray, false);

					AddRectangle(x + ((w - 126) / 2), y, 126, 25, Color.Black, true);
					AddHtml(x + ((w - 126) / 2), y + 2, 126, 40, play, false, false);
				}
			}

			if (User.AccessLevel < Arcade.Access)
			{
				return;
			}

			y += 25;

			var label = SelectedGame.Enabled ? "Enabled" : "Disabled";

			label = label.WrapUOHtmlCenter();

			if (SupportsUltimaStore)
			{
				if (SelectedGame.Enabled)
				{
					label = label.WrapUOHtmlColor(Color.PaleGreen, false);

					AddButton(x + ((w - 126) / 2), y, 40019, 40029, b => DisableGame(SelectedGame));
					AddHtml(x + ((w - 126) / 2), y + 2, 126, 40, label, false, false);
				}
				else
				{
					label = label.WrapUOHtmlColor(Color.Gray, false);

					AddButton(x + ((w - 126) / 2), y, 40019, 40029, b => EnableGame(SelectedGame));
					AddHtml(x + ((w - 126) / 2), y + 2, 126, 40, label, false, false);
				}
			}
			else
			{
				if (SelectedGame.Enabled)
				{
					label = label.WrapUOHtmlBig();

					AddHtmlButton(
						x + ((w - 126) / 2),
						y,
						126,
						25,
						b => DisableGame(SelectedGame),
						label,
						Color.PaleGreen,
						Color.Black);
				}
				else
				{
					label = label.WrapUOHtmlBig();

					AddHtmlButton(x + ((w - 126) / 2), y, 126, 25, b => EnableGame(SelectedGame), label, Color.Gray, Color.Black);
				}
			}
		}

		public string GetDescription(IGame game)
		{
			if (game == null)
			{
				return String.Empty;
			}

			var desc = new StringBuilder();

			desc.Append("<CENTER>");
			desc.AppendLine(game.Name.WrapUOHtmlBig().WrapUOHtmlColor(Color.Gold, false));
			desc.AppendLine(String.Empty.WrapUOHtmlColor(Color.White, false));
			desc.AppendLine(game.Desc);
			desc.Append("</CENTER>");

			return desc.ToString();
		}

		public void EnableGame(IGame game)
		{
			if (game == null)
			{
				Refresh(true);
				return;
			}

			game.Enabled = true;

			Refresh(true);
		}

		public void DisableGame(IGame game)
		{
			if (game == null)
			{
				Refresh(true);
				return;
			}

			game.Enabled = false;

			Refresh(true);
		}

		public void PlayGame(IGame game)
		{
			if (game == null || !game.Open(User))
			{
				Refresh(true);
			}
		}

		public void SelectGame(IGame game)
		{
			SelectedGame = SelectedGame == game ? null : game;

			Refresh(true);
		}

		public void SelectHelp(IGame game)
		{
			if (game == null)
			{
				Refresh(true);
				return;
			}

			new NoticeDialogGump(User, Refresh())
			{
				Title = game.Name,
				Html = game.Help,
				Width = 500,
				Height = 400,
				HtmlColor = Color.White,
				AcceptHandler = Refresh,
				CancelHandler = Refresh
			}.Send();
		}

		protected override void OnLayoutApplied()
		{
			base.OnLayoutApplied();

			if (!SupportsUltimaStore)
			{
				return;
			}

			foreach (var e in Entries.OfType<GumpBackground>())
			{
				switch (e.GumpID)
				{
					case 2600:
						e.GumpID = 39925;
						break;
					case 9250:
						e.GumpID = 40000;
						break;
				}
			}
		}
	}
}