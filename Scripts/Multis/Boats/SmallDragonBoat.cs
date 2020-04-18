namespace Server.Multis
{
    public class SmallDragonBoat : BaseBoat
    {
        public override int NorthID => 0x4;
        public override int EastID => 0x5;
        public override int SouthID => 0x6;
        public override int WestID => 0x7;

        public override int HoldDistance => 4;
        public override int TillerManDistance => -4;

        public override Point2D StarboardOffset => new Point2D(2, 0);
        public override Point2D PortOffset => new Point2D(-2, 0);

        public override Point3D MarkOffset => new Point3D(0, 1, 3);

        public override BaseDockedBoat DockedBoat => new SmallDockedDragonBoat(this);

        [Constructable]
        public SmallDragonBoat(Direction d) : base(d, true)
        {
        }

        public SmallDragonBoat(Serial serial) : base(serial)
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

    public class SmallDragonBoatDeed : BaseBoatDeed
    {
        public override int LabelNumber => 1041206;  // small dragon ship deed
        public override BaseBoat Boat => new SmallDragonBoat(BoatDirection);

        [Constructable]
        public SmallDragonBoatDeed() : base(0x4, Point3D.Zero)
        {
        }

        public SmallDragonBoatDeed(Serial serial) : base(serial)
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

    public class SmallDockedDragonBoat : BaseDockedBoat
    {
        public override int LabelNumber => 1116742;  //Small Dragon Ship
        public override BaseBoat Boat => new SmallDragonBoat(BoatDirection);

        public SmallDockedDragonBoat(BaseBoat boat) : base(0x4, Point3D.Zero, boat)
        {
        }

        public SmallDockedDragonBoat(Serial serial) : base(serial)
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