using System;

namespace Server.Multis
{
    public class MediumBoat : BaseBoat
    {
        [Constructable]
        public MediumBoat()
        {
        }

        public MediumBoat(Serial serial)
            : base(serial)
        {
        }

        public override int NorthID
        {
            get
            {
                return 0x8;
            }
        }
        public override int EastID
        {
            get
            {
                return 0x9;
            }
        }
        public override int SouthID
        {
            get
            {
                return 0xA;
            }
        }
        public override int WestID
        {
            get
            {
                return 0xB;
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
                return -5;
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
                return new MediumDockedBoat(this);
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

    public class MediumBoatDeed : BaseBoatDeed
    {
        [Constructable]
        public MediumBoatDeed()
            : base(0x8, Point3D.Zero)
        {
        }

        public MediumBoatDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1041207;
            }
        }// medium ship deed
        public override BaseBoat Boat
        {
            get
            {
                return new MediumBoat();
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

    public class MediumDockedBoat : BaseDockedBoat
    {
        public MediumDockedBoat(BaseBoat boat)
            : base(0x8, Point3D.Zero, boat)
        {
        }

        public MediumDockedBoat(Serial serial)
            : base(serial)
        {
        }

        public override BaseBoat Boat
        {
            get
            {
                return new MediumBoat();
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