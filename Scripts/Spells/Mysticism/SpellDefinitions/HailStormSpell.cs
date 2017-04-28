using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Targeting;

namespace Server.Spells.Mysticism
{
    public class HailStormSpell : MysticSpell
    {
        public override SpellCircle Circle { get { return SpellCircle.Seventh; } }
        public override bool DelayedDamage { get { return true; } }

        private static SpellInfo m_Info = new SpellInfo(
                "Hail Storm", "Kal Des Ylem",
                230,
                9022,
                Reagent.BlackPearl,
                Reagent.Bloodmoss,
                Reagent.MandrakeRoot,
                Reagent.DragonBlood
            );

        public HailStormSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void OnTarget(IPoint3D p)
        {
            if (SpellHelper.CheckTown(p, Caster) && CheckSequence())
            {
                Point3D point = new Point3D(p);
                Map map = Caster.Map;

                if (map == null)
                    return;

                IPooledEnumerable eable = map.GetMobilesInRange(point, 2);
                Rectangle2D effectArea = new Rectangle2D(p.X - 3, p.Y - 3, 6, 6);

                List<Mobile> toEffect = new List<Mobile>();

                foreach (Mobile m in eable)
                {
                    if (Caster != m && SpellHelper.ValidIndirectTarget(Caster, m) && Caster.CanBeHarmful(m, false))
                        toEffect.Add(m);
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

                        Point3D pn = new Point3D(x, y, map.GetAverageZ(x, y));
                        Timer.DelayCall<Point3D>(TimeSpan.FromMilliseconds(Utility.RandomMinMax(100, 300)), pnt =>
                            {
                                Effects.SendLocationEffect(pnt, map, 0x3779, 12, 11, 0x63, 0);
                            },
                            pn);
                    }
                }

                foreach (Mobile m in toEffect)
                {
                    if (m.Deleted || !m.Alive)
                        continue;

                    int damage = GetNewAosDamage(51, 1, 5, m is PlayerMobile, m);

                    if (toEffect.Count > 2)
                        damage = (damage * 2) / toEffect.Count;

                    Caster.DoHarmful(m);
                    SpellHelper.Damage(this, m, damage, 0, 0, 100, 0, 0);

                    m.FixedParticles(0x374A, 1, 15, 9502, 97, 3, (EffectLayer)255);
                }
            }

            FinishSequence();
        }

        public class InternalTarget : Target
        {
            private HailStormSpell m_Owner;

            public InternalTarget(HailStormSpell owner)
                : base(10, true, TargetFlags.None)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is IPoint3D)
                    m_Owner.OnTarget((IPoint3D)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}