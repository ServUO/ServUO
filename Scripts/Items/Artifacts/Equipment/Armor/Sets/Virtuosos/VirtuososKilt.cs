namespace Server.Items
{
    public class VirtuososKilt : GargishClothKilt, ISetItem
    {
        public override int LabelNumber => 1151559;  // Virtuoso's Kilt

        #region ISetItem Members
        public override SetItem SetID => SetItem.Virtuoso;
        public override int Pieces => 4;
        public override bool BardMasteryBonus => true;
        #endregion

        public override int BasePhysicalResistance => 7;
        public override int BaseFireResistance => 8;
        public override int BaseColdResistance => 21;
        public override int BasePoisonResistance => 8;
        public override int BaseEnergyResistance => 8;
        public override int InitMinHits => 125;
        public override int InitMaxHits => 125;

        [Constructable]
        public VirtuososKilt()
        {
            Hue = 1374;
            Weight = 5;
            SetHue = 1374;
        }

        public VirtuososKilt(Serial serial)
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