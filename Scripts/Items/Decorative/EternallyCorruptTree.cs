namespace Server.Items
{
    public class EternallyCorruptTree : Item
    {
        [Constructable]
        public EternallyCorruptTree()
            : base(0x20FA)
        {
            Hue = Utility.RandomMinMax(0x899, 0x8B0);
        }

        public EternallyCorruptTree(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1072093;// Eternally Corrupt Tree
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