using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Targeting;
using System;
using System.Collections;

namespace Server.Spells.Fifth
{
    public class PoisonFieldSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Poison Field", "In Nox Grav",
            230,
            9052,
            false,
            Reagent.BlackPearl,
            Reagent.Nightshade,
            Reagent.SpidersSilk);
        public PoisonFieldSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle => SpellCircle.Fifth;
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
            else if (SpellHelper.CheckTown(p, Caster) && SpellHelper.CheckWater(new Point3D(p), Caster.Map) && CheckSequence())
            {
                SpellHelper.Turn(Caster, p);

                SpellHelper.GetSurfaceTop(ref p);

                int dx = Caster.Location.X - p.X;
                int dy = Caster.Location.Y - p.Y;
                int rx = (dx - dy) * 44;
                int ry = (dx + dy) * 44;

                bool eastToWest;

                if (rx >= 0 && ry >= 0)
                {
                    eastToWest = false;
                }
                else if (rx >= 0)
                {
                    eastToWest = true;
                }
                else if (ry >= 0)
                {
                    eastToWest = true;
                }
                else
                {
                    eastToWest = false;
                }

                Effects.PlaySound(p, Caster.Map, 0x20B);
                int itemID = eastToWest ? 0x3915 : 0x3922;

                Point3D pnt = new Point3D(p);
                TimeSpan duration = TimeSpan.FromSeconds(3 + (Caster.Skills.Magery.Fixed / 25));

                if (SpellHelper.CheckField(pnt, Caster.Map))
                    new InternalItem(itemID, pnt, Caster, Caster.Map, duration);

                for (int i = 1; i <= 2; ++i)
                {
                    Timer.DelayCall(TimeSpan.FromMilliseconds(i * 300), index =>
                    {
                        Point3D point = new Point3D(eastToWest ? pnt.X + index : pnt.X, eastToWest ? pnt.Y : pnt.Y + index, pnt.Z);
                        SpellHelper.AdjustField(ref point, Caster.Map, 16, false);

                        if (SpellHelper.CheckField(point, Caster.Map))
                            new InternalItem(itemID, point, Caster, Caster.Map, duration);

                        point = new Point3D(eastToWest ? pnt.X + -index : pnt.X, eastToWest ? pnt.Y : pnt.Y + -index, pnt.Z);
                        SpellHelper.AdjustField(ref point, Caster.Map, 16, false);

                        if (SpellHelper.CheckField(point, Caster.Map))
                            new InternalItem(itemID, point, Caster, Caster.Map, duration);
                    }, i);
                }
            }

            FinishSequence();
        }

        [DispellableField]
        public class InternalItem : Item
        {
            private Timer m_Timer;
            private DateTime m_End;
            private Mobile m_Caster;

            public Mobile Caster => m_Caster;

            public InternalItem(int itemID, Point3D loc, Mobile caster, Map map, TimeSpan duration)
                : base(itemID)
            {
                bool canFit = SpellHelper.AdjustField(ref loc, map, 12, false);

                Movable = false;
                Light = LightType.Circle300;

                MoveToWorld(loc, map);
                Effects.SendLocationParticles(EffectItem.Create(loc, map, EffectItem.DefaultDuration), 0x376A, 9, 10, 5029);

                m_Caster = caster;

                m_End = DateTime.UtcNow + duration;

                m_Timer = new InternalTimer(this, caster.InLOS(this), canFit);
                m_Timer.Start();
            }

            public InternalItem(Serial serial)
                : base(serial)
            {
            }

            public override bool BlocksFit => true;
            public override void OnAfterDelete()
            {
                base.OnAfterDelete();

                if (m_Timer != null)
                    m_Timer.Stop();
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write(1); // version

                writer.Write(m_Caster);
                writer.WriteDeltaTime(m_End);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();

                switch (version)
                {
                    case 1:
                        {
                            m_Caster = reader.ReadMobile();

                            goto case 0;
                        }
                    case 0:
                        {
                            m_End = reader.ReadDeltaTime();

                            m_Timer = new InternalTimer(this, true, true);
                            m_Timer.Start();

                            break;
                        }
                }
            }

            public void ApplyPoisonTo(Mobile m)
            {
                if (m_Caster == null)
                    return;

                Poison p;

                int total = (m_Caster.Skills.Magery.Fixed + m_Caster.Skills.Poisoning.Fixed) / 2;

                if (total >= 1000)
                    p = Poison.Deadly;
                else if (total > 850)
                    p = Poison.Greater;
                else if (total > 650)
                    p = Poison.Regular;
                else
                    p = Poison.Lesser;

                if (m.ApplyPoison(m_Caster, p) == ApplyPoisonResult.Poisoned)
                    if (SpellHelper.CanRevealCaster(m))
                        m_Caster.RevealingAction();

                if (m is BaseCreature)
                    ((BaseCreature)m).OnHarmfulSpell(m_Caster);
            }

            public override bool OnMoveOver(Mobile m)
            {
                if (Visible && m_Caster != null && m != m_Caster && SpellHelper.ValidIndirectTarget(m_Caster, m) && m_Caster.CanBeHarmful(m, false))
                {
                    m_Caster.DoHarmful(m);

                    ApplyPoisonTo(m);
                    m.PlaySound(0x474);
                }

                return true;
            }

            private class InternalTimer : Timer
            {
                private static readonly Queue m_Queue = new Queue();
                private readonly InternalItem m_Item;
                private readonly bool m_InLOS;
                private readonly bool m_CanFit;
                public InternalTimer(InternalItem item, bool inLOS, bool canFit)
                    : base(TimeSpan.FromMilliseconds(500), TimeSpan.FromSeconds(1.5))
                {
                    m_Item = item;
                    m_InLOS = inLOS;
                    m_CanFit = canFit;

                    Priority = TimerPriority.FiftyMS;
                }

                protected override void OnTick()
                {
                    if (m_Item.Deleted)
                        return;

                    if (DateTime.UtcNow > m_Item.m_End)
                    {
                        m_Item.Delete();
                        Stop();
                    }
                    else
                    {
                        Map map = m_Item.Map;
                        Mobile caster = m_Item.m_Caster;

                        if (map != null && caster != null)
                        {
                            bool eastToWest = (m_Item.ItemID == 0x3915);
                            IPooledEnumerable eable = map.GetMobilesInBounds(new Rectangle2D(m_Item.X - (eastToWest ? 0 : 1), m_Item.Y - (eastToWest ? 1 : 0), (eastToWest ? 1 : 2), (eastToWest ? 2 : 1)));

                            foreach (Mobile m in eable)
                            {
                                if ((m.Z + 16) > m_Item.Z && (m_Item.Z + 12) > m.Z && m != caster && SpellHelper.ValidIndirectTarget(caster, m) && caster.CanBeHarmful(m, false))
                                    m_Queue.Enqueue(m);
                            }

                            eable.Free();

                            while (m_Queue.Count > 0)
                            {
                                Mobile m = (Mobile)m_Queue.Dequeue();

                                caster.DoHarmful(m);

                                m_Item.ApplyPoisonTo(m);
                                m.PlaySound(0x474);
                            }
                        }
                    }
                }
            }
        }

        public class InternalTarget : Target
        {
            private readonly PoisonFieldSpell m_Owner;
            public InternalTarget(PoisonFieldSpell owner)
                : base(15, true, TargetFlags.None)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is IPoint3D)
                    m_Owner.Target((IPoint3D)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}
