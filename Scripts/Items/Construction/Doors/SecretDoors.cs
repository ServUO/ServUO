using System;

namespace Server.Items
{
    public class SecretStoneDoor1 : BaseDoor
    {
        [Constructable]
        public SecretStoneDoor1(DoorFacing facing)
            : base(0xE8 + (2 * (int)facing), 0xE9 + (2 * (int)facing), 0xED, 0xF4, BaseDoor.GetOffset(facing))
        {
        }

        public SecretStoneDoor1(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer) // Default Serialize method
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader) // Default Deserialize method
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class SecretDungeonDoor : BaseDoor
    {
        [Constructable]
        public SecretDungeonDoor(DoorFacing facing)
            : base(0x314 + (2 * (int)facing), 0x315 + (2 * (int)facing), 0xED, 0xF4, BaseDoor.GetOffset(facing))
        {
        }

        public SecretDungeonDoor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer) // Default Serialize method
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader) // Default Deserialize method
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class SecretStoneDoor2 : BaseDoor
    {
        [Constructable]
        public SecretStoneDoor2(DoorFacing facing)
            : base(0x324 + (2 * (int)facing), 0x325 + (2 * (int)facing), 0xED, 0xF4, BaseDoor.GetOffset(facing))
        {
        }

        public SecretStoneDoor2(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer) // Default Serialize method
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader) // Default Deserialize method
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class SecretWoodenDoor : BaseDoor
    {
        [Constructable]
        public SecretWoodenDoor(DoorFacing facing)
            : base(0x334 + (2 * (int)facing), 0x335 + (2 * (int)facing), 0xED, 0xF4, BaseDoor.GetOffset(facing))
        {
        }

        public SecretWoodenDoor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer) // Default Serialize method
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader) // Default Deserialize method
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class SecretLightWoodDoor : BaseDoor
    {
        [Constructable]
        public SecretLightWoodDoor(DoorFacing facing)
            : base(0x344 + (2 * (int)facing), 0x345 + (2 * (int)facing), 0xED, 0xF4, BaseDoor.GetOffset(facing))
        {
        }

        public SecretLightWoodDoor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer) // Default Serialize method
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader) // Default Deserialize method
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class SecretStoneDoor3 : BaseDoor
    {
        [Constructable]
        public SecretStoneDoor3(DoorFacing facing)
            : base(0x354 + (2 * (int)facing), 0x355 + (2 * (int)facing), 0xED, 0xF4, BaseDoor.GetOffset(facing))
        {
        }

        public SecretStoneDoor3(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer) // Default Serialize method
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader) // Default Deserialize method
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}