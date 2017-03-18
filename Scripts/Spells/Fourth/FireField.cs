using System;
using System.Collections;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Fourth
{
    public class FireFieldSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Fire Field", "In Flam Grav",
            215,
            9041,
            false,
            Reagent.BlackPearl,
            Reagent.SpidersSilk,
            Reagent.SulfurousAsh);
        public FireFieldSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Fourth;
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

                Effects.PlaySound(p, this.Caster.Map, 0x20C);

                int itemID = eastToWest ? 0x398C : 0x3996;

                TimeSpan duration;

                if (Core.AOS)
                    duration = TimeSpan.FromSeconds((15 + (this.Caster.Skills.Magery.Fixed / 5)) / 4);
                else
                    duration = TimeSpan.FromSeconds(4.0 + (this.Caster.Skills[SkillName.Magery].Value * 0.5));

                for (int i = -2; i <= 2; ++i)
                {
                    Point3D loc = new Point3D(eastToWest ? p.X + i : p.X, eastToWest ? p.Y : p.Y + i, p.Z);

                    new FireFieldItem(itemID, loc, this.Caster, this.Caster.Map, duration, i);
                }
            }

            this.FinishSequence();
        }

        [DispellableField]
        public class FireFieldItem : Item
        {
            private Timer m_Timer;
            private DateTime m_End;
            private Mobile m_Caster;
            private int m_Damage;

            public Mobile Caster { get { return m_Caster; } }

            public FireFieldItem(int itemID, Point3D loc, Mobile caster, Map map, TimeSpan duration, int val)
                : this(itemID, loc, caster, map, duration, val, 2)
            {
            }

            public FireFieldItem(int itemID, Point3D loc, Mobile caster, Map map, TimeSpan duration, int val, int damage)
                : base(itemID)
            {
                bool canFit = SpellHelper.AdjustField(ref loc, map, 12, false);

                this.Visible = false;
                this.Movable = false;
                this.Light = LightType.Circle300;

                this.MoveToWorld(loc, map);

                this.m_Caster = caster;

                this.m_Damage = damage;

                this.m_End = DateTime.UtcNow + duration;

                this.m_Timer = new InternalTimer(this, TimeSpan.FromSeconds(Math.Abs(val) * 0.2), caster.InLOS(this), canFit);
                this.m_Timer.Start();
            }

            public FireFieldItem(Serial serial)
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

                writer.Write((int)2); // version

                writer.Write(this.m_Damage);
                writer.Write(this.m_Caster);
                writer.WriteDeltaTime(this.m_End);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();

                switch ( version )
                {
                    case 2:
                        {
                            this.m_Damage = reader.ReadInt();
                            goto case 1;
                        }
                    case 1:
                        {
                            this.m_Caster = reader.ReadMobile();

                            goto case 0;
                        }
                    case 0:
                        {
                            this.m_End = reader.ReadDeltaTime();

                            this.m_Timer = new InternalTimer(this, TimeSpan.Zero, true, true);
                            this.m_Timer.Start();

                            break;
                        }
                }

                if (version < 2)
                    this.m_Damage = 2;
            }

            public override bool OnMoveOver(Mobile m)
            {
                if (this.Visible && this.m_Caster != null && (!Core.AOS || m != this.m_Caster) && SpellHelper.ValidIndirectTarget(this.m_Caster, m) && this.m_Caster.CanBeHarmful(m, false))
                {
                    if (SpellHelper.CanRevealCaster(m))
                        this.m_Caster.RevealingAction();
					
                    this.m_Caster.DoHarmful(m);

                    int damage = this.m_Damage;

                    if (!Core.AOS && m.CheckSkill(SkillName.MagicResist, 0.0, 30.0))
                    {
                        damage = 1;

                        m.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
                    }

                    AOS.Damage(m, this.m_Caster, damage, 0, 100, 0, 0, 0);
                    m.PlaySound(0x208);

                    if (m is BaseCreature)
                        ((BaseCreature)m).OnHarmfulSpell(this.m_Caster);
                }

                return true;
            }

            private class InternalTimer : Timer
            {
                private static readonly Queue m_Queue = new Queue();
                private readonly FireFieldItem m_Item;
                private readonly bool m_InLOS;
                private readonly bool m_CanFit;
                public InternalTimer(FireFieldItem item, TimeSpan delay, bool inLOS, bool canFit)
                    : base(delay, TimeSpan.FromSeconds(1.0))
                {
                    this.m_Item = item;
                    this.m_InLOS = inLOS;
                    this.m_CanFit = canFit;

                    this.Priority = TimerPriority.FiftyMS;
                }

                protected override void OnTick()
                {
                    if (this.m_Item.Deleted)
                        return;

                    if (!this.m_Item.Visible)
                    {
                        if (this.m_InLOS && this.m_CanFit)
                            this.m_Item.Visible = true;
                        else
                            this.m_Item.Delete();

                        if (!this.m_Item.Deleted)
                        {
                            this.m_Item.ProcessDelta();
                            Effects.SendLocationParticles(EffectItem.Create(this.m_Item.Location, this.m_Item.Map, EffectItem.DefaultDuration), 0x376A, 9, 10, 5029);
                        }
                    }
                    else if (DateTime.UtcNow > this.m_Item.m_End)
                    {
                        this.m_Item.Delete();
                        this.Stop();
                    }
                    else
                    {
                        Map map = this.m_Item.Map;
                        Mobile caster = this.m_Item.m_Caster;

                        if (map != null && caster != null)
                        {
                            foreach (Mobile m in this.m_Item.GetMobilesInRange(0))
                            {
                                if ((m.Z + 16) > this.m_Item.Z && (this.m_Item.Z + 12) > m.Z && (!Core.AOS || m != caster) && SpellHelper.ValidIndirectTarget(caster, m) && caster.CanBeHarmful(m, false))
                                    m_Queue.Enqueue(m);
                            }

                            while (m_Queue.Count > 0)
                            {
                                Mobile m = (Mobile)m_Queue.Dequeue();
								
                                if (SpellHelper.CanRevealCaster(m))
                                    caster.RevealingAction();

                                caster.DoHarmful(m);

                                int damage = this.m_Item.m_Damage;

                                if (!Core.AOS && m.CheckSkill(SkillName.MagicResist, 0.0, 30.0))
                                {
                                    damage = 1;

                                    m.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
                                }

                                AOS.Damage(m, caster, damage, 0, 100, 0, 0, 0);
                                m.PlaySound(0x208);

                                if (m is BaseCreature)
                                    ((BaseCreature)m).OnHarmfulSpell(caster);
                            }
                        }
                    }
                }
            }
        }

        public class InternalTarget : Target
        {
            private readonly FireFieldSpell m_Owner;
            public InternalTarget(FireFieldSpell owner)
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