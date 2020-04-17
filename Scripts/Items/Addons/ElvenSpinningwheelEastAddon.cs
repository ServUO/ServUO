using System;

namespace Server.Items
{
    public class ElvenSpinningwheelEastAddon : BaseAddon, ISpinningWheel
    {
        private Timer m_Timer;
        [Constructable]
        public ElvenSpinningwheelEastAddon()
        {
            AddComponent(new AddonComponent(0x2E3D), 0, 0, 0);
        }

        public ElvenSpinningwheelEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new ElvenSpinningwheelEastDeed();
        public bool Spinning => m_Timer != null;
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

        public override void OnComponentLoaded(AddonComponent c)
        {
            switch (c.ItemID)
            {
                case 0x2E3C:
                    ++c.ItemID;
                    break;
            }
        }

        public void BeginSpin(SpinCallback callback, Mobile from, int hue)
        {
            m_Timer = new SpinTimer(this, callback, from, hue);
            m_Timer.Start();

            foreach (AddonComponent c in Components)
            {
                switch (c.ItemID)
                {
                    case 0x2E3D:
                        --c.ItemID;
                        break;
                }
            }
        }

        public void EndSpin(SpinCallback callback, Mobile from, int hue)
        {
            if (m_Timer != null)
                m_Timer.Stop();

            m_Timer = null;

            foreach (AddonComponent c in Components)
            {
                switch (c.ItemID)
                {
                    case 0x2E3C:
                        ++c.ItemID;
                        break;
                }
            }

            if (callback != null)
                callback(this, from, hue);
        }

        private class SpinTimer : Timer
        {
            private readonly ElvenSpinningwheelEastAddon m_Wheel;
            private readonly SpinCallback m_Callback;
            private readonly Mobile m_From;
            private readonly int m_Hue;
            public SpinTimer(ElvenSpinningwheelEastAddon wheel, SpinCallback callback, Mobile from, int hue)
                : base(TimeSpan.FromSeconds(6.0))
            {
                m_Wheel = wheel;
                m_Callback = callback;
                m_From = from;
                m_Hue = hue;
                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                m_Wheel.EndSpin(m_Callback, m_From, m_Hue);
            }
        }
    }

    public class ElvenSpinningwheelEastDeed : BaseAddonDeed
    {
        [Constructable]
        public ElvenSpinningwheelEastDeed()
        {
        }

        public ElvenSpinningwheelEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new ElvenSpinningwheelEastAddon();
        public override int LabelNumber => 1073393;// elven spinning wheel (east)
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
