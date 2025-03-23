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

using Server.Gumps;
using Server.Mobiles;

using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Modules.Toolbar
{
	public class ToolbarGump : ListGump<ToolbarEntry>
	{
		public static string DefaultToolbarTitle = "Toolbar";

		public ToolbarState State { get; protected set; }
		public bool GlobalEdit { get; protected set; }
		public ToolbarThemeBase Theme { get; set; }
		public Color HeaderColor { get; set; }

		public override bool Minimized
		{
			get => State != null ? State.Minimized : base.Minimized;
			set
			{
				if (State != null)
				{
					State.Minimized = value;
				}

				base.Minimized = value;
			}
		}

		public ToolbarGump(ToolbarState state, Color? headerColor = null, ToolbarTheme theme = ToolbarTheme.Default)
			: base(state.User, null, state.X, state.Y, null, null, DefaultToolbarTitle)
		{
			State = state;
			HeaderColor = headerColor ?? Color.DarkBlue;
			GlobalEdit = false;
			Theme = ToolbarThemes.GetTheme(theme);

			CanSearch = false;
			CanMove = false;
			CanDispose = false;
			CanClose = false;
			CanResize = false;
		}

		public virtual bool CanGlobalEdit()
		{
			return (User is PlayerMobile && User.AccessLevel >= Toolbars.Access &&
					(Toolbars.DefaultEntries.User == User || Toolbars.DefaultEntries.User == null));
		}

		public virtual void BeginGlobalEdit()
		{
			if (CanGlobalEdit())
			{
				if (!GlobalEdit || State != Toolbars.DefaultEntries)
				{
					GlobalEdit = true;
					State = Toolbars.DefaultEntries;
					State.User = User as PlayerMobile;
				}

				Refresh(true);
			}
			else
			{
				EndGlobalEdit();
			}
		}

		public virtual void EndGlobalEdit()
		{
			if (State == Toolbars.DefaultEntries)
			{
				State.User = null;
				State = Toolbars.EnsureState(User as PlayerMobile);
			}

			GlobalEdit = false;
			Refresh(true);
		}

		protected virtual void ShowPositionSelect(GumpButton b)
		{
			if (!(User is PlayerMobile))
			{
				return;
			}

			var user = (PlayerMobile)User;

			new OffsetSelectorGump(
				user,
				Refresh(true),
				Toolbars.GetOffset(user),
				(self, oldValue) =>
				{
					Toolbars.SetOffset(user, self.Value);
					X = self.Value.X;
					Y = self.Value.Y;
					Refresh(true);
				}).Send();
		}

		public override SuperGump Refresh(bool recompile)
		{
			if (!CanGlobalEdit())
			{
				GlobalEdit = false;
			}

			return base.Refresh(recompile);
		}

		protected override void SelectEntry(GumpButton button, ToolbarEntry entry)
		{
			base.SelectEntry(button, entry);

			if (entry != null && entry.ValidateState(State))
			{
				entry.Invoke(State);
			}

			Refresh();
		}

		protected virtual void SelectEntryMenu(GumpButton button, Point loc, ToolbarEntry entry)
		{
			if (entry != null)
			{
				entry.Edit(this, loc, button);
			}
			else
			{
				var opts = new MenuGumpOptions();

				if (!GlobalEdit)
				{
					opts.AppendEntry(
						"Load Default",
						b => new ConfirmDialogGump(User, this)
						{
							Title = "Load Default",
							Html = "Loading the default entry will overwrite your custom entry.\n\nDo you want to continue?",
							AcceptHandler = db =>
							{
								var def = Toolbars.DefaultEntries.GetContent(loc.X, loc.Y);

								State.SetContent(loc.X, loc.Y, def != null ? def.Clone() : null);

								Refresh(true);
							}
						}.Send(),
						HighlightHue);
				}

				foreach (var eType in Toolbars.EntryTypes)
				{
					var eName = "New " + eType.Name.Replace("Toolbar", String.Empty);

					var type = eType;

					opts.AppendEntry(
						eName,
						b =>
						{
							State.SetContent(loc.X, loc.Y, CreateToolbarEntry(type));
							Refresh(true);
						},
						HighlightHue);
				}

				new MenuGump(User, this, opts, button).Send();
			}
		}

		protected virtual ToolbarEntry CreateToolbarEntry(Type type)
		{
			return type.CreateInstanceSafe<ToolbarEntry>();
		}

		protected override void Compile()
		{
			Theme = ToolbarThemes.GetTheme(State.Theme);
			base.Compile();
		}

		protected override void CompileMenuOptions(MenuGumpOptions list)
		{
			list.Clear();

			if (CanGlobalEdit())
			{
				if (GlobalEdit)
				{
					list.AppendEntry("End Global Edit", b => EndGlobalEdit(), ErrorHue);

					list.AppendEntry("Edit Defaults", b => User.SendGump(new PropertiesGump(User, Toolbars.CMOptions)), HighlightHue);

					list.AppendEntry(
						"Reset Global Entries",
						b => new ConfirmDialogGump(User, this)
						{
							Title = "Reset Global Entries",
							Html = "Applying global defaults will copy the global toolbar to all existing toolbars.\n" +
								   "This will overwrite any custom entries that exist.\n\nDo you want to continue?",
							AcceptHandler = db =>
							{
								Toolbars.SetGlobalEntries();
								Refresh(true);
							}
						}.Send(),
						HighlightHue);

					list.AppendEntry(
						"Reset Global Themes",
						b => new ConfirmDialogGump(User, this)
						{
							Title = "Reset Global Themes",
							Html = "Applying global theme will reset the theme of all existing toolbars.\n\n" + //
								   "Do you want to continue?",
							AcceptHandler = db =>
							{
								Toolbars.SetGlobalTheme();
								Refresh(true);
							}
						}.Send(),
						HighlightHue);

					list.AppendEntry(
						"Reset Global Positions",
						b => new ConfirmDialogGump(
							User,
							this,
							title: "Reset Global Positions",
							html: "Applying global position will reset the position of all existing toolbars.\n\n" + //
								  "Do you want to continue?",
							onAccept: db =>
							{
								Toolbars.SetGlobalPosition();
								Refresh(true);
							}).Send(),
						HighlightHue);

					list.AppendEntry(
						"Reset Global Sizes",
						b => new ConfirmDialogGump(User, this)
						{
							Title = "Reset Global Sizes",
							Html = "Applying global size will reset the size of all existing toolbars.\n" +
								   "Any entries located beyond the new size will be lost.\n\n" + //
								   "Do you want to continue?",
							AcceptHandler = db =>
							{
								Toolbars.SetGlobalSize();
								Refresh(true);
							}
						}.Send(),
						HighlightHue);
				}
				else
				{
					list.AppendEntry("Begin Global Edit", b => BeginGlobalEdit(), HighlightHue);
				}
			}

			list.AppendEntry(
				"Load Defaults",
				b => new ConfirmDialogGump(User, this)
				{
					Title = "Load Defaults",
					Html = "Loadng defaults will overwrite any custom entries that exist in your toolbar.\n\n" + //
						   "Do you want to continue?",
					AcceptHandler = db =>
					{
						State.SetDefaultEntries();
						Refresh(true);
					}
				}.Send(),
				HighlightHue);

			list.AppendEntry(
				"Set Position",
				b => new OffsetSelectorGump(
					User,
					this,
					new Point(State.X, State.Y),
					(self, oldValue) =>
					{
						X = State.X = self.Value.X;
						Y = State.Y = self.Value.Y;
						Refresh(true);
					}).Send(),
				HighlightHue);

			list.AppendEntry(
				"Set Size",
				b =>
				{
					var html = String.Format(
						"Set the size for your toolbar.\nFormat: Width,Height\nWidth Range: {0}\nHeight Range: {1}\n\nIf you shrink the size, any entires located beyond the new size will be lost.",
						String.Format("{0}-{1}", Toolbars.CMOptions.DefaultWidth, Toolbars.DefaultEntries.Width),
						String.Format("{0}-{1}", Toolbars.CMOptions.DefaultHeight, Toolbars.DefaultEntries.Height));

					new InputDialogGump(User, this)
					{
						Title = "Set Size",
						Html = html,
						InputText = String.Format("{0},{1}", State.Width, State.Height),
						Callback = (cb, text) =>
						{
							int w = State.Width, h = State.Height;

							if (text.IndexOf(",", StringComparison.Ordinal) != -1)
							{
								var split = text.Split(',');

								if (split.Length >= 2)
								{
									if (Int32.TryParse(split[0], out w))
									{
										if (w < Toolbars.CMOptions.DefaultWidth)
										{
											w = Toolbars.CMOptions.DefaultWidth;
										}
										else if (!GlobalEdit && w > Toolbars.DefaultEntries.Width)
										{
											w = Toolbars.DefaultEntries.Width;
										}
									}
									else
									{
										w = State.Width;
									}

									if (Int32.TryParse(split[1], out h))
									{
										if (h < Toolbars.CMOptions.DefaultHeight)
										{
											h = Toolbars.CMOptions.DefaultHeight;
										}
										else if (!GlobalEdit && h > Toolbars.DefaultEntries.Height)
										{
											h = Toolbars.DefaultEntries.Height;
										}
									}
									else
									{
										h = State.Height;
									}
								}
							}

							State.Resize(w, h);
							Refresh(true);
						}
					}.Send();
				},
				HighlightHue);

			list.AppendEntry(
				"Set Theme",
				b =>
				{
					var opts = new MenuGumpOptions();

					var themes = default(ToolbarTheme).EnumerateValues<ToolbarTheme>(false);

					foreach (var themeID in themes)
					{
						if (State.Theme == themeID)
						{
							continue;
						}

						var id = themeID;
						var theme = ToolbarThemes.GetTheme(themeID);

						opts.AppendEntry(
							theme.Name,
							tb =>
							{
								State.Theme = id;
								Refresh(true);
							},
							HighlightHue);
					}

					new MenuGump(User, this, opts, b).Send();
				},
				HighlightHue);

			base.CompileMenuOptions(list);

			list.RemoveEntry("New Search");
			list.RemoveEntry("Clear Search");

			list.Replace("Refresh", "Exit", b => Close(b));
		}

		protected override string GetLabelText(int index, int pageIndex, ToolbarEntry entry)
		{
			string label;

			if (entry != null)
			{
				var labelColor = (entry.LabelColor ?? (entry.Highlight ? Theme.EntryLabelColorH : Theme.EntryLabelColorN));

				label = String.Format("<basefont color=#{0:X6}><center>{1}</center>", labelColor.ToRgb(), entry.GetDisplayLabel());
			}
			else
			{
				label = String.Format("<basefont color=#{0:X6}><center>{1}</center>", Theme.EntryLabelColorN.ToRgb(), "*Unused*");
			}

			return label;
		}

		protected override void CompileList(List<ToolbarEntry> list)
		{
			list.Clear();
			list.AddRange(State.GetCells());

			base.CompileList(list);

			EntriesPerPage = list.Count;
		}

		public override string GetSearchKeyFor(ToolbarEntry key)
		{
			if (key == null)
			{
				return base.GetSearchKeyFor(null);
			}

			if (!String.IsNullOrWhiteSpace(key.Label))
			{
				return key.Label;
			}

			return key.Value;
		}

		protected override void CompileLayout(SuperGumpLayout layout)
		{
			layout.Replace(
				"button/header/minmax",
				() =>
				{
					if (Minimized)
					{
						AddButton(0, 0, 2328, 2329, Maximize);
						AddTooltip(3002086);
					}
					else
					{
						AddButton(0, 0, 2328, 2329, Minimize);
						AddTooltip(3002085);
					}
				});

			layout.Add("imagetiled/header/base", () => AddImageTiled(0, 0, 84, 56, Theme.TitleBackground));

			layout.Replace(
				"html/header/title",
				() => AddHtml(
					0,
					0,
					84,
					56,
					String.Format(
						"<basefont color=#{0:X6}><center><big>{1}</big></center>",
						Theme.TitleLabelColor.ToRgb(),
						String.Format(
							"{0} {1}",
							String.IsNullOrWhiteSpace(Title) ? DefaultTitle : Title,
							GlobalEdit ? "[GLOBAL]" : String.Empty)),
					false,
					false));

			layout.Replace(
				"button/header/options",
				() =>
				{
					AddButton(84, 0, Theme.EntryOptionsN, Theme.EntryOptionsP, ShowPositionSelect);
					AddButton(84, 28, Theme.EntryOptionsN, Theme.EntryOptionsP, ShowOptionMenu);
				});

			if (Minimized)
			{
				return;
			}

			var ec = IsEnhancedClient;

			var index = 0;

			State.ForEach(
				(x, y, entry) =>
				{
					var idx = index;
					var loc = new Point(x, y);

					if (ec)
					{
						layout.Add("ec/1/" + idx, AddInputEC);
					}

					layout.Add(
						"button1/entry/" + idx,
						() => AddButton(110 + (loc.X * 130), (loc.Y * 28), 2445, 2445, b => SelectEntry(b, entry)));

					layout.Add(
						"imagetiled/entry/" + idx,
						() =>
						{
							AddImageTiled(
								106 + (loc.X * 130),
								(loc.Y * 28),
								112,
								28,
								(entry != null && entry.Highlight) ? Theme.EntryBackgroundH : Theme.EntryBackgroundN);
							AddImageTiled(106 + (loc.X * 130) + 112, (loc.Y * 28), 18, 28, Theme.EntrySeparator);
						});

					layout.Add(
						"html/entry/" + idx,
						() => AddHtml(
							106 + (loc.X * 130) + 3,
							(loc.Y * 28) + 3,
							112 - 6,
							28 - 6,
							GetLabelText(idx, idx, entry),
							false,
							false));

					layout.Add(
						"button2/entry/" + idx,
						() => AddButton(
							106 + (loc.X * 130) + 112,
							(loc.Y * 28),
							Theme.EntryOptionsN,
							Theme.EntryOptionsP,
							b =>
							{
								Refresh();
								SelectEntryMenu(b, loc, entry);
							}));

					if (ec)
					{
						layout.Add("ec/2/" + idx, AddInputEC);
					}

					index++;
				});
		}
	}
}