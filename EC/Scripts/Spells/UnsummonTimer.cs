using System;
using Server.Mobiles;

namespace Server.Spells
{
    class UnsummonTimer : Timer
    {
        private readonly BaseCreature m_Creature;
        private readonly Mobile m_Caster;
        public UnsummonTimer(Mobile caster, BaseCreature creature, TimeSpan delay)
            : base(delay)
        {
            this.m_Caster = caster;
            this.m_Creature = creature;
            this.Priority = TimerPriority.OneSecond;
        }

        protected override void OnTick()
        {
            if (!this.m_Creature.Deleted)
                this.m_Creature.Delete();
        }
    }
}