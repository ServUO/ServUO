using System;
using System.Collections.Generic;
using Server.Items;
using Server.Spells;
using Server.Targeting;
using Server.Spells.Sixth;
using Server.Spells.Fourth;
using Server.Spells.Mysticism;
using Server.Spells.Spellweaving;

namespace Server.Mobiles
{
    public abstract class MagicalAI : BaseAI
    {
        protected const double HealChance = 0.25;// 10% chance to heal at gm skill

        private DateTime m_NextCastTime;
        private DateTime m_NextHealTime;
        private Mobile m_LastTarget;
		private Point3D m_LastTargetLoc;

        public DateTime NextCastTime { get { return m_NextCastTime; } set { m_NextCastTime = value; } }
		public DateTime NextHealTime { get { return m_NextHealTime; } set { m_NextHealTime = value; } }
		public Mobile LastTarget { get { return m_LastTarget; } set { m_LastTarget = value; } }
		public Point3D LastTargetLoc { get { return m_LastTargetLoc; } set { m_LastTargetLoc = value; } }

		public abstract SkillName CastSkill { get; }
		
        public MagicalAI(BaseCreature m)
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

        public override bool DoActionWander()
        {
            if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
            {
                m_Mobile.DebugSay("I am going to attack {0}", m_Mobile.FocusMob.Name);

                m_Mobile.Combatant = m_Mobile.FocusMob;
                Action = ActionType.Combat;
                m_NextCastTime = DateTime.UtcNow;
            }
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

        public void RunTo(IDamageable d)
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
            if (AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, true))
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
		
		// Basic, will need to be overridden in most AI's
		public virtual Spell RandomCombatSpell()
		{
			Spell spell = CheckCastHealingSpell();

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
					m_Mobile.DebugSay("Cursing Thy!");
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

		public virtual Spell GetRandomCurseSpell()
        {
            return null;
        }
		
		public virtual Spell GetRandomBuffSpell()
        {
            return null;
        }
		
        public virtual Spell GetRandomDamageSpell()
        {
			return null;
        }

        public virtual Spell ChooseSpell(IDamageable d)
        {
            if (!(d is Mobile))
            {
                m_Mobile.DebugSay("Just doing damage");
                return GetRandomDamageSpell();
            }

            Spell spell = RandomCombatSpell();

            return spell;
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

                if (spell != null)
                {
                    // We are ready to cast a spell
                    spell = ChooseSpell(c);
                    // Now we have a spell picked
                    RunTo(c);
                }

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

        public virtual void TryReveal()
        {
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
			
			if(spell != null)
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
		
		public virtual Spell GetHealSpell()
		{
			return null;
		}
		
		public virtual Spell GetCureSpell()
		{
			return null;
		}

        protected TimeSpan GetDelay(Spell spell)
        {
            if (SmartAI || (spell is DispelSpell))
            {
                return TimeSpan.FromSeconds(m_Mobile.ActiveSpeed);
            }
            else
            {
                double del = ScaleBySkill(3.0, CastSkill);
                double min = 6.0 - (del * 0.75);
                double max = 6.0 - (del * 1.25);

                return TimeSpan.FromSeconds(min + ((max - min) * Utility.RandomDouble()));
            }
        }

        protected virtual DateTime GetCastDelay(Spell spell)
        {
            TimeSpan delay = spell == null ? TimeSpan.FromSeconds(m_Mobile.ActiveSpeed) : spell.GetCastDelay() + spell.GetCastRecovery() + TimeSpan.FromSeconds(1);

            return DateTime.UtcNow + delay;
        }

        protected virtual bool ProcessTarget()
        {
            Target targ = m_Mobile.Target;

            if (targ == null)
                return false;

            bool harmful = (targ.Flags & TargetFlags.Harmful) != 0 || targ is HailStormSpell.InternalTarget || targ is WildfireSpell.InternalTarget;
            bool beneficial = (targ.Flags & TargetFlags.Beneficial) != 0 || targ is ArchCureSpell.InternalTarget;

            IDamageable toTarget = m_Mobile.Combatant;

            if (toTarget != null)
                RunTo(toTarget);

            if (harmful && toTarget != null)
            {
                if ((targ.Range == -1 || m_Mobile.InRange(toTarget, targ.Range)) && m_Mobile.CanSee(toTarget) && m_Mobile.InLOS(toTarget))
                {
                    targ.Invoke(m_Mobile, toTarget);
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