namespace Server.Items
{
    [Flipable(0xc77, 0xc78)]
    public class Carrot : Food
    {
        [Constructable]
        public Carrot()
            : this(1)
        {
        }

        [Constructable]
        public Carrot(int amount)
            : base(amount, 0xc78)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public Carrot(Serial serial)
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

    [Flipable(0xc7b, 0xc7c)]
    public class Cabbage : Food
    {
        [Constructable]
        public Cabbage()
            : this(1)
        {
        }

        [Constructable]
        public Cabbage(int amount)
            : base(amount, 0xc7b)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public Cabbage(Serial serial)
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

    [Flipable(0xc6d, 0xc6e)]
    public class Onion : Food
    {
        [Constructable]
        public Onion()
            : this(1)
        {
        }

        [Constructable]
        public Onion(int amount)
            : base(amount, 0xc6d)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public Onion(Serial serial)
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

    [Flipable(0xc70, 0xc71)]
    public class Lettuce : Food
    {
        [Constructable]
        public Lettuce()
            : this(1)
        {
        }

        [Constructable]
        public Lettuce(int amount)
            : base(amount, 0xc70)
        {
            Weight = 1.0;
            FillFactor = 1;
        }

        public Lettuce(Serial serial)
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

    [Flipable(0xC6A, 0xC6B)]
    public class Pumpkin : Food
    {
        [Constructable]
        public Pumpkin()
            : this(1)
        {
        }

        [Constructable]
        public Pumpkin(int amount)
            : base(amount, 0xC6A)
        {
            Weight = 1.0;
            FillFactor = 8;
        }

        public Pumpkin(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version < 1)
            {
                if (FillFactor == 4)
                    FillFactor = 8;

                if (Weight == 5.0)
                    Weight = 1.0;
            }
        }
    }

    public class SmallPumpkin : Food
    {
        [Constructable]
        public SmallPumpkin()
            : this(1)
        {
        }

        [Constructable]
        public SmallPumpkin(int amount)
            : base(amount, 0xC6C)
        {
            Weight = 1.0;
            FillFactor = 8;
        }

        public SmallPumpkin(Serial serial)
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