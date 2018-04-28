using System;
using System.Collections.Generic;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Spells.Seventh
{
    public class MeteorSwarmSpell : MagerySpell
    {
        public override DamageType SpellDamageType { get { return DamageType.SpellAOE; } }

        private static readonly SpellInfo m_Info = new SpellInfo(
            "Meteor Swarm", "Flam Kal Des Ylem",
            233,
            9042,
            false,
            Reagent.Bloodmoss,
            Reagent.MandrakeRoot,
            Reagent.SulfurousAsh,
            Reagent.SpidersSilk);
        public MeteorSwarmSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Seventh;
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

                List<IDamageable> targets = new List<IDamageable>();

                Map map = Caster.Map;

                if (map != null)
                {
                    IPooledEnumerable eable = map.GetObjectsInRange(new Point3D(p), 2);

                    foreach (object o in eable)
                    {
                        IDamageable id = o as IDamageable;

                        if (id == null || (Core.AOS && id is Mobile && (Mobile)id == Caster))
                            continue;

                        if ((!(id is Mobile) || SpellHelper.ValidIndirectTarget(Caster, id as Mobile)) && Caster.CanBeHarmful(id, false))
                        {
                            if (Core.AOS && !Caster.InLOS(id))
                                continue;

                            targets.Add(id);
                        }
                    }

                    eable.Free();
                }

                double damage;

                if (targets.Count > 0)
                {
                    Effects.PlaySound(p, Caster.Map, 0x160);

                    for (int i = 0; i < targets.Count; ++i)
                    {
                        IDamageable id = targets[i];
                        Mobile m = id as Mobile;

                        if (Core.AOS)
                            damage = GetNewAosDamage(51, 1, 5, id is PlayerMobile, id);
                        else
                            damage = Utility.Random(27, 22);

                        if (Core.AOS && targets.Count > 2)
                            damage = (damage * 2) / targets.Count;
                        else if (!Core.AOS)
                            damage /= targets.Count;

                        if (!Core.AOS && m != null && CheckResisted(m))
                        {
                            damage *= 0.5;

                            m.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
                        }

                        IDamageable source = Caster;
                        IDamageable target = id;

                        if (SpellHelper.CheckReflect((int)Circle, ref source, ref target, SpellDamageType))
                        {
                            Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
                                {
                                    source.MovingParticles(target, 0x36D4, 7, 0, false, true, 9501, 1, 0, 0x100);
                                });
                        }

                        if (m != null)
                        {
                            damage *= GetDamageScalar(m);
                        }

                        Caster.DoHarmful(id);
                        SpellHelper.Damage(this, target, damage, 0, 100, 0, 0, 0);

                        Caster.MovingParticles(id, 0x36D4, 7, 0, false, true, 9501, 1, 0, 0x100);
                    }
                }

                ColUtility.Free(targets);
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly MeteorSwarmSpell m_Owner;
            public InternalTarget(MeteorSwarmSpell owner)
                : base(Core.ML ? 10 : 12, true, TargetFlags.None)
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