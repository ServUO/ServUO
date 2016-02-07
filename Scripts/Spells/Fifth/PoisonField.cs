using System;
using System.Collections;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Targeting;

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

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Fifth;
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

                Effects.PlaySound(p, this.Caster.Map, 0x20B);

                int itemID = eastToWest ? 0x3915 : 0x3922;

                TimeSpan duration = TimeSpan.FromSeconds(3 + (this.Caster.Skills.Magery.Fixed / 25));

                for (int i = -2; i <= 2; ++i)
                {
                    Point3D loc = new Point3D(eastToWest ? p.X + i : p.X, eastToWest ? p.Y : p.Y + i, p.Z);

                    new InternalItem(itemID, loc, this.Caster, this.Caster.Map, duration, i);
                }
            }

            this.FinishSequence();
        }

        [DispellableField]
        public class InternalItem : Item
        {
            private Timer m_Timer;
            private DateTime m_End;
            private Mobile m_Caster;
            public InternalItem(int itemID, Point3D loc, Mobile caster, Map map, TimeSpan duration, int val)
                : base(itemID)
            {
                bool canFit = SpellHelper.AdjustField(ref loc, map, 12, false);

                this.Visible = false;
                this.Movable = false;
                this.Light = LightType.Circle300;

                this.MoveToWorld(loc, map);

                this.m_Caster = caster;

                this.m_End = DateTime.UtcNow + duration;

                this.m_Timer = new InternalTimer(this, TimeSpan.FromSeconds(Math.Abs(val) * 0.2), caster.InLOS(this), canFit);
                this.m_Timer.Start();
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

                writer.Write((int)1); // version

                writer.Write(this.m_Caster);
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
            }

            public void ApplyPoisonTo(Mobile m)
            {
                if (this.m_Caster == null)
                    return;

                Poison p;

                if (Core.AOS)
                {
                    int total = (this.m_Caster.Skills.Magery.Fixed + this.m_Caster.Skills.Poisoning.Fixed) / 2;

                    if (total >= 1000)
                        p = Poison.Deadly;
                    else if (total > 850)
                        p = Poison.Greater;
                    else if (total > 650)
                        p = Poison.Regular;
                    else
                        p = Poison.Lesser;
                }
                else
                {
                    p = Poison.Regular;
                }

                if (m.ApplyPoison(this.m_Caster, p) == ApplyPoisonResult.Poisoned)
                    if (SpellHelper.CanRevealCaster(m))
                        this.m_Caster.RevealingAction();

                if (m is BaseCreature)
                    ((BaseCreature)m).OnHarmfulSpell(this.m_Caster);
            }

            public override bool OnMoveOver(Mobile m)
            {
                if (this.Visible && this.m_Caster != null && (!Core.AOS || m != this.m_Caster) && SpellHelper.ValidIndirectTarget(this.m_Caster, m) && this.m_Caster.CanBeHarmful(m, false))
                {
                    this.m_Caster.DoHarmful(m);

                    this.ApplyPoisonTo(m);
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
                public InternalTimer(InternalItem item, TimeSpan delay, bool inLOS, bool canFit)
                    : base(delay, TimeSpan.FromSeconds(1.5))
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
                            Effects.SendLocationParticles(EffectItem.Create(this.m_Item.Location, this.m_Item.Map, EffectItem.DefaultDuration), 0x376A, 9, 10, 5040);
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
                            bool eastToWest = (this.m_Item.ItemID == 0x3915);
                            IPooledEnumerable eable = map.GetMobilesInBounds(new Rectangle2D(this.m_Item.X - (eastToWest ? 0 : 1), this.m_Item.Y - (eastToWest ? 1 : 0), (eastToWest ? 1 : 2), (eastToWest ? 2 : 1)));

                            foreach (Mobile m in eable)
                            {
                                if ((m.Z + 16) > this.m_Item.Z && (this.m_Item.Z + 12) > m.Z && (!Core.AOS || m != caster) && SpellHelper.ValidIndirectTarget(caster, m) && caster.CanBeHarmful(m, false))
                                    m_Queue.Enqueue(m);
                            }

                            eable.Free();

                            while (m_Queue.Count > 0)
                            {
                                Mobile m = (Mobile)m_Queue.Dequeue();

                                caster.DoHarmful(m);

                                this.m_Item.ApplyPoisonTo(m);
                                m.PlaySound(0x474);
                            }
                        }
                    }
                }
            }
        }

        private class InternalTarget : Target
        {
            private readonly PoisonFieldSpell m_Owner;
            public InternalTarget(PoisonFieldSpell owner)
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