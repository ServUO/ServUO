using System;
using Server.Items;

namespace Server.Items
{
    public class BestialEarrings : GargishEarrings, ISetItem
    {
        public override int LabelNumber { get { return 1151543; } } // Bestial Earrings

        #region ISetItem Members
        public override SetItem SetID { get { return SetItem.Bestial; } }
        public override int Pieces { get { return 4; } }
        public override int Berserk { get { return 1; } }
        #endregion        

        public override int BasePhysicalResistance { get { return 3; } }
        public override int BaseFireResistance { get { return 4; } }
        public override int BaseColdResistance { get { return 4; } }
        public override int BasePoisonResistance { get { return 4; } }
        public override int BaseEnergyResistance { get { return 17; } }
        public override int InitMinHits { get { return 125; } }
        public override int InitMaxHits { get { return 125; } }

        [Constructable]
        public BestialEarrings()
        {
            Hue = 2010;
            this.Weight = 1;
            SetHue = 2010;
        }

        public BestialEarrings(Serial serial)
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