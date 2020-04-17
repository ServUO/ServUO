namespace Server.Items
{
    [Flipable(0xA493, 0xA494)]
    public class PillowTear : Item
    {
        public override int LabelNumber => 1025015;  // pillow

        [Constructable]
        public PillowTear()
            : base(0xA493)
        {
            Weight = 1;
        }

        public PillowTear(Serial serial)
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
