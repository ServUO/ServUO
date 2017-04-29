using System;
using System.Collections.Generic;
using Server.Items;
using Server.Targeting;

namespace Server.Spells.Mysticism
{
	public class NetherCycloneSpell : MysticSpell
	{
        public override SpellCircle Circle { get { return SpellCircle.Eighth; } }

		private static SpellInfo m_Info = new SpellInfo(
				"Nether Cyclone", "Grav Hur",
				230,
				9022,
				Reagent.MandrakeRoot,
				Reagent.Nightshade,
				Reagent.SulfurousAsh,
				Reagent.Bloodmoss
			);

		public NetherCycloneSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new MysticSpellTarget(this, true, TargetFlags.None);
		}

		public override void OnTarget( object o )
		{
            IPoint3D p = o as IPoint3D;

			if (p != null && CheckSequence())
            {
                Map map = Caster.Map;

                if (map != null)
                {
                    List<Mobile> targets = new List<Mobile>();

                    Rectangle2D effectArea = new Rectangle2D(p.X - 3, p.Y - 3, 6, 6);
                    IPooledEnumerable eable = map.GetMobilesInBounds(effectArea);

                    foreach (Mobile m in eable)
                    {
                        if (Caster != m && Caster.InLOS(m) && SpellHelper.ValidIndirectTarget(Caster, m) && Caster.CanBeHarmful(m, false))
                            targets.Add(m);
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
                                Effects.SendLocationEffect(pnt, map, 0x375A, 8, 11, 0x49A, 0);
                            },
                                pn);
                        }
                    }

                    for (int i = 0; i < targets.Count; ++i)
                    {
                        Mobile m = targets[i];

                        m.FixedParticles(0x374A, 1, 15, 9502, 97, 3, (EffectLayer)255);

                        double damage = (((Caster.Skills[CastSkill].Value + (Caster.Skills[DamageSkill].Value / 2)) * .66) + Utility.RandomMinMax(1, 6));

                        SpellHelper.Damage(this, m, damage, 0, 0, 0, 0, 0, 100, 0);

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

                        Effects.SendLocationParticles(EffectItem.Create(m.Location, map, EffectItem.DefaultDuration), 0x37CC, 1, 40, 97, 3, 9917, 0);
                    }
                }
            }

			FinishSequence();
		}
	}
}