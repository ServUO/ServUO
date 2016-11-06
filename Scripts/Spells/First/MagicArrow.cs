using System;
using Server.Targeting;

namespace Server.Spells.First
{
    public class MagicArrowSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Magic Arrow", "In Por Ylem",
            212,
            9041,
            Reagent.SulfurousAsh);
        public MagicArrowSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.First;
            }
        }
        public override bool DelayedDamageStacking
        {
            get
            {
                return !Core.AOS;
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

        public void Target(IDamageable d)
        {
            Mobile m = d as Mobile;

            if (!this.Caster.CanSee(d))
            {
                this.Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (this.CheckHSequence(d))
            {
                Mobile source = this.Caster;

                SpellHelper.Turn(source, d);

                if(m != null)
                    SpellHelper.CheckReflect((int)this.Circle, ref source, ref m);

                double damage = 0;
				
                if (Core.AOS)
                {
                    damage = this.GetNewAosDamage(10, 1, 4, d);
                }
                else if (m != null)
                {
                    damage = Utility.Random(4, 4);

                    if (this.CheckResisted(m))
                    {
                        damage *= 0.75;

                        m.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
                    }

                    damage *= this.GetDamageScalar(m);
                }

                if (damage > 0)
                {
                    source.MovingParticles(d, 0x36E4, 5, 0, false, false, 3006, 0, 0);
                    source.PlaySound(0x1E5);

                    SpellHelper.Damage(this, d, damage, 0, 100, 0, 0, 0);
                }
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly MagicArrowSpell m_Owner;
            public InternalTarget(MagicArrowSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.Harmful)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is IDamageable)
                {
                    this.m_Owner.Target((IDamageable)o);
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}