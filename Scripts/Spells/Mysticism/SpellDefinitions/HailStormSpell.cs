using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Targeting;

namespace Server.Spells.Mystic
{
    public class HailStormSpell : MysticSpell
    {
        public override SpellCircle Circle { get { return SpellCircle.Seventh; } }

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

                IPooledEnumerable eable = Caster.Map.GetMobilesInRange(point, 2);
                Rectangle2D effectArea = new Rectangle2D(p.X - 3, p.Y - 3, 6, 6);

                List<Mobile> toEffect = new List<Mobile>();

                foreach (Mobile m in eable)
                {
                    if (Caster != m && SpellHelper.ValidIndirectTarget(Caster, m) && Caster.CanBeHarmful(m, false))
                        toEffect.Add(m);
                }
                eable.Free();

                new HailstormTimer(Caster, this, toEffect, effectArea);
            }

            FinishSequence();
        }

        public class InternalTarget : Target
        {
            private HailStormSpell m_Owner;

            public InternalTarget(HailStormSpell owner) : base(10, true, TargetFlags.None)
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

        private class HailstormTimer : Timer
        {
            private List<Mobile> m_ToEffect;
            private Rectangle2D m_EffectArea;
            private Mobile m_Caster;
            private Map m_Map;
            private int m_Ticks;
            private Spell m_Spell;

            public HailstormTimer(Mobile caster, Spell spell, List<Mobile> toEffect, Rectangle2D area)
                : base(TimeSpan.FromMilliseconds(100.0), TimeSpan.FromMilliseconds(100.0))
            {
                m_ToEffect = toEffect;
                m_EffectArea = area;
                m_Caster = caster;
                m_Map = caster.Map;
                m_Spell = spell;
                Start();
            }

            protected override void OnTick()
            {
                m_Ticks++;

                if (m_Spell == null)
                {
                    Stop();
                    return;
                }

                if (m_Ticks >= 20)
                {
                    int damage = 0;

                    foreach (Mobile m in m_ToEffect)
                    {
                        if (m.Deleted || !m.Alive)
                            continue;

                        damage = m_Spell.GetNewAosDamage(51, 1, 5, m is PlayerMobile, m);

                        if (m_ToEffect.Count > 2)
                            damage = (damage * 2) / m_ToEffect.Count;

                        m_Caster.DoHarmful(m);
                        SpellHelper.Damage(m_Spell, m, damage, 0, 0, 100, 0, 0);

                        m.FixedParticles(0x374A, 1, 15, 9502, 97, 3, (EffectLayer)255);
                    }

                    Stop();
                }

                int x = m_EffectArea.X + Utility.Random(m_EffectArea.Width);
                int y = m_EffectArea.Y + Utility.Random(m_EffectArea.Height);
                int z = m_Map.GetAverageZ(x, y);

                int fromX = x + Utility.RandomMinMax(-8, 8);
                int fromY = y - 10;
                int fromZ = z + 30;

                Point3D start = new Point3D(fromX, fromY, fromZ);
                Point3D finish = new Point3D(x, y, z);

                Effects.SendMovingParticles(
                    new Entity(Serial.Zero, start, m_Map),
                    new Entity(Serial.Zero, finish, m_Map),
                    0x36D4, 15, 0, false, false, 1365, 0, 9502, 1, 0, (EffectLayer)255, 0x100);

                Effects.SendLocationEffect(finish, m_Map, 0x3728, 10, 20, 1365, 0);
                Effects.PlaySound(finish, m_Map, 0x64F);
            }
        }
    }
}