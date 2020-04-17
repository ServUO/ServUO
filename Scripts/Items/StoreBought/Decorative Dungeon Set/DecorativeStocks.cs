namespace Server.Items
{
    [Flipable(0xA4ED, 0xA4EE)]
    public class DecorativeStocks : Item
    {
        public override int LabelNumber => 1159472;  // decorative stocks

        [Constructable]
        public DecorativeStocks()
            : base(0xA4ED)
        {
            Weight = 1;
            LootType = LootType.Blessed;
        }

        public DecorativeStocks(Serial serial)
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
