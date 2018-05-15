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
using Server.Spells.Mysticism;
using Server.Spells.Spellweaving;
using Server.Targeting;
using Server.Spells.SkillMasteries;
using Server.SkillHandlers;

namespace Server.Mobiles
{
	public class MageAI : BaseAI
	{
        protected const double HealChance = 0.25;// 10% chance to heal at gm skill

		public virtual SkillName CastSkill { get { return SkillName.Magery; } }
        public virtual bool UsesMagery { get { return true; } }

		protected const double DispelChance = 0.75; // 75% chance to dispel at gm skill
        protected double TeleportChance { get { return m_Mobile.TeleportChance; } }
		
        private LandTarget m_RevealTarget;
        private DateTime m_NextCastTime;
        private DateTime m_NextHealTime;
        private Mobile m_LastTarget;
        private Point3D m_LastTargetLoc;

        public DateTime NextCastTime { get { return m_NextCastTime; } set { m_NextCastTime = value; } }
        public DateTime NextHealTime { get { return m_NextHealTime; } set { m_NextHealTime = value; } }
        public Mobile LastTarget { get { return m_LastTarget; } set { m_LastTarget = value; } }
        public Point3D LastTargetLoc { get { return m_LastTargetLoc; } set { m_LastTargetLoc = value; } }

        public virtual bool SmartAI
        {
            get
            {
                return PetTrainingHelper.GetAbilityProfile(m_Mobile, true).HasAbility(MagicalAbility.MageryMastery);
            }
        }
		
		public MageAI(BaseCreature m)
            : base(m)
        {
        }

        public override bool Think()
        {
            if (m_Mobile.Deleted)
                return false;

            if (ProcessTarget())
                return true;
            else
                return base.Think();
        }

        public virtual double ScaleByCastSkill(double v)
        {
            return m_Mobile.Skills[CastSkill].Value * v * 0.01;
        }

        public virtual double ScaleBySkill(double v, SkillName skill)
        {
            return v * m_Mobile.Skills[skill].Value / 100;
        }

        protected TimeSpan GetDelay(Spell spell)
        {
            double del = ScaleBySkill(3.0, spell != null ? spell.CastSkill : CastSkill);
            double min = 6.0 - (del * 0.75);
            double max = 6.0 - (del * 1.25);

            return TimeSpan.FromSeconds(min + ((max - min) * Utility.RandomDouble()));
        }

        protected virtual DateTime GetCastDelay(Spell spell)
		{
            if (UsesMagery)
            {
                TimeSpan ts = !SmartAI && !(spell is DispelSpell) ? TimeSpan.FromSeconds(1) : m_Combo > -1 ? TimeSpan.FromSeconds(.5) : TimeSpan.FromSeconds(1.5);
                TimeSpan delay = spell == null ? TimeSpan.FromSeconds(m_Mobile.ActiveSpeed * 2.0) : spell.GetCastDelay() + spell.GetCastRecovery() + ts;

                return DateTime.UtcNow + delay;
            }
            else
            {
                TimeSpan delay = spell == null ? TimeSpan.FromSeconds(m_Mobile.ActiveSpeed * 2.0) : spell.GetCastDelay() + spell.GetCastRecovery() + GetDelay(spell);
                return DateTime.UtcNow + delay;
            }
		}


        public override bool DoActionWander()
        {
            if (SmartAI && m_Mobile.Skills[SkillName.Meditation].Base > 0 && m_Mobile.Mana < m_Mobile.ManaMax && !m_Mobile.Meditating)
            {
                m_Mobile.DebugSay("I am going to meditate");

                m_Mobile.UseSkill(SkillName.Meditation);
                return true;
            }

            if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
            {
                m_Mobile.DebugSay("I am going to attack {0}", m_Mobile.FocusMob.Name);

                m_Mobile.Combatant = m_Mobile.FocusMob;
                Action = ActionType.Combat;
                m_NextCastTime = DateTime.UtcNow;
            }
            else
            {
                m_Mobile.DebugSay("I am wandering");

                m_Mobile.Warmode = false;

                base.DoActionWander();

                if (Utility.RandomDouble() < 0.05)
                {
                    Spell spell = CheckCastHealingSpell();

                    if (spell != null)
                        spell.Cast();
                }
            }

            return true;
        }

        public override bool DoActionCombat()
        {
            IDamageable c = m_Mobile.Combatant;
            m_Mobile.Warmode = true;

            if (m_Mobile.Target != null)
                ProcessTarget();

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
                m_Mobile.DebugSay("I can't see my target");

                if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
                {
                    m_Mobile.DebugSay("I will switch to {0}", m_Mobile.FocusMob.Name);
                    m_Mobile.Combatant = c = m_Mobile.FocusMob;
                    m_Mobile.FocusMob = null;
                }
            }

            if (!Core.AOS && SmartAI && !m_Mobile.StunReady && m_Mobile.Skills[SkillName.Wrestling].Value >= 80.0 && m_Mobile.Skills[SkillName.Anatomy].Value >= 80.0)
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

                c = m_Mobile.Combatant as Mobile;

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

            if (m_Mobile.Spell == null && DateTime.UtcNow > m_NextCastTime && m_Mobile.InRange(c, Core.ML ? 10 : 12))
            {
                Spell spell = null;

                if (m_Mobile.Poisoned)
                {
                    spell = GetCureSpell();
                }

                if (SmartAI &&  spell == null && 
                                !m_Mobile.InRange(c.Location, 3) && 
                                UsesMagery && c is Mobile && 
                                !m_Mobile.DisallowAllMoves && 
                                CheckCanCastMagery(3) &&
                                m_Mobile.ControlOrder == OrderType.Attack &&
                                m_Mobile.InRange(c.Location, 10) &&
                                0.6 > Utility.RandomDouble())
                {
                    spell = new TeleportSpell(m_Mobile, null);
                }

                if (spell == null)
                {
                    // We are ready to cast a spell
                    spell = ChooseSpell(c);
                }

                m_Mobile.DebugSay(spell != null ? "Choosen Spell" : "No spell chosen");
                RunTo(c);

                if (spell != null)
                    spell.Cast();

                m_NextCastTime = GetCastDelay(spell);
            }
            else
            {
                RunTo(c);
            }

            m_LastTarget = c as Mobile;
            m_LastTargetLoc = c.Location;

            return true;
        }

        public override bool DoActionGuard()
        {
            if (m_LastTarget != null && m_LastTarget.Hidden)
            {
                Map map = m_Mobile.Map;

                if (map == null || !m_Mobile.InRange(m_LastTargetLoc, Core.ML ? 10 : 12))
                {
                    m_LastTarget = null;
                }
                else if (m_Mobile.Spell == null && DateTime.UtcNow > m_NextCastTime)
                {
                    // virutal method needs to go in BaseAI
                    TryReveal();
                    m_Mobile.DebugSay("I am going to reveal my last target");
                }
            }

            if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
            {
                m_Mobile.DebugSay("I am going to attack {0}", m_Mobile.FocusMob.Name);

                m_Mobile.Combatant = m_Mobile.FocusMob;
                Action = ActionType.Combat;
            }
            else
            {
                ProcessTarget();

                Spell spell = CheckCastHealingSpell();

                if (spell != null)
                    spell.Cast();

                base.DoActionGuard();
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

        public virtual void RunTo(IDamageable d)
        {
            if (!SmartAI)
            {
                if (!MoveTo(d, true, m_Mobile.RangeFight))
                    OnFailedMove();

                return;
            }

            if (d is Mobile && (((Mobile)d).Paralyzed || ((Mobile)d).Frozen))
            {
                if (m_Mobile.InRange(d, 1))
                    RunFrom(d);
                else if (!m_Mobile.InRange(d, m_Mobile.RangeFight > 2 ? m_Mobile.RangeFight : 2) && !MoveTo(d, true, 1))
                    OnFailedMove();
            }
            else
            {
                if (!m_Mobile.InRange(d, m_Mobile.RangeFight))
                {
                    if (!MoveTo(d, true, 1))
                        OnFailedMove();
                }
                else if (m_Mobile.InRange(d, m_Mobile.RangeFight - 1))
                {
                    RunFrom(d);
                }
            }
        }

        public void RunFrom(IDamageable d)
        {
            Run((Direction)((int)m_Mobile.GetDirectionTo(d) - 4) & Direction.Mask);
        }

        public virtual void OnFailedMove()
        {
            if (UsesMagery && !m_Mobile.DisallowAllMoves && (SmartAI ? Utility.Random(10) == 0 : ScaleByCastSkill(TeleportChance) > Utility.RandomDouble()))
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
		
		public virtual Spell ChooseSpell(IDamageable c)
		{
            if (c == null || !c.Alive)
                return null;

            Spell spell = null;

            if (!UsesMagery)
            {
                if (!(c is Mobile))
                {
                    m_Mobile.DebugSay("Just doing damage...");
                    spell = GetRandomDamageSpell();
                }
                else
                {
                    m_Mobile.DebugSay("Random combat spell...");
                    spell = RandomCombatSpell();
                }

                return spell;
            }

			if (m_Mobile.Poisoned) // Top cast priority is cure
			{
				m_Mobile.DebugSay("I am going to cure myself");

				spell = GetCureSpell();
			}

            if (spell != null)
                return spell;

            Mobile toDispel = FindDispelTarget(true);
            
            if (toDispel != null) // Something dispellable is attacking us
			{
				m_Mobile.DebugSay("I am going to dispel {0}", toDispel);

				spell = DoDispel(toDispel);
			}

            if (spell != null)
                return spell;

            if (c is Mobile && SmartAI && m_Combo != -1) // We are doing a spell combo
			{
				spell = DoCombo((Mobile)c);
			}
			else if (c is Mobile && SmartAI && (((Mobile)c).Spell is HealSpell || ((Mobile)c).Spell is GreaterHealSpell) && !((Mobile)c).Poisoned) // They have a heal spell out
			{
				spell = new PoisonSpell(m_Mobile, null);
			}
			else
			{
				spell = RandomCombatSpell();
			}
			
			return spell;
		}

        protected virtual Spell CheckCastHealingSpell()
        {
            // Summoned creatures never heal themselves.
            if (m_Mobile.Summoned)
                return null;

            Spell spell = null;

            // If I'm poisoned, always attempt to cure.
            if (m_Mobile.Poisoned)
            {
                spell = GetCureSpell();
            }

            if (spell != null)
                return spell;

            if (m_Mobile.Controlled && DateTime.UtcNow < m_NextHealTime)
            {
                return null;
            }

            if (!SmartAI && ScaleByCastSkill(HealChance) < Utility.RandomDouble())
            {
                return null;
            }
            else if (Utility.Random(0, 4 + (m_Mobile.Hits == 0 ? m_Mobile.HitsMax : (m_Mobile.HitsMax / m_Mobile.Hits))) < 3)
            {
                return null;
            }

            spell = GetHealSpell();

            double delay;

            if (m_Mobile.Int >= 500)
                delay = Utility.RandomMinMax(7, 10);
            else
                delay = Math.Sqrt(600 - m_Mobile.Int);

            m_NextHealTime = DateTime.UtcNow + TimeSpan.FromSeconds(delay);

            return spell;
        }
		
		public virtual Spell DoDispel(Mobile toDispel)
        {
            if (!SmartAI)
            {
                if (CheckCanCastMagery(6) && ScaleByCastSkill(DispelChance) > Utility.RandomDouble())
                    return new DispelSpell(m_Mobile, null);

                return null;
            }

            Spell spell = CheckCastHealingSpell();

            if (spell == null && CheckCanCastMagery(6))
            {
                spell = new DispelSpell(m_Mobile, null);
            }

            return spell;
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

                if (comb != null && !comb.Deleted && comb.Alive && !comb.IsDeadBondedPet && m_Mobile.InRange(comb, Core.ML ? 10 : 12) && CanDispel(comb))
                {
                    active = comb;
                    activePrio = m_Mobile.GetDistanceToSqrt(comb);

                    if (activePrio <= 2)
                        return active;
                }

                for (int i = 0; i < aggressed.Count; ++i)
                {
                    AggressorInfo info = aggressed[i];
                    Mobile m = info.Defender;

                    if (m != comb && m.Combatant == m_Mobile && m_Mobile.InRange(m, Core.ML ? 10 : 12) && CanDispel(m))
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
                    AggressorInfo info = aggressors[i];
                    Mobile m = info.Attacker;

                    if (m != comb && m.Combatant == m_Mobile && m_Mobile.InRange(m, Core.ML ? 10 : 12) && CanDispel(m))
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

                    IPooledEnumerable eable = m_Mobile.GetMobilesInRange(Core.ML ? 10 : 12);

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
		
		public Spell CheckCastDispelField()
        {
            if (m_Mobile.Frozen || m_Mobile.Paralyzed)
                return null;

            int mana = m_Mobile.Mana;

            if (mana < 14)
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
            if (m_Mobile.Map == null)
                return null;

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
		
		public virtual Spell RandomCombatSpell()
		{
            Spell spell = null;

            if (!UsesMagery)
            {
                spell = CheckCastHealingSpell();

                if (spell != null)
                    return spell;

                switch (Utility.Random(6))
                {
                    case 0:  // Buff
                        {
                            m_Mobile.DebugSay("Cursing Thyself!");
                            spell = GetRandomBuffSpell();
                            break;
                        }
                    case 1:	// Curse
                        {
                            m_Mobile.DebugSay("Cursing Thou!");
                            spell = GetRandomCurseSpell();
                            break;
                        }
                    case 2:
                    case 3:
                    case 4:
                    case 5:	// damage
                        {
                            m_Mobile.DebugSay("Just doing damage");
                            spell = GetRandomDamageSpell();
                        }
                        break;
                }

                return spell;
            }

            Mobile c = m_Mobile.Combatant as Mobile;

            if (c == null || !c.Alive)
                return null;

			if(!SmartAI)
			{
				spell = CheckCastHealingSpell();

				if (spell != null)
					return spell;

				if (spell == null && m_Mobile.RawInt >= 80)
					spell = CheckCastDispelField();

				switch (Utility.Random(15))
				{
					case 0:
					case 1:	// Poison them
						{
							if (c.Poisoned || !CheckCanCastMagery(3))
								goto default;

							m_Mobile.DebugSay("Attempting to poison");

							spell = new PoisonSpell(m_Mobile, null);
							break;
						}
					case 2:	// Bless ourselves
						{
							if (!CheckCanCastMagery(3))
								goto default;

							m_Mobile.DebugSay("Blessing myself");

							spell = GetRandomBuffSpell();//new BlessSpell(m_Mobile, null);
							break;
						}
					case 3:
					case 4: // Curse them
						{
							m_Mobile.DebugSay("Attempting to curse");

							spell = GetRandomCurseSpell();

							if (spell == null)
								goto default;
							break;
						}
					case 5:	// Paralyze them
						{
							m_Mobile.DebugSay("Attempting to paralyze");

							if (CheckCanCastMagery(5))
								spell = new ParalyzeSpell(m_Mobile, null);
							else
								spell = GetRandomCurseSpell();

							if (spell == null)
								goto default;
							break;
						}
					case 6: // Drain mana
						{
							m_Mobile.DebugSay("Attempting to drain mana");

							spell = GetRandomManaDrainSpell();

							if (spell == null)
								goto default;
							break;
						}
					default: // Damage them
						{
							m_Mobile.DebugSay("Just doing damage");

							spell = GetRandomDamageSpell();
							break;
						}
				}
			}
			else
			{
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

							spell = new PoisonSpell(m_Mobile, null);
							break;
						}
					case 1: // Deal some damage
						{
							spell = GetRandomDamageSpell();

							break;
						}
					default: // Set up a combo
						{
							if (m_Mobile.Mana > 15 && m_Mobile.Mana < 40)
							{
								if (c.Paralyzed && !c.Poisoned && !m_Mobile.Meditating)
								{
									m_Mobile.DebugSay("I am going to meditate");

									m_Mobile.UseSkill(SkillName.Meditation);
								}
								else if (!c.Poisoned)
								{
									spell = new ParalyzeSpell(m_Mobile, null);
								}
							}
							else if (m_Mobile.Mana > 60)
							{
								if (Utility.RandomBool() && !c.Paralyzed && !c.Frozen && !c.Poisoned)
								{
									m_Combo = 0;
									spell = new ParalyzeSpell(m_Mobile, null);
								}
								else
								{
									m_Combo = 1;
									spell = new ExplosionSpell(m_Mobile, null);
								}
							}

							break;
						}
				}
			}
			
			return spell;
		}
		
		 //TODO: This needs to be tested on EA
		/// <summary>
        /// Obsolete. Creatures don't fizzle based on spell, but cast spells depending on mana pool
        /// </summary>
        /// <returns></returns>
        public virtual int GetMaxCircle()
        {
            return (int)((m_Mobile.Skills[SkillName.Magery].Value + 20.0) / (100.0 / 7.0));
        }

        private int[] m_ManaTable = new int[] { 4, 6, 9, 11, 14, 20, 40, 50 };

        public virtual bool CheckCanCastMagery(int circle)
        {
            if (circle < 1) circle = 1;
            if (circle > 8) circle = 8;

            double skill = (100.0 / 7.0) * (double)circle;

            return m_Mobile.Mana >= m_ManaTable[circle - 1] && (Core.SA || m_Mobile.Skills[SkillName.Magery].Value >= skill - 20);
        }

        public virtual Spell GetRandomDamageSpell()
        {
            int select;

			if(SmartAI)
			{
                if (m_Mobile.Mana > 100)
                {
                    if (!SkillMasterySpell.HasSpell(m_Mobile, typeof(DeathRaySpell)))
                        select = Utility.RandomMinMax(0, 4);
                    else
                        select = Utility.RandomMinMax(1, 4);
                }
                else if (CheckCanCastMagery(7))
                {
                    select = Utility.Random(5, 11);
                }
                else
                {
                    select = Utility.Random(5, 8);
                }
				
				switch (select)
				{
                    case 0:return new DeathRaySpell(m_Mobile, null);
					case 1: return new MindBlastSpell(m_Mobile, null);
					case 2: return new EnergyBoltSpell(m_Mobile, null);
					case 3: return new ExplosionSpell(m_Mobile, null);
					case 4: return new FlameStrikeSpell(m_Mobile, null);
					case 5: return new MagicArrowSpell(m_Mobile, null);
					case 6: return new HarmSpell(m_Mobile, null);
					case 7: return new FireballSpell(m_Mobile, null);
					case 8: return new LightningSpell(m_Mobile, null);
                    case 9:
                    case 10:
                    case 11: return new ManaVampireSpell(m_Mobile, null);
				}
			}
			else
			{
				if (CheckCanCastMagery(8)) select = 8;
				else if (CheckCanCastMagery(6)) select = 6;
				else if (CheckCanCastMagery(5)) select = 5;
				else if (CheckCanCastMagery(4)) select = 4;
				else if (CheckCanCastMagery(3)) select = 3;
				else if (CheckCanCastMagery(2)) select = 2;
				else select = 1;
				
				switch (Utility.Random(select))
				{
					default:
					case 0: return new MagicArrowSpell(m_Mobile, null);
					case 1: return new HarmSpell(m_Mobile, null);
					case 2: return new FireballSpell(m_Mobile, null);
					case 3: return new LightningSpell(m_Mobile, null);
					case 4: return new MindBlastSpell(m_Mobile, null);
					case 5: return new EnergyBoltSpell(m_Mobile, null);
					case 6: return new ExplosionSpell(m_Mobile, null);
					case 7: return new FlameStrikeSpell(m_Mobile, null);
				}
			}

            return null;
        }

        public virtual Spell GetRandomCurseSpell()
        {
            if (Utility.RandomBool() && CheckCanCastMagery(5))
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
            if (Utility.RandomBool() && CheckCanCastMagery(7))
                return new ManaVampireSpell(m_Mobile, null);
            else if ((CheckCanCastMagery(4)))
                return new ManaDrainSpell(m_Mobile, null);

            return null;
        }

        public virtual Spell GetRandomSummonSpell()
        {
			// TODO: I left this here if anyone wants summons from magery
            /*if (CheckCanCastMagery(8))
                return new EnergyVortexSpell(m_Mobile, null);
            else if (CheckCanCastMagery(5))
                return new BladeSpiritsSpell(m_Mobile, null);*/

            return null;
        }

        public virtual Spell GetRandomFieldSpell()
        {
			// I left this here if someone wants field spells
            /*bool pois = m_Mobile.Skills[SkillName.Poisoning].Value >= 80.0 || m_Mobile.HitPoison == Poison.Greater || m_Mobile.HitPoison == Poison.Lethal;

            if (pois && CheckCanCastMagery(5))
                return new PoisonFieldSpell(m_Mobile, null);

            else if (Utility.RandomBool() && CheckCanCastMagery(6))
                return new ParalyzeFieldSpell(m_Mobile, null);
            else if (CheckCanCastMagery(4))
                return new FireFieldSpell(m_Mobile, null);*/

            return null;
        }

        public virtual Spell GetRandomBuffSpell()
        {
            if (!BlessSpell.IsBlessed(m_Mobile) && CheckCanCastMagery(3))
                return new BlessSpell(m_Mobile, null);
            else if (CheckCanCastMagery(6))
                return new InvisibilitySpell(m_Mobile, null);

            return null;
        }

        public virtual Spell GetHealSpell()
        {
            Spell spell = null;

            if (m_Mobile.Hits < (m_Mobile.HitsMax - 50))
            {
                if (CheckCanCastMagery(4))
                {
                    spell = new GreaterHealSpell(m_Mobile, null);
                }
                else
                {
                    spell = new HealSpell(m_Mobile, null);
                }
            }
            else if (m_Mobile.Hits < (m_Mobile.HitsMax - 10))
            {
                spell = new HealSpell(m_Mobile, null);
            }

            return spell;
        }

        public virtual Spell GetCureSpell()
		{
			if (SmartAI && m_Mobile.Poison.Level > 1 && CheckCanCastMagery(4))
            {
                return new ArchCureSpell(m_Mobile, null);
            }
            else if (CheckCanCastMagery(2))
            {
                return new CureSpell(m_Mobile, null);
            }

            return null;
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
		
		protected virtual bool ProcessTarget()
		{
			Target targ = m_Mobile.Target;

            if (targ == null)
                return false;

            bool harmful = (targ.Flags & TargetFlags.Harmful) != 0 || targ is HailStormSpell.InternalTarget || targ is WildfireSpell.InternalTarget;
            bool beneficial = (targ.Flags & TargetFlags.Beneficial) != 0 || targ is ArchCureSpell.InternalTarget;

            if (UsesMagery)
            {
                bool isDispel = (targ is DispelSpell.InternalTarget || targ is MassDispelSpell.InternalTarget);
                bool isParalyze = (targ is ParalyzeSpell.InternalTarget);
                bool isTeleport = (targ is TeleportSpell.InternalTarget);
                bool isSummon = (targ is EnergyVortexSpell.InternalTarget || targ is BladeSpiritsSpell.InternalTarget || targ is NatureFurySpell.InternalTarget);
                bool isField = (targ is FireFieldSpell.InternalTarget || targ is PoisonFieldSpell.InternalTarget || targ is ParalyzeFieldSpell.InternalTarget);
                bool isAnimate = (targ is AnimateDeadSpell.InternalTarget);
                bool isDispelField = (targ is DispelFieldSpell.InternalTarget);
                bool teleportAway = false;

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
            else
            {
                IDamageable toTarget = m_Mobile.Combatant;

                if (toTarget != null)
                    RunTo(toTarget);

                if (harmful && toTarget != null)
                {
                    if ((targ.Range == -1 || m_Mobile.InRange(toTarget, targ.Range)) && m_Mobile.CanSee(toTarget) && m_Mobile.InLOS(toTarget))
                    {
                        targ.Invoke(m_Mobile, toTarget);
                    }
                    else
                    {
                        targ.Cancel(m_Mobile, TargetCancelType.Canceled);
                    }
                }
                else if (beneficial)
                {
                    targ.Invoke(m_Mobile, m_Mobile);
                }
                else
                {
                    targ.Cancel(m_Mobile, TargetCancelType.Canceled);
                }

                return true;
            }
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
		
		public virtual void TryReveal()
		{
            if (!m_Mobile.Alive || m_Mobile.Map == null)
                return;

			m_RevealTarget = new LandTarget(LastTargetLoc, m_Mobile.Map);

            if (UsesMagery)
            {
                Spell spell = new RevealSpell(m_Mobile, null);

                if (spell.Cast())
                    LastTarget = null; // only do it once

                NextCastTime = DateTime.UtcNow + GetDelay(spell);
            }
            else if (m_Mobile.Skills[SkillName.DetectHidden].Value > 0)
            {
                DetectHidden.OnUse(m_Mobile);

                if (m_Mobile.Target is DetectHidden.InternalTarget)
                {
                    m_Mobile.Target.Invoke(m_Mobile, m_RevealTarget);
                }

                NextCastTime = DateTime.UtcNow + TimeSpan.FromSeconds(1);
            }
		}
	}
}
