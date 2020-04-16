namespace Server.Items
{
    public class PaperSlidingDoor : BaseDoor
    {
        [Constructable]
        public PaperSlidingDoor(DoorFacing facing)
            : base(0x2A05 + (2 * (int)facing), 0x2A06 + (2 * (int)facing), 0x539, 0x539, new Point3D(0, 0, 0))
        {
        }

        public PaperSlidingDoor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class ClothSlidingDoor : BaseDoor
    {
        [Constructable]
        public ClothSlidingDoor(DoorFacing facing)
            : base(0x2A0D + (2 * (int)facing), 0x2A0E + (2 * (int)facing), 0x539, 0x539, new Point3D(0, 0, 0))
        {
        }

        public ClothSlidingDoor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class WoodenSlidingDoor : BaseDoor
    {
        [Constructable]
        public WoodenSlidingDoor(DoorFacing facing)
            : base(0x2A15 + (2 * (int)facing), 0x2A16 + (2 * (int)facing), 0x539, 0x539, new Point3D(0, 0, 0))
        {
        }

        public WoodenSlidingDoor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}