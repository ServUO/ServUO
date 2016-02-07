using System;
using Server.Targeting;

namespace Server.Spells.Second
{
    public class HarmSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Harm", "An Mani",
            212,
            Core.AOS ? 9001 : 9041,
            Reagent.Nightshade,
            Reagent.SpidersSilk);
        public HarmSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Second;
            }
        }
        public override bool DelayedDamage
        {
            get
            {
                return false;
            }
        }
        public override void OnCast()
        {
            this.Caster.Target = new InternalTarget(this);
        }

        public override double GetSlayerDamageScalar(Mobile target)
        {
            return 1.0; //This spell isn't affected by slayer spellbooks
        }

        public void Target(Mobile m)
        {
            if (!this.Caster.CanSee(m))
            {
                this.Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (this.CheckHSequence(m))
            {
                SpellHelper.Turn(this.Caster, m);

                SpellHelper.CheckReflect((int)this.Circle, this.Caster, ref m);

                double damage;
				
                if (Core.AOS)
                {
                    damage = this.GetNewAosDamage(17, 1, 5, m);
                }
                else
                {
                    damage = Utility.Random(1, 15);

                    if (this.CheckResisted(m))
                    {
                        damage *= 0.75;

                        m.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
                    }

                    damage *= this.GetDamageScalar(m);
                }

                if (!m.InRange(this.Caster, 2))
                    damage *= 0.25; // 1/4 damage at > 2 tile range
                else if (!m.InRange(this.Caster, 1))
                    damage *= 0.50; // 1/2 damage at 2 tile range

                if (Core.AOS)
                {
                    m.FixedParticles(0x374A, 10, 30, 5013, 1153, 2, EffectLayer.Waist);
                    m.PlaySound(0x0FC);
                }
                else
                {
                    m.FixedParticles(0x374A, 10, 15, 5013, EffectLayer.Waist);
                    m.PlaySound(0x1F1);
                }

                SpellHelper.Damage(this, m, damage, 0, 0, 100, 0, 0);
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly HarmSpell m_Owner;
            public InternalTarget(HarmSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.Harmful)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                {
                    this.m_Owner.Target((Mobile)o);
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}