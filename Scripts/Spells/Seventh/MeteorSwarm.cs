using System;
using System.Collections.Generic;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Spells.Seventh
{
    public class MeteorSwarmSpell : MagerySpell
    {
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
            this.Caster.Target = new InternalTarget(this);
        }

        public void Target(IPoint3D p)
        {
            if (!this.Caster.CanSee(p))
            {
                this.Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (SpellHelper.CheckTown(p, this.Caster) && this.CheckSequence())
            {
                SpellHelper.Turn(this.Caster, p);

                if (p is Item)
                    p = ((Item)p).GetWorldLocation();

                List<IDamageable> targets = new List<IDamageable>();

                Map map = this.Caster.Map;

                if (map != null)
                {
                    IPooledEnumerable eable = map.GetObjectsInRange(new Point3D(p), 2);

                    foreach (object o in eable)
                    {
                        IDamageable id = o as IDamageable;

                        if (id == null || (Core.AOS && id is Mobile && (Mobile)id == this.Caster))
                            continue;

                        if ((!(id is Mobile) || SpellHelper.ValidIndirectTarget(this.Caster, id as Mobile)) && this.Caster.CanBeHarmful(id, false))
                        {
                            if (Core.AOS && !this.Caster.InLOS(id))
                                continue;

                            targets.Add(id);
                        }
                    }

                    eable.Free();
                }

                double damage;

                if (targets.Count > 0)
                {
                    Effects.PlaySound(p, this.Caster.Map, 0x160);

                    for (int i = 0; i < targets.Count; ++i)
                    {
                        IDamageable id = targets[i];
                        Mobile m = id as Mobile;

                        if (Core.AOS)
                            damage = this.GetNewAosDamage(51, 1, 5, id is PlayerMobile, id);
                        else
                            damage = Utility.Random(27, 22);

                        if (Core.AOS && targets.Count > 2)
                            damage = (damage * 2) / targets.Count;
                        else if (!Core.AOS)
                            damage /= targets.Count;

                        if (!Core.AOS && m != null && this.CheckResisted(m))
                        {
                            damage *= 0.5;

                            m.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
                        }

                        if(m != null)
                            damage *= this.GetDamageScalar(m);

                        this.Caster.DoHarmful(id);
                        SpellHelper.Damage(this, id, damage, 0, 100, 0, 0, 0);

                        this.Caster.MovingParticles(id, 0x36D4, 7, 0, false, true, 9501, 1, 0, 0x100);
                    }
                }

                targets.Clear();
                targets.TrimExcess();
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly MeteorSwarmSpell m_Owner;
            public InternalTarget(MeteorSwarmSpell owner)
                : base(Core.ML ? 10 : 12, true, TargetFlags.None)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                IPoint3D p = o as IPoint3D;

                if (p != null)
                    this.m_Owner.Target(p);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}