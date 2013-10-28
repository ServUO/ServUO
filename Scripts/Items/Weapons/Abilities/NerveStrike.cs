using System;

namespace Server.Items
{
    /// <summary>
    /// Does damage and paralyses your opponent for a short time.
    /// </summary>
    public class NerveStrike : WeaponAbility
    {
        public NerveStrike()
        {
        }

        public override int BaseMana
        {
            get
            {
                return 30;
            }
        }
        public override bool CheckSkills(Mobile from)
        {
            if (this.GetSkill(from, SkillName.Bushido) < 50.0)
            {
                from.SendLocalizedMessage(1070768, "50"); // You need ~1_SKILL_REQUIREMENT~ Bushido skill to perform that attack!
                return false;
            }

            return base.CheckSkills(from);
        }

        public override bool OnBeforeSwing(Mobile attacker, Mobile defender)
        {
            if (defender.Paralyzed)
            {
                attacker.SendLocalizedMessage(1061923); // The target is already frozen.
                return false;
            }

            return true;
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!this.Validate(attacker) || !this.CheckMana(attacker, true))
                return;

            ClearCurrentAbility(attacker);

            if (Server.Items.ParalyzingBlow.IsImmune(defender))	//After mana consumption intentional
            {
                attacker.SendLocalizedMessage(1070804); // Your target resists paralysis.
                defender.SendLocalizedMessage(1070813); // You resist paralysis.
                return;
            }

            attacker.SendLocalizedMessage(1063356); // You cripple your target with a nerve strike!
            defender.SendLocalizedMessage(1063357); // Your attacker dealt a crippling nerve strike!

            attacker.PlaySound(0x204);
            defender.FixedEffect(0x376A, 9, 32);
            defender.FixedParticles(0x37C4, 1, 8, 0x13AF, 0, 0, EffectLayer.Waist);

            if (Core.ML)
            {
                AOS.Damage(defender, attacker, (int)(15.0 * (attacker.Skills[SkillName.Bushido].Value - 50.0) / 70.0 + Utility.Random(10)), true, 100, 0, 0, 0, 0);	//0-25
                if (((150.0 / 7.0 + (4.0 * attacker.Skills[SkillName.Bushido].Value) / 7.0) / 100.0) > Utility.RandomDouble())
                {
                    defender.Paralyze(TimeSpan.FromSeconds(2.0));
                    Server.Items.ParalyzingBlow.BeginImmunity(defender, Server.Items.ParalyzingBlow.FreezeDelayDuration);				
                }
            }
            else
            {
                AOS.Damage(defender, attacker, (int)(15.0 * (attacker.Skills[SkillName.Bushido].Value - 50.0) / 70.0 + 10), true, 100, 0, 0, 0, 0); //10-25
                defender.Freeze(TimeSpan.FromSeconds(2.0));
                Server.Items.ParalyzingBlow.BeginImmunity(defender, Server.Items.ParalyzingBlow.FreezeDelayDuration);				
            }
        }
    }
}