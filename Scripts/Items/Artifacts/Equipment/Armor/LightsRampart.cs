namespace Server.Items
{
    public class LightsRampart : MetalShield
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1112407;// Light's Rampart 
        public override int ArtifactRarity => 11;
        public override int BasePhysicalResistance => 4;
        public override int BaseFireResistance => 5;
        public override int BaseColdResistance => 13;
        public override int BasePoisonResistance => 3;
        public override int BaseEnergyResistance => 3;
        public override int InitMinHits => 150;
        public override int InitMaxHits => 150;

        [Constructable]
        public LightsRampart()
        {
            Hue = 1272;
            Attributes.SpellChanneling = 1;
            Attributes.DefendChance = 20;
        }

        public LightsRampart(Serial serial)
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
