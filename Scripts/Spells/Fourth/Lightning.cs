using System;
using Server.Targeting;

namespace Server.Spells.Fourth
{
    public class LightningSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Lightning", "Por Ort Grav",
            239,
            9021,
            Reagent.MandrakeRoot,
            Reagent.SulfurousAsh);
        public LightningSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Fourth;
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

        public void Target(IDamageable m)
        {
            Mobile mob = m as Mobile;

            if (!this.Caster.CanSee(m))
            {
                this.Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (this.CheckHSequence(m))
            {
                SpellHelper.Turn(this.Caster, m);

                if(mob != null)
                    SpellHelper.CheckReflect((int)this.Circle, this.Caster, ref mob);

                double damage = 0;

                if (Core.AOS)
                {
                    damage = this.GetNewAosDamage(23, 1, 4, m);
                }
                else if (mob != null)
                {
                    damage = Utility.Random(12, 9);

                    if (this.CheckResisted(mob))
                    {
                        damage *= 0.75;

                        mob.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
                    }

                    damage *= this.GetDamageScalar(mob);
                }

                Effects.SendBoltEffect(m, true, 0);

                if (damage > 0)
                {
                    SpellHelper.Damage(this, m, damage, 0, 0, 0, 0, 100);
                }
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly LightningSpell m_Owner;
            public InternalTarget(LightningSpell owner)
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