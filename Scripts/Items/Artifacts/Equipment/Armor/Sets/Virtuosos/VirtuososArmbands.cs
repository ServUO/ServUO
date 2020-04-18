namespace Server.Items
{
    public class VirtuososArmbands : GargishPlateArms
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1151558;  // Virtuoso's Armbands

        public override SetItem SetID => SetItem.Virtuoso;
        public override int Pieces => 4;
        public override bool BardMasteryBonus => true;

        public override int BasePhysicalResistance => 24;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 9;
        public override int BasePoisonResistance => 10;
        public override int BaseEnergyResistance => 9;
        public override int InitMinHits => 125;
        public override int InitMaxHits => 125;

        [Constructable]
        public VirtuososArmbands() : base()
        {
            Hue = 1374;
            Weight = 5;
            StrRequirement = 80;
            SetHue = 1374;
        }

        public VirtuososArmbands(Serial serial) : base(serial)
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