using System;

namespace Server.Mobiles
{
    public class BerserkAI : BaseAI
    {
        public BerserkAI(BaseCreature m)
            : base(m)
        {
        }

        public override bool DoActionWander()
        {
            this.m_Mobile.DebugSay("I have No Combatant");
			
            if (this.AcquireFocusMob(this.m_Mobile.RangePerception, FightMode.Closest, false, true, true))
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.DebugSay("I have detected " + this.m_Mobile.FocusMob.Name + " and I will attack");

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
            if (this.m_Mobile.Combatant == null || this.m_Mobile.Combatant.Deleted)
            {
                this.m_Mobile.DebugSay("My combatant is deleted");
                this.Action = ActionType.Guard;
                return true;
            }

            if (this.WalkMobileRange(this.m_Mobile.Combatant, 1, true, this.m_Mobile.RangeFight, this.m_Mobile.RangeFight))
            {
                // Be sure to face the combatant
                this.m_Mobile.Direction = this.m_Mobile.GetDirectionTo(this.m_Mobile.Combatant.Location);
            }
            else
            {
                if (this.m_Mobile.Combatant != null)
                {
                    if (this.m_Mobile.Debug)
                        this.m_Mobile.DebugSay("I am still not in range of " + this.m_Mobile.Combatant.Name);

                    if ((int)this.m_Mobile.GetDistanceToSqrt(this.m_Mobile.Combatant) > this.m_Mobile.RangePerception + 1)
                    {
                        if (this.m_Mobile.Debug)
                            this.m_Mobile.DebugSay("I have lost " + this.m_Mobile.Combatant.Name);

                        this.Action = ActionType.Guard;
                        return true;
                    }
                }
            }
			
            return true;
        }

        public override bool DoActionGuard()
        {
            if (this.AcquireFocusMob(this.m_Mobile.RangePerception, this.m_Mobile.FightMode, false, true, true))
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.DebugSay("I have detected {0}, attacking", this.m_Mobile.FocusMob.Name);

                this.m_Mobile.Combatant = this.m_Mobile.FocusMob;
                this.Action = ActionType.Combat;
            }
            else
            {
                base.DoActionGuard();
            }

            return true;
        }
    }
}