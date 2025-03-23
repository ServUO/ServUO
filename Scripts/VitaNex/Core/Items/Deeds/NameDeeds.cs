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

using Server;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Targeting;

using VitaNex.Network;
using VitaNex.SuperGumps;
using VitaNex.SuperGumps.UI;
using VitaNex.Targets;
#endregion

namespace VitaNex.Items
{
	public static class NameDeeds
	{
		public static event Action<INameDeed> OnCreated;

		public static void Register<T>(Action<T> onCreated)
			where T : INameDeed
		{
			OnCreated += o =>
			{
				if (o is T d)
				{
					onCreated(d);
				}
			};
		}

		public static void InvokeCreated(INameDeed d)
		{
			if (OnCreated != null)
			{
				OnCreated(d);
			}
		}
	}

	public interface INameDeed
	{
		List<string> Names { get; }

		void AddName(string name);
		void AddNames(params string[] names);
	}

	public abstract class BaseNameDeed<TEntity> : Item, INameDeed
		where TEntity : IEntity
	{
		private SuperGump _Gump;
		private GenericSelectTarget<TEntity> _Target;

		public List<string> Names { get; private set; }

		public virtual string TargetUsage => "an object";

		public BaseNameDeed()
			: base(0x14F0)
		{
			Names = new List<string>();

			Name = "Rename Deed";
			Weight = 1.0;
			LootType = LootType.Blessed;

			Timer.DelayCall(NameDeeds.InvokeCreated, this);
		}

		public BaseNameDeed(Serial serial)
			: base(serial)
		{ }

		public void AddName(string name)
		{
			Names.Update(name);
		}

		public void AddNames(params string[] names)
		{
			if (names == null || names.Length == 0)
			{
				return;
			}

			foreach (var n in names)
			{
				AddName(n);
			}
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			new ExtendedOPL(list)
			{
				{"Use: Change the name of {0}".WrapUOHtmlColor(Color.LawnGreen), TargetUsage}
			}.Apply();
		}

		public override void OnDoubleClick(Mobile m)
		{
			if (this.CheckDoubleClick(m, true, false, 2, true))
			{
				BeginTarget(m);
			}
		}

		protected virtual void BeginTarget(Mobile m)
		{
			if (_Target != null)
			{
				if (_Target.User != null && _Target.User.Target == _Target)
				{
					_Target.Cancel(_Target.User, TargetCancelType.Overriden);
				}

				_Target = null;
			}

			m.SendMessage("Target an object to rename...");

			_Target = new GenericSelectTarget<TEntity>(
				(u, t) =>
				{
					_Target = null;
					OpenGump(u, t);
				},
				u =>
				{
					OnTargetFail(u);
					_Target = null;
				});

			m.Target = _Target;
		}

		protected virtual void OnTargetFail(Mobile m)
		{
			if (_Target.User == m && _Target.Result == TargetResult.Invalid)
			{
				m.SendMessage("That is not a valid target.");
			}
		}

		protected virtual void OpenGump(Mobile m, TEntity t)
		{
			if (_Gump != null)
			{
				_Gump.Close();
				_Gump = null;
			}

			if (Names != null && Names.Count > 0)
			{
				m.SendMessage("Select a name from the registry...");

				var opts = new MenuGumpOptions();

				foreach (var name in Names.OrderByNatural())
				{
					var n = name;

					opts.AppendEntry(
						new ListGumpEntry(
							n,
							() =>
							{
								_Gump = null;
								ApplyName(m, t, n);
							}));
				}

				var menu = new MenuGump(m, null, opts);

				menu.OnActionClose += HandleGumpClose;
				_Gump = menu.Send();
			}
			else
			{
				string name;
				int limit;

				if (t is Item)
				{
					name = (t as Item).Name;
					limit = 20;
				}
				else if (t is Mobile)
				{
					name = (t as Mobile).RawName;
					limit = 20;
				}
				else
				{
					name = String.Empty;
					limit = 0;
				}

				var dialog = new InputDialogGump(m)
				{
					Title = "Name Selection",
					Html = "Type the name you wish to use and click OK to accept.",
					InputText = name ?? String.Empty,
					Limit = limit,
					Callback = (b, n) =>
					{
						_Gump = null;
						ApplyName(m, t, n);
					}
				};

				dialog.OnActionClose += HandleGumpClose;
				_Gump = dialog.Send();
			}
		}

		private void HandleGumpClose(SuperGump g, bool all)
		{
			if (_Gump == g && all)
			{
				_Gump.OnActionClose -= HandleGumpClose;
				_Gump = null;
			}
		}

		protected virtual void ApplyName(Mobile m, TEntity t, string name)
		{
			if (m == null || t == null)
			{
				return;
			}

			if (String.IsNullOrWhiteSpace(name))
			{
				m.SendMessage(0x22, "You cannot use a blank name.");
				OpenGump(m, t);
				return;
			}

			name = Utility.FixHtml(name.Trim());

			if (!NameVerification.Validate(name, 2, 20, true, t is Item, true, 1, NameVerification.SpaceDashPeriodQuote))
			{
				m.SendMessage(0x22, "That name is unacceptable.");
				OpenGump(m, t);
				return;
			}

			var item = t as Item;

			if (item != null)
			{
				if (item.Name == name)
				{
					m.SendMessage(0x22, "The item is already named {0}.", name);
					OpenGump(m, t);
					return;
				}

				item.Name = name;

				m.SendMessage(0x55, "The item has been renamed.");
				Delete();
				return;
			}

			var mob = t as Mobile;

			if (mob != null)
			{
				if (mob.RawName == name)
				{
					m.SendMessage(0x22, "{0} are already named {1}.", mob == m ? "You" : "They", name);
					OpenGump(m, t);
					return;
				}

				mob.RawName = name;

				m.SendMessage(0x55, "{0} name has been changed.", mob == m ? "Your" : "Their");
				Delete();

				if (mob is PlayerMobile)
				{
					var pm = (PlayerMobile)mob;

					PlayerNames.Register(pm);
					PlayerNames.ValidateSharedName(pm);
				}

				return;
			}

			m.SendMessage(0x22, "Could not rename that object.");
		}

		public override void OnDelete()
		{
			base.OnDelete();

			_Gump = null;
			_Target = null;

			Names.Free(true);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);

			writer.WriteList(Names, writer.Write);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			Names = reader.ReadList(reader.ReadString);
		}
	}

	public abstract class ItemNameDeed<TItem> : BaseNameDeed<TItem>
		where TItem : Item
	{
		public override string TargetUsage => "an item";

		public ItemNameDeed()
		{ }

		public ItemNameDeed(Serial serial)
			: base(serial)
		{ }

		protected override void OpenGump(Mobile m, TItem t)
		{
			if (m == null || t == null)
			{
				return;
			}

			if (m.AccessLevel < AccessLevel.GameMaster)
			{
				if (!t.Movable && !t.IsAccessibleTo(m))
				{
					m.SendMessage("That item is not accessible.");
					return;
				}

				if (!t.IsChildOf(m.Backpack) && t.RootParent != m)
				{
					m.SendMessage("That item must be equipped or in your pack to rename it.");
					return;
				}
			}

			base.OpenGump(m, t);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}

	public abstract class MobileNameDeed<TMobile> : BaseNameDeed<TMobile>
		where TMobile : Mobile
	{
		public override string TargetUsage => "a mobile";

		public MobileNameDeed()
		{ }

		public MobileNameDeed(Serial serial)
			: base(serial)
		{ }

		protected override void OpenGump(Mobile m, TMobile t)
		{
			if (m == null || t == null)
			{
				return;
			}

			if (m.AccessLevel < t.AccessLevel)
			{
				m.SendMessage("They are not accessible.");
				return;
			}

			base.OpenGump(m, t);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}

	public class PlayerNameDeed : MobileNameDeed<PlayerMobile>
	{
		public override string TargetUsage => "your character";

		[Constructable]
		public PlayerNameDeed()
		{
			Name = "Player Rename Deed";
		}

		public PlayerNameDeed(Serial serial)
			: base(serial)
		{ }

		protected override void BeginTarget(Mobile m)
		{
			OpenGump(m, m as PlayerMobile);
		}

		protected override void OpenGump(Mobile m, PlayerMobile t)
		{
			if (m == null || t == null)
			{
				return;
			}

			if (m.AccessLevel < AccessLevel.GameMaster)
			{
				if (m != t)
				{
					m.SendMessage("You can only change your own name.");
					return;
				}
			}

			base.OpenGump(m, t);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}

	public class WeaponNameDeed : ItemNameDeed<BaseWeapon>
	{
		public override string TargetUsage => "your weapon";

		[Constructable]
		public WeaponNameDeed()
		{
			Name = "Weapon Rename Deed";
		}

		public WeaponNameDeed(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}

	public class ArmorNameDeed : ItemNameDeed<BaseArmor>
	{
		public override string TargetUsage => "your armor";

		[Constructable]
		public ArmorNameDeed()
		{
			Name = "Armor Rename Deed";
		}

		public ArmorNameDeed(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}

	public class ClothingNameDeed : ItemNameDeed<BaseClothing>
	{
		public override string TargetUsage => "your clothing";

		[Constructable]
		public ClothingNameDeed()
		{
			Name = "Clothing Rename Deed";
		}

		public ClothingNameDeed(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}

	public class JewelNameDeed : ItemNameDeed<BaseJewel>
	{
		public override string TargetUsage => "your jewelry";

		[Constructable]
		public JewelNameDeed()
		{
			Name = "Jewelry Rename Deed";
		}

		public JewelNameDeed(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}

	public class QuiverNameDeed : ItemNameDeed<BaseQuiver>
	{
		public override string TargetUsage => "your quiver";

		[Constructable]
		public QuiverNameDeed()
		{
			Name = "Quiver Rename Deed";
		}

		public QuiverNameDeed(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}

	public class LightNameDeed : ItemNameDeed<BaseEquipableLight>
	{
		public override string TargetUsage => "your light source";

		[Constructable]
		public LightNameDeed()
		{
			Name = "Light Source Rename Deed";
		}

		public LightNameDeed(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}

	public class SpellbookNameDeed : ItemNameDeed<Spellbook>
	{
		public override string TargetUsage => "your spellbook";

		[Constructable]
		public SpellbookNameDeed()
		{
			Name = "Spellbook Rename Deed";
		}

		public SpellbookNameDeed(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}

	public class ContainerNameDeed : ItemNameDeed<Container>
	{
		public override string TargetUsage => "a container";

		[Constructable]
		public ContainerNameDeed()
		{
			Name = "Container Rename Deed";
		}

		public ContainerNameDeed(Serial serial)
			: base(serial)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}
}