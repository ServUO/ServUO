using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Targeting;

namespace Server.Spells.Mysticism
{
    public class EagleStrikeSpell : MysticSpell
    {
        public override SpellCircle Circle { get { return SpellCircle.Third; } }
        public override bool DelayedDamage { get { return true; } }
        public override bool DelayedDamageStacking { get { return false; } }

        private static SpellInfo m_Info = new SpellInfo(
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
            Caster.Target = new InternalTarget(this, TargetFlags.Harmful);
        }

        public void OnTarget(IDamageable target)
        {
            if (target == null)
            {
                return;
            }
            else if (CheckHSequence(target))
            {
                SpellHelper.Turn(Caster, target);

                if (Core.SA && HasDelayContext(target))
                {
                    DoHurtFizzle();
                    return;
                }

                SpellHelper.CheckReflect((int)Circle, Caster, ref target);

                Caster.MovingEffect(target, 0x407A, 8, 1, false, true, 0, 0);
                Caster.PlaySound(0x2EE);

                Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
                {
                    Caster.PlaySound(0x64D);
                });

                SpellHelper.Damage(this, target, (int)GetNewAosDamage(19, 1, 5, target), 0, 0, 0, 0, 100);
            }

            FinishSequence();
        }

        public class InternalTarget : Target
        {
            public EagleStrikeSpell Owner { get; set; }

            public InternalTarget(EagleStrikeSpell owner, TargetFlags flags)
                : this(owner, false, flags)
            {
            }

            public InternalTarget(EagleStrikeSpell owner, bool allowland, TargetFlags flags)
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
