namespace Server.Items
{
    public class IcicleLargeSouth : Item
    {
        [Constructable]
        public IcicleLargeSouth()
            : base(0x4572)
        {
        }

        public IcicleLargeSouth(Serial serial)
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

    public class IcicleMedSouth : Item
    {
        [Constructable]
        public IcicleMedSouth()
            : base(0x4573)
        {
        }

        public IcicleMedSouth(Serial serial)
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

    public class IcicleSmallSouth : Item
    {
        [Constructable]
        public IcicleSmallSouth()
            : base(0x4574)
        {
        }

        public IcicleSmallSouth(Serial serial)
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

    public class IcicleLargeEast : Item
    {
        [Constructable]
        public IcicleLargeEast()
            : base(0x4575)
        {
        }

        public IcicleLargeEast(Serial serial)
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

    public class IcicleMedEast : Item
    {
        [Constructable]
        public IcicleMedEast()
            : base(0x4576)
        {
        }

        public IcicleMedEast(Serial serial)
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

    public class IcicleSmallEast : Item
    {
        [Constructable]
        public IcicleSmallEast()
            : base(0x4577)
        {
        }

        public IcicleSmallEast(Serial serial)
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