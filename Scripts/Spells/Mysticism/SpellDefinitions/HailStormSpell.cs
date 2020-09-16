using Server.Mobiles;
using Server.Targeting;
using System;
using System.Linq;

namespace Server.Spells.Mysticism
{
    public class HailStormSpell : MysticSpell
    {
        public override SpellCircle Circle => SpellCircle.Seventh;
        public override bool DelayedDamage => false;
        public override DamageType SpellDamageType => DamageType.SpellAOE;

        private static readonly SpellInfo m_Info = new SpellInfo(
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
                SpellHelper.Turn(Caster, p);
                SpellHelper.GetSurfaceTop(ref p);

                Map map = Caster.Map;

                if (map == null)
                    return;

                Rectangle2D effectArea = new Rectangle2D(p.X - 3, p.Y - 3, 6, 6);
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

                        Timer.DelayCall(TimeSpan.FromMilliseconds(Utility.RandomMinMax(100, 300)), point =>
                            {
                                Effects.SendLocationEffect(point, map, 0x3779, 12, 11, 0x63, 0);
                            },
                            new Point3D(pnt));
                    }
                }

                System.Collections.Generic.List<IDamageable> list = AcquireIndirectTargets(p, 2).ToList();
                int count = list.Count;

                foreach (IDamageable id in list)
                {
                    if (id.Deleted)
                        continue;

                    int damage = GetNewAosDamage(51, 1, 5, id is PlayerMobile, id);

                    if (count > 2)
                        damage = (damage * 2) / count;

                    Caster.DoHarmful(id);
                    SpellHelper.Damage(this, id, damage, 0, 0, 100, 0, 0);

                    Effects.SendTargetParticles(id, 0x374A, 1, 15, 9502, 97, 3, (EffectLayer)255, 0);
                }

                ColUtility.Free(list);
            }

            FinishSequence();
        }

        public class InternalTarget : Target
        {
            private readonly HailStormSpell m_Owner;

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