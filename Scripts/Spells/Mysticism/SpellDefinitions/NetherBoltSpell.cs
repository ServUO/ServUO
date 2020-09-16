using Server.Targeting;
using System;

namespace Server.Spells.Mysticism
{
    public class NetherBoltSpell : MysticSpell
    {
        public override SpellCircle Circle => SpellCircle.First;

        private static readonly SpellInfo m_Info = new SpellInfo(
                "Nether Bolt", "In Corp Ylem",
                230,
                9022,
                Reagent.BlackPearl,
                Reagent.SulfurousAsh
            );

        public NetherBoltSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override bool DelayedDamage => true;
        public override bool DelayedDamageStacking => false;
        public override Type[] DelayDamageFamily => new Type[] { typeof(First.MagicArrowSpell) };

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
                        source.MovingParticles(target, 0x36D4, 7, 0, false, true, 0x49A, 0, 0, 9502, 4019, 0x160);
                        source.PlaySound(0x211);
                    });
                }

                double damage = GetNewAosDamage(10, 1, 4, target);

                SpellHelper.Damage(this, target, damage, 0, 0, 0, 0, 0, 100, 0);

                Caster.MovingParticles(d, 0x36D4, 7, 0, false, true, 0x49A, 0, 0, 9502, 4019, 0x160);
                Caster.PlaySound(0x211);
            }

            FinishSequence();
        }

        public class InternalTarget : Target
        {
            public NetherBoltSpell Owner { get; set; }

            public InternalTarget(NetherBoltSpell owner)
                : this(owner, false)
            {
            }

            public InternalTarget(NetherBoltSpell owner, bool allowland)
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
