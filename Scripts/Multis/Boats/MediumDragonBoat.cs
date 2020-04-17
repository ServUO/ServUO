namespace Server.Multis
{
    public class MediumDragonBoat : BaseBoat
    {
        public override int NorthID => 0xC;
        public override int EastID => 0xD;
        public override int SouthID => 0xE;
        public override int WestID => 0xF;

        public override int HoldDistance => 4;
        public override int TillerManDistance => -5;

        public override Point2D StarboardOffset => new Point2D(2, 0);
        public override Point2D PortOffset => new Point2D(-2, 0);

        public override Point3D MarkOffset => new Point3D(0, 1, 3);

        public override BaseDockedBoat DockedBoat => new MediumDockedDragonBoat(this);

        [Constructable]
        public MediumDragonBoat(Direction d) : base(d, true)
        {
        }

        public MediumDragonBoat(Serial serial) : base(serial)
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

    public class MediumDragonBoatDeed : BaseBoatDeed
    {
        public override int LabelNumber => 1041208;  // medium dragon ship deed
        public override BaseBoat Boat => new MediumDragonBoat(BoatDirection);

        [Constructable]
        public MediumDragonBoatDeed() : base(0xC, Point3D.Zero)
        {
        }

        public MediumDragonBoatDeed(Serial serial) : base(serial)
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

    public class MediumDockedDragonBoat : BaseDockedBoat
    {
        public override int LabelNumber => 1116744;  //Medium Dragon Ship
        public override BaseBoat Boat => new MediumDragonBoat(BoatDirection);

        public MediumDockedDragonBoat(BaseBoat boat) : base(0xC, Point3D.Zero, boat)
        {
        }

        public MediumDockedDragonBoat(Serial serial) : base(serial)
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