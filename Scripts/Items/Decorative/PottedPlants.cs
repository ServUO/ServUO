namespace Server.Items
{
    public class PottedPlant : Item
    {
        [Constructable]
        public PottedPlant()
            : base(0x11CA)
        {
            Weight = 100;
        }

        public PottedPlant(Serial serial)
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

    public class PottedPlant1 : Item
    {
        [Constructable]
        public PottedPlant1()
            : base(0x11CB)
        {
            Weight = 100;
        }

        public PottedPlant1(Serial serial)
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

    public class PottedPlant2 : Item
    {
        [Constructable]
        public PottedPlant2()
            : base(0x11CC)
        {
            Weight = 100;
        }

        public PottedPlant2(Serial serial)
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