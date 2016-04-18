using System;
using System.Collections;
using Server.Network;
using Server.Spells;

namespace Server.Items
{
    public abstract class WeaponAbility
    {
        public virtual int BaseMana
        {
            get
            {
                return 0;
            }
        }

        public virtual int AccuracyBonus
        {
            get
            {
                return 0;
            }
        }
        public virtual double DamageScalar
        {
            get
            {
                return 1.0;
            }
        }

        public virtual bool RequiresSE
        {
            get
            {
                return false;
            }
        }

		/// <summary>
		///		Return false to make this special ability consume no ammo from ranged weapons
		/// </summary>
		public virtual bool ConsumeAmmo
		{
			get
			{
				return true;
			}
		}

        public virtual void OnHit(Mobile attacker, Mobile defender, int damage)
        {
        }

        public virtual void OnMiss(Mobile attacker, Mobile defender)
        {
        }

        public virtual bool OnBeforeSwing(Mobile attacker, Mobile defender)
        {
            // Here because you must be sure you can use the skill before calling CheckHit if the ability has a HCI bonus for example
            return true;
        }

        public virtual bool OnBeforeDamage(Mobile attacker, Mobile defender)
        {
            return true;
        }

        public virtual bool RequiresTactics(Mobile from)
        {
            return true;
        }

        public virtual double GetRequiredSkill(Mobile from)
        {
            BaseWeapon weapon = from.Weapon as BaseWeapon;

            if (weapon != null && weapon.PrimaryAbility == this)
                return 70.0;
            else if (weapon != null && weapon.SecondaryAbility == this)
                return 90.0;

            return 200.0;
        }

        public virtual int CalculateMana(Mobile from)
        {
            int mana = this.BaseMana;

            double skillTotal = this.GetSkill(from, SkillName.Swords) + this.GetSkill(from, SkillName.Macing) +
                                this.GetSkill(from, SkillName.Fencing) + this.GetSkill(from, SkillName.Archery) + this.GetSkill(from, SkillName.Parry) +
                                this.GetSkill(from, SkillName.Lumberjacking) + this.GetSkill(from, SkillName.Stealth) +
                                this.GetSkill(from, SkillName.Poisoning) + this.GetSkill(from, SkillName.Bushido) + this.GetSkill(from, SkillName.Ninjitsu);

            if (skillTotal >= 300.0)
                mana -= 10;
            else if (skillTotal >= 200.0)
                mana -= 5;

            double scalar = 1.0;
            if (!Server.Spells.Necromancy.MindRotSpell.GetMindRotScalar(from, ref scalar))
                scalar = 1.0;

            // Lower Mana Cost = 40%
            int lmc = Math.Min(AosAttributes.GetValue(from, AosAttribute.LowerManaCost), 40);

            scalar -= (double)lmc / 100;
            mana = (int)(mana * scalar);

            // Using a special move within 3 seconds of the previous special move costs double mana 
            if (GetContext(from) != null)
                mana *= 2;

            return mana;
        }

        public virtual bool CheckWeaponSkill(Mobile from)
        {
            BaseWeapon weapon = from.Weapon as BaseWeapon;

            if (weapon == null)
                return false;

            Skill skill = from.Skills[weapon.Skill];
            double reqSkill = this.GetRequiredSkill(from);
            bool reqTactics = Core.ML && this.RequiresTactics(from);

            if (Core.ML && reqTactics && from.Skills[SkillName.Tactics].Base < reqSkill)
            {
                from.SendLocalizedMessage(1079308, reqSkill.ToString()); // You need ~1_SKILL_REQUIREMENT~ weapon and tactics skill to perform that attack
                return false;
            }

            if (skill != null && skill.Base >= reqSkill)
                return true;

            /* <UBWS> */
            if (weapon.WeaponAttributes.UseBestSkill > 0 && (from.Skills[SkillName.Swords].Base >= reqSkill || from.Skills[SkillName.Macing].Base >= reqSkill || from.Skills[SkillName.Fencing].Base >= reqSkill))
                return true;
            /* </UBWS> */

            if (reqTactics)
            {
                from.SendLocalizedMessage(1079308, reqSkill.ToString()); // You need ~1_SKILL_REQUIREMENT~ weapon and tactics skill to perform that attack
            }
            else
            {
                from.SendLocalizedMessage(1060182, reqSkill.ToString()); // You need ~1_SKILL_REQUIREMENT~ weapon skill to perform that attack
            }

            return false;
        }

        public virtual bool CheckSkills(Mobile from)
        {
            return this.CheckWeaponSkill(from);
        }

        public virtual double GetSkill(Mobile from, SkillName skillName)
        {
            Skill skill = from.Skills[skillName];

            if (skill == null)
                return 0.0;

            return skill.Value;
        }

        public virtual bool CheckMana(Mobile from, bool consume)
        {
            int mana = this.CalculateMana(from);

            if (from.Mana < mana)
            {
                from.SendLocalizedMessage(1060181, mana.ToString()); // You need ~1_MANA_REQUIREMENT~ mana to perform that attack
                return false;
            }

            if (consume)
            {
                if (GetContext(from) == null)
                {
                    Timer timer = new WeaponAbilityTimer(from);
                    timer.Start();

                    AddContext(from, new WeaponAbilityContext(timer));
                }

                if (ManaPhasingOrb.IsInManaPhase(from))
                    ManaPhasingOrb.RemoveFromTable(from);
                else
                    from.Mana -= mana;
            }

            return true;
        }

        public virtual bool Validate(Mobile from)
        {
            if (!from.Player)
                return true;

            NetState state = from.NetState;

            if (state == null)
                return false;

            if (this.RequiresSE && !state.SupportsExpansion(Expansion.SE))
            {
                from.SendLocalizedMessage(1063456); // You must upgrade to Samurai Empire in order to use that ability.
                return false;
            }

            if (Spells.Bushido.HonorableExecution.IsUnderPenalty(from) || Spells.Ninjitsu.AnimalForm.UnderTransformation(from))
            {
                from.SendLocalizedMessage(1063024); // You cannot perform this special move right now.
                return false;
            }

            if (Core.ML && from.Spell != null)
            {
                from.SendLocalizedMessage(1063024); // You cannot perform this special move right now.
                return false;
            }

            #region Dueling
            string option = null;

            if (this is ArmorIgnore)
                option = "Armor Ignore";
            else if (this is BleedAttack)
                option = "Bleed Attack";
            else if (this is ConcussionBlow)
                option = "Concussion Blow";
            else if (this is CrushingBlow)
                option = "Crushing Blow";
            else if (this is Disarm)
                option = "Disarm";
            else if (this is Dismount)
                option = "Dismount";
            else if (this is DoubleStrike)
                option = "Double Strike";
            else if (this is InfectiousStrike)
                option = "Infectious Strike";
            else if (this is MortalStrike)
                option = "Mortal Strike";
            else if (this is MovingShot)
                option = "Moving Shot";
            else if (this is ParalyzingBlow)
                option = "Paralyzing Blow";
            else if (this is ShadowStrike)
                option = "Shadow Strike";
            else if (this is WhirlwindAttack)
                option = "Whirlwind Attack";
            else if (this is RidingSwipe)
                option = "Riding Swipe";
            else if (this is FrenziedWhirlwind)
                option = "Frenzied Whirlwind";
            else if (this is Block)
                option = "Block";
            else if (this is DefenseMastery)
                option = "Defense Mastery";
            else if (this is NerveStrike)
                option = "Nerve Strike";
            else if (this is TalonStrike)
                option = "Talon Strike";
            else if (this is Feint)
                option = "Feint";
            else if (this is DualWield)
                option = "Dual Wield";
            else if (this is DoubleShot)
                option = "Double Shot";
            else if (this is ArmorPierce)
                option = "Armor Pierce";

            if (option != null && !Engines.ConPVP.DuelContext.AllowSpecialAbility(from, option, true))
                return false;
            #endregion

            return this.CheckSkills(from) && this.CheckMana(from, false);
        }

        private static readonly WeaponAbility[] m_Abilities = new WeaponAbility[33]
        {
            null,
            new ArmorIgnore(),
            new BleedAttack(),
            new ConcussionBlow(),
            new CrushingBlow(),
            new Disarm(),
            new Dismount(),
            new DoubleStrike(),
            new InfectiousStrike(),
            new MortalStrike(),
            new MovingShot(),
            new ParalyzingBlow(),
            new ShadowStrike(),
            new WhirlwindAttack(),
            new RidingSwipe(),
            new FrenziedWhirlwind(),
            new Block(),
            new DefenseMastery(),
            new NerveStrike(),
            new TalonStrike(),
            new Feint(),
            new DualWield(),
            new DoubleShot(),
            new ArmorPierce(),
            new Bladeweave(),
            new ForceArrow(),
            new LightningArrow(),
            new PsychicAttack(),
            new SerpentArrow(),
            new ForceOfNature(),
            new InfusedThrow(),
            new MysticArc(),
            new Disrobe()
        };

        public static WeaponAbility[] Abilities
        {
            get
            {
                return m_Abilities;
            }
        }

        private static readonly Hashtable m_Table = new Hashtable();

        public static Hashtable Table
        {
            get
            {
                return m_Table;
            }
        }

        public static readonly WeaponAbility ArmorIgnore = m_Abilities[1];
        public static readonly WeaponAbility BleedAttack = m_Abilities[2];
        public static readonly WeaponAbility ConcussionBlow = m_Abilities[3];
        public static readonly WeaponAbility CrushingBlow = m_Abilities[4];
        public static readonly WeaponAbility Disarm = m_Abilities[5];
        public static readonly WeaponAbility Dismount = m_Abilities[6];
        public static readonly WeaponAbility DoubleStrike = m_Abilities[7];
        public static readonly WeaponAbility InfectiousStrike = m_Abilities[8];
        public static readonly WeaponAbility MortalStrike = m_Abilities[9];
        public static readonly WeaponAbility MovingShot = m_Abilities[10];
        public static readonly WeaponAbility ParalyzingBlow = m_Abilities[11];
        public static readonly WeaponAbility ShadowStrike = m_Abilities[12];
        public static readonly WeaponAbility WhirlwindAttack = m_Abilities[13];

        public static readonly WeaponAbility RidingSwipe = m_Abilities[14];
        public static readonly WeaponAbility FrenziedWhirlwind = m_Abilities[15];
        public static readonly WeaponAbility Block = m_Abilities[16];
        public static readonly WeaponAbility DefenseMastery = m_Abilities[17];
        public static readonly WeaponAbility NerveStrike = m_Abilities[18];
        public static readonly WeaponAbility TalonStrike = m_Abilities[19];
        public static readonly WeaponAbility Feint = m_Abilities[20];
        public static readonly WeaponAbility DualWield = m_Abilities[21];
        public static readonly WeaponAbility DoubleShot = m_Abilities[22];
        public static readonly WeaponAbility ArmorPierce = m_Abilities[23];

        public static readonly WeaponAbility Bladeweave = m_Abilities[24];
        public static readonly WeaponAbility ForceArrow = m_Abilities[25];
        public static readonly WeaponAbility LightningArrow = m_Abilities[26];
        public static readonly WeaponAbility PsychicAttack = m_Abilities[27];
        public static readonly WeaponAbility SerpentArrow = m_Abilities[28];
        public static readonly WeaponAbility ForceOfNature = m_Abilities[29];

        public static readonly WeaponAbility InfusedThrow = m_Abilities[30];
        public static readonly WeaponAbility MysticArc = m_Abilities[31];

        public static readonly WeaponAbility Disrobe = m_Abilities[32];

        public static bool IsWeaponAbility(Mobile m, WeaponAbility a)
        {
            if (a == null)
                return true;

            if (!m.Player)
                return true;

            BaseWeapon weapon = m.Weapon as BaseWeapon;

            return (weapon != null && (weapon.PrimaryAbility == a || weapon.SecondaryAbility == a));
        }

        public virtual bool ValidatesDuringHit
        {
            get
            {
                return true;
            }
        }

        public static WeaponAbility GetCurrentAbility(Mobile m)
        {
            if (!Core.AOS)
            {
                ClearCurrentAbility(m);
                return null;
            }

            WeaponAbility a = (WeaponAbility)m_Table[m];

            if (!IsWeaponAbility(m, a))
            {
                ClearCurrentAbility(m);
                return null;
            }

            if (a != null && a.ValidatesDuringHit && !a.Validate(m))
            {
                ClearCurrentAbility(m);
                return null;
            }

            return a;
        }

        public static bool SetCurrentAbility(Mobile m, WeaponAbility a)
        {
            if (!Core.AOS)
            {
                ClearCurrentAbility(m);
                return false;
            }

            if (!IsWeaponAbility(m, a))
            {
                ClearCurrentAbility(m);
                return false;
            }

            if (a != null && !a.Validate(m))
            {
                ClearCurrentAbility(m);
                return false;
            }

            if (a == null)
            {
                m_Table.Remove(m);
            }
            else
            {
                SpecialMove.ClearCurrentMove(m);

                m_Table[m] = a;
            }

            return true;
        }

        public static void ClearCurrentAbility(Mobile m)
        {
            m_Table.Remove(m);

            if (Core.AOS && m.NetState != null)
                m.Send(ClearWeaponAbility.Instance);
        }

        public static void Initialize()
        {
            EventSink.SetAbility += new SetAbilityEventHandler(EventSink_SetAbility);
        }

        public WeaponAbility()
        {
        }

        private static void EventSink_SetAbility(SetAbilityEventArgs e)
        {
            int index = e.Index;

            if (index == 0)
                ClearCurrentAbility(e.Mobile);
            else if (index >= 1 && index < m_Abilities.Length)
                SetCurrentAbility(e.Mobile, m_Abilities[index]);
        }

        private static readonly Hashtable m_PlayersTable = new Hashtable();

        private static void AddContext(Mobile m, WeaponAbilityContext context)
        {
            m_PlayersTable[m] = context;
        }

        private static void RemoveContext(Mobile m)
        {
            WeaponAbilityContext context = GetContext(m);

            if (context != null)
                RemoveContext(m, context);
        }

        private static void RemoveContext(Mobile m, WeaponAbilityContext context)
        {
            m_PlayersTable.Remove(m);

            context.Timer.Stop();
        }

        private static WeaponAbilityContext GetContext(Mobile m)
        {
            return (m_PlayersTable[m] as WeaponAbilityContext);
        }

        private class WeaponAbilityTimer : Timer
        {
            private readonly Mobile m_Mobile;

            public WeaponAbilityTimer(Mobile from)
                : base(TimeSpan.FromSeconds(3.0))
            {
                this.m_Mobile = from;

                this.Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                RemoveContext(this.m_Mobile);
            }
        }

        private class WeaponAbilityContext
        {
            private readonly Timer m_Timer;

            public Timer Timer
            {
                get
                {
                    return this.m_Timer;
                }
            }

            public WeaponAbilityContext(Timer timer)
            {
                this.m_Timer = timer;
            }
        }
    }
}