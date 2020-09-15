using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;
using Server.Spells.SkillMasteries;
using System;
using System.Collections.Generic;

namespace Server.Misc
{
    public delegate int RegenBonusHandler(Mobile from);

    public class RegenRates
    {
        public static List<RegenBonusHandler> HitsBonusHandlers = new List<RegenBonusHandler>();
        public static List<RegenBonusHandler> StamBonusHandlers = new List<RegenBonusHandler>();
        public static List<RegenBonusHandler> ManaBonusHandlers = new List<RegenBonusHandler>();

        [CallPriority(10)]
        public static void Configure()
        {
            Mobile.DefaultHitsRate = TimeSpan.FromSeconds(11.0);
            Mobile.DefaultStamRate = TimeSpan.FromSeconds(7.0);
            Mobile.DefaultManaRate = TimeSpan.FromSeconds(7.0);

            Mobile.ManaRegenRateHandler = Mobile_ManaRegenRate;
            Mobile.StamRegenRateHandler = Mobile_StamRegenRate;
            Mobile.HitsRegenRateHandler = Mobile_HitsRegenRate;
        }

        public static double GetArmorOffset(Mobile from)
        {
            double rating = 0.0;

            rating += GetArmorMeditationValue(from.NeckArmor as BaseArmor);
            rating += GetArmorMeditationValue(from.HandArmor as BaseArmor);
            rating += GetArmorMeditationValue(from.HeadArmor as BaseArmor);
            rating += GetArmorMeditationValue(from.ArmsArmor as BaseArmor);
            rating += GetArmorMeditationValue(from.LegsArmor as BaseArmor);
            rating += GetArmorMeditationValue(from.ChestArmor as BaseArmor);

            return rating / 4;
        }

        private static void CheckBonusSkill(Mobile m, int cur, int max, SkillName skill)
        {
            if (!m.Alive)
                return;

            double n = (double)cur / max;
            double v = Math.Sqrt(m.Skills[skill].Value * 0.005);

            n *= (1.0 - v);
            n += v;

            m.CheckSkill(skill, n);
        }

        public static bool CheckTransform(Mobile m, Type type)
        {
            return TransformationSpellHelper.UnderTransformation(m, type);
        }

        public static bool CheckAnimal(Mobile m, Type type)
        {
            return AnimalForm.UnderTransformation(m, type);
        }

        private static TimeSpan Mobile_HitsRegenRate(Mobile from)
        {
            return TimeSpan.FromSeconds(1.0 / (0.1 * (1 + HitPointRegen(from))));
        }

        private static TimeSpan Mobile_StamRegenRate(Mobile from)
        {
            if (from.Skills == null)
                return Mobile.DefaultStamRate;

            CheckBonusSkill(from, from.Stam, from.StamMax, SkillName.Focus);

            double bonus = from.Skills[SkillName.Focus].Value * 0.1;

            bonus += StamRegen(from);

            double rate = 1.0 / (1.42 + (bonus / 100));

            if (from is BaseCreature && ((BaseCreature)from).IsMonster)
            {
                rate *= 1.95;
            }

            return TimeSpan.FromSeconds(rate);

        }

        private static TimeSpan Mobile_ManaRegenRate(Mobile from)
        {
            if (from.Skills == null)
                return Mobile.DefaultManaRate;

            if (!from.Meditating)
                CheckBonusSkill(from, from.Mana, from.ManaMax, SkillName.Meditation);

            double rate;
            double armorPenalty = GetArmorOffset(from);


            double med = from.Skills[SkillName.Meditation].Value;
            double focus = from.Skills[SkillName.Focus].Value;

            double focusBonus = focus / 200;
            double medBonus = 0;

            CheckBonusSkill(from, from.Mana, from.ManaMax, SkillName.Focus);

            if (armorPenalty == 0)
            {
                medBonus = (0.0075 * med) + (0.0025 * from.Int);

                if (medBonus >= 100.0)
                    medBonus *= 1.1;

                if (from.Meditating)
                {
                    medBonus *= 2;
                }
            }

            double itemBase = ((((med / 2) + (focus / 4)) / 90) * .65) + 2.35;
            double intensityBonus = Math.Sqrt(ManaRegen(from));

            if (intensityBonus > 5.5)
                intensityBonus = 5.5;

            double itemBonus = ((itemBase * intensityBonus) - (itemBase - 1)) / 10;

            rate = 1.0 / (0.2 + focusBonus + medBonus + itemBonus);

            if (double.IsNaN(rate))
            {
                return Mobile.DefaultManaRate;
            }

            return TimeSpan.FromSeconds(rate);
        }

        public static double HitPointRegen(Mobile from)
        {
            double points = AosAttributes.GetValue(from, AosAttribute.RegenHits);

            if (from is BaseCreature)
                points += ((BaseCreature)from).DefaultHitsRegen;

            if (from is PlayerMobile && from.Race == Race.Human)	//Is this affected by the cap?
                points += 2;

            if (points < 0)
                points = 0;

            if (from is PlayerMobile)	//does racial bonus go before/after?
                points = Math.Min(points, 18);

            if (CheckTransform(from, typeof(HorrificBeastSpell)))
                points += 20;

            if (CheckAnimal(from, typeof(Dog)) || CheckAnimal(from, typeof(Cat)))
                points += from.Skills[SkillName.Ninjitsu].Fixed / 30;

            // Skill Masteries - goes after cap
            points += RampageSpell.GetBonus(from, RampageSpell.BonusType.HitPointRegen);
            points += CombatTrainingSpell.RegenBonus(from);
            points += BarrabHemolymphConcentrate.HPRegenBonus(from);

            foreach (RegenBonusHandler handler in HitsBonusHandlers)
                points += handler(from);

            return points;
        }

        public static double StamRegen(Mobile from)
        {
            double points = AosAttributes.GetValue(from, AosAttribute.RegenStam);

            if (from is BaseCreature)
                points += ((BaseCreature)from).DefaultStamRegen;

            if (CheckTransform(from, typeof(VampiricEmbraceSpell)))
                points += 15;

            if (CheckAnimal(from, typeof(Kirin)))
                points += 20;

            if (from is PlayerMobile)
                points = Math.Min(points, 24);

            // Skill Masteries - goes after cap
            points += RampageSpell.GetBonus(from, RampageSpell.BonusType.StamRegen);

            if (points < -1)
                points = -1;

            foreach (RegenBonusHandler handler in StamBonusHandlers)
                points += handler(from);

            return points;
        }

        public static double ManaRegen(Mobile from)
        {
            double points = AosAttributes.GetValue(from, AosAttribute.RegenMana);

            if (from is BaseCreature)
                points += ((BaseCreature)from).DefaultManaRegen;

            if (CheckTransform(from, typeof(VampiricEmbraceSpell)))
                points += 3;
            else if (CheckTransform(from, typeof(LichFormSpell)))
                points += 13;

            if (from is PlayerMobile && from.Race == Race.Gargoyle)
                points += 2;

            foreach (RegenBonusHandler handler in ManaBonusHandlers)
                points += handler(from);

            return points;
        }

        public static double GetArmorMeditationValue(BaseArmor ar)
        {
            if (ar == null || ar.ArmorAttributes.MageArmor != 0 || ar.Attributes.SpellChanneling != 0)
                return 0.0;

            switch (ar.MeditationAllowance)
            {
                default:
                case ArmorMeditationAllowance.None:
                    return ar.BaseArmorRatingScaled;
                case ArmorMeditationAllowance.Half:
                    return ar.BaseArmorRatingScaled / 2.0;
                case ArmorMeditationAllowance.All:
                    return 0.0;
            }
        }
    }
}
