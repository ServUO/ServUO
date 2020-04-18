namespace Server.Items
{
    public class VirtuososKidGloves : LeatherGloves
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1151322;  // Virtuoso's Kid Gloves

        public override SetItem SetID => SetItem.Virtuoso;
        public override int Pieces => 4;
        public override bool BardMasteryBonus => true;

        public override int BasePhysicalResistance => 4;
        public override int BaseFireResistance => 19;
        public override int BaseColdResistance => 5;
        public override int BasePoisonResistance => 5;
        public override int BaseEnergyResistance => 5;
        public override int InitMinHits => 125;
        public override int InitMaxHits => 125;

        [Constructable]
        public VirtuososKidGloves() : base()
        {
            Hue = 1374;
            Weight = 1;
            StrRequirement = 20;
            SetHue = 1374;
        }

        public VirtuososKidGloves(Serial serial) : base(serial)
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