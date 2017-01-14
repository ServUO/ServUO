using System;
using Server.Items;

namespace Server.Items
{
    public class VirtuososKilt : GargishClothKilt, ISetItem
    {
        public override int LabelNumber { get { return 1151559; } } // Virtuoso's Kilt

        #region ISetItem Members
        public override SetItem SetID { get { return SetItem.Virtuoso; } }
        public override int Pieces { get { return 4; } }
        public override bool BardMasteryBonus { get { return true; } }
        #endregion

        public override int BasePhysicalResistance { get { return 7; } }
        public override int BaseFireResistance { get { return 8; } }
        public override int BaseColdResistance { get { return 21; } }
        public override int BasePoisonResistance { get { return 8; } }
        public override int BaseEnergyResistance { get { return 8; } }
        public override int InitMinHits { get { return 125; } }
        public override int InitMaxHits { get { return 125; } }

        [Constructable]
        public VirtuososKilt()
        {
            this.Hue = 1374;
            this.Weight = 5;
            this.SetHue = 1374;
        }

        public VirtuososKilt(Serial serial)
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