namespace Server.Items
{
    public enum DoorFacing
    {
        WestCW,
        EastCCW,
        WestCCW,
        EastCW,
        SouthCW,
        NorthCCW,
        SouthCCW,
        NorthCW,
        //Sliding Doors
        SouthSW,
        SouthSE,
        WestSS,
        WestSN
    }

    public class IronGateShort : BaseDoor
    {
        [Constructable]
        public IronGateShort(DoorFacing facing)
            : base(0x84c + (2 * (int)facing), 0x84d + (2 * (int)facing), 0xEC, 0xF3, GetOffset(facing))
        {
        }

        public IronGateShort(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer) // Default Serialize method
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader) // Default Deserialize method
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class IronGate : BaseDoor
    {
        [Constructable]
        public IronGate(DoorFacing facing)
            : base(0x824 + (2 * (int)facing), 0x825 + (2 * (int)facing), 0xEC, 0xF3, GetOffset(facing))
        {
        }

        public IronGate(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer) // Default Serialize method
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader) // Default Deserialize method
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class LightWoodGate : BaseDoor
    {
        [Constructable]
        public LightWoodGate(DoorFacing facing)
            : base(0x839 + (2 * (int)facing), 0x83A + (2 * (int)facing), 0xEB, 0xF2, GetOffset(facing))
        {
        }

        public LightWoodGate(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer) // Default Serialize method
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader) // Default Deserialize method
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DarkWoodGate : BaseDoor
    {
        [Constructable]
        public DarkWoodGate(DoorFacing facing)
            : base(0x866 + (2 * (int)facing), 0x867 + (2 * (int)facing), 0xEB, 0xF2, GetOffset(facing))
        {
        }

        public DarkWoodGate(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer) // Default Serialize method
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader) // Default Deserialize method
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class MetalDoor : BaseDoor
    {
        [Constructable]
        public MetalDoor(DoorFacing facing)
            : base(0x675 + (2 * (int)facing), 0x676 + (2 * (int)facing), 0xEC, 0xF3, GetOffset(facing))
        {
        }

        public MetalDoor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer) // Default Serialize method
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader) // Default Deserialize method
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class BarredMetalDoor : BaseDoor
    {
        [Constructable]
        public BarredMetalDoor(DoorFacing facing)
            : base(0x685 + (2 * (int)facing), 0x686 + (2 * (int)facing), 0xEC, 0xF3, GetOffset(facing))
        {
        }

        public BarredMetalDoor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer) // Default Serialize method
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader) // Default Deserialize method
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class BarredMetalDoor2 : BaseDoor
    {
        [Constructable]
        public BarredMetalDoor2(DoorFacing facing)
            : base(0x1FED + (2 * (int)facing), 0x1FEE + (2 * (int)facing), 0xEC, 0xF3, GetOffset(facing))
        {
        }

        public BarredMetalDoor2(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer) // Default Serialize method
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader) // Default Deserialize method
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class RattanDoor : BaseDoor
    {
        [Constructable]
        public RattanDoor(DoorFacing facing)
            : base(0x695 + (2 * (int)facing), 0x696 + (2 * (int)facing), 0xEB, 0xF2, GetOffset(facing))
        {
        }

        public RattanDoor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer) // Default Serialize method
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader) // Default Deserialize method
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DarkWoodDoor : BaseDoor
    {
        [Constructable]
        public DarkWoodDoor(DoorFacing facing)
            : base(0x6A5 + (2 * (int)facing), 0x6A6 + (2 * (int)facing), 0xEA, 0xF1, GetOffset(facing))
        {
        }

        public DarkWoodDoor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer) // Default Serialize method
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader) // Default Deserialize method
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class MediumWoodDoor : BaseDoor
    {
        [Constructable]
        public MediumWoodDoor(DoorFacing facing)
            : base(0x6B5 + (2 * (int)facing), 0x6B6 + (2 * (int)facing), 0xEA, 0xF1, GetOffset(facing))
        {
        }

        public MediumWoodDoor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer) // Default Serialize method
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader) // Default Deserialize method
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class MetalDoor2 : BaseDoor
    {
        [Constructable]
        public MetalDoor2(DoorFacing facing)
            : base(0x6C5 + (2 * (int)facing), 0x6C6 + (2 * (int)facing), 0xEC, 0xF3, GetOffset(facing))
        {
        }

        public MetalDoor2(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer) // Default Serialize method
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader) // Default Deserialize method
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class LightWoodDoor : BaseDoor
    {
        [Constructable]
        public LightWoodDoor(DoorFacing facing)
            : base(0x6D5 + (2 * (int)facing), 0x6D6 + (2 * (int)facing), 0xEA, 0xF1, GetOffset(facing))
        {
        }

        public LightWoodDoor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer) // Default Serialize method
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader) // Default Deserialize method
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class StrongWoodDoor : BaseDoor
    {
        [Constructable]
        public StrongWoodDoor(DoorFacing facing)
            : base(0x6E5 + (2 * (int)facing), 0x6E6 + (2 * (int)facing), 0xEA, 0xF1, GetOffset(facing))
        {
        }

        public StrongWoodDoor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer) // Default Serialize method
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader) // Default Deserialize method
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}