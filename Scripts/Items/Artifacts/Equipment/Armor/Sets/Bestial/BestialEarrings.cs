namespace Server.Items
{
    public class BestialEarrings : GargishEarrings, ISetItem
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1151543;  // Bestial Earrings

        #region ISetItem Members
        public override SetItem SetID => SetItem.Bestial;
        public override int Pieces => 4;
        #endregion

        public override int BasePhysicalResistance => 3;
        public override int BaseFireResistance => 4;
        public override int BaseColdResistance => 4;
        public override int BasePoisonResistance => 4;
        public override int BaseEnergyResistance => 17;
        public override int InitMinHits => 125;
        public override int InitMaxHits => 125;

        [Constructable]
        public BestialEarrings()
        {
            Hue = 2010;
            Weight = 1;
        }

        public BestialEarrings(Serial serial)
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
