using System;
using Server;
using Server.Mobiles;
using Server.Engines.CityLoyalty;
using Server.Gumps;

namespace Server.Items
{
    public abstract class CityBook : Item
    {
        public abstract int Title { get; }
        public abstract int Content { get; }

        [Constructable]
        public CityBook() : base(4030)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if(Title > 0)
                list.Add(Title);

            list.Add(1154760, "Adamu Edom"); // By: ~1_NAME~
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (IsChildOf(m.Backpack))
            {
                Gump g = new Gump(150, 150);
                g.AddImage(0, 0, 30236);
                g.AddHtmlLocalized(110, 30, 350, 630, Content, false, false);

                m.SendGump(g);
            }
        }

        public static CityBook GetRandom()
        {
            switch (Utility.Random(9))
            {
                default:
                case 0: return new LoyaltyBookBritain();
                case 1: return new LoyaltyBookYew();
                case 2: return new LoyaltyBookMoonglow();
                case 3: return new LoyaltyBookNewMagincia();
                case 4: return new LoyaltyBookVesper();
                case 5: return new LoyaltyBookSkaraBrae();
                case 6: return new LoyaltyBookJhelom();
                case 7: return new LoyaltyBookMinoc();
                case 8: return new LoyaltyBookTrinsic();
            }
        }

        public CityBook(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class LoyaltyBookYew : CityBook
    {
        public override int Title { get { return 1153933; } }
        public override int Content { get { return 1152698; } }

        [Constructable]
        public LoyaltyBookYew()
        {
        }

        public LoyaltyBookYew(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class LoyaltyBookJhelom : CityBook
    {
        public override int Title { get { return 1153934; } }
        public override int Content { get { return 1153436; } }

        [Constructable]
        public LoyaltyBookJhelom()
        {
        }

        public LoyaltyBookJhelom(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class LoyaltyBookMoonglow : CityBook
    {
        public override int Title { get { return 1153935; } }
        public override int Content { get { return 1153437; } }

        [Constructable]
        public LoyaltyBookMoonglow()
        {
        }

        public LoyaltyBookMoonglow(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class LoyaltyBookTrinsic : CityBook
    {
        public override int Title { get { return 1153727; } }
        public override int Content { get { return 1151755; } }

        [Constructable]
        public LoyaltyBookTrinsic()
        {
        }

        public LoyaltyBookTrinsic(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class LoyaltyBookMinoc : CityBook
    {
        public override int Title { get { return 1153728; } }
        public override int Content { get { return 1152317; } }

        [Constructable]
        public LoyaltyBookMinoc()
        {
        }

        public LoyaltyBookMinoc(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class LoyaltyBookNewMagincia : CityBook
    {
        public override int Title { get { return 1153008; } }
        public override int Content { get { return 1153723; } }

        [Constructable]
        public LoyaltyBookNewMagincia()
        {
        }

        public LoyaltyBookNewMagincia(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class LoyaltyBookVesper : CityBook
    {
        public override int Title { get { return 1153012; } }
        public override int Content { get { return 1153724; } }

        [Constructable]
        public LoyaltyBookVesper()
        {
        }

        public LoyaltyBookVesper(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class LoyaltyBookSkaraBrae : CityBook
    {
        public override int Title { get { return 0; } }
        public override int Content { get { return 1153725; } }

        [Constructable]
        public LoyaltyBookSkaraBrae()
        {
        }

        public LoyaltyBookSkaraBrae(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class LoyaltyBookBritain : CityBook
    {
        public override int Title { get { return 1153726; } }
        public override int Content { get { return 1151754; } }

        [Constructable]
        public LoyaltyBookBritain()
        {
        }

        public LoyaltyBookBritain(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}