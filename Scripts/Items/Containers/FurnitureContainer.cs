using System;
using Server.Engines.Craft;
using System.Collections.Generic;

namespace Server.Items
{
    public class FurnitureContainer : BaseContainer, IResource
    {
        #region Old Item Serialization Vars
        /* DO NOT USE! Only used in serialization of old furniture that originally derived from BaseContainer */
        private bool m_InheritsItem;

        protected bool InheritsItem
        {
            get
            {
                return this.m_InheritsItem;
            }
        }
        #endregion

        private Mobile m_Crafter;
        private CraftResource m_Resource;
        private ItemQuality m_Quality;
        private bool m_PlayerConstructed;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Crafter
        {
            get { return m_Crafter; }
            set
            {
                m_Crafter = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ItemQuality Quality
        {
            get { return m_Quality; }
            set { m_Quality = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get { return m_Resource; }
            set
            {
                m_Resource = value;
                Hue = CraftResources.GetHue(m_Resource);
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool PlayerConstructed
        {
            get { return m_PlayerConstructed; }
            set
            {
                m_PlayerConstructed = value;
                InvalidateProperties();
            }
        }

        public FurnitureContainer(int id) : base(id)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Crafter != null)
            {
                list.Add(1050043, m_Crafter.Name); // crafted by ~1_NAME~
            }

            if (Quality == ItemQuality.Exceptional)
            {
                list.Add(1060636); // Exceptional
            }

            if (m_Resource > CraftResource.Iron)
            {
                list.Add(1114057, "#{0}", CraftResources.GetLocalizationNumber(m_Resource)); // ~1_val~
            }
        }

        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, ITool tool, CraftItem craftItem, int resHue)
        {
            PlayerConstructed = true;

            Quality = (ItemQuality)quality;

            if (makersMark)
            {
                Crafter = from;
            }

            if (!craftItem.ForceNonExceptional)
            {
                if (typeRes == null)
                {
                    typeRes = craftItem.Resources.GetAt(0).ItemType;
                }

                Resource = CraftResources.GetFromType(typeRes);
            }

            return quality;
        }

        public FurnitureContainer(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)2); // version

            writer.Write(m_PlayerConstructed);
            writer.Write((int)m_Resource);
            writer.Write((int)m_Quality);
            writer.Write(m_Crafter);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 2:
                case 1:
                    if (version == 1 && this is EmptyBookcase)
                    {
                        m_InheritsItem = true;
                        break;
                    }

                    m_PlayerConstructed = reader.ReadBool();
                    m_Resource = (CraftResource)reader.ReadInt();
                    m_Quality = (ItemQuality)reader.ReadInt();
                    m_Crafter = reader.ReadMobile();
                    break;
                case 0:
                    m_InheritsItem = true;
                    break;
            }
        }
    }

    [Furniture]
    [Flipable(0x2815, 0x2816)]
    public class TallCabinet : FurnitureContainer
    {
        [Constructable]
        public TallCabinet()
            : base(0x2815)
        {
            this.Weight = 1.0;
        }

        public TallCabinet(Serial serial)
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
            int version = (this.InheritsItem ? 0 : reader.ReadInt()); // Required for FurnitureContainer insertion
        }
    }

    [Furniture]
    [Flipable(0x2817, 0x2818)]
    public class ShortCabinet : FurnitureContainer
    {
        [Constructable]
        public ShortCabinet()
            : base(0x2817)
        {
            this.Weight = 1.0;
        }

        public ShortCabinet(Serial serial)
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
            int version = (this.InheritsItem ? 0 : reader.ReadInt()); // Required for FurnitureContainer insertion
        }
    }

    [Furniture]
    [Flipable(0x2857, 0x2858)]
    public class RedArmoire : FurnitureContainer
    {
        [Constructable]
        public RedArmoire()
            : base(0x2857)
        {
            this.Weight = 1.0;
        }

        public RedArmoire(Serial serial)
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
            int version = (this.InheritsItem ? 0 : reader.ReadInt()); // Required for FurnitureContainer insertion
        }
    }

    [Furniture]
    [Flipable(0x285D, 0x285E)]
    public class CherryArmoire : FurnitureContainer
    {
        [Constructable]
        public CherryArmoire()
            : base(0x285D)
        {
            this.Weight = 1.0;
        }

        public CherryArmoire(Serial serial)
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
            int version = (this.InheritsItem ? 0 : reader.ReadInt()); // Required for FurnitureContainer insertion
        }
    }

    [Furniture]
    [Flipable(0x285B, 0x285C)]
    public class MapleArmoire : FurnitureContainer
    {
        [Constructable]
        public MapleArmoire()
            : base(0x285B)
        {
            this.Weight = 1.0;
        }

        public MapleArmoire(Serial serial)
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
            int version = (this.InheritsItem ? 0 : reader.ReadInt()); // Required for FurnitureContainer insertion
        }
    }

    [Furniture]
    [Flipable(0x2859, 0x285A)]
    public class ElegantArmoire : FurnitureContainer
    {
        [Constructable]
        public ElegantArmoire()
            : base(0x2859)
        {
            this.Weight = 1.0;
        }

        public ElegantArmoire(Serial serial)
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
            int version = (this.InheritsItem ? 0 : reader.ReadInt()); // Required for FurnitureContainer insertion
        }
    }

    [Furniture]
    [Flipable(0xa97, 0xa99, 0xa98, 0xa9a, 0xa9b, 0xa9c)]
    public class FullBookcase : FurnitureContainer
    {
        [Constructable]
        public FullBookcase()
            : base(0xA97)
        {
            this.Weight = 1.0;
        }

        public FullBookcase(Serial serial)
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
            int version = (this.InheritsItem ? 0 : reader.ReadInt()); // Required for FurnitureContainer insertion
        }
    }

    [Furniture]
    [Flipable(0xa9d, 0xa9e)]
    public class EmptyBookcase : FurnitureContainer
    {
        [Constructable]
        public EmptyBookcase()
            : base(0xA9D)
        {
        }

        public EmptyBookcase(Serial serial)
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
            int version = (this.InheritsItem ? 0 : reader.ReadInt()); // Required for FurnitureContainer insertion
        }
    }

    [Furniture]
    [Flipable(0xa2c, 0xa34)]
    public class Drawer : FurnitureContainer
    {
        [Constructable]
        public Drawer()
            : base(0xA2C)
        {
            this.Weight = 1.0;
        }

        public Drawer(Serial serial)
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
            int version = (this.InheritsItem ? 0 : reader.ReadInt()); // Required for FurnitureContainer insertion
        }
    }

    [Furniture]
    [Flipable(0xa30, 0xa38)]
    public class FancyDrawer : FurnitureContainer
    {
        [Constructable]
        public FancyDrawer()
            : base(0xA30)
        {
            this.Weight = 1.0;
        }

        public FancyDrawer(Serial serial)
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
            int version = (this.InheritsItem ? 0 : reader.ReadInt()); // Required for FurnitureContainer insertion
        }
    }

    [Furniture]
    [Flipable(0xa4f, 0xa53)]
    public class Armoire : FurnitureContainer
    {
        [Constructable]
        public Armoire()
            : base(0xA4F)
        {
            this.Weight = 1.0;
        }

        public Armoire(Serial serial)
            : base(serial)
        {
        }

        public override void DisplayTo(Mobile m)
        {
            if (DynamicFurniture.Open(this, m))
                base.DisplayTo(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = (this.InheritsItem ? 0 : reader.ReadInt()); // Required for FurnitureContainer insertion

            DynamicFurniture.Close(this);
        }
    }

    [Furniture]
    [Flipable(0xa4d, 0xa51)]
    public class FancyArmoire : FurnitureContainer
    {
        [Constructable]
        public FancyArmoire()
            : base(0xA4D)
        {
            this.Weight = 1.0;
        }

        public FancyArmoire(Serial serial)
            : base(serial)
        {
        }

        public override void DisplayTo(Mobile m)
        {
            if (DynamicFurniture.Open(this, m))
                base.DisplayTo(m);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = (this.InheritsItem ? 0 : reader.ReadInt()); // Required for FurnitureContainer insertion

            DynamicFurniture.Close(this);
        }
    }

    public class DynamicFurniture
    {
        private static readonly Dictionary<Container, Timer> m_Table = new Dictionary<Container, Timer>();
        public static bool Open(Container c, Mobile m)
        {
            if (m_Table.ContainsKey(c))
            {
                c.SendRemovePacket();
                Close(c);
                c.Delta(ItemDelta.Update);
                c.ProcessDelta();
                return false;
            }

            if (c is Armoire || c is FancyArmoire)
            {
                Timer t = new FurnitureTimer(c, m);
                t.Start();
                m_Table[c] = t;

                switch ( c.ItemID )
                {
                    case 0xA4D:
                        c.ItemID = 0xA4C;
                        break;
                    case 0xA4F:
                        c.ItemID = 0xA4E;
                        break;
                    case 0xA51:
                        c.ItemID = 0xA50;
                        break;
                    case 0xA53:
                        c.ItemID = 0xA52;
                        break;
                }
            }

            return true;
        }

        public static void Close(Container c)
        {
            Timer t = null;

            m_Table.TryGetValue(c, out t);

            if (t != null)
            {
                t.Stop();
                m_Table.Remove(c);
            }

            if (c is Armoire || c is FancyArmoire)
            {
                switch ( c.ItemID )
                {
                    case 0xA4C:
                        c.ItemID = 0xA4D;
                        break;
                    case 0xA4E:
                        c.ItemID = 0xA4F;
                        break;
                    case 0xA50:
                        c.ItemID = 0xA51;
                        break;
                    case 0xA52:
                        c.ItemID = 0xA53;
                        break;
                }
            }
        }
    }

    public class FurnitureTimer : Timer
    {
        private readonly Container m_Container;
        private readonly Mobile m_Mobile;
        public FurnitureTimer(Container c, Mobile m)
            : base(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.5))
        {
            this.Priority = TimerPriority.TwoFiftyMS;

            this.m_Container = c;
            this.m_Mobile = m;
        }

        protected override void OnTick()
        {
            if (this.m_Mobile.Map != this.m_Container.Map || !this.m_Mobile.InRange(this.m_Container.GetWorldLocation(), 3))
                DynamicFurniture.Close(this.m_Container);
        }
    }
}