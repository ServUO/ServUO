using Server.Items;
using System.Collections.Generic;

namespace Server.Multis
{
    public class PumpkinRowBoat : BaseBoat
    {
        public override int NorthID => 0x50;
        public override int EastID => 0x51;
        public override int SouthID => 0x52;
        public override int WestID => 0x53;

        public override int HoldDistance => -1;
        public override int TillerManDistance => -2;

        public override Point3D MarkOffset => new Point3D(0, 1, 3);

        public override BaseDockedBoat DockedBoat => new DockedPumpkinRowBoat(this);

        public override bool IsClassicBoat => false;
        public override bool IsRowBoat => true;
        public override bool CanLinkToLighthouse => false;

        [CommandProperty(AccessLevel.GameMaster)]
        public MooringBlock Line { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Rudder Rudder { get; private set; }

        [Constructable]
        public PumpkinRowBoat(Direction d)
            : base(d, false)
        {
            Rudder = new PumpkinRudder(this, d);
            TillerMan = Rudder;
            Line = new MooringBlock(this, d);

            switch (d)
            {
                default:
                case Direction.North:
                    Rudder.Location = new Point3D(X + 1, Y + 3, Z);
                    Line.Location = new Point3D(X, Y - 1, Z + 2);
                    break;
                case Direction.South:
                    Rudder.Location = new Point3D(X, Y - 2, Z);
                    Line.Location = new Point3D(X, Y + 1, Z + 2);
                    break;
                case Direction.East:
                    Rudder.Location = new Point3D(X - 2, Y, Z);
                    Line.Location = new Point3D(X + 1, Y, Z + 2);
                    break;
                case Direction.West:
                    Rudder.Location = new Point3D(X + 3, Y + 1, Z);
                    Line.Location = new Point3D(X - 1, Y, Z + 2);
                    break;
            }

            Rudder.Handle = new PumpkinRudderHandle(Rudder, d);
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

            switch (facing)
            {
                case Direction.North:
                    {
                        Rudder.Y++;
                        break;
                    }
                case Direction.West:
                    {
                        Rudder.X++;
                        Rudder.Y++;
                        break;
                    }
            }

            Line.SetFacing(facing);
            Rudder.Handle.SetFacing(facing);
        }

        public override void OnLocationChange(Point3D old)
        {
            base.OnLocationChange(old);

            if (Line != null)
                Line.Location = new Point3D(X + (Line.X - old.X), Y + (Line.Y - old.Y), Z + (Line.Z - old.Z));

            if (Rudder != null && Rudder.Handle != null)
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

        public PumpkinRowBoat(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Rudder);
            writer.Write(Line);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Rudder = reader.ReadItem() as PumpkinRudder;
            Line = reader.ReadItem() as MooringBlock;

            TillerMan = Rudder;
        }
    }

    public class PumpkinRowBoatDeed : BaseBoatDeed
    {
        public override int LabelNumber => 1159233;  // Pumpkin Rowboat
        public override BaseBoat Boat => new PumpkinRowBoat(BoatDirection);

        [Constructable]
        public PumpkinRowBoatDeed()
            : base(0x50, Point3D.Zero)
        {
        }

        public PumpkinRowBoatDeed(Serial serial)
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
            writer.Write(0);
        }
    }

    public class MooringBlock : MooringLine
    {
        public MooringBlock(BaseBoat boat, Direction d)
            : base(boat)
        {
            SetFacing(d);
        }

        public void SetFacing(Direction dir)
        {
            switch (dir)
            {
                case Direction.North:
                case Direction.South:
                    ItemID = 42088;
                    break;
                case Direction.East:
                case Direction.West:
                    ItemID = 42087;
                    break;
            }
        }

        public MooringBlock(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class PumpkinRudder : Rudder
    {
        public PumpkinRudder(BaseBoat boat, Direction d)
            : base(boat, d)
        {
        }

        public override void SetFacing(Direction dir)
        {
            switch (dir)
            {
                case Direction.South:
                    ItemID = 42073;
                    break;
                case Direction.North:
                    ItemID = 42030;
                    Y++;
                    break;
                case Direction.West:
                    ItemID = 42044;
                    X++;
                    Y++;
                    break;
                case Direction.East:
                    ItemID = 42058;
                    break;
            }
        }

        public PumpkinRudder(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (ItemID == 42030)
            {
                Y++;
            }

            if (ItemID == 42044)
            {
                X++;
                Y++;
            }
        }
    }

    public class PumpkinRudderHandle : RudderHandle
    {
        public PumpkinRudderHandle(Rudder rudder, Direction d)
            : base(rudder, d)
        {
        }

        public override void SetFacing(Direction dir)
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
                        MoveToWorld(new Point3D(Rudder.X + 1, Rudder.Y + 1, Rudder.Z + 9), Map);
                        break;
                    case Direction.North:
                        ItemID = 16061;
                        MoveToWorld(new Point3D(Rudder.X, Rudder.Y - 1, Rudder.Z + 2), Map);
                        break;
                    case Direction.West:
                        ItemID = 15991;
                        MoveToWorld(new Point3D(Rudder.X - 1, Rudder.Y, Rudder.Z + 2), Map);
                        break;
                    case Direction.East:
                        ItemID = 15970;
                        MoveToWorld(new Point3D(Rudder.X + 1, Rudder.Y + 1, Rudder.Z + 9), Map);
                        break;
                }
            }
        }

        public PumpkinRudderHandle(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class DockedPumpkinRowBoat : BaseDockedBoat
    {
        public override BaseBoat Boat => new PumpkinRowBoat(BoatDirection);

        public DockedPumpkinRowBoat(BaseBoat boat)
            : base(0x50, Point3D.Zero, boat)
        {
        }

        public DockedPumpkinRowBoat(Serial serial)
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
            writer.Write(0);
        }
    }
}
