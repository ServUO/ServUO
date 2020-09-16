using Server.Targeting;
using System;

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

        public override SpellCircle Circle => SpellCircle.First;
        public override bool DelayedDamageStacking => false;
        public override bool DelayedDamage => true;
        public override Type[] DelayDamageFamily => new Type[] { typeof(Mysticism.NetherBoltSpell) };
        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void Target(IDamageable d)
        {
            if (!Caster.CanSee(d))
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (CheckHSequence(d))
            {
                IDamageable source = Caster;
                IDamageable target = d;

                SpellHelper.Turn(Caster, d);

                if (HasDelayContext(d))
                {
                    DoHurtFizzle();
                    return;
                }

                if (SpellHelper.CheckReflect(this, ref source, ref target))
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
                    {
                        source.MovingParticles(target, 0x36E4, 5, 0, false, true, 3043, 4043, 0x211);
                        source.PlaySound(0x1E5);
                    });
                }

                double damage = GetNewAosDamage(10, 1, 4, d);

                if (damage > 0)
                {
                    Caster.MovingParticles(d, 0x36E4, 5, 0, false, false, 3006, 0, 0);
                    Caster.PlaySound(0x1E5);

                    SpellHelper.Damage(this, target, damage, 0, 100, 0, 0, 0);
                }
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly MagicArrowSpell m_Owner;
            public InternalTarget(MagicArrowSpell owner)
                : base(10, false, TargetFlags.Harmful)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is IDamageable)
                {
                    m_Owner.Target((IDamageable)o);
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}
