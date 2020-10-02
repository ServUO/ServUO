using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class FireBarrier : Item
    {
        private static readonly string _TimerID = "FireBarrierTimer";

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
                    {
                        ActivateTimer();
                    }
                    else
                    {
                        DeactivateTimer();
                    }
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
        }

        protected void PlaySound()
        {
            if (Active)
            {
                Effects.PlaySound(Location, Map, Utility.RandomList(0x1DD, 0x345, 0x346, 0x347, 0x348, 0x349, 0x34A));
                TimerRegistry.UpdateRegistry(_TimerID, this, TimeSpan.FromSeconds(Utility.RandomMinMax(5, 25)));
            }
            else
            {
                TimerRegistry.RemoveFromRegistry(_TimerID, this);
            }
        }

        private readonly Dictionary<Mobile, Timer> m_DamageTable = new Dictionary<Mobile, Timer>();

        public override bool HandlesOnMovement => true;

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (!Active || !m.Alive || !m.IsPlayer())
                return;

            if (m.InRange(this, 1) && !m_DamageTable.ContainsKey(m))
            {
                // Should start the timer
                m_DamageTable[m] = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(5.0), Damage, m);
            }
        }

        private void ActivateTimer()
        {
            TimerRegistry.Register(_TimerID, this, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), false, barrier => barrier.PlaySound());
        }

        private void DeactivateTimer()
        {
            TimerRegistry.RemoveFromRegistry(_TimerID, this);
        }

        public override void OnSectorActivate()
        {
            ActivateTimer();
        }

        public override void OnSectorDeactivate()
        {
            DeactivateTimer();
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

        public FireBarrier(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
