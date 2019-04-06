using System;
using System.Collections.Generic;
using System.Linq;

using Server.Spells;

namespace Server.Items
{
    /// <summary>
    /// A godsend to a warrior surrounded, the Whirlwind Attack allows the fighter to strike at all nearby targets in one mighty spinning swing.
    /// </summary>
    public class WhirlwindAttack : WeaponAbility
    {
        public WhirlwindAttack()
        {
        }

        public override int BaseMana
        {
            get
            {
                return 15;
            }
        }

        public override bool OnBeforeDamage(Mobile attacker, Mobile defender)
        {
            BaseWeapon wep = attacker.Weapon as BaseWeapon;

            if (wep != null)
                wep.ProcessingMultipleHits = true;

            return true;
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!this.Validate(attacker))
                return;

            ClearCurrentAbility(attacker);

            Map map = attacker.Map;

            if (map == null)
                return;

            BaseWeapon weapon = attacker.Weapon as BaseWeapon;

            if (weapon == null)
                return;

            if (!this.CheckMana(attacker, true))
                return;

            attacker.FixedEffect(0x3728, 10, 15);
            attacker.PlaySound(0x2A1);

            var list = SpellHelper.AcquireIndirectTargets(attacker, attacker, attacker.Map, 1)
                .OfType<Mobile>()
                .Where(m => attacker.InRange(m, weapon.MaxRange) && m != defender).ToList();

            int count = list.Count;

            if (count > 0)
            {
                double bushido = attacker.Skills.Bushido.Value;
                double damageBonus = 1.0 + Math.Pow((count * bushido) / 60, 2) / 100;

                if (damageBonus > 2.0)
                    damageBonus = 2.0;
					
                attacker.RevealingAction();

                foreach(var m in list)
                {
                    attacker.SendLocalizedMessage(1060161); // The whirling attack strikes a target!
                    m.SendLocalizedMessage(1060162); // You are struck by the whirling attack and take damage!

                    weapon.OnHit(attacker, m, damageBonus);
                }
            }

            ColUtility.Free(list);

            weapon.ProcessingMultipleHits = false;
        }
    }
}