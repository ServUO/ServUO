using System;

//
// This is a first simple AI
//
//
namespace Server.Mobiles
{
    public class MeleeAI : BaseAI
    {
        public MeleeAI(BaseCreature m)
            : base(m)
        {
        }

        public override bool DoActionWander()
        {
            this.m_Mobile.DebugSay("I have no combatant");

            if (this.AcquireFocusMob(this.m_Mobile.RangePerception, this.m_Mobile.FightMode, false, false, true))
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

            if (combatant == null || combatant.Deleted || combatant.Map != this.m_Mobile.Map || !combatant.Alive || (combatant is Mobile && ((Mobile)combatant).IsDeadBondedPet))
            {
                this.m_Mobile.DebugSay("My combatant is gone, so my guard is up");

                this.Action = ActionType.Guard;

                return true;
            }

            if (!this.m_Mobile.InRange(combatant, this.m_Mobile.RangePerception))
            {
                // They are somewhat far away, can we find something else?
                if (this.AcquireFocusMob(this.m_Mobile.RangePerception, this.m_Mobile.FightMode, false, false, true))
                {
                    this.m_Mobile.Combatant = this.m_Mobile.FocusMob;
                    this.m_Mobile.FocusMob = null;
                }
                else if (!this.m_Mobile.InRange(combatant, this.m_Mobile.RangePerception * 3))
                {
                    this.m_Mobile.Combatant = null;
                }

                combatant = this.m_Mobile.Combatant;

                if (combatant == null)
                {
                    this.m_Mobile.DebugSay("My combatant has fled, so I am on guard");
                    this.Action = ActionType.Guard;

                    return true;
                }
            }

            if (this.MoveTo(combatant, true, this.m_Mobile.RangeFight))
            {
                this.m_Mobile.Direction = this.m_Mobile.GetDirectionTo(combatant);
            }
            else if (this.AcquireFocusMob(this.m_Mobile.RangePerception, this.m_Mobile.FightMode, false, false, true))
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.DebugSay("My move is blocked, so I am going to attack {0}", this.m_Mobile.FocusMob.Name);

                this.m_Mobile.Combatant = this.m_Mobile.FocusMob;
                this.Action = ActionType.Combat;

                return true;
            }
            else if (this.m_Mobile.GetDistanceToSqrt(combatant) > this.m_Mobile.RangePerception + 1)
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.DebugSay("I cannot find {0}, so my guard is up", combatant.Name);

                this.Action = ActionType.Guard;

                return true;
            }
            else
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.DebugSay("I should be closer to {0}", combatant.Name);
            }

            if (!this.m_Mobile.Controlled && !this.m_Mobile.Summoned && this.m_Mobile.CanFlee)
            {
                if (this.m_Mobile.Hits < this.m_Mobile.HitsMax * 20 / 100)
                {
                    // We are low on health, should we flee?
                    bool flee = false;

                    if (this.m_Mobile.Hits < combatant.Hits)
                    {
                        // We are more hurt than them
                        int diff = combatant.Hits - this.m_Mobile.Hits;

                        flee = (Utility.Random(0, 100) < (10 + diff)); // (10 + diff)% chance to flee
                    }
                    else
                    {
                        flee = Utility.Random(0, 100) < 10; // 10% chance to flee
                    }

                    if (flee)
                    {
                        if (this.m_Mobile.Debug)
                            this.m_Mobile.DebugSay("I am going to flee from {0}", combatant.Name);

                        this.Action = ActionType.Flee;
                    }
                }
            }

            return true;
        }

        public override bool DoActionGuard()
        {
            if (this.AcquireFocusMob(this.m_Mobile.RangePerception, this.m_Mobile.FightMode, false, false, true))
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

        public override bool DoActionFlee()
        {
            if (this.m_Mobile.Hits > this.m_Mobile.HitsMax / 2)
            {
                this.m_Mobile.DebugSay("I am stronger now, so I will continue fighting");
                this.Action = ActionType.Combat;
            }
            else
            {
                this.m_Mobile.FocusMob = this.m_Mobile.Combatant as Mobile;
                base.DoActionFlee();
            }

            return true;
        }
    }
}