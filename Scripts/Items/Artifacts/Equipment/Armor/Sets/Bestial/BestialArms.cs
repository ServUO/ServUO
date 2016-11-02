using System;
using Server.Items;

namespace Server.Items
{
    public class BestialArms : GargishLeatherArms, ISetItem
    {
        public override int LabelNumber { get { return 1151545; } } // Bestial Arms

        #region ISetItem Members
        public override SetItem SetID { get { return SetItem.Bestial; } }
        public override int Pieces { get { return 4; } }
        public override int Berserk { get { return 1; } }
        #endregion

        public override int BasePhysicalResistance { get { return 7; } }
        public override int BaseFireResistance { get { return 8; } }
        public override int BaseColdResistance { get { return 21; } }
        public override int BasePoisonResistance { get { return 8; } }
        public override int BaseEnergyResistance { get { return 8; } }
        public override int InitMinHits { get { return 125; } }
        public override int InitMaxHits { get { return 125; } }

        [Constructable]
        public BestialArms()
        {
            ItemID = 0x4052;
            this.Hue = 2010;
            this.Weight = 4;
            this.StrRequirement = 25;
            this.SetHue = 2010;
        }

        public BestialArms(Serial serial)
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