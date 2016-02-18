using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class HotGoo : Item
    {
        private bool m_Drying;
        private readonly DateTime m_Created;
        private readonly TimeSpan m_Duration;
        private readonly int m_MaxDamage;
        private readonly int m_MinDamage;
        private readonly Timer m_Timer;

        [Constructable]
        public HotGoo() : this(TimeSpan.FromSeconds(10.0), 5, 10)
        {
        }

        [Constructable]
        public HotGoo(TimeSpan duration, int minDamage, int maxDamage)
            : base(0x122A)
        {
            Hue = 1932;
            Movable = false;
            m_MinDamage = minDamage;
            m_MaxDamage = maxDamage;
            m_Created = DateTime.Now;
            m_Duration = duration;
            m_Timer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(1), OnTick);
        }

        public HotGoo(Serial serial) : base(serial)
        {
        }

        public override string DefaultName
        {
            get { return "hot goo"; }
        }

        public override void OnAfterDelete()
        {
            if (m_Timer != null)
                m_Timer.Stop();
        }

        private void OnTick()
        {
            var now = DateTime.Now;
            var age = now - m_Created;

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

                var toDamage = new List<Mobile>();

                foreach (var m in GetMobilesInRange(0))
                {
                    var bc = m as BaseCreature;
                    if (m.Alive && !m.IsDeadBondedPet && (bc == null || bc.Controlled || bc.Summoned))
                    {
                        toDamage.Add(m);
                    }
                }

                for (var i = 0; i < toDamage.Count; i++)
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
            var damage = Utility.RandomMinMax(m_MinDamage, m_MaxDamage);
            if (Core.AOS)
                AOS.Damage(m, damage, 0, 0, 0, 100, 0);
            else
                m.Damage(damage);
        }

        public override void Serialize(GenericWriter writer)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
        }
    }
}