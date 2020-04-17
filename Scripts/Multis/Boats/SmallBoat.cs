namespace Server.Multis
{
    public class SmallBoat : BaseBoat
    {
        public override int NorthID => 0x0;
        public override int EastID => 0x1;
        public override int SouthID => 0x2;
        public override int WestID => 0x3;

        public override int HoldDistance => 4;
        public override int TillerManDistance => -4;

        public override Point2D StarboardOffset => new Point2D(2, 0);
        public override Point2D PortOffset => new Point2D(-2, 0);

        public override Point3D MarkOffset => new Point3D(0, 1, 3);

        public override BaseDockedBoat DockedBoat => new SmallDockedBoat(this);

        [Constructable]
        public SmallBoat(Direction d) : base(d, true)
        {
        }

        public SmallBoat(Serial serial) : base(serial)
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

    public class SmallBoatDeed : BaseBoatDeed
    {
        public override int LabelNumber => 1041205;  // small ship deed
        public override BaseBoat Boat => new SmallBoat(BoatDirection);

        [Constructable]
        public SmallBoatDeed() : base(0x0, Point3D.Zero)
        {
        }

        public SmallBoatDeed(Serial serial) : base(serial)
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

    public class SmallDockedBoat : BaseDockedBoat
    {
        public override int LabelNumber => 1116741;  //Small Ship
        public override BaseBoat Boat => new SmallBoat(BoatDirection);

        public SmallDockedBoat(BaseBoat boat) : base(0x0, Point3D.Zero, boat)
        {
        }

        public SmallDockedBoat(Serial serial) : base(serial)
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