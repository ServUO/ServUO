using System;
using Server.Items;

namespace Server.Items
{
    public class VirtuososTunic : StuddedChest, ISetItem
    {
        public override int LabelNumber { get { return 1151321; } } // Virtuoso's Tunic

        #region ISetItem Members
        public override SetItem SetID { get { return SetItem.Virtuoso; } }
        public override int Pieces { get { return 4; } }
        public override bool BardMasteryBonus { get { return true; } }
        #endregion

        public override int BasePhysicalResistance { get { return 6; } }
        public override int BaseFireResistance { get { return 20; } }
        public override int BaseColdResistance { get { return 7; } }
        public override int BasePoisonResistance { get { return 7; } }
        public override int BaseEnergyResistance { get { return 8; } }
        public override int InitMinHits { get { return 125; } }
        public override int InitMaxHits { get { return 125; } }

        [Constructable]
        public VirtuososTunic()
        {
            this.Hue = 1374;
            this.StrRequirement = 35;
            this.SetHue = 1374;
        }

        public VirtuososTunic(Serial serial)
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