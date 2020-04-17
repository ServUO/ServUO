namespace Server.Items
{
    [Flipable(0xA48D, 0xA48E)]
    public class PaintingAnkh : Item
    {
        public override int LabelNumber => 1023744;  // painting

        [Constructable]
        public PaintingAnkh()
            : base(0xA48D)
        {
            Weight = 10;
        }

        public PaintingAnkh(Serial serial)
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
