using System;

namespace Server.Items
{
    public class SpinningwheelSouthAddon : BaseAddon, ISpinningWheel
    {
        private Timer m_Timer;
        [Constructable]
        public SpinningwheelSouthAddon()
        {
            this.AddComponent(new AddonComponent(0x1015), 0, 0, 0);
        }

        public SpinningwheelSouthAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed
        {
            get
            {
                return new SpinningwheelSouthDeed();
            }
        }
        public bool Spinning
        {
            get
            {
                return this.m_Timer != null;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void OnComponentLoaded(AddonComponent c)
        {
            switch ( c.ItemID )
            {
                case 0x1016:
                case 0x101A:
                case 0x101D:
                case 0x10A5:
                    --c.ItemID;
                    break;
            }
        }

        public void BeginSpin(SpinCallback callback, Mobile from, int hue)
        {
            this.m_Timer = new SpinTimer(this, callback, from, hue);
            this.m_Timer.Start();

            foreach (AddonComponent c in this.Components)
            {
                switch ( c.ItemID )
                {
                    case 0x1015:
                    case 0x1019:
                    case 0x101C:
                    case 0x10A4:
                        ++c.ItemID;
                        break;
                }
            }
        }

        public void EndSpin(SpinCallback callback, Mobile from, int hue)
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = null;

            foreach (AddonComponent c in this.Components)
            {
                switch ( c.ItemID )
                {
                    case 0x1016:
                    case 0x101A:
                    case 0x101D:
                    case 0x10A5:
                        --c.ItemID;
                        break;
                }
            }

            if (callback != null)
                callback(this, from, hue);
        }

        private class SpinTimer : Timer
        {
            private readonly SpinningwheelSouthAddon m_Wheel;
            private readonly SpinCallback m_Callback;
            private readonly Mobile m_From;
            private readonly int m_Hue;
            public SpinTimer(SpinningwheelSouthAddon wheel, SpinCallback callback, Mobile from, int hue)
                : base(TimeSpan.FromSeconds(3.0))
            {
                this.m_Wheel = wheel;
                this.m_Callback = callback;
                this.m_From = from;
                this.m_Hue = hue;
                this.Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                this.m_Wheel.EndSpin(this.m_Callback, this.m_From, this.m_Hue);
            }
        }
    }

    public class SpinningwheelSouthDeed : BaseAddonDeed
    {
        [Constructable]
        public SpinningwheelSouthDeed()
        {
        }

        public SpinningwheelSouthDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon
        {
            get
            {
                return new SpinningwheelSouthAddon();
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1044342;
            }
        }// spining wheel (south)
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}