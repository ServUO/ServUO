using System;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Mysticism
{
    public class BombardSpell : MysticSpell
    {
        public override SpellCircle Circle { get { return SpellCircle.Sixth; } }
        public override bool DelayedDamage { get { return true; } }
        public override bool DelayedDamageStacking { get { return false; } }

        private static SpellInfo m_Info = new SpellInfo(
                "Bombard", "Corp Por Ylem",
                230,
                9022,
                Reagent.Bloodmoss,
                Reagent.Garlic,
                Reagent.SulfurousAsh,
                Reagent.DragonBlood
            );

        public BombardSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this, TargetFlags.Harmful);
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

                if (Core.SA && HasDelayContext(target))
                {
                    DoHurtFizzle();
                    return;
                }

                if (SpellHelper.CheckReflect((int)Circle, ref source, ref target))
                {
                    Server.Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
                    {
                        source.MovingEffect(target, 0x1363, 12, 1, false, true, 0, 0);
                        source.PlaySound(0x64B);
                    });
                }

                Caster.MovingEffect(d, 0x1363, 12, 1, false, true, 0, 0);
                Caster.PlaySound(0x64B);

                SpellHelper.Damage(this, target, (int)GetNewAosDamage(40, 1, 5, target), 100, 0, 0, 0, 0);

                if (target is Mobile)
                {
                    Timer.DelayCall(TimeSpan.FromMilliseconds(1200), () =>
                    {
                        if (!CheckResisted((Mobile)target))
                        {
                            ((Mobile)target).Paralyze(TimeSpan.FromSeconds(3));
                        }
                    });
                }
            }

            FinishSequence();
        }

        public class InternalTarget : Target
        {
            public BombardSpell Owner { get; set; }

            public InternalTarget(BombardSpell owner, TargetFlags flags)
                : this(owner, false, flags)
            {
            }

            public InternalTarget(BombardSpell owner, bool allowland, TargetFlags flags)
                : base(12, allowland, flags)
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
