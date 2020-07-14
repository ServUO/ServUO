namespace Server.Items
{
    [Furniture]
    [Flipable(0xB32, 0xB33)]
    public class Throne : CraftableFurniture
    {
        [Constructable]
        public Throne()
            : base(0xB33)
        {
            Weight = 1.0;
        }

        public Throne(Serial serial)
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

    [Furniture]
    [Flipable(0xB2E, 0xB2F, 0xB31, 0xB30)]
    public class WoodenThrone : CraftableFurniture
    {
        [Constructable]
        public WoodenThrone()
            : base(0xB2E)
        {
            Weight = 15.0;
        }

        public WoodenThrone(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1015082;  // Wooden Throne

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
