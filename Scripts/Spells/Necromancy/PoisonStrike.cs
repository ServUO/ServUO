using Server.Items;
using Server.Mobiles;
using Server.Spells.SkillMasteries;
using Server.Targeting;
using System;

namespace Server.Spells.Necromancy
{
    public class PoisonStrikeSpell : NecromancerSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Poison Strike", "In Vas Nox",
            203,
            9031,
            Reagent.NoxCrystal);

        public PoisonStrikeSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override DamageType SpellDamageType => DamageType.SpellAOE;

        public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(2.0);
        public override double RequiredSkill => 50.0;
        public override int RequiredMana => 17;
        public override bool DelayedDamage => false;
        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void Target(IDamageable m)
        {
            if (CheckHSequence(m))
            {
                Mobile mob = m as Mobile;
                SpellHelper.Turn(Caster, m);

                SpellHelper.CheckReflect(this, Caster, ref m);

                ApplyEffects(m);
                ConduitSpell.CheckAffected(Caster, m, ApplyEffects);
            }

            FinishSequence();
        }

        public void ApplyEffects(IDamageable m, double strength = 1.0)
        {
            /* Creates a blast of poisonous energy centered on the target.
                * The main target is inflicted with a large amount of Poison damage, and all valid targets in a radius of 2 tiles around the main target are inflicted with a lesser effect.
                * One tile from main target receives 50% damage, two tiles from target receives 33% damage.
                */

            Effects.SendLocationParticles(EffectItem.Create(m.Location, m.Map, EffectItem.DefaultDuration), 0x36B0, 1, 14, 63, 7, 9915, 0);
            Effects.PlaySound(m.Location, m.Map, 0x229);

            double damage = Utility.RandomMinMax(32, 40) * ((300 + (GetDamageSkill(Caster) * 9)) / 1000);
            damage *= strength;

            double sdiBonus = (double)SpellHelper.GetSpellDamageBonus(Caster, m, CastSkill, m is PlayerMobile) / 100;

            double pvmDamage = (damage * (1 + sdiBonus)) * strength;
            double pvpDamage = damage * (1 + sdiBonus);

            Map map = m.Map;

            if (map != null)
            {
                foreach (IDamageable id in AcquireIndirectTargets(m.Location, 2))
                {
                    int num;

                    if (Utility.InRange(id.Location, m.Location, 0))
                        num = 1;
                    else if (Utility.InRange(id.Location, m.Location, 1))
                        num = 2;
                    else
                        num = 3;

                    Caster.DoHarmful(id);
                    SpellHelper.Damage(this, id, ((id is PlayerMobile && Caster.Player) ? pvpDamage : pvmDamage) / num, 0, 0, 0, 100, 0);
                }
            }
        }

        private class InternalTarget : Target
        {
            private readonly PoisonStrikeSpell m_Owner;
            public InternalTarget(PoisonStrikeSpell owner)
                : base(10, false, TargetFlags.Harmful)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is IDamageable)
                    m_Owner.Target((IDamageable)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}
