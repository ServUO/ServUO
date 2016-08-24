using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
    public abstract class BaseContainer : Container, IEngravable
    {
        public BaseContainer(int itemID)
            : base(itemID)
        {
        }

        public BaseContainer(Serial serial)
            : base(serial)
        {
        }

        public override int DefaultMaxWeight
        {
            get
            {
                if (this.IsSecure)
                    return 0;

                return base.DefaultMaxWeight;
            }
        }

		private string m_EngravedText = string.Empty;

		[CommandProperty(AccessLevel.GameMaster)]
		public string EngravedText
		{
			get { return this.m_EngravedText; }
			set
			{
				if (value != null)
					this.m_EngravedText = value;
				else
					this.m_EngravedText = string.Empty;
				this.InvalidateProperties();
			}
		}

		public override bool IsAccessibleTo(Mobile m)
        {
            if (!BaseHouse.CheckAccessible(m, this))
                return false;

            return base.IsAccessibleTo(m);
        }

        public override bool CheckHold(Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight)
        {
            if (this.IsSecure && !BaseHouse.CheckHold(m, this, item, message, checkItems, plusItems, plusWeight))
                return false;

            return base.CheckHold(m, item, message, checkItems, plusItems, plusWeight);
        }

        public override bool CheckItemUse(Mobile from, Item item)
        {
            if (this.IsDecoContainer && item is BaseBook)
                return true;

            return base.CheckItemUse(from, item);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public override bool TryDropItem(Mobile from, Item dropped, bool sendFullMessage)
        {
            if (!this.CheckHold(from, dropped, sendFullMessage, true))
                return false;

            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && house.IsLockedDown(this))
            {
                if (dropped is VendorRentalContract || (dropped is Container && ((Container)dropped).FindItemByType(typeof(VendorRentalContract)) != null))
                {
                    from.SendLocalizedMessage(1062492); // You cannot place a rental contract in a locked down container.
                    return false;
                }

                if (!house.LockDown(from, dropped, false))
                    return false;
            }

            List<Item> list = this.Items;

            for (int i = 0; i < list.Count; ++i)
            {
                Item item = list[i];

                if (!(item is Container) && item.StackWith(from, dropped, false))
                    return true;
            }

            this.DropItem(dropped);

            ItemFlags.SetTaken(dropped, true);

            if (dropped.HonestyItem && dropped.HonestyPickup == DateTime.MinValue)
            {
                dropped.HonestyPickup = DateTime.UtcNow;
                dropped.StartHonestyTimer();

                from.SendLocalizedMessage(1151536); // You have three hours to turn this item in for Honesty credit, otherwise it will cease to be a quest item.
            }

            return true;
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            if (!this.CheckHold(from, item, true, true))
                return false;

            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && house.IsLockedDown(this))
            {
                if (item is VendorRentalContract || (item is Container && ((Container)item).FindItemByType(typeof(VendorRentalContract)) != null))
                {
                    from.SendLocalizedMessage(1062492); // You cannot place a rental contract in a locked down container.
                    return false;
                }

                if (!house.LockDown(from, item, false))
                    return false;
            }

            item.Location = new Point3D(p.X, p.Y, 0);
            this.AddItem(item);

            from.SendSound(this.GetDroppedSound(item), this.GetWorldLocation());

            ItemFlags.SetTaken(item, true);

            if (item.HonestyItem && item.HonestyPickup == DateTime.MinValue)
            {
                item.HonestyPickup = DateTime.UtcNow;
                item.StartHonestyTimer();

                from.SendLocalizedMessage(1151536); // You have three hours to turn this item in for Honesty credit, otherwise it will cease to be a quest item.
            }
            return true;
        }

        public override void UpdateTotal(Item sender, TotalType type, int delta)
        {
            base.UpdateTotal(sender, type, delta);

            if (type == TotalType.Weight && this.RootParent is Mobile)
                ((Mobile)this.RootParent).InvalidateProperties();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.IsStaff() || from.InRange(this.GetWorldLocation(), 2) || this.RootParent is PlayerVendor)
                this.Open(from);
            else
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
        }

		public override void AddNameProperty(ObjectPropertyList list)
		{
			base.AddNameProperty(list);

			if(!String.IsNullOrEmpty(this.EngravedText))
			{
				list.Add(1072305, this.EngravedText); // Engraved: ~1_INSCRIPTION~
			}
		}

		public virtual void Open(Mobile from)
        {
            this.DisplayTo(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

			writer.Write(1000); // Version
			writer.Write(m_EngravedText);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

			int version = reader.PeekInt();
			switch(version)
			{
				case 1000:
					reader.ReadInt();
					m_EngravedText = reader.ReadString();
					break;
			}
        }
    }

    public class CreatureBackpack : Backpack	//Used on BaseCreature
    {
        [Constructable]
        public CreatureBackpack(string name)
        {
            this.Name = name;
            this.Layer = Layer.Backpack;
            this.Hue = 5;
            this.Weight = 3.0;
        }

        public CreatureBackpack(Serial serial)
            : base(serial)
        {
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (this.Name != null)
                list.Add(1075257, this.Name); // Contents of ~1_PETNAME~'s pack.
            else
                base.AddNameProperty(list);
        }

        public override void OnItemRemoved(Item item)
        {
            if (this.Items.Count == 0)
                this.Delete();

            base.OnItemRemoved(item);
        }

        public override bool OnDragLift(Mobile from)
        {
            if (from.IsPlayer())
                return true;

            from.SendLocalizedMessage(500169); // You cannot pick that up.
            return false;
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            return false;
        }

        public override bool TryDropItem(Mobile from, Item dropped, bool sendFullMessage)
        {
            return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
                this.Weight = 13.0;
        }
    }

    public class StrongBackpack : Backpack	//Used on Pack animals
    {
        [Constructable]
        public StrongBackpack()
        {
            this.Layer = Layer.Backpack;
            this.Weight = 13.0;
        }

        public StrongBackpack(Serial serial)
            : base(serial)
        {
        }

        public override int DefaultMaxWeight
        {
            get
            {
                return 1600;
            }
        }
        public override bool CheckHold(Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight)
        {
            return base.CheckHold(m, item, false, checkItems, plusItems, plusWeight);
        }

        public override bool CheckContentDisplay(Mobile from)
        {
            object root = this.RootParent;

            if (root is BaseCreature && ((BaseCreature)root).Controlled && ((BaseCreature)root).ControlMaster == from)
                return true;

            return base.CheckContentDisplay(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
                this.Weight = 13.0;
        }
    }

    public class Backpack : BaseContainer, IDyable
    {
        [Constructable]
        public Backpack()
            : base(0xE75)
        {
            this.Layer = Layer.Backpack;
            this.Weight = 3.0;
        }

        public Backpack(Serial serial)
            : base(serial)
        {
        }

        public override int DefaultMaxWeight
        {
            get
            {
                if (Core.ML)
                {
                    Mobile m = this.ParentEntity as Mobile;
                    if (m != null && m.Player && m.Backpack == this)
                    {
                        return 550;
                    }
                    else
                    {
                        return base.DefaultMaxWeight;
                    }
                }
                else
                {
                    return base.DefaultMaxWeight;
                }
            }
        }
        public bool Dye(Mobile from, DyeTub sender)
        {
            if (this.Deleted)
                return false;

            this.Hue = sender.DyedHue;

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0 && this.ItemID == 0x9B2)
                this.ItemID = 0xE75;
        }
    }

    public class Pouch : TrapableContainer
    {
        [Constructable]
        public Pouch()
            : base(0xE79)
        {
            this.Weight = 1.0;
        }

        public Pouch(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public abstract class BaseBagBall : BaseContainer, IDyable
    {
        public BaseBagBall(int itemID)
            : base(itemID)
        {
            this.Weight = 1.0;
        }

        public BaseBagBall(Serial serial)
            : base(serial)
        {
        }

        public bool Dye(Mobile from, DyeTub sender)
        {
            if (this.Deleted)
                return false;

            this.Hue = sender.DyedHue;

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class SmallBagBall : BaseBagBall
    {
        [Constructable]
        public SmallBagBall()
            : base(0x2256)
        {
        }

        public SmallBagBall(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class LargeBagBall : BaseBagBall
    {
        [Constructable]
        public LargeBagBall()
            : base(0x2257)
        {
        }

        public LargeBagBall(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class Bag : BaseContainer, IDyable
    {
        [Constructable]
        public Bag()
            : base(0xE76)
        {
            this.Weight = 2.0;
        }

        public Bag(Serial serial)
            : base(serial)
        {
        }

        public bool Dye(Mobile from, DyeTub sender)
        {
            if (this.Deleted)
                return false;

            this.Hue = sender.DyedHue;

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class Barrel : BaseContainer
    {
        [Constructable]
        public Barrel()
            : base(0xE77)
        {
            this.Weight = 25.0;
        }

        public Barrel(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Weight == 0.0)
                this.Weight = 25.0;
        }
    }

    public class Keg : BaseContainer
    {
        [Constructable]
        public Keg()
            : base(0xE7F)
        {
            this.Weight = 15.0;
        }

        public Keg(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class PicnicBasket : BaseContainer
    {
        [Constructable]
        public PicnicBasket()
            : base(0xE7A)
        {
            this.Weight = 2.0; // Stratics doesn't know weight
        }

        public PicnicBasket(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class Basket : BaseContainer
    {
        [Constructable]
        public Basket()
            : base(0x990)
        {
            this.Weight = 1.0; // Stratics doesn't know weight
        }

        public Basket(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [Furniture]
    [Flipable(0x9AA, 0xE7D)]
    public class WoodenBox : LockableContainer
    {
        [Constructable]
        public WoodenBox()
            : base(0x9AA)
        {
            this.Weight = 4.0;
        }

        public WoodenBox(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [Furniture]
    [Flipable(0x9A9, 0xE7E)]
    public class SmallCrate : LockableContainer
    {
        [Constructable]
        public SmallCrate()
            : base(0x9A9)
        {
            this.Weight = 2.0;
        }

        public SmallCrate(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Weight == 4.0)
                this.Weight = 2.0;
        }
    }

    [Furniture]
    [Flipable(0xE3F, 0xE3E)]
    public class MediumCrate : LockableContainer
    {
        [Constructable]
        public MediumCrate()
            : base(0xE3F)
        {
            this.Weight = 2.0;
        }

        public MediumCrate(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Weight == 6.0)
                this.Weight = 2.0;
        }
    }

    [Furniture]
    [Flipable(0xE3D, 0xE3C)]
    public class LargeCrate : LockableContainer
    {
        [Constructable]
        public LargeCrate()
            : base(0xE3D)
        {
            this.Weight = 1.0;
        }

        public LargeCrate(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Weight == 8.0)
                this.Weight = 1.0;
        }
    }

    [DynamicFliping]
    [Flipable(0x9A8, 0xE80)]
    public class MetalBox : LockableContainer
    {
        [Constructable]
        public MetalBox()
            : base(0x9A8)
        {
        }

        public MetalBox(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0 && this.Weight == 3)
                this.Weight = -1;
        }
    }

    [DynamicFliping]
    [Flipable(0x9AB, 0xE7C)]
    public class MetalChest : LockableContainer
    {
        [Constructable]
        public MetalChest()
            : base(0x9AB)
        {
        }

        public MetalChest(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0 && this.Weight == 25)
                this.Weight = -1;
        }
    }

    [DynamicFliping]
    [Flipable(0xE41, 0xE40)]
    public class MetalGoldenChest : LockableContainer
    {
        [Constructable]
        public MetalGoldenChest()
            : base(0xE41)
        {
        }

        public MetalGoldenChest(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0 && this.Weight == 25)
                this.Weight = -1;
        }
    }

    [Furniture]
    [Flipable(0xe43, 0xe42)]
    public class WoodenChest : LockableContainer
    {
        [Constructable]
        public WoodenChest()
            : base(0xe43)
        {
            this.Weight = 2.0;
        }

        public WoodenChest(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Weight == 15.0)
                this.Weight = 2.0;
        }
    }

    [Furniture]
    [Flipable(0x280B, 0x280C)]
    public class PlainWoodenChest : LockableContainer
    {
        [Constructable]
        public PlainWoodenChest()
            : base(0x280B)
        {
        }

        public PlainWoodenChest(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0 && this.Weight == 15)
                this.Weight = -1;
        }
    }

    [Furniture]
    [Flipable(0x280D, 0x280E)]
    public class OrnateWoodenChest : LockableContainer
    {
        [Constructable]
        public OrnateWoodenChest()
            : base(0x280D)
        {
        }

        public OrnateWoodenChest(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0 && this.Weight == 15)
                this.Weight = -1;
        }
    }

    [Furniture]
    [Flipable(0x280F, 0x2810)]
    public class GildedWoodenChest : LockableContainer
    {
        [Constructable]
        public GildedWoodenChest()
            : base(0x280F)
        {
        }

        public GildedWoodenChest(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0 && this.Weight == 15)
                this.Weight = -1;
        }
    }

    [Furniture]
    [Flipable(0x2811, 0x2812)]
    public class WoodenFootLocker : LockableContainer
    {
        [Constructable]
        public WoodenFootLocker()
            : base(0x2811)
        {
            this.GumpID = 0x10B;
        }

        public WoodenFootLocker(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0 && this.Weight == 15)
                this.Weight = -1;
			
            if (version < 2)
                this.GumpID = 0x10B;
        }
    }

    [Furniture]
    [Flipable(0x2813, 0x2814)]
    public class FinishedWoodenChest : LockableContainer
    {
        [Constructable]
        public FinishedWoodenChest()
            : base(0x2813)
        {
        }

        public FinishedWoodenChest(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0 && this.Weight == 15)
                this.Weight = -1;
        }
    }
}