using System;
using System.Collections.Generic;

namespace Server.Items
{
    [Furniture]
    [Flipable(0x2815, 0x2816)]
    public class TallCabinet : BaseContainer
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
            int version = reader.ReadInt();
        }
    }

    [Furniture]
    [Flipable(0x2817, 0x2818)]
    public class ShortCabinet : BaseContainer
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
            int version = reader.ReadInt();
        }
    }

    [Furniture]
    [Flipable(0x2857, 0x2858)]
    public class RedArmoire : BaseContainer
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
            int version = reader.ReadInt();
        }
    }

    [Furniture]
    [Flipable(0x285D, 0x285E)]
    public class CherryArmoire : BaseContainer
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
            int version = reader.ReadInt();
        }
    }

    [Furniture]
    [Flipable(0x285B, 0x285C)]
    public class MapleArmoire : BaseContainer
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
            int version = reader.ReadInt();
        }
    }

    [Furniture]
    [Flipable(0x2859, 0x285A)]
    public class ElegantArmoire : BaseContainer
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
            int version = reader.ReadInt();
        }
    }

    [Furniture]
    [Flipable(0xa97, 0xa99, 0xa98, 0xa9a, 0xa9b, 0xa9c)]
    public class FullBookcase : BaseContainer
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
            int version = reader.ReadInt();
        }
    }

    [Furniture]
    [Flipable(0xa9d, 0xa9e)]
    public class EmptyBookcase : BaseContainer
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

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0 && this.Weight == 1.0)
                this.Weight = -1;
        }
    }

    [Furniture]
    [Flipable(0xa2c, 0xa34)]
    public class Drawer : BaseContainer
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
            int version = reader.ReadInt();
        }
    }

    [Furniture]
    [Flipable(0xa30, 0xa38)]
    public class FancyDrawer : BaseContainer
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
            int version = reader.ReadInt();
        }
    }

    [Furniture]
    [Flipable(0xa4f, 0xa53)]
    public class Armoire : BaseContainer
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
            int version = reader.ReadInt();

            DynamicFurniture.Close(this);
        }
    }

    [Furniture]
    [Flipable(0xa4d, 0xa51)]
    public class FancyArmoire : BaseContainer
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
            int version = reader.ReadInt();

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