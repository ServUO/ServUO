using Server.Targeting;
using System;

namespace Server.Spells.Mysticism
{
    public class EagleStrikeSpell : MysticSpell
    {
        public override SpellCircle Circle => SpellCircle.Third;
        public override bool DelayedDamage => true;
        public override bool DelayedDamageStacking => false;

        private static readonly SpellInfo m_Info = new SpellInfo(
                "Eagle Strike", "Kal Por Xen",
                230,
                9022,
                Reagent.Bloodmoss,
                Reagent.Bone,
                Reagent.MandrakeRoot,
                Reagent.SpidersSilk
            );

        public EagleStrikeSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void OnTarget(IDamageable d)
        {
            if (d == null)
            {
                return;
            }
            else if (CheckHSequence(d))
            {
                IDamageable target = d;
                IDamageable source = Caster;

                SpellHelper.Turn(Caster, target);

                if (HasDelayContext(target))
                {
                    DoHurtFizzle();
                    return;
                }

                if (SpellHelper.CheckReflect(this, ref source, ref target))
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
                    {
                        source.MovingEffect(target, 0x407A, 8, 1, false, true, 0, 0);
                        source.PlaySound(0x2EE);
                    });
                }

                Caster.MovingEffect(d, 0x407A, 8, 1, false, true, 0, 0);
                Caster.PlaySound(0x2EE);

                Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
                {
                    Caster.PlaySound(0x64D);
                });

                SpellHelper.Damage(this, target, GetNewAosDamage(19, 1, 5, target), 0, 0, 0, 0, 100);
            }

            FinishSequence();
        }

        public class InternalTarget : Target
        {
            public EagleStrikeSpell Owner { get; set; }

            public InternalTarget(EagleStrikeSpell owner)
                : this(owner, false)
            {
            }

            public InternalTarget(EagleStrikeSpell owner, bool allowland)
                : base(12, allowland, TargetFlags.Harmful)
            {
                Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o == null)
                    return;

                if (!from.CanSee(o))
                    from.SendLocalizedMessage(500237); // Target can not be seen.
                else if (o is IDamageable)
                {
                    SpellHelper.Turn(from, o);
                    Owner.OnTarget((IDamageable)o);
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                Owner.FinishSequence();
            }
        }
    }
}
