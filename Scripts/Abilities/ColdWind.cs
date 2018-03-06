using System;
using System.Collections.Generic;

namespace Server.Items
{
    /// <summary>
    /// Currently on EA, this is only available for Creatures
    /// </summary>
    public class ColdWind : WeaponAbility
    {
        public ColdWind()
        {
        }

        public override int BaseMana
        {
            get
            {
                return 20;
            }
        }
        public override double DamageScalar
        {
            get
            {
                return 1.5;
            }
        }
        public override void OnHit(Mobile attacker, Mobile defender, int damage)
        {
            if (!this.Validate(attacker) || !this.CheckMana(attacker, true))
                return;

            if (attacker.Map == null || attacker.Map == Map.Internal)
                return;

            IPooledEnumerable eable = attacker.GetMobilesInRange(4);
            List<Mobile> toAffect = new List<Mobile>();

            foreach (Mobile m in eable)
            {
                if (m.Alive && !m.IsDeadBondedPet &&
                    m.CanBeHarmful(creature) &&
                    SpellHelper.ValidIndirectTarget(m, creature) &&
                    (!Core.AOS || creature.InLOS(m)))
                {
                    toAffect.Add(m);
                }
            }

            eable.Free();

            foreach (var m in toEffect)
            {
                AOS.Damage(m, attacker, Utility.RandomMinMax(20, 30), 0, 0, 100, 0, 0);
                m.SendLocalizedMessage(1008111, false, this.Name); //  : The intense cold is damaging you!

                m.FixedParticles(0x374A, 10, 30, 5052, 1319, 0, EffectLayer.Waist);
                m.PlaySound(0x5C6);
            }

            ColUtility.Free(toEffect);
        }
    }
}