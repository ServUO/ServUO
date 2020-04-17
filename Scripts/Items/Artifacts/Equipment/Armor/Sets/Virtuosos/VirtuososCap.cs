namespace Server.Items
{
    public class VirtuososCap : JesterHat
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1151320;  // Virtuoso's Cap

        public override SetItem SetID => SetItem.Virtuoso;
        public override int Pieces => 4;
        public override bool BardMasteryBonus => true;

        public override int BasePhysicalResistance => 3;
        public override int BaseFireResistance => 8;
        public override int BaseColdResistance => 23;
        public override int BasePoisonResistance => 8;
        public override int BaseEnergyResistance => 8;
        public override int InitMinHits => 125;
        public override int InitMaxHits => 125;

        [Constructable]
        public VirtuososCap() : base()
        {
            Hue = 1374;
            Weight = 5;
            StrRequirement = 10;
            SetHue = 1374;
        }

        public VirtuososCap(Serial serial) : base(serial)
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