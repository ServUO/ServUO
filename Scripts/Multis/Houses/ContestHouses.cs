using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public virtual int SignPostID => 9;

        public override Point3D BaseBanLocation => new Point3D(Components.Min.X, Components.Height - 1 - Components.Center.Y, 0);

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

            Static hanger = new Static(0xB9E);
            hanger.MoveToWorld(new Point3D(X + xOffset, Y + yOffset, Z + zOffset), Map);

            AddFixture(hanger);

            if (post)
            {
                Static signPost = new Static(SignPostID);
                signPost.MoveToWorld(new Point3D(X + xOffset, Y + yOffset - 1, Z + zOffset), Map);

                AddFixture(signPost);
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (Fixtures != null)
            {
                foreach (Item item in Fixtures)
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
                foreach (Item item in Fixtures)
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
                foreach (Item item in Fixtures)
                {
                    item.Map = Map;
                }
            }
        }

        public void AddTeleporters(int id, Point3D offset1, Point3D offset2)
        {
            HouseTeleporter tele1 = new HouseTeleporter(id);
            HouseTeleporter tele2 = new HouseTeleporter(id);

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
            MultiComponentList components = MultiData.GetComponents(ItemID);

            Dictionary<int, List<MultiTileEntry>> teleporters = new Dictionary<int, List<MultiTileEntry>>();

            foreach (MultiTileEntry entry in components.List.Where(e => e.m_Flags == 0))
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

            foreach (BaseDoor door in Doors.OfType<BaseDoor>())
            {
                foreach (BaseDoor check in Doors.OfType<BaseDoor>().Where(d => d != door))
                {
                    if (door.InRange(check.Location, 1))
                    {
                        door.Link = check;
                        check.Link = door;
                    }
                }
            }

            foreach (KeyValuePair<int, List<MultiTileEntry>> kvp in teleporters)
            {
                if (kvp.Value.Count > 2)
                {
                    Utility.WriteConsoleColor(ConsoleColor.Yellow, string.Format("Warning: More than 2 teleporters detected for {0}!", kvp.Key.ToString("X")));
                }
                else if (kvp.Value.Count <= 1)
                {
                    Utility.WriteConsoleColor(ConsoleColor.Yellow, string.Format("Warning: 1 or less teleporters detected for {0}!", kvp.Key.ToString("X")));

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
            writer.Write(0);//version

            writer.Write((int)HouseType);

            writer.Write(Fixtures != null ? Fixtures.Count : 0);

            if (Fixtures != null)
            {
                foreach (Item item in Fixtures)
                {
                    writer.Write(item);
                }
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            HouseType = (ContestHouseType)reader.ReadInt();

            int count = reader.ReadInt();

            for (int i = 0; i < count; i++)
            {
                Item item = reader.ReadItem();

                if (item != null)
                {
                    AddFixture(item);
                }
            }
        }
    }

    public class TrinsicKeep : BaseContestHouse
    {
        private static readonly Rectangle2D[] AreaArray = new Rectangle2D[]
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

        public override Rectangle2D[] Area => AreaArray;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class GothicRoseCastle : BaseContestHouse
    {
        public override Rectangle2D[] Area => AreaArray;

        private static readonly Rectangle2D[] AreaArray = new Rectangle2D[]
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
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
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
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
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
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
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
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class FeudalCastle : BaseContestHouse
    {
        private static readonly Rectangle2D[] AreaArray = new Rectangle2D[]
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

        public override Rectangle2D[] Area => AreaArray;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
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
            writer.Write(1);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class TraditionalKeep : BaseContestHouse
    {
        private static readonly Rectangle2D[] AreaArray = new Rectangle2D[]
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

        public override Rectangle2D[] Area => AreaArray;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
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
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
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
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class SandalwoodKeep : BaseContestHouse
    {
        public override int SignPostID => 353;

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
            writer.Write(1);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
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
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
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
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
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
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
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
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class OkinawaSweetDreamCastle : BaseContestHouse
    {
        private static readonly Rectangle2D[] AreaArray = new Rectangle2D[]
        {
            new Rectangle2D(-15, -15, 31, 31),
            new Rectangle2D(-14, 16, 6, 1),
            new Rectangle2D(-7, 16, 8, 1),
            new Rectangle2D(10, 16, 5, 1)
        };

        public override Rectangle2D[] Area => AreaArray;

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
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
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
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class GrimswindSisters : BaseContestHouse
    {
        private static readonly Rectangle2D[] AreaArray = new Rectangle2D[]
        {
            new Rectangle2D(-15, -15, 31, 31),
            new Rectangle2D(-14, 16, 9, 1),
            new Rectangle2D(-3, 16, 8, 1),
            new Rectangle2D(7, 16, 9, 1)
        };

        public override Rectangle2D[] Area => AreaArray;

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
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class FortressOfLestat : BaseContestHouse
    {
        public FortressOfLestat(Mobile owner)
            : base(ContestHouseType.Keep, 0x1490, owner, 2113, 18)
        {
            SetSign(-11, 13, 7, true);
        }

        public FortressOfLestat(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class CitadelOfTheFarEast : BaseContestHouse
    {
        public CitadelOfTheFarEast(Mobile owner)
            : base(ContestHouseType.Keep, 0x1491, owner, 2113, 18)
        {
            SetSign(-11, 13, 7, true);
        }

        public CitadelOfTheFarEast(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class KeepIncarcerated : BaseContestHouse
    {
        private static readonly Rectangle2D[] AreaArray = new Rectangle2D[]
        {
            new Rectangle2D(-11, -11, 23, 23), new Rectangle2D(-10, 13, 4, 1),
            new Rectangle2D(-3, 13, 9, 1), new Rectangle2D(9, 13, 4, 1)
        };

        public override Rectangle2D[] Area => AreaArray;

        public KeepIncarcerated(Mobile owner)
            : base(ContestHouseType.Keep, 0x1492, owner, 2113, 18)
        {
            SetSign(-11, 13, 7, true);
        }

        public KeepIncarcerated(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class SallyTreesRefurbishedKeep : BaseContestHouse
    {
        public SallyTreesRefurbishedKeep(Mobile owner)
            : base(ContestHouseType.Keep, 0x1493, owner, 2113, 18)
        {
            SetSign(-11, 13, 7, true);
        }

        public SallyTreesRefurbishedKeep(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class DesertRose : BaseContestHouse
    {
        private static readonly Rectangle2D[] AreaArray = new Rectangle2D[]
        {
            new Rectangle2D(-11, -11, 23, 23), new Rectangle2D(-7, 13, 11, 1),
        };

        public override Rectangle2D[] Area => AreaArray;

        public DesertRose(Mobile owner)
            : base(ContestHouseType.Keep, 0x1494, owner, 2113, 18)
        {
            SetSign(-11, 13, 7, true);
        }

        public DesertRose(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class TheCloversKeep : BaseContestHouse
    {
        private static readonly Rectangle2D[] AreaArray = new Rectangle2D[]
        {
            new Rectangle2D(-11, -11, 23, 23), new Rectangle2D(-8, 13, 2, 1),
            new Rectangle2D(-4, 13, 10, 1), new Rectangle2D(8, 13, 2, 1)
        };

        public override Rectangle2D[] Area => AreaArray;

        public TheCloversKeep(Mobile owner)
            : base(ContestHouseType.Keep, 0x1495, owner, 2113, 18)
        {
            SetSign(-11, 13, 7, false);
        }

        public TheCloversKeep(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class TheSorceresCastle : BaseContestHouse
    {
        public TheSorceresCastle(Mobile owner)
            : base(ContestHouseType.Castle, 0x1496, owner, 3281, 28)
        {
            SetSign(-15, 16, 7, true);
        }

        public TheSorceresCastle(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class TheCastleCascade : BaseContestHouse
    {
        public TheCastleCascade(Mobile owner)
            : base(ContestHouseType.Castle, 0x1497, owner, 3281, 28)
        {
            SetSign(-15, 16, 7, false);
        }

        public TheCastleCascade(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class TheHouseBuiltOnTheRuins : BaseContestHouse
    {
        private static readonly Rectangle2D[] AreaArray = new Rectangle2D[]
        {
            new Rectangle2D(-15, -15, 31, 31),
            new Rectangle2D(-13, 16, 3, 1),
            new Rectangle2D(-7, 16, 3, 1),
            new Rectangle2D(2, 16, 3, 1),
             new Rectangle2D(10, 16, 3, 1)
        };

        public override Rectangle2D[] Area => AreaArray;

        public TheHouseBuiltOnTheRuins(Mobile owner)
            : base(ContestHouseType.Castle, 0x1498, owner, 3281, 28)
        {
            SetSign(-15, 16, 7, true);
        }

        public TheHouseBuiltOnTheRuins(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class TheSandstoneFortressOfGrand : BaseContestHouse
    {
        public TheSandstoneFortressOfGrand(Mobile owner)
            : base(ContestHouseType.Castle, 0x1499, owner, 3281, 28)
        {
            SetSign(-15, 16, 7, false);
        }

        public TheSandstoneFortressOfGrand(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class TheDragonstoneCastle : BaseContestHouse
    {
        public TheDragonstoneCastle(Mobile owner)
            : base(ContestHouseType.Castle, 0x149A, owner, 3281, 28)
        {
            SetSign(-15, 16, 7, true);
        }

        public TheDragonstoneCastle(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class TheTerraceGardens : BaseContestHouse
    {
        private static readonly Rectangle2D[] AreaArray = new Rectangle2D[]
        {
            new Rectangle2D(-15, -15, 31, 31),
            new Rectangle2D(-11, 16, 4, 1),
            new Rectangle2D(-3, 16, 8, 1),
            new Rectangle2D(9, 16, 3, 1),
        };

        public override Rectangle2D[] Area => AreaArray;

        public TheTerraceGardens(Mobile owner)
            : base(ContestHouseType.Castle, 0x149B, owner, 3281, 28)
        {
            SetSign(-15, 16, 7, false);
        }

        public TheTerraceGardens(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class TheKeepCalmAndCarryOnKeep : BaseContestHouse
    {
        public TheKeepCalmAndCarryOnKeep(Mobile owner)
            : base(ContestHouseType.Keep, 0x149C, owner, 2113, 18)
        {
            SetSign(-11, 13, 7, true);
        }

        public TheKeepCalmAndCarryOnKeep(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class TheRavenloftKeep : BaseContestHouse
    {
        public TheRavenloftKeep(Mobile owner)
            : base(ContestHouseType.Keep, 0x149D, owner, 2113, 18)
        {
            SetSign(-11, 13, 7, false);
        }

        public TheRavenloftKeep(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class TheQueensRetreatKeep : BaseContestHouse
    {
        public TheQueensRetreatKeep(Mobile owner)
            : base(ContestHouseType.Keep, 0x149E, owner, 2113, 18)
        {
            SetSign(-11, 13, 7, true);
        }

        public TheQueensRetreatKeep(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
