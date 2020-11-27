namespace Server.Items
{
    public class KhaldunFirstAidBelt : FirstAidBelt
    {
        public override bool IsArtifact => true;

        [Constructable]
        public KhaldunFirstAidBelt()
        {
            LootType = LootType.Blessed;
            WeightReduction = 50;
            Attributes.RegenHits = 2;
            HealingBonus = 10;
        }

        public KhaldunFirstAidBelt(Serial serial)
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
            reader.ReadInt();
        }
    }
}
