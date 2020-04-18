namespace Server.Items
{
    [Flipable(0xA495, 0xA496)]
    public class PillowMoon : Item
    {
        public override int LabelNumber => 1025015;  // pillow

        [Constructable]
        public PillowMoon()
            : base(0xA495)
        {
            Weight = 1;
        }

        public PillowMoon(Serial serial)
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
