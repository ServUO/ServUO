using Server.Network;
using Server.Spells;
using Server.Spells.SkillMasteries;
using System;
using System.Collections;

namespace Server.Items
{
    public abstract class WeaponAbility
    {
        public virtual int BaseMana => 0;

        public virtual int AccuracyBonus => 0;

        public virtual double DamageScalar => 1.0;

        /// <summary>
        ///		Return false to make this special ability consume no ammo from ranged weapons
        /// </summary>
        public virtual bool ConsumeAmmo => true;

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

        public virtual bool RequiresSecondarySkill(Mobile from)
        {
            return true;
        }

        public virtual double GetRequiredSkill(Mobile from)
        {
            BaseWeapon weapon = from.Weapon as BaseWeapon;

            if (weapon != null && (weapon.PrimaryAbility == this || weapon.PrimaryAbility == Bladeweave))
                return 70.0;
            else if (weapon != null && (weapon.SecondaryAbility == this || weapon.SecondaryAbility == Bladeweave))
                return 90.0;

            return 200.0;
        }

        public virtual double GetRequiredSecondarySkill(Mobile from)
        {
            if (!RequiresSecondarySkill(from))
                return 0.0;

            BaseWeapon weapon = from.Weapon as BaseWeapon;

            if (weapon != null && (weapon.PrimaryAbility == this || weapon.PrimaryAbility == Bladeweave))
                return 30.0;
            else if (weapon != null && (weapon.SecondaryAbility == this || weapon.SecondaryAbility == Bladeweave))
                return 60.0;

            return 200.0;
        }

        public virtual SkillName GetSecondarySkill(Mobile from)
        {
            return SkillName.Tactics;
        }

        public virtual int CalculateMana(Mobile from)
        {
            int mana = BaseMana;

            double skillTotal = GetSkillTotal(from);

            if (skillTotal >= 300.0)
                mana -= 10;
            else if (skillTotal >= 200.0)
                mana -= 5;

            double scalar = 1.0;

            if (!Spells.Necromancy.MindRotSpell.GetMindRotScalar(from, ref scalar))
            {
                scalar = 1.0;
            }

            if (Spells.Mysticism.PurgeMagicSpell.IsUnderCurseEffects(from))
            {
                scalar += .5;
            }

            // Lower Mana Cost = 40%
            int lmc = Math.Min(AosAttributes.GetValue(from, AosAttribute.LowerManaCost), 40);

            lmc += BaseArmor.GetInherentLowerManaCost(from);

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

            double reqSkill = GetRequiredSkill(from);
            double reqSecondarySkill = GetRequiredSecondarySkill(from);
            SkillName secondarySkill = GetSecondarySkill(from);

            if (from.Skills[secondarySkill].Base < reqSecondarySkill)
            {
                int loc = GetSkillLocalization(secondarySkill);

                if (loc == 1060184)
                {
                    from.SendLocalizedMessage(loc);
                }
                else
                {
                    from.SendLocalizedMessage(loc, reqSecondarySkill.ToString());
                }

                return false;
            }

            if (skill != null && skill.Base >= reqSkill)
                return true;

            /* <UBWS> */
            if (weapon.WeaponAttributes.UseBestSkill > 0 && (from.Skills[SkillName.Swords].Base >= reqSkill || from.Skills[SkillName.Macing].Base >= reqSkill || from.Skills[SkillName.Fencing].Base >= reqSkill))
                return true;
            /* </UBWS> */

            from.SendLocalizedMessage(1060182, reqSkill.ToString()); // You need ~1_SKILL_REQUIREMENT~ weapon skill to perform that attack

            return false;
        }

        private int GetSkillLocalization(SkillName skill)
        {
            switch (skill)
            {
                default: return 1157351;
                // You need ~1_SKILL_REQUIREMENT~ weapon and tactics skill to perform that attack                                                             
                // You need ~1_SKILL_REQUIREMENT~ tactics skill to perform that attack
                case SkillName.Bushido:
                case SkillName.Ninjitsu: return 1063347;
                // You need ~1_SKILL_REQUIREMENT~ Bushido or Ninjitsu skill to perform that attack!
                case SkillName.Poisoning: return 1060184;
                    // You lack the required poisoning to perform that attack
            }
        }

        public virtual bool CheckSkills(Mobile from)
        {
            return CheckWeaponSkill(from);
        }

        public virtual double GetSkillTotal(Mobile from)
        {
            return GetSkill(from, SkillName.Swords) + GetSkill(from, SkillName.Macing) +
                   GetSkill(from, SkillName.Fencing) + GetSkill(from, SkillName.Archery) + GetSkill(from, SkillName.Parry) +
                   GetSkill(from, SkillName.Lumberjacking) + GetSkill(from, SkillName.Stealth) + GetSkill(from, SkillName.Throwing) +
                   GetSkill(from, SkillName.Poisoning) + GetSkill(from, SkillName.Bushido) + GetSkill(from, SkillName.Ninjitsu);
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
            int mana = CalculateMana(from);

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
            if (!from.Player && CheckMana(from, false))
                return true;

            NetState state = from.NetState;

            if (state == null)
                return false;

            if (Spells.Bushido.HonorableExecution.IsUnderPenalty(from) || Spells.Ninjitsu.AnimalForm.UnderTransformation(from))
            {
                from.SendLocalizedMessage(1063024); // You cannot perform this special move right now.
                return false;
            }

            if (from.Spell != null)
            {
                from.SendLocalizedMessage(1063024); // You cannot perform this special move right now.
                return false;
            }

            return CheckSkills(from) && CheckMana(from, false);
        }

        private static readonly WeaponAbility[] m_Abilities = new WeaponAbility[34]
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
            null,
            new ColdWind()
        };

        public static WeaponAbility[] Abilities => m_Abilities;

        private static readonly Hashtable m_Table = new Hashtable();

        public static Hashtable Table => m_Table;

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

        public static readonly WeaponAbility Empty = m_Abilities[32];
        public static readonly WeaponAbility ColdWind = m_Abilities[33];

        public static bool IsWeaponAbility(Mobile m, WeaponAbility a)
        {
            if (a == null)
                return true;

            if (!m.Player)
                return true;

            BaseWeapon weapon = m.Weapon as BaseWeapon;

            return (weapon != null && (weapon.PrimaryAbility == a || weapon.SecondaryAbility == a));
        }

        public virtual bool ValidatesDuringHit => true;

        public static WeaponAbility GetCurrentAbility(Mobile m)
        {
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

                SkillMasterySpell.CancelWeaponAbility(m);
            }

            return true;
        }

        public static void ClearCurrentAbility(Mobile m)
        {
            m_Table.Remove(m);

            if (m.NetState != null)
                m.Send(ClearWeaponAbility.Instance);
        }

        public static void Initialize()
        {
            EventSink.SetAbility += EventSink_SetAbility;
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
                m_Mobile = from;

                Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                RemoveContext(m_Mobile);
            }
        }

        private class WeaponAbilityContext
        {
            private readonly Timer m_Timer;

            public Timer Timer => m_Timer;

            public WeaponAbilityContext(Timer timer)
            {
                m_Timer = timer;
            }
        }
    }
}
