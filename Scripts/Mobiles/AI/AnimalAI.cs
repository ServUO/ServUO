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
            double hitPercent = (double)this.m_Mobile.Hits / this.m_Mobile.HitsMax;

            if (!this.m_Mobile.Summoned && !this.m_Mobile.Controlled && hitPercent < 0.1 && this.m_Mobile.CanFlee) // Less than 10% health
            {
                this.m_Mobile.DebugSay("I am low on health!");
                this.Action = ActionType.Flee;
            }
            else if (this.AcquireFocusMob(this.m_Mobile.RangePerception, this.m_Mobile.FightMode, false, false, true))
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.DebugSay("I have detected {0}, attacking", this.m_Mobile.FocusMob.Name);

                this.m_Mobile.Combatant = this.m_Mobile.FocusMob;
                this.Action = ActionType.Combat;
            }
            else
            {
                base.DoActionWander();
            }

            return true;
        }

        public override bool DoActionCombat()
        {
            IDamageable combatant = this.m_Mobile.Combatant;

            if (combatant == null || combatant.Deleted || combatant.Map != this.m_Mobile.Map)
            {
                this.m_Mobile.DebugSay("My combatant is gone..");

                this.Action = ActionType.Wander;

                return true;
            }

            if (this.WalkMobileRange(combatant, 1, true, this.m_Mobile.RangeFight, this.m_Mobile.RangeFight))
            {
                this.m_Mobile.Direction = this.m_Mobile.GetDirectionTo(combatant);
            }
            else
            {
                if (this.m_Mobile.GetDistanceToSqrt(combatant) > this.m_Mobile.RangePerception + 1)
                {
                    if (this.m_Mobile.Debug)
                        this.m_Mobile.DebugSay("I cannot find {0}", combatant.Name);

                    this.Action = ActionType.Wander;

                    return true;
                }
                else
                {
                    if (this.m_Mobile.Debug)
                        this.m_Mobile.DebugSay("I should be closer to {0}", combatant.Name);
                }
            }

            if (!this.m_Mobile.Controlled && !this.m_Mobile.Summoned && this.m_Mobile.CanFlee)
            {
                double hitPercent = (double)this.m_Mobile.Hits / this.m_Mobile.HitsMax;

                if (hitPercent < 0.1)
                {
                    this.m_Mobile.DebugSay("I am low on health!");
                    this.Action = ActionType.Flee;
                }
            }

            return true;
        }

        public override bool DoActionBackoff()
        {
            double hitPercent = (double)this.m_Mobile.Hits / this.m_Mobile.HitsMax;

            if (!this.m_Mobile.Summoned && !this.m_Mobile.Controlled && hitPercent < 0.1 && this.m_Mobile.CanFlee) // Less than 10% health
            {
                this.Action = ActionType.Flee;
            }
            else
            {
                if (this.AcquireFocusMob(this.m_Mobile.RangePerception * 2, FightMode.Closest, true, false, true))
                {
                    if (this.WalkMobileRange(this.m_Mobile.FocusMob, 1, false, this.m_Mobile.RangePerception, this.m_Mobile.RangePerception * 2))
                    {
                        this.m_Mobile.DebugSay("Well, here I am safe");
                        this.Action = ActionType.Wander;
                    }
                }
                else
                {
                    this.m_Mobile.DebugSay("I have lost my focus, lets relax");
                    this.Action = ActionType.Wander;
                }
            }

            return true;
        }

        public override bool DoActionFlee()
        {
            this.AcquireFocusMob(this.m_Mobile.RangePerception * 2, this.m_Mobile.FightMode, true, false, true);

            if (this.m_Mobile.FocusMob == null)
                this.m_Mobile.FocusMob = this.m_Mobile.Combatant as Mobile;

            return base.DoActionFlee();
        }
    }
}