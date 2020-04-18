namespace Server.Items
{
    public class VirtuososEarpieces : GargishEarrings, ISetItem
    {
        public override int LabelNumber => 1151557;  // Virtuoso's Earpieces

        #region ISetItem Members
        public override SetItem SetID => SetItem.Virtuoso;
        public override int Pieces => 4;
        public override bool BardMasteryBonus => true;
        #endregion

        public override int BasePhysicalResistance => 3;
        public override int BaseFireResistance => 4;
        public override int BaseColdResistance => 4;
        public override int BasePoisonResistance => 4;
        public override int BaseEnergyResistance => 17;
        public override int InitMinHits => 125;
        public override int InitMaxHits => 125;

        [Constructable]
        public VirtuososEarpieces()
        {
            Hue = 1374;
            Weight = 1;
            SetHue = 1374;
        }

        public VirtuososEarpieces(Serial serial)
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