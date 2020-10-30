namespace Server.Items
{
    [Flipable(0xA2FE, 0xA2FF)]
    public class DecorativeFeedingTrough : Item
    {
        public override int LabelNumber => 1125750; // trough

        [Constructable]
        public DecorativeFeedingTrough()
            : base(0xA2FE)
        {
            Weight = 1;
            LootType = LootType.Blessed;
        }

        public DecorativeFeedingTrough(Serial serial)
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
            reader.ReadInt();
        }
    }
}
