using System;
using Server.Targeting;

namespace Server.Spells.Seventh
{
    public class FlameStrikeSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Flame Strike", "Kal Vas Flam",
            245,
            9042,
            Reagent.SpidersSilk,
            Reagent.SulfurousAsh);
        public FlameStrikeSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Seventh;
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
            this.Caster.Target = new InternalTarget(this);
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
                    damage = this.GetNewAosDamage(48, 1, 5, m);
                }
                else
                {
                    damage = Utility.Random(27, 22);

                    if (this.CheckResisted(m))
                    {
                        damage *= 0.6;

                        m.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
                    }

                    damage *= this.GetDamageScalar(m);
                }

                m.FixedParticles(0x3709, 10, 30, 5052, EffectLayer.LeftFoot);
                m.PlaySound(0x208);

                SpellHelper.Damage(this, m, damage, 0, 100, 0, 0, 0);
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly FlameStrikeSpell m_Owner;
            public InternalTarget(FlameStrikeSpell owner)
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