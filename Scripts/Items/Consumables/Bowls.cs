namespace Server.Items
{
    public class EmptyWoodenBowl : Item
    {
        [Constructable]
        public EmptyWoodenBowl()
            : base(0x15F8)
        {
            Weight = 1.0;
        }

        public EmptyWoodenBowl(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class EmptyPewterBowl : Item
    {
        [Constructable]
        public EmptyPewterBowl()
            : base(0x15FD)
        {
            Weight = 1.0;
        }

        public EmptyPewterBowl(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class WoodenBowlOfCarrots : Food
    {
        [Constructable]
        public WoodenBowlOfCarrots()
            : base(0x15F9)
        {
            Stackable = false;
            Weight = 1.0;
            FillFactor = 2;
        }

        public WoodenBowlOfCarrots(Serial serial)
            : base(serial)
        {
        }

        public override bool Eat(Mobile from)
        {
            if (!base.Eat(from))
                return false;

            from.AddToBackpack(new EmptyWoodenBowl());
            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class WoodenBowlOfCorn : Food
    {
        [Constructable]
        public WoodenBowlOfCorn()
            : base(0x15FA)
        {
            Stackable = false;
            Weight = 1.0;
            FillFactor = 2;
        }

        public WoodenBowlOfCorn(Serial serial)
            : base(serial)
        {
        }

        public override bool Eat(Mobile from)
        {
            if (!base.Eat(from))
                return false;

            from.AddToBackpack(new EmptyWoodenBowl());
            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class WoodenBowlOfLettuce : Food
    {
        [Constructable]
        public WoodenBowlOfLettuce()
            : base(0x15FB)
        {
            Stackable = false;
            Weight = 1.0;
            FillFactor = 2;
        }

        public WoodenBowlOfLettuce(Serial serial)
            : base(serial)
        {
        }

        public override bool Eat(Mobile from)
        {
            if (!base.Eat(from))
                return false;

            from.AddToBackpack(new EmptyWoodenBowl());
            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class WoodenBowlOfPeas : Food
    {
        [Constructable]
        public WoodenBowlOfPeas()
            : base(0x15FC)
        {
            Stackable = false;
            Weight = 1.0;
            FillFactor = 2;
        }

        public WoodenBowlOfPeas(Serial serial)
            : base(serial)
        {
        }

        public override bool Eat(Mobile from)
        {
            if (!base.Eat(from))
                return false;

            from.AddToBackpack(new EmptyWoodenBowl());
            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class PewterBowlOfCarrots : Food
    {
        [Constructable]
        public PewterBowlOfCarrots()
            : base(0x15FE)
        {
            Stackable = false;
            Weight = 1.0;
            FillFactor = 2;
        }

        public PewterBowlOfCarrots(Serial serial)
            : base(serial)
        {
        }

        public override bool Eat(Mobile from)
        {
            if (!base.Eat(from))
                return false;

            from.AddToBackpack(new EmptyPewterBowl());
            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class PewterBowlOfCorn : Food
    {
        [Constructable]
        public PewterBowlOfCorn()
            : base(0x15FF)
        {
            Stackable = false;
            Weight = 1.0;
            FillFactor = 2;
        }

        public PewterBowlOfCorn(Serial serial)
            : base(serial)
        {
        }

        public override bool Eat(Mobile from)
        {
            if (!base.Eat(from))
                return false;

            from.AddToBackpack(new EmptyPewterBowl());
            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class PewterBowlOfLettuce : Food
    {
        [Constructable]
        public PewterBowlOfLettuce()
            : base(0x1600)
        {
            Stackable = false;
            Weight = 1.0;
            FillFactor = 2;
        }

        public PewterBowlOfLettuce(Serial serial)
            : base(serial)
        {
        }

        public override bool Eat(Mobile from)
        {
            if (!base.Eat(from))
                return false;

            from.AddToBackpack(new EmptyPewterBowl());
            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class PewterBowlOfPeas : Food
    {
        [Constructable]
        public PewterBowlOfPeas()
            : base(0x1601)
        {
            Stackable = false;
            Weight = 1.0;
            FillFactor = 2;
        }

        public PewterBowlOfPeas(Serial serial)
            : base(serial)
        {
        }

        public override bool Eat(Mobile from)
        {
            if (!base.Eat(from))
                return false;

            from.AddToBackpack(new EmptyPewterBowl());
            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class PewterBowlOfPotatos : Food
    {
        [Constructable]
        public PewterBowlOfPotatos()
            : base(0x1602)
        {
            Stackable = false;
            Weight = 1.0;
            FillFactor = 2;
        }

        public PewterBowlOfPotatos(Serial serial)
            : base(serial)
        {
        }

        public override bool Eat(Mobile from)
        {
            if (!base.Eat(from))
                return false;

            from.AddToBackpack(new EmptyPewterBowl());
            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [TypeAlias("Server.Items.EmptyLargeWoodenBowl")]
    public class EmptyWoodenTub : Item
    {
        [Constructable]
        public EmptyWoodenTub()
            : base(0x1605)
        {
            Weight = 2.0;
        }

        public EmptyWoodenTub(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [TypeAlias("Server.Items.EmptyLargePewterBowl")]
    public class EmptyPewterTub : Item
    {
        [Constructable]
        public EmptyPewterTub()
            : base(0x1603)
        {
            Weight = 2.0;
        }

        public EmptyPewterTub(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class BowlOfRotwormStew : Food
    {
        public override int LabelNumber => 1031706;  // bowl of rotworm stew

        [Constructable]
        public BowlOfRotwormStew()
            : base(0x2DBA)
        {
            Stackable = false;
            Weight = 2.0;
            FillFactor = 1;
        }

        public BowlOfRotwormStew(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class WoodenBowlOfStew : Food
    {
        [Constructable]
        public WoodenBowlOfStew()
            : base(0x1604)
        {
            Stackable = false;
            Weight = 2.0;
            FillFactor = 2;
        }

        public WoodenBowlOfStew(Serial serial)
            : base(serial)
        {
        }

        public override bool Eat(Mobile from)
        {
            if (!base.Eat(from))
                return false;

            from.AddToBackpack(new EmptyWoodenTub());
            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class WoodenBowlOfTomatoSoup : Food
    {
        [Constructable]
        public WoodenBowlOfTomatoSoup()
            : base(0x1606)
        {
            Stackable = false;
            Weight = 2.0;
            FillFactor = 2;
        }

        public WoodenBowlOfTomatoSoup(Serial serial)
            : base(serial)
        {
        }

        public override bool Eat(Mobile from)
        {
            if (!base.Eat(from))
                return false;

            from.AddToBackpack(new EmptyWoodenTub());
            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class BowlOfBlackrockStew : Food
    {
        public override int LabelNumber => 1115752;  // blackrock stew

        [Constructable]
        public BowlOfBlackrockStew()
            : base(0x2DBA)
        {
            Stackable = false;
            Weight = 2.0;
            FillFactor = 1;

            Hue = 1954;
        }

        public override bool Eat(Mobile from)
        {
            from.SendLocalizedMessage(1115751); // You don't want to eat this, it smells horrible.  It looks like food for some kind of demon.
            return false;
        }

        public BowlOfBlackrockStew(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
