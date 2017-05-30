using System;

// Ideas
// When you run on animals the panic
// When if ( distance < 8 && Utility.RandomDouble() * Math.Sqrt( (8 - distance) / 6 ) >= incoming.Skills[SkillName.AnimalTaming].Value )
// More your close, the more it can panic
/*
* AnimalHunterAI, AnimalHidingAI, AnimalDomesticAI...
*
*/

namespace Server.Mobiles
{
    public class AnimalAI : BaseAI
    {
        public AnimalAI(BaseCreature m)
            : base(m)
        {
        }

        public override bool DoActionWander()
        {
            // Old:
            #if false
			if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, true, false, true))
			{
				m_Mobile.DebugSay( "There is something near, I go away" );
				Action = ActionType.Backoff;
			}
			else if ( m_Mobile.IsHurt() || m_Mobile.Combatant != null )
			{
				m_Mobile.DebugSay( "I am hurt or being attacked, I flee" );
				Action = ActionType.Flee;
			}
			else
			{
				base.DoActionWander();
			}
			return true;
            #endif
            // New, only flee @ 10%
            double hitPercent = (double)m_Mobile.Hits / m_Mobile.HitsMax;

            if (!m_Mobile.Summoned && !m_Mobile.Controlled && hitPercent < 0.1 && m_Mobile.CanFlee) // Less than 10% health
            {
                m_Mobile.DebugSay("I am low on health!");
                Action = ActionType.Flee;
            }
            else if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
            {
                m_Mobile.DebugSay("I have detected {0}, attacking", m_Mobile.FocusMob.Name);

                m_Mobile.Combatant = m_Mobile.FocusMob;
                Action = ActionType.Combat;
            }
            else
            {
                base.DoActionWander();
            }

            return true;
        }

        public override bool DoActionCombat()
        {
            IDamageable combatant = m_Mobile.Combatant;

            if (combatant == null || combatant.Deleted || combatant.Map != m_Mobile.Map)
            {
                m_Mobile.DebugSay("My combatant is gone..");

                Action = ActionType.Wander;

                return true;
            }

            if (WalkMobileRange(combatant, 1, true, m_Mobile.RangeFight, m_Mobile.RangeFight))
            {
                m_Mobile.Direction = m_Mobile.GetDirectionTo(combatant);
            }
            else
            {
                if (m_Mobile.GetDistanceToSqrt(combatant) > m_Mobile.RangePerception + 1)
                {
                    m_Mobile.DebugSay("I cannot find {0}", combatant.Name);

                    Action = ActionType.Wander;

                    return true;
                }
                else
                {
                    m_Mobile.DebugSay("I should be closer to {0}", combatant.Name);
                }
            }

            if (!m_Mobile.Controlled && !m_Mobile.Summoned && m_Mobile.CanFlee)
            {
                double hitPercent = (double)m_Mobile.Hits / m_Mobile.HitsMax;

                if (hitPercent < 0.1)
                {
                    m_Mobile.DebugSay("I am low on health!");
                    Action = ActionType.Flee;
                }
            }

            return true;
        }

        public override bool DoActionBackoff()
        {
            double hitPercent = (double)m_Mobile.Hits / m_Mobile.HitsMax;

            if (!m_Mobile.Summoned && !m_Mobile.Controlled && hitPercent < 0.1 && m_Mobile.CanFlee) // Less than 10% health
            {
                Action = ActionType.Flee;
            }
            else
            {
                if (AcquireFocusMob(m_Mobile.RangePerception * 2, FightMode.Closest, true, false, true))
                {
                    if (WalkMobileRange(m_Mobile.FocusMob, 1, false, m_Mobile.RangePerception, m_Mobile.RangePerception * 2))
                    {
                        m_Mobile.DebugSay("Well, here I am safe");
                        Action = ActionType.Wander;
                    }
                }
                else
                {
                    m_Mobile.DebugSay("I have lost my focus, lets relax");
                    Action = ActionType.Wander;
                }
            }

            return true;
        }
    }
}
