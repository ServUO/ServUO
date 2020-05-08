namespace Server.Items
{
    public class LordBlackthornsExemplar : ChaosShield
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1079793;// Lord Blackthorn's Exemplar
        public override int ArtifactRarity => 11;
        public override int BasePhysicalResistance => 6;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 10;
        public override int BasePoisonResistance => 15;
        public override int BaseEnergyResistance => 10;
        public override int InitMinHits => 150;
        public override int InitMaxHits => 150;

        [Constructable]
        public LordBlackthornsExemplar()
            : base()
        {
            Hue = 0x501;
        }

        public LordBlackthornsExemplar(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
