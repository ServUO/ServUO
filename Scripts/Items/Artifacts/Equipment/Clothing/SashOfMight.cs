namespace Server.Items
{
    [Flipable(0x1541, 0x1542)]
    public class SashOfMight : BodySash
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1075412;  // Sash of Might

        [Constructable]
        public SashOfMight() : base(0x1541)
        {
            Hue = 0x481;
        }

        public SashOfMight(Serial serial) : base(serial)
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