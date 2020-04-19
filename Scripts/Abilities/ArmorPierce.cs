using System;
using System.Collections.Generic;

namespace Server.Items
{
    /// <summary>
    /// Strike your opponent with great force, partially bypassing their armor and inflicting greater damage. Requires either Bushido or Ninjitsu skill
    /// </summary>
    public class ArmorPierce : WeaponAbility
    {
        public static Dictionary<Mobile, Timer> _Table = new Dictionary<Mobile, Timer>();

        public override SkillName GetSecondarySkill(Mobile from)
        {
            return from.Skills[SkillName.Ninjitsu].Base > from.Skills[SkillName.Bushido].Base ? SkillName.Ninjitsu : SkillName.Bushido;
        }

        public override int BaseMana => 30;

        public override double DamageScalar => 1.0;

        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!Validate(attacker) || !CheckMana(attacker, true))
                return;

            ClearCurrentAbility(attacker);

            attacker.SendLocalizedMessage(1063350); // You pierce your opponent's armor!

            defender.SendLocalizedMessage(1153764); // Your armor has been pierced!
            defender.SendLocalizedMessage(1063351); // Your attacker pierced your armor!            

            if (_Table.ContainsKey(defender))
            {
                if (attacker.Weapon is BaseRanged)
                    return;

                _Table[defender].Stop();
            }

            BuffInfo.AddBuff(defender, new BuffInfo(BuffIcon.ArmorPierce, 1028860, 1154367, TimeSpan.FromSeconds(3), defender, "10"));
            _Table[defender] = Timer.DelayCall(TimeSpan.FromSeconds(3), RemoveEffects, defender);

            defender.PlaySound(0x28E);
            defender.FixedParticles(0x3728, 1, 26, 0x26D6, 0, 0, EffectLayer.Waist);
        }

        public static void RemoveEffects(Mobile m)
        {
            if (IsUnderEffects(m))
            {
                m.SendLocalizedMessage(1153904); // Your armor has returned to normal.
                _Table.Remove(m);
            }
        }

        public static bool IsUnderEffects(Mobile m)
        {
            if (m == null)
                return false;

            return _Table.ContainsKey(m);
        }
    }
}
