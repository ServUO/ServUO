namespace Server.Items
{
    public class AndrosGratitude : SmithHammer
    {
        [Constructable]
        public AndrosGratitude()
            : base(10)
        {
            LootType = LootType.Blessed;
        }

        public AndrosGratitude(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1075345;// Andros’ Gratitude

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
