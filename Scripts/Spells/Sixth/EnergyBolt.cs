using System;
using Server.Targeting;

namespace Server.Spells.Sixth
{
    public class EnergyBoltSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Energy Bolt", "Corp Por",
            230,
            9022,
            Reagent.BlackPearl,
            Reagent.Nightshade);
        public EnergyBoltSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Sixth;
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
            Mobile mob = m as Mobile;

            if (!this.Caster.CanSee(m))
            {
                this.Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (this.CheckHSequence(m))
            {
                Mobile source = this.Caster;
                SpellHelper.Turn(this.Caster, m);

                if (mob != null)
                {
                    if (SpellHelper.CheckReflect((int)this.Circle, ref source, ref mob))
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
                        {
                            source.MovingParticles(mob, 0x379F, 7, 0, false, true, 3043, 4043, 0x211);
                            source.PlaySound(0x20A);
                        });
                    }
                }

                double damage = 0;

                if (Core.AOS)
                {
                    damage = this.GetNewAosDamage(120, 1, 14, m);
                }
                else if (mob != null)
                {
                    damage = Utility.Random(24, 18);

                    if (this.CheckResisted(mob))
                    {
                        damage *= 0.75;

                        mob.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
                    }

                    // Scale damage based on evalint and resist
                    damage *= this.GetDamageScalar(mob);
                }

                // Do the effects
                this.Caster.MovingParticles(m, 0x379F, 7, 0, false, true, 3043, 4043, 0x211);
                this.Caster.PlaySound(0x20A);

                if (damage > 0)
                {
                    // Deal the damage
                    SpellHelper.Damage(this, mob != null ? mob : m, damage, 0, 0, 0, 0, 100);
                }
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly EnergyBoltSpell m_Owner;
            public InternalTarget(EnergyBoltSpell owner)
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