namespace Server.Items
{
    public class SteamPoweredBeverageMaker : Item
    {
        public override int LabelNumber => 1123598;  // Steam Powered Beverage Maker

        [Constructable]
        public SteamPoweredBeverageMaker()
            : base(0x9A96)
        {
            LootType = LootType.Blessed;
        }

        public SteamPoweredBeverageMaker(Serial serial)
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
