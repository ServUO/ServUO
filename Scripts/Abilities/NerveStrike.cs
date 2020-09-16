using Server.Mobiles;
using System;

namespace Server.Items
{
    /// <summary>
    /// Does damage and paralyses your opponent for a short time.
    /// </summary>
    public class NerveStrike : WeaponAbility
    {
        public override int BaseMana => 30;

        public override bool CheckSkills(Mobile from)
        {
            if (GetSkill(from, SkillName.Bushido) < 50.0)
            {
                from.SendLocalizedMessage(1070768, "50"); // You need ~1_SKILL_REQUIREMENT~ Bushido skill to perform that attack!
                return false;
            }

            return base.CheckSkills(from);
        }

        public override bool OnBeforeSwing(Mobile attacker, Mobile defender)
        {
            return true;
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!Validate(attacker) || !CheckMana(attacker, true))
                return;

            ClearCurrentAbility(attacker);

            bool immune = Items.ParalyzingBlow.IsImmune(defender);
            bool doEffects = false;

            AOS.Damage(defender, attacker, (int)(15.0 * (attacker.Skills[SkillName.Bushido].Value - 50.0) / 70.0 + Utility.Random(10)), true, 100, 0, 0, 0, 0);	//0-25

            if (!immune && ((150.0 / 7.0 + (4.0 * attacker.Skills[SkillName.Bushido].Value) / 7.0) / 100.0) > Utility.RandomDouble())
            {
                defender.Paralyze(TimeSpan.FromSeconds(2.0));
                doEffects = true;
            }

            if (attacker is BaseCreature)
                PetTrainingHelper.OnWeaponAbilityUsed((BaseCreature)attacker, SkillName.Bushido);

            if (!immune)
            {
                attacker.SendLocalizedMessage(1063356); // You cripple your target with a nerve strike!
                defender.SendLocalizedMessage(1063357); // Your attacker dealt a crippling nerve strike!
            }
            else
            {
                attacker.SendLocalizedMessage(1070804); // Your target resists paralysis.
                defender.SendLocalizedMessage(1070813); // You resist paralysis.
            }

            if (doEffects)
            {
                attacker.PlaySound(0x204);
                defender.FixedEffect(0x376A, 9, 32);
                defender.FixedParticles(0x37C4, 1, 8, 0x13AF, 0, 0, EffectLayer.Waist);
            }

            Items.ParalyzingBlow.BeginImmunity(defender, Items.ParalyzingBlow.FreezeDelayDuration);
        }
    }
}
