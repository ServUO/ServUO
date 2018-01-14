using System;
using Server.Misc;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Third
{
    public class WallOfStoneSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Wall of Stone", "In Sanct Ylem",
            227,
            9011,
            false,
            Reagent.Bloodmoss,
            Reagent.Garlic);
        public WallOfStoneSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Third;
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

                Effects.PlaySound(p, Caster.Map, 0x1F6);

                for (int i = -1; i <= 1; ++i)
                {
                    Point3D loc = new Point3D(eastToWest ? p.X + i : p.X, eastToWest ? p.Y : p.Y + i, p.Z);

                    if (SpellHelper.CheckWater(loc, Caster.Map) && SpellHelper.CheckField(loc, Caster.Map))
                    {
                        Item item = new InternalItem(loc, Caster.Map, Caster);
                        Effects.SendLocationParticles(item, 0x376A, 9, 10, 5025);
                    }
                }
            }

            FinishSequence();
        }

        [DispellableField]
        private class InternalItem : Item
        {
            private readonly Mobile m_Caster;
            private Timer m_Timer;
            private DateTime m_End;
            public InternalItem(Point3D loc, Map map, Mobile caster)
                : base(0x82)
            {
                Movable = false;

                MoveToWorld(loc, map);

                m_Caster = caster;

                if (Deleted)
                    return;

                m_Timer = new InternalTimer(this, TimeSpan.FromSeconds(10.0));
                m_Timer.Start();

                m_End = DateTime.UtcNow + TimeSpan.FromSeconds(10.0);
            }

            public InternalItem(Serial serial)
                : base(serial)
            {
            }

            public override bool BlocksFit
            {
                get
                {
                    return true;
                }
            }
            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write((int)1); // version

                writer.WriteDeltaTime(m_End);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();

                switch ( version )
                {
                    case 1:
                        {
                            m_End = reader.ReadDeltaTime();

                            m_Timer = new InternalTimer(this, m_End - DateTime.UtcNow);
                            m_Timer.Start();

                            break;
                        }
                    case 0:
                        {
                            TimeSpan duration = TimeSpan.FromSeconds(10.0);

                            m_Timer = new InternalTimer(this, duration);
                            m_Timer.Start();

                            m_End = DateTime.UtcNow + duration;

                            break;
                        }
                }
            }

            public override bool OnMoveOver(Mobile m)
            {
                int noto;

                if (m is PlayerMobile)
                {
                    noto = Notoriety.Compute(m_Caster, m);
                    if (noto == Notoriety.Enemy || noto == Notoriety.Ally)
                        return false;

                    if (m.Map != null && (m.Map.Rules & MapRules.FreeMovement) == 0)
                        return false;
                }
                return base.OnMoveOver(m);
            }

            public override void OnAfterDelete()
            {
                base.OnAfterDelete();

                if (m_Timer != null)
                    m_Timer.Stop();
            }

            private class InternalTimer : Timer
            {
                private readonly InternalItem m_Item;
                public InternalTimer(InternalItem item, TimeSpan duration)
                    : base(duration)
                {
                    Priority = TimerPriority.OneSecond;
                    m_Item = item;
                }

                protected override void OnTick()
                {
                    m_Item.Delete();
                }
            }
        }

        public class InternalTarget : Target
        {
            private readonly WallOfStoneSpell m_Owner;
            public InternalTarget(WallOfStoneSpell owner)
                : base(Core.TOL ? 15 : Core.ML ? 10 : 12, true, TargetFlags.None)
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