using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    public class BestialEarrings : GargishEarrings, ISetItem
    {
        public override bool IsArtifact { get { return true; } }
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
            this.Hue = 2010;
            this.Weight = 1;
        }

        public BestialEarrings(Serial serial)
            : base(serial)
        {
        }

        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            if (parent is PlayerMobile)
            {
                PlayerMobile pm = parent as PlayerMobile;

                this.Hue = pm.AddBestialHueParent();
            }
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            if (parent is PlayerMobile && !Deleted)
            {
                PlayerMobile pm = parent as PlayerMobile;

                this.Hue = 2010;
                pm.DropBestialHueParent();
            }
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

            this.Hue = 2010;
        }
    }
}