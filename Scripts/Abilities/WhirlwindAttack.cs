using Server.Spells;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
    /// <summary>
    /// A godsend to a warrior surrounded, the Whirlwind Attack allows the fighter to strike at all nearby targets in one mighty spinning swing.
    /// </summary>
    public class WhirlwindAttack : WeaponAbility
    {
        public override int BaseMana => 15;

        public static List<WhirlwindAttackContext> Contexts { get; set; } 

        public override bool OnBeforeDamage(Mobile attacker, Mobile defender)
        {
            BaseWeapon wep = attacker.Weapon as BaseWeapon;

            if (wep != null)
                wep.ProcessingMultipleHits = true;

            return true;
        }

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!Validate(attacker) || (Contexts != null && Contexts.Any(c => c.Attacker == attacker)))
                return;

            ClearCurrentAbility(attacker);

            Map map = attacker.Map;

            if (map == null)
                return;

            BaseWeapon weapon = attacker.Weapon as BaseWeapon;

            if (weapon == null)
                return;

            if (!CheckMana(attacker, true))
                return;

            attacker.FixedEffect(0x3728, 10, 15);
            attacker.PlaySound(0x2A1);

            List<Mobile> list = SpellHelper.AcquireIndirectTargets(attacker, attacker, attacker.Map, 1)
                .OfType<Mobile>()
                .Where(m => attacker.InRange(m, weapon.MaxRange) && m != defender).ToList();

            int count = list.Count;

            if (count > 0)
            {
                double bushido = attacker.Skills.Bushido.Value;
                int bonus = 0;

                if (bushido > 0)
                {
                    bonus = (int)Math.Min(100, ((list.Count * bushido) * (list.Count * bushido)) / 3600);
                }

                var context = new WhirlwindAttackContext(attacker, list, bonus);
                AddContext(context);

                attacker.RevealingAction();

                foreach (Mobile m in list)
                {
                    attacker.SendLocalizedMessage(1060161); // The whirling attack strikes a target!
                    m.SendLocalizedMessage(1060162); // You are struck by the whirling attack and take damage!

                    weapon.OnHit(attacker, m);
                }

                RemoveContext(context);
            }

            ColUtility.Free(list);

            weapon.ProcessingMultipleHits = false;
        }

        private static void AddContext(WhirlwindAttackContext context)
        {
            if (Contexts == null)
            {
                Contexts = new List<WhirlwindAttackContext>();
            }

            Contexts.Add(context);
        }

        private static void RemoveContext(WhirlwindAttackContext context)
        {
            if (Contexts != null)
            {
                Contexts.Remove(context);
            }
        }

        public static int DamageBonus(Mobile attacker, Mobile defender)
        {
            if (Contexts == null)
                return 0;

            var context = Contexts.FirstOrDefault(c => c.Attacker == attacker && c.Victims.Contains(defender));

            if (context != null)
            {
                return context.DamageBonus;
            }

            return 0;
        }

        public class WhirlwindAttackContext
        {
            public Mobile Attacker { get; set; }
            public List<Mobile> Victims { get; set; }
            public int DamageBonus { get; set; }

            public WhirlwindAttackContext(Mobile attacker, List<Mobile> list, int bonus)
            {
                Attacker = attacker;
                Victims = list;
                DamageBonus = bonus;
            }
        }
    }
}
