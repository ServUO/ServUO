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
        private long _NextAggressorCheck;

        private DespiseCreature m_Creature;

        public DespiseMeleeAI(DespiseCreature m) : base(m)
        {
            m_Creature = m;
        }

        public override bool Obey()
        {
            if (m_Creature.Orb == null || !m_Creature.Controlled)
                return base.Obey();

            switch (m_Creature.Orb.Aggression)
            {
                default:
                    {
                        m_Creature.ControlOrder = OrderType.Follow;
                        DoOrderFollow();
                    }
                    break;
                case Aggression.Defensive:
                    {
                        if (m_Creature.Combatant != null)
                        {
                            if (m_Creature.ControlOrder == OrderType.Follow)
                            {
                                m_Creature.ControlOrder = OrderType.Attack;
                                Action = ActionType.Combat;
                            }

                            break;
                        }

                        if (_NextAggressorCheck <= Core.TickCount)
                        {
                            IPoint3D p = m_Creature.Orb.GetAnchorActual();
                            double range = m_Creature.RangePerception;

                            IPooledEnumerable eable = m_Creature.Map.GetMobilesInRange(new Point3D(p), (int)range);
                            Mobile closest = null;

                            foreach (Mobile m in eable)
                            {
                                if (m.Combatant == m_Creature || m.Combatant == m_Creature.ControlMaster)
                                {
                                    double dist = closest == null ? range : closest.GetDistanceToSqrt(m_Creature);
                                    if (closest == null || dist < range)
                                    {
                                        range = dist;
                                        closest = m;
                                    }
                                }
                            }

                            eable.Free();

                            if (closest != null)
                            {
                                m_Creature.ControlTarget = closest;
                                m_Creature.ControlOrder = OrderType.Attack;
                                m_Creature.Combatant = closest;
                                m_Creature.DebugSay("But -that- is not dead. Here we go again...");

                                Action = ActionType.Combat;
                            }

                            _NextAggressorCheck = Core.TickCount + 1000;
                        }
                    }
                    break;
                case Aggression.Aggressive:
                    {
                        if (m_Creature.Combatant != null)
                        {
                            if (m_Creature.ControlOrder == OrderType.Follow)
                            {
                                m_Creature.ControlOrder = OrderType.Attack;
                                Action = ActionType.Combat;
                            }

                            break;
                        }

                        if (AcquireFocusMob(m_Creature.RangePerception, m_Creature.FightMode, false, false, true))
                        {
                            if (m_Creature.FocusMob == m_Creature.ControlMaster)
                                break;

                            if (m_Creature.Debug)
                                m_Creature.DebugSay("I have detected {0}, attacking", m_Creature.FocusMob.Name);

                            m_Creature.ControlOrder = OrderType.Attack;
                            m_Creature.Combatant = m_Creature.FocusMob;

                            Action = ActionType.Combat;
                        }
                    }
                    break;
            }

            if (m_Creature.Combatant == null)
            {
                m_Creature.ControlOrder = OrderType.Follow;
                m_Creature.ControlTarget = m_Creature.ControlMaster;
                Action = ActionType.Guard;
                DoOrderFollow();
            }

            Think();
            return true;
        }

        public override bool DoOrderFollow()
        {
            if (m_Creature.Orb != null)
                m_Creature.Orb.InvalidateHue();

            return base.DoOrderFollow();
        }

        public override bool AcquireFocusMob(int iRange, FightMode acqType, bool bPlayerOnly, bool bFacFriend, bool bFacFoe)
        {
            if (m_Creature.Orb == null || m_Creature.ControlMaster == null)
            {
                return base.AcquireFocusMob(iRange, acqType, bPlayerOnly, bFacFriend, bFacFoe);
            }

            if (m_Creature.Orb.Aggression != Aggression.Aggressive)
                return false;

            if (Core.TickCount - m_Creature.NextReacquireTime < 0)
            {
                m_Creature.FocusMob = null;
                return false;
            }

            m_Creature.NextReacquireTime = Core.TickCount + (int)m_Creature.ReacquireDelay.TotalMilliseconds;

            int range = m_Creature.RangePerception;
            IPoint3D p = m_Creature.Orb.Anchor as IPoint3D;

            if (p == null)
                p = m_Creature;

            Mobile focus = GetFocus(p, range);

            if (focus != null)
            {
                m_Creature.FocusMob = focus;
                return true;
            }

            return false;
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

                    if(m is DespiseBoss || (dc != null && (dc.Orb == null && !dc.Controlled) || (dc.Alignment != m_Creature.Alignment)))
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

        public override bool WalkMobileRange(IPoint3D p, int iSteps, bool bRun, int iWantDistMin, int iWantDistMax)
        {
            if (m_Creature.Orb == null || m_Creature.ControlMaster == null)
            {
                return base.WalkMobileRange(p, iSteps, bRun, iWantDistMin, iWantDistMax);
            }

            int range = m_Creature.GetLeashLength();

            if (p is Mobile && (Mobile)p == m_Creature.ControlMaster)
            {
                p = m_Creature.Orb.GetAnchorActual();

                if (m_Creature.InRange(p, range))
                {
                    return false;
                }
                else
                {
                    return base.WalkMobileRange(p, iSteps, bRun, range, range);
                }
            }

            return base.WalkMobileRange(p, iSteps, bRun, iWantDistMin, iWantDistMax);
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
        private long _NextAggressorCheck;

        private DespiseCreature m_Creature;

        public DespiseMageAI(DespiseCreature m)
            : base(m)
        {
            m_Creature = m;
        }

        public override bool Obey()
        {
            if (m_Creature.Orb == null || !m_Creature.Controlled)
                return base.Obey();

            switch (m_Creature.Orb.Aggression)
            {
                default:
                    {
                        m_Creature.ControlOrder = OrderType.Follow;
                        DoOrderFollow();
                    }
                    break;
                case Aggression.Defensive:
                    {
                        if (m_Creature.Combatant != null)
                        {
                            if (m_Creature.ControlOrder == OrderType.Follow)
                            {
                                m_Creature.ControlOrder = OrderType.Attack;
                                Action = ActionType.Combat;
                            }

                            break;
                        }

                        if (_NextAggressorCheck <= Core.TickCount)
                        {
                            IPoint3D p = m_Creature.Orb.GetAnchorActual();
                            double range = m_Creature.RangePerception;

                            IPooledEnumerable eable = m_Creature.Map.GetMobilesInRange(new Point3D(p), (int)range);
                            Mobile closest = null;

                            foreach (Mobile m in eable)
                            {
                                if (m.Combatant == m_Creature || m.Combatant == m_Creature.ControlMaster)
                                {
                                    double dist = closest == null ? range : closest.GetDistanceToSqrt(m_Creature);
                                    if (closest == null || dist < range)
                                    {
                                        range = dist;
                                        closest = m;
                                    }
                                }
                            }

                            eable.Free();

                            if (closest != null)
                            {
                                m_Creature.ControlTarget = closest;
                                m_Creature.ControlOrder = OrderType.Attack;
                                m_Creature.Combatant = closest;
                                m_Creature.DebugSay("But -that- is not dead. Here we go again...");

                                Action = ActionType.Combat;
                            }

                            _NextAggressorCheck = Core.TickCount + 1000;
                        }
                    }
                    break;
                case Aggression.Aggressive:
                    {
                        if (m_Creature.Combatant != null)
                        {
                            if (m_Creature.ControlOrder == OrderType.Follow)
                            {
                                m_Creature.ControlOrder = OrderType.Attack;
                                Action = ActionType.Combat;
                            }

                            break;
                        }

                        if (AcquireFocusMob(m_Creature.RangePerception, m_Creature.FightMode, false, false, true))
                        {
                            if (m_Creature.FocusMob == m_Creature.ControlMaster)
                                break;

                            if (m_Creature.Debug)
                                m_Creature.DebugSay("I have detected {0}, attacking", m_Creature.FocusMob.Name);

                            m_Creature.ControlOrder = OrderType.Attack;
                            m_Creature.Combatant = m_Creature.FocusMob;

                            Action = ActionType.Combat;
                        }
                    }
                    break;
            }

            if (m_Creature.Combatant == null)
            {
                m_Creature.ControlOrder = OrderType.Follow;
                m_Creature.ControlTarget = m_Creature.ControlMaster;
                Action = ActionType.Guard;
                DoOrderFollow();
            }

            Think();
            return true;
        }

        public override bool DoOrderFollow()
        {
            if (m_Creature.Orb != null)
                m_Creature.Orb.InvalidateHue();

            return base.DoOrderFollow();
        }

        public override bool AcquireFocusMob(int iRange, FightMode acqType, bool bPlayerOnly, bool bFacFriend, bool bFacFoe)
        {
            if (m_Creature.Orb == null || m_Creature.ControlMaster == null)
            {
                return base.AcquireFocusMob(iRange, acqType, bPlayerOnly, bFacFriend, bFacFoe);
            }

            if (m_Creature.Orb.Aggression != Aggression.Aggressive)
                return false;

            if (Core.TickCount - m_Creature.NextReacquireTime < 0)
            {
                m_Creature.FocusMob = null;
                return false;
            }

            m_Creature.NextReacquireTime = Core.TickCount + (int)m_Creature.ReacquireDelay.TotalMilliseconds;

            int range = m_Creature.RangePerception;
            IPoint3D p = m_Creature.Orb.Anchor as IPoint3D;

            if (p == null)
                p = m_Creature;

            Mobile focus = GetFocus(p, range);

            if (focus != null)
            {
                m_Creature.FocusMob = focus;
                return true;
            }

            return false;
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

                    if (m is DespiseBoss || (dc != null && (dc.Orb == null && !dc.Controlled) || (dc.Alignment != m_Creature.Alignment)))
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

        public override bool WalkMobileRange(IPoint3D p, int iSteps, bool bRun, int iWantDistMin, int iWantDistMax)
        {
            if (m_Creature.Orb == null || m_Creature.ControlMaster == null)
            {
                return base.WalkMobileRange(p, iSteps, bRun, iWantDistMin, iWantDistMax);
            }

            int range = m_Creature.GetLeashLength();

            if (p is Mobile && (Mobile)p == m_Creature.ControlMaster)
            {
                p = m_Creature.Orb.GetAnchorActual();

                if (m_Creature.InRange(p, range))
                {
                    return false;
                }
                else
                {
                    return base.WalkMobileRange(p, iSteps, bRun, range, range);
                }
            }

            return base.WalkMobileRange(p, iSteps, bRun, iWantDistMin, iWantDistMax);
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
        }
    }
}