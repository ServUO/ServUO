using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class AcidSlime : Item
    {
        private readonly TimeSpan m_Duration;
        private readonly int m_MinDamage;
        private readonly int m_MaxDamage;
        private readonly DateTime m_Created;
        private readonly Timer m_Timer;
        private bool m_Drying;
		
		public override int LabelNumber {get {return 1018143;} } // slime
		
        [Constructable]
        public AcidSlime()
            : this(TimeSpan.FromSeconds(10.0), 5, 10)
        {
        }

        [Constructable]
        public AcidSlime(TimeSpan duration, int minDamage, int maxDamage)
            : base(0x122A)
        {
            Hue = 0x3F;
            Movable = false;
            m_MinDamage = minDamage;
            m_MaxDamage = maxDamage;
            m_Created = DateTime.UtcNow;
            m_Duration = duration;
            m_Timer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(1), new TimerCallback(OnTick));
        }

        public AcidSlime(Serial serial)
            : base(serial)
        {
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
            int damage = Utility.RandomMinMax(this.m_MinDamage, this.m_MaxDamage);
            AOS.Damage(m, damage, 0, 0, 0, 100, 0);
        }

        public override void Serialize(GenericWriter writer)
        {
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
                    this.Damage(toDamage[i]);
            }
        }
    }
}
