using System;

namespace Server.Multis
{
    public class SmallDragonBoat : BaseBoat
    {
        [Constructable]
        public SmallDragonBoat()
        {
        }

        public SmallDragonBoat(Serial serial)
            : base(serial)
        {
        }

        public override int NorthID
        {
            get
            {
                return 0x4;
            }
        }
        public override int EastID
        {
            get
            {
                return 0x5;
            }
        }
        public override int SouthID
        {
            get
            {
                return 0x6;
            }
        }
        public override int WestID
        {
            get
            {
                return 0x7;
            }
        }
        public override int HoldDistance
        {
            get
            {
                return 4;
            }
        }
        public override int TillerManDistance
        {
            get
            {
                return -4;
            }
        }
        public override Point2D StarboardOffset
        {
            get
            {
                return new Point2D(2, 0);
            }
        }
        public override Point2D PortOffset
        {
            get
            {
                return new Point2D(-2, 0);
            }
        }
        public override Point3D MarkOffset
        {
            get
            {
                return new Point3D(0, 1, 3);
            }
        }
        public override BaseDockedBoat DockedBoat
        {
            get
            {
                return new SmallDockedDragonBoat(this);
            }
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

    public class SmallDragonBoatDeed : BaseBoatDeed
    {
        [Constructable]
        public SmallDragonBoatDeed()
            : base(0x4, Point3D.Zero)
        {
        }

        public SmallDragonBoatDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1041206;
            }
        }// small dragon ship deed
        public override BaseBoat Boat
        {
            get
            {
                return new SmallDragonBoat();
            }
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

    public class SmallDockedDragonBoat : BaseDockedBoat
    {
        public SmallDockedDragonBoat(BaseBoat boat)
            : base(0x4, Point3D.Zero, boat)
        {
        }

        public SmallDockedDragonBoat(Serial serial)
            : base(serial)
        {
        }

        public override BaseBoat Boat
        {
            get
            {
                return new SmallDragonBoat();
            }
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