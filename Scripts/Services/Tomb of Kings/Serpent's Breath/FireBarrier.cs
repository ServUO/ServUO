using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class FireBarrier : Item
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

        public FireBarrier(Point3D location, Map map)
            : base(0x3E27)
        {
            Movable = false;
            Light = LightType.Circle150;
            Hue = 0x47E;

            MoveToWorld(location, map);

            m_SoundTimer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(Utility.RandomMinMax(5, 50)), new TimerCallback(PlaySound));
            m_SoundTimer.Start();
        }

        protected void PlaySound()
        {
            // Randomize the Timer Interval
            m_SoundTimer.Interval = TimeSpan.FromSeconds(Utility.RandomMinMax(5, 50));

            //Effects.PlaySound( Location, Map, Utility.RandomList( 0x1DC, 0x210, 0x2F4 ) );
            Effects.PlaySound(Location, Map, Utility.RandomList(0x1DD, 0x345, 0x346, 0x347, 0x348, 0x349, 0x34A));
        }

        private Dictionary<Mobile, Timer> m_DamageTable = new Dictionary<Mobile, Timer>();

        public override bool HandlesOnMovement { get { return true; } }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (!Active || !m.Alive || !m.IsPlayer())
                return;

            if (m.InRange(this, 1) && !m_DamageTable.ContainsKey(m))
            {
                // Should start the timer
                m_DamageTable[m] = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(5.0), new TimerStateCallback<Mobile>(Damage), m);
            }
        }

        protected void Damage(Mobile m)
        {
            if (Active && m.IsPlayer() && m.Alive && !Deleted && m.InRange(this, 1))
            {
                AOS.Damage(m, Utility.RandomMinMax(5, 7), 0, 100, 0, 0, 0);
            }
            else
            {
                Timer t = m_DamageTable[m];
                t.Stop();

                m_DamageTable.Remove(m);
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_SoundTimer != null)
                m_SoundTimer.Stop();
        }

        public FireBarrier(Serial serial)
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