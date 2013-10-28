using System;

namespace Server.Engines.XmlSpawner2
{
    public class XmlDrag : XmlAttachment
    {
        private Mobile m_DraggedBy = null;// mobile doing the dragging
        private Point3D m_currentloc = Point3D.Zero;
        private Map m_currentmap = null;
        private int m_Distance = -3;
        private InternalTimer m_Timer;
        // These are the various ways in which the message attachment can be constructed.  
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments

        // a serial constructor is REQUIRED
        public XmlDrag(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlDrag()
        {
        }

        [Attachable]
        public XmlDrag(Mobile draggedby)
        {
            this.DraggedBy = draggedby;
        }

        [Attachable]
        public XmlDrag(string name, Mobile draggedby)
        {
            this.Name = name;
            this.DraggedBy = draggedby;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile DraggedBy
        {
            get
            {
                return this.m_DraggedBy;
            }
            set
            {
                this.m_DraggedBy = value;
                if (this.m_DraggedBy != null)
                {
                    this.DoTimer();
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Distance
        {
            get
            {
                return this.m_Distance;
            }
            set
            {
                this.m_Distance = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D CurrentLoc
        {
            get
            {
                return this.m_currentloc;
            }
            set
            {
                this.m_currentloc = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Map CurrentMap
        {
            get
            {
                return this.m_currentmap;
            }
            set
            {
                this.m_currentmap = value;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            // version 0
            writer.Write(this.m_DraggedBy);
            writer.Write(this.m_Distance);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            // version 0
            this.m_DraggedBy = reader.ReadMobile();
            this.m_Distance = reader.ReadInt();
        }

        public override void OnAttach()
        {
            base.OnAttach();

            if (this.AttachedTo is Mobile || this.AttachedTo is Item)
            {
                this.DoTimer();
            }
            else
                this.Delete();
        }

        public override void OnReattach()
        {
            base.OnReattach();

            this.DoTimer();
        }

        public override void OnDelete()
        {
            base.OnDelete();

            if (this.m_Timer != null)
                this.m_Timer.Stop();
        }

        public void DoTimer()
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = new InternalTimer(this);
            this.m_Timer.Start();
        }

        public override string OnIdentify(Mobile from)
        {
            if (from == null || from.IsPlayer())
                return null;

            if (this.Expiration > TimeSpan.Zero)
            {
                return String.Format("{2}: Dragged by {0} expires in {1} mins", this.DraggedBy, this.Expiration.TotalMinutes, this.Name);
            }
            else
            {
                return String.Format("{1}: Dragged by {0}", this.DraggedBy, this.Name);
            }
        }

        // added the duration timer that begins on spawning
        private class InternalTimer : Timer
        {
            private readonly XmlDrag m_attachment;
            public InternalTimer(XmlDrag attachment)
                : base(TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(500))
            {
                this.Priority = TimerPriority.FiftyMS;
                this.m_attachment = attachment;
            }

            protected override void OnTick()
            {
                if (this.m_attachment == null)
                    return;

                Mobile draggedby = this.m_attachment.DraggedBy;
                object parent = this.m_attachment.AttachedTo;

                if (parent == null || !(parent is Mobile || parent is Item) || draggedby == null || draggedby.Deleted || draggedby == parent)
                {
                    this.Stop();
                    return;
                }

                // get the location of the mobile dragging

                Point3D newloc = draggedby.Location;
                Map newmap = draggedby.Map;

                if (newmap == null || newmap == Map.Internal)
                {
                    // if the mobile dragging has an invalid map, then disconnect
                    this.m_attachment.DraggedBy = null;
                    this.Stop();
                    return;
                }

                // update the location of the dragged object if the parent has moved
                if (newloc != this.m_attachment.CurrentLoc || newmap != this.m_attachment.CurrentMap)
                {
                    this.m_attachment.CurrentLoc = newloc;
                    this.m_attachment.CurrentMap = newmap;

                    int x = newloc.X;
                    int y = newloc.Y;
                    int lag = this.m_attachment.Distance;
                    // compute the new location for the dragged object
                    switch (draggedby.Direction & Direction.Mask)
                    {
                        case Direction.North:
                            y -= lag;
                            break;
                        case Direction.Right:
                            x += lag;
                            y -= lag;
                            break;
                        case Direction.East:
                            x += lag;
                            break;
                        case Direction.Down:
                            x += lag;
                            y += lag;
                            break;
                        case Direction.South:
                            y += lag;
                            break;
                        case Direction.Left:
                            x -= lag;
                            y += lag;
                            break;
                        case Direction.West:
                            x -= lag;
                            break;
                        case Direction.Up:
                            x -= lag;
                            y -= lag;
                            break;
                    }

                    if (parent is Mobile)
                    {
                        ((Mobile)parent).Location = new Point3D(x, y, newloc.Z);
                        ((Mobile)parent).Map = newmap;
                    }
                    else if (parent is Item)
                    {
                        ((Item)parent).Location = new Point3D(x, y, newloc.Z);
                        ((Item)parent).Map = newmap;
                    }
                }
            }
        }
    }
}