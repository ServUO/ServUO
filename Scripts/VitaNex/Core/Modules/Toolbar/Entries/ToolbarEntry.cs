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

using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Modules.Toolbar
{
	[PropertyObject]
	public abstract class ToolbarEntry : IEquatable<ToolbarEntry>
	{
		public ToolbarEntry()
			: this(String.Empty)
		{ }

		public ToolbarEntry(
			string value,
			string label = null,
			bool canDelete = true,
			bool canEdit = true,
			bool highlight = false,
			Color? labelColor = null)
		{
			Value = value;
			Label = String.IsNullOrEmpty(label) ? Value : label;
			CanDelete = canDelete;
			CanEdit = canEdit;
			Highlight = highlight;
			LabelColor = labelColor;
		}

		public ToolbarEntry(GenericReader reader)
		{
			Deserialize(reader);
		}

		[CommandProperty(Toolbars.Access)]
		public virtual string Value { get; set; }

		[CommandProperty(Toolbars.Access)]
		public virtual string Label { get; set; }

		[CommandProperty(Toolbars.Access)]
		public virtual Color? LabelColor { get; set; }

		[CommandProperty(Toolbars.Access)]
		public virtual bool CanEdit { get; set; }

		[CommandProperty(Toolbars.Access)]
		public virtual bool CanDelete { get; set; }

		[CommandProperty(Toolbars.Access)]
		public virtual bool Highlight { get; set; }

		[CommandProperty(Toolbars.Access)]
		public virtual string FullValue => String.Format("{0}", Value);

		public virtual string GetDisplayLabel()
		{
			if (!String.IsNullOrWhiteSpace(Label))
			{
				return Label;
			}

			if (!String.IsNullOrWhiteSpace(Value))
			{
				return Value;
			}

			return "*Empty*";
		}

		public virtual ToolbarEntry Clone()
		{
			var clone = Activator.CreateInstance(GetType()) as ToolbarEntry;

			if (clone != null)
			{
				OnCloned(clone);
			}

			return clone;
		}

		protected virtual void OnCloned(ToolbarEntry clone)
		{
			if (clone == null)
			{
				return;
			}

			clone.Value = Value;
			clone.Label = Label;
			clone.CanDelete = CanDelete;
			clone.CanEdit = CanEdit;
			clone.LabelColor = LabelColor;
			clone.Highlight = Highlight;
		}

		public virtual MenuGumpOptions GetOptions(ToolbarGump toolbar, GumpButton clicked, Point loc)
		{
			var opts = new MenuGumpOptions();

			if (toolbar == null)
			{
				return opts;
			}

			CompileOptions(toolbar, clicked, loc, opts);
			return opts;
		}

		protected virtual void CompileOptions(ToolbarGump toolbar, GumpButton clicked, Point loc, MenuGumpOptions opts)
		{
			if (toolbar == null)
			{
				return;
			}

			var user = toolbar.User;

			if (CanEdit || user.AccessLevel >= Toolbars.Access)
			{
				if (!toolbar.GlobalEdit)
				{
					opts.AppendEntry(
						new ListGumpEntry(
							"Load Default",
							b => SuperGump.Send(
								new ConfirmDialogGump(user, toolbar)
								{
									Title = "Load Default",
									Html = "Loading the default entry will overwrite your custom entry.\n\nDo you want to continue?",
									AcceptHandler = db =>
									{
										var def = Toolbars.DefaultEntries.GetContent(loc.X, loc.Y);

										toolbar.State.SetContent(loc.X, loc.Y, def != null ? def.Clone() : null);
										toolbar.Refresh(true);
									}
								}),
							toolbar.HighlightHue));
				}

				opts.AppendEntry(
					new ListGumpEntry(
						"Reset",
						b =>
						{
							Reset(toolbar);
							toolbar.Refresh(true);
						},
						toolbar.HighlightHue));
			}

			if (CanDelete || user.AccessLevel >= Toolbars.Access)
			{
				opts.AppendEntry(
					new ListGumpEntry(
						"Delete",
						b =>
						{
							toolbar.State.SetContent(loc.X, loc.Y, null);
							toolbar.Refresh(true);
						},
						toolbar.HighlightHue));
			}

			if (CanEdit || user.AccessLevel >= Toolbars.Access)
			{
				opts.AppendEntry(
					new ListGumpEntry(
						"Set Value",
						b => SuperGump.Send(
							new InputDialogGump(user, toolbar)
							{
								Title = "Set Value",
								Html = "Set the value of this entry.",
								InputText = Value,
								Callback = (cb, text) =>
								{
									Value = text;
									toolbar.Refresh(true);
								}
							}),
						toolbar.HighlightHue));

				opts.AppendEntry(
					new ListGumpEntry(
						"Set Label",
						b => SuperGump.Send(
							new InputDialogGump(user, toolbar)
							{
								Title = "Set Label",
								Html = "Set the label of this entry.",
								InputText = Label,
								Callback = (cb, text) =>
								{
									Label = text;
									toolbar.Refresh(true);
								}
							}),
						toolbar.HighlightHue));

				opts.AppendEntry(
					new ListGumpEntry(
						"Set Label Color",
						b =>
						{
							int rrr = 255, ggg = 255, bbb = 255;

							if (LabelColor != null)
							{
								rrr = LabelColor.Value.R;
								ggg = LabelColor.Value.G;
								bbb = LabelColor.Value.B;
							}

							SuperGump.Send(
								new InputDialogGump(user, toolbar)
								{
									Title = "Set Label Color",
									Html = "Set the label color for this entry.\nFormat 1: NamedColor (EG; Red)\nFormat 2: RRR,GGG,BBB",
									InputText = String.Format("{0:D3},{1:D3},{2:D3}", rrr, ggg, bbb),
									Callback = (cb, text) =>
									{
										if (!String.IsNullOrWhiteSpace(text))
										{
											if (text.IndexOf(',') != -1)
											{
												var args = text.Split(',');

												if (args.Length >= 3)
												{
													Int32.TryParse(args[0], out rrr);
													Int32.TryParse(args[1], out ggg);
													Int32.TryParse(args[2], out bbb);

													rrr = Math.Min(255, Math.Max(0, rrr));
													ggg = Math.Min(255, Math.Max(0, ggg));
													bbb = Math.Min(255, Math.Max(0, bbb));

													LabelColor = Color.FromArgb(rrr, ggg, bbb);
												}
											}
											else
											{
												try
												{
													LabelColor = Color.FromName(text);
												}
												catch
												{ }
											}
										}

										toolbar.Refresh(true);
									}
								});
						},
						toolbar.HighlightHue));

				if (Highlight)
				{
					opts.Replace(
						"Highlight",
						new ListGumpEntry(
							"Unhighlight",
							b =>
							{
								Highlight = false;
								toolbar.Refresh(true);
							},
							toolbar.ErrorHue));
				}
				else
				{
					opts.Replace(
						"Unhighlight",
						new ListGumpEntry(
							"Highlight",
							b =>
							{
								Highlight = true;
								toolbar.Refresh(true);
							},
							toolbar.HighlightHue));
				}
			}

			if (user.AccessLevel < Toolbars.Access)
			{
				return;
			}

			opts.AppendEntry(
				new ListGumpEntry(
					"Open Props",
					b => SuperGump.Send(
						new NoticeDialogGump(user, toolbar)
						{
							Title = "Props Note",
							Html =
								"Editing the properties of an entry this way requires a hard refresh.\nExit and re-open the Toolbar when you make any changes.",
							AcceptHandler = cb =>
							{
								toolbar.Refresh(true);

								var pg = new PropertiesGump(user, this)
								{
									X = b.X,
									Y = b.Y
								};
								user.SendGump(pg);
							}
						}),
					toolbar.HighlightHue));

			if (toolbar.GlobalEdit && toolbar.CanGlobalEdit())
			{
				opts.AppendEntry(
					new ListGumpEntry(
						"Global Apply",
						b => SuperGump.Send(
							new ConfirmDialogGump(user, toolbar)
							{
								Title = "Global Apply",
								Html =
									"Applying this entry globally will overwrite any custom entries at the entry location on all existing toolbars.\n\nDo you want to continue?",
								AcceptHandler = db =>
								{
									var def = Toolbars.DefaultEntries.GetContent(loc.X, loc.Y);

									foreach (var tbs in Toolbars.Profiles.Values)
									{
										try
										{
											tbs.SetContent(loc.X, loc.Y, def != null ? def.Clone() : null);

											var tb = tbs.GetToolbarGump();

											if (tb != null && tb.IsOpen)
											{
												tb.Refresh(true);
											}
										}
										catch
										{ }
									}
								}
							}),
						toolbar.HighlightHue));
			}
		}

		public virtual void Edit(ToolbarGump toolbar, Point loc, GumpButton clicked)
		{
			if (toolbar == null)
			{
				return;
			}

			var user = toolbar.State.User;

			if (user == null || user.Deleted || user.NetState == null)
			{
				return;
			}

			if (CanEdit || user.AccessLevel >= Toolbars.Access)
			{
				SuperGump.Send(new MenuGump(user, toolbar.Refresh(), GetOptions(toolbar, clicked, loc), clicked));
			}
		}

		public virtual void Reset(ToolbarGump state)
		{
			if (state == null)
			{
				return;
			}

			var user = state.User;

			if (!CanEdit && user.AccessLevel < Toolbars.Access)
			{
				return;
			}

			Value = String.Empty;
			Label = null;
			LabelColor = null;
		}

		public virtual bool ValidateState(ToolbarState state)
		{
			return state != null && state.User != null && !state.User.Deleted;
		}

		public abstract void Invoke(ToolbarState state);

		public override string ToString()
		{
			return FullValue;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = (Value != null ? Value.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Label != null ? Label.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ LabelColor.GetHashCode();
				hashCode = (hashCode * 397) ^ CanEdit.GetHashCode();
				hashCode = (hashCode * 397) ^ CanDelete.GetHashCode();
				hashCode = (hashCode * 397) ^ Highlight.GetHashCode();
				return hashCode;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is ToolbarEntry && Equals((ToolbarEntry)obj);
		}

		public bool Equals(ToolbarEntry other)
		{
			return !ReferenceEquals(other, null) && Value == other.Value && Label == other.Label &&
				   LabelColor == other.LabelColor && CanEdit == other.CanEdit && CanDelete == other.CanDelete &&
				   Highlight == other.Highlight;
		}

		public virtual void Serialize(GenericWriter writer)
		{
			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.Write(Value);
					writer.Write(Label);
					writer.Write(CanDelete);
					writer.Write(CanEdit);
					writer.Write(Highlight);

					if (LabelColor != null)
					{
						writer.Write(true);
						writer.Write(LabelColor.Value.ToArgb());
					}
					else
					{
						writer.Write(false);
					}
				}
				break;
			}
		}

		public virtual void Deserialize(GenericReader reader)
		{
			var version = reader.GetVersion();

			switch (version)
			{
				case 0:
				{
					Value = reader.ReadString();
					Label = reader.ReadString();
					CanDelete = reader.ReadBool();
					CanEdit = reader.ReadBool();
					Highlight = reader.ReadBool();

					if (reader.ReadBool())
					{
						LabelColor = Color.FromArgb(reader.ReadInt());
					}
				}
				break;
			}
		}

		public static bool operator ==(ToolbarEntry l, ToolbarEntry r)
		{
			return ReferenceEquals(l, null) ? ReferenceEquals(r, null) : l.Equals(r);
		}

		public static bool operator !=(ToolbarEntry l, ToolbarEntry r)
		{
			return ReferenceEquals(l, null) ? !ReferenceEquals(r, null) : !l.Equals(r);
		}
	}
}