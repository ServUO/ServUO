namespace Server.Items
{
    public class VirtuososTunic : StuddedChest, ISetItem
    {
        public override int LabelNumber => 1151321;  // Virtuoso's Tunic

        #region ISetItem Members
        public override SetItem SetID => SetItem.Virtuoso;
        public override int Pieces => 4;
        public override bool BardMasteryBonus => true;
        #endregion

        public override int BasePhysicalResistance => 6;
        public override int BaseFireResistance => 20;
        public override int BaseColdResistance => 7;
        public override int BasePoisonResistance => 7;
        public override int BaseEnergyResistance => 8;
        public override int InitMinHits => 125;
        public override int InitMaxHits => 125;

        [Constructable]
        public VirtuososTunic()
        {
            Hue = 1374;
            StrRequirement = 35;
            SetHue = 1374;
        }

        public VirtuososTunic(Serial serial)
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