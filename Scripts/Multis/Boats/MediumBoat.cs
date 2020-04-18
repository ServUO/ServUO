namespace Server.Multis
{
    public class MediumBoat : BaseBoat
    {
        public override int NorthID => 0x8;
        public override int EastID => 0x9;
        public override int SouthID => 0xA;
        public override int WestID => 0xB;

        public override int HoldDistance => 4;
        public override int TillerManDistance => -5;

        public override Point2D StarboardOffset => new Point2D(2, 0);
        public override Point2D PortOffset => new Point2D(-2, 0);

        public override Point3D MarkOffset => new Point3D(0, 1, 3);

        public override BaseDockedBoat DockedBoat => new MediumDockedBoat(this);

        [Constructable]
        public MediumBoat(Direction d) : base(d, true)
        {
        }

        public MediumBoat(Serial serial) : base(serial)
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

    public class MediumBoatDeed : BaseBoatDeed
    {
        public override int LabelNumber => 1041207;  // medium ship deed
        public override BaseBoat Boat => new MediumBoat(BoatDirection);

        [Constructable]
        public MediumBoatDeed() : base(0x8, Point3D.Zero)
        {
        }

        public MediumBoatDeed(Serial serial) : base(serial)
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

    public class MediumDockedBoat : BaseDockedBoat
    {
        public override int LabelNumber => 1116743;  //Medium Ship
        public override BaseBoat Boat => new MediumBoat(BoatDirection);

        public MediumDockedBoat(BaseBoat boat) : base(0x8, Point3D.Zero, boat)
        {
        }

        public MediumDockedBoat(Serial serial) : base(serial)
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