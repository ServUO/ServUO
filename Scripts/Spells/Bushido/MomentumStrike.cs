using Server.Items;
using System.Collections.Generic;

namespace Server.Spells.Bushido
{
    public class MomentumStrike : SamuraiMove
    {
        public override int BaseMana => 10;
        public override double RequiredSkill => 70.0;
        public override TextDefinition AbilityMessage => new TextDefinition(1070757);// You prepare to strike two enemies with one blow.
        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!Validate(attacker) || !CheckMana(attacker, false))
                return;

            ClearCurrentMove(attacker);

            BaseWeapon weapon = attacker.Weapon as BaseWeapon;

            List<Mobile> targets = new List<Mobile>();
            IPooledEnumerable eable = attacker.GetMobilesInRange(weapon.MaxRange);

            foreach (Mobile m in eable)
            {
                if (m != defender && m != attacker && m.CanBeHarmful(attacker, false) && attacker.InLOS(m) &&
                    SpellHelper.ValidIndirectTarget(attacker, m))
                {
                    targets.Add(m);
                }
            }
            eable.Free();

            if (targets.Count > 0)
            {
                if (!CheckMana(attacker, true))
                    return;

                Mobile target = targets[Utility.Random(targets.Count)];

                double damageBonus = attacker.Skills[SkillName.Bushido].Value / 100.0;

                if (!defender.Alive)
                    damageBonus *= 1.5;

                attacker.SendLocalizedMessage(1063171); // You transfer the momentum of your weapon into another enemy!
                target.SendLocalizedMessage(1063172); // You were hit by the momentum of a Samurai's weapon!

                target.FixedParticles(0x37B9, 1, 4, 0x251D, 0, 0, EffectLayer.Waist);

                attacker.PlaySound(0x510);

                weapon.OnSwing(attacker, target, damageBonus);

                if (defender.Alive)
                    attacker.Combatant = defender;

                CheckGain(attacker);
            }
            else
            {
                attacker.SendLocalizedMessage(1063123); // There are no valid targets to attack!
            }

            ColUtility.Free(targets);
        }

        public override void OnUse(Mobile m)
        {
            base.OnUse(m);

            BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.MomentumStrike, 1060600, 1063268));
        }

        public override void OnClearMove(Mobile from)
        {
            base.OnClearMove(from);

            BuffInfo.RemoveBuff(from, BuffIcon.MomentumStrike);
        }

        public override void CheckGain(Mobile m)
        {
            m.CheckSkill(MoveSkill, RequiredSkill, 120.0);
        }
    }
}
