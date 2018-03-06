using System;
using System.Collections.Generic;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.First;
using Server.Spells.Fourth;
using Server.Spells.Necromancy;
using Server.Spells.Second;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Third;
using Server.Targeting;

namespace Server.Mobiles
{
    public class SpellbinderAI : BaseAI
    {
        private static readonly int[] m_Offsets = new int[]
        {
            -1, -1,
            -1, 0,
            -1, 1,
            0, -1,
            0, 1,
            1, -1,
            1, 0,
            1, 1,
            -2, -2,
            -2, -1,
            -2, 0,
            -2, 1,
            -2, 2,
            -1, -2,
            -1, 2,
            0, -2,
            0, 2,
            1, -2,
            1, 2,
            2, -2,
            2, -1,
            2, 0,
            2, 1,
            2, 2
        };
        private const double HealChance = 0.05;// 5% chance to heal at gm necromancy, uses spirit speak healing
        private const double TeleportChance = 0.05;// 5% chance to teleport at gm magery
        private const double DispelChance = 0.75;// 75% chance to dispel at gm magery
        private DateTime m_NextCastTime;
        private DateTime m_NextHealTime = DateTime.UtcNow;
        public SpellbinderAI(BaseCreature m)
            : base(m)
        {
        }

        public override bool Think()
        {
            if (m_Mobile.Deleted)
                return false;

            Target targ = m_Mobile.Target;

            if (targ != null)
            {
                ProcessTarget(targ);

                return true;
            }
            else
            {
                return base.Think();
            }
        }

        public virtual double ScaleByNecromancy(double v)
        {
            return m_Mobile.Skills[SkillName.Necromancy].Value * v * 0.01;
        }

        public virtual double ScaleByMagery(double v)
        {
            return m_Mobile.Skills[SkillName.Magery].Value * v * 0.01;
        }

        public override bool DoActionWander()
        {
            if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
            {
                m_Mobile.DebugSay("I am going to attack {0}", m_Mobile.FocusMob.Name);

                m_Mobile.Combatant = m_Mobile.FocusMob;
                Action = ActionType.Combat;
                m_NextCastTime = DateTime.UtcNow;
            }
            else if (m_Mobile.Mana < m_Mobile.ManaMax)
            {
                m_Mobile.DebugSay("I am going to meditate");

                m_Mobile.UseSkill(SkillName.Meditation);
            }
            else
            {
                m_Mobile.DebugSay("I am wandering");

                m_Mobile.Warmode = false;

                base.DoActionWander();

                if (m_Mobile.Poisoned)
                {
                    new CureSpell(m_Mobile, null).Cast();
                }
                else if (!m_Mobile.Summoned)
                {
                    if (m_Mobile.Hits < (m_Mobile.HitsMax - 50))
                    {
                        if (!new GreaterHealSpell(m_Mobile, null).Cast())
                            new HealSpell(m_Mobile, null).Cast();
                    }
                    else if (m_Mobile.Hits < (m_Mobile.HitsMax - 10))
                    {
                        new HealSpell(m_Mobile, null).Cast();
                    }
                }
            }

            return true;
        }

        public void RunTo(Mobile m)
        {
            if (m.Paralyzed || m.Frozen)
            {
                if (m_Mobile.InRange(m, 1))
                    RunFrom(m);
                else if (!m_Mobile.InRange(m, m_Mobile.RangeFight > 2 ? m_Mobile.RangeFight : 2) && !MoveTo(m, true, 1))
                    OnFailedMove();
            }
            else
            {
                if (!m_Mobile.InRange(m, m_Mobile.RangeFight))
                {
                    if (!MoveTo(m, true, 1))
                        OnFailedMove();
                }
                else if (m_Mobile.InRange(m, m_Mobile.RangeFight - 1))
                {
                    RunFrom(m);
                }
            }
        }

        public void RunFrom(IDamageable m)
        {
            Run((Direction)((int)m_Mobile.GetDirectionTo(m) - 4) & Direction.Mask);
        }

        public void OnFailedMove()
        {
            if (!m_Mobile.DisallowAllMoves && ScaleByMagery(TeleportChance) > Utility.RandomDouble())
            {
                if (m_Mobile.Target != null)
                    m_Mobile.Target.Cancel(m_Mobile, TargetCancelType.Canceled);

                new TeleportSpell(m_Mobile, null).Cast();

                m_Mobile.DebugSay("I am stuck, I'm going to try teleporting away");
            }
            else if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
            {
                m_Mobile.DebugSay("My move is blocked, so I am going to attack {0}", m_Mobile.FocusMob.Name);

                m_Mobile.Combatant = m_Mobile.FocusMob;
                Action = ActionType.Combat;
            }
            else
            {
                m_Mobile.DebugSay("I am stuck");
            }
        }

        public void Run(Direction d)
        {
            if ((m_Mobile.Spell != null && m_Mobile.Spell.IsCasting) || m_Mobile.Paralyzed || m_Mobile.Frozen || m_Mobile.DisallowAllMoves)
                return;

            m_Mobile.Direction = d | Direction.Running;

            if (!DoMove(m_Mobile.Direction, true))
                OnFailedMove();
        }

        public virtual Spell GetRandomCurseSpell()
        {
            int necro = (int)((m_Mobile.Skills[SkillName.Necromancy].Value + 50.0) / (100.0 / 7.0));
            int mage = (int)((m_Mobile.Skills[SkillName.Magery].Value + 50.0) / (100.0 / 7.0));

            if (mage < 1)
                mage = 1;

            if (necro < 1)
                necro = 1;

            if (m_Mobile.Skills[SkillName.Necromancy].Value > 30 && Utility.Random(necro) > Utility.Random(mage))
            {
                switch ( Utility.Random(necro - 5) )
                {
                    case 0:
                    case 1:
                        return new CorpseSkinSpell(m_Mobile, null);
                    case 2:
                    case 3:
                        return new MindRotSpell(m_Mobile, null);
                    default:
                        return new MindRotSpell(m_Mobile, null);
                }
            }

            if (Utility.RandomBool() && mage > 3)
                return new CurseSpell(m_Mobile, null);

            switch ( Utility.Random(3) )
            {
                default:
                case 0:
                    return new WeakenSpell(m_Mobile, null);
                case 1:
                    return new ClumsySpell(m_Mobile, null);
                case 2:
                    return new FeeblemindSpell(m_Mobile, null);
            }
        }

        public virtual Spell GetRandomManaDrainSpell()
        {
            if (Utility.RandomBool())
            {
                if (m_Mobile.Skills[SkillName.Magery].Value >= 80.0)
                    return new ManaVampireSpell(m_Mobile, null);
            }

            return new ManaDrainSpell(m_Mobile, null);
        }

        public virtual Spell DoDispel(Mobile toDispel)
        {
            if (ScaleByMagery(DispelChance) > Utility.RandomDouble())
                return new DispelSpell(m_Mobile, null);

            return ChooseSpell(toDispel);
        }

        public virtual Spell ChooseSpell(IDamageable c)
        {
            Spell spell = CheckCastHealingSpell();

            if (spell != null)
                return spell;

            if(!(c is Mobile))
            {
                return null;
            }

            Mobile mob = c as Mobile;
            double damage = ((m_Mobile.Skills[SkillName.SpiritSpeak].Value - mob.Skills[SkillName.MagicResist].Value) / 10) + (mob.Player ? 18 : 30);

            if (damage > c.Hits)
                spell = new ManaDrainSpell(m_Mobile, null);

            switch ( Utility.Random(16) )
            {
                case 0:
                case 1:
                case 2:	// Poison them
                    {
                        m_Mobile.DebugSay("Attempting to BloodOath");

                        if (!mob.Poisoned)
                            spell = new BloodOathSpell(m_Mobile, null);

                        break;
                    }
                case 3:	// Bless ourselves.
                    {
                        m_Mobile.DebugSay("Blessing myself");

                        spell = new BlessSpell(m_Mobile, null);
                        break;
                    }
                case 4:
                case 5:
                case 6: // Curse them.
                    {
                        m_Mobile.DebugSay("Attempting to curse");

                        spell = GetRandomCurseSpell();
                        break;
                    }
                case 7:	// Paralyze them.
                    {
                        m_Mobile.DebugSay("Attempting to paralyze");

                        if (m_Mobile.Skills[SkillName.Magery].Value > 50.0)
                            spell = new ParalyzeSpell(m_Mobile, null);

                        break;
                    }
                case 8: // Drain mana
                    {
                        m_Mobile.DebugSay("Attempting to drain mana");

                        spell = GetRandomManaDrainSpell();
                        break;
                    }
                case 9: // Blood oath them
                    {
                        m_Mobile.DebugSay("Attempting to blood oath");

                        if (m_Mobile.Skills[SkillName.Necromancy].Value > 30 && BloodOathSpell.GetBloodOath(mob) != m_Mobile)
                            spell = new BloodOathSpell(m_Mobile, null);

                        break;
                    }
            }

            return spell;
        }

        public override bool DoActionCombat()
        {
            IDamageable c = m_Mobile.Combatant;
            m_Mobile.Warmode = true;

            if (c == null || c.Deleted || !c.Alive || (c is Mobile && ((Mobile)c).IsDeadBondedPet) || !m_Mobile.CanSee(c) || !m_Mobile.CanBeHarmful(c, false) || c.Map != m_Mobile.Map)
            {
                // Our combatant is deleted, dead, hidden, or we cannot hurt them
                // Try to find another combatant
                if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
                {
                    m_Mobile.DebugSay("Something happened to my combatant, so I am going to fight {0}", m_Mobile.FocusMob.Name);

                    m_Mobile.Combatant = c = m_Mobile.FocusMob;
                    m_Mobile.FocusMob = null;
                }
                else
                {
                    m_Mobile.DebugSay("Something happened to my combatant, and nothing is around. I am on guard.");
                    Action = ActionType.Guard;
                    return true;
                }
            }

            if (!m_Mobile.InLOS(c))
            {
                if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
                {
                    m_Mobile.Combatant = c = m_Mobile.FocusMob;
                    m_Mobile.FocusMob = null;
                }
            }

            if (!m_Mobile.StunReady && m_Mobile.Skills[SkillName.Wrestling].Value >= 80.0 && m_Mobile.Skills[SkillName.Anatomy].Value >= 80.0)
                EventSink.InvokeStunRequest(new StunRequestEventArgs(m_Mobile));

            if (!m_Mobile.InRange(c, m_Mobile.RangePerception))
            {
                // They are somewhat far away, can we find something else?
                if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
                {
                    m_Mobile.Combatant = m_Mobile.FocusMob;
                    m_Mobile.FocusMob = null;
                }
                else if (!m_Mobile.InRange(c, m_Mobile.RangePerception * 3))
                {
                    m_Mobile.Combatant = null;
                }

                c = m_Mobile.Combatant;

                if (c == null)
                {
                    m_Mobile.DebugSay("My combatant has fled, so I am on guard");
                    Action = ActionType.Guard;

                    return true;
                }
            }

            if (!m_Mobile.Controlled && !m_Mobile.Summoned && m_Mobile.CanFlee)
            {
                if (m_Mobile.Hits < m_Mobile.HitsMax * 20 / 100)
                {
                    // We are low on health, should we flee?
                    if (Utility.Random(100) <= Math.Max(10, 10 + c.Hits - m_Mobile.Hits))
                    {
                        m_Mobile.DebugSay("I am going to flee from {0}", c.Name);
                        Action = ActionType.Flee;
                        return true;
                    }
                }
            }

            if (m_Mobile.Spell == null && DateTime.UtcNow > m_NextCastTime && m_Mobile.InRange(c, 12))
            {
                // We are ready to cast a spell
                Spell spell = null;
                Mobile toDispel = FindDispelTarget(true);

                if (m_Mobile.Poisoned) // Top cast priority is cure
                {
                    m_Mobile.DebugSay("I am going to cure myself");

                    spell = new CureSpell(m_Mobile, null);
                }
                else if (toDispel != null) // Something dispellable is attacking us
                {
                    m_Mobile.DebugSay("I am going to dispel {0}", toDispel);

                    spell = DoDispel(toDispel);
                }
                else if (c is Mobile && (((Mobile)c).Spell is HealSpell || ((Mobile)c).Spell is GreaterHealSpell) && !((Mobile)c).Poisoned) // They have a heal spell out
                {
                    spell = new BloodOathSpell(m_Mobile, null);
                }
                else
                {
                    spell = ChooseSpell(c);
                }

                // Now we have a spell picked
                // Move first before casting

                if (toDispel != null)
                {
                    if (m_Mobile.InRange(toDispel, 10))
                        RunFrom(toDispel);
                    else if (!m_Mobile.InRange(toDispel, 12))
                        RunTo(toDispel);
                }
                else if(c is Mobile)
                {
                    RunTo((Mobile)c);
                }

                if (spell != null)
                    spell.Cast();

                TimeSpan delay;

                if (spell is DispelSpell)
                    delay = TimeSpan.FromSeconds(m_Mobile.ActiveSpeed);
                else
                    delay = GetDelay();

                m_NextCastTime = DateTime.UtcNow + delay;
            }
            else if (c is Mobile && (m_Mobile.Spell == null || !m_Mobile.Spell.IsCasting))
            {
                RunTo((Mobile)c);
            }

            return true;
        }

        public override bool DoActionGuard()
        {
            if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
            {
                m_Mobile.DebugSay("I am going to attack {0}", m_Mobile.FocusMob.Name);

                m_Mobile.Combatant = m_Mobile.FocusMob;
                Action = ActionType.Combat;
            }
            else
            {
                if (m_Mobile.Poisoned)
                {
                    new CureSpell(m_Mobile, null).Cast();
                }
                else if (!m_Mobile.Summoned && ((ScaleByMagery(HealChance) > Utility.RandomDouble())))
                {
                    if (m_Mobile.Hits < (m_Mobile.HitsMax - 50))
                    {
                        if (!new GreaterHealSpell(m_Mobile, null).Cast())
                            new HealSpell(m_Mobile, null).Cast();
                    }
                    else if (m_Mobile.Hits < (m_Mobile.HitsMax - 10))
                    {
                        new HealSpell(m_Mobile, null).Cast();
                    }
                    else
                    {
                        base.DoActionGuard();
                    }
                }
                else
                {
                    base.DoActionGuard();
                }
            }

            return true;
        }

        public override bool DoActionFlee()
        {
            Mobile c = m_Mobile.Combatant as Mobile;

            if ((m_Mobile.Mana > 20 || m_Mobile.Mana == m_Mobile.ManaMax) && m_Mobile.Hits > (m_Mobile.HitsMax / 2))
            {
                // If I have a target, go back and fight them
                if (c != null && m_Mobile.GetDistanceToSqrt(c) <= m_Mobile.RangePerception * 2)
                {
                    m_Mobile.DebugSay("I am stronger now, reengaging {0}", c.Name);
                    Action = ActionType.Combat;
                }
                else
                {
                    m_Mobile.DebugSay("I am stronger now, my guard is up");
                    Action = ActionType.Guard;
                }
            }
            else
            {
                base.DoActionFlee();
            }

            return true;
        }

        public Mobile FindDispelTarget(bool activeOnly)
        {
            if (m_Mobile.Deleted || m_Mobile.Int < 95 || CanDispel(m_Mobile) || m_Mobile.AutoDispel)
                return null;

            if (activeOnly)
            {
                List<AggressorInfo> aggressed = m_Mobile.Aggressed;
                List<AggressorInfo> aggressors = m_Mobile.Aggressors;

                Mobile active = null;
                double activePrio = 0.0;

                Mobile comb = m_Mobile.Combatant as Mobile;

                if (comb != null && !comb.Deleted && comb.Alive && !comb.IsDeadBondedPet && m_Mobile.InRange(comb, 12) && CanDispel(comb))
                {
                    active = comb;
                    activePrio = m_Mobile.GetDistanceToSqrt(comb);

                    if (activePrio <= 2)
                        return active;
                }

                for (int i = 0; i < aggressed.Count; ++i)
                {
                    AggressorInfo info = (AggressorInfo)aggressed[i];
                    Mobile m = (Mobile)info.Defender;

                    if (m != comb && m.Combatant == m_Mobile && m_Mobile.InRange(m, 12) && CanDispel(m))
                    {
                        double prio = m_Mobile.GetDistanceToSqrt(m);

                        if (active == null || prio < activePrio)
                        {
                            active = m;
                            activePrio = prio;

                            if (activePrio <= 2)
                                return active;
                        }
                    }
                }

                for (int i = 0; i < aggressors.Count; ++i)
                {
                    AggressorInfo info = (AggressorInfo)aggressors[i];
                    Mobile m = (Mobile)info.Attacker;

                    if (m != comb && m.Combatant == m_Mobile && m_Mobile.InRange(m, 12) && CanDispel(m))
                    {
                        double prio = m_Mobile.GetDistanceToSqrt(m);

                        if (active == null || prio < activePrio)
                        {
                            active = m;
                            activePrio = prio;

                            if (activePrio <= 2)
                                return active;
                        }
                    }
                }

                return active;
            }
            else
            {
                Map map = m_Mobile.Map;

                if (map != null)
                {
                    Mobile active = null, inactive = null;
                    double actPrio = 0.0, inactPrio = 0.0;

                    Mobile comb = m_Mobile.Combatant as Mobile;

                    if (comb != null && !comb.Deleted && comb.Alive && !comb.IsDeadBondedPet && CanDispel(comb))
                    {
                        active = inactive = comb;
                        actPrio = inactPrio = m_Mobile.GetDistanceToSqrt(comb);
                    }

                    IPooledEnumerable eable = m_Mobile.GetMobilesInRange(12);
                    foreach (Mobile m in eable)
                    {
                        if (m != m_Mobile && CanDispel(m))
                        {
                            double prio = m_Mobile.GetDistanceToSqrt(m);

                            if (!activeOnly && (inactive == null || prio < inactPrio))
                            {
                                inactive = m;
                                inactPrio = prio;
                            }

                            if ((m_Mobile.Combatant == m || m.Combatant == m_Mobile) && (active == null || prio < actPrio))
                            {
                                active = m;
                                actPrio = prio;
                            }
                        }
                    }

                    eable.Free();

                    return active != null ? active : inactive;
                }
            }

            return null;
        }

        public bool CanDispel(Mobile m)
        {
            return (m is BaseCreature && ((BaseCreature)m).Summoned && m_Mobile.CanBeHarmful(m, false) && !((BaseCreature)m).IsAnimatedDead);
        }

        private Spell CheckCastHealingSpell()
        {
            // If I'm poisoned, always attempt to cure.
            if (m_Mobile.Poisoned)
                return new CureSpell(m_Mobile, null);

            // Summoned creatures never heal themselves.
            if (m_Mobile.Summoned)
                return null;

            if (m_Mobile.Controlled)
            {
                if (DateTime.UtcNow < m_NextHealTime)
                    return null;
            }

            if (ScaleByMagery(HealChance) < Utility.RandomDouble())
                return null;

            Spell spell = null;

            if (m_Mobile.Hits < (m_Mobile.HitsMax - 50))
            {
                spell = new GreaterHealSpell(m_Mobile, null);

                if (spell == null)
                    spell = new HealSpell(m_Mobile, null);
            }
            else if (m_Mobile.Hits < (m_Mobile.HitsMax - 10))
                spell = new HealSpell(m_Mobile, null);

            double delay;

            if (m_Mobile.Int >= 500)
                delay = Utility.RandomMinMax(7, 10);
            else
                delay = Math.Sqrt(600 - m_Mobile.Int);

            m_Mobile.UseSkill(SkillName.SpiritSpeak);

            m_NextHealTime = DateTime.UtcNow + TimeSpan.FromSeconds(delay);

            return spell;
        }

        private TimeSpan GetDelay()
        {
            double del = ScaleByMagery(3.0);
            double min = 6.0 - (del * 0.75);
            double max = 6.0 - (del * 1.25);

            return TimeSpan.FromSeconds(min + ((max - min) * Utility.RandomDouble()));
        }

        private void ProcessTarget(Target targ)
        {
            bool isDispel = (targ is DispelSpell.InternalTarget);
            bool isParalyze = (targ is ParalyzeSpell.InternalTarget);
            bool isTeleport = (targ is TeleportSpell.InternalTarget);
            bool teleportAway = false;

            Mobile toTarget;

            if (isDispel)
            {
                toTarget = FindDispelTarget(false);

                if (toTarget != null && m_Mobile.InRange(toTarget, 10))
                    RunFrom(toTarget);
            }
            else if (isParalyze || isTeleport)
            {
                toTarget = FindDispelTarget(true);

                if (toTarget == null)
                {
                    toTarget = m_Mobile.Combatant as Mobile;

                    if (toTarget != null)
                        RunTo(toTarget);
                }
                else if (m_Mobile.InRange(toTarget, 10))
                {
                    RunFrom(toTarget);
                    teleportAway = true;
                }
                else
                {
                    teleportAway = true;
                }
            }
            else
            {
                toTarget = m_Mobile.Combatant as Mobile;

                if (toTarget != null)
                    RunTo(toTarget);
            }

            if ((targ.Flags & TargetFlags.Harmful) != 0 && toTarget != null)
            {
                if ((targ.Range == -1 || m_Mobile.InRange(toTarget, targ.Range)) && m_Mobile.CanSee(toTarget) && m_Mobile.InLOS(toTarget))
                {
                    targ.Invoke(m_Mobile, toTarget);
                }
                else if (isDispel)
                {
                    targ.Cancel(m_Mobile, TargetCancelType.Canceled);
                }
            }
            else if ((targ.Flags & TargetFlags.Beneficial) != 0)
            {
                targ.Invoke(m_Mobile, m_Mobile);
            }
            else if (isTeleport && toTarget != null)
            {
                Map map = m_Mobile.Map;

                if (map == null)
                {
                    targ.Cancel(m_Mobile, TargetCancelType.Canceled);
                    return;
                }

                int px, py;

                if (teleportAway)
                {
                    int rx = m_Mobile.X - toTarget.X;
                    int ry = m_Mobile.Y - toTarget.Y;

                    double d = m_Mobile.GetDistanceToSqrt(toTarget);

                    px = toTarget.X + (int)(rx * (10 / d));
                    py = toTarget.Y + (int)(ry * (10 / d));
                }
                else
                {
                    px = toTarget.X;
                    py = toTarget.Y;
                }

                for (int i = 0; i < m_Offsets.Length; i += 2)
                {
                    int x = m_Offsets[i], y = m_Offsets[i + 1];

                    Point3D p = new Point3D(px + x, py + y, 0);

                    LandTarget lt = new LandTarget(p, map);

                    if ((targ.Range == -1 || m_Mobile.InRange(p, targ.Range)) && m_Mobile.InLOS(lt) && map.CanSpawnMobile(px + x, py + y, lt.Z) && !SpellHelper.CheckMulti(p, map))
                    {
                        targ.Invoke(m_Mobile, lt);
                        return;
                    }
                }

                int teleRange = targ.Range;

                if (teleRange < 0)
                    teleRange = 12;

                for (int i = 0; i < 10; ++i)
                {
                    Point3D randomPoint = new Point3D(m_Mobile.X - teleRange + Utility.Random(teleRange * 2 + 1), m_Mobile.Y - teleRange + Utility.Random(teleRange * 2 + 1), 0);

                    LandTarget lt = new LandTarget(randomPoint, map);

                    if (m_Mobile.InLOS(lt) && map.CanSpawnMobile(lt.X, lt.Y, lt.Z) && !SpellHelper.CheckMulti(randomPoint, map))
                    {
                        targ.Invoke(m_Mobile, new LandTarget(randomPoint, map));
                        return;
                    }
                }

                targ.Cancel(m_Mobile, TargetCancelType.Canceled);
            }
            else
            {
                targ.Cancel(m_Mobile, TargetCancelType.Canceled);
            }
        }
    }
}
