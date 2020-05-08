namespace Server.Items
{
    public class OrcishVisage : OrcHelm
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1070691; // Orcish Visage
        public override int BasePhysicalResistance => 8;
        public override int BaseFireResistance => 5;
        public override int BaseColdResistance => 3;
        public override int BasePoisonResistance => 3;
        public override int BaseEnergyResistance => 5;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public OrcishVisage()
        {
            Hue = 0x592;
            ArmorAttributes.SelfRepair = 3;
            Attributes.BonusStr = 10;
            Attributes.BonusStam = 5;
        }

        public OrcishVisage(Serial serial)
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
