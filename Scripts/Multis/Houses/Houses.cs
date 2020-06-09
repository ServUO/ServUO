using Server.Items;

namespace Server.Multis
{
    public class SmallOldHouse : BaseHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-3, -3, 7, 7), new Rectangle2D(-1, 4, 3, 1) };
        public SmallOldHouse(Mobile owner, int id)
            : base(id, owner, 425, 3)
        {
            AddSouthDoor(0, 3, 7);

            SetSign(2, 4, 5);
        }

        public SmallOldHouse(Serial serial)
            : base(serial)
        {
        }

        public override Rectangle2D[] Area => AreaArray;
        public override Point3D BaseBanLocation => new Point3D(2, 4, 0);
        public override HousePlacementEntry ConvertEntry => HousePlacementEntry.TwoStoryFoundations[0];

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GuildHouse : BaseHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-7, -7, 14, 14), new Rectangle2D(-2, 7, 4, 1) };
        public GuildHouse(Mobile owner)
            : base(0x74, owner, 1100, 8)
        {
            AddSouthDoors(-1, 6, 7);

            SetSign(4, 8, 16);

            AddSouthDoor(-3, -1, 7);
            AddSouthDoor(3, -1, 7);
        }

        public GuildHouse(Serial serial)
            : base(serial)
        {
        }

        public override HousePlacementEntry ConvertEntry => HousePlacementEntry.ThreeStoryFoundations[20];
        public override int ConvertOffsetX => -1;
        public override int ConvertOffsetY => -1;
        public override Rectangle2D[] Area => AreaArray;
        public override Point3D BaseBanLocation => new Point3D(4, 8, 0);

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class TwoStoryHouse : BaseHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-7, 0, 14, 7), new Rectangle2D(-7, -7, 9, 7), new Rectangle2D(-4, 7, 4, 1) };
        public TwoStoryHouse(Mobile owner, int id)
            : base(id, owner, 1370, 10)
        {
            AddSouthDoors(-3, 6, 7);

            SetSign(2, 8, 16);

            AddSouthDoor(-3, 0, 7);
            AddSouthDoor(id == 0x76 ? -2 : -3, 0, 27);
        }

        public TwoStoryHouse(Serial serial)
            : base(serial)
        {
        }

        public override Rectangle2D[] Area => AreaArray;
        public override Point3D BaseBanLocation => new Point3D(2, 8, 0);

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class Tower : BaseHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-7, -7, 16, 14), new Rectangle2D(-1, 7, 4, 2), new Rectangle2D(-11, 0, 4, 7), new Rectangle2D(9, 0, 4, 7) };
        public Tower(Mobile owner)
            : base(0x7A, owner, 2119, 15)
        {
            AddSouthDoors(false, 0, 6, 6);

            SetSign(5, 8, 16);

            AddSouthDoor(false, 3, -2, 6);
            AddEastDoor(false, 1, 4, 26);
            AddEastDoor(false, 1, 4, 46);
        }

        public Tower(Serial serial)
            : base(serial)
        {
        }

        public override HousePlacementEntry ConvertEntry => HousePlacementEntry.ThreeStoryFoundations[37];
        public override int ConvertOffsetY => -1;
        public override Rectangle2D[] Area => AreaArray;
        public override Point3D BaseBanLocation => new Point3D(5, 8, 0);

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class Keep : BaseHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-11, -11, 7, 8), new Rectangle2D(-11, 5, 7, 8), new Rectangle2D(6, -11, 7, 8), new Rectangle2D(6, 5, 7, 8), new Rectangle2D(-9, -3, 5, 8), new Rectangle2D(6, -3, 5, 8), new Rectangle2D(-4, -9, 10, 20), new Rectangle2D(-1, 11, 4, 1) };
        public Keep(Mobile owner)
            : base(0x7C, owner, 2625, 18)
        {
            AddSouthDoors(false, 0, 10, 6);

            SetSign(5, 12, 16);
        }

        public Keep(Serial serial)
            : base(serial)
        {
        }

        public override Rectangle2D[] Area => AreaArray;
        public override Point3D BaseBanLocation => new Point3D(5, 13, 0);

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class Castle : BaseHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-15, -15, 31, 31), new Rectangle2D(-1, 16, 4, 1) };
        public Castle(Mobile owner)
            : base(0x7E, owner, 4076, 28)
        {
            AddSouthDoors(false, 0, 15, 6);

            SetSign(5, 17, 16);

            AddSouthDoors(false, 0, 11, 6, true);
            AddSouthDoors(false, 0, 5, 6, false);
            AddSouthDoors(false, -1, -11, 6, false);
        }

        public Castle(Serial serial)
            : base(serial)
        {
        }

        public override Rectangle2D[] Area => AreaArray;
        public override Point3D BaseBanLocation => new Point3D(5, 17, 0);

        protected override bool IsInsideSpecial(Point3D p, StaticTile[] tiles)
        {
            return p.X >= X - 10 && p.X <= X + 10 && p.Y >= Y - 10 && p.Y <= Y + 10;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class LargePatioHouse : BaseHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-7, -7, 15, 14), new Rectangle2D(-5, 7, 4, 1) };
        public LargePatioHouse(Mobile owner)
            : base(0x8C, owner, 1100, 8)
        {
            AddSouthDoors(-4, 6, 7);

            SetSign(1, 8, 16);

            AddEastDoor(1, 4, 7);
            AddEastDoor(1, -4, 7);
            AddSouthDoor(4, -1, 7);
        }

        public LargePatioHouse(Serial serial)
            : base(serial)
        {
        }

        public override HousePlacementEntry ConvertEntry => HousePlacementEntry.ThreeStoryFoundations[29];
        public override int ConvertOffsetY => -1;
        public override Rectangle2D[] Area => AreaArray;
        public override Point3D BaseBanLocation => new Point3D(1, 8, 0);

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class LargeMarbleHouse : BaseHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-7, -7, 15, 14), new Rectangle2D(-6, 7, 6, 1) };
        public LargeMarbleHouse(Mobile owner)
            : base(0x96, owner, 1370, 10)
        {
            AddSouthDoors(false, -4, 3, 4);

            SetSign(1, 8, 11);
        }

        public LargeMarbleHouse(Serial serial)
            : base(serial)
        {
        }

        public override HousePlacementEntry ConvertEntry => HousePlacementEntry.ThreeStoryFoundations[29];
        public override int ConvertOffsetY => -1;
        public override Rectangle2D[] Area => AreaArray;
        public override Point3D BaseBanLocation => new Point3D(1, 8, 0);

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class SmallTower : BaseHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-3, -3, 8, 7), new Rectangle2D(2, 4, 3, 1) };
        public SmallTower(Mobile owner)
            : base(0x98, owner, 580, 4)
        {
            AddSouthDoor(false, 3, 3, 6);

            SetSign(1, 4, 5);
        }

        public SmallTower(Serial serial)
            : base(serial)
        {
        }

        public override HousePlacementEntry ConvertEntry => HousePlacementEntry.TwoStoryFoundations[6];
        public override Rectangle2D[] Area => AreaArray;
        public override Point3D BaseBanLocation => new Point3D(1, 4, 0);

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class LogCabin : BaseHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-3, -6, 8, 13) };
        public LogCabin(Mobile owner)
            : base(0x9A, owner, 1100, 8)
        {
            AddSouthDoor(1, 4, 8);

            SetSign(5, 8, 20);

            AddSouthDoor(1, 0, 29);
        }

        public LogCabin(Serial serial)
            : base(serial)
        {
        }

        public override HousePlacementEntry ConvertEntry => HousePlacementEntry.TwoStoryFoundations[12];
        public override Rectangle2D[] Area => AreaArray;
        public override Point3D BaseBanLocation => new Point3D(5, 8, 0);

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class SandStonePatio : BaseHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-5, -4, 12, 8), new Rectangle2D(-2, 4, 3, 1) };
        public SandStonePatio(Mobile owner)
            : base(0x9C, owner, 850, 6)
        {
            AddSouthDoor(-1, 3, 6);

            SetSign(4, 6, 24);
        }

        public SandStonePatio(Serial serial)
            : base(serial)
        {
        }

        public override HousePlacementEntry ConvertEntry => HousePlacementEntry.TwoStoryFoundations[35];
        public override int ConvertOffsetY => -1;
        public override Rectangle2D[] Area => AreaArray;
        public override Point3D BaseBanLocation => new Point3D(4, 6, 0);

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class TwoStoryVilla : BaseHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-5, -5, 11, 11), new Rectangle2D(2, 6, 4, 1) };
        public TwoStoryVilla(Mobile owner)
            : base(0x9E, owner, 1100, 8)
        {
            AddSouthDoors(3, 1, 5);

            SetSign(3, 8, 24);

            AddEastDoor(1, 0, 25);
            AddSouthDoor(-3, -1, 25);
        }

        public TwoStoryVilla(Serial serial)
            : base(serial)
        {
        }

        public override HousePlacementEntry ConvertEntry => HousePlacementEntry.TwoStoryFoundations[31];
        public override Rectangle2D[] Area => AreaArray;
        public override Point3D BaseBanLocation => new Point3D(3, 8, 0);

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class SmallShop : BaseHouse
    {
        public static Rectangle2D[] AreaArray1 = new Rectangle2D[] { new Rectangle2D(-3, -3, 7, 7), new Rectangle2D(-1, 4, 4, 1) };
        public static Rectangle2D[] AreaArray2 = new Rectangle2D[] { new Rectangle2D(-3, -3, 7, 7), new Rectangle2D(-2, 4, 3, 1) };
        public SmallShop(Mobile owner, int id)
            : base(id, owner, 425, 3)
        {
            BaseDoor door = MakeDoor(false, DoorFacing.EastCW);

            if (door is BaseHouseDoor)
                ((BaseHouseDoor)door).Facing = DoorFacing.EastCCW;

            AddDoor(door, -2, 0, id == 0xA2 ? 24 : 27);

            SetSign(3, 4, 7 - (id == 0xA2 ? 2 : 0));
        }

        public SmallShop(Serial serial)
            : base(serial)
        {
        }

        public override Rectangle2D[] Area => (ItemID == 0x40A2 ? AreaArray1 : AreaArray2);
        public override Point3D BaseBanLocation => new Point3D(3, 4, 0);
        public override HousePlacementEntry ConvertEntry => HousePlacementEntry.TwoStoryFoundations[0];

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
