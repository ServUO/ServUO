using System;
using System.Linq;
using System.Collections.Generic;

using Server.Items;

namespace Server.Multis
{
    public enum ContestHouseType
    {
        Keep,
        Castle,
        Other
    }

    public class BaseContestHouse : BaseHouse
    {
        public ContestHouseType HouseType { get; set; }
        public List<Item> Fixtures { get; private set; }

        public virtual int SignPostID { get { return 9; } }

        public override Point3D BaseBanLocation
        { 
            get
            { 
                return new Point3D(Components.Min.X, Components.Height - 1 - Components.Center.Y, 0); 
            } 
        }

        public override Rectangle2D[] Area
        {
            get
            {
                MultiComponentList mcl = Components;

                return new Rectangle2D[] { new Rectangle2D(mcl.Min.X, mcl.Min.Y, mcl.Width, mcl.Height) };
            }
        }

        public BaseContestHouse(ContestHouseType type, int multiID, Mobile owner, int MaxLockDown, int MaxSecure)
            : base(multiID, owner, MaxLockDown, MaxSecure)
        {
            HouseType = type;
        }

        protected void SetSign(int xOffset, int yOffset, int zOffset, bool post)
        {
            SetSign(xOffset, yOffset, zOffset);

            var hanger = new Static(0xB9E);
            hanger.MoveToWorld(new Point3D(X + xOffset, Y + yOffset, Z + zOffset), Map);

            AddFixture(hanger);

            if (post)
            {
                var signPost = new Static(SignPostID);
                signPost.MoveToWorld(new Point3D(X + xOffset, Y + yOffset - 1, Z + zOffset), Map);

                AddFixture(signPost);
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (Fixtures != null)
            {
                foreach (var item in Fixtures)
                {
                    item.Delete();
                }
            }
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            base.OnLocationChange(oldLocation);

            int x = base.Location.X - oldLocation.X;
            int y = base.Location.Y - oldLocation.Y;
            int z = base.Location.Z - oldLocation.Z;

            if (Fixtures != null)
            {
                foreach (var item in Fixtures)
                {
                    item.Location = new Point3D(item.X + x, item.Y + y, item.Z + z);
                }
            }
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            if (Fixtures != null)
            {
                foreach (var item in Fixtures)
                {
                    item.Map = Map;
                }
            }
        }

        public void AddLinkedSouthDoors(int id1, int id2, int x, int y, int z)
        {
            var door1 = AddDoor(id1, x, y, z);
            var door2 = AddDoor(id2, x + 1, y, z);

            door1.Link = door2;
            door2.Link = door1;
        }

        public void AddLinkedEastDoors(int id1, int id2, int x, int y, int z)
        {
            var door1 = AddDoor(id1, x, y, z);
            var door2 = AddDoor(id2, x, y - 1, z);

            door1.Link = door2;
            door2.Link = door1;
        }

        public void AddTeleporters(int id, Point3D offset1, Point3D offset2)
        {
            var tele1 = new HouseTeleporter(id);
            var tele2 = new HouseTeleporter(id);

            tele1.Target = tele2;
            tele2.Target = tele1;

            tele1.MoveToWorld(new Point3D(X + offset1.X, Y + offset1.Y, offset1.Z), Map);
            tele2.MoveToWorld(new Point3D(X + offset2.X, Y + offset2.Y, offset2.Z), Map);

            AddFixture(tele1);
            AddFixture(tele2);
        }

        public void AddFixture(Item item)
        {
            if (Fixtures == null)
            {
                Fixtures = new List<Item>();
            }

            Fixtures.Add(item);
        }

        public override bool IsInside(Point3D p, int height)
        {
            bool isInside = base.IsInside(p, height);

            if (!isInside)
            {
                return Fixtures != null && Fixtures.OfType<HouseTeleporter>().Any(fix => fix.Location == p);
            }

            return isInside;
        }

        public BaseContestHouse(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);//version

            writer.Write((int)HouseType);

            writer.Write(Fixtures != null ? Fixtures.Count : 0);

            if (Fixtures != null)
            {
                foreach (var item in Fixtures)
                {
                    writer.Write(item);
                }
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            HouseType = (ContestHouseType)reader.ReadInt();

            int count = reader.ReadInt();

            for(int i = 0; i < count; i++)
            {
                var item = reader.ReadItem();

                if (item != null)
                {
                    AddFixture(item);
                }
            }
        }
    }

    public class TrinsicKeep : BaseContestHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] 
        { 
            new Rectangle2D(-11, -11, 23, 23), new Rectangle2D(-10, 13, 6, 1),
            new Rectangle2D(-2, 13, 6, 1), new Rectangle2D(6, 13, 7, 1) 
        };
        
        public TrinsicKeep(Mobile owner)
            : base(ContestHouseType.Keep, 0x147E, owner, 2113, 18)
        {
            AddSouthDoors(false, 0, 10, 7);
            AddEastDoors(false, -9, 0, 7);
            AddEastDoors(false, 10, 0, 7, 0, false, true);

            SetSign(-11, 13, 7, false);

            AddSouthDoors(false, 6, 5, 7);
            AddSouthDoors(false, -6, 5, 7);

            AddSouthDoor(false, -7, 4, 47);
            AddSouthDoor(false, 8, 4, 47);
            AddSouthDoor(false, -6, -2, 47);
            AddSouthDoor(false, 8, -2, 47);

            AddSouthDoors(false, 0, -2, 47);

            AddTeleporters(0x181E, new Point3D(9, 4, 7), new Point3D(9, 3, 47));
            AddTeleporters(0x181D, new Point3D(8, 4, 7), new Point3D(9, 4, 27));
            AddTeleporters(0x181F, new Point3D(9, 3, 7), new Point3D(-10, -6, 67));
        }

        public TrinsicKeep(Serial serial)
            : base(serial)
        {
        }

        public override Rectangle2D[] Area
        {
            get
            {
                return AreaArray;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GothicRoseCastle : BaseContestHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[]
        { 
            new Rectangle2D(-15, -15, 31, 31),
            new Rectangle2D(-14, 16, 11, 1), 
            new Rectangle2D(-2, 16, 6, 1),
            new Rectangle2D(5, 16, 11, 1) 
        };

        public GothicRoseCastle(Mobile owner)
            : base(ContestHouseType.Castle, 0x147F, owner, 3281, 28)
        {
            AddDoor(0x41CF, 0, 15, 7);
            AddDoor(0x41D1, 2, 15, 7);

            SetSign(-15, 16, 7, false);
        }

        public GothicRoseCastle(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class ElsaCastle : BaseContestHouse
    {
        public ElsaCastle(Mobile owner)
            : base(ContestHouseType.Castle, 0x1480, owner, 3281, 28)
        {
            AddDoor(0x31A0, -13, 9, 7);
            AddDoor(0x31A0, 14, 9, 7);

            AddLinkedEastDoors(0x2D63, 0x31A2, -8, 6, 7);
            AddLinkedEastDoors(0x2D63, 0x31A2, 6, 6, 7);
            AddLinkedEastDoors(0x2D63, 0x31A2, -8, -10, 7);
            AddLinkedEastDoors(0x2D63, 0x31A2, 6, -10, 7);

            AddDoor(0x1FF9, -12, 14, 7);
            AddDoor(0x1FF5, 12, 14, 7);

            AddDoor(0x854, 4, 14, 7);
            AddDoor(0x858, -3, 14, 7);

            AddLinkedSouthDoors(0x2D65, 0x31A0, 0, 9, 27);
            AddLinkedSouthDoors(0x2D65, 0x31A0, -7, -1, 27);
            AddLinkedSouthDoors(0x2D65, 0x31A0, 8, -1, 27);

            AddDoor(0x2D65, -13, 9, 27);
            AddDoor(0x2D65, 14, 9, 27);

            AddDoor(0x1FF3, 2, -13, 47);

            SetSign(-15, 16, 7, false);
        }

        public ElsaCastle(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class Spires : BaseContestHouse
    {
        public Spires(Mobile owner)
            : base(ContestHouseType.Castle, 0x1481, owner, 3281, 28)
        {
            AddSouthDoors(true, 0, 12, 7);

            AddEastDoor(false, -7, 13, 27);
            AddEastDoors(false, 7, 6, 27, false, true);
            AddSouthDoors(false, 13, 1, 27);

            AddEastDoors(false, 7, -4, 47, false, true);
            AddDoor(0x32A, -13, 12, 47);
            AddDoor(0x32A, -13, 1, 47);
            AddDoor(0x32A, -13, -2, 47);
            AddDoor(0x32A, -13, -12, 47);
            AddDoor(0x330, -7, -13, 47);
            AddDoor(0x330, 4, -13, 47);

            SetSign(-15, 16, 7, false);
        }

        public Spires(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class CastleOfOceania : BaseContestHouse
    {
        public CastleOfOceania(Mobile owner)
            : base(ContestHouseType.Castle, 0x1482, owner, 3281, 28)
        {
            AddLinkedSouthDoors(0x50C8, 0x50CA, 0, 10, 7);
            AddLinkedEastDoors(0x5146, 0x5148, -10, -12, 7);
            AddLinkedEastDoors(0x5146, 0x5148, 9, -12, 7);

            AddLinkedEastDoors(0x2D6B, 0x31A8, -8, -10, 27);
            AddLinkedEastDoors(0x2D6B, 0x31A8, -8, -4, 27);
            AddLinkedEastDoors(0x2D6B, 0x31A8, -8, 1, 27);
            AddLinkedEastDoors(0x2D6B, 0x31A8, 7, -7, 27);
            AddLinkedEastDoors(0x2D6B, 0x31A8, 7, -1, 27);
            AddLinkedSouthDoors(0x2D6D, 0x31AA, 10, -5, 27);

            AddDoor(0x31AA, 11, 3, 27);
            AddDoor(0x31AA, -11, 3, 27);
            AddDoor(0x2D6D, -11, -7, 27);

            AddDoor(0x2D6B, -6, -10, 47);
            AddDoor(0x2D6B, -6, -5, 47);
            AddDoor(0x2D6B, -6, 0, 47);
            AddDoor(0x31A8, 1, -10, 47);
            AddDoor(0x31A8, 1, -1, 47);

            AddLinkedEastDoors(0x2D6B, 0x31A8, 5, -5, 47);

            SetSign(-15, 16, 7, false);
        }

        public CastleOfOceania(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class FeudalCastle : BaseContestHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] 
        {
            new Rectangle2D(-15, -15, 31, 31), 
            new Rectangle2D(5, 16, 1, 1),
            new Rectangle2D(7, 16, 4, 1),
            new Rectangle2D(12, 16, 1, 1) 
        };

        public FeudalCastle(Mobile owner)
            : base(ContestHouseType.Castle, 0x1483, owner, 3281, 28)
        {
            AddLinkedSouthDoors(0x2A0D, 0x2A0F, -5, -1, 7);

            AddDoor(0x2A0F, 4, -7, 7);
            AddDoor(0x2A0F, 7, -7, 7);
            AddDoor(0x2A0D, -12, -1, 7);

            AddDoor(0x2A0D, -11, -1, 27);
            AddDoor(0x2A0F, -10, -1, 27);
            AddDoor(0x2A11, 4, -12, 27);

            AddDoor(0x2A11, 4, -9, 47);

            SetSign(-15, 16, 7, true);
        }

        public FeudalCastle(Serial serial)
            : base(serial)
        {
        }

        public override Rectangle2D[] Area
        {
            get
            {
                return AreaArray;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class RobinsNest : BaseContestHouse
    {
        public RobinsNest(Mobile owner)
            : base(ContestHouseType.Keep, 0x1484, owner, 2113, 18)
        {
            AddLinkedSouthDoors(0x6E5, 0x6E7, -3, 11, 7);
            AddLinkedSouthDoors(0x6E5, 0x6E7, 8, 5, 7);
            AddLinkedEastDoors(0x6ED, 0x6EF, 10, -2, 7);

            AddLinkedSouthDoors(0x6E5, 0x6E7, -3, 5, 27);

            AddLinkedEastDoors(0x6ED, 0x6EF, 5, 3, 47);

            AddTeleporters(0x181E, new Point3D(4, 10, 7), new Point3D(9, 4, 27));
            AddTeleporters(0x181F, new Point3D(4, 4, 47), new Point3D(9, 3, 27));
            AddTeleporters(0x1822, new Point3D(4, 3, 47), new Point3D(11, -10, 67));

            SetSign(-11, 13, 7, false);
        }

        public RobinsNest(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                AddTeleporters(0x1822, new Point3D(4, 3, 47), new Point3D(11, -10, 67));
            }
        }
    }

    public class TraditionalKeep : BaseContestHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] 
        { 
            new Rectangle2D(-11, -11, 23, 23),
            new Rectangle2D(-10, 13, 6, 1),
            new Rectangle2D(-2, 13, 6, 1),
            new Rectangle2D(6, 13, 7, 1),
        };

        public TraditionalKeep(Mobile owner)
            : base(ContestHouseType.Keep, 0x1485, owner, 2113, 18)
        {
            AddSouthDoors(false, 0, 10, 7);
            AddDoor(0x328, -9, 12, 7);
            AddDoor(0x328, 11, 12, 7);
            AddDoor(0x32C, -7, -3, 7);

            SetSign(-11, 13, 7, false);
        }

        public TraditionalKeep(Serial serial)
            : base(serial)
        {
        }

        public override Rectangle2D[] Area
        {
            get
            {
                return AreaArray;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class VillaCrowley : BaseContestHouse
    {
        public VillaCrowley(Mobile owner)
            : base(ContestHouseType.Keep, 0x1486, owner, 2113, 18)
        {
            AddSouthDoors(0, 4, 7);
            AddSouthDoor(-10, 4, 7);

            AddEastDoor(3, -6, 27);

            AddDoor(0x6AB, -6, -4, 47);
            AddDoor(0x6AB, 1, -9, 47);
            AddDoor(0x6AB, 8, -9, 47);

            AddSouthDoor(1, -3, 47);
            AddDoor(0x6AD, -3, 1, 47);

            SetSign(-11, 13, 7, true);
        }

        public VillaCrowley(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class DarkthornKeep : BaseContestHouse
    {
        public DarkthornKeep(Mobile owner)
            : base(ContestHouseType.Keep, 0x1487, owner, 2113, 18)
        {
            AddLinkedSouthDoors(0x41CF, 0x41D1, 0, 10, 7);
            AddLinkedSouthDoors(0x41CF, 0x41D1, 0, 6, 27);

            SetSign(-11, 13, 7, false);
        }

        public DarkthornKeep(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class SandalwoodKeep : BaseContestHouse
    {
        public override int SignPostID { get { return 353; } }

        public SandalwoodKeep(Mobile owner)
            : base(ContestHouseType.Keep, 0x1488, owner, 2113, 18)
        {
            AddLinkedSouthDoors(0x9B3C, 0x9B3E, 1, 6, 7);
            AddLinkedEastDoors(0x9B48, 0x9B4A, -6, -8, 7);
            AddLinkedEastDoors(0x9B44, 0x9B46, 8, -8, 7);
            AddLinkedEastDoors(0x9B44, 0x9B46, 10, 1, 7);
            AddLinkedSouthDoors(0x86A, 0x86C, -6, 12, 7);
            AddLinkedSouthDoors(0x86A, 0x86C, 7, 12, 7);

            AddDoor(0x86C, 11, 6, 7);
            AddDoor(0x86C, 11, -6, 7);

            AddTeleporters(0x1820, new Point3D(9, 5, 7), new Point3D(7, -7, 47));
            AddTeleporters(0x181E, new Point3D(8, 5, 7), new Point3D(9, 5, 27));

            AddLinkedSouthDoors(0x9B3C, 0x9B3E, -10, 2, 27);

            AddDoor(0x9B42, 1, 3, 47);

            SetSign(-11, 13, 7, true);
        }

        public SandalwoodKeep(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                AddTeleporters(0x1820, new Point3D(9, 5, 7), new Point3D(7, -7, 47));
                AddTeleporters(0x181E, new Point3D(8, 5, 7), new Point3D(9, 5, 27));
            }
        }
    }

    public class CasaMoga : BaseContestHouse
    {
        public CasaMoga(Mobile owner)
            : base(ContestHouseType.Keep, 0x1489, owner, 2113, 18)
        {
            AddLinkedSouthDoors(0x6D5, 0x6D7, 0, 8, 7);
            AddDoor(0x6DF, -2, 5, 7);
            AddLinkedEastDoors(0x6DD, 0x6DF, -2, -7, 7);
            AddLinkedEastDoors(0x6E1, 0x6E3, 3, -7, 7);

            AddTeleporters(0x1822, new Point3D(-1, 9, 7), new Point3D(4, -9, 7));
            AddTeleporters(0x1821, new Point3D(-2, -9, 27), new Point3D(-10, 0, 67));

            SetSign(-11, 13, 7, false);
        }
        
        public CasaMoga(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}