using System;
using Server.Items;

namespace Server.Items
{
    public class VirtuososNecklace : GargishNecklace, ISetItem
    {
        public override int LabelNumber { get { return 1151556; } } // Virtuoso's Necklace

        #region ISetItem Members
        public override SetItem SetID { get { return SetItem.Virtuoso; } }
        public override int Pieces { get { return 4; } }
        public override bool BardMasteryBonus { get { return true; } }
        #endregion

        public override int BasePhysicalResistance { get { return 3; } }
        public override int BaseFireResistance { get { return 4; } }
        public override int BaseColdResistance { get { return 4; } }
        public override int BasePoisonResistance { get { return 4; } }
        public override int BaseEnergyResistance { get { return 17; } }
        public override int InitMinHits { get { return 125; } }
        public override int InitMaxHits { get { return 125; } }

        [Constructable]
        public VirtuososNecklace()
        {
            this.Hue = 1374;
            this.Weight = 1;
            this.SetHue = 1374;
        }

        public VirtuososNecklace(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}