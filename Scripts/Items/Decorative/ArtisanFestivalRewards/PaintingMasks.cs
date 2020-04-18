namespace Server.Items
{
    [Flipable(0xA48F, 0xA490)]
    public class PaintingMasks : Item
    {
        public override int LabelNumber => 1023744;  // painting

        [Constructable]
        public PaintingMasks()
            : base(0xA48F)
        {
            Weight = 10;
        }

        public PaintingMasks(Serial serial)
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
