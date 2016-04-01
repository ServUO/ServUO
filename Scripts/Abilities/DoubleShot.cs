using System;

namespace Server.Items
{
    /// <summary>
    /// Send two arrows flying at your opponent if you're mounted. Requires Bushido or Ninjitsu skill.
    /// </summary>
    public class DoubleShot : WeaponAbility
    {
        public DoubleShot()
        {
        }

        public override int BaseMana
        {
            get
            {
                return 35;
            }
        }
        public override bool CheckSkills(Mobile from)
        {
            if (this.GetSkill(from, SkillName.Ninjitsu) < 50.0 && this.GetSkill(from, SkillName.Bushido) < 50.0)
            {
                from.SendLocalizedMessage(1063347, "50"); // You need ~1_SKILL_REQUIREMENT~ Bushido or Ninjitsu skill to perform that attack!
                return false;
            }

            return base.CheckSkills(from);
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            this.Use(attacker, defender);
        }

        public override void OnMiss(Mobile attacker, Mobile defender)
        {
            this.Use(attacker, defender);
        }

        public override bool Validate(Mobile from)
        {
            if (base.Validate(from))
            {
                if (from.Mounted)
                    return true;
                else
                {
                    from.SendLocalizedMessage(1070770); // You can only execute this attack while mounted!
                    ClearCurrentAbility(from);
                }
            }

            return false;
        }

        public void Use(Mobile attacker, Mobile defender)
        {
            if (!this.Validate(attacker) || !this.CheckMana(attacker, true) || attacker.Weapon == null)	//sanity
                return;

            ClearCurrentAbility(attacker);

            attacker.SendLocalizedMessage(1063348); // You launch two shots at once!
            defender.SendLocalizedMessage(1063349); // You're attacked with a barrage of shots!

            defender.FixedParticles(0x37B9, 1, 19, 0x251D, EffectLayer.Waist);

            attacker.Weapon.OnSwing(attacker, defender);
        }
    }
}