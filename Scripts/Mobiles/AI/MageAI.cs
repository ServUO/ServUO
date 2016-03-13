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
    public class MageAI : BaseAI
    {
        protected int m_Combo = -1;
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
        private const double HealChance = 0.10;// 10% chance to heal at gm magery
        private const double TeleportChance = 0.05;// 5% chance to teleport at gm magery
        private const double DispelChance = 0.75;// 75% chance to dispel at gm magery
        private DateTime m_NextCastTime;
        private DateTime m_NextHealTime;
        private Mobile m_LastTarget;
        private Point3D m_LastTargetLoc;
        private LandTarget m_RevealTarget;
        public MageAI(BaseCreature m)
            : base(m)
        {
        }

        public virtual bool SmartAI
        {
            get
            {
                return (this.m_Mobile is BaseVendor || this.m_Mobile is BaseEscortable || this.m_Mobile is Changeling);
            }
        }
        public virtual bool IsNecromancer
        {
            get
            {
                return (Core.AOS && this.m_Mobile.Skills[SkillName.Necromancy].Value > 50);
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

        public void RunTo(Mobile m)
        {
            if (!this.SmartAI)
            {
                if (!this.MoveTo(m, true, this.m_Mobile.RangeFight))
                    this.OnFailedMove();

                return;
            }

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

        public void RunFrom(Mobile m)
        {
            this.Run((Direction)((int)this.m_Mobile.GetDirectionTo(m) - 4) & Direction.Mask);
        }

        public void OnFailedMove()
        {
            if (!this.m_Mobile.DisallowAllMoves && (this.SmartAI ? Utility.Random(4) == 0 : this.ScaleBySkill(TeleportChance, SkillName.Magery) > Utility.RandomDouble()))
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

        public virtual bool UseNecromancy()
        {
            if (this.IsNecromancer)
                return (Utility.Random(this.m_Mobile.Skills[SkillName.Magery].BaseFixedPoint + this.m_Mobile.Skills[SkillName.Necromancy].BaseFixedPoint) >= this.m_Mobile.Skills[SkillName.Magery].BaseFixedPoint);

            return false;
        }

        public virtual Spell GetRandomDamageSpell()
        {
            return this.UseNecromancy() ? this.GetRandomDamageSpellNecro() : this.GetRandomDamageSpellMage();
        }

        public virtual Spell GetRandomDamageSpellNecro()
        {
            int bound = (this.m_Mobile.Skills[SkillName.Necromancy].Value >= 100) ? 5 : 3;

            switch( Utility.Random(bound) )
            {
                case 0:
                    this.m_Mobile.DebugSay("Pain Spike");
                    return new PainSpikeSpell(this.m_Mobile, null);
                case 1:
                    this.m_Mobile.DebugSay("Poison Strike");
                    return new PoisonStrikeSpell(this.m_Mobile, null);
                case 2:
                    this.m_Mobile.DebugSay("Strangle");
                    return new StrangleSpell(this.m_Mobile, null);
                case 3:
                    this.m_Mobile.DebugSay("Wither");
                    return new WitherSpell(this.m_Mobile, null);
                default:
                    this.m_Mobile.DebugSay("Vengeful Spirit");
                    return new VengefulSpiritSpell(this.m_Mobile, null);
            }
        }

        public virtual Spell GetRandomDamageSpellMage()
        {
            int maxCircle = (int)((this.m_Mobile.Skills[SkillName.Magery].Value + 20.0) / (100.0 / 7.0));

            if (maxCircle < 1)
                maxCircle = 1;
            else if (maxCircle > 8)
                maxCircle = 8;

            switch( Utility.Random(maxCircle * 2) )
            {
                case 0:
                case 1:
                    return new MagicArrowSpell(this.m_Mobile, null);
                case 2:
                case 3:
                    return new HarmSpell(this.m_Mobile, null);
                case 4:
                case 5:
                    return new FireballSpell(this.m_Mobile, null);
                case 6:
                case 7:
                    return new LightningSpell(this.m_Mobile, null);
                case 8:
                case 9:
                    return new MindBlastSpell(this.m_Mobile, null);
                case 10:
                    return new EnergyBoltSpell(this.m_Mobile, null);
                case 11:
                    return new ExplosionSpell(this.m_Mobile, null);
                default:
                    return new FlameStrikeSpell(this.m_Mobile, null);
            }
        }

        public virtual Spell GetRandomCurseSpell()
        {
            return this.UseNecromancy() ? this.GetRandomCurseSpellNecro() : this.GetRandomCurseSpellMage();
        }

        public virtual Spell GetRandomCurseSpellNecro()
        {
            switch( Utility.Random(4) )
            {
                case 0:
                    this.m_Mobile.DebugSay("Blood Oath");
                    return new BloodOathSpell(this.m_Mobile, null);
                case 1:
                    this.m_Mobile.DebugSay("Corpse Skin");
                    return new CorpseSkinSpell(this.m_Mobile, null);
                case 2:
                    this.m_Mobile.DebugSay("Evil Omen");
                    return new EvilOmenSpell(this.m_Mobile, null);
                default:
                    this.m_Mobile.DebugSay("Mind Rot");
                    return new MindRotSpell(this.m_Mobile, null);
            }
        }

        public virtual Spell GetRandomCurseSpellMage()
        {
            if (this.m_Mobile.Skills[SkillName.Magery].Value >= 40.0 && Utility.Random(4) == 0)
                return new CurseSpell(this.m_Mobile, null);

            switch( Utility.Random(3) )
            {
                case 0:
                    return new WeakenSpell(this.m_Mobile, null);
                case 1:
                    return new ClumsySpell(this.m_Mobile, null);
                default:
                    return new FeeblemindSpell(this.m_Mobile, null);
            }
        }

        public virtual Spell GetRandomManaDrainSpell()
        {
            if (this.m_Mobile.Skills[SkillName.Magery].Value >= 80.0 && Utility.RandomBool())
                return new ManaVampireSpell(this.m_Mobile, null);

            return new ManaDrainSpell(this.m_Mobile, null);
        }

        public virtual Spell GetRandomBuffSpell()
        {
            return new BlessSpell(m_Mobile, null);
        }

        public virtual Spell DoDispel(Mobile toDispel)
        {
            if (!this.SmartAI)
            {
                if (this.ScaleBySkill(DispelChance, SkillName.Magery) > Utility.RandomDouble())
                    return new DispelSpell(this.m_Mobile, null);

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

        public virtual Spell ChooseSpell(Mobile c)
        {
            Spell spell = null;

            if (!this.SmartAI)
            {
                spell = this.CheckCastHealingSpell();

                if (spell != null)
                    return spell;

                if (this.IsNecromancer)
                {
                    double psDamage = ((this.m_Mobile.Skills[SkillName.SpiritSpeak].Value - c.Skills[SkillName.MagicResist].Value) / 10) + (c.Player ? 18 : 30);

                    if (psDamage > c.Hits)
                        return new PainSpikeSpell(this.m_Mobile, null);
                }

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
                            if (c.Paralyzed || this.m_Mobile.Skills[SkillName.Magery].Value <= 50.0)
                                goto default;

                            this.m_Mobile.DebugSay("Attempting to paralyze");

                            spell = new ParalyzeSpell(this.m_Mobile, null);
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

            spell = this.CheckCastHealingSpell();

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

        public virtual Spell DoCombo(Mobile c)
        {
            Spell spell = null;

            if (this.m_Combo == 0)
            {
                spell = new ExplosionSpell(this.m_Mobile, null);
                ++this.m_Combo; // Move to next spell
            }
            else if (this.m_Combo == 1)
            {
                spell = new WeakenSpell(this.m_Mobile, null);
                ++this.m_Combo; // Move to next spell
            }
            else if (this.m_Combo == 2)
            {
                if (!c.Poisoned)
                    spell = new PoisonSpell(this.m_Mobile, null);
                else if (this.IsNecromancer)
                    spell = new StrangleSpell(this.m_Mobile, null);

                ++this.m_Combo; // Move to next spell
            }

            if (this.m_Combo == 3 && spell == null)
            {
                switch( Utility.Random(this.IsNecromancer ? 4 : 3) )
                {
                    case 0:
                        {
                            if (c.Int < c.Dex)
                                spell = new FeeblemindSpell(this.m_Mobile, null);
                            else
                                spell = new ClumsySpell(this.m_Mobile, null);

                            ++this.m_Combo; // Move to next spell

                            break;
                        }
                    case 1:
                        {
                            spell = new EnergyBoltSpell(this.m_Mobile, null);
                            this.m_Combo = -1; // Reset combo state
                            break;
                        }
                    case 2:
                        {
                            spell = new FlameStrikeSpell(this.m_Mobile, null);
                            this.m_Combo = -1; // Reset combo state
                            break;
                        }
                    default:
                        {
                            spell = new PainSpikeSpell(this.m_Mobile, null);
                            this.m_Combo = -1; // Reset combo state
                            break;
                        }
                }
            }
            else if (this.m_Combo == 4 && spell == null)
            {
                spell = new MindBlastSpell(this.m_Mobile, null);
                this.m_Combo = -1;
            }

            return spell;
        }

        public override bool DoActionCombat()
        {
            Mobile c = this.m_Mobile.Combatant;
            this.m_Mobile.Warmode = true;

            if (c == null || c.Deleted || !c.Alive || c.IsDeadBondedPet || !this.m_Mobile.CanSee(c) || !this.m_Mobile.CanBeHarmful(c, false) || c.Map != this.m_Mobile.Map)
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

                c = this.m_Mobile.Combatant;

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
                else if (this.SmartAI && this.m_Combo != -1) // We are doing a spell combo
                {
                    spell = this.DoCombo(c);
                }
                else if (this.SmartAI && (c.Spell is HealSpell || c.Spell is GreaterHealSpell) && !c.Poisoned) // They have a heal spell out
                {
                    spell = new PoisonSpell(this.m_Mobile, null);
                }
                else
                {
                    spell = this.ChooseSpell(c);
                }

                // Now we have a spell picked
                // Move first before casting

                if (this.SmartAI && toDispel != null)
                {
                    if (this.m_Mobile.InRange(toDispel, 10))
                        this.RunFrom(toDispel);
                    else if (!this.m_Mobile.InRange(toDispel, Core.ML ? 10 : 12))
                        this.RunTo(toDispel);
                }
                else
                {
                    this.RunTo(c);
                }

                if (spell != null)
                    spell.Cast();

                this.m_NextCastTime = DateTime.UtcNow + this.GetDelay(spell);
            }
            else if (this.m_Mobile.Spell == null || !this.m_Mobile.Spell.IsCasting)
            {
                this.RunTo(c);
            }

            this.m_LastTarget = c;
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
            Mobile c = this.m_Mobile.Combatant;

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

                Mobile comb = this.m_Mobile.Combatant;

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

                    Mobile comb = this.m_Mobile.Combatant;

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
                if (this.ScaleBySkill(HealChance, SkillName.Magery) < Utility.RandomDouble())
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
                if (this.UseNecromancy())
                {
                    this.m_Mobile.UseSkill(SkillName.SpiritSpeak);
                }
                else
                {
                    spell = new GreaterHealSpell(this.m_Mobile, null);

                    if (spell == null)
                        spell = new HealSpell(this.m_Mobile, null);
                }
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
            Target targ = this.m_Mobile.Target;

            if (targ == null)
                return false;

            bool isReveal = (targ is RevealSpell.InternalTarget);
            bool isDispel = (targ is DispelSpell.InternalTarget);
            bool isParalyze = (targ is ParalyzeSpell.InternalTarget);
            bool isTeleport = (targ is TeleportSpell.InternalTarget);
            bool isInvisible = (targ is InvisibilitySpell.InternalTarget);
            bool teleportAway = false;

            Mobile toTarget;

            if (isInvisible)
            {
                toTarget = this.m_Mobile;
            }
            else if (isDispel)
            {
                toTarget = this.FindDispelTarget(false);

                if (!this.SmartAI && toTarget != null)
                    this.RunTo(toTarget);
                else if (toTarget != null && this.m_Mobile.InRange(toTarget, 10))
                    this.RunFrom(toTarget);
            }
            else if (this.SmartAI && (isParalyze || isTeleport))
            {
                toTarget = this.FindDispelTarget(true);

                if (toTarget == null)
                {
                    toTarget = this.m_Mobile.Combatant;

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
                toTarget = this.m_Mobile.Combatant;

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
            else if (isReveal && this.m_RevealTarget != null)
            {
                targ.Invoke(this.m_Mobile, this.m_RevealTarget);
            }
            else if (isTeleport && toTarget != null)
            {
                Map map = this.m_Mobile.Map;

                if (map == null)
                {
                    targ.Cancel(this.m_Mobile, TargetCancelType.Canceled);
                    return true;
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
                        return true;
                    }
                }

                int teleRange = targ.Range;

                if (teleRange < 0)
                    teleRange = Core.ML ? 11 : 12;

                for (int i = 0; i < 10; ++i)
                {
                    Point3D randomPoint = new Point3D(this.m_Mobile.X - teleRange + Utility.Random(teleRange * 2 + 1), this.m_Mobile.Y - teleRange + Utility.Random(teleRange * 2 + 1), 0);

                    LandTarget lt = new LandTarget(randomPoint, map);

                    if (this.m_Mobile.InLOS(lt) && map.CanSpawnMobile(lt.X, lt.Y, lt.Z) && !SpellHelper.CheckMulti(randomPoint, map))
                    {
                        targ.Invoke(this.m_Mobile, new LandTarget(randomPoint, map));
                        return true;
                    }
                }

                targ.Cancel(this.m_Mobile, TargetCancelType.Canceled);
            }
            else
            {
                targ.Cancel(this.m_Mobile, TargetCancelType.Canceled);
            }

            return true;
        }
    }
}