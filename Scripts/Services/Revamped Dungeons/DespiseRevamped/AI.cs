using System;
using System.Collections;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Spells;

namespace Server.Engines.Despise
{
    public class DespiseMeleeAI : MeleeAI
    {
        private DespiseCreature m_Creature;

        public DespiseMeleeAI(DespiseCreature m) : base(m)
        {
            m_Creature = m;
        }

        public override bool DoOrderAttack()
        {
            if (m_Creature.IsDeadPet)
                return true;

            if (m_Creature.Orb == null)
                return base.DoOrderAttack();

            if (m_Creature.ControlTarget == null || m_Creature.ControlTarget.Deleted || m_Creature.ControlTarget.Map != m_Creature.Map || !m_Creature.ControlTarget.Alive || (m_Creature.ControlTarget is Mobile && ((Mobile)m_Creature.ControlTarget).IsDeadBondedPet))
            {
                m_Creature.DebugSay("I think he might be dead. He's not anywhere around here at least. That's cool. I'm glad he's dead.");

                if (m_Creature.Orb.Aggression == Aggression.Following)
                {
                    m_Creature.ControlTarget = m_Creature.ControlMaster;
                    m_Creature.ControlOrder = OrderType.Follow;

                    m_Creature.CurrentSpeed = 0.1;
                }
                else if (m_Creature.Orb.Anchor is Mobile && ((Mobile)m_Creature.Orb.Anchor).Alive)
                {
                    m_Creature.ControlTarget = (Mobile)m_Creature.Orb.Anchor;
                    m_Creature.ControlOrder = OrderType.Follow;

                    m_Creature.CurrentSpeed = 0.1;
                }
                else
                {
                    m_Creature.ControlTarget = null;
                    m_Creature.ControlOrder = OrderType.None;

                    m_Creature.CurrentSpeed = m_Creature.Orb.Anchor == null ? m_Creature.PassiveSpeed : m_Creature.ActiveSpeed;
                }

                Think();
            }
            else
            {
                m_Creature.DebugSay("Attacking target...");
                Think();
            }

            return true;
        }

        public override bool DoOrderFollow()
        {
            if (m_Creature.Orb == null || !m_Creature.Controlled)
                return base.DoOrderFollow();

            bool fighting = false;

            switch (m_Creature.Orb.Aggression)
            {
                case Aggression.Following:
                    return base.DoOrderFollow();
                case Aggression.Defensive:
                    {
                        int length = m_Creature.GetLeashLength();
                        IPoint3D p = m_Creature.Orb.Anchor as IPoint3D;

                        if (p == null)
                            p = m_Creature;

                        Mobile combatant = m_Creature.Combatant != null ? m_Creature.Combatant as Mobile : m_Creature.ControlMaster.Combatant != null ? m_Creature.ControlMaster.Combatant as Mobile : null;

                        if (combatant != null && combatant.GetDistanceToSqrt(p) <= length)
                        {
                            if (m_Creature.Debug)
                                m_Creature.DebugSay("I have detected {0} as an aggressor, attacking", m_Creature.FocusMob.Name);

                            fighting = true;

                            m_Creature.Combatant = combatant;
                            m_Creature.ControlTarget = combatant;
                            m_Creature.ControlOrder = OrderType.Attack;
                            Action = ActionType.Combat;
                        }
                        else
                            return base.DoOrderFollow();
                    }
                    break;
                case Aggression.Aggressive:
                    {
                        if (AcquireFocusMob(m_Creature.GetLeashLength(), m_Creature.FightMode, false, false, true))
                        {
                            if (m_Creature.Debug)
                                m_Creature.DebugSay("I have detected {0}, attacking", m_Creature.FocusMob.Name);

                            fighting = true;

                            m_Creature.Combatant = m_Creature.FocusMob;
                            m_Creature.ControlTarget = m_Creature.FocusMob;
                            m_Creature.ControlOrder = OrderType.Attack;
                            Action = ActionType.Combat;
                        }
                        else
                            return base.DoOrderFollow();
                    }
                    break;
            }

            if (!fighting)
                return base.DoOrderFollow();

            return true;
        }

        public override bool DoOrderNone()
        {
            if (m_Creature.Orb == null || !m_Creature.Controlled)
                return base.DoOrderNone();

            switch (m_Creature.Orb.Aggression)
            {
                case Aggression.Following:
                    return base.DoOrderNone();
                case Aggression.Defensive:
                    {
                        int length = m_Creature.GetLeashLength();
                        IPoint3D p = m_Creature.Orb.Anchor as IPoint3D;

                        if (p == null)
                            p = m_Creature;

                        Mobile combatant = m_Creature.Combatant != null ? m_Creature.Combatant as Mobile : m_Creature.ControlMaster != null ? m_Creature.ControlMaster.Combatant as Mobile : null;

                        if (combatant != null && combatant.GetDistanceToSqrt(p) <= length)
                        {
                            if (m_Creature.Debug)
                                m_Creature.DebugSay("I have detected {0} as an aggressor, attacking", m_Creature.FocusMob.Name);

                            m_Creature.Combatant = combatant;
                            m_Creature.ControlTarget = combatant;
                            m_Creature.ControlOrder = OrderType.Attack;
                            Action = ActionType.Combat;
                        }
                        else
                            return base.DoOrderNone();
                    }
                    break;
                case Aggression.Aggressive:
                    {
                        if (AcquireFocusMob(m_Creature.GetLeashLength(), m_Creature.FightMode, false, false, true))
                        {
                            if (m_Creature.Debug)
                                m_Creature.DebugSay("I have detected {0}, attacking", m_Creature.FocusMob.Name);

                            m_Creature.Combatant = m_Creature.FocusMob;
                            m_Creature.ControlTarget = m_Creature.FocusMob;
                            m_Creature.ControlOrder = OrderType.Attack;
                            Action = ActionType.Combat;
                        }
                        else
                            return base.DoOrderNone();
                    }
                    break;
            }
            return true;
        }

        public override bool DoActionWander()
        {
            m_Creature.DebugSay("I have no combatant");

            if (m_Creature.Orb == null || !m_Creature.Controlled)
                return base.DoActionWander();

            switch (m_Creature.Orb.Aggression)
            {
                case Aggression.Following:
                    break;
                case Aggression.Defensive:
                    {
                        int length = m_Creature.GetLeashLength();
                        IPoint3D p = m_Creature.Orb.Anchor as IPoint3D;

                        if (p == null)
                            p = m_Creature;

                        Mobile combatant = m_Creature.Combatant != null ? m_Creature.Combatant as Mobile : m_Creature.ControlMaster.Combatant != null ? m_Creature.ControlMaster.Combatant as Mobile : null;

                        if (combatant != null && combatant.GetDistanceToSqrt(p) <= length)
                        {
                            if (m_Creature.Debug)
                                m_Creature.DebugSay("I have detected {0} as an aggressor, attacking", m_Creature.FocusMob.Name);

                            m_Creature.Combatant = m_Creature.FocusMob;
                            m_Creature.ControlTarget = combatant;
                            m_Creature.ControlOrder = OrderType.Attack;
                            Action = ActionType.Combat;
                        }
                    }
                    break;
                case Aggression.Aggressive:
                    {
                        if (AcquireFocusMob(m_Creature.GetLeashLength(), m_Creature.FightMode, false, false, true))
                        {
                            if (m_Creature.Debug)
                                m_Creature.DebugSay("I have detected {0}, attacking", m_Creature.FocusMob.Name);

                            m_Creature.Combatant = m_Creature.FocusMob;
                            m_Creature.ControlOrder = OrderType.Attack;
                            Action = ActionType.Combat;
                        }
                        else
                            return base.DoActionWander();
                    }
                    break;
            }
            return true;
        }

        public override bool AcquireFocusMob(int iRange, FightMode acqType, bool bPlayerOnly, bool bFacFriend, bool bFacFoe)
        {
            bool found = base.AcquireFocusMob(iRange, acqType, bPlayerOnly, bFacFriend, bFacFoe);

            if (m_Creature.Orb == null || m_Creature.ControlMaster == null)
                return found;

            if (!found && Core.TickCount - m_Creature.NextReacquireTime < 0)
            {
                m_Creature.FocusMob = null;
                return false;
            }

            m_Creature.NextReacquireTime = Core.TickCount + (int)m_Creature.ReacquireDelay.TotalMilliseconds;

            if (!found && m_Creature.Map != null && m_Creature.Map != Map.Internal)
            {
                switch (m_Creature.Orb.Aggression)
                {
                    case Aggression.Following:
                        break;
                    case Aggression.Defensive:
                        {
                            Mobile focus = m_Creature.Combatant as Mobile;

                            if (focus == null)
                                focus = m_Creature.ControlMaster.Combatant as Mobile;

                            if (focus != null)
                            {
                                m_Creature.FocusMob = focus;
                                found = true;
                            }
                        }
                        break;
                    case Aggression.Aggressive:
                        {
                            int range = m_Creature.GetLeashLength();
                            IPoint3D p = m_Creature.Orb.Anchor as IPoint3D;

                            if (p == null)
                                p = m_Creature;

                            Mobile focus = GetFocus(p, range);

                            if (focus != null)
                            {
                                m_Creature.FocusMob = focus;
                                found = true;
                            }
                        }
                        break;
                }
            }

            return found;
        }

        private Mobile GetFocus(IPoint3D p, int range)
        {
            IPooledEnumerable eable = m_Creature.Map.GetMobilesInRange(new Point3D(p), range);
            Mobile focus = null;
            int dist = range;

            foreach (Mobile m in eable)
            {
                if (m is DespiseCreature || m is DespiseBoss)
                {
                    DespiseCreature dc = m as DespiseCreature;

                    if(m is DespiseBoss || (dc != null && dc.Orb == null && !dc.Controlled))
                    {
                        int distance = (int)m_Creature.GetDistanceToSqrt(m);

                        if (focus == null || distance < dist)
                        {
                            focus = m;
                            dist = distance;
                        }
                    }
                }
            }

            eable.Free();

            return focus;
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
        }
    }

    public class DespiseMageAI : MageAI
    {
        private DespiseCreature m_Creature;

        public DespiseMageAI(DespiseCreature m) : base(m)
        {
            m_Creature = m;
        }

        public override bool DoOrderAttack()
        {
            if (m_Creature.IsDeadPet)
                return true;

            if (m_Creature.Orb == null)
                return base.DoOrderAttack();

            if (m_Creature.ControlTarget == null || m_Creature.ControlTarget.Deleted || m_Creature.ControlTarget.Map != m_Creature.Map || !m_Creature.ControlTarget.Alive || (m_Creature.ControlTarget is Mobile && ((Mobile)m_Creature.ControlTarget).IsDeadBondedPet))
            {
                m_Creature.DebugSay("I think he might be dead. He's not anywhere around here at least. That's cool. I'm glad he's dead.");

                if (m_Creature.Orb.Aggression == Aggression.Following)
                {
                    m_Creature.ControlTarget = m_Creature.ControlMaster;
                    m_Creature.ControlOrder = OrderType.Follow;

                    m_Creature.CurrentSpeed = 0.1;
                }
                else if (m_Creature.Orb.Anchor is Mobile && ((Mobile)m_Creature.Orb.Anchor).Alive)
                {
                    m_Creature.ControlTarget = (Mobile)m_Creature.Orb.Anchor;
                    m_Creature.ControlOrder = OrderType.Follow;

                    m_Creature.CurrentSpeed = 0.1;
                }
                else
                {
                    m_Creature.ControlTarget = null;
                    m_Creature.ControlOrder = OrderType.None;

                    m_Creature.CurrentSpeed = m_Creature.Orb.Anchor == null ? m_Creature.PassiveSpeed : m_Creature.ActiveSpeed;
                }

                Think();
            }
            else
            {
                m_Creature.DebugSay("Attacking target...");
                Think();
            }

            return true;
        }

        public override bool DoOrderFollow()
        {
            if (m_Creature.Orb == null || !m_Creature.Controlled)
                return base.DoOrderFollow();

            bool fighting = false;

            switch (m_Creature.Orb.Aggression)
            {
                case Aggression.Following:
                    return base.DoOrderFollow();
                case Aggression.Defensive:
                    {
                        int length = m_Creature.GetLeashLength();
                        IPoint3D p = m_Creature.Orb.Anchor as IPoint3D;

                        if (p == null)
                            p = m_Creature.ControlMaster;

                        Mobile combatant = m_Creature.Combatant != null ? m_Creature.Combatant as Mobile : m_Creature.ControlMaster.Combatant != null ? m_Creature.ControlMaster.Combatant as Mobile : null;

                        if (combatant != null && combatant.GetDistanceToSqrt(p) <= length)
                        {
                            if (m_Creature.Debug)
                                m_Creature.DebugSay("I have detected {0} as an aggressor, attacking", m_Creature.FocusMob.Name);

                            fighting = true;

                            m_Creature.Combatant = m_Creature.FocusMob;
                            m_Creature.ControlTarget = combatant;
                            m_Creature.ControlOrder = OrderType.Attack;
                            Action = ActionType.Combat;
                        }
                        else
                            return base.DoOrderFollow();
                    }
                    break;
                case Aggression.Aggressive:
                    {
                        if (AcquireFocusMob(m_Creature.GetLeashLength(), m_Creature.FightMode, false, false, true))
                        {
                            if (m_Creature.Debug)
                                m_Creature.DebugSay("I have detected {0}, attacking", m_Creature.FocusMob.Name);

                            fighting = true;

                            m_Creature.Combatant = m_Creature.FocusMob;
                            m_Creature.ControlTarget = m_Creature.FocusMob;
                            m_Creature.ControlOrder = OrderType.Attack;
                            Action = ActionType.Combat;
                        }
                        else
                            return base.DoOrderFollow();
                    }
                    break;
            }

            if (!fighting)
                return base.DoOrderFollow();

            return true;
        }

        public override bool DoOrderNone()
        {
            if (m_Creature.Orb == null || !m_Creature.Controlled)
                return base.DoOrderNone();

            switch (m_Creature.Orb.Aggression)
            {
                case Aggression.Following:
                    return base.DoOrderNone();
                case Aggression.Defensive:
                    {
                        int length = m_Creature.GetLeashLength();
                        IPoint3D p = m_Creature.Orb.Anchor as IPoint3D;

                        if (p == null)
                            p = m_Creature.ControlMaster;

                        Mobile combatant = m_Creature.Combatant != null ? m_Creature.Combatant as Mobile : m_Creature.ControlMaster != null ? m_Creature.ControlMaster.Combatant as Mobile : null;

                        if (combatant != null && combatant.GetDistanceToSqrt(p) <= length)
                        {
                            if (m_Creature.Debug)
                                m_Creature.DebugSay("I have detected {0} as an aggressor, attacking", m_Creature.FocusMob.Name);

                            m_Creature.Combatant = combatant;
                            m_Creature.ControlTarget = combatant;
                            m_Creature.ControlOrder = OrderType.Attack;
                            Action = ActionType.Combat;
                        }
                        else
                            return base.DoOrderNone();
                    }
                    break;
                case Aggression.Aggressive:
                    {
                        if (AcquireFocusMob(m_Creature.GetLeashLength(), m_Creature.FightMode, false, false, true))
                        {
                            if (m_Creature.Debug)
                                m_Creature.DebugSay("I have detected {0}, attacking", m_Creature.FocusMob.Name);

                            m_Creature.Combatant = m_Creature.FocusMob;
                            m_Creature.ControlTarget = m_Creature.FocusMob;
                            m_Creature.ControlOrder = OrderType.Attack;
                            Action = ActionType.Combat;
                        }
                        else
                            return base.DoOrderNone();
                    }
                    break;
            }

            return true;
        }

        public override bool DoActionWander()
        {
            if (m_Creature.Orb == null || !m_Creature.Controlled)
                return base.DoActionWander();

            bool incombat = false;

            switch (m_Creature.Orb.Aggression)
            {
                case Aggression.Following:
                    break;
                case Aggression.Defensive:
                    {
                        int length = m_Creature.GetLeashLength();
                        IPoint3D p = m_Creature.Orb.Anchor as IPoint3D;

                        if (p == null)
                            p = m_Creature.ControlMaster;

                        Mobile combatant = m_Creature.Combatant != null ? m_Creature.Combatant as Mobile : m_Creature.ControlMaster.Combatant != null ? m_Creature.ControlMaster.Combatant as Mobile : null;

                        if (combatant != null && combatant.GetDistanceToSqrt(p) <= length)
                        {
                            if (m_Creature.Debug)
                                m_Creature.DebugSay("I have detected {0} as an aggressor, attacking", m_Creature.FocusMob.Name);

                            incombat = true;

                            m_Creature.Combatant = combatant;
                            m_Creature.ControlTarget = combatant;
                            m_Creature.ControlOrder = OrderType.Attack;
                            Action = ActionType.Combat;
                        }
                    }
                    break;
                case Aggression.Aggressive:
                    {
                        if (AcquireFocusMob(m_Creature.GetLeashLength(), m_Creature.FightMode, false, false, true))
                        {
                            if (m_Creature.Debug)
                                m_Creature.DebugSay("I have detected {0}, attacking", m_Creature.FocusMob.Name);

                            incombat = true;

                            m_Creature.Combatant = m_Creature.FocusMob;
                            m_Creature.ControlTarget = m_Creature.FocusMob;
                            m_Creature.ControlOrder = OrderType.Attack;
                            Action = ActionType.Combat;
                        }
                        else
                            return base.DoActionWander();
                    }
                    break;
            }

            if (!incombat && SmartAI && m_Creature.Mana < m_Creature.ManaMax)
            {
                m_Creature.DebugSay("I am going to meditate");
                m_Creature.UseSkill(SkillName.Meditation);
            }
            else if (!incombat)
            {
                m_Creature.DebugSay("I am wandering");

                m_Creature.Warmode = false;

                base.DoActionWander();

                Spell spell = CheckCastHealingSpell();

                if (spell != null)
                    spell.Cast();
            }

            return true;
        }

        public override bool AcquireFocusMob(int iRange, FightMode acqType, bool bPlayerOnly, bool bFacFriend, bool bFacFoe)
        {
            bool found = base.AcquireFocusMob(iRange, acqType, bPlayerOnly, bFacFriend, bFacFoe);

            if (m_Creature.Orb == null || m_Creature.ControlMaster == null)
                return found;

            if (!found && Core.TickCount - m_Creature.NextReacquireTime < 0)
            {
                m_Creature.FocusMob = null;
                return false;
            }

            m_Creature.NextReacquireTime = Core.TickCount + (int)m_Creature.ReacquireDelay.TotalMilliseconds;

            if (!found && m_Creature.Map != null && m_Creature.Map != Map.Internal)
            {
                switch (m_Creature.Orb.Aggression)
                {
                    case Aggression.Following:
                        break;
                    case Aggression.Defensive:
                        {
                            Mobile focus = m_Creature.Combatant as Mobile;

                            if (focus == null)
                                focus = m_Creature.ControlMaster.Combatant as Mobile;

                            if (focus != null)
                            {
                                m_Creature.FocusMob = focus;
                                found = true;
                            }
                        }
                        break;
                    case Aggression.Aggressive:
                        {
                            int range = m_Creature.GetLeashLength();
                            IPoint3D p = m_Creature.Orb.Anchor as IPoint3D;

                            if (p == null)
                                p = m_Creature;

                            Mobile focus = GetFocus(p, range);

                            if (focus != null)
                            {
                                m_Creature.FocusMob = focus;
                                found = true;
                            }
                        }
                        break;
                }
            }

            return found;
        }

        private Mobile GetFocus(IPoint3D p, int range)
        {
            IPooledEnumerable eable = m_Creature.Map.GetMobilesInRange(new Point3D(p), range);
            Mobile focus = null;
            int dist = range;

            foreach (Mobile m in eable)
            {
                if (m is DespiseCreature)
                {
                    DespiseCreature dc = m as DespiseCreature;

                    if (dc.Orb == null && !dc.Controlled && dc.Alignment != Alignment.Neutral && dc.Alignment != m_Creature.Alignment)
                    {
                        int distance = (int)m_Creature.GetDistanceToSqrt(dc);

                        if (focus == null || distance < dist)
                        {
                            focus = dc;
                            dist = distance;
                        }
                    }
                }
                else if (m is DespiseBoss)
                {
                    focus = m;
                    break;
                }
            }

            eable.Free();

            return focus;
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
        }
    }
}