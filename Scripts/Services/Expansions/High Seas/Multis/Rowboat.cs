using System;
using Server;
using Server.Items;
using Server.Engines.PartySystem;

namespace Server.Multis
{
    public class RowBoat : BaseBoat
    {
        public override int NorthID { get { return 0x3C; } }
        public override int EastID { get { return 0x3D; } }
        public override int SouthID { get { return 0x3E; } }
        public override int WestID { get { return 0x3F; } }

        public override int HoldDistance { get { return -1; } }
        public override int TillerManDistance { get { return -4; } }

        public override Point3D MarkOffset { get { return new Point3D(0, 1, 3); } }

        public override BaseDockedBoat DockedBoat { get { return new DockedRowBoat(this); } }

        public override bool IsClassicBoat { get { return false; } }
        public override bool CanLinkToLighthouse { get { return false; } }

        private Rudder m_Rudder;
        private MooringLine m_Line;

        [CommandProperty(AccessLevel.GameMaster)]
        public Rudder Rudder { get { return m_Rudder; } }

        [Constructable]
        public RowBoat(Direction d) : base(d, false)
        {
            m_Rudder = new Rudder(this, d);
            TillerMan = m_Rudder;
            m_Line = new MooringLine(this);

            switch (d)
            {
                default:
                case Direction.North:
                    m_Rudder.Location = new Point3D(X, Y - TillerManDistance, Z);
                    m_Line.Location = new Point3D(X, Y - 2, Z + 5);
                    break;
                case Direction.South:
                    m_Rudder.Location = new Point3D(X, Y + TillerManDistance, Z);
                    m_Line.Location = new Point3D(X, Y + 2, Z + 5);
                    break;
                case Direction.East:
                    m_Rudder.Location = new Point3D(X + TillerManDistance, Y, Z);
                    m_Line.Location = new Point3D(X + 2, Y, Z + 5);
                    break;
                case Direction.West:
                    m_Rudder.Location = new Point3D(X - TillerManDistance, Y, Z);
                    m_Line.Location = new Point3D(X - 2, Y, Z + 5);
                    break;
            }

            m_Rudder.Handle = new RudderHandle(m_Rudder, d);
        }

        public override TimeSpan GetMovementInterval(bool fast, bool drifting, out int clientSpeed)
        {
            clientSpeed = 0x2;
            return SlowDriftInterval;
        }

        public override void Delete()
        {
            if (m_Line != null)
                m_Line.Delete();

            if (m_Rudder != null && m_Rudder.Handle != null)
                m_Rudder.Handle.Delete();

            base.Delete();
        }

        public override void SetFacingComponents(Direction facing, Direction old, bool ignore)
        {
            if (m_Rudder == null || m_Rudder.Handle == null)
                return;

            if (m_Rudder != null && facing == Direction.North)
                m_Rudder.X--;

            m_Rudder.Handle.SetFacing(facing);
        }

        public override void OnLocationChange(Point3D old)
        {
            base.OnLocationChange(old);

            if (m_Line != null)
                m_Line.Location = new Point3D(X + (m_Line.X - old.X), Y + (m_Line.Y - old.Y), Z + (m_Line.Z - old.Z));

            if(m_Rudder != null && m_Rudder.Handle != null)
                m_Rudder.Handle.Location = new Point3D(X + (m_Rudder.Handle.X - old.X), Y + (m_Rudder.Handle.Y - old.Y), Z + (m_Rudder.Handle.Z - old.Z));
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            if (m_Line != null)
                m_Line.Map = this.Map;
        }

        public override void OnPlacement(Mobile from)
        {
            base.OnPlacement(from);

            if (m_Line == null)
                return;

            switch (Facing)
            {
                default:
                case Direction.North:
                    m_Line.Location = new Point3D(X, Y - 2, Z + 5);
                    break;
                case Direction.South:
                    m_Line.Location = new Point3D(X, Y + 2, Z + 5);
                    break;
                case Direction.East:
                    m_Line.Location = new Point3D(X + 2, Y, Z + 5);
                    break;
                case Direction.West:
                    m_Line.Location = new Point3D(X - 2, Y, Z + 5);
                    break;
            }
        }

        public override bool IsComponentItem(ISpawnable item)
        {
            return item == this || item == m_Line || item == m_Rudder || item is RudderHandle;
        }

        public bool HasAccess(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.Player || this.Owner == null)
                return true;

            if (from.Guild != null && from.Guild == this.Owner.Guild)
                return true;

            Party fromParty = Party.Get(from);
            Party ownerParty = Party.Get(this.Owner);

            if (fromParty != null && ownerParty != null && fromParty == ownerParty)
                return true;

            return from == this.Owner;
        }

        public RowBoat(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_Rudder);
            writer.Write(m_Line);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Rudder = reader.ReadItem() as Rudder;
            m_Line = reader.ReadItem() as MooringLine;

            TillerMan = m_Rudder;
        }
    }

    public class Rudder : TillerMan
    {
        private RudderHandle m_Handle;

        [CommandProperty(AccessLevel.GameMaster)]
        public RudderHandle Handle { get { return m_Handle; } set { m_Handle = value; } }

        public Rudder(BaseBoat boat, Direction d)
            : base(boat)
        {
            SetFacing(d);
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            if (m_Handle != null)
                m_Handle.Map = this.Map;
        }

        public override void Delete()
        {
            if (m_Handle != null)
                m_Handle.Delete();

            base.Delete();
        }

        public override void SetFacing(Direction dir)
        {
            switch (dir)
            {
                case Direction.South: 
                    ItemID = 16068;
                    break;
                case Direction.North: 
                    ItemID = 16062;
                    X--;
                    break;
                case Direction.West: 
                    ItemID =  15990;
                    break;
                case Direction.East: 
                    ItemID =  15971;
                    break;
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            //base.GetProperties(list);
            //list.Add(m_Boat.Status);
        }

        public override void Say(int number)
        {
        }

        public override void Say(int number, string args)
        {
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            return false;
        }

        public Rudder(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_Handle);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Handle = reader.ReadItem() as RudderHandle;

            if (ItemID == 16062)
                X--;
        }
    }

    public class RudderHandle : Static
    {
        private Rudder m_Rudder;

        [CommandProperty(AccessLevel.GameMaster)]
        public Rudder Rudder { get { return m_Rudder; } set { m_Rudder = value; } }

        public RudderHandle(Rudder rudder, Direction d)
        {
            m_Rudder = rudder;
            SetFacing(d);
        }

        public void SetFacing(Direction dir)
        {
            if (m_Rudder == null)
                Delete();
            else
            {
                switch (dir)
                {
                    default:
                    case Direction.South:
                        ItemID = 16067;
                        MoveToWorld(new Point3D(m_Rudder.X, m_Rudder.Y + 1, m_Rudder.Z), this.Map);
                        break;
                    case Direction.North:
                        ItemID = 16063;
                        MoveToWorld(new Point3D(m_Rudder.X + 1, m_Rudder.Y - 1, m_Rudder.Z), this.Map);
                        break;
                    case Direction.West:
                        ItemID = 15991;
                        MoveToWorld(new Point3D(m_Rudder.X -1, m_Rudder.Y + 1, m_Rudder.Z), this.Map);
                        break;
                    case Direction.East:
                        ItemID = 15970;
                        MoveToWorld(new Point3D(m_Rudder.X + 1, m_Rudder.Y, m_Rudder.Z), this.Map);
                        break;
                }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_Rudder != null)
                m_Rudder.OnDoubleClick(from);
        }

        public RudderHandle(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_Rudder);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Rudder = reader.ReadItem() as Rudder;
        }
    }

    public class RowBoatDeed : BaseBoatDeed
    {
        public override int LabelNumber { get { return 1116491; } }
        public override BaseBoat Boat { get { return new RowBoat(this.BoatDirection); } }

        [Constructable]
        public RowBoatDeed()
            : base(0x3C, Point3D.Zero)
        {
        }

        public RowBoatDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }
    }

    public class DockedRowBoat : BaseDockedBoat
    {
        public override BaseBoat Boat { get { return new RowBoat(this.BoatDirection); } }

        public DockedRowBoat(BaseBoat boat) : base(0x3C, Point3D.Zero, boat)
        {
        }

        public DockedRowBoat(Serial serial) : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }
    }
}