using System;
using System.Collections.Generic;
using Server.Commands;
using Server.ContextMenus;
using Server.Items;
using Server.Multis;

namespace Server.ACC.YS
{
    public class YardGate : BaseDoor
    {
        #region Properties
        private Mobile m_Placer;
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Placer
        {
            get { return m_Placer; }
            set { m_Placer = value; }
        }

        private int m_Price;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Price
        {
            get { return m_Price; }
            set { m_Price = value; }
        }

        private BaseHouse m_House;
        [CommandProperty(AccessLevel.GameMaster)]
        public BaseHouse House
        {
            get { return m_House; }
            set { m_House = value; }
        }
        #endregion

        #region Constructors
        public YardGate(int itemID, Mobile placer, int price, BaseHouse house, Point3D location, DoorFacing facing)
            : base(itemID, itemID + 1, GetOpenedSound(itemID), GetClosedSound(itemID), BaseDoor.GetOffset(facing))
        {
            Placer = placer;
            Price = price;

            Movable = false;
            MoveToWorld(location, placer.Map);

            if (house == null)
            {
                FindHouseOfPlacer();
            }
            else
            {
                House = house;
            }

            SetName();
        }

        public YardGate(Serial serial)
            : base(serial)
        {
        }
        #endregion

        #region Overrides
        public override void Use(Mobile from)
        {
            if (Locked && from == Placer)
            {
                Locked = false;
                from.SendMessage("You quickly unlock your gate, enter, and lock it behind you");
                base.Use(from);
                Locked = true;
            }
            else if (Locked && from != Placer)
            {
                from.SendMessage("You are not wanted here.  Please go away!");
            }
            else
            {
                base.Use(from);
            }
        }

        public override void GetContextMenuEntries(Mobile from, System.Collections.Generic.List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            if (m_Placer == null || from == m_Placer || from.AccessLevel >= AccessLevel.GameMaster)
            {
                list.Add(new YardSecurityEntry(from, this));
                list.Add(new RefundEntry(from, this, m_Price));
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            if (House == null || House.Deleted)
            {
                writer.Write(false);
                YardSystem.AddOrphanedItem(this);
            }
            else
            {
                writer.Write(true);
                writer.Write(House);
            }

            writer.WriteMobile(Placer);
            writer.Write(Price);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (reader.ReadBool())
            {
                House = reader.ReadItem() as BaseHouse;
            }

            Placer = reader.ReadMobile();
            Price = reader.ReadInt();

            if (House == null)
            {
                FindHouseOfPlacer();
                if (House == null)
                {
                    Refund();
                }
            }
        }
        #endregion

        #region Methods
        public void SetName()
        {
            switch (ItemID)
            {
                case 0x824:
                    Name = Placer.Name + "'s Gate"; break;
                case 0x84C:
                    Name = Placer.Name + "'s Gate"; break;
                case 0x839:
                    Name = Placer.Name + "'s Gate"; break;
                case 0x866:
                    Name = Placer.Name + "'s Gate"; break;
                case 0x675:
                    Name = Placer.Name + "'s Door"; break;
                case 0x6C5:
                    Name = Placer.Name + "'s Door"; break;
                case 0x685:
                    Name = Placer.Name + "'s Door"; break;
                case 0x1FED:
                    Name = Placer.Name + "'s Door"; break;
                case 0x695:
                    Name = Placer.Name + "'s Door"; break;
                case 0x6A5:
                    Name = Placer.Name + "'s Door"; break;
                case 0x6B5:
                    Name = Placer.Name + "'s Door"; break;
                case 0x6D5:
                    Name = Placer.Name + "'s Door"; break;
                case 0x6EF:
                    Name = Placer.Name + "'s Door"; break;
                default:
                    Name = Placer.Name + "'s Gate"; break;
            }
        }

        public void Refund()
        {
            Gold toGive = new Gold(Price);
            if (Placer.BankBox.TryDropItem(Placer, toGive, false))
            {
                Delete();
                Placer.SendLocalizedMessage(1060397, toGive.Amount.ToString()); // ~1_AMOUNT~ gold has been deposited into your bank box.
            }
            else
            {
                toGive.Delete();
                Placer.SendMessage("Your bankbox is full!");
            }
        }

        public void FindHouseOfPlacer()
        {
            if (Placer == null || House != null)
            {
                return;
            }

            IPooledEnumerable eable = Map.GetItemsInRange(Location, 20);
            foreach (Item item in eable)
            {
                if (item is BaseHouse)
                {
                    BaseHouse house = (BaseHouse)item;
                    if (house.Owner == Placer)
                    {
                        House = house;
                        return;
                    }
                }
            }
        }
        #endregion

        #region Static
        public static int GetClosedSound(int itemID)
        {
            if ((itemID >= 2084 && itemID <= 2098) ||
                (itemID >= 2124 && itemID <= 2138))
            {
                return 243;
            }
            else if ((itemID >= 2105 && itemID <= 2119) ||
                     (itemID >= 2150 && itemID <= 2162))
            {
                return 242;
            }
            else
            {
                return 243;
            }
        }

        public static int GetOpenedSound(int itemID)
        {
            if ((itemID >= 2084 && itemID <= 2098) ||
                (itemID >= 2124 && itemID <= 2138))
            {
                return 236;
            }
            else if ((itemID >= 2105 && itemID <= 2119) ||
                     (itemID >= 2150 && itemID <= 2162))
            {
                return 235;
            }
            else
            {
                return 236;
            }
        }
        #endregion
    }

    #region Old Gates
    public class YardIronGate : IronGate
    {
        private Mobile m_Placer;
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Placer
        {
            get { return m_Placer; }
            set { m_Placer = value; }
        }

        private int m_Price;
        public int Price
        {
            get { return m_Price; }
            set { m_Price = value; }
        }

        [Constructable]
        public YardIronGate(Mobile from, int price, DoorFacing facing, Point3D loc)
            : base(facing)
        {
            Price = price;
            Placer = from;
            Movable = false;
            MoveToWorld(loc, from.Map);
            Name = from.Name + "'s Gate";
        }

        public override void Use(Mobile from)
        {
            if (((BaseDoor)this).Locked && from == Placer)
            {
                ((BaseDoor)this).Locked = false;
                from.SendMessage("You quickly unlock your gate, enter, and lock it behind you");
                base.Use(from);
                ((BaseDoor)this).Locked = true;
            }
            else if (((BaseDoor)this).Locked && from != Placer)
            {
                from.SendMessage("You are not wanted here.  Please go away!");
            }
            else
            {
                base.Use(from);
            }
        }

        public YardIronGate(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.WriteMobile(Placer);
            writer.Write(Price);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            Placer = reader.ReadMobile();
            Price = reader.ReadInt();
            Console.WriteLine();
            Console.Write("Updating YardIronGate...");
            YardGate newGate = new YardGate(2084, Placer, Price, null, Location, (DoorFacing)((ClosedID - 2084) / 2));
            newGate.Map = Map;
            if (newGate != null)
            {
                Console.WriteLine(String.Format("New gate = {0}, ItemID = {1}, Location = {2}", newGate.Serial.ToString(), newGate.ItemID, newGate.Location));
            }
            else
            {
                Console.WriteLine("Null");
            }
            Delete();
        }
    }

    public class YardShortIronGate : IronGateShort
    {
        private Mobile m_Placer;
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Placer
        {
            get { return m_Placer; }
            set { m_Placer = value; }
        }

        private int m_Price;
        public int Price
        {
            get { return m_Price; }
            set { m_Price = value; }
        }

        [Constructable]
        public YardShortIronGate(Mobile from, int price, DoorFacing facing, Point3D loc)
            : base(facing)
        {
            Price = price;
            Placer = from;
            Movable = false;
            MoveToWorld(loc, from.Map);
            Name = from.Name + "'s Gate";
        }

        public override void Use(Mobile from)
        {
            if (((BaseDoor)this).Locked && from == Placer)
            {
                ((BaseDoor)this).Locked = false;
                from.SendMessage("You quickly unlock your gate, enter, and lock it behind you");
                base.Use(from);
                ((BaseDoor)this).Locked = true;
            }
            else if (((BaseDoor)this).Locked && from != Placer)
            {
                from.SendMessage("You are not wanted here.  Please go away!");
            }
            else
            {
                base.Use(from);
            }
        }

        public YardShortIronGate(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.WriteMobile(Placer);
            writer.Write(Price);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            Placer = reader.ReadMobile();
            Price = reader.ReadInt();
            Console.WriteLine();
            Console.Write("Updating YardShortIronGate...");
            YardGate newGate = new YardGate(2124, Placer, Price, null, Location, (DoorFacing)((ClosedID - 2124) / 2));
            newGate.Map = Map;
            if (newGate != null)
            {
                Console.WriteLine(String.Format("New gate = {0}, ItemID = {1}, Location = {2}", newGate.Serial.ToString(), newGate.ItemID, newGate.Location));
            }
            else
            {
                Console.WriteLine("Null");
            }
            Delete();
        }
    }

    public class YardLightWoodGate : LightWoodGate
    {
        private Mobile m_Placer;
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Placer
        {
            get { return m_Placer; }
            set { m_Placer = value; }
        }

        private int m_Price;
        public int Price
        {
            get { return m_Price; }
            set { m_Price = value; }
        }

        [Constructable]
        public YardLightWoodGate(Mobile from, int price, DoorFacing facing, Point3D loc)
            : base(facing)
        {
            Price = price;
            Placer = from;
            Movable = false;
            MoveToWorld(loc, from.Map);
            Name = from.Name + "'s Gate";
        }

        public override void Use(Mobile from)
        {
            if (((BaseDoor)this).Locked && from == Placer)
            {
                ((BaseDoor)this).Locked = false;
                from.SendMessage("You quickly unlock your gate, enter, and lock it behind you");
                base.Use(from);
                ((BaseDoor)this).Locked = true;
            }
            else if (((BaseDoor)this).Locked && from != Placer)
            {
                from.SendMessage("You are not wanted here.  Please go away!");
            }
            else
            {
                base.Use(from);
            }
        }

        public YardLightWoodGate(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.WriteMobile(Placer);
            writer.Write(Price);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            Placer = reader.ReadMobile();
            Price = reader.ReadInt();
            Console.WriteLine();
            Console.Write("Updating YardLightWoodGate...");
            YardGate newGate = new YardGate(2105, Placer, Price, null, Location, (DoorFacing)((ClosedID - 2105) / 2));
            newGate.Map = Map;
            if (newGate != null)
            {
                Console.WriteLine(String.Format("New gate = {0}, ItemID = {1}, Location = {2}", newGate.Serial.ToString(), newGate.ItemID, newGate.Location));
            }
            else
            {
                Console.WriteLine("Null");
            }
            Delete();
        }
    }

    public class YardDarkWoodGate : DarkWoodGate
    {
        private Mobile m_Placer;
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Placer
        {
            get { return m_Placer; }
            set { m_Placer = value; }
        }

        private int m_Price;
        public int Price
        {
            get { return m_Price; }
            set { m_Price = value; }
        }

        [Constructable]
        public YardDarkWoodGate(Mobile from, int price, DoorFacing facing, Point3D loc)
            : base(facing)
        {
            Price = price;
            Placer = from;
            Movable = false;
            MoveToWorld(loc, from.Map);
            Name = from.Name + "'s Gate";
        }

        public override void Use(Mobile from)
        {
            if (((BaseDoor)this).Locked && from == Placer)
            {
                ((BaseDoor)this).Locked = false;
                from.SendMessage("You quickly unlock your gate, enter, and lock it behind you");
                base.Use(from);
                ((BaseDoor)this).Locked = true;
            }
            else if (((BaseDoor)this).Locked && from != Placer)
            {
                from.SendMessage("You are not wanted here.  Please go away!");
            }
            else
            {
                base.Use(from);
            }
        }

        public YardDarkWoodGate(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write(Placer);
            writer.Write(Price);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            Placer = reader.ReadMobile();
            Price = reader.ReadInt();
            Console.WriteLine();
            Console.Write("Updating YardDarkWoodGate...");
            YardGate newGate = new YardGate(2150, Placer, Price, null, Location, (DoorFacing)((ClosedID - 2150) / 2));
            newGate.Map = Map;
            if (newGate != null)
            {
                Console.WriteLine(String.Format("New gate = {0}, ItemID = {1}, Location = {2}", newGate.Serial.ToString(), newGate.ItemID, newGate.Location));
            }
            else
            {
                Console.WriteLine("Null");
            }
            Delete();
        }
    }
    #endregion
}