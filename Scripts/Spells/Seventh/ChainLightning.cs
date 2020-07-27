using Server.Mobiles;
using Server.Targeting;
using System;
using System.Linq;

namespace Server.Spells.Seventh
{
    public class ChainLightningSpell : MagerySpell
    {
        public override DamageType SpellDamageType => DamageType.SpellAOE;

        private static readonly SpellInfo m_Info = new SpellInfo(
            "Chain Lightning", "Vas Ort Grav",
            209,
            9022,
            false,
            Reagent.BlackPearl,
            Reagent.Bloodmoss,
            Reagent.MandrakeRoot,
            Reagent.SulfurousAsh);
        public ChainLightningSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle => SpellCircle.Seventh;
        public override bool DelayedDamage => true;
        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void Target(IPoint3D p)
        {
            if (!Caster.CanSee(p))
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (SpellHelper.CheckTown(p, Caster) && CheckSequence())
            {
                SpellHelper.Turn(Caster, p);

                if (p is Item)
                    p = ((Item)p).GetWorldLocation();

                System.Collections.Generic.List<IDamageable> targets = AcquireIndirectTargets(p, 2).ToList();
                int count = Math.Max(1, targets.Count);

                foreach (IDamageable dam in targets)
                {
                    IDamageable id = dam;
                    Mobile m = id as Mobile;
                    double damage = GetNewAosDamage(51, 1, 5, id is PlayerMobile, id);

                    if (count > 2)
                        damage = (damage * 2) / count;

                    Mobile source = Caster;
                    SpellHelper.CheckReflect(this, ref source, ref id);

                    if (m != null)
                    {
                        damage *= GetDamageScalar(m);
                    }

                    Effects.SendBoltEffect(id, true, 0, false);

                    Caster.DoHarmful(id);
                    SpellHelper.Damage(this, id, damage, 0, 0, 0, 0, 100);
                }

                ColUtility.Free(targets);
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly ChainLightningSpell m_Owner;
            public InternalTarget(ChainLightningSpell owner)
                : base(10, true, TargetFlags.None)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                IPoint3D p = o as IPoint3D;

                if (p != null)
                    m_Owner.Target(p);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}
