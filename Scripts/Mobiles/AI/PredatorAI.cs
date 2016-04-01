using System;

/*
* PredatorAI, its an animal that can attack
*	Dont flee but dont attack if not hurt or attacked
* 
*/
namespace Server.Mobiles
{
    public class PredatorAI : BaseAI
    {
        public PredatorAI(BaseCreature m)
            : base(m)
        {
        }

        public override bool DoActionWander()
        {
            if (this.m_Mobile.Combatant != null)
            {
                this.m_Mobile.DebugSay("I am hurt or being attacked, I kill him");						
                this.Action = ActionType.Combat;
            }
            else if (this.AcquireFocusMob(this.m_Mobile.RangePerception, this.m_Mobile.FightMode, true, false, true))
            {
                this.m_Mobile.DebugSay("There is something near, I go away");
                this.Action = ActionType.Backoff;
            }
            else
            {
                base.DoActionWander();
            }

            return true;
        }

        public override bool DoActionCombat()
        {
            Mobile combatant = this.m_Mobile.Combatant;

            if (combatant == null || combatant.Deleted || combatant.Map != this.m_Mobile.Map)
            {
                this.m_Mobile.DebugSay("My combatant is gone, so my guard is up");
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
                    this.m_Mobile.DebugSay("I cannot find {0}", combatant.Name);

                    this.Action = ActionType.Wander;
                    return true;
                }
                else
                {
                    this.m_Mobile.DebugSay("I should be closer to {0}", combatant.Name);
                }
            }

            return true;
        }

        public override bool DoActionBackoff()
        {
            if (this.m_Mobile.IsHurt() || this.m_Mobile.Combatant != null)
            {
                this.Action = ActionType.Combat;
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
    }
}