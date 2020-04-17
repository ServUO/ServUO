namespace Server.Items
{
    public class YuccaTree : Item
    {
        [Constructable]
        public YuccaTree()
            : base(0x0D37)
        {
            Weight = 10;
        }

        public YuccaTree(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1023383;// yucca
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