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
using Server.Mobiles;
using Server.Targeting;

using VitaNex.Network;
using VitaNex.SuperGumps.UI;
using VitaNex.Targets;
#endregion

namespace VitaNex.Items
{
	public static class HueDeeds
	{
		public static event Action<IHueDeed> OnCreated;

		public static void Register<T>(Action<T> onCreated)
			where T : IHueDeed
		{
			OnCreated += o =>
			{
				if (o is T d)
				{
					onCreated(d);
				}
			};
		}

		public static void InvokeCreated(IHueDeed d)
		{
			if (OnCreated != null)
			{
				OnCreated(d);
			}
		}
	}

	public interface IHueDeed
	{
		List<int> Hues { get; }

		void AddHue(int hue);
		void AddHues(params int[] hues);
		void AddHueRange(int hue, int count);
	}

	public abstract class BaseHueDeed<TEntity> : Item, IHueDeed
		where TEntity : IEntity
	{
		private HueSelector _Gump;
		private GenericSelectTarget<TEntity> _Target;

		public List<int> Hues { get; private set; }

		public virtual string TargetUsage => "an object";

		public BaseHueDeed()
			: base(0x14F0)
		{
			Hues = new List<int>();

			Name = "Color Change Deed";
			Weight = 1.0;
			LootType = LootType.Blessed;

			Timer.DelayCall(HueDeeds.InvokeCreated, this);
		}

		public BaseHueDeed(Serial serial)
			: base(serial)
		{ }

		public void AddHue(int hue)
		{
			Hues.Update(hue);
		}

		public void AddHues(params int[] hues)
		{
			if (hues == null || hues.Length == 0)
			{
				return;
			}

			foreach (var h in hues)
			{
				AddHue(h);
			}
		}

		public void AddHueRange(int hue, int count)
		{
			count = Math.Max(0, count);

			while (--count >= 0)
			{
				AddHue(hue++);
			}
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			ExtendedOPL.AddTo(list, "Use: Change the color of {0}".WrapUOHtmlColor(Color.LawnGreen), TargetUsage);
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

			m.SendMessage("Target an object to recolor...");

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

			m.SendMessage("Select a color from the chart...");

			_Gump = new HueSelector(m)
			{
				PreviewIcon = GetPreviewIcon(t),
				Hues = Hues.ToArray(),
				AcceptCallback = hue =>
				{
					_Gump = null;
					ApplyHue(m, t, hue);
				},
				CancelCallback = hue => _Gump = null
			};

			_Gump.Send();
		}

		public virtual int GetPreviewIcon(TEntity t)
		{
			return HueSelector.DefaultIcon;
		}

		protected virtual void ApplyHue(Mobile m, TEntity t, int hue)
		{
			if (m == null || t == null)
			{
				return;
			}

			if (hue < 0 || hue >= 3000)
			{
				m.SendMessage(0x22, "You cannot use an invalid hue.");
				OpenGump(m, t);
				return;
			}

			var item = t as Item;

			if (item != null)
			{
				if (item.Hue == hue)
				{
					m.SendMessage(0x22, "The item is already that color.");
					OpenGump(m, t);
					return;
				}

				item.Hue = hue;
				m.SendMessage(0x55, "The item has been recolored.");
				Delete();
				return;
			}

			var mob = t as Mobile;

			if (mob != null)
			{
				if (mob.Hue == hue)
				{
					m.SendMessage(0x22, "{0} skin is already that color.", mob == m ? "Your" : "Their");
					OpenGump(m, t);
					return;
				}

				mob.Hue = hue;
				m.SendMessage(0x55, "{0} skin has been recolored.", mob == m ? "Your" : "Their");
				Delete();
				return;
			}

			m.SendMessage(0x22, "Could not recolor that object.");
		}

		public override void OnDelete()
		{
			base.OnDelete();

			_Gump = null;
			_Target = null;

			Hues.Free(true);
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);

			writer.WriteList(Hues, writer.Write);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();

			Hues = reader.ReadList(reader.ReadInt);
		}
	}

	public abstract class ItemHueDeed<TItem> : BaseHueDeed<TItem>
		where TItem : Item
	{
		public override string TargetUsage => "an item";

		public ItemHueDeed()
		{ }

		public ItemHueDeed(Serial serial)
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
					m.SendMessage("That item must be equipped or in your pack to recolor it.");
					return;
				}
			}

			base.OpenGump(m, t);
		}

		public override int GetPreviewIcon(TItem t)
		{
			return t == null ? HueSelector.DefaultIcon : t.ItemID;
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

	public abstract class MobileHueDeed<TMobile> : BaseHueDeed<TMobile>
		where TMobile : Mobile
	{
		public override string TargetUsage => "a mobile";

		public MobileHueDeed()
		{ }

		public MobileHueDeed(Serial serial)
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

		public override int GetPreviewIcon(TMobile t)
		{
			return t == null ? HueSelector.DefaultIcon : ShrinkTable.Lookup(t);
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

	public class SkinHueDeed : MobileHueDeed<PlayerMobile>
	{
		public override string TargetUsage => "your character";

		[Constructable]
		public SkinHueDeed()
		{
			Name = "Skin Recolor Deed";
			AddHueRange(1002, 57);
		}

		public SkinHueDeed(Serial serial)
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
					m.SendMessage("You can only recolor your own skin.");
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

	public class PetHueDeed : MobileHueDeed<BaseCreature>
	{
		public override string TargetUsage => "your pet";

		[Constructable]
		public PetHueDeed()
		{
			Name = "Pet Recolor Deed";
			AddHueRange(2301, 18);
		}

		public PetHueDeed(Serial serial)
			: base(serial)
		{ }

		protected override void OpenGump(Mobile m, BaseCreature t)
		{
			if (m == null || t == null)
			{
				return;
			}

			if (m.AccessLevel < AccessLevel.GameMaster)
			{
				if (t.GetMaster() != m)
				{
					m.SendMessage("You can only recolor the skin of pets that you own.");
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

	public class WeaponHueDeed : ItemHueDeed<BaseWeapon>
	{
		public override string TargetUsage => "a weapon";

		[Constructable]
		public WeaponHueDeed()
		{
			Name = "Weapon Recolor Deed";
			AddHueRange(2401, 30);
		}

		public WeaponHueDeed(Serial serial)
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

	public class ArmorHueDeed : ItemHueDeed<BaseArmor>
	{
		public override string TargetUsage => "armor";

		[Constructable]
		public ArmorHueDeed()
		{
			Name = "Armor Recolor Deed";
			AddHueRange(2401, 30);
		}

		public ArmorHueDeed(Serial serial)
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

	public class ClothingHueDeed : ItemHueDeed<BaseClothing>
	{
		public override string TargetUsage => "clothing";

		[Constructable]
		public ClothingHueDeed()
		{
			Name = "Clothing Recolor Deed";
			AddHueRange(2, 1000);
		}

		public ClothingHueDeed(Serial serial)
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

	public class JewelHueDeed : ItemHueDeed<BaseJewel>
	{
		public override string TargetUsage => "your jewelry";

		[Constructable]
		public JewelHueDeed()
		{
			Name = "Jewelry Recolor Deed";
			AddHueRange(2401, 30);
		}

		public JewelHueDeed(Serial serial)
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

	public class QuiverHueDeed : ItemHueDeed<BaseQuiver>
	{
		public override string TargetUsage => "your quiver";

		[Constructable]
		public QuiverHueDeed()
		{
			Name = "Quiver Recolor Deed";
			AddHueRange(2, 1000);
		}

		public QuiverHueDeed(Serial serial)
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

	public class LightHueDeed : ItemHueDeed<BaseEquipableLight>
	{
		public override string TargetUsage => "your light source";

		[Constructable]
		public LightHueDeed()
		{
			Name = "Light Source Recolor Deed";
			AddHueRange(2401, 30);
		}

		public LightHueDeed(Serial serial)
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

	public class SpellbookHueDeed : ItemHueDeed<Spellbook>
	{
		public override string TargetUsage => "your spellbook";

		[Constructable]
		public SpellbookHueDeed()
		{
			Name = "Spellbook Recolor Deed";
			AddHueRange(2, 1000);
		}

		public SpellbookHueDeed(Serial serial)
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

	public class ContainerHueDeed : ItemHueDeed<Container>
	{
		public override string TargetUsage => "a container";

		[Constructable]
		public ContainerHueDeed()
		{
			Name = "Container Recolor Deed";
			AddHueRange(2, 1000);
		}

		public ContainerHueDeed(Serial serial)
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
