using Server.Mobiles;
using System;

namespace Server.Spells
{
    class UnsummonTimer : Timer
    {
        private readonly BaseCreature m_Creature;
        private readonly Mobile m_Caster;
        public UnsummonTimer(Mobile caster, BaseCreature creature, TimeSpan delay)
            : base(delay)
        {
            m_Caster = caster;
            m_Creature = creature;
            Priority = TimerPriority.OneSecond;
        }

        protected override void OnTick()
        {
            if (!m_Creature.Deleted)
                m_Creature.Delete();
        }
    }
}