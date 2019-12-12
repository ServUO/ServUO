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

            AutoAddFixtures();
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

        public virtual void AutoAddFixtures()
        {
            var components = MultiData.GetComponents(ItemID);

            var teleporters = new Dictionary<int, List<MultiTileEntry>>();

            foreach (var entry in components.List.Where(e => e.m_Flags == 0))
            {
                // Telepoters
                if (entry.m_ItemID >= 0x181D && entry.m_ItemID <= 0x1828)
                {
                    if (teleporters.ContainsKey(entry.m_ItemID))
                    {
                        teleporters[entry.m_ItemID].Add(entry);
                    }
                    else
                    {
                        teleporters[entry.m_ItemID] = new List<MultiTileEntry>();
                        teleporters[entry.m_ItemID].Add(entry);
                    }
                }
                else
                {
                    ItemData data = TileData.ItemTable[entry.m_ItemID & TileData.MaxItemValue];

                    // door
                    if ((data.Flags & TileFlag.Door) != 0)
                    {
                        AddDoor(entry.m_ItemID, entry.m_OffsetX, entry.m_OffsetY, entry.m_OffsetZ);
                    }
                    else
                    {
                        Item st = new Static((int)entry.m_ItemID);

                        st.MoveToWorld(new Point3D(X + entry.m_OffsetX, Y + entry.m_OffsetY, entry.m_OffsetZ), Map);
                        AddFixture(st);
                    }
                }
            }

            foreach (var door in Doors.OfType<BaseDoor>())
            {
                foreach (var check in Doors.OfType<BaseDoor>().Where(d => d != door))
                {
                    if (door.InRange(check.Location, 1))
                    {
                        door.Link = check;
                        check.Link = door;
                    }
                }
            }

            foreach (var kvp in teleporters)
            {
                if (kvp.Value.Count > 2)
                {
                    Utility.WriteConsoleColor(ConsoleColor.Yellow, String.Format("Warning: More than 2 teleporters detected for {0}!", kvp.Key.ToString("X")));
                }
                else if (kvp.Value.Count <= 1)
                {
                    Utility.WriteConsoleColor(ConsoleColor.Yellow, String.Format("Warning: 1 or less teleporters detected for {0}!", kvp.Key.ToString("X")));

                    continue;
                }

                AddTeleporters(kvp.Key, new Point3D(kvp.Value[0].m_OffsetX, kvp.Value[0].m_OffsetY, kvp.Value[0].m_OffsetZ), new Point3D(kvp.Value[1].m_OffsetX, kvp.Value[1].m_OffsetY, kvp.Value[1].m_OffsetZ));
            }

            teleporters.Clear();
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
            SetSign(-11, 13, 7, false);
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

    public class RobinsRoost : BaseContestHouse
    {
        public RobinsRoost(Mobile owner)
            : base(ContestHouseType.Castle, 0x148A, owner, 3281, 28)
        {
            SetSign(-15, 16, 7, true);
        }

        public RobinsRoost(Serial serial)
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

    public class Camelot : BaseContestHouse
    {
        public Camelot(Mobile owner)
            : base(ContestHouseType.Castle, 0x148B, owner, 3281, 28)
        {
            SetSign(-15, 16, 7, false);
        }

        public Camelot(Serial serial)
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

    public class LacrimaeInCaelo : BaseContestHouse
    {
        public LacrimaeInCaelo(Mobile owner)
            : base(ContestHouseType.Castle, 0x148C, owner, 3281, 28)
        {
            SetSign(-15, 16, 7, false);
        }

        public LacrimaeInCaelo(Serial serial)
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

    public class OkinawaSweetDreamCastle : BaseContestHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[]
        {
            new Rectangle2D(-15, -15, 31, 31),
            new Rectangle2D(-14, 16, 6, 1),
            new Rectangle2D(-7, 16, 8, 1),
            new Rectangle2D(10, 16, 5, 1)
        };

        public override Rectangle2D[] Area { get { return AreaArray; } }

        public OkinawaSweetDreamCastle(Mobile owner)
            : base(ContestHouseType.Castle, 0x148D, owner, 3281, 28)
        {
            SetSign(-15, 16, 7, true);
        }

        public OkinawaSweetDreamCastle(Serial serial)
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

    public class TheSandstoneCastle : BaseContestHouse
    {
        public TheSandstoneCastle(Mobile owner)
            : base(ContestHouseType.Castle, 0x148E, owner, 3281, 28)
        {
            SetSign(-15, 16, 7, true);
        }

        public TheSandstoneCastle(Serial serial)
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

    public class GrimswindSisters : BaseContestHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[]
        {
            new Rectangle2D(-15, -15, 31, 31),
            new Rectangle2D(-14, 16, 9, 1),
            new Rectangle2D(-3, 16, 8, 1),
            new Rectangle2D(7, 16, 9, 1)
        };

        public override Rectangle2D[] Area { get { return AreaArray; } }

        public GrimswindSisters(Mobile owner)
            : base(ContestHouseType.Castle, 0x148F, owner, 3281, 28)
        {
            SetSign(-15, 16, 7, false);
        }

        public GrimswindSisters(Serial serial)
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
