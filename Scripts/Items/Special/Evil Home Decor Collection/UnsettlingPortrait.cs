using System;
using Server.Network;

namespace Server.Items
{
    [Flipable(0x2A65, 0x2A67)]
    public class UnsettlingPortraitComponent : AddonComponent
    {
        private Timer m_Timer;
        public UnsettlingPortraitComponent()
            : base(0x2A65)
        {
            this.m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(3), new TimerCallback(ChangeDirection));
        }

        public UnsettlingPortraitComponent(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1074480;
            }
        }// Unsettling portrait
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

            if (this.m_Timer != null)
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

            this.m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(3), new TimerCallback(ChangeDirection));
        }

        private void ChangeDirection()
        {
            if (this.ItemID == 0x2A65)
                this.ItemID += 1;
            else if (this.ItemID == 0x2A66)
                this.ItemID -= 1;
            else if (this.ItemID == 0x2A67)
                this.ItemID += 1;
            else if (this.ItemID == 0x2A68)
                this.ItemID -= 1;
        }
    }

    public class UnsettlingPortraitAddon : BaseAddon
    {
        [Constructable]
        public UnsettlingPortraitAddon()
            : base()
        {
            this.AddComponent(new UnsettlingPortraitComponent(), 0, 0, 0);
        }

        public UnsettlingPortraitAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new UnsettlingPortraitDeed();
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

    public class UnsettlingPortraitDeed : BaseAddonDeed
    {
        [Constructable]
        public UnsettlingPortraitDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public UnsettlingPortraitDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new UnsettlingPortraitAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1074480;
            }
        }// Unsettling portrait
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