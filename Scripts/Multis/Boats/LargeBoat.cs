namespace Server.Multis
{
    public class LargeBoat : BaseBoat
    {
        public override int NorthID => 0x10;
        public override int EastID => 0x11;
        public override int SouthID => 0x12;
        public override int WestID => 0x13;

        public override int HoldDistance => 5;
        public override int TillerManDistance => -5;

        public override Point2D StarboardOffset => new Point2D(2, -1);
        public override Point2D PortOffset => new Point2D(-2, -1);

        public override Point3D MarkOffset => new Point3D(0, 0, 3);

        public override BaseDockedBoat DockedBoat => new LargeDockedBoat(this);

        [Constructable]
        public LargeBoat(Direction d) : base(d, true)
        {
        }

        public LargeBoat(Serial serial) : base(serial)
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

    public class LargeBoatDeed : BaseBoatDeed
    {
        public override int LabelNumber => 1041209;  // large ship deed
        public override BaseBoat Boat => new LargeBoat(BoatDirection);

        [Constructable]
        public LargeBoatDeed() : base(0x10, new Point3D(0, -1, 0))
        {
        }

        public LargeBoatDeed(Serial serial) : base(serial)
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

    public class LargeDockedBoat : BaseDockedBoat
    {
        public override int LabelNumber => 1116745;  //Large Ship
        public override BaseBoat Boat => new LargeBoat(BoatDirection);

        public LargeDockedBoat(BaseBoat boat) : base(0x10, new Point3D(0, -1, 0), boat)
        {
        }

        public LargeDockedBoat(Serial serial) : base(serial)
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