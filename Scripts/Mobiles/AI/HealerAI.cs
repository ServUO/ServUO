using System;
using Server.Spells.First;
using Server.Spells.Fourth;
using Server.Spells.Second;
using Server.Targeting;

namespace Server.Mobiles
{
    public class HealerAI : BaseAI
    {
        private static readonly NeedDelegate m_Cure = new NeedDelegate(NeedCure);
        private static readonly NeedDelegate m_GHeal = new NeedDelegate(NeedGHeal);
        private static readonly NeedDelegate m_LHeal = new NeedDelegate(NeedLHeal);
        private static readonly NeedDelegate[] m_ACure = new NeedDelegate[] { m_Cure };
        private static readonly NeedDelegate[] m_AGHeal = new NeedDelegate[] { m_GHeal };
        private static readonly NeedDelegate[] m_ALHeal = new NeedDelegate[] { m_LHeal };
        private static readonly NeedDelegate[] m_All = new NeedDelegate[] { m_Cure, m_GHeal, m_LHeal };
        public HealerAI(BaseCreature m)
            : base(m)
        {
        }

        private delegate bool NeedDelegate(Mobile m);
        public override bool Think()
        {
            if (this.m_Mobile.Deleted)
                return false;

            Target targ = this.m_Mobile.Target;

            if (targ != null)
            {
                if (targ is CureSpell.InternalTarget)
                {
                    this.ProcessTarget(targ, m_ACure);
                }
                else if (targ is GreaterHealSpell.InternalTarget)
                {
                    this.ProcessTarget(targ, m_AGHeal);
                }
                else if (targ is HealSpell.InternalTarget)
                {
                    this.ProcessTarget(targ, m_ALHeal);
                }
                else
                {
                    targ.Cancel(this.m_Mobile, TargetCancelType.Canceled);
                }
            }
            else
            {
                Mobile toHelp = this.Find(m_All);

                if (toHelp != null)
                {
                    if (NeedCure(toHelp))
                    {
                        if (this.m_Mobile.Debug)
                            this.m_Mobile.DebugSay("{0} needs a cure", toHelp.Name);

                        if (!(new CureSpell(this.m_Mobile, null)).Cast())
                            new CureSpell(this.m_Mobile, null).Cast();
                    }
                    else if (NeedGHeal(toHelp))
                    {
                        if (this.m_Mobile.Debug)
                            this.m_Mobile.DebugSay("{0} needs a greater heal", toHelp.Name);

                        if (!(new GreaterHealSpell(this.m_Mobile, null)).Cast())
                            new HealSpell(this.m_Mobile, null).Cast();
                    }
                    else if (NeedLHeal(toHelp))
                    {
                        if (this.m_Mobile.Debug)
                            this.m_Mobile.DebugSay("{0} needs a lesser heal", toHelp.Name);

                        new HealSpell(this.m_Mobile, null).Cast();
                    }
                }
                else
                {
                    if (this.AcquireFocusMob(this.m_Mobile.RangePerception, FightMode.Weakest, false, true, false))
                    {
                        this.WalkMobileRange(this.m_Mobile.FocusMob, 1, false, 4, 7);
                    }
                    else
                    {
                        this.WalkRandomInHome(3, 2, 1);
                    }
                }
            }

            return true;
        }

        private static bool NeedCure(Mobile m)
        {
            return m.Poisoned;
        }

        private static bool NeedGHeal(Mobile m)
        {
            return m.Hits < m.HitsMax - 40;
        }

        private static bool NeedLHeal(Mobile m)
        {
            return m.Hits < m.HitsMax - 10;
        }

        private void ProcessTarget(Target targ, NeedDelegate[] func)
        {
            Mobile toHelp = this.Find(func);

            if (toHelp != null)
            {
                if (targ.Range != -1 && !this.m_Mobile.InRange(toHelp, targ.Range))
                {
                    this.DoMove(this.m_Mobile.GetDirectionTo(toHelp) | Direction.Running);
                }
                else
                {
                    targ.Invoke(this.m_Mobile, toHelp);
                }
            }
            else
            {
                targ.Cancel(this.m_Mobile, TargetCancelType.Canceled);
            }
        }

        private Mobile Find(params NeedDelegate[] funcs)
        {
            if (this.m_Mobile.Deleted)
                return null;

            Map map = this.m_Mobile.Map;

            if (map != null)
            {
                double prio = 0.0;
                Mobile found = null;

                foreach (Mobile m in this.m_Mobile.GetMobilesInRange(this.m_Mobile.RangePerception))
                {
                    if (!this.m_Mobile.CanSee(m) || !(m is BaseCreature) || ((BaseCreature)m).Team != this.m_Mobile.Team)
                        continue;

                    for (int i = 0; i < funcs.Length; ++i)
                    {
                        if (funcs[i](m))
                        {
                            double val = -this.m_Mobile.GetDistanceToSqrt(m);

                            if (found == null || val > prio)
                            {
                                prio = val;
                                found = m;
                            }

                            break;
                        }
                    }
                }

                return found;
            }

            return null;
        }
    }
}