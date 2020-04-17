namespace Server.Items
{
    [TypeAlias("Server.Items.Dressform")]
    public class DressformFront : Item
    {
        [Constructable]
        public DressformFront()
            : base(0xec6)
        {
            Weight = 10;
        }

        public DressformFront(Serial serial)
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

    public class DressformSide : Item
    {
        [Constructable]
        public DressformSide()
            : base(0xec7)
        {
            Weight = 10;
        }

        public DressformSide(Serial serial)
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