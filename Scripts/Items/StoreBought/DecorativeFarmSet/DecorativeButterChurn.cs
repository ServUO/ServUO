namespace Server.Items
{
    public class DecorativeButterChurn : Item
    {
        public override int LabelNumber => 1125749; // churn

        [Constructable]
        public DecorativeButterChurn()
            : base(0xA2FD)
        {
            Weight = 1;
            LootType = LootType.Blessed;
        }

        public DecorativeButterChurn(Serial serial)
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
