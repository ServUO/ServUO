namespace Server.Items
{
    public class ThrashersTail : Item
    {
        [Constructable]
        public ThrashersTail()
            : base(0x1A9D)
        {
            LootType = LootType.Blessed;
            Hue = 0x455;
        }

        public ThrashersTail(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074230;// Thrasher's Tail
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