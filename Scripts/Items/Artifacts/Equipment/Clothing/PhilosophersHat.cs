namespace Server.Items
{
    public class PhilosophersHat : WizardsHat
    {
        public override bool IsArtifact => true;
        [Constructable]
        public PhilosophersHat()
        {
            LootType = LootType.Blessed;
            Attributes.RegenMana = 1;
            Attributes.LowerRegCost = 7;
        }

        public PhilosophersHat(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1077602;// Philosopher's Hat
        public override int BasePhysicalResistance => 5;
        public override int BaseFireResistance => 5;
        public override int BaseColdResistance => 9;
        public override int BasePoisonResistance => 5;
        public override int BaseEnergyResistance => 5;
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