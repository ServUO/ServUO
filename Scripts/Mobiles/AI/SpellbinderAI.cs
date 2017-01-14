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
            if (this.m_Mobile.Deleted)
                return false;

            Target targ = this.m_Mobile.Target;

            if (targ != null)
            {
                this.ProcessTarget(targ);

                return true;
            }
            else
            {
                return base.Think();
            }
        }

        public virtual double ScaleByNecromancy(double v)
        {
            return this.m_Mobile.Skills[SkillName.Necromancy].Value * v * 0.01;
        }

        public virtual double ScaleByMagery(double v)
        {
            return this.m_Mobile.Skills[SkillName.Magery].Value * v * 0.01;
        }

        public override bool DoActionWander()
        {
            if (this.AcquireFocusMob(this.m_Mobile.RangePerception, this.m_Mobile.FightMode, false, false, true))
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.DebugSay("I am going to attack {0}", this.m_Mobile.FocusMob.Name);

                this.m_Mobile.Combatant = this.m_Mobile.FocusMob;
                this.Action = ActionType.Combat;
                this.m_NextCastTime = DateTime.UtcNow;
            }
            else if (this.m_Mobile.Mana < this.m_Mobile.ManaMax)
            {
                this.m_Mobile.DebugSay("I am going to meditate");

                this.m_Mobile.UseSkill(SkillName.Meditation);
            }
            else
            {
                this.m_Mobile.DebugSay("I am wandering");

                this.m_Mobile.Warmode = false;

                base.DoActionWander();

                if (this.m_Mobile.Poisoned)
                {
                    new CureSpell(this.m_Mobile, null).Cast();
                }
                else if (!this.m_Mobile.Summoned)
                {
                    if (this.m_Mobile.Hits < (this.m_Mobile.HitsMax - 50))
                    {
                        if (!new GreaterHealSpell(this.m_Mobile, null).Cast())
                            new HealSpell(this.m_Mobile, null).Cast();
                    }
                    else if (this.m_Mobile.Hits < (this.m_Mobile.HitsMax - 10))
                    {
                        new HealSpell(this.m_Mobile, null).Cast();
                    }
                }
            }

            return true;
        }

        public void RunTo(Mobile m)
        {
            if (m.Paralyzed || m.Frozen)
            {
                if (this.m_Mobile.InRange(m, 1))
                    this.RunFrom(m);
                else if (!this.m_Mobile.InRange(m, this.m_Mobile.RangeFight > 2 ? this.m_Mobile.RangeFight : 2) && !this.MoveTo(m, true, 1))
                    this.OnFailedMove();
            }
            else
            {
                if (!this.m_Mobile.InRange(m, this.m_Mobile.RangeFight))
                {
                    if (!this.MoveTo(m, true, 1))
                        this.OnFailedMove();
                }
                else if (this.m_Mobile.InRange(m, this.m_Mobile.RangeFight - 1))
                {
                    this.RunFrom(m);
                }
            }
        }

        public void RunFrom(IDamageable m)
        {
            this.Run((Direction)((int)this.m_Mobile.GetDirectionTo(m) - 4) & Direction.Mask);
        }

        public void OnFailedMove()
        {
            if (!this.m_Mobile.DisallowAllMoves && this.ScaleByMagery(TeleportChance) > Utility.RandomDouble())
            {
                if (this.m_Mobile.Target != null)
                    this.m_Mobile.Target.Cancel(this.m_Mobile, TargetCancelType.Canceled);

                new TeleportSpell(this.m_Mobile, null).Cast();

                this.m_Mobile.DebugSay("I am stuck, I'm going to try teleporting away");
            }
            else if (this.AcquireFocusMob(this.m_Mobile.RangePerception, this.m_Mobile.FightMode, false, false, true))
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.DebugSay("My move is blocked, so I am going to attack {0}", this.m_Mobile.FocusMob.Name);

                this.m_Mobile.Combatant = this.m_Mobile.FocusMob;
                this.Action = ActionType.Combat;
            }
            else
            {
                this.m_Mobile.DebugSay("I am stuck");
            }
        }

        public void Run(Direction d)
        {
            if ((this.m_Mobile.Spell != null && this.m_Mobile.Spell.IsCasting) || this.m_Mobile.Paralyzed || this.m_Mobile.Frozen || this.m_Mobile.DisallowAllMoves)
                return;

            this.m_Mobile.Direction = d | Direction.Running;

            if (!this.DoMove(this.m_Mobile.Direction, true))
                this.OnFailedMove();
        }

        public virtual Spell GetRandomCurseSpell()
        {
            int necro = (int)((this.m_Mobile.Skills[SkillName.Necromancy].Value + 50.0) / (100.0 / 7.0));
            int mage = (int)((this.m_Mobile.Skills[SkillName.Magery].Value + 50.0) / (100.0 / 7.0));
			
            if (mage < 1)
                mage = 1;
				
            if (necro < 1)
                necro = 1;
				
            if (this.m_Mobile.Skills[SkillName.Necromancy].Value > 30 && Utility.Random(necro) > Utility.Random(mage))
            { 
                switch ( Utility.Random(necro - 5) )
                {
                    case 0:
                    case 1:
                        return new CorpseSkinSpell(this.m_Mobile, null);
                    case 2:
                    case 3:
                        return new MindRotSpell(this.m_Mobile, null);
                    default:
                        return new MindRotSpell(this.m_Mobile, null);
                }
            }
			
            if (Utility.RandomBool() && mage > 3)
                return new CurseSpell(this.m_Mobile, null);

            switch ( Utility.Random(3) )
            {
                default:
                case 0:
                    return new WeakenSpell(this.m_Mobile, null);
                case 1:
                    return new ClumsySpell(this.m_Mobile, null);
                case 2:
                    return new FeeblemindSpell(this.m_Mobile, null);
            }
        }

        public virtual Spell GetRandomManaDrainSpell()
        {
            if (Utility.RandomBool())
            {
                if (this.m_Mobile.Skills[SkillName.Magery].Value >= 80.0)
                    return new ManaVampireSpell(this.m_Mobile, null);
            }

            return new ManaDrainSpell(this.m_Mobile, null);
        }

        public virtual Spell DoDispel(Mobile toDispel)
        {
            if (this.ScaleByMagery(DispelChance) > Utility.RandomDouble())
                return new DispelSpell(this.m_Mobile, null);

            return this.ChooseSpell(toDispel);
        }

        public virtual Spell ChooseSpell(IDamageable c)
        {
            Spell spell = this.CheckCastHealingSpell();

            if (spell != null)
                return spell;

            if(!(c is Mobile))
            {
                return null;
            }

            Mobile mob = c as Mobile;
            double damage = ((this.m_Mobile.Skills[SkillName.SpiritSpeak].Value - mob.Skills[SkillName.MagicResist].Value) / 10) + (mob.Player ? 18 : 30);
			
            if (damage > c.Hits)
                spell = new ManaDrainSpell(this.m_Mobile, null);

            switch ( Utility.Random(16) )
            {
                case 0:
                case 1:
                case 2:	// Poison them
                    {
                        this.m_Mobile.DebugSay("Attempting to BloodOath");

                        if (!mob.Poisoned)
                            spell = new BloodOathSpell(this.m_Mobile, null);

                        break;
                    }
                case 3:	// Bless ourselves.
                    {
                        this.m_Mobile.DebugSay("Blessing myself");

                        spell = new BlessSpell(this.m_Mobile, null);
                        break;
                    }
                case 4:
                case 5:
                case 6: // Curse them.
                    {
                        this.m_Mobile.DebugSay("Attempting to curse");

                        spell = this.GetRandomCurseSpell();
                        break;
                    }
                case 7:	// Paralyze them.
                    {
                        this.m_Mobile.DebugSay("Attempting to paralyze");

                        if (this.m_Mobile.Skills[SkillName.Magery].Value > 50.0)
                            spell = new ParalyzeSpell(this.m_Mobile, null);

                        break;
                    }
                case 8: // Drain mana
                    {
                        this.m_Mobile.DebugSay("Attempting to drain mana");

                        spell = this.GetRandomManaDrainSpell();
                        break;
                    }
                case 9: // Blood oath them
                    {
                        this.m_Mobile.DebugSay("Attempting to blood oath");

                        if (this.m_Mobile.Skills[SkillName.Necromancy].Value > 30 && BloodOathSpell.GetBloodOath(mob) != this.m_Mobile)
                            spell = new BloodOathSpell(this.m_Mobile, null);
						
                        break;
                    }
            }

            return spell;
        }

        public override bool DoActionCombat()
        {
            IDamageable c = this.m_Mobile.Combatant;
            this.m_Mobile.Warmode = true;

            if (c == null || c.Deleted || !c.Alive || (c is Mobile && ((Mobile)c).IsDeadBondedPet) || !this.m_Mobile.CanSee(c) || !this.m_Mobile.CanBeHarmful(c, false) || c.Map != this.m_Mobile.Map)
            {
                // Our combatant is deleted, dead, hidden, or we cannot hurt them
                // Try to find another combatant
                if (this.AcquireFocusMob(this.m_Mobile.RangePerception, this.m_Mobile.FightMode, false, false, true))
                {
                    if (this.m_Mobile.Debug)
                        this.m_Mobile.DebugSay("Something happened to my combatant, so I am going to fight {0}", this.m_Mobile.FocusMob.Name);

                    this.m_Mobile.Combatant = c = this.m_Mobile.FocusMob;
                    this.m_Mobile.FocusMob = null;
                }
                else
                {
                    this.m_Mobile.DebugSay("Something happened to my combatant, and nothing is around. I am on guard.");
                    this.Action = ActionType.Guard;
                    return true;
                }
            }

            if (!this.m_Mobile.InLOS(c))
            {
                if (this.AcquireFocusMob(this.m_Mobile.RangePerception, this.m_Mobile.FightMode, false, false, true))
                {
                    this.m_Mobile.Combatant = c = this.m_Mobile.FocusMob;
                    this.m_Mobile.FocusMob = null;
                }
            }

            if (!this.m_Mobile.StunReady && this.m_Mobile.Skills[SkillName.Wrestling].Value >= 80.0 && this.m_Mobile.Skills[SkillName.Anatomy].Value >= 80.0)
                EventSink.InvokeStunRequest(new StunRequestEventArgs(this.m_Mobile));

            if (!this.m_Mobile.InRange(c, this.m_Mobile.RangePerception))
            {
                // They are somewhat far away, can we find something else?
                if (this.AcquireFocusMob(this.m_Mobile.RangePerception, this.m_Mobile.FightMode, false, false, true))
                {
                    this.m_Mobile.Combatant = this.m_Mobile.FocusMob;
                    this.m_Mobile.FocusMob = null;
                }
                else if (!this.m_Mobile.InRange(c, this.m_Mobile.RangePerception * 3))
                {
                    this.m_Mobile.Combatant = null;
                }

                c = this.m_Mobile.Combatant;

                if (c == null)
                {
                    this.m_Mobile.DebugSay("My combatant has fled, so I am on guard");
                    this.Action = ActionType.Guard;

                    return true;
                }
            }

            if (!this.m_Mobile.Controlled && !this.m_Mobile.Summoned && !this.m_Mobile.IsParagon)
            {
                if (this.m_Mobile.Hits < this.m_Mobile.HitsMax * 20 / 100)
                {
                    // We are low on health, should we flee?
                    bool flee = false;

                    if (this.m_Mobile.Hits < c.Hits)
                    {
                        // We are more hurt than them
                        int diff = c.Hits - this.m_Mobile.Hits;

                        flee = (Utility.Random(0, 100) > (10 + diff)); // (10 + diff)% chance to flee
                    }
                    else
                    {
                        flee = Utility.Random(0, 100) > 10; // 10% chance to flee
                    }
					
                    if (flee)
                    {
                        if (this.m_Mobile.Debug)
                            this.m_Mobile.DebugSay("I am going to flee from {0}", c.Name);

                        this.Action = ActionType.Flee;
                        return true;
                    }
                }
            }

            if (this.m_Mobile.Spell == null && DateTime.UtcNow > this.m_NextCastTime && this.m_Mobile.InRange(c, 12))
            {
                // We are ready to cast a spell
                Spell spell = null;
                Mobile toDispel = this.FindDispelTarget(true);

                if (this.m_Mobile.Poisoned) // Top cast priority is cure
                {
                    this.m_Mobile.DebugSay("I am going to cure myself");

                    spell = new CureSpell(this.m_Mobile, null);
                }
                else if (toDispel != null) // Something dispellable is attacking us
                {
                    this.m_Mobile.DebugSay("I am going to dispel {0}", toDispel);

                    spell = this.DoDispel(toDispel);
                }
                else if (c is Mobile && (((Mobile)c).Spell is HealSpell || ((Mobile)c).Spell is GreaterHealSpell) && !((Mobile)c).Poisoned) // They have a heal spell out
                {
                    spell = new BloodOathSpell(this.m_Mobile, null);
                }
                else
                {
                    spell = this.ChooseSpell(c);
                }

                // Now we have a spell picked
                // Move first before casting

                if (toDispel != null)
                {
                    if (this.m_Mobile.InRange(toDispel, 10))
                        this.RunFrom(toDispel);
                    else if (!this.m_Mobile.InRange(toDispel, 12))
                        this.RunTo(toDispel);
                }
                else if(c is Mobile)
                {
                    this.RunTo((Mobile)c);
                }

                if (spell != null)
                    spell.Cast();

                TimeSpan delay;

                if (spell is DispelSpell)
                    delay = TimeSpan.FromSeconds(this.m_Mobile.ActiveSpeed);
                else
                    delay = this.GetDelay();

                this.m_NextCastTime = DateTime.UtcNow + delay;
            }
            else if (c is Mobile && (this.m_Mobile.Spell == null || !this.m_Mobile.Spell.IsCasting))
            {
                this.RunTo((Mobile)c);
            }

            return true;
        }

        public override bool DoActionGuard()
        {
            if (this.AcquireFocusMob(this.m_Mobile.RangePerception, this.m_Mobile.FightMode, false, false, true))
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.DebugSay("I am going to attack {0}", this.m_Mobile.FocusMob.Name);

                this.m_Mobile.Combatant = this.m_Mobile.FocusMob;
                this.Action = ActionType.Combat;
            }
            else
            {
                if (this.m_Mobile.Poisoned)
                {
                    new CureSpell(this.m_Mobile, null).Cast();
                }
                else if (!this.m_Mobile.Summoned && ((this.ScaleByMagery(HealChance) > Utility.RandomDouble())))
                {
                    if (this.m_Mobile.Hits < (this.m_Mobile.HitsMax - 50))
                    {
                        if (!new GreaterHealSpell(this.m_Mobile, null).Cast())
                            new HealSpell(this.m_Mobile, null).Cast();
                    }
                    else if (this.m_Mobile.Hits < (this.m_Mobile.HitsMax - 10))
                    {
                        new HealSpell(this.m_Mobile, null).Cast();
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
            Mobile c = this.m_Mobile.Combatant as Mobile;

            if ((this.m_Mobile.Mana > 20 || this.m_Mobile.Mana == this.m_Mobile.ManaMax) && this.m_Mobile.Hits > (this.m_Mobile.HitsMax / 2))
            {
                this.m_Mobile.DebugSay("I am stronger now, my guard is up");
                this.Action = ActionType.Guard;
            }
            else if (this.AcquireFocusMob(this.m_Mobile.RangePerception, this.m_Mobile.FightMode, false, false, true))
            {
                if (this.m_Mobile.Debug)
                    this.m_Mobile.DebugSay("I am scared of {0}", this.m_Mobile.FocusMob.Name);

                this.RunFrom(this.m_Mobile.FocusMob);
                this.m_Mobile.FocusMob = null;

                if (this.m_Mobile.Poisoned && Utility.Random(0, 5) == 0)
                    new CureSpell(this.m_Mobile, null).Cast();
            }
            else
            {
                this.m_Mobile.DebugSay("Area seems clear, but my guard is up");

                this.Action = ActionType.Guard;
                this.m_Mobile.Warmode = true;
            }

            return true;
        }

        public Mobile FindDispelTarget(bool activeOnly)
        {
            if (this.m_Mobile.Deleted || this.m_Mobile.Int < 95 || this.CanDispel(this.m_Mobile) || this.m_Mobile.AutoDispel)
                return null;

            if (activeOnly)
            {
                List<AggressorInfo> aggressed = this.m_Mobile.Aggressed;
                List<AggressorInfo> aggressors = this.m_Mobile.Aggressors;

                Mobile active = null;
                double activePrio = 0.0;

                Mobile comb = this.m_Mobile.Combatant as Mobile;

                if (comb != null && !comb.Deleted && comb.Alive && !comb.IsDeadBondedPet && this.m_Mobile.InRange(comb, 12) && this.CanDispel(comb))
                {
                    active = comb;
                    activePrio = this.m_Mobile.GetDistanceToSqrt(comb);

                    if (activePrio <= 2)
                        return active;
                }

                for (int i = 0; i < aggressed.Count; ++i)
                {
                    AggressorInfo info = (AggressorInfo)aggressed[i];
                    Mobile m = (Mobile)info.Defender;

                    if (m != comb && m.Combatant == this.m_Mobile && this.m_Mobile.InRange(m, 12) && this.CanDispel(m))
                    {
                        double prio = this.m_Mobile.GetDistanceToSqrt(m);

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

                    if (m != comb && m.Combatant == this.m_Mobile && this.m_Mobile.InRange(m, 12) && this.CanDispel(m))
                    {
                        double prio = this.m_Mobile.GetDistanceToSqrt(m);

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
                Map map = this.m_Mobile.Map;

                if (map != null)
                {
                    Mobile active = null, inactive = null;
                    double actPrio = 0.0, inactPrio = 0.0;

                    Mobile comb = this.m_Mobile.Combatant as Mobile;

                    if (comb != null && !comb.Deleted && comb.Alive && !comb.IsDeadBondedPet && this.CanDispel(comb))
                    {
                        active = inactive = comb;
                        actPrio = inactPrio = this.m_Mobile.GetDistanceToSqrt(comb);
                    }

                    foreach (Mobile m in this.m_Mobile.GetMobilesInRange(12))
                    {
                        if (m != this.m_Mobile && this.CanDispel(m))
                        {
                            double prio = this.m_Mobile.GetDistanceToSqrt(m);

                            if (!activeOnly && (inactive == null || prio < inactPrio))
                            {
                                inactive = m;
                                inactPrio = prio;
                            }

                            if ((this.m_Mobile.Combatant == m || m.Combatant == this.m_Mobile) && (active == null || prio < actPrio))
                            {
                                active = m;
                                actPrio = prio;
                            }
                        }
                    }

                    return active != null ? active : inactive;
                }
            }

            return null;
        }

        public bool CanDispel(Mobile m)
        {
            return (m is BaseCreature && ((BaseCreature)m).Summoned && this.m_Mobile.CanBeHarmful(m, false) && !((BaseCreature)m).IsAnimatedDead);
        }

        private Spell CheckCastHealingSpell()
        {
            // If I'm poisoned, always attempt to cure.
            if (this.m_Mobile.Poisoned)
                return new CureSpell(this.m_Mobile, null);

            // Summoned creatures never heal themselves.
            if (this.m_Mobile.Summoned)
                return null;

            if (this.m_Mobile.Controlled)
            {
                if (DateTime.UtcNow < this.m_NextHealTime)
                    return null;
            }
			
            if (this.ScaleByMagery(HealChance) < Utility.RandomDouble())
                return null;

            Spell spell = null;

            if (this.m_Mobile.Hits < (this.m_Mobile.HitsMax - 50))
            {
                spell = new GreaterHealSpell(this.m_Mobile, null);

                if (spell == null)
                    spell = new HealSpell(this.m_Mobile, null);
            }
            else if (this.m_Mobile.Hits < (this.m_Mobile.HitsMax - 10))
                spell = new HealSpell(this.m_Mobile, null);

            double delay;

            if (this.m_Mobile.Int >= 500)
                delay = Utility.RandomMinMax(7, 10);
            else
                delay = Math.Sqrt(600 - this.m_Mobile.Int);
				
            this.m_Mobile.UseSkill(SkillName.SpiritSpeak);

            this.m_NextHealTime = DateTime.UtcNow + TimeSpan.FromSeconds(delay);

            return spell;
        }

        private TimeSpan GetDelay()
        {
            double del = this.ScaleByMagery(3.0);
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
                toTarget = this.FindDispelTarget(false);

                if (toTarget != null && this.m_Mobile.InRange(toTarget, 10))
                    this.RunFrom(toTarget);
            }
            else if (isParalyze || isTeleport)
            {
                toTarget = this.FindDispelTarget(true);

                if (toTarget == null)
                {
                    toTarget = this.m_Mobile.Combatant as Mobile;

                    if (toTarget != null)
                        this.RunTo(toTarget);
                }
                else if (this.m_Mobile.InRange(toTarget, 10))
                {
                    this.RunFrom(toTarget);
                    teleportAway = true;
                }
                else
                {
                    teleportAway = true;
                }
            }
            else
            {
                toTarget = this.m_Mobile.Combatant as Mobile;

                if (toTarget != null)
                    this.RunTo(toTarget);
            }

            if ((targ.Flags & TargetFlags.Harmful) != 0 && toTarget != null)
            {
                if ((targ.Range == -1 || this.m_Mobile.InRange(toTarget, targ.Range)) && this.m_Mobile.CanSee(toTarget) && this.m_Mobile.InLOS(toTarget))
                {
                    targ.Invoke(this.m_Mobile, toTarget);
                }
                else if (isDispel)
                {
                    targ.Cancel(this.m_Mobile, TargetCancelType.Canceled);
                }
            }
            else if ((targ.Flags & TargetFlags.Beneficial) != 0)
            {
                targ.Invoke(this.m_Mobile, this.m_Mobile);
            }
            else if (isTeleport && toTarget != null)
            {
                Map map = this.m_Mobile.Map;

                if (map == null)
                {
                    targ.Cancel(this.m_Mobile, TargetCancelType.Canceled);
                    return;
                }

                int px, py;

                if (teleportAway)
                {
                    int rx = this.m_Mobile.X - toTarget.X;
                    int ry = this.m_Mobile.Y - toTarget.Y;

                    double d = this.m_Mobile.GetDistanceToSqrt(toTarget);

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

                    if ((targ.Range == -1 || this.m_Mobile.InRange(p, targ.Range)) && this.m_Mobile.InLOS(lt) && map.CanSpawnMobile(px + x, py + y, lt.Z) && !SpellHelper.CheckMulti(p, map))
                    {
                        targ.Invoke(this.m_Mobile, lt);
                        return;
                    }
                }

                int teleRange = targ.Range;

                if (teleRange < 0)
                    teleRange = 12;

                for (int i = 0; i < 10; ++i)
                {
                    Point3D randomPoint = new Point3D(this.m_Mobile.X - teleRange + Utility.Random(teleRange * 2 + 1), this.m_Mobile.Y - teleRange + Utility.Random(teleRange * 2 + 1), 0);

                    LandTarget lt = new LandTarget(randomPoint, map);

                    if (this.m_Mobile.InLOS(lt) && map.CanSpawnMobile(lt.X, lt.Y, lt.Z) && !SpellHelper.CheckMulti(randomPoint, map))
                    {
                        targ.Invoke(this.m_Mobile, new LandTarget(randomPoint, map));
                        return;
                    }
                }

                targ.Cancel(this.m_Mobile, TargetCancelType.Canceled);
            }
            else
            {
                targ.Cancel(this.m_Mobile, TargetCancelType.Canceled);
            }
        }
    }
}