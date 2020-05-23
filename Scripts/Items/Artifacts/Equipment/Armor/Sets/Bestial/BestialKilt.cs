namespace Server.Items
{
    public class BestialKilt : GargishClothKilt, ISetItem
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1151546;  // Bestial Kilt

        #region ISetItem Members
        public override SetItem SetID => SetItem.Bestial;
        public override int Pieces => 4;
        #endregion

        public override int BasePhysicalResistance => 24;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 9;
        public override int BasePoisonResistance => 10;
        public override int BaseEnergyResistance => 9;
        public override int InitMinHits => 125;
        public override int InitMaxHits => 125;

        [Constructable]
        public BestialKilt()
        {
            Hue = 2010;
            Weight = 5;
        }

        public BestialKilt(Serial serial)
            : base(serial)
        {
        }

        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            if (parent is Mobile && !Deleted)
            {
                BestialSetHelper.OnAdded((Mobile)parent, this);
            }
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            if (parent is Mobile && !Deleted)
            {
                BestialSetHelper.OnRemoved((Mobile)parent, this);
            }
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
