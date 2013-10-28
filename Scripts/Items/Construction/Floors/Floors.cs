using System;

namespace Server.Items
{
    public abstract class BaseFloor : Item
    {
        public BaseFloor(int itemID, int count)
            : base(Utility.Random(itemID, count))
        {
            this.Movable = false;
        }

        public BaseFloor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class StonePaversLight : BaseFloor
    {
        [Constructable]
        public StonePaversLight()
            : base(0x519, 4)
        {
        }

        public StonePaversLight(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class StonePaversMedium : BaseFloor
    {
        [Constructable]
        public StonePaversMedium()
            : base(0x51D, 4)
        {
        }

        public StonePaversMedium(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class StonePaversDark : BaseFloor
    {
        [Constructable]
        public StonePaversDark()
            : base(0x521, 4)
        {
        }

        public StonePaversDark(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class GreyFlagstones : BaseFloor
    {
        [Constructable]
        public GreyFlagstones()
            : base(0x4FC, 4)
        {
        }

        public GreyFlagstones(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class SandFlagstones : BaseFloor
    {
        [Constructable]
        public SandFlagstones()
            : base(0x500, 4)
        {
        }

        public SandFlagstones(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class MarbleFloor : BaseFloor
    {
        [Constructable]
        public MarbleFloor()
            : base(0x50D, 2)
        {
        }

        public MarbleFloor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class GreenMarbleFloor : BaseFloor
    {
        [Constructable]
        public GreenMarbleFloor()
            : base(0x50F, 2)
        {
        }

        public GreenMarbleFloor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class GreyMarbleFloor : BaseFloor
    {
        [Constructable]
        public GreyMarbleFloor()
            : base(0x511, 4)
        {
        }

        public GreyMarbleFloor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class CobblestonesFloor : BaseFloor
    {
        [Constructable]
        public CobblestonesFloor()
            : base(0x515, 4)
        {
        }

        public CobblestonesFloor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class SandstoneFloorN : BaseFloor
    {
        [Constructable]
        public SandstoneFloorN()
            : base(0x525, 4)
        {
        }

        public SandstoneFloorN(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class SandstoneFloorW : BaseFloor
    {
        [Constructable]
        public SandstoneFloorW()
            : base(0x529, 4)
        {
        }

        public SandstoneFloorW(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DarkSandstoneFloorN : BaseFloor
    {
        [Constructable]
        public DarkSandstoneFloorN()
            : base(0x52F, 4)
        {
        }

        public DarkSandstoneFloorN(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DarkSandstoneFloorW : BaseFloor
    {
        [Constructable]
        public DarkSandstoneFloorW()
            : base(0x533, 4)
        {
        }

        public DarkSandstoneFloorW(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class BricksFloor1 : BaseFloor
    {
        [Constructable]
        public BricksFloor1()
            : base(0x4E2, 8)
        {
        }

        public BricksFloor1(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class BricksFloor2 : BaseFloor
    {
        [Constructable]
        public BricksFloor2()
            : base(0x537, 4)
        {
        }

        public BricksFloor2(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class CaveFloorCenter : BaseFloor
    {
        [Constructable]
        public CaveFloorCenter()
            : base(0x53B, 4)
        {
        }

        public CaveFloorCenter(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class CaveFloorSouth : BaseFloor
    {
        [Constructable]
        public CaveFloorSouth()
            : base(0x541, 3)
        {
        }

        public CaveFloorSouth(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class CaveFloorEast : BaseFloor
    {
        [Constructable]
        public CaveFloorEast()
            : base(0x544, 3)
        {
        }

        public CaveFloorEast(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class CaveFloorWest : BaseFloor
    {
        [Constructable]
        public CaveFloorWest()
            : base(0x54A, 3)
        {
        }

        public CaveFloorWest(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class CaveFloorNorth : BaseFloor
    {
        [Constructable]
        public CaveFloorNorth()
            : base(0x54D, 3)
        {
        }

        public CaveFloorNorth(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class MarblePavers : BaseFloor
    {
        [Constructable]
        public MarblePavers()
            : base(0x495, 4)
        {
        }

        public MarblePavers(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class BlueSlateFloorCenter : BaseFloor
    {
        [Constructable]
        public BlueSlateFloorCenter()
            : base(0x49B, 1)
        {
        }

        public BlueSlateFloorCenter(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class GreySlateFloor : BaseFloor
    {
        [Constructable]
        public GreySlateFloor()
            : base(0x49C, 1)
        {
        }

        public GreySlateFloor(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}