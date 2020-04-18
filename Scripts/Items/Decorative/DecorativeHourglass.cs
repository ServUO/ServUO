namespace Server.Items
{
    [Flipable(0xA339, 0xA33A)]
    public class DecorativeHourglass : Item
    {
        public override int LabelNumber => 1125809;  // hourglass

        [Constructable]
        public DecorativeHourglass()
            : base(0xA339)
        {
        }

        public DecorativeHourglass(Serial serial)
            : base(serial)
        {
        }

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
