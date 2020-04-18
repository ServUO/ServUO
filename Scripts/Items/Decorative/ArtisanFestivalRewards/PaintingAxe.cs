namespace Server.Items
{
    [Flipable(0xA489, 0xA48A)]
    public class PaintingAxe : Item
    {
        public override int LabelNumber => 1023744;  // painting

        [Constructable]
        public PaintingAxe()
            : base(0xA489)
        {
            Weight = 10;
        }

        public PaintingAxe(Serial serial)
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
