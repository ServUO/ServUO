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

                SpellHelper.GetSurfaceTop(ref p);

                int dx = this.Caster.Location.X - p.X;
                int dy = this.Caster.Location.Y - p.Y;
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

                Effects.PlaySound(p, this.Caster.Map, 0x1F6);

                for (int i = -1; i <= 1; ++i)
                {
                    Point3D loc = new Point3D(eastToWest ? p.X + i : p.X, eastToWest ? p.Y : p.Y + i, p.Z);
                    bool canFit = SpellHelper.AdjustField(ref loc, this.Caster.Map, 22, true);

                    //Effects.SendLocationParticles( EffectItem.Create( loc, Caster.Map, EffectItem.DefaultDuration ), 0x376A, 9, 10, 5025 );

                    if (!canFit)
                        continue;

                    Item item = new InternalItem(loc, this.Caster.Map, this.Caster);

                    Effects.SendLocationParticles(item, 0x376A, 9, 10, 5025);
                    //new InternalItem( loc, Caster.Map, Caster );
                }
            }

            this.FinishSequence();
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
                this.Visible = false;
                this.Movable = false;

                this.MoveToWorld(loc, map);

                this.m_Caster = caster;

                if (caster.InLOS(this))
                    this.Visible = true;
                else
                    this.Delete();

                if (this.Deleted)
                    return;

                this.m_Timer = new InternalTimer(this, TimeSpan.FromSeconds(10.0));
                this.m_Timer.Start();

                this.m_End = DateTime.UtcNow + TimeSpan.FromSeconds(10.0);
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

                writer.WriteDeltaTime(this.m_End);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();

                switch ( version )
                {
                    case 1:
                        {
                            this.m_End = reader.ReadDeltaTime();

                            this.m_Timer = new InternalTimer(this, this.m_End - DateTime.UtcNow);
                            this.m_Timer.Start();

                            break;
                        }
                    case 0:
                        {
                            TimeSpan duration = TimeSpan.FromSeconds(10.0);

                            this.m_Timer = new InternalTimer(this, duration);
                            this.m_Timer.Start();

                            this.m_End = DateTime.UtcNow + duration;

                            break;
                        }
                }
            }

            public override bool OnMoveOver(Mobile m)
            {
                int noto;

                if (m is PlayerMobile)
                {
                    noto = Notoriety.Compute(this.m_Caster, m);
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

                if (this.m_Timer != null)
                    this.m_Timer.Stop();
            }

            private class InternalTimer : Timer
            {
                private readonly InternalItem m_Item;
                public InternalTimer(InternalItem item, TimeSpan duration)
                    : base(duration)
                {
                    this.Priority = TimerPriority.OneSecond;
                    this.m_Item = item;
                }

                protected override void OnTick()
                {
                    this.m_Item.Delete();
                }
            }
        }

        private class InternalTarget : Target
        {
            private readonly WallOfStoneSpell m_Owner;
            public InternalTarget(WallOfStoneSpell owner)
                : base(Core.ML ? 10 : 12, true, TargetFlags.None)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is IPoint3D)
                    this.m_Owner.Target((IPoint3D)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}