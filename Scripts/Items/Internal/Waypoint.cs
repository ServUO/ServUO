using System;
using Server.Commands;
using Server.Targeting;

namespace Server.Items
{
    [FlipableAttribute(0x1f14, 0x1f15, 0x1f16, 0x1f17)]
    public class WayPoint : Item
    {
        private WayPoint m_Next;
        [Constructable]
        public WayPoint()
            : base(0x1f14)
        {
            this.Hue = 0x498;
            this.Visible = false;
            //this.Movable = false;
        }

        public WayPoint(WayPoint prev)
            : this()
        {
            if (prev != null)
                prev.NextPoint = this;
        }

        public WayPoint(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return "AI Way Point";
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public WayPoint NextPoint
        {
            get
            {
                return this.m_Next;
            }
            set
            {
                if (this.m_Next != this)
                    this.m_Next = value;
            }
        }
        public static void Initialize()
        {
            CommandSystem.Register("WayPointSeq", AccessLevel.GameMaster, new CommandEventHandler(WayPointSeq_OnCommand));
        }

        public static void WayPointSeq_OnCommand(CommandEventArgs arg)
        {
            arg.Mobile.SendMessage("Target the position of the first way point.");
            arg.Mobile.Target = new WayPointSeqTarget(null);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
            {
                from.SendMessage("Target the next way point in the sequence.");

                from.Target = new NextPointTarget(this);
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (this.m_Next == null)
                this.LabelTo(from, "(Unlinked)");
            else
                this.LabelTo(from, "(Linked: {0})", this.m_Next.Location);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch( version )
            {
                case 0:
                    {
                        this.m_Next = reader.ReadItem() as WayPoint;
                        break;
                    }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);

            writer.Write(this.m_Next);
        }
    }

    public class NextPointTarget : Target
    {
        private readonly WayPoint m_Point;
        public NextPointTarget(WayPoint pt)
            : base(-1, false, TargetFlags.None)
        {
            this.m_Point = pt;
        }

        protected override void OnTarget(Mobile from, object target)
        {
            if (target is WayPoint && this.m_Point != null)
            {
                this.m_Point.NextPoint = (WayPoint)target;
            }
            else
            {
                from.SendMessage("Target a way point.");
            }
        }
    }

    public class WayPointSeqTarget : Target
    {
        private readonly WayPoint m_Last;
        public WayPointSeqTarget(WayPoint last)
            : base(-1, true, TargetFlags.None)
        {
            this.m_Last = last;
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (targeted is WayPoint)
            {
                if (this.m_Last != null)
                    this.m_Last.NextPoint = (WayPoint)targeted;
            }
            else if (targeted is IPoint3D)
            {
                Point3D p = new Point3D((IPoint3D)targeted);

                WayPoint point = new WayPoint(this.m_Last);
                point.MoveToWorld(p, from.Map);

                from.Target = new WayPointSeqTarget(point);
                from.SendMessage("Target the position of the next way point in the sequence, or target a way point link the newest way point to.");
            }
            else
            {
                from.SendMessage("Target a position, or another way point.");
            }
        }
    }
}