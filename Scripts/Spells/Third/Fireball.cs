using System;
using Server.Targeting;

namespace Server.Spells.Third
{
    public class FireballSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Fireball", "Vas Flam",
            203,
            9041,
            Reagent.BlackPearl);
        public FireballSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Third;
            }
        }
        public override bool DelayedDamage
        {
            get
            {
                return true;
            }
        }
        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void Target(IDamageable m)
        {
            if (!Caster.CanSee(m))
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (CheckHSequence(m))
            {
                IDamageable source = Caster;
                IDamageable target = m;

                SpellHelper.Turn(Caster, m);

                if (SpellHelper.CheckReflect((int)Circle, ref source, ref target))
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
                        {
                            source.MovingParticles(target, 0x36D4, 7, 0, false, true, 9502, 4019, 0x160);
                            source.PlaySound(Core.AOS ? 0x15E : 0x44B);
                        });
                }

                double damage = 0;

                if (Core.AOS)
                {
                    damage = GetNewAosDamage(19, 1, 5, m);
                }
                else if (m is Mobile)
                {
                    damage = Utility.Random(10, 7);

                    if (CheckResisted((Mobile)m))
                    {
                        damage *= 0.75;

                        ((Mobile)m).SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
                    }

                    damage *= GetDamageScalar((Mobile)m);
                }

                if (damage > 0)
                {
                    Caster.MovingParticles(m, 0x36D4, 7, 0, false, true, 9502, 4019, 0x160);
                    Caster.PlaySound(Core.AOS ? 0x15E : 0x44B);

                    SpellHelper.Damage(this, target, damage, 0, 100, 0, 0, 0);
                }
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly FireballSpell m_Owner;
            public InternalTarget(FireballSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.Harmful)
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