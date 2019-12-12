using System;
using System.Linq;
using System.Collections.Generic;

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
        public override bool IsRowBoat { get { return true; } }
        public override bool CanLinkToLighthouse { get { return false; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public MooringLine Line { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Rudder Rudder { get; private set; }

        [Constructable]
        public RowBoat(Direction d)
            : base(d, false)
        {
            Rudder = new Rudder(this, d);
            TillerMan = Rudder;
            Line = new MooringLine(this);

            switch (d)
            {
                default:
                case Direction.North:
                    Rudder.Location = new Point3D(X, Y - TillerManDistance, Z);
                    Line.Location = new Point3D(X, Y - 2, Z + 5);
                    break;
                case Direction.South:
                    Rudder.Location = new Point3D(X, Y + TillerManDistance, Z);
                    Line.Location = new Point3D(X, Y + 2, Z + 5);
                    break;
                case Direction.East:
                    Rudder.Location = new Point3D(X + TillerManDistance, Y, Z);
                    Line.Location = new Point3D(X + 2, Y, Z + 5);
                    break;
                case Direction.West:
                    Rudder.Location = new Point3D(X - TillerManDistance, Y, Z);
                    Line.Location = new Point3D(X - 2, Y, Z + 5);
                    break;
            }

            Rudder.Handle = new RudderHandle(Rudder, d);
        }

        public override void Delete()
        {
            if (Line != null)
                Line.Delete();

            if (Rudder != null && Rudder.Handle != null)
                Rudder.Handle.Delete();

            base.Delete();
        }

        public override void SetFacingComponents(Direction facing, Direction old, bool ignore)
        {
            if (Rudder == null || Rudder.Handle == null)
                return;

            if (Rudder != null && facing == Direction.North)
                Rudder.X--;

            Rudder.Handle.SetFacing(facing);
        }

        public override void OnLocationChange(Point3D old)
        {
            base.OnLocationChange(old);

            if (Line != null)
                Line.Location = new Point3D(X + (Line.X - old.X), Y + (Line.Y - old.Y), Z + (Line.Z - old.Z));

            if(Rudder != null && Rudder.Handle != null)
                Rudder.Handle.Location = new Point3D(X + (Rudder.Handle.X - old.X), Y + (Rudder.Handle.Y - old.Y), Z + (Rudder.Handle.Z - old.Z));
        }

        /// <summary>
        /// This must be overriden due to the tillerman not being in the MCL bounds
        /// </summary>
        /// <returns></returns>
        public override IEnumerable<IEntity> GetEntitiesOnBoard()
        {
            Map map = Map;

            if (map == null || map == Map.Internal)
                yield break;

            MultiComponentList mcl = Components;
            IPooledEnumerable eable = map.GetObjectsInBounds(new Rectangle2D(X + mcl.Min.X, Y + mcl.Min.Y, mcl.Width, mcl.Height));

            foreach (IEntity ent in eable)
            {
                if (Contains(ent) && CheckOnBoard(ent))
                {
                    yield return ent;
                }
            }

            eable.Free();
            yield return Rudder;
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            if (Line != null)
                Line.Map = Map;
        }

        public override void OnPlacement(Mobile from)
        {
            base.OnPlacement(from);

            if (Line == null)
                return;

            switch (Facing)
            {
                default:
                case Direction.North:
                    Line.Location = new Point3D(X, Y - 2, Z + 5);
                    break;
                case Direction.South:
                    Line.Location = new Point3D(X, Y + 2, Z + 5);
                    break;
                case Direction.East:
                    Line.Location = new Point3D(X + 2, Y, Z + 5);
                    break;
                case Direction.West:
                    Line.Location = new Point3D(X - 2, Y, Z + 5);
                    break;
            }
        }

        public override bool IsComponentItem(IEntity item)
        {
            return item == this || item == Line || item == Rudder || (Rudder != null && item == Rudder.Handle);
        }

        public override bool HasAccess(Mobile from)
        {
            return true;
        }

        public RowBoat(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(Rudder);
            writer.Write(Line);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Rudder = reader.ReadItem() as Rudder;
            Line = reader.ReadItem() as MooringLine;

            TillerMan = Rudder;
        }
    }

    public class Rudder : TillerMan
    {
        public override int LabelNumber { get { return 1149698; } } // wheel

        public override bool ForceShowProperties { get { return true; } }

        public override bool Babbles { get { return false; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public RudderHandle Handle { get; set; }

        public Rudder(BaseBoat boat, Direction d)
            : base(boat)
        {
            SetFacing(d);
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            if (Handle != null)
                Handle.Map = Map;
        }

        public override void Delete()
        {
            if (Handle != null)
                Handle.Delete();

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

        public override void Say(int number)
        {
        }

        public override void Say(int number, string args)
        {
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            return false;
        }

        public Rudder(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(Handle);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Handle = reader.ReadItem() as RudderHandle;

            if (ItemID == 16062)
                X--;
        }
    }

    public class RudderHandle : Static
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public Rudder Rudder { get; set; }

        public RudderHandle(Rudder rudder, Direction d)
        {
            Rudder = rudder;
            SetFacing(d);
        }

        public virtual void SetFacing(Direction dir)
        {
            if (Rudder == null)
                Delete();
            else
            {
                switch (dir)
                {
                    default:
                    case Direction.South:
                        ItemID = 16067;
                        MoveToWorld(new Point3D(Rudder.X, Rudder.Y + 1, Rudder.Z), Map);
                        break;
                    case Direction.North:
                        ItemID = 16063;
                        MoveToWorld(new Point3D(Rudder.X + 1, Rudder.Y - 1, Rudder.Z), Map);
                        break;
                    case Direction.West:
                        ItemID = 15991;
                        MoveToWorld(new Point3D(Rudder.X -1, Rudder.Y + 1, Rudder.Z), Map);
                        break;
                    case Direction.East:
                        ItemID = 15970;
                        MoveToWorld(new Point3D(Rudder.X + 1, Rudder.Y, Rudder.Z), Map);
                        break;
                }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Rudder != null)
                Rudder.OnDoubleClick(from);
        }

        public RudderHandle(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(Rudder);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            Rudder = reader.ReadItem() as Rudder;
        }
    }

    public class RowBoatDeed : BaseBoatDeed
    {
        public override int LabelNumber { get { return 1116491; } }
        public override BaseBoat Boat { get { return new RowBoat(BoatDirection); } }

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
        public override BaseBoat Boat { get { return new RowBoat(BoatDirection); } }

        public DockedRowBoat(BaseBoat boat)
            : base(0x3C, Point3D.Zero, boat)
        {
        }

        public DockedRowBoat(Serial serial)
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
}
