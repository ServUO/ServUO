using System;

namespace Server.Items
{
    public class EmptyWoodenBowl : Item
    {
        [Constructable]
        public EmptyWoodenBowl()
            : base(0x15F8)
        {
            this.Weight = 1.0;
        }

        public EmptyWoodenBowl(Serial serial)
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

    public class EmptyPewterBowl : Item
    {
        [Constructable]
        public EmptyPewterBowl()
            : base(0x15FD)
        {
            this.Weight = 1.0;
        }

        public EmptyPewterBowl(Serial serial)
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

    public class WoodenBowlOfCarrots : Food
    {
        [Constructable]
        public WoodenBowlOfCarrots()
            : base(0x15F9)
        {
            this.Stackable = false;
            this.Weight = 1.0;
            this.FillFactor = 2;
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

            writer.Write((int)0); // version
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
            this.Stackable = false;
            this.Weight = 1.0;
            this.FillFactor = 2;
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

            writer.Write((int)0); // version
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
            this.Stackable = false;
            this.Weight = 1.0;
            this.FillFactor = 2;
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

            writer.Write((int)0); // version
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
            this.Stackable = false;
            this.Weight = 1.0;
            this.FillFactor = 2;
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

            writer.Write((int)0); // version
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
            this.Stackable = false;
            this.Weight = 1.0;
            this.FillFactor = 2;
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

            writer.Write((int)0); // version
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
            this.Stackable = false;
            this.Weight = 1.0;
            this.FillFactor = 2;
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

            writer.Write((int)0); // version
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
            this.Stackable = false;
            this.Weight = 1.0;
            this.FillFactor = 2;
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

            writer.Write((int)0); // version
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
            this.Stackable = false;
            this.Weight = 1.0;
            this.FillFactor = 2;
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

            writer.Write((int)0); // version
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
            this.Stackable = false;
            this.Weight = 1.0;
            this.FillFactor = 2;
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

            writer.Write((int)0); // version
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
            this.Weight = 2.0;
        }

        public EmptyWoodenTub(Serial serial)
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

    [TypeAlias("Server.Items.EmptyLargePewterBowl")]
    public class EmptyPewterTub : Item
    {
        [Constructable]
        public EmptyPewterTub()
            : base(0x1603)
        {
            this.Weight = 2.0;
        }

        public EmptyPewterTub(Serial serial)
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

    public class BowlOfRotwormStew : Food
    {
        public override int LabelNumber { get { return 1031706; } } // bowl of rotworm stew

        [Constructable]
        public BowlOfRotwormStew()
            : base(0x2DBA)
        {
            this.Stackable = false;
            this.Weight = 2.0;
            this.FillFactor = 1;
        }

        public BowlOfRotwormStew(Serial serial)
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

    public class WoodenBowlOfStew : Food
    {
        [Constructable]
        public WoodenBowlOfStew()
            : base(0x1604)
        {
            this.Stackable = false;
            this.Weight = 2.0;
            this.FillFactor = 2;
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

            writer.Write((int)0); // version
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
            this.Stackable = false;
            this.Weight = 2.0;
            this.FillFactor = 2;
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

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
