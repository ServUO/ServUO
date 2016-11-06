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
            this.Caster.Target = new InternalTarget(this);
        }

        public void Target(IDamageable m)
        {
            if (!this.Caster.CanSee(m))
            {
                this.Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (this.CheckHSequence(m))
            {
                Mobile source = this.Caster;
                Mobile target = m as Mobile;

                SpellHelper.Turn(source, m);

                if(target != null)
                    SpellHelper.CheckReflect((int)this.Circle, ref source, ref target);

                double damage = 0;

                if (Core.AOS)
                {
                    damage = this.GetNewAosDamage(19, 1, 5, m);
                }
                else if (target != null)
                {
                    damage = Utility.Random(10, 7);

                    if (this.CheckResisted(target))
                    {
                        damage *= 0.75;

                        target.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
                    }

                    damage *= this.GetDamageScalar(target);
                }

                if (damage > 0)
                {
                    source.MovingParticles(m, 0x36D4, 7, 0, false, true, 9502, 4019, 0x160);
                    source.PlaySound(Core.AOS ? 0x15E : 0x44B);

                    SpellHelper.Damage(this, m, damage, 0, 100, 0, 0, 0);
                }
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly FireballSpell m_Owner;
            public InternalTarget(FireballSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.Harmful)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is IDamageable)
                    this.m_Owner.Target((IDamageable)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}