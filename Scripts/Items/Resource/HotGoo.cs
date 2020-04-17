using Server.Mobiles;
using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class ExplosiveGoo : Item
    {
        public override int LabelNumber => 1157463;  // Explosive Goo

        private bool m_Drying;
        private readonly DateTime m_Created;
        private readonly TimeSpan m_Duration;
        private readonly int m_MaxDamage;
        private readonly int m_MinDamage;
        private readonly Timer m_Timer;

        [Constructable]
        public ExplosiveGoo() : this(TimeSpan.FromSeconds(10.0), 5, 10)
        {
        }

        [Constructable]
        public ExplosiveGoo(TimeSpan duration, int minDamage, int maxDamage)
            : base(0x122A)
        {
            Hue = 1932;
            Movable = false;
            m_MinDamage = minDamage;
            m_MaxDamage = maxDamage;
            m_Created = DateTime.UtcNow;
            m_Duration = duration;
            m_Timer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(1), OnTick);
        }

        public ExplosiveGoo(Serial serial)
            : base(serial)
        {
        }

        public override void OnAfterDelete()
        {
            if (m_Timer != null)
                m_Timer.Stop();
        }

        private void OnTick()
        {
            DateTime now = DateTime.UtcNow;
            TimeSpan age = now - m_Created;

            if (age > m_Duration)
            {
                Delete();
            }
            else
            {
                if (!m_Drying && age > (m_Duration - age))
                {
                    m_Drying = true;
                    ItemID = 0x122B;
                }

                List<Mobile> toDamage = new List<Mobile>();
                IPooledEnumerable eable = GetMobilesInRange(0);

                foreach (Mobile m in eable)
                {
                    BaseCreature bc = m as BaseCreature;
                    if (m.Alive && !m.IsDeadBondedPet && (bc == null || bc.Controlled || bc.Summoned))
                    {
                        toDamage.Add(m);
                    }
                }
                eable.Free();

                for (int i = 0; i < toDamage.Count; i++)
                    Damage(toDamage[i]);
            }
        }

        public override bool OnMoveOver(Mobile m)
        {
            Damage(m);
            return true;
        }

        public void Damage(Mobile m)
        {
            m.SendLocalizedMessage(1112366); // The flammable goo covering you bursts into flame!

            AOS.Damage(m, Utility.RandomMinMax(m_MinDamage, m_MaxDamage), 0, 100, 0, 0, 0);
        }

        public override void Serialize(GenericWriter writer)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
        }
    }
}