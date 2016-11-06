using System;
using System.Collections.Generic;
using Server.Items;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.First;
using Server.Spells.Fourth;
using Server.Spells.Second;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Third;
using Server.Spells.Eighth;
using Server.Spells.Necromancy;
using Server.Spells.Mystic;
using Server.Spells.Spellweaving;
using Server.Targeting;

namespace Server.Mobiles
{
    public class MageAI : BaseAI
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

        protected const double HealChance = 0.10;// 10% chance to heal at gm magery
        protected const double TeleportChance = 0.05;// 5% chance to teleport at gm magery
        protected const double DispelChance = 0.75;// 75% chance to dispel at gm magery

        private DateTime m_NextCastTime;
        private DateTime m_NextHealTime;
        private Mobile m_LastTarget;
        private Point3D m_LastTargetLoc;
        private LandTarget m_RevealTarget;

        public DateTime NextCastTime { get { return m_NextCastTime; } set { m_NextCastTime = value; } }

        public MageAI(BaseCreature m)
            : base(m)
        {
        }

        public virtual bool SmartAI
        {
            get
            {
                return m_Mobile.UseSmartAI;
            }
        }

        public override bool Think()
        {
            if (this.m_Mobile.Deleted)
                return false;

            if (this.ProcessTarget())
                return true;
            else
                return base.Think();
        }

        public virtual double ScaleByMagery(double v)
        {
            return m_Mobile.Skills[SkillName.Magery].Value * v * 0.01;
        }

        public virtual double ScaleBySkill(double v, SkillName skill)
        {
            return v * this.m_Mobile.Skills[skill].Value / 100;
        }

        public override bool DoActionWander()
        {
            if (this.AcquireFocusMob(this.m_Mobile.RangePerception, this.m_Mobile.FightMode, false, false, true))
            {
                this.m_Mobile.DebugSay("I am going to attack {0}", this.m_Mobile.FocusMob.Name);

                this.m_Mobile.Combatant = this.m_Mobile.FocusMob;
                this.Action = ActionType.Combat;
                this.m_NextCastTime = DateTime.UtcNow;
            }
            else if (this.SmartAI && this.m_Mobile.Mana < this.m_Mobile.ManaMax && !this.m_Mobile.Meditating)
            {
                this.m_Mobile.DebugSay("I am going to meditate");

                this.m_Mobile.UseSkill(SkillName.Meditation);
            }
            else
            {
                this.m_Mobile.DebugSay("I am wandering");

                this.m_Mobile.Warmode = false;

                base.DoActionWander();

                if (Utility.RandomDouble() < 0.05)
                {
                    Spell spell = this.CheckCastHealingSpell();

                    if (spell != null)
                        spell.Cast();
                }
            }

            return true;
        }

        public void RunTo(IDamageable d)
        {
            if (!this.SmartAI)
            {
                if (!this.MoveTo(d, true, this.m_Mobile.RangeFight))
                    this.OnFailedMove();

                return;
            }

            if (d is Mobile && (((Mobile)d).Paralyzed || ((Mobile)d).Frozen))
            {
                if (this.m_Mobile.InRange(d, 1))
                    this.RunFrom(d);
                else if (!this.m_Mobile.InRange(d, this.m_Mobile.RangeFight > 2 ? this.m_Mobile.RangeFight : 2) && !this.MoveTo(d, true, 1))
                    this.OnFailedMove();
            }
            else
            {
                if (!this.m_Mobile.InRange(d, this.m_Mobile.RangeFight))
                {
                    if (!this.MoveTo(d, true, 1))
                        this.OnFailedMove();
                }
                else if (this.m_Mobile.InRange(d, this.m_Mobile.RangeFight - 1))
                {
                    this.RunFrom(d);
                }
            }
        }

        public void RunFrom(IDamageable d)
        {
            this.Run((Direction)((int)this.m_Mobile.GetDirectionTo(d) - 4) & Direction.Mask);
        }

        public void OnFailedMove()
        {
            if (!m_Mobile.DisallowAllMoves && (SmartAI ? Utility.Random(10) == 0 : ScaleByMagery(TeleportChance) > Utility.RandomDouble()))
            {
                if (this.m_Mobile.Target != null)
                    this.m_Mobile.Target.Cancel(this.m_Mobile, TargetCancelType.Canceled);

                new TeleportSpell(this.m_Mobile, null).Cast();

                this.m_Mobile.DebugSay("I am stuck, I'm going to try teleporting away");
            }
            else if (this.AcquireFocusMob(this.m_Mobile.RangePerception, this.m_Mobile.FightMode, false, false, true))
            {
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

        public virtual int GetMaxCircle()
        {
            return (int)((m_Mobile.Skills[SkillName.Magery].Value + 20.0) / (100.0 / 7.0));
        }

        public virtual Spell GetRandomDamageSpell()
        {
            int maxCircle = GetMaxCircle() * 2;

            if (maxCircle < 1) maxCircle = 1;
            if (maxCircle > 11) maxCircle = 12;

            switch (Utility.Random(maxCircle))
            {
                case 0:
                case 1: return new MagicArrowSpell(m_Mobile, null);
                case 2:
                case 3: return new HarmSpell(m_Mobile, null);
                case 4:
                case 5: return new FireballSpell(m_Mobile, null);
                case 6:
                case 7: return new LightningSpell(m_Mobile, null);
                case 8:
                case 9: return new MindBlastSpell(m_Mobile, null);
                case 10: return new EnergyBoltSpell(m_Mobile, null);
                case 11: return new ExplosionSpell(m_Mobile, null);
                default: return new FlameStrikeSpell(m_Mobile, null);
            }
        }

        public virtual Spell GetRandomCurseSpell()
        {
            if (Utility.RandomBool() && GetMaxCircle() >= 5)
                return new CurseSpell(m_Mobile, null);

            switch (Utility.Random(3))
            {
                default:
                case 0: return new WeakenSpell(m_Mobile, null);
                case 1: return new ClumsySpell(m_Mobile, null);
                case 2: return new FeeblemindSpell(m_Mobile, null);
            }
        }

        public virtual Spell GetRandomManaDrainSpell()
        {
            int maxCircle = GetMaxCircle();
            if (Utility.RandomBool() && maxCircle >= 7)
                return new ManaVampireSpell(m_Mobile, null);
            else if (maxCircle >= 4)
                return new ManaDrainSpell(m_Mobile, null);

            return null;
        }

        public virtual Spell GetRandomSummonSpell()
        {
            if (GetMaxCircle() >= 8)
                return new EnergyVortexSpell(m_Mobile, null);
            else if (GetMaxCircle() >= 5)
                return new BladeSpiritsSpell(m_Mobile, null);

            return null;
        }

        public virtual Spell GetRandomFieldSpell()
        {
            int maxCircle = GetMaxCircle();
            bool pois = m_Mobile.Skills[SkillName.Poisoning].Value >= 80.0 || m_Mobile.HitPoison == Poison.Greater || m_Mobile.HitPoison == Poison.Lethal;

            if (pois && maxCircle >= 5)
                return new PoisonFieldSpell(m_Mobile, null);

            else if (Utility.RandomBool() && maxCircle >= 6)
                return new ParalyzeFieldSpell(m_Mobile, null);
            else if (maxCircle >= 4)
                return new FireFieldSpell(m_Mobile, null);

            return null;
        }

        public virtual Spell GetRandomBuffSpell()
        {
            return new BlessSpell(m_Mobile, null);
        }

        public virtual Spell DoDispel(Mobile toDispel)
        {
            if (!this.SmartAI)
            {
                if (ScaleByMagery(DispelChance) > Utility.RandomDouble())
                    return new DispelSpell(m_Mobile, null);

                return this.ChooseSpell(toDispel);
            }

            Spell spell = this.CheckCastHealingSpell();

            if (spell == null)
            {
                if (!this.m_Mobile.DisallowAllMoves && Utility.Random((int)this.m_Mobile.GetDistanceToSqrt(toDispel)) == 0)
                    spell = new TeleportSpell(this.m_Mobile, null);
                else if (Utility.Random(3) == 0 && !this.m_Mobile.InRange(toDispel, 3) && !toDispel.Paralyzed && !toDispel.Frozen)
                    spell = new ParalyzeSpell(this.m_Mobile, null);
                else
                    spell = new DispelSpell(this.m_Mobile, null);
            }

            return spell;
        }

        public virtual Spell ChooseSpell(IDamageable d)
        {
            if (!(d is Mobile))
            {
                m_Mobile.DebugSay("Just doing damage");
                return GetRandomDamageSpell();
            }

            Mobile c = d as Mobile;
            Spell spell = null;

            if (!this.SmartAI)
            {
                spell = this.CheckCastHealingSpell();

                if (spell == null && m_Mobile.RawInt >= 80)
                    spell = CheckCastDispelField();

                if (spell != null)
                    return spell;

                int maxCircle = GetMaxCircle();

                switch (Utility.Random(15))
                {
                    case 0:
                    case 1:	// Poison them
                        {
                            if (c.Poisoned)
                                goto default;

                            this.m_Mobile.DebugSay("Attempting to poison");

                            spell = new PoisonSpell(this.m_Mobile, null);
                            break;
                        }
                    case 2:	// Bless ourselves
                        {
                            this.m_Mobile.DebugSay("Blessing myself");

                            spell = GetRandomBuffSpell();//new BlessSpell(this.m_Mobile, null);
                            break;
                        }
                    case 3:
                    case 4: // Curse them
                        {
                            this.m_Mobile.DebugSay("Attempting to curse");

                            spell = this.GetRandomCurseSpell();
                            break;
                        }
                    case 5:	// Paralyze them
                        {
                            this.m_Mobile.DebugSay("Attempting to paralyze");

                            if (maxCircle >= 5)
                                spell = new ParalyzeSpell(m_Mobile, null);
                            else
                                spell = GetRandomCurseSpell();
                            break;
                        }
                    case 6: // Drain mana
                        {
                            this.m_Mobile.DebugSay("Attempting to drain mana");

                            spell = this.GetRandomManaDrainSpell();
                            break;
                        }
                    default: // Damage them
                        {
                            this.m_Mobile.DebugSay("Just doing damage");

                            spell = this.GetRandomDamageSpell();
                            break;
                        }
                }

                return spell;
            }

            if (m_Mobile.Hidden)
                return null;

            spell = CheckCastDispelField();

            if (spell == null)
                spell = CheckCastHealingSpell();

            if (spell == null && 0.05 >= Utility.RandomDouble())
                spell = GetRandomBuffSpell();

            else if (spell == null && m_Mobile.Followers + 1 < m_Mobile.FollowersMax && 0.05 >= Utility.RandomDouble())
                spell = GetRandomSummonSpell();

            else if (spell == null && 0.05 >= Utility.RandomDouble())
                spell = GetRandomFieldSpell();

            else if (spell == null && 0.05 >= Utility.RandomDouble())
                spell = GetRandomManaDrainSpell();

            if (spell != null)
                return spell;

            switch( Utility.Random(3) )
            {
                case 0: // Poison them
                    {
                        if (c.Poisoned)
                            goto case 1;

                        spell = new PoisonSpell(this.m_Mobile, null);
                        break;
                    }
                case 1: // Deal some damage
                    {
                        spell = this.GetRandomDamageSpell();

                        break;
                    }
                default: // Set up a combo
                    {
                        if (this.m_Mobile.Mana > 15 && this.m_Mobile.Mana < 40)
                        {
                            if (c.Paralyzed && !c.Poisoned && !this.m_Mobile.Meditating)
                            {
                                this.m_Mobile.DebugSay("I am going to meditate");

                                this.m_Mobile.UseSkill(SkillName.Meditation);
                            }
                            else if (!c.Poisoned)
                            {
                                spell = new ParalyzeSpell(this.m_Mobile, null);
                            }
                        }
                        else if (this.m_Mobile.Mana > 60)
                        {
                            if (Utility.RandomBool() && !c.Paralyzed && !c.Frozen && !c.Poisoned)
                            {
                                this.m_Combo = 0;
                                spell = new ParalyzeSpell(this.m_Mobile, null);
                            }
                            else
                            {
                                this.m_Combo = 1;
                                spell = new ExplosionSpell(this.m_Mobile, null);
                            }
                        }

                        break;
                    }
            }

            return spell;
        }

        protected int m_Combo = -1;
        protected ComboType m_ComboType;

        protected enum ComboType
        {
            None,
            Exp_FS_Poison,
            Exp_MB_Poison,
            Exp_EB_Poison,
            Exp_FB_MA_Poison,
            Exp_FB_Poison_Light,
            Exp_FB_MA_Light,
            Exp_Poison_FB_Light,
        }

        public virtual Spell DoCombo(Mobile c)
        {
            Spell spell = null;

            if (m_ComboType == ComboType.None)
                m_ComboType = (ComboType)Utility.RandomMinMax(1, 7);

            if (m_Combo == 1)
            {
                switch (m_ComboType)
                {
                    case ComboType.Exp_FS_Poison:
                    case ComboType.Exp_MB_Poison:
                    case ComboType.Exp_EB_Poison:
                    case ComboType.Exp_FB_MA_Poison:
                    case ComboType.Exp_FB_Poison_Light:
                    case ComboType.Exp_FB_MA_Light:
                    case ComboType.Exp_Poison_FB_Light: spell = new ExplosionSpell(m_Mobile, null); break;
                }
            }
            else if (m_Combo == 2)
            {
                switch (m_ComboType)
                {
                    case ComboType.Exp_FS_Poison: spell = new FlameStrikeSpell(m_Mobile, null); break;
                    case ComboType.Exp_MB_Poison: spell = new MindBlastSpell(m_Mobile, null); break;
                    case ComboType.Exp_EB_Poison: spell = new EnergyBoltSpell(m_Mobile, null); break;
                    case ComboType.Exp_FB_MA_Poison: spell = new FireballSpell(m_Mobile, null); break;
                    case ComboType.Exp_FB_Poison_Light: spell = new FireballSpell(m_Mobile, null); break;
                    case ComboType.Exp_FB_MA_Light: spell = new FireballSpell(m_Mobile, null); break;
                    case ComboType.Exp_Poison_FB_Light: spell = new PoisonSpell(m_Mobile, null); break;
                }
            }
            else if (m_Combo == 3)
            {
                switch (m_ComboType)
                {
                    case ComboType.Exp_FS_Poison:
                    case ComboType.Exp_MB_Poison:
                    case ComboType.Exp_EB_Poison:
                        spell = new PoisonSpell(m_Mobile, null);
                        EndCombo();
                        return spell;
                    case ComboType.Exp_FB_MA_Poison: spell = new MagicArrowSpell(m_Mobile, null); break;
                    case ComboType.Exp_FB_Poison_Light: spell = new PoisonSpell(m_Mobile, null); break;
                    case ComboType.Exp_FB_MA_Light: spell = new MagicArrowSpell(m_Mobile, null); break;
                    case ComboType.Exp_Poison_FB_Light: spell = new FireballSpell(m_Mobile, null); break;
                }
            }
            else if (m_Combo == 4)
            {
                switch (m_ComboType)
                {
                    case ComboType.Exp_FS_Poison:
                    case ComboType.Exp_MB_Poison:
                    case ComboType.Exp_EB_Poison:
                        spell = new LightningSpell(m_Mobile, null);
                        EndCombo();
                        return spell;
                    case ComboType.Exp_FB_MA_Poison: spell = new PoisonSpell(m_Mobile, null); break;
                    case ComboType.Exp_FB_Poison_Light:
                    case ComboType.Exp_FB_MA_Light:
                    case ComboType.Exp_Poison_FB_Light: spell = new LightningSpell(m_Mobile, null);
                        EndCombo();
                        return spell;
                }
            }
            else if (m_Combo == 5)
            {
                switch (m_ComboType)
                {
                    case ComboType.Exp_FS_Poison:
                    case ComboType.Exp_MB_Poison:
                    case ComboType.Exp_EB_Poison:
                    case ComboType.Exp_FB_MA_Poison:
                    case ComboType.Exp_FB_Poison_Light:
                    case ComboType.Exp_FB_MA_Light:
                    case ComboType.Exp_Poison_FB_Light:
                        spell = new LightningSpell(m_Mobile, null);
                        EndCombo();
                        return spell;
                }
            }

            m_Combo++; // Move to next spell

            if (spell == null)
                spell = new PoisonSpell(m_Mobile, null);

            return spell;
        }

        public virtual void EndCombo()
        {
            m_ComboType = ComboType.None;
            m_Combo = -1;
        }

        protected TimeSpan GetDelay()
        {
            double del = ScaleByMagery(3.0);
            double min = 6.0 - (del * 0.75);
            double max = 6.0 - (del * 1.25);

            return TimeSpan.FromSeconds(min + ((max - min) * Utility.RandomDouble()));
        }

        public override bool DoActionCombat()
        {
            IDamageable c = this.m_Mobile.Combatant;
            m_Mobile.Warmode = true;

            if (m_Mobile.Target != null)
                ProcessTarget();

            if (c == null || c.Deleted || !c.Alive || (c is Mobile && ((Mobile)c).IsDeadBondedPet) || !this.m_Mobile.CanSee(c) || !this.m_Mobile.CanBeHarmful(c, false) || c.Map != this.m_Mobile.Map)
            {
                // Our combatant is deleted, dead, hidden, or we cannot hurt them
                // Try to find another combatant
                if (this.AcquireFocusMob(this.m_Mobile.RangePerception, this.m_Mobile.FightMode, false, false, true))
                {
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
                this.m_Mobile.DebugSay("I can't see my target");

                if (this.AcquireFocusMob(this.m_Mobile.RangePerception, this.m_Mobile.FightMode, false, false, true))
                {
                    this.m_Mobile.DebugSay("I will switch to {0}", this.m_Mobile.FocusMob.Name);
                    this.m_Mobile.Combatant = c = this.m_Mobile.FocusMob;
                    this.m_Mobile.FocusMob = null;
                }
            }

            if (!Core.AOS && this.SmartAI && !this.m_Mobile.StunReady && this.m_Mobile.Skills[SkillName.Wrestling].Value >= 80.0 && this.m_Mobile.Skills[SkillName.Anatomy].Value >= 80.0)
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

                c = this.m_Mobile.Combatant as Mobile;

                if (c == null)
                {
                    this.m_Mobile.DebugSay("My combatant has fled, so I am on guard");
                    this.Action = ActionType.Guard;

                    return true;
                }
            }

            if (!this.m_Mobile.Controlled && !this.m_Mobile.Summoned && this.m_Mobile.CanFlee)
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
                        this.m_Mobile.DebugSay("I am going to flee from {0}", c.Name);

                        this.Action = ActionType.Flee;
                        return true;
                    }
                }
            }

            if (this.m_Mobile.Spell == null && DateTime.UtcNow > this.m_NextCastTime && this.m_Mobile.InRange(c, Core.ML ? 10 : 12))
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
                else if (c is Mobile && this.SmartAI && this.m_Combo != -1) // We are doing a spell combo
                {
                    spell = this.DoCombo((Mobile)c);
                }
                else if (c is Mobile && this.SmartAI && (((Mobile)c).Spell is HealSpell || ((Mobile)c).Spell is GreaterHealSpell) && !((Mobile)c).Poisoned) // They have a heal spell out
                {
                    spell = new PoisonSpell(this.m_Mobile, null);
                }
                else
                {
                    spell = this.ChooseSpell(c);
                }

                // Now we have a spell picked
                // Move first before casting

                TimeSpan ts = !SmartAI && !(spell is DispelSpell) ? TimeSpan.FromSeconds(1.5) : m_Combo > -1 ? TimeSpan.FromSeconds(.5) : TimeSpan.FromSeconds(1.5);
                TimeSpan delay = spell == null ? TimeSpan.FromSeconds(m_Mobile.ActiveSpeed) : spell.GetCastDelay() + spell.GetCastRecovery() + ts;

                RunTo(c);

                if (spell != null)
                    spell.Cast();

                this.m_NextCastTime = DateTime.UtcNow + delay;
            }
            else/* if (this.m_Mobile.Spell == null || !this.m_Mobile.Spell.IsCasting)*/
            {
                this.RunTo(c);
            }

            this.m_LastTarget = c as Mobile;
            this.m_LastTargetLoc = c.Location;

            return true;
        }

        public override bool DoActionGuard()
        {
            if (this.m_LastTarget != null && this.m_LastTarget.Hidden)
            {
                Map map = this.m_Mobile.Map;

                if (map == null || !this.m_Mobile.InRange(this.m_LastTargetLoc, Core.ML ? 10 : 12))
                {
                    this.m_LastTarget = null;
                }
                else if (this.m_Mobile.Spell == null && DateTime.UtcNow > this.m_NextCastTime)
                {
                    this.m_Mobile.DebugSay("I am going to reveal my last target");

                    this.m_RevealTarget = new LandTarget(this.m_LastTargetLoc, map);
                    Spell spell = new RevealSpell(this.m_Mobile, null);

                    if (spell.Cast())
                        this.m_LastTarget = null; // only do it once

                    this.m_NextCastTime = DateTime.UtcNow + this.GetDelay(spell);
                }
            }

            if (this.AcquireFocusMob(this.m_Mobile.RangePerception, this.m_Mobile.FightMode, false, false, true))
            {
                this.m_Mobile.DebugSay("I am going to attack {0}", this.m_Mobile.FocusMob.Name);

                this.m_Mobile.Combatant = this.m_Mobile.FocusMob;
                this.Action = ActionType.Combat;
            }
            else
            {
                if (!this.m_Mobile.Controlled)
                {
                    this.ProcessTarget();

                    Spell spell = this.CheckCastHealingSpell();

                    if (spell != null)
                        spell.Cast();
                }

                base.DoActionGuard();
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

                if (comb != null && !comb.Deleted && comb.Alive && !comb.IsDeadBondedPet && this.m_Mobile.InRange(comb, Core.ML ? 10 : 12) && this.CanDispel(comb))
                {
                    active = comb;
                    activePrio = this.m_Mobile.GetDistanceToSqrt(comb);

                    if (activePrio <= 2)
                        return active;
                }

                for (int i = 0; i < aggressed.Count; ++i)
                {
                    AggressorInfo info = aggressed[i];
                    Mobile m = info.Defender;

                    if (m != comb && m.Combatant == this.m_Mobile && this.m_Mobile.InRange(m, Core.ML ? 10 : 12) && this.CanDispel(m))
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
                    AggressorInfo info = aggressors[i];
                    Mobile m = info.Attacker;

                    if (m != comb && m.Combatant == this.m_Mobile && this.m_Mobile.InRange(m, Core.ML ? 10 : 12) && this.CanDispel(m))
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

                    foreach (Mobile m in this.m_Mobile.GetMobilesInRange(Core.ML ? 10 : 12))
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

        protected Spell CheckCastHealingSpell()
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

            if (!this.SmartAI)
            {
                if (ScaleByMagery(HealChance) < Utility.RandomDouble())
                    return null;
            }
            else
            {
                if (Utility.Random(0, 4 + (this.m_Mobile.Hits == 0 ? this.m_Mobile.HitsMax : (this.m_Mobile.HitsMax / this.m_Mobile.Hits))) < 3)
                    return null;
            }

            Spell spell = null;

            if (this.m_Mobile.Hits < (this.m_Mobile.HitsMax - 50))
            {
                spell = new GreaterHealSpell(this.m_Mobile, null);

                if (spell == null)
                    spell = new HealSpell(this.m_Mobile, null);
            }
            else if (this.m_Mobile.Hits < (this.m_Mobile.HitsMax - 10))
            {
                spell = new HealSpell(this.m_Mobile, null);
            }

            double delay;

            if (this.m_Mobile.Int >= 500)
                delay = Utility.RandomMinMax(7, 10);
            else
                delay = Math.Sqrt(600 - this.m_Mobile.Int);

            this.m_NextHealTime = DateTime.UtcNow + TimeSpan.FromSeconds(delay);

            return spell;
        }

        public Spell CheckCastDispelField()
        {
            if (m_Mobile.Frozen || m_Mobile.Paralyzed)
                return null;

            int maxCircle = GetMaxCircle();

            if (maxCircle < 5)
                return null;

            Item field = GetHarmfulFieldItem();

            if (field != null)
            {
                m_Mobile.DebugSay("Found harmful field, attempting to dispel");
                return new DispelFieldSpell(m_Mobile, null);
            }

            return null;
        }

        public Item GetHarmfulFieldItem()
        {
            IPooledEnumerable eable = m_Mobile.Map.GetItemsInRange(m_Mobile.Location, 0);

            foreach (Item item in eable)
            {
                if (item is PoisonFieldSpell.InternalItem)
                {
                    PoisonFieldSpell.InternalItem field = (PoisonFieldSpell.InternalItem)item;

                    if (field.Visible && field.Caster != null && (!Core.AOS || m_Mobile != field.Caster) && SpellHelper.ValidIndirectTarget(field.Caster, m_Mobile) && field.Caster.CanBeHarmful(m_Mobile, false))
                    {
                        eable.Free();
                        return item;
                    }
                }
                else if (item is ParalyzeFieldSpell.InternalItem)
                {
                    ParalyzeFieldSpell.InternalItem field = (ParalyzeFieldSpell.InternalItem)item;

                    if (field.Visible && field.Caster != null && (!Core.AOS || m_Mobile != field.Caster) && SpellHelper.ValidIndirectTarget(field.Caster, m_Mobile) && field.Caster.CanBeHarmful(m_Mobile, false))
                    {
                        eable.Free();
                        return item;
                    }
                }
                else if (item is FireFieldSpell.FireFieldItem)
                {
                    FireFieldSpell.FireFieldItem field = (FireFieldSpell.FireFieldItem)item;

                    if (field.Visible && field.Caster != null && (!Core.AOS || m_Mobile != field.Caster) && SpellHelper.ValidIndirectTarget(field.Caster, m_Mobile) && field.Caster.CanBeHarmful(m_Mobile, false))
                    {
                        eable.Free();
                        return item;
                    }
                }
            }

            eable.Free();
            return null;
        }

        private TimeSpan GetDelay(Spell spell)
        {
            if (this.SmartAI || (spell is DispelSpell))
            {
                return TimeSpan.FromSeconds(this.m_Mobile.ActiveSpeed);
            }
            else
            {
                double del = this.ScaleBySkill(3.0, SkillName.Magery);
                double min = 6.0 - (del * 0.75);
                double max = 6.0 - (del * 1.25);

                return TimeSpan.FromSeconds(min + ((max - min) * Utility.RandomDouble()));
            }
        }

        protected virtual bool ProcessTarget()
        {
            Target targ = m_Mobile.Target;

            if (targ == null)
                return false;

            bool isDispel = (targ is DispelSpell.InternalTarget || targ is MassDispelSpell.InternalTarget);
            bool isParalyze = (targ is ParalyzeSpell.InternalTarget);
            bool isTeleport = (targ is TeleportSpell.InternalTarget);
            bool isSummon = (targ is EnergyVortexSpell.InternalTarget || targ is BladeSpiritsSpell.InternalTarget || targ is NatureFurySpell.InternalTarget);
            bool isField = (targ is FireFieldSpell.InternalTarget || targ is PoisonFieldSpell.InternalTarget || targ is ParalyzeFieldSpell.InternalTarget);
            bool isAnimate = (targ is AnimateDeadSpell.InternalTarget);
            bool isDispelField = (targ is DispelFieldSpell.InternalTarget);
            bool teleportAway = false;
            bool harmful = (targ.Flags & TargetFlags.Harmful) != 0 || targ is HailStormSpell.InternalTarget || targ is WildfireSpell.InternalTarget;
            bool beneficial = (targ.Flags & TargetFlags.Beneficial) != 0 || targ is ArchCureSpell.InternalTarget;

            if (isTeleport && m_Mobile.CanSwim)
                targ.Cancel(m_Mobile, TargetCancelType.Canceled);

            IDamageable toTarget = null;

            if (isDispel)
            {
                toTarget = FindDispelTarget(false);

                if (toTarget != null)
                    RunTo(toTarget);
            }
            else if (isDispelField)
            {
                Item field = GetHarmfulFieldItem();

                if (field != null)
                    targ.Invoke(m_Mobile, field);
                else
                    targ.Cancel(m_Mobile, TargetCancelType.Canceled);
            }
            else if (SmartAI && (isParalyze || isTeleport))
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
            else if (isAnimate)
            {
                Item corpse = FindCorpseToAnimate();

                if (corpse != null)
                    targ.Invoke(m_Mobile, corpse);
            }
            else
            {
                toTarget = m_Mobile.Combatant;

                if (toTarget != null)
                    RunTo(toTarget);
            }

            if (isSummon && toTarget != null)
            {
                int failSafe = 0;
                Map map = toTarget.Map;

                while (failSafe <= 25)
                {
                    int x = Utility.RandomMinMax(toTarget.X - 2, toTarget.X + 2);
                    int y = Utility.RandomMinMax(toTarget.Y - 2, toTarget.Y + 2);
                    int z = toTarget.Z;

                    LandTarget lt = new LandTarget(new Point3D(x, y, z), map);

                    if (map.CanSpawnMobile(x, y, z))
                    {
                        targ.Invoke(m_Mobile, lt);
                        break;
                    }

                    failSafe++;
                }
            }
            else if (isField && toTarget != null)
            {
                Map map = toTarget.Map;

                int x = m_Mobile.X;
                int y = m_Mobile.Y;
                int z = m_Mobile.Z;

                if (toTarget == null || m_Mobile.InRange(toTarget.Location, 3))
                {
                    targ.Invoke(m_Mobile, toTarget);
                    return true;
                }

                Direction d = Utility.GetDirection(m_Mobile, toTarget);
                int dist = (int)m_Mobile.GetDistanceToSqrt(toTarget.Location) / 2;
                Point3D p = m_Mobile.Location;

                switch ((int)d)
                {
                    case (int)Direction.Running:
                    case (int)Direction.North:
                        y = p.Y - dist;
                        break;
                    case 129:
                    case (int)Direction.Right:
                        x = p.X + dist;
                        y = p.Y - dist;
                        break;
                    case 130:
                    case (int)Direction.East:
                        x = p.X + dist;
                        break;
                    case 131:
                    case (int)Direction.Down:
                        x = p.X + dist;
                        y = p.Y + dist;
                        break;
                    case 132:
                    case (int)Direction.South:
                        y = p.Y + dist;
                        break;
                    case 133:
                    case (int)Direction.Left:
                        x = p.X - dist;
                        y = p.Y + dist;
                        break;
                    case 134:
                    case (int)Direction.West:
                        x = p.X - dist;
                        break;
                    case (int)Direction.ValueMask:
                    case (int)Direction.Up:
                        x = p.X - dist;
                        y = p.Y - dist;
                        break;
                }

                LandTarget lt = new LandTarget(new Point3D(x, y, z), map);
                targ.Invoke(m_Mobile, lt);
            }

            if (harmful && toTarget != null)
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
            else if (beneficial)
            {
                targ.Invoke(m_Mobile, m_Mobile);
            }
            else if (isTeleport && toTarget != null)
            {
                Map map = m_Mobile.Map;

                if (map == null)
                {
                    targ.Cancel(m_Mobile, TargetCancelType.Canceled);
                    return true;
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
                        return true;
                    }
                }

                int teleRange = targ.Range;

                if (teleRange < 0)
                    teleRange = Core.ML ? 11 : 12;

                for (int i = 0; i < 10; ++i)
                {
                    Point3D randomPoint = new Point3D(m_Mobile.X - teleRange + Utility.Random(teleRange * 2 + 1), m_Mobile.Y - teleRange + Utility.Random(teleRange * 2 + 1), 0);

                    LandTarget lt = new LandTarget(randomPoint, map);

                    if (m_Mobile.InLOS(lt) && map.CanSpawnMobile(lt.X, lt.Y, lt.Z) && !SpellHelper.CheckMulti(randomPoint, map))
                    {
                        targ.Invoke(m_Mobile, new LandTarget(randomPoint, map));
                        return true;
                    }
                }

                targ.Cancel(m_Mobile, TargetCancelType.Canceled);
            }
            else
            {
                targ.Cancel(m_Mobile, TargetCancelType.Canceled);
            }

            return true;
        }

        public Item FindCorpseToAnimate()
        {
            IPooledEnumerable eable = m_Mobile.GetItemsInRange(12);
            foreach (Item item in eable)
            {
                Corpse c = item as Corpse;

                if (c != null)
                {
                    Type type = null;

                    if (c.Owner != null)
                        type = c.Owner.GetType();

                    BaseCreature owner = c.Owner as BaseCreature;

                    if ((c.ItemID < 0xECA || c.ItemID > 0xED5) && m_Mobile.InLOS(c) && !c.Channeled && type != typeof(PlayerMobile) && type != null && (owner == null || (!owner.Summoned && !owner.IsBonded)))
                    {
                        eable.Free();
                        return item;
                    }
                }
            }
            eable.Free();
            return null;
        }
    }
}