using System;
using Server.Network;

namespace Server.Items
{
    [Flipable(0x2A5D, 0x2A61)]
    public class AwesomeDisturbingPortraitComponent : AddonComponent
    {
        private InternalTimer m_Timer;
        public AwesomeDisturbingPortraitComponent()
            : base(0x2A5D)
        {
            this.m_Timer = new InternalTimer(this, TimeSpan.FromSeconds(1));
            this.m_Timer.Start();
        }

        public AwesomeDisturbingPortraitComponent(Serial serial)
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
        public bool FacingSouth
        {
            get
            {
                return this.ItemID < 0x2A61;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (Utility.InRange(this.Location, from.Location, 2))
            {
                int hours;
                int minutes;

                Clock.GetTime(this.Map, this.X, this.Y, out hours, out minutes);

                if (hours < 4 || hours > 20)
                    Effects.PlaySound(this.Location, this.Map, 0x569);

                this.UpdateImage();
            }
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

            this.m_Timer = new InternalTimer(this, TimeSpan.Zero);
            this.m_Timer.Start();
        }

        private void UpdateImage()
        {
            int hours;
            int minutes;

            Clock.GetTime(this.Map, this.X, this.Y, out hours, out minutes);

            if (this.FacingSouth)
            {
                if (hours < 4)
                    this.ItemID = 0x2A60;
                else if (hours < 6)
                    this.ItemID = 0x2A5F;
                else if (hours < 8)
                    this.ItemID = 0x2A5E;
                else if (hours < 16)
                    this.ItemID = 0x2A5D;
                else if (hours < 18)
                    this.ItemID = 0x2A5E;
                else if (hours < 20)
                    this.ItemID = 0x2A5F;
                else
                    this.ItemID = 0x2A60;
            }
            else
            {
                if (hours < 4)
                    this.ItemID = 0x2A64;
                else if (hours < 6)
                    this.ItemID = 0x2A63;
                else if (hours < 8)
                    this.ItemID = 0x2A62;
                else if (hours < 16)
                    this.ItemID = 0x2A61;
                else if (hours < 18)
                    this.ItemID = 0x2A62;
                else if (hours < 20)
                    this.ItemID = 0x2A63;
                else
                    this.ItemID = 0x2A64;
            }
        }

        private class InternalTimer : Timer
        {
            private readonly AwesomeDisturbingPortraitComponent m_Component;
            public InternalTimer(AwesomeDisturbingPortraitComponent c, TimeSpan delay)
                : base(delay, TimeSpan.FromMinutes(10))
            {
                this.m_Component = c;

                this.Priority = TimerPriority.OneMinute;
            }

            protected override void OnTick()
            {
                if (this.m_Component != null && !this.m_Component.Deleted)
                    this.m_Component.UpdateImage();
            }
        }
    }

    public class AwesomeDisturbingPortraitAddon : BaseAddon
    {
        [Constructable]
        public AwesomeDisturbingPortraitAddon()
            : base()
        {
            this.AddComponent(new AwesomeDisturbingPortraitComponent(), 0, 0, 0);
        }

        public AwesomeDisturbingPortraitAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new AwesomeDisturbingPortraitDeed();
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

    public class AwesomeDisturbingPortraitDeed : BaseAddonDeed
    {
        [Constructable]
        public AwesomeDisturbingPortraitDeed()
            : base()
        {
            this.LootType = LootType.Blessed;
        }

        public AwesomeDisturbingPortraitDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new AwesomeDisturbingPortraitAddon();
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