namespace Server.Items
{
    public class Wasabi : Item
    {
        [Constructable]
        public Wasabi()
            : base(0x24E8)
        {
            Weight = 1.0;
        }

        public Wasabi(Serial serial)
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

    public class WasabiClumps : Food
    {
        [Constructable]
        public WasabiClumps()
            : base(0x24EB)
        {
            Stackable = false;
            Weight = 1.0;
            FillFactor = 2;
        }

        public WasabiClumps(Serial serial)
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

    public class EmptyBentoBox : Item
    {
        [Constructable]
        public EmptyBentoBox()
            : base(0x2834)
        {
            Weight = 5.0;
        }

        public EmptyBentoBox(Serial serial)
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

    public class BentoBox : Food
    {
        [Constructable]
        public BentoBox()
            : base(0x2836)
        {
            Stackable = false;
            Weight = 5.0;
            FillFactor = 2;
        }

        public BentoBox(Serial serial)
            : base(serial)
        {
        }

        public override bool Eat(Mobile from)
        {
            if (!base.Eat(from))
                return false;

            from.AddToBackpack(new EmptyBentoBox());
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

    public class SushiRolls : Food
    {
        [Constructable]
        public SushiRolls()
            : this(1)
        {
        }

        [Constructable]
        public SushiRolls(int amount)
            : base(0x283E)
        {
            Stackable = true;
            Amount = amount;
            Weight = 3.0;
            FillFactor = 2;
        }

        public SushiRolls(Serial serial)
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

    public class SushiPlatter : Food
    {
        [Constructable]
        public SushiPlatter()
            : this(1)
        {
        }

        [Constructable]
        public SushiPlatter(int amount)
            : base(0x2840)
        {
            Stackable = true;
            Amount = amount;
            Weight = 3.0;
            FillFactor = 2;
        }

        public SushiPlatter(Serial serial)
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

    public class GreenTeaBasket : Item
    {
        [Constructable]
        public GreenTeaBasket()
            : base(0x284B)
        {
            Weight = 1.0;
            Stackable = true;
        }

        public GreenTeaBasket(Serial serial)
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

    public class GreenTea : Food
    {
        [Constructable]
        public GreenTea()
            : base(0x284C)
        {
            Stackable = false;
            Weight = 4.0;
            FillFactor = 2;
        }

        public GreenTea(Serial serial)
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

    public class MisoSoup : Food
    {
        [Constructable]
        public MisoSoup()
            : base(0x284D)
        {
            Stackable = false;
            Weight = 4.0;
            FillFactor = 2;
        }

        public MisoSoup(Serial serial)
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

    public class WhiteMisoSoup : Food
    {
        [Constructable]
        public WhiteMisoSoup()
            : base(0x284E)
        {
            Stackable = false;
            Weight = 4.0;
            FillFactor = 2;
        }

        public WhiteMisoSoup(Serial serial)
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

    public class RedMisoSoup : Food
    {
        [Constructable]
        public RedMisoSoup()
            : base(0x284F)
        {
            Stackable = false;
            Weight = 4.0;
            FillFactor = 2;
        }

        public RedMisoSoup(Serial serial)
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

    public class AwaseMisoSoup : Food
    {
        [Constructable]
        public AwaseMisoSoup()
            : base(0x2850)
        {
            Stackable = false;
            Weight = 4.0;
            FillFactor = 2;
        }

        public AwaseMisoSoup(Serial serial)
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
