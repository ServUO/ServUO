using System;

namespace Server.Items
{
    public class EnergyBarrier : Item
    {
        private Timer m_SoundTimer;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get { return Visible; }
            set
            {
                if (Active != value)
                {
                    Visible = value;

                    if (Active)
                        m_SoundTimer.Start();
                    else
                        m_SoundTimer.Stop();
                }
            }
        }

        public EnergyBarrier(Point3D location, Map map)
            : base(0x3967)
        {
            Movable = false;
            Light = LightType.Circle150;

            MoveToWorld(location, map);

            m_SoundTimer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(Utility.RandomMinMax(5, 50)), new TimerCallback(PlaySound));
            m_SoundTimer.Start();
        }

        protected void PlaySound()
        {
            // Randomize the Timer Interval
            m_SoundTimer.Interval = TimeSpan.FromSeconds(Utility.RandomMinMax(5, 50));

            Effects.PlaySound(Location, Map, Utility.RandomList(0x1DC, 0x210, 0x2F4));
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_SoundTimer != null)
                m_SoundTimer.Stop();
        }

        public EnergyBarrier(Serial serial)
            : base(serial)
        {
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

            m_SoundTimer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(5.0), new TimerCallback(PlaySound));
            m_SoundTimer.Start();
        }
    }
}