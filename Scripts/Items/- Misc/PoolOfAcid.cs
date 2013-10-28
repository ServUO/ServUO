using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class PoolOfAcid : Item
    {
        private readonly TimeSpan m_Duration;
        private readonly int m_MinDamage;
        private readonly int m_MaxDamage;
        private readonly DateTime m_Created;
        private readonly Timer m_Timer;
        private bool m_Drying;
        [Constructable]
        public PoolOfAcid()
            : this(TimeSpan.FromSeconds(10.0), 2, 5)
        {
        }

        [Constructable]
        public PoolOfAcid(TimeSpan duration, int minDamage, int maxDamage)
            : base(0x122A)
        {
            this.Hue = 0x3F;
            this.Movable = false;

            this.m_MinDamage = minDamage;
            this.m_MaxDamage = maxDamage;
            this.m_Created = DateTime.UtcNow;
            this.m_Duration = duration;

            this.m_Timer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(1), new TimerCallback(OnTick));
        }

        public PoolOfAcid(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "a pool of acid";
            }
        }
        public override void OnAfterDelete()
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();
        }

        public override bool OnMoveOver(Mobile m)
        {
            this.Damage(m);
            return true;
        }

        public void Damage(Mobile m)
        {
            m.Damage(Utility.RandomMinMax(this.m_MinDamage, this.m_MaxDamage));
        }

        public override void Serialize(GenericWriter writer)
        {
            //Don't serialize these
        }

        public override void Deserialize(GenericReader reader)
        {
        }

        private void OnTick()
        {
            DateTime now = DateTime.UtcNow;
            TimeSpan age = now - this.m_Created;

            if (age > this.m_Duration)
            {
                this.Delete();
            }
            else
            {
                if (!this.m_Drying && age > (this.m_Duration - age))
                {
                    this.m_Drying = true;
                    this.ItemID = 0x122B;
                }

                List<Mobile> toDamage = new List<Mobile>();

                foreach (Mobile m in this.GetMobilesInRange(0))
                {
                    BaseCreature bc = m as BaseCreature;

                    if (m.Alive && !m.IsDeadBondedPet && (bc == null || bc.Controlled || bc.Summoned))
                    {
                        toDamage.Add(m);
                    }
                }

                for (int i = 0; i < toDamage.Count; i++)
                    this.Damage(toDamage[i]);
            }
        }
    }
}