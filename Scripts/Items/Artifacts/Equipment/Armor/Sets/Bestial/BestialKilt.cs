using System;
using Server.Items;

namespace Server.Items
{
    public class BestialKilt : GargishClothKilt, ISetItem
    {
        public override int LabelNumber { get { return 1151546; } } // Bestial Kilt

        #region ISetItem Members
        public override SetItem SetID { get { return SetItem.Bestial; } }
        public override int Pieces { get { return 4; } }
        public override int Berserk { get { return 1; } }
        #endregion

        public override int BasePhysicalResistance { get { return 24; } }
        public override int BaseFireResistance { get { return 10; } }
        public override int BaseColdResistance { get { return 9; } }
        public override int BasePoisonResistance { get { return 10; } }
        public override int BaseEnergyResistance { get { return 9; } }
        public override int InitMinHits { get { return 125; } }
        public override int InitMaxHits { get { return 125; } }

        [Constructable]
        public BestialKilt()
        {
            Hue = 2010;
            this.Weight = 5;
            SetHue = 2010;
        }

        public BestialKilt(Serial serial)
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