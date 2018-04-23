using System;
using System.Collections.Generic;
using Server.Items;
using Server.Targeting;

namespace Server.Spells.Mysticism
{
    public class NetherCycloneSpell : MysticSpell
    {
        public override SpellCircle Circle { get { return SpellCircle.Eighth; } }
        public override DamageType SpellDamageType { get { return DamageType.SpellAOE; } }

        private static SpellInfo m_Info = new SpellInfo(
                "Nether Cyclone", "Grav Hur",
                230,
                9022,
                Reagent.MandrakeRoot,
                Reagent.Nightshade,
                Reagent.SulfurousAsh,
                Reagent.Bloodmoss
            );

        public NetherCycloneSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this, true, TargetFlags.None);
        }

        public void OnTarget(IPoint3D p)
        {
            if (p != null && CheckSequence())
            {
                SpellHelper.Turn(Caster, p);
                SpellHelper.GetSurfaceTop(ref p);
                Map map = Caster.Map;

                if (map != null)
                {
                    List<IDamageable> targets = new List<IDamageable>();

                    Rectangle2D effectArea = new Rectangle2D(p.X - 3, p.Y - 3, 6, 6);
                    IPooledEnumerable eable = map.GetObjectsInBounds(effectArea);

                    foreach (object o in eable)
                    {
                        IDamageable id = o as IDamageable;

                        if (id == null || (id is Mobile && (Mobile)id == Caster))
                            continue;

                        if ((!(id is Mobile) || SpellHelper.ValidIndirectTarget(Caster, id as Mobile)) && Caster.CanBeHarmful(id, false))
                        {
                            if (Core.AOS && !Caster.InLOS(id))
                                continue;

                            targets.Add(id);
                        }
                    }
                    eable.Free();

                    Effects.PlaySound(p, map, 0x64F);

                    for (int x = effectArea.X; x <= effectArea.X + effectArea.Width; x++)
                    {
                        for (int y = effectArea.Y; y <= effectArea.Y + effectArea.Height; y++)
                        {
                            if (x == effectArea.X && y == effectArea.Y ||
                                x >= effectArea.X + effectArea.Width - 1 && y >= effectArea.Y + effectArea.Height - 1 ||
                                y >= effectArea.Y + effectArea.Height - 1 && x == effectArea.X ||
                                y == effectArea.Y && x >= effectArea.X + effectArea.Width - 1)
                                continue;

                            IPoint3D pnt = new Point3D(x, y, p.Z);
                            SpellHelper.GetSurfaceTop(ref pnt);

                            Timer.DelayCall<Point3D>(TimeSpan.FromMilliseconds(Utility.RandomMinMax(100, 300)), point =>
                            {
                                Effects.SendLocationEffect(point, map, 0x375A, 8, 11, 0x49A, 0);
                            },
                            new Point3D(pnt));
                        }
                    }

                    for (int i = 0; i < targets.Count; ++i)
                    {
                        IDamageable d = targets[i];

                        Server.Effects.SendTargetParticles(d, 0x374A, 1, 15, 9502, 97, 3, (EffectLayer)255, 0);

                        double damage = (((Caster.Skills[CastSkill].Value + (Caster.Skills[DamageSkill].Value / 2)) * .66) + Utility.RandomMinMax(1, 6));

                        SpellHelper.Damage(this, d, damage, 0, 0, 0, 0, 0, 100, 0);

                        if (d is Mobile)
                        {
                            Mobile m = d as Mobile;

                            double stamSap = (damage / 3);
                            double manaSap = (damage / 3);
                            double mod = m.Skills[SkillName.MagicResist].Value - ((Caster.Skills[CastSkill].Value + Caster.Skills[DamageSkill].Value) / 2);

                            if (mod > 0)
                            {
                                mod /= 100;

                                stamSap *= mod;
                                manaSap *= mod;
                            }

                            m.Stam -= (int)stamSap;
                            m.Mana -= (int)manaSap;

                            Timer.DelayCall(TimeSpan.FromSeconds(10), () =>
                            {
                                if (m.Alive)
                                {
                                    m.Stam += (int)stamSap;
                                    m.Mana += (int)manaSap;
                                }
                            });
                        }

                        Effects.SendLocationParticles(EffectItem.Create(d.Location, map, EffectItem.DefaultDuration), 0x37CC, 1, 40, 97, 3, 9917, 0);
                    }

                    ColUtility.Free(targets);
                }
            }

            FinishSequence();
        }

        public class InternalTarget : Target
        {
            public NetherCycloneSpell Owner { get; set; }

            public InternalTarget(NetherCycloneSpell owner, TargetFlags flags)
                : this(owner, false, flags)
            {
            }

            public InternalTarget(NetherCycloneSpell owner, bool allowland, TargetFlags flags)
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
                else if(o is IPoint3D)
                {
                    SpellHelper.Turn(from, o);
                    Owner.OnTarget((IPoint3D)o);
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                Owner.FinishSequence();
            }
        }
    }
}
