namespace Server.Items
{
    public class PottedCactus : Item
    {
        [Constructable]
        public PottedCactus()
            : base(0x1E0F)
        {
            Weight = 100;
        }

        public PottedCactus(Serial serial)
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

    public class PottedCactus1 : Item
    {
        [Constructable]
        public PottedCactus1()
            : base(0x1E10)
        {
            Weight = 100;
        }

        public PottedCactus1(Serial serial)
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

    public class PottedCactus2 : Item
    {
        [Constructable]
        public PottedCactus2()
            : base(0x1E11)
        {
            Weight = 100;
        }

        public PottedCactus2(Serial serial)
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

    public class PottedCactus3 : Item
    {
        [Constructable]
        public PottedCactus3()
            : base(0x1E12)
        {
            Weight = 100;
        }

        public PottedCactus3(Serial serial)
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

    public class PottedCactus4 : Item
    {
        [Constructable]
        public PottedCactus4()
            : base(0x1E13)
        {
            Weight = 100;
        }

        public PottedCactus4(Serial serial)
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

    public class PottedCactus5 : Item
    {
        [Constructable]
        public PottedCactus5()
            : base(0x1E14)
        {
            Weight = 100;
        }

        public PottedCactus5(Serial serial)
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