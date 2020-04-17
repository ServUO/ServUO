namespace Server.Items
{
    public class BraceletOfResilience : GoldBracelet
    {
        public override bool IsArtifact => true;
        [Constructable]
        public BraceletOfResilience()
        {
            LootType = LootType.Blessed;
            Attributes.DefendChance = 5;
            Resistances.Fire = 5;
            Resistances.Cold = 5;
            Resistances.Poison = 5;
            Resistances.Energy = 5;
        }

        public BraceletOfResilience(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1077627;// Bracelet of Resilience
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}