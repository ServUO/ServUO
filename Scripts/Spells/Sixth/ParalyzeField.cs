using System;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Sixth
{
    public class ParalyzeFieldSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Paralyze Field", "In Ex Grav",
            230,
            9012,
            false,
            Reagent.BlackPearl,
            Reagent.Ginseng,
            Reagent.SpidersSilk);
        public ParalyzeFieldSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Sixth;
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
                    eastToWest = false;
                else if (rx >= 0)
                    eastToWest = true;
                else if (ry >= 0)
                    eastToWest = true;
                else
                    eastToWest = false;

                Effects.PlaySound(p, this.Caster.Map, 0x20B);

                int itemID = eastToWest ? 0x3967 : 0x3979;

                TimeSpan duration = TimeSpan.FromSeconds(3.0 + (this.Caster.Skills[SkillName.Magery].Value / 3.0));

                for (int i = -2; i <= 2; ++i)
                {
                    Point3D loc = new Point3D(eastToWest ? p.X + i : p.X, eastToWest ? p.Y : p.Y + i, p.Z);
                    bool canFit = SpellHelper.AdjustField(ref loc, this.Caster.Map, 12, false);

                    if (!canFit)
                        continue;

                    Item item = new InternalItem(this.Caster, itemID, loc, this.Caster.Map, duration);
                    item.ProcessDelta();

                    Effects.SendLocationParticles(EffectItem.Create(loc, this.Caster.Map, EffectItem.DefaultDuration), 0x376A, 9, 10, 5048);
                }
            }

            this.FinishSequence();
        }

        [DispellableField]
        public class InternalItem : Item
        {
            private Timer m_Timer;
            private Mobile m_Caster;
            private DateTime m_End;

            public Mobile Caster { get { return m_Caster; } }

            public InternalItem(Mobile caster, int itemID, Point3D loc, Map map, TimeSpan duration)
                : base(itemID)
            {
                this.Visible = false;
                this.Movable = false;
                this.Light = LightType.Circle300;

                this.MoveToWorld(loc, map);

                if (caster.InLOS(this))
                    this.Visible = true;
                else
                    this.Delete();

                if (this.Deleted)
                    return;

                this.m_Caster = caster;

                this.m_Timer = new InternalTimer(this, duration);
                this.m_Timer.Start();

                this.m_End = DateTime.UtcNow + duration;
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
            public override void OnAfterDelete()
            {
                base.OnAfterDelete();

                if (this.m_Timer != null)
                    this.m_Timer.Stop();
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write((int)0); // version

                writer.Write(this.m_Caster);
                writer.WriteDeltaTime(this.m_End);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();

                switch ( version )
                {
                    case 0:
                        {
                            this.m_Caster = reader.ReadMobile();
                            this.m_End = reader.ReadDeltaTime();

                            this.m_Timer = new InternalTimer(this, this.m_End - DateTime.UtcNow);
                            this.m_Timer.Start();

                            break;
                        }
                }
            }

            public override bool OnMoveOver(Mobile m)
            {
                if (this.Visible && this.m_Caster != null && (!Core.AOS || m != this.m_Caster) && SpellHelper.ValidIndirectTarget(this.m_Caster, m) && this.m_Caster.CanBeHarmful(m, false))
                {
                    if (SpellHelper.CanRevealCaster(m))
                        this.m_Caster.RevealingAction();

                    this.m_Caster.DoHarmful(m);

                    double duration;

                    if (Core.AOS)
                    {
                        duration = 2.0 + ((int)(this.m_Caster.Skills[SkillName.EvalInt].Value / 10) - (int)(m.Skills[SkillName.MagicResist].Value / 10));

                        if (!m.Player)
                            duration *= 3.0;

                        if (duration < 0.0)
                            duration = 0.0;
                    }
                    else
                    {
                        duration = 7.0 + (this.m_Caster.Skills[SkillName.Magery].Value * 0.2);
                    }

                    m.Paralyze(TimeSpan.FromSeconds(duration));

                    m.PlaySound(0x204);
                    m.FixedEffect(0x376A, 10, 16);
					
                    if (m is BaseCreature)
                        ((BaseCreature)m).OnHarmfulSpell(this.m_Caster);
                }

                return true;
            }

            private class InternalTimer : Timer
            {
                private readonly Item m_Item;
                public InternalTimer(Item item, TimeSpan duration)
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

        public class InternalTarget : Target
        {
            private readonly ParalyzeFieldSpell m_Owner;
            public InternalTarget(ParalyzeFieldSpell owner)
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