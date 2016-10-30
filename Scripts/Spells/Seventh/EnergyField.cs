using System;
using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Seventh
{
    public class EnergyFieldSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Energy Field", "In Sanct Grav",
            221,
            9022,
            false,
            Reagent.BlackPearl,
            Reagent.MandrakeRoot,
            Reagent.SpidersSilk,
            Reagent.SulfurousAsh);
        public EnergyFieldSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Seventh;
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

                TimeSpan duration;

                if (Core.AOS)
                    duration = TimeSpan.FromSeconds((15 + (this.Caster.Skills.Magery.Fixed / 5)) / 7);
                else
                    duration = TimeSpan.FromSeconds(this.Caster.Skills[SkillName.Magery].Value * 0.28 + 2.0); // (28% of magery) + 2.0 seconds

                int itemID = eastToWest ? 0x3946 : 0x3956;

                for (int i = -2; i <= 2; ++i)
                {
                    Point3D loc = new Point3D(eastToWest ? p.X + i : p.X, eastToWest ? p.Y : p.Y + i, p.Z);
                    bool canFit = SpellHelper.AdjustField(ref loc, this.Caster.Map, 12, false);

                    if (!canFit)
                        continue;

                    Item item = new InternalItem(loc, this.Caster.Map, duration, itemID, this.Caster);
                    item.ProcessDelta();

                    Effects.SendLocationParticles(EffectItem.Create(loc, this.Caster.Map, EffectItem.DefaultDuration), 0x376A, 9, 10, 5051);
                }
            }

            this.FinishSequence();
        }

        [DispellableField]
        private class InternalItem : Item
        {
            private readonly Timer m_Timer;
            private readonly Mobile m_Caster;
            public InternalItem(Point3D loc, Map map, TimeSpan duration, int itemID, Mobile caster)
                : base(itemID)
            {
                this.Visible = false;
                this.Movable = false;
                this.Light = LightType.Circle300;

                this.MoveToWorld(loc, map);

                this.m_Caster = caster;

                if (caster.InLOS(this))
                    this.Visible = true;
                else
                    this.Delete();

                if (this.Deleted)
                    return;

                this.m_Timer = new InternalTimer(this, duration);
                this.m_Timer.Start();
            }

            public InternalItem(Serial serial)
                : base(serial)
            {
                this.m_Timer = new InternalTimer(this, TimeSpan.FromSeconds(5.0));
                this.m_Timer.Start();
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

                writer.Write((int)0); // version
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();
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

        public class InternalTarget : Target
        {
            private readonly EnergyFieldSpell m_Owner;
            public InternalTarget(EnergyFieldSpell owner)
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