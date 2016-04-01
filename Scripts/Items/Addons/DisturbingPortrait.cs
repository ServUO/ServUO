using System;
using Server.Network;

namespace Server.Items
{
    [Flipable(0x2A5D, 0x2A61)]
    public class DisturbingPortraitComponent : AddonComponent
    {
        private Timer m_Timer;
        public DisturbingPortraitComponent()
            : base(0x2A5D)
        {
            this.m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(3), new TimerCallback(Change));
        }

        public DisturbingPortraitComponent(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074479;
            }
        }// Disturbing portrait
        public override void OnDoubleClick(Mobile from)
        {
            if (Utility.InRange(this.Location, from.Location, 2))
                Effects.PlaySound(this.Location, this.Map, Utility.RandomMinMax(0x567, 0x568));
            else
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (this.m_Timer != null && this.m_Timer.Running)
                this.m_Timer.Stop();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(3), new TimerCallback(Change));
        }

        private void Change()
        {
            if (this.ItemID < 0x2A61)
                this.ItemID = Utility.RandomMinMax(0x2A5D, 0x2A60);
            else
                this.ItemID = Utility.RandomMinMax(0x2A61, 0x2A64);
        }
    }

    public class DisturbingPortraitAddon : BaseAddon
    {
        [Constructable]
        public DisturbingPortraitAddon()
            : base()
        {
            this.AddComponent(new DisturbingPortraitComponent(), 0, 0, 0);
        }

        public DisturbingPortraitAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new DisturbingPortraitDeed();
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class DisturbingPortraitDeed : BaseAddonDeed
    {
        [Constructable]
        public DisturbingPortraitDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public DisturbingPortraitDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new DisturbingPortraitAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1074479;
            }
        }// Disturbing portrait
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}