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
using Server.Items;
using Server.Spells;

using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
#endregion

namespace VitaNex.Modules.Toolbar
{
	public class ToolbarSpell : ToolbarEntry, IEquatable<ToolbarSpell>
	{
		[CommandProperty(Toolbars.Access)]
		public virtual int SpellID { get; set; }

		public ToolbarSpell()
		{
			SpellID = -1;
		}

		public ToolbarSpell(
			int spellID = -1,
			string label = null,
			bool canDelete = true,
			bool canEdit = true,
			bool highlight = false,
			Color? labelColor = null)
			: base(String.Empty, label, canDelete, canEdit, highlight, labelColor)
		{
			SpellID = spellID;
		}

		public ToolbarSpell(GenericReader reader)
			: base(reader)
		{ }

		public override string GetDisplayLabel()
		{
			return "*" + base.GetDisplayLabel() + "*";
		}

		protected override void CompileOptions(ToolbarGump toolbar, GumpButton clicked, Point loc, MenuGumpOptions opts)
		{
			if (toolbar == null)
			{
				return;
			}

			base.CompileOptions(toolbar, clicked, loc, opts);

			var user = toolbar.State.User;

			if (CanEdit || user.AccessLevel >= Toolbars.Access)
			{
				opts.Replace(
					"Set Value",
					new ListGumpEntry(
						"Set Spell",
						b =>
						{
							toolbar.Refresh(true);
							MenuGump menu1 = null;
							var menuOpts1 = new MenuGumpOptions();

							foreach (var kvp1 in SpellUtility.TreeStructure)
							{
								var circle = kvp1.Key;
								var types = kvp1.Value;

								menuOpts1.AppendEntry(
									new ListGumpEntry(
										circle,
										b2 =>
										{
											var menuOpts2 = new MenuGumpOptions();

											foreach (var kvp2 in types)
											{
												var id = SpellRegistry.GetRegistryNumber(kvp2.Key);
												var si = kvp2.Value;
												var book = Spellbook.Find(user, id);

												if (book != null && book.HasSpell(id))
												{
													menuOpts2.AppendEntry(
														new ListGumpEntry(
															si.Name,
															menu2Button =>
															{
																SpellID = id;
																Value = si.Name;
																Label = String.Empty;
																toolbar.Refresh(true);
															},
															(SpellID == id) ? toolbar.HighlightHue : toolbar.TextHue));
												}
											}

											if (menu1 != null)
											{
												SuperGump.Send(new MenuGump(user, clicked.Parent, menuOpts2, clicked));
											}
										}));
							}

							menu1 = new MenuGump(user, clicked.Parent, menuOpts1, clicked);
							SuperGump.Send(menu1);
						},
						toolbar.HighlightHue));
			}
		}

		protected override void OnCloned(ToolbarEntry clone)
		{
			base.OnCloned(clone);

			var spell = clone as ToolbarSpell;

			if (spell == null)
			{
				return;
			}

			spell.SpellID = SpellID;
		}

		public override void Invoke(ToolbarState state)
		{
			if (state == null || SpellID < 0 || SpellID >= SpellRegistry.Types.Length)
			{
				return;
			}

			var user = state.User;

			if (user != null && !user.Deleted && user.NetState != null)
			{
				EventSink.InvokeCastSpellRequest(new CastSpellRequestEventArgs(user, SpellID, null));
			}
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (base.GetHashCode() * 397) ^ SpellID;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is ToolbarSpell && Equals((ToolbarSpell)obj);
		}

		public bool Equals(ToolbarSpell other)
		{
			return !ReferenceEquals(other, null) && base.Equals(other) && SpellID == other.SpellID;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
					writer.Write(SpellID);
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.ReadInt();

			switch (version)
			{
				case 0:
					SpellID = reader.ReadInt();
					break;
			}
		}

		public static bool operator ==(ToolbarSpell l, ToolbarSpell r)
		{
			return ReferenceEquals(l, null) ? ReferenceEquals(r, null) : l.Equals(r);
		}

		public static bool operator !=(ToolbarSpell l, ToolbarSpell r)
		{
			return ReferenceEquals(l, null) ? !ReferenceEquals(r, null) : !l.Equals(r);
		}
	}
}