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
using System.Globalization;
using System.Linq;

using Server;
using Server.Gumps;
using Server.Mobiles;

using VitaNex.SuperGumps;
using VitaNex.Targets;
#endregion

namespace VitaNex.Items
{
	public class RuneCodexGump : SuperGump
	{
		public virtual RuneCodex Codex { get; set; }

		public virtual string Title { get; set; }

		public virtual int ScrollWidth { get; set; }
		public virtual int ScrollHeight { get; set; }

		public bool Minimized { get; set; }

		public RuneCodex.UICache UI
		{
			get
			{
				var ui = Codex.Users.FirstOrDefault(uic => uic.User == User);

				if (ui == null)
				{
					Codex.Users.Add(ui = new RuneCodex.UICache(User as PlayerMobile));
				}

				return ui;
			}
		}

		public RuneCodexGump(Mobile user, RuneCodex codex)
			: base(user)
		{
			CanDispose = false;

			Codex = codex;

			ScrollWidth = 5;
			ScrollHeight = 5;
		}

		protected override void Compile()
		{
			var ui = UI;

			if (!Codex.Categories.Contains(ui.Category))
			{
				ui.Category = Codex.Categories.FirstOrDefault(c => c != null);
			}

			if (ui.Category == null)
			{
				ui.Entry = null;
			}
			else if (!ui.Category.Entries.Contains(ui.Entry))
			{
				ui.Entry = ui.Category.Entries.FirstOrDefault();
			}

			Title = String.Empty;

			if (!User.InRange(Codex.GetWorldLocation(), 3))
			{
				Minimized = true;
				Title += "(Out Of Range) ";
			}

			Title += String.Format(
				"[{0:#,0} charge{1}] {2}",
				Codex.Charges,
				Codex.Charges != 1 ? "s" : String.Empty,
				Codex.ResolveName(User));

			switch (ui.Mode)
			{
				case RuneCodex.UICache.ViewMode.Categories:
				{
					if (Codex.Categories.Count <= 0)
					{
						ui.Category = null;
					}
					else if (!Codex.Categories.Contains(ui.Category))
					{
						ui.Category = Codex.Categories.FirstOrDefault(c => c != null);
					}

					if (ui.EditMode && !Codex.CanEditCategories(User))
					{
						ui.EditMode = false;
					}

					if (ui.Category != null)
					{
						Title += " > " + ui.Category.Name;
					}
				}
				break;
				case RuneCodex.UICache.ViewMode.Entries:
				{
					if (ui.Category == null)
					{
						ui.Mode = RuneCodex.UICache.ViewMode.Categories;
						goto case RuneCodex.UICache.ViewMode.Categories;
					}

					if (ui.Category.Entries.Count <= 0)
					{
						ui.Entry = null;
					}
					else if (!Codex.Categories.Contains(ui.Category))
					{
						ui.Entry = ui.Category.Entries.FirstOrDefault(e => e != null);
					}

					if (ui.EditMode && !Codex.CanEditEntries(User))
					{
						ui.EditMode = false;
					}

					Title += " > " + ui.Category.Name;

					if (ui.Entry != null)
					{
						Title += " > " + ui.Entry.Name;
					}
				}
				break;
			}

			base.Compile();
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			base.CompileLayout(layout);

			var w = 50 + (70 * ScrollWidth);
			var h = 50 + (70 * ScrollHeight);

			/* Layout:
			 *  ___________
			 * [___________|<]
			 * |  |O O O O |^|
			 * |  |O O O O | |
			 * |  |O O O O | |
			 * |  |________|v|
			 * |__|<______>|>]
			 */

			layout.Add("panel/top", () => AddBackground(0, 0, w + 200, 30, 2620));

			if (!Minimized)
			{
				layout.Add("panel/left", () => AddBackground(0, 30, 200, h + 30, 2620));
				layout.Add("panel/center", () => AddBackground(200, 30, w, h, 2620));
				layout.Add("panel/right", () => AddBackground(w + 201, 30, 26, h, 2620));
				layout.Add("panel/bottom", () => AddBackground(200, h + 31, w, 30, 2620));
			}

			layout.Add("title", () => AddLabelCropped(10, 5, 180 + (w - 100), 20, TextHue, Title));

			var ui = UI;

			if ((ui.Mode == RuneCodex.UICache.ViewMode.Categories && Codex.CanEditCategories(User)) ||
				(ui.Mode == RuneCodex.UICache.ViewMode.Entries && Codex.CanEditEntries(User)))
			{
				layout.Add(
					"edit",
					() =>
					{
						ui = UI;

						AddButton(
							w + 120,
							10,
							2362,
							2362,
							b =>
							{
								ui = UI;
								ui.EditMode = !ui.EditMode;
								Refresh(true);
							});
						AddTooltip(!ui.EditMode ? 3000414 : 3000415);
					});
			}

			layout.Add(
				"help",
				() =>
				{
					ui = UI;

					AddButton(
						w + 150,
						10,
						2361,
						2361,
						b =>
						{
							ui = UI;
							ui.EditMode = false;

							switch (ui.Mode)
							{
								case RuneCodex.UICache.ViewMode.Categories:
									ui.Category = null;
									goto case RuneCodex.UICache.ViewMode.Entries;
								case RuneCodex.UICache.ViewMode.Entries:
									ui.Entry = null;
									break;
							}

							Refresh(true);
						});
					AddTooltip(1046004);
				});

			layout.Add(
				"minimize",
				() =>
				{
					AddButton(
						w + 180,
						10,
						2360,
						2360,
						b =>
						{
							if (Minimized)
							{
								Maximize(b);
							}
							else
							{
								Minimize(b);
							}
						});
					AddTooltip(!Minimized ? 3002085 : 3002086);
				});

			if (Minimized)
			{
				return;
			}

			layout.Add(
				"preview",
				() =>
				{
					ui = UI;

					switch (ui.Mode)
					{
						case RuneCodex.UICache.ViewMode.Categories:
						{
							if (ui.Category == null)
							{
								AddHtml(
									10,
									50,
									190,
									h - 10,
									"Select or add a category using the grid on the right.\nTo view the runes in a category, click the accept button in the bottom-right corner.\n\nClone: Create new rune books containing the runes from the selected category.\nThe runes are not removed from the codex."
										.WrapUOHtmlColor(Color.LawnGreen),
									false,
									true);
								break;
							}

							var cat = ui.Category;

							if (ui.EditMode)
							{
								AddLabelCropped(10, 50, 180, 20, HighlightHue, "Name: (20 Chars)");
								AddTextEntryLimited(10, 80, 180, 20, TextHue, cat.Name, 20, (e, s) => cat.Name = s);

								AddLabelCropped(10, 110, 180, 20, HighlightHue, "Description: (60 Chars)");
								AddTextEntryLimited(10, 130, 180, 60, TextHue, cat.Description, 60, (e, s) => cat.Description = s);

								AddLabelCropped(10, 200, 180, 20, HighlightHue, "Hue: (0 - 2999)");
								AddTextEntryLimited(
									10,
									220,
									180,
									20,
									TextHue,
									cat.Hue.ToString(CultureInfo.InvariantCulture),
									4,
									(e, s) =>
									{
										if (Int32.TryParse(s, out var hue))
										{
											cat.Hue = Math.Max(0, Math.Min(2999, hue));
										}
									});

								AddButton(
									10,
									280,
									2714,
									2715,
									b =>
									{
										cat.UseDefaults();
										Refresh(true);
									});
								AddLabelCropped(30, 280, 160, 20, HighlightHue, "Use Defaults");

								if (Codex.CanRemoveEntries(User))
								{
									AddButton(
										10,
										300,
										2708,
										2709,
										b =>
										{
											cat.Empty();
											Codex.InvalidateProperties();
											Refresh(true);
										});
									AddLabelCropped(30, 300, 160, 20, ErrorHue, "Empty Entries");
								}

								if (Codex.CanRemoveCategories(User) && cat != Codex.Categories[0, 0])
								{
									AddButton(
										10,
										320,
										2708,
										2709,
										b =>
										{
											ui = UI;

											if (Codex.Remove(cat))
											{
												ui.Category = null;
											}

											Refresh(true);
										});
									AddLabelCropped(30, 320, 160, 20, ErrorHue, "Remove");
								}
							}
							else
							{
								AddLabelCropped(10, 50, 180, 20, cat.Hue, GetCategoryLabel(cat));
								AddHtml(10, 80, 190, 110, cat.ToHtmlString(User), false, true);

								var cost = Codex.CloneEntryChargeCost * cat.Entries.Count;

								AddButton(
									10,
									200,
									2714,
									2715,
									b =>
									{
										Codex.Drop(User, cat, true);
										Refresh(true);
									});
								AddLabelCropped(
									30,
									200,
									160,
									20,
									HighlightHue,
									String.Format("Clone ({0:#,0} charge{1})", cost, cost != 1 ? "s" : String.Empty));

								if (Codex.CanRemoveCategories(User) && cat != Codex.Categories[0, 0])
								{
									AddButton(
										10,
										320,
										2708,
										2709,
										b =>
										{
											ui = UI;

											if (Codex.Remove(cat))
											{
												ui.Category = null;
											}

											Refresh(true);
										});
									AddLabelCropped(30, 320, 160, 20, ErrorHue, "Remove");
								}
							}
						}
						break;
						case RuneCodex.UICache.ViewMode.Entries:
						{
							if (ui.Entry == null)
							{
								AddHtml(
									10,
									50,
									190,
									h - 10,
									"Select or add a rune using the grid on the right.\n\nYou can add:\n* Marked Recall Runes\n* Rune Books\n\nAdding an item will extract the location(s) and destroy the item.\n\nYou can also drop items directly to this codex.\nThey will be added to the last category you selected.\n\nClone: Create a recall rune for the selected rune.\nThe rune is not removed from the codex."
										.WrapUOHtmlColor(Color.LawnGreen),
									false,
									true);
								break;
							}

							var entry = ui.Entry;

							if (ui.EditMode)
							{
								AddLabelCropped(10, 50, 180, 20, HighlightHue, "Name: (20 Chars)");
								AddTextEntryLimited(10, 80, 180, 20, TextHue, entry.Name, 20, (e, s) => entry.Name = s);

								AddLabelCropped(10, 110, 180, 20, HighlightHue, "Description: (60 Chars)");
								AddTextEntryLimited(10, 130, 180, 60, TextHue, entry.Description, 60, (e, s) => entry.Description = s);

								AddButton(
									10,
									300,
									2714,
									2715,
									b =>
									{
										entry.UseDefaults();
										Codex.InvalidateProperties();
										Refresh(true);
									});
								AddLabelCropped(30, 300, 160, 20, HighlightHue, "Use Defaults");

								if (Codex.CanRemoveEntries(User))
								{
									AddButton(
										10,
										320,
										2708,
										2709,
										b =>
										{
											ui = UI;

											if (ui.Category.Remove(entry))
											{
												ui.Entry = null;
												Codex.InvalidateProperties();
											}

											Refresh(true);
										});
									AddLabelCropped(30, 320, 160, 20, ErrorHue, "Remove");
								}
							}
							else
							{
								AddLabelCropped(10, 50, 180, 20, ui.Category.Hue, GetEntryLabel(ui.Entry));
								AddHtml(10, 80, 190, 110, ui.Entry.ToHtmlString(User), false, true);

								AddButton(
									10,
									200,
									2714,
									2715,
									b =>
									{
										Codex.Recall(User, entry, true);
										Minimized = true;
										Refresh(true);
									});
								AddLabelCropped(
									30,
									200,
									160,
									20,
									HighlightHue,
									String.Format(
										"Recall ({0:#,0} charge{1})",
										Codex.RecallChargeCost,
										Codex.RecallChargeCost != 1 ? "s" : String.Empty));

								AddButton(
									10,
									220,
									2714,
									2715,
									b =>
									{
										Codex.Gate(User, entry, true);
										Minimized = true;
										Refresh(true);
									});
								AddLabelCropped(
									30,
									220,
									160,
									20,
									HighlightHue,
									String.Format(
										"Gate ({0:#,0} charge{1})",
										Codex.GateChargeCost,
										Codex.GateChargeCost != 1 ? "s" : String.Empty));

								AddButton(
									10,
									240,
									2714,
									2715,
									b =>
									{
										Codex.Drop(User, entry, true);
										Refresh(true);
									});
								AddLabelCropped(
									30,
									240,
									160,
									20,
									HighlightHue,
									String.Format(
										"Clone ({0:#,0} charge{1})",
										Codex.CloneEntryChargeCost,
										Codex.CloneEntryChargeCost != 1 ? "s" : String.Empty));

								if (Codex.CanRemoveEntries(User))
								{
									AddButton(
										10,
										320,
										2708,
										2709,
										b =>
										{
											ui = UI;

											if (ui.Category.Remove(entry))
											{
												ui.Entry = null;
												Codex.InvalidateProperties();
											}

											Refresh(true);
										});
									AddLabelCropped(30, 320, 160, 20, ErrorHue, "Remove");
								}
							}
						}
						break;
					}
				});

			CompileGrid(layout);

			layout.Add(
				"scrollX",
				() =>
				{
					ui = UI;

					switch (ui.Mode)
					{
						case RuneCodex.UICache.ViewMode.Categories:
							AddScrollbarH(
								200,
								38 + h,
								Math.Max(0, (Codex.Categories.Width + 1) - ScrollWidth),
								ui.CategoryScroll.X,
								b => ScrollLeft(),
								b => ScrollRight(),
								new Rectangle(30, 0, w - 60, 16),
								new Rectangle(7, 0, 16, 16),
								new Rectangle(w - 25, 0, 16, 16),
								Tuple.Create(9354, 9304),
								Tuple.Create(5607, 5603, 5607),
								Tuple.Create(5605, 5601, 5605));
							break;
						case RuneCodex.UICache.ViewMode.Entries:
							AddScrollbarH(
								200,
								38 + h,
								Math.Max(0, (ui.Category.Entries.Width + 1) - ScrollWidth),
								ui.EntryScroll.X,
								b => ScrollLeft(),
								b => ScrollRight(),
								new Rectangle(30, 0, w - 60, 16),
								new Rectangle(7, 0, 16, 16),
								new Rectangle(w - 25, 0, 16, 16),
								Tuple.Create(9354, 9304),
								Tuple.Create(5607, 5603, 5607),
								Tuple.Create(5605, 5601, 5605));
							break;
					}
				});

			layout.Add(
				"scrollY",
				() =>
				{
					ui = UI;

					switch (ui.Mode)
					{
						case RuneCodex.UICache.ViewMode.Categories:
							AddScrollbarV(
								206 + w,
								30,
								Math.Max(0, (Codex.Categories.Height + 1) - ScrollHeight),
								ui.CategoryScroll.Y,
								b => ScrollUp(),
								b => ScrollDown(),
								new Rectangle(0, 30, 16, h - 60),
								new Rectangle(0, 10, 16, 16),
								new Rectangle(0, h - 25, 16, 16),
								Tuple.Create(9354, 9304),
								Tuple.Create(5604, 5600, 5604),
								Tuple.Create(5606, 5602, 5606));
							break;
						case RuneCodex.UICache.ViewMode.Entries:
							AddScrollbarV(
								206 + w,
								30,
								Math.Max(0, (ui.Category.Entries.Height + 1) - ScrollHeight),
								ui.EntryScroll.Y,
								b => ScrollUp(),
								b => ScrollDown(),
								new Rectangle(0, 30, 16, h - 60),
								new Rectangle(0, 10, 16, 16),
								new Rectangle(0, h - 25, 16, 16),
								Tuple.Create(9354, 9304),
								Tuple.Create(5604, 5600, 5604),
								Tuple.Create(5606, 5602, 5606));
							break;
					}
				});

			layout.Add(
				"cancel",
				() =>
				{
					AddButton(w + 204, 4, 5538, 5539, OnCancel);
					AddTooltip(1006045);
				});

			if (ui.Mode == RuneCodex.UICache.ViewMode.Categories)
			{
				layout.Add(
					"accept",
					() =>
					{
						AddButton(w + 204, h + 34, 5541, 5542, OnAccept);
						AddTooltip(1006044);
					});
			}
		}

		protected virtual void CompileGrid(SuperGumpLayout layout)
		{
			var ui = UI;

			switch (ui.Mode)
			{
				case RuneCodex.UICache.ViewMode.Categories:
				{
					var cells = Codex.Categories.SelectCells(ui.CategoryScroll.X, ui.CategoryScroll.Y, ScrollWidth, ScrollHeight);

					var i = 0;

					for (var y = 0; y < ScrollHeight; y++)
					{
						for (var x = 0; x < ScrollWidth; x++)
						{
							CompileCategory(layout, x, y, i++, x < cells.Length && y < cells[x].Length ? cells[x][y] : null);
						}
					}
				}
				break;
				case RuneCodex.UICache.ViewMode.Entries:
				{
					var cells = ui.Category.Entries.SelectCells(ui.EntryScroll.X, ui.EntryScroll.Y, ScrollWidth, ScrollHeight);

					var i = 0;

					for (var y = 0; y < ScrollHeight; y++)
					{
						for (var x = 0; x < ScrollWidth; x++)
						{
							CompileEntry(layout, x, y, i++, x < cells.Length && y < cells[x].Length ? cells[x][y] : null);
						}
					}
				}
				break;
			}
		}

		protected virtual void CompileCategory(SuperGumpLayout layout, int x, int y, int idx, RuneCodexCategory cat)
		{
			if (x >= Codex.Categories.Width || y >= Codex.Categories.Height)
			{
				return;
			}

			layout.Add(
				"cat/" + idx,
				() =>
				{
					var ui = UI;

					var xOffset = 220 + (x * 70);
					var yOffset = 50 + (y * 70);
					var gx = x + ui.CategoryScroll.X;
					var gy = y + ui.CategoryScroll.Y;

					const int itemID = 8901;
					var s = cat != null && ui.Category == cat;

					if (cat != null)
					{
						AddButton(xOffset + 5, yOffset + 5, 24024, 24024, b => SelectCategory(gx, gy, idx, cat));
					}
					else if (Codex.CanAddCategories(User))
					{
						AddButton(xOffset + 5, yOffset + 5, 24024, 24024, b => AddCategory(gx, gy, idx));
					}

					AddImageTiled(xOffset, yOffset, 60, 60, 2702);

					if (s)
					{
						AddItem(xOffset + 10, yOffset + 4, itemID, 2050);
						AddItem(xOffset + 10, yOffset + 2, itemID, 1);
					}

					if (cat != null)
					{
						AddItem(xOffset + 10, yOffset, itemID, cat.Hue);
						AddHtml(
							xOffset,
							yOffset + 25,
							60,
							40,
							GetCategoryLabel(cat).WrapUOHtmlTag("center").WrapUOHtmlColor(Color.LawnGreen, false),
							false,
							false);
					}
					else if (Codex.CanAddCategories(User))
					{
						AddImage(xOffset + 25, yOffset, 2511, HighlightHue);
						AddHtml(
							xOffset,
							yOffset + 25,
							60,
							40,
							"Add".WrapUOHtmlTag("center").WrapUOHtmlColor(Color.Yellow, false),
							false,
							false);
					}
				});
		}

		protected virtual void CompileEntry(SuperGumpLayout layout, int x, int y, int idx, RuneCodexEntry entry)
		{
			var ui = UI;

			if (x >= ui.Category.Entries.Width || y >= ui.Category.Entries.Height)
			{
				return;
			}

			layout.Add(
				"entry/" + idx,
				() =>
				{
					ui = UI;

					var xOffset = 220 + (x * 70);
					var yOffset = 50 + (y * 70);
					var gx = x + ui.EntryScroll.X;
					var gy = y + ui.EntryScroll.Y;

					const int itemID = 7956;
					var s = entry != null && ui.Entry == entry;

					if (entry != null)
					{
						AddButton(xOffset + 5, yOffset + 5, 24024, 24024, b => SelectEntry(gx, gy, idx, entry));
					}
					else if (Codex.CanAddEntries(User))
					{
						AddButton(xOffset + 5, yOffset + 5, 24024, 24024, b => AddEntry(gx, gy, idx));
					}

					AddImageTiled(xOffset, yOffset, 60, 60, 2702);

					if (s)
					{
						AddItem(xOffset + 10, yOffset + 4, itemID, 2050);
						AddItem(xOffset + 10, yOffset + 2, itemID, 1);
					}

					if (entry != null)
					{
						AddItem(xOffset + 10, yOffset, itemID, ui.Category.Hue);
						AddHtml(
							xOffset,
							yOffset + 25,
							60,
							40,
							GetEntryLabel(entry).WrapUOHtmlTag("center").WrapUOHtmlColor(Color.LawnGreen, false),
							false,
							false);
					}
					else if (Codex.CanAddEntries(User))
					{
						AddImage(xOffset + 25, yOffset, 2511, HighlightHue);
						AddHtml(
							xOffset,
							yOffset + 25,
							60,
							40,
							"Add".WrapUOHtmlTag("center").WrapUOHtmlColor(Color.Yellow, false),
							false,
							false);
					}
				});
		}

		protected virtual void AddCategory(int gx, int gy, int idx)
		{
			var ui = UI;

			ui.CategoryPoint = new Point2D(gx, gy);

			SelectCategory(gx, gy, idx, Codex.Categories[gx, gy] ?? (Codex.Categories[gx, gy] = new RuneCodexCategory()));
			Codex.InvalidateProperties();
		}

		protected virtual void AddEntry(int gx, int gy, int idx)
		{
			var ui = UI;

			ui.EntryPoint = new Point2D(gx, gy);

			User.SendMessage("Select a rune book or recall rune to add to the Codex...");

			Minimized = true;
			Refresh(true);

			ItemSelectTarget<Item>.Begin(
				User,
				(m, t) =>
				{
					if (Codex.Add(User, t, ui.Category, true))
					{
						Minimized = false;
						SelectEntry(gx, gy, idx, ui.Category.Entries[gx, gy]);
					}
					else
					{
						AddEntry(gx, gy, idx);
					}
				},
				m =>
				{
					Minimized = false;
					Refresh(true);
				});
		}

		protected virtual void SelectCategory(int gx, int gy, int idx, RuneCodexCategory cat)
		{
			var ui = UI;

			ui.CategoryPoint = new Point2D(gx, gy);
			ui.Category = cat;

			OnSelected(gx, gy, idx, cat);
		}

		protected virtual void SelectEntry(int gx, int gy, int idx, RuneCodexEntry entry)
		{
			var ui = UI;

			ui.EntryPoint = new Point2D(gx, gy);
			ui.Entry = entry;

			OnSelected(gx, gy, idx, entry);
		}

		protected virtual void OnSelected(int gx, int gy, int idx, RuneCodexCategory cat)
		{
			if (DoubleClicked)
			{
				OnAccept(null);
				return;
			}

			Refresh(true);
		}

		protected virtual void OnSelected(int gx, int gy, int idx, RuneCodexEntry entry)
		{
			if (DoubleClicked && Codex.Recall(User, entry, true))
			{
				Minimized = true;
				Refresh(true);
				return;
			}

			Refresh(true);
		}

		public virtual void OnCancel(GumpButton b)
		{
			var ui = UI;

			switch (ui.Mode)
			{
				case RuneCodex.UICache.ViewMode.Categories:
					Close();
					break;
				case RuneCodex.UICache.ViewMode.Entries:
				{
					ui.Mode = RuneCodex.UICache.ViewMode.Categories;
					Refresh(true);
				}
				break;
			}
		}

		public virtual void OnAccept(GumpButton b)
		{
			var ui = UI;

			ui.Mode = RuneCodex.UICache.ViewMode.Entries;
			Refresh(true);
		}

		public virtual void ScrollLeft()
		{
			var ui = UI;

			switch (ui.Mode)
			{
				case RuneCodex.UICache.ViewMode.Categories:
					ui.CategoryScroll = ui.CategoryScroll.Clone2D(-1);
					break;
				case RuneCodex.UICache.ViewMode.Entries:
					ui.EntryScroll = ui.EntryScroll.Clone2D(-1);
					break;
			}

			Refresh(true);
		}

		public virtual void ScrollRight()
		{
			var ui = UI;

			switch (ui.Mode)
			{
				case RuneCodex.UICache.ViewMode.Categories:
					ui.CategoryScroll = ui.CategoryScroll.Clone2D(1);
					break;
				case RuneCodex.UICache.ViewMode.Entries:
					ui.EntryScroll = ui.EntryScroll.Clone2D(1);
					break;
			}

			Refresh(true);
		}

		public virtual void ScrollUp()
		{
			var ui = UI;

			switch (ui.Mode)
			{
				case RuneCodex.UICache.ViewMode.Categories:
					ui.CategoryScroll = ui.CategoryScroll.Clone2D(0, -1);
					break;
				case RuneCodex.UICache.ViewMode.Entries:
					ui.EntryScroll = ui.EntryScroll.Clone2D(0, -1);
					break;
			}

			Refresh(true);
		}

		public virtual void ScrollDown()
		{
			var ui = UI;

			switch (ui.Mode)
			{
				case RuneCodex.UICache.ViewMode.Categories:
					ui.CategoryScroll = ui.CategoryScroll.Clone2D(0, 1);
					break;
				case RuneCodex.UICache.ViewMode.Entries:
					ui.EntryScroll = ui.EntryScroll.Clone2D(0, 1);
					break;
			}

			Refresh(true);
		}

		public virtual void Minimize(GumpButton b)
		{
			Minimized = true;
			Refresh(true);
		}

		public virtual void Maximize(GumpButton b)
		{
			Minimized = false;
			Refresh(true);
		}

		public virtual string GetCategoryLabel(RuneCodexCategory cat)
		{
			return cat != null ? cat.Name : String.Empty;
		}

		public virtual string GetEntryLabel(RuneCodexEntry entry)
		{
			return entry != null ? entry.Name : String.Empty;
		}
	}
}