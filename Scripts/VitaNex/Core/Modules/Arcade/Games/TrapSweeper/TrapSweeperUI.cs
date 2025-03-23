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

using VitaNex.SuperGumps;
#endregion

namespace VitaNex.Modules.Games
{
	public class TrapSweeperUI : GameUI<TrapSweeperEngine>
	{
		public TrapSweeperUI(TrapSweeperEngine engine)
			: base(engine)
		{
			HighlightHue = 1258;
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			if (Engine == null || Engine.IsDisposed)
			{
				return;
			}

			switch (Engine.State)
			{
				case TrapSweeperState.Menu:
					CompileMenuLayout(layout);
					break;
				default:
					CompilePlayLayout(layout);
					break;
			}
		}

		private void CompileMenuLayout(SuperGumpLayout layout)
		{
			layout.Add(
				"window/menu/start",
				() =>
				{
					AddImage(354, 256, 4501, HighlightHue);
					AddButton(351, 254, 4501, 4501, b => Engine.DoPlay());

					AddHtml(
						140,
						300,
						435,
						40,
						"START SWEEPING!".WrapUOHtmlBold().WrapUOHtmlCenter().WrapUOHtmlColor(Color.Gold, false),
						false,
						false);
				});

			layout.Add(
				"window/menu/option/easy",
				() =>
				{
					var s = false;

					// EASY
					if (Engine.Mode != TrapSweeperMode.Easy)
					{
						AddImage(155, 330, 1417);
						AddButton(164, 339, 5575, 5576, b => Engine.DoMode(TrapSweeperMode.Easy));
						AddButton(145, 420, 5403, 5403, b => Engine.DoMode(TrapSweeperMode.Easy));
					}
					else
					{
						AddImage(155, 330, 1417, HighlightHue);
						AddImage(164, 339, 5576);

						s = true;
					}

					var text = "ROOKIE";

					text = s ? text.WrapUOHtmlTag("U") : text;
					text = text.WrapUOHtmlBig().WrapUOHtmlCenter().WrapUOHtmlColor(Color.Green, false);

					AddBackground(145, 415, 100, 30, 9350);
					AddHtml(145, 420, 100, 40, text, false, false);
				});

			layout.Add(
				"window/menu/option/normal",
				() =>
				{
					var s = false;

					// NORMAL
					if (Engine.Mode != TrapSweeperMode.Normal)
					{
						AddImage(262, 330, 1417);
						AddButton(271, 339, 5587, 5588, b => Engine.DoMode(TrapSweeperMode.Normal));
						AddButton(252, 420, 5403, 5403, b => Engine.DoMode(TrapSweeperMode.Normal));
					}
					else
					{
						AddImage(262, 330, 1417, HighlightHue);
						AddImage(271, 339, 5588);

						s = true;
					}

					var text = "GUARD";

					text = s ? text.WrapUOHtmlTag("U") : text;
					text = text.WrapUOHtmlBig().WrapUOHtmlCenter().WrapUOHtmlColor(Color.Yellow, false);

					AddBackground(252, 415, 100, 30, 9350);
					AddHtml(252, 420, 100, 40, text, false, false);
				});

			layout.Add(
				"window/menu/option/hard",
				() =>
				{
					var s = false;

					// HARD
					if (Engine.Mode != TrapSweeperMode.Hard)
					{
						AddImage(368, 330, 1417);
						AddButton(377, 339, 5547, 5548, b => Engine.DoMode(TrapSweeperMode.Hard));
						AddButton(358, 420, 5403, 5403, b => Engine.DoMode(TrapSweeperMode.Hard));
					}
					else
					{
						AddImage(368, 330, 1417, HighlightHue);
						AddImage(377, 339, 5548);

						s = true;
					}

					var text = "KNIGHT";

					text = s ? text.WrapUOHtmlTag("U") : text;
					text = text.WrapUOHtmlBig().WrapUOHtmlCenter().WrapUOHtmlColor(Color.Red, false);

					AddBackground(358, 415, 100, 30, 9350);
					AddHtml(358, 420, 100, 40, text, false, false);
				});

			layout.Add(
				"window/menu/option/random",
				() =>
				{
					var s = false;

					// RANDOM
					if (Engine.Mode != TrapSweeperMode.Random)
					{
						AddImage(475, 330, 1417);
						AddButton(484, 339, 5583, 5584, b => Engine.DoMode(TrapSweeperMode.Random));
						AddButton(465, 420, 5403, 5403, b => Engine.DoMode(TrapSweeperMode.Random));
					}
					else
					{
						AddImage(475, 330, 1417, HighlightHue);
						AddImage(484, 339, 5584);

						s = true;
					}

					var text = "RANDOM";

					text = s ? text.WrapUOHtmlTag("U") : text;
					text = text.WrapUOHtmlBig().WrapUOHtmlCenter().WrapUOHtmlColor(Color.Blue, false);

					AddBackground(465, 415, 100, 30, 9350);
					AddHtml(465, 420, 100, 40, text, false, false);
				});
		}

		private void CompilePlayLayout(SuperGumpLayout layout)
		{
			layout.Add(
				"window/play",
				() =>
				{
					AddBackground(133, 100, 450, 350, 9200);
					AddBackground(142, 130, 435, 315, 9300);
				});

			layout.Add(
				"window/play/info",
				() =>
				{
					if (Engine.State == TrapSweeperState.Lose)
					{
						AddImage(325, 55, 7034, 34);
						AddImage(270, 32, 50562);
					}
					else
					{
						AddImage(325, 55, 7034);
					}

					AddImage(230, 60, 30082, HighlightHue);
					AddImage(315, 45, 30061, HighlightHue);
					AddImage(390, 60, 30080, HighlightHue);
				});

			layout.Add(
				"window/play/info/traps",
				() =>
				{
					AddImage(260, 50, 20999);
					AddBackground(185, 50, 75, 44, 9300);

					var count = Engine.Traps - Engine.Marked;

					AddHtml(185, 65, 75, 40, count.ToString("#,0").WrapUOHtmlCenter(), false, false);
				});

			layout.Add(
				"window/play/info/time",
				() =>
				{
					AddImage(415, 50, 23000);
					AddBackground(460, 50, 75, 44, 9300);

					TimeSpan time;

					switch (Engine.State)
					{
						case TrapSweeperState.Lose:
							time = Engine.Ended - Engine.Started;
							break;
						default:
							time = DateTime.UtcNow - Engine.Started;
							break;
					}

					AddHtml(460, 65, 75, 40, time.ToSimpleString("h:m:s").WrapUOHtmlCenter(), false, false);
				});

			layout.Add(
				"window/play/game/grid",
				() =>
				{
					var w = Engine.Width * 19;
					var h = Engine.Height * 20;

					var xo = 148 + ((418 - w) / 2);
					var yo = 137 + ((300 - h) / 2);

					foreach (var t in Engine.AllTiles())
					{
						if (t == null)
						{
							return;
						}

						if (Engine.State == TrapSweeperState.Play && !t.Visible)
						{
							AddButton(xo + (t.X * 19), yo + (t.Y * 20), t.Marked ? t.MarkID : t.HiddenID, t.ClickID, b => t.Click());
						}
						else
						{
							AddImage(xo + (t.X * 19), yo + (t.Y * 20), t.DisplayID, t.Hue);
						}

						AddImage(xo + (t.X * 19), yo + (t.Y * 20), 9028, t.Marked ? HighlightHue : 0);
					}
				});

			switch (Engine.State)
			{
				case TrapSweeperState.Play:
				{
					layout.Add(
						"window/play/mark",
						() =>
						{
							var text = "MARK";

							text = text.WrapUOHtmlBold().WrapUOHtmlCenter().WrapUOHtmlColor(Color.PaleGoldenrod, false);

							AddHtml(45, 355, 80, 40, text, false, false);
							AddButton(50, 380, 7031, 7031, b => Engine.DoMark());

							if (Engine.Mark)
							{
								AddImage(50, 380, 7031, HighlightHue);
							}
						});
				}
				break;
				case TrapSweeperState.Win:
				{
					layout.Add("window/play/results", () => AddAlphaRegion(142, 130, 435, 315));

					layout.Add(
						"window/play/collect",
						() =>
						{
							var text = "COLLECT";

							text = text.WrapUOHtmlBold().WrapUOHtmlCenter().WrapUOHtmlColor(Color.LawnGreen, false);

							AddHtml(45, 355, 80, 40, text, false, false);
							AddButton(50, 380, 7012, 7012, b => Engine.DoCollect());
							AddImage(50, 380, 7012, 85);
						});
				}
				break;
				case TrapSweeperState.Lose:
				{
					layout.Add(
						"window/play/menu",
						() =>
						{
							var text = "RESURRECT";

							text = text.WrapUOHtmlBold().WrapUOHtmlCenter().WrapUOHtmlColor(Color.Orange, false);

							AddHtml(45, 355, 80, 40, text, false, false);
							AddButton(50, 380, 7007, 7007, b => Engine.DoMenu());
							AddImage(50, 380, 7007, HighlightHue);
						});

					layout.Add(
						"window/play/blood",
						() =>
						{
							AddItem(65, 73, 7572);
							AddItem(273, 59, 7574);
							AddItem(348, 58, 4655);
							AddItem(599, 37, 7573);
							AddItem(585, 310, 4652);
							AddItem(603, 327, 4653);
							AddItem(594, 450, 4650);
						});
				}
				break;
			}
		}

		protected override void OnQuit()
		{
			base.OnQuit();

			if (Engine.State == TrapSweeperState.Win)
			{
				Engine.DoCollect();
			}
			else
			{
				Engine.DoQuit();
			}
		}
	}
}