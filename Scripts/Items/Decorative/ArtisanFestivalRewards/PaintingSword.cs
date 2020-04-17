namespace Server.Items
{
    [Flipable(0xA48B, 0xA48C)]
    public class PaintingSword : Item
    {
        public override int LabelNumber => 1023744;  // painting

        [Constructable]
        public PaintingSword()
            : base(0xA48B)
        {
            Weight = 10;
        }

        public PaintingSword(Serial serial)
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
