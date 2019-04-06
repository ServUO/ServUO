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
                return Core.TOL ? 30 : 35;
            }
        }

        public override bool OnBeforeDamage(Mobile attacker, Mobile defender)
        {
            BaseWeapon wep = attacker.Weapon as BaseWeapon;

            if (wep != null)
                wep.ProcessingMultipleHits = true;

            return true;
        }

        public override SkillName GetSecondarySkill(Mobile from)
        {
            return from.Skills[SkillName.Ninjitsu].Base > from.Skills[SkillName.Bushido].Base ? SkillName.Ninjitsu : SkillName.Bushido;
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            Use(attacker, defender);
        }

        public override void OnMiss(Mobile attacker, Mobile defender)
        {
            Use(attacker, defender);
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
            if (!Validate(attacker) || !CheckMana(attacker, true) || attacker.Weapon == null)	//sanity
                return;

            ClearCurrentAbility(attacker);

            attacker.SendLocalizedMessage(1063348); // You launch two shots at once!
            defender.SendLocalizedMessage(1063349); // You're attacked with a barrage of shots!

            defender.FixedParticles(0x37B9, 1, 19, 0x251D, EffectLayer.Waist);

            attacker.Weapon.OnSwing(attacker, defender);

            if (attacker.Weapon is BaseWeapon)
                ((BaseWeapon)attacker.Weapon).ProcessingMultipleHits = false;
        }
    }
}