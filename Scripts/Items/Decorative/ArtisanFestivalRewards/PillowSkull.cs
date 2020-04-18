namespace Server.Items
{
    [Flipable(0xA497, 0xA498)]
    public class PillowSkull : Item
    {
        public override int LabelNumber => 1025015;  // pillow

        [Constructable]
        public PillowSkull()
            : base(0xA497)
        {
            Weight = 1;
        }

        public PillowSkull(Serial serial)
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
