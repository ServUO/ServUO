using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Spells.Necromancy
{
    public class WitherSpell : NecromancerSpell
    {
        public override DamageType SpellDamageType => DamageType.SpellAOE;

        private static readonly SpellInfo m_Info = new SpellInfo(
            "Wither", "Kal Vas An Flam",
            203,
            9031,
            Reagent.NoxCrystal,
            Reagent.GraveDust,
            Reagent.PigIron);
        public WitherSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(1.5);
        public override double RequiredSkill => 60.0;
        public override int RequiredMana => 23;
        public override bool DelayedDamage => false;
        public override void OnCast()
        {
            if (CheckSequence())
            {
                /* Creates a withering frost around the Caster,
                * which deals Cold Damage to all valid targets in a radius of 5 tiles.
                */
                Map map = Caster.Map;

                if (map != null)
                {
                    Effects.PlaySound(Caster.Location, map, 0x1FB);
                    Effects.PlaySound(Caster.Location, map, 0x10B);
                    Effects.SendLocationParticles(EffectItem.Create(Caster.Location, map, EffectItem.DefaultDuration), 0x37CC, 1, 40, 97, 3, 9917, 0);

                    foreach (IDamageable id in AcquireIndirectTargets(Caster.Location, 4))
                    {
                        Mobile m = id as Mobile;

                        Caster.DoHarmful(id);

                        if (m != null)
                        {
                            m.FixedParticles(0x374A, 1, 15, 9502, 97, 3, (EffectLayer)255);
                        }
                        else
                        {
                            Effects.SendLocationParticles(id, 0x374A, 1, 30, 97, 3, 9502, 0);
                        }

                        double damage = Utility.RandomMinMax(30, 35);
                        int karma = m != null ? m.Karma / 100 : 0;

                        damage *= 300 + karma + (GetDamageSkill(Caster) * 10);
                        damage /= 1000;

                        int sdiBonus = SpellHelper.GetSpellDamageBonus(Caster, m, CastSkill, m is PlayerMobile);

                        damage *= (100 + sdiBonus);
                        damage /= 100;

                        SpellHelper.Damage(this, id, damage, 0, 0, 100, 0, 0);
                    }
                }
            }

            FinishSequence();
        }
    }
}
