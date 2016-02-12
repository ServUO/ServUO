using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Server.Events;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Multis;

namespace Server.Multis
{
    public enum FoundationType
    {
        Stone,
        DarkWood,
        LightWood,
        Dungeon,
        Brick,
        ElvenStone,
        ElvenWood,
        ElvenSimple,
        ElvenPlain,
        Crystal,
        Shadow,
        Jungle,
        Shadowguard,
        GargishGreenMarble,
        GargishTwoToneStone
    }

    [TypeAlias("Server.Multis.HouseFoundation")]
    public class HouseFoundation : BaseHouse
    {
        private DesignState m_Current; // State which is currently visible.
        private DesignState m_Design; // State of current design.
        private DesignState m_Backup; // State at last user backup.
        private Item m_SignHanger; // Item hanging the sign.
        private Item m_Signpost; // Item supporting the hanger.
        private int m_SignpostGraphic; // ItemID number of the chosen signpost.
        private int m_LastRevision; // Latest revision number.
        private List<Item> m_Fixtures; // List of fixtures (teleporters and doors) associated with this house.
        private FoundationType m_Type; // Graphic type of this foundation.
        private Mobile m_Customizer; // Who is currently customizing this -or- null if not customizing.

        public FoundationType Type { get { return m_Type; } set { m_Type = value; } }
        public int LastRevision { get { return m_LastRevision; } set { m_LastRevision = value; } }
        public List<Item> Fixtures { get { return m_Fixtures; } }
        public Item SignHanger { get { return m_SignHanger; } }
        public Item Signpost { get { return m_Signpost; } }
        public int SignpostGraphic { get { return m_SignpostGraphic; } set { m_SignpostGraphic = value; } }
        public Mobile Customizer { get { return m_Customizer; } set { m_Customizer = value; } }

        public override bool IsActive { get { return Customizer == null; } }

        public bool IsFixture(Item item)
        {
            return (m_Fixtures != null && m_Fixtures.Contains(item));
        }

        public override MultiComponentList Components
        {
            get
            {
                if (m_Current == null)
                    SetInitialState();

                return m_Current.Components;
            }
        }

        public override int GetMaxUpdateRange()
        {
            return Core.GlobalMaxUpdateRange;
        }

        public override int GetUpdateRange(Mobile m)
        {
            int w = this.CurrentState.Components.Width;
            int h = this.CurrentState.Components.Height - 1;
            int v = Core.GlobalUpdateRange + ((w > h ? w : h) / 2);

            return Math.Max(Core.GlobalUpdateRange, Math.Min(Core.GlobalMaxUpdateRange, v));
        }

        public DesignState CurrentState
        {
            get
            {
                if (m_Current == null)
                {
                    SetInitialState();
                }
                return m_Current;
            }
            set { m_Current = value; }
        }

        public DesignState DesignState
        {
            get
            {
                if (m_Design == null)
                {
                    SetInitialState();
                }
                return m_Design;
            }
            set { m_Design = value; }
        }

        public DesignState BackupState
        {
            get
            {
                if (m_Backup == null)
                {
                    SetInitialState();
                }
                return m_Backup;
            }
            set { m_Backup = value; }
        }

        public void SetInitialState()
        {
            // This is a new house, it has not yet loaded a design state
            m_Current = new DesignState(this, GetEmptyFoundation());
            m_Design = new DesignState(m_Current);
            m_Backup = new DesignState(m_Current);
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_SignHanger != null)
                m_SignHanger.Delete();

            if (m_Signpost != null)
                m_Signpost.Delete();

            if (m_Fixtures == null)
                return;

            for (int i = 0; i < m_Fixtures.Count; ++i)
            {
                Item item = (Item)m_Fixtures[i];

                if (item != null)
                    item.Delete();
            }

            m_Fixtures.Clear();
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            base.OnLocationChange(oldLocation);

            int x = Location.X - oldLocation.X;
            int y = Location.Y - oldLocation.Y;
            int z = Location.Z - oldLocation.Z;

            if (m_SignHanger != null)
                m_SignHanger.MoveToWorld(new Point3D(m_SignHanger.X + x, m_SignHanger.Y + y, m_SignHanger.Z + z), Map);

            if (m_Signpost != null)
                m_Signpost.MoveToWorld(new Point3D(m_Signpost.X + x, m_Signpost.Y + y, m_Signpost.Z + z), Map);

            if (m_Fixtures == null)
                return;

            for (int i = 0; i < m_Fixtures.Count; ++i)
            {
                Item item = (Item)m_Fixtures[i];

                if (Doors.Contains(item))
                    continue;

                item.MoveToWorld(new Point3D(item.X + x, item.Y + y, item.Z + z), Map);
            }
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            if (m_SignHanger != null)
                m_SignHanger.Map = this.Map;

            if (m_Signpost != null)
                m_Signpost.Map = this.Map;

            if (m_Fixtures == null)
                return;

            for (int i = 0; i < m_Fixtures.Count; ++i)
                ((Item)m_Fixtures[i]).Map = this.Map;
        }

        public void ClearFixtures(Mobile from)
        {
            if (m_Fixtures == null)
                return;

            RemoveKeys(from);

            for (int i = 0; i < m_Fixtures.Count; ++i)
            {
                ((Item)m_Fixtures[i]).Delete();
                Doors.Remove(m_Fixtures[i]);
            }

            m_Fixtures.Clear();
        }

        private static int[] m_SADoorIds = new int[]
			{
				0x4097, // Gargish Carved Green Door
				0x4108, // Gargish Brown Door
				0x41BE, // Sun Door
				0x41CB, // Gargish Grey Door
				0x436E, // Gargish Set Door
				0x46D9, // Ruined Door
				0x4D1E, // Gargish Blue Door
				0x50C8, // Gargish Red Doors
				0x513E  // Gargish Prison Door
			};

        public void AddFixtures(Mobile from, MultiTileEntry[] list)
        {
            if (m_Fixtures == null)
                m_Fixtures = new List<Item>();

            uint keyValue = 0;

            for (int i = 0; i < list.Length; ++i)
            {
                MultiTileEntry mte = list[i];
                int itemID = mte.m_ItemID & TileData.MaxItemValue;

                if (itemID >= 0x181D && itemID < 0x1829)
                {
                    HouseTeleporter tp = new HouseTeleporter(itemID);

                    AddFixture(tp, mte);
                }
                else
                {
                    BaseDoor door = null;

                    if (itemID >= 0x675 && itemID < 0x6F5)
                    {
                        int type = (itemID - 0x675) / 16;
                        DoorFacing facing = (DoorFacing)(((itemID - 0x675) / 2) % 8);

                        switch (type)
                        {
                            case 0:
                                door = new GenericHouseDoor(facing, 0x675, 0xEC, 0xF3);
                                break;
                            case 1:
                                door = new GenericHouseDoor(facing, 0x685, 0xEC, 0xF3);
                                break;
                            case 2:
                                door = new GenericHouseDoor(facing, 0x695, 0xEB, 0xF2);
                                break;
                            case 3:
                                door = new GenericHouseDoor(facing, 0x6A5, 0xEA, 0xF1);
                                break;
                            case 4:
                                door = new GenericHouseDoor(facing, 0x6B5, 0xEA, 0xF1);
                                break;
                            case 5:
                                door = new GenericHouseDoor(facing, 0x6C5, 0xEC, 0xF3);
                                break;
                            case 6:
                                door = new GenericHouseDoor(facing, 0x6D5, 0xEA, 0xF1);
                                break;
                            case 7:
                                door = new GenericHouseDoor(facing, 0x6E5, 0xEA, 0xF1);
                                break;
                        }
                    }
                    else if (itemID >= 0x314 && itemID < 0x364)
                    {
                        int type = (itemID - 0x314) / 16;
                        DoorFacing facing = (DoorFacing)(((itemID - 0x314) / 2) % 8);

                        switch (type)
                        {
                            case 0:
                                door = new GenericHouseDoor(facing, 0x314, 0xED, 0xF4);
                                break;
                            case 1:
                                door = new GenericHouseDoor(facing, 0x324, 0xED, 0xF4);
                                break;
                            case 2:
                                door = new GenericHouseDoor(facing, 0x334, 0xED, 0xF4);
                                break;
                            case 3:
                                door = new GenericHouseDoor(facing, 0x344, 0xED, 0xF4);
                                break;
                            case 4:
                                door = new GenericHouseDoor(facing, 0x354, 0xED, 0xF4);
                                break;
                        }
                    }
                    else if (itemID >= 0x824 && itemID < 0x834)
                    {
                        DoorFacing facing = (DoorFacing)(((itemID - 0x824) / 2) % 8);
                        door = new GenericHouseDoor(facing, 0x824, 0xEC, 0xF3);
                    }
                    else if (itemID >= 0x839 && itemID < 0x849)
                    {
                        DoorFacing facing = (DoorFacing)(((itemID - 0x839) / 2) % 8);
                        door = new GenericHouseDoor(facing, 0x839, 0xEB, 0xF2);
                    }
                    else if (itemID >= 0x84C && itemID < 0x85C)
                    {
                        DoorFacing facing = (DoorFacing)(((itemID - 0x84C) / 2) % 8);
                        door = new GenericHouseDoor(facing, 0x84C, 0xEC, 0xF3);
                    }
                    else if (itemID >= 0x866 && itemID < 0x876)
                    {
                        DoorFacing facing = (DoorFacing)(((itemID - 0x866) / 2) % 8);
                        door = new GenericHouseDoor(facing, 0x866, 0xEB, 0xF2);
                    }
                    else if (itemID >= 0xE8 && itemID < 0xF8)
                    {
                        DoorFacing facing = (DoorFacing)(((itemID - 0xE8) / 2) % 8);
                        door = new GenericHouseDoor(facing, 0xE8, 0xED, 0xF4);
                    }
                    else if (itemID >= 0x1FED && itemID < 0x1FFD)
                    {
                        DoorFacing facing = (DoorFacing)(((itemID - 0x1FED) / 2) % 8);
                        door = new GenericHouseDoor(facing, 0x1FED, 0xEC, 0xF3);
                    }
                    else if (itemID >= 0x241F && itemID < 0x2421)
                    {
                        //DoorFacing facing = (DoorFacing)(((itemID - 0x241F) / 2) % 8);
                        door = new GenericHouseDoor(DoorFacing.NorthCCW, 0x2415, -1, -1);
                    }
                    else if (itemID >= 0x2423 && itemID < 0x2425)
                    {
                        //DoorFacing facing = (DoorFacing)(((itemID - 0x241F) / 2) % 8);
                        //This one and the above one are 'special' cases, ie: OSI had the ItemID pattern discombobulated for these
                        door = new GenericHouseDoor(DoorFacing.WestCW, 0x2423, -1, -1);
                    }
                    else if (itemID >= 0x2A05 && itemID < 0x2A1D)
                    {
                        DoorFacing facing = (DoorFacing)((((itemID - 0x2A05) / 2) % 4) + 8);

                        int sound = (itemID >= 0x2A0D && itemID < 0x2a15) ? 0x539 : -1;

                        door = new GenericHouseDoor(facing, 0x29F5 + (8 * ((itemID - 0x2A05) / 8)), sound, sound);
                    }

                        // Mondain's Legacy
                    else if (itemID == 0x31AE)
                        door = new GenericHouseDoor(DoorFacing.WestCCW, 0x31AE - (2 * (int)DoorFacing.WestCCW), 0xEA, 0xF1);
                    else if (itemID == 0x31AC)
                        door = new GenericHouseDoor(DoorFacing.EastCW, 0x31AC - (2 * (int)DoorFacing.EastCW), 0xEA, 0xF1);
                    else if (itemID == 0x2D48)
                        door = new GenericHouseDoor(DoorFacing.SouthCCW, 0x2D48 - (2 * (int)DoorFacing.SouthCCW), 0xEA, 0xF1);
                    else if (itemID == 0x2D46)
                        door = new GenericHouseDoor(DoorFacing.NorthCW, 0x2D46 - (2 * (int)DoorFacing.NorthCW), 0xEA, 0xF1);

                    else if (itemID == 0x2D65)
                        door = new GenericHouseDoor(DoorFacing.WestCCW, 0x2D65 - (2 * (int)DoorFacing.WestCCW), 0xEA, 0xF1);
                    else if (itemID == 0x31A0)
                        door = new GenericHouseDoor(DoorFacing.EastCW, 0x31A0 - (2 * (int)DoorFacing.EastCW), 0xEA, 0xF1);
                    else if (itemID == 0x2D63)
                        door = new GenericHouseDoor(DoorFacing.SouthCCW, 0x2D63 - (2 * (int)DoorFacing.SouthCCW), 0xEA, 0xF1);
                    else if (itemID == 0x31A2)
                        door = new GenericHouseDoor(DoorFacing.NorthCW, 0x31A2 - (2 * (int)DoorFacing.NorthCW), 0xEA, 0xF1);

                    else if (itemID == 0x2D69)
                        door = new GenericHouseDoor(DoorFacing.WestCCW, 0x2D69 - (2 * (int)DoorFacing.WestCCW), 0xEA, 0xF1);
                    else if (itemID == 0x31A4)
                        door = new GenericHouseDoor(DoorFacing.EastCW, 0x31A4 - (2 * (int)DoorFacing.EastCW), 0xEA, 0xF1);
                    else if (itemID == 0x2D67)
                        door = new GenericHouseDoor(DoorFacing.SouthCCW, 0x2D67 - (2 * (int)DoorFacing.SouthCCW), 0xEA, 0xF1);
                    else if (itemID == 0x31A6)
                        door = new GenericHouseDoor(DoorFacing.NorthCW, 0x31A6 - (2 * (int)DoorFacing.NorthCW), 0xEA, 0xF1);

                    else if (itemID == 0x2D6D)
                        door = new GenericHouseDoor(DoorFacing.WestCCW, 0x2D6D - (2 * (int)DoorFacing.WestCCW), 0xEA, 0xF1);
                    else if (itemID == 0x31AA)
                        door = new GenericHouseDoor(DoorFacing.EastCW, 0x31AA - (2 * (int)DoorFacing.EastCW), 0xEA, 0xF1);
                    else if (itemID == 0x2D6B)
                        door = new GenericHouseDoor(DoorFacing.SouthCCW, 0x2D6B - (2 * (int)DoorFacing.SouthCCW), 0xEA, 0xF1);
                    else if (itemID == 0x31A8)
                        door = new GenericHouseDoor(DoorFacing.NorthCW, 0x31A8 - (2 * (int)DoorFacing.NorthCW), 0xEA, 0xF1);

                    else if (itemID == 0x2FE4)
                        door = new GenericHouseDoor(DoorFacing.WestCCW, 0x2FE4 - (2 * (int)DoorFacing.WestCCW), 0xEA, 0xF1);
                    else if (itemID == 0x319C)
                        door = new GenericHouseDoor(DoorFacing.EastCW, 0x319C - (2 * (int)DoorFacing.EastCW), 0xEA, 0xF1);
                    else if (itemID == 0x2FE2)
                        door = new GenericHouseDoor(DoorFacing.SouthCCW, 0x2FE2 - (2 * (int)DoorFacing.SouthCCW), 0xEA, 0xF1);
                    else if (itemID == 0x319E)
                        door = new GenericHouseDoor(DoorFacing.NorthCW, 0x319E - (2 * (int)DoorFacing.NorthCW), 0xEA, 0xF1);

                        // 9th Anniversary
                    else if (itemID >= 0x367B && itemID < 0x369B)
                    {
                        int type = (itemID - 0x367B) / 16;
                        DoorFacing facing = (DoorFacing)(((itemID - 0x367B) / 2) % 8);

                        switch (type)
                        {
                            case 0:
                                door = new GenericHouseDoor(facing, 0x367B, 0xED, 0xF4);
                                break;
                            case 1:
                                door = new GenericHouseDoor(facing, 0x368B, 0xED, 0xF4);
                                break;
                        }
                    }

                        // Stygian Abyss
                    else if (itemID >= 0x409B && itemID <= 0x5148)
                    {
                        int baseId = -1;

                        for (int j = m_SADoorIds.Length - 1; j >= 0; j--)
                        {
                            baseId = m_SADoorIds[j];

                            if (itemID >= baseId)
                                break;
                        }

                        int facing = ((itemID - baseId) / 2) % 8;

                        if (itemID >= 0x50C8 && itemID <= 0x50D6) // special case
                        {
                            facing = (facing % 2) + (((facing / 2) % 2 != 0) ? 6 : 2);
                        }
                        else
                        {
                            if ((facing / 2) % 2 == 0)
                                facing += 2;
                        }

                        door = new GenericHouseDoor((DoorFacing)facing, itemID - (2 * facing), 0xEA, 0xF1);
                    }

                    if (door != null)
                    {
                        if (keyValue == 0)
                            keyValue = CreateKeys(from);

                        door.Locked = true;
                        door.KeyValue = keyValue;

                        AddDoor(door, mte.m_OffsetX, mte.m_OffsetY, mte.m_OffsetZ);
                        m_Fixtures.Add(door);
                    }
                }
            }

            for (int i = 0; i < m_Fixtures.Count; ++i)
            {
                Item fixture = (Item)m_Fixtures[i];

                if (fixture is HouseTeleporter)
                {
                    HouseTeleporter tp = (HouseTeleporter)fixture;

                    for (int j = 1; j <= m_Fixtures.Count; ++j)
                    {
                        HouseTeleporter check = m_Fixtures[(i + j) % m_Fixtures.Count] as HouseTeleporter;

                        if (check != null && check.ItemID == tp.ItemID)
                        {
                            tp.Target = check;
                            break;
                        }
                    }
                }
                else if (fixture is BaseHouseDoor)
                {
                    BaseHouseDoor door = (BaseHouseDoor)fixture;

                    if (door.Link != null)
                        continue;

                    DoorFacing linkFacing;
                    int xOffset, yOffset;

                    switch (door.Facing)
                    {
                        default:
                        case DoorFacing.WestCW:
                            linkFacing = DoorFacing.EastCCW;
                            xOffset = 1;
                            yOffset = 0;
                            break;
                        case DoorFacing.EastCCW:
                            linkFacing = DoorFacing.WestCW;
                            xOffset = -1;
                            yOffset = 0;
                            break;
                        case DoorFacing.WestCCW:
                            linkFacing = DoorFacing.EastCW;
                            xOffset = 1;
                            yOffset = 0;
                            break;
                        case DoorFacing.EastCW:
                            linkFacing = DoorFacing.WestCCW;
                            xOffset = -1;
                            yOffset = 0;
                            break;
                        case DoorFacing.SouthCW:
                            linkFacing = DoorFacing.NorthCCW;
                            xOffset = 0;
                            yOffset = -1;
                            break;
                        case DoorFacing.NorthCCW:
                            linkFacing = DoorFacing.SouthCW;
                            xOffset = 0;
                            yOffset = 1;
                            break;
                        case DoorFacing.SouthCCW:
                            linkFacing = DoorFacing.NorthCW;
                            xOffset = 0;
                            yOffset = -1;
                            break;
                        case DoorFacing.NorthCW:
                            linkFacing = DoorFacing.SouthCCW;
                            xOffset = 0;
                            yOffset = 1;
                            break;
                        case DoorFacing.SouthSW:
                            linkFacing = DoorFacing.SouthSE;
                            xOffset = 1;
                            yOffset = 0;
                            break;
                        case DoorFacing.SouthSE:
                            linkFacing = DoorFacing.SouthSW;
                            xOffset = -1;
                            yOffset = 0;
                            break;
                        case DoorFacing.WestSN:
                            linkFacing = DoorFacing.WestSS;
                            xOffset = 0;
                            yOffset = 1;
                            break;
                        case DoorFacing.WestSS:
                            linkFacing = DoorFacing.WestSN;
                            xOffset = 0;
                            yOffset = -1;
                            break;
                    }

                    for (int j = i + 1; j < m_Fixtures.Count; ++j)
                    {
                        BaseHouseDoor check = m_Fixtures[j] as BaseHouseDoor;

                        if (check != null && check.Link == null && check.Facing == linkFacing && (check.X - door.X) == xOffset && (check.Y - door.Y) == yOffset && (check.Z == door.Z))
                        {
                            check.Link = door;
                            door.Link = check;
                            break;
                        }
                    }
                }
            }
        }

        public void AddFixture(Item item, MultiTileEntry mte)
        {
            m_Fixtures.Add(item);
            item.MoveToWorld(new Point3D(X + mte.m_OffsetX, Y + mte.m_OffsetY, Z + mte.m_OffsetZ), Map);
        }

        public static void GetFoundationGraphics(FoundationType type, out int east, out int south, out int post, out int corner)
        {
            switch (type)
            {
                default:
                case FoundationType.DarkWood: corner = 0x0014; east = 0x0015; south = 0x0016; post = 0x0017; break;
                case FoundationType.LightWood: corner = 0x00BD; east = 0x00BE; south = 0x00BF; post = 0x00C0; break;
                case FoundationType.Dungeon: corner = 0x02FD; east = 0x02FF; south = 0x02FE; post = 0x0300; break;
                case FoundationType.Brick: corner = 0x0041; east = 0x0043; south = 0x0042; post = 0x0044; break;
                case FoundationType.Stone: corner = 0x0065; east = 0x0064; south = 0x0063; post = 0x0066; break;
                case FoundationType.ElvenStone: corner = 0x2DF7; east = 0x2DF9; south = 0x2DFA; post = 0x2DF8; break;
                case FoundationType.ElvenWood: corner = 0x2DFB; east = 0x2DFD; south = 0x2DFE; post = 0x2DFC; break;
                case FoundationType.ElvenSimple: corner = 0x2BC7; east = 0x2CEF; south = 0x2CF0; post = 0x2BC8; break;
                case FoundationType.ElvenPlain: corner = 0x2DC3; east = 0x2DCF; south = 0x2DD0; post = 0x2DC6; break;
                case FoundationType.Crystal: corner = 0x3672; east = 0x3671; south = 0x3670; post = 0x3673; break;
                case FoundationType.Shadow: corner = 0x3676; east = 0x3675; south = 0x3674; post = 0x3677; break;
                case FoundationType.Jungle: corner = 0x9ABE; east = 0x9AC0; south = 0x9ABF; post = 0x9AC1; break;
                case FoundationType.Shadowguard: corner = 0x9BD0; east = 0x9BD2; south = 0x9BD1; post = 0x9BD3; break;
                case FoundationType.GargishGreenMarble: corner = 0x41A6; east = 0x41A8; south = 0x41A7; post = 0x419E; break;
                case FoundationType.GargishTwoToneStone: corner = 0x415C; east = 0x4166; south = 0x4167; post = 0x415F; break;
            }
        }

        public static void ApplyFoundation(FoundationType type, MultiComponentList mcl)
        {
            int east, south, post, corner;

            GetFoundationGraphics(type, out east, out south, out post, out corner);

            int xCenter = mcl.Center.X;
            int yCenter = mcl.Center.Y;

            mcl.Add(post, 0 - xCenter, 0 - yCenter, 0);
            mcl.Add(corner, mcl.Width - 1 - xCenter, mcl.Height - 2 - yCenter, 0);

            for (int x = 1; x < mcl.Width; ++x)
            {
                mcl.Add(south, x - xCenter, 0 - yCenter, 0);

                if (x < mcl.Width - 1)
                    mcl.Add(south, x - xCenter, mcl.Height - 2 - yCenter, 0);
            }

            for (int y = 1; y < mcl.Height - 1; ++y)
            {
                mcl.Add(east, 0 - xCenter, y - yCenter, 0);

                if (y < mcl.Height - 2)
                    mcl.Add(east, mcl.Width - 1 - xCenter, y - yCenter, 0);
            }
        }

        public static void AddStairsTo(ref MultiComponentList mcl)
        {
            // copy the original..
            mcl = new MultiComponentList(mcl);

            mcl.Resize(mcl.Width, mcl.Height + 1);

            int xCenter = mcl.Center.X;
            int yCenter = mcl.Center.Y;
            int y = mcl.Height - 1;

            for (int x = 0; x < mcl.Width; ++x)
                mcl.Add(0x63, x - xCenter, y - yCenter, 0);
        }

        public MultiComponentList GetEmptyFoundation()
        {
            // Copy original foundation layout
            MultiComponentList mcl = new MultiComponentList(MultiData.GetComponents(ItemID));

            mcl.Resize(mcl.Width, mcl.Height + 1);

            int xCenter = mcl.Center.X;
            int yCenter = mcl.Center.Y;
            int y = mcl.Height - 1;

            ApplyFoundation(m_Type, mcl);

            for (int x = 1; x < mcl.Width; ++x)
                mcl.Add(0x751, x - xCenter, y - yCenter, 0);

            return mcl;
        }

        public override Rectangle2D[] Area
        {
            get
            {
                MultiComponentList mcl = Components;

                return new Rectangle2D[] { new Rectangle2D(mcl.Min.X, mcl.Min.Y, mcl.Width, mcl.Height) };
            }
        }

        public override Point3D BaseBanLocation { get { return new Point3D(Components.Min.X, Components.Height - 1 - Components.Center.Y, 0); } }

        public void CheckSignpost()
        {
            MultiComponentList mcl = this.Components;

            int x = mcl.Min.X;
            int y = mcl.Height - 2 - mcl.Center.Y;

            if (CheckWall(mcl, x, y))
            {
                if (m_Signpost != null)
                    m_Signpost.Delete();

                m_Signpost = null;
            }
            else if (m_Signpost == null)
            {
                m_Signpost = new Static(m_SignpostGraphic);
                m_Signpost.MoveToWorld(new Point3D(X + x, Y + y, Z + 7), Map);
            }
            else
            {
                m_Signpost.ItemID = m_SignpostGraphic;
                m_Signpost.MoveToWorld(new Point3D(X + x, Y + y, Z + 7), Map);
            }
        }

        public bool CheckWall(MultiComponentList mcl, int x, int y)
        {
            x += mcl.Center.X;
            y += mcl.Center.Y;

            if (x >= 0 && x < mcl.Width && y >= 0 && y < mcl.Height)
            {
                StaticTile[] tiles = mcl.Tiles[x][y];

                for (int i = 0; i < tiles.Length; ++i)
                {
                    StaticTile tile = tiles[i];

                    if (tile.Z == 7 && tile.Height == 20)
                        return true;
                }
            }

            return false;
        }

        public HouseFoundation(Mobile owner, int multiID, int maxLockdowns, int maxSecures)
            : base(multiID, owner, maxLockdowns, maxSecures)
        {
            m_SignpostGraphic = 9;

            m_Fixtures = new List<Item>();

            int x = Components.Min.X;
            int y = Components.Height - 1 - Components.Center.Y;

            m_SignHanger = new Static(0xB98);
            m_SignHanger.MoveToWorld(new Point3D(X + x, Y + y, Z + 7), Map);

            CheckSignpost();

            SetSign(x, y, 7);

            NetState ns = owner.NetState;

            if (ns.IsKRClient)
            {
                RefreshHouse(owner);
                //Timer.DelayCall(TimeSpan.Zero, ECEndConfirmCommit, owner); //ZeroDowned Owned this Temp Fix. EverOne please email him and thank him
            }
        }

        public HouseFoundation(Serial serial)
            : base(serial)
        {
        }

        public void BeginCustomize(Mobile m)
        {
            if (!m.CheckAlive())
                return;

            RelocateEntities();

            foreach (Item item in GetItems())
            {
                item.Location = BanLocation;
            }

            foreach (Mobile mobile in GetMobiles())
            {
                if (mobile != m)
                    mobile.Location = BanLocation;
            }

            DesignContext.Add(m, this);
            m.Send(new BeginHouseCustomization(this));

            NetState ns = m.NetState;
            if (ns != null)
                SendInfoTo(ns);

            DesignState.SendDetailedInfoTo(ns);
        }

        public override void SendInfoTo(NetState state, bool sendOplPacket)
        {
            base.SendInfoTo(state, sendOplPacket);

            DesignContext context = DesignContext.Find(state.Mobile);
            DesignState stateToSend;

            if (context != null && context.Foundation == this)
                stateToSend = DesignState;
            else
                stateToSend = CurrentState;

            stateToSend.SendGeneralInfoTo(state);
        }

        public override void Serialize(GenericWriter writer)
        {
            writer.Write((int)5); // version

            writer.Write(m_Signpost);
            writer.Write((int)m_SignpostGraphic);

            writer.Write((int)m_Type);

            writer.Write(m_SignHanger);

            writer.Write((int)m_LastRevision);
            writer.WriteItemList(m_Fixtures, true);

            CurrentState.Serialize(writer);
            DesignState.Serialize(writer);
            BackupState.Serialize(writer);

            base.Serialize(writer);
        }

        private int m_DefaultPrice;

        public override int DefaultPrice { get { return m_DefaultPrice; } }

        public override void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            switch (version)
            {
                case 5:
                case 4:
                    {
                        m_Signpost = reader.ReadItem();
                        m_SignpostGraphic = reader.ReadInt();

                        goto case 3;
                    }
                case 3:
                    {
                        m_Type = (FoundationType)reader.ReadInt();

                        goto case 2;
                    }
                case 2:
                    {
                        m_SignHanger = reader.ReadItem();

                        goto case 1;
                    }
                case 1:
                    {
                        if (version < 5)
                            m_DefaultPrice = reader.ReadInt();

                        goto case 0;
                    }
                case 0:
                    {
                        if (version < 3)
                            m_Type = FoundationType.Stone;

                        if (version < 4)
                            m_SignpostGraphic = 9;

                        m_LastRevision = reader.ReadInt();
                        m_Fixtures = reader.ReadStrongItemList();

                        m_Current = new DesignState(this, reader);
                        m_Design = new DesignState(this, reader);
                        m_Backup = new DesignState(this, reader);

                        break;
                    }
            }

            base.Deserialize(reader);
        }

        public bool IsHiddenToCustomizer(Item item)
        {
            return (item == m_Signpost || (m_Fixtures != null && m_Fixtures.Contains(item)));
        }

        public static void Initialize()
        {
            PacketHandlers.RegisterExtended(0x1E, true, new OnPacketReceive(QueryDesignDetails));

            PacketHandlers.RegisterEncoded(0x02, true, new OnEncodedPacketReceive(Designer_Backup));
            PacketHandlers.RegisterEncoded(0x03, true, new OnEncodedPacketReceive(Designer_Restore));
            PacketHandlers.RegisterEncoded(0x04, true, new OnEncodedPacketReceive(Designer_Commit));
            PacketHandlers.RegisterEncoded(0x05, true, new OnEncodedPacketReceive(Designer_Delete));
            PacketHandlers.RegisterEncoded(0x06, true, new OnEncodedPacketReceive(Designer_Build));
            PacketHandlers.RegisterEncoded(0x0C, true, new OnEncodedPacketReceive(Designer_Close));
            PacketHandlers.RegisterEncoded(0x0D, true, new OnEncodedPacketReceive(Designer_Stairs));
            PacketHandlers.RegisterEncoded(0x0E, true, new OnEncodedPacketReceive(Designer_Sync));
            PacketHandlers.RegisterEncoded(0x10, true, new OnEncodedPacketReceive(Designer_Clear));
            PacketHandlers.RegisterEncoded(0x12, true, new OnEncodedPacketReceive(Designer_Level));

            PacketHandlers.RegisterEncoded(0x13, true, new OnEncodedPacketReceive(Designer_Roof));
            PacketHandlers.RegisterEncoded(0x14, true, new OnEncodedPacketReceive(Designer_RoofDelete));

            PacketHandlers.RegisterEncoded(0x1A, true, new OnEncodedPacketReceive(Designer_Revert));

            EventSink.Speech += new SpeechEventHandler(EventSink_Speech);
        }

        private static void EventSink_Speech(SpeechEventArgs e)
        {
            if (DesignContext.Find(e.Mobile) != null)
            {
                e.Mobile.SendLocalizedMessage(1061925); // You cannot speak while customizing your house.
                e.Blocked = true;
            }
        }

        public static void Designer_Sync(NetState state, IEntity e, EncodedReader pvSrc)
        {
            Mobile from = state.Mobile;
            DesignContext context = DesignContext.Find(from);

            if (context != null)
            {
                /* Client requested state synchronization
                 *  - Resend full house state
                 */

                DesignState design = context.Foundation.DesignState;

                // Resend full house state
                design.SendDetailedInfoTo(state);
            }
        }

        public static void Designer_Clear(NetState state, IEntity e, EncodedReader pvSrc)
        {
            Mobile from = state.Mobile;
            DesignContext context = DesignContext.Find(from);

            if (context != null)
            {
                /* Client chose to clear the design
                 *  - Restore empty foundation
                 *     - Construct new design state from empty foundation
                 *     - Assign constructed state to foundation
                 *  - Update revision
                 *  - Update client with new state
                 */

                // Restore empty foundation : Construct new design state from empty foundation
                DesignState newDesign = new DesignState(context.Foundation, context.Foundation.GetEmptyFoundation());

                // Restore empty foundation : Assign constructed state to foundation
                context.Foundation.DesignState = newDesign;

                // Update revision
                newDesign.OnRevised();

                // Update client with new state
                context.Foundation.SendInfoTo(state);
                newDesign.SendDetailedInfoTo(state);
            }
        }

        public static void Designer_Restore(NetState state, IEntity e, EncodedReader pvSrc)
        {
            Mobile from = state.Mobile;
            DesignContext context = DesignContext.Find(from);

            if (context != null)
            {
                /* Client chose to restore design to the last backup state
                 *  - Restore backup
                 *     - Construct new design state from backup state
                 *     - Assign constructed state to foundation
                 *  - Update revision
                 *  - Update client with new state
                 */

                // Restore backup : Construct new design state from backup state
                DesignState backupDesign = new DesignState(context.Foundation.BackupState);

                // Restore backup : Assign constructed state to foundation
                context.Foundation.DesignState = backupDesign;

                // Update revision;
                backupDesign.OnRevised();

                // Update client with new state
                context.Foundation.SendInfoTo(state);
                backupDesign.SendDetailedInfoTo(state);
            }
        }

        public static void Designer_Backup(NetState state, IEntity e, EncodedReader pvSrc)
        {
            Mobile from = state.Mobile;
            DesignContext context = DesignContext.Find(from);

            if (context != null)
            {
                /* Client chose to backup design state
                 *  - Construct a copy of the current design state
                 *  - Assign constructed state to backup state field
                 */

                // Construct a copy of the current design state
                DesignState copyState = new DesignState(context.Foundation.DesignState);

                // Assign constructed state to backup state field
                context.Foundation.BackupState = copyState;
            }
        }

        public static void Designer_Revert(NetState state, IEntity e, EncodedReader pvSrc)
        {
            Mobile from = state.Mobile;
            DesignContext context = DesignContext.Find(from);

            if (context != null)
            {
                /* Client chose to revert design state to currently visible state
                 *  - Revert design state
                 *     - Construct a copy of the current visible state
                 *     - Freeze fixtures in constructed state
                 *     - Assign constructed state to foundation
                 *     - If a signpost is needed, add it
                 *  - Update revision
                 *  - Update client with new state
                 */

                // Revert design state : Construct a copy of the current visible state
                DesignState copyState = new DesignState(context.Foundation.CurrentState);

                // Revert design state : Freeze fixtures in constructed state
                copyState.FreezeFixtures();

                // Revert design state : Assign constructed state to foundation
                context.Foundation.DesignState = copyState;

                // Revert design state : If a signpost is needed, add it
                context.Foundation.CheckSignpost();

                // Update revision
                copyState.OnRevised();

                // Update client with new state
                context.Foundation.SendInfoTo(state);
                copyState.SendDetailedInfoTo(state);
            }
        }

        public void RefreshHouse(Mobile owner)
        {
            DesignState copyState = new DesignState(DesignState);

            // Commit design state : Clear visible fixtures
            ClearFixtures(owner);

            // Commit design state : Melt fixtures from constructed state
            copyState.MeltFixtures();

            // Commit design state : Add melted fixtures from constructed state
            AddFixtures(owner, copyState.Fixtures);

            // Commit design state : Assign constructed state to foundation
            CurrentState = copyState;

            Delta(ItemDelta.Update);
            ProcessDelta();
            CurrentState.SendDetailedInfoTo(owner.NetState);

        }

        public void ECEndConfirmCommit(Mobile from)
        {
            if (this.Deleted)
                return;

            /* Client chose to commit current design state
             *  - Commit design state
             *     - Construct a copy of the current design state
             *     - Clear visible fixtures
             *     - Melt fixtures from constructed state
             *     - Add melted fixtures from constructed state
             *     - Assign constructed state to foundation
             *  - Update house price
             *  - Remove design context
             *  - Notify the client that customization has ended
             *  - Notify the core that the foundation has changed and should be resent to all clients
             *  - If a signpost is needed, add it
             *  - Eject all from house
             *  - Restore relocated entities
             */

            // Commit design state : Construct a copy of the current design state
            DesignState copyState = new DesignState(DesignState);

            // Commit design state : Clear visible fixtures
            ClearFixtures(from);

            // Commit design state : Melt fixtures from constructed state
            copyState.MeltFixtures();

            // Commit design state : Add melted fixtures from constructed state
            AddFixtures(from, copyState.Fixtures);

            // Commit design state : Assign constructed state to foundation
            CurrentState = copyState;

            //// Update house price
            //Price = newPrice;

            // Remove design context
            DesignContext.Remove(from);

            // Notify the client that customization has ended
            from.Send(new EndHouseCustomization(this));

            // Notify the core that the foundation has changed and should be resent to all clients
            Delta(ItemDelta.Update);
            ProcessDelta();
            CurrentState.SendDetailedInfoTo(from.NetState);

            // If a signpost is needed, add it
            CheckSignpost();

            // Eject all from house
            from.RevealingAction();

            foreach (Item item in GetItems())
                item.Location = BanLocation;

            foreach (Mobile mobile in GetMobiles())
                mobile.Location = BanLocation;

            // Restore relocated entities
            RestoreRelocatedEntities();
        }

        public void EndConfirmCommit(Mobile from)
        {
            if (this.Deleted)
                return;

            int oldPrice = Price;
            int newPrice = oldPrice + ((DesignState.Components.List.Length - CurrentState.Components.List.Length) * 500);
            int cost = newPrice - oldPrice;

            if (cost >= 0)
            {
                if (Banker.Withdraw(from, cost))
                {
                    from.SendLocalizedMessage(1060398, cost.ToString()); // ~1_AMOUNT~ gold has been withdrawn from your bank box.
                }
                else
                {
                    from.SendLocalizedMessage(1061903); // You cannot commit this house design, because you do not have the necessary funds in your bank box to pay for the upgrade.  Please back up your design, obtain the required funds, and commit your design again.
                    return;
                }
            }
            else
            {
                if (Banker.Deposit(from, -cost))
                    from.SendLocalizedMessage(1060397, (-cost).ToString()); // ~1_AMOUNT~ gold has been deposited into your bank box.
                else
                    return;
            }

            /* Client chose to commit current design state
             *  - Commit design state
             *     - Construct a copy of the current design state
             *     - Clear visible fixtures
             *     - Melt fixtures from constructed state
             *     - Add melted fixtures from constructed state
             *     - Assign constructed state to foundation
             *  - Update house price
             *  - Remove design context
             *  - Notify the client that customization has ended
             *  - Notify the core that the foundation has changed and should be resent to all clients
             *  - If a signpost is needed, add it
             *  - Eject all from house
             *  - Restore relocated entities
             */

            // Commit design state : Construct a copy of the current design state
            DesignState copyState = new DesignState(DesignState);

            // Commit design state : Clear visible fixtures
            ClearFixtures(from);

            // Commit design state : Melt fixtures from constructed state
            copyState.MeltFixtures();

            // Commit design state : Add melted fixtures from constructed state
            AddFixtures(from, copyState.Fixtures);

            // Commit design state : Assign constructed state to foundation
            CurrentState = copyState;

            // Update house price
            Price = newPrice;

            // Remove design context
            DesignContext.Remove(from);

            // Notify the client that customization has ended
            from.Send(new EndHouseCustomization(this));

            // Notify the core that the foundation has changed and should be resent to all clients
            Delta(ItemDelta.Update);
            ProcessDelta();
            CurrentState.SendDetailedInfoTo(from.NetState);

            // If a signpost is needed, add it
            CheckSignpost();

            // Eject all from house
            from.RevealingAction();

            foreach (Item item in GetItems())
                item.Location = BanLocation;

            foreach (Mobile mobile in GetMobiles())
                mobile.Location = BanLocation;

            // Restore relocated entities
            RestoreRelocatedEntities();
        }

        public static void Designer_Commit(NetState state, IEntity e, EncodedReader pvSrc)
        {
            Mobile from = state.Mobile;
            DesignContext context = DesignContext.Find(from);

            if (context != null)
            {
                int oldPrice = context.Foundation.Price;
                int newPrice = oldPrice + ((context.Foundation.DesignState.Components.List.Length - context.Foundation.CurrentState.Components.List.Length) * 500);
                int bankBalance = Banker.GetBalance(from);

                from.SendGump(new ConfirmCommitGump(from, context.Foundation, bankBalance, oldPrice, newPrice));
            }
        }

        public static bool IsSignHanger(int itemID)
        {
            itemID &= 0x7FFF;

            return (itemID >= 0xB97 && itemID < 0xBA3);
        }

        public int MaxLevels
        {
            get
            {
                MultiComponentList mcl = this.Components;

                if (mcl.Width >= 14 || mcl.Height >= 14)
                    return 4;
                else
                    return 3;
            }
        }

        public static int GetLevelZ(int level, HouseFoundation house)
        {
            if (level < 1 || level > house.MaxLevels)
                level = 1;

            return (level - 1) * 20 + 7;
        }

        public static int GetZLevel(int z, HouseFoundation house)
        {
            int level = (z - 7) / 20 + 1;

            if (level < 1 || level > house.MaxLevels)
                level = 1;

            return level;
        }

        private static ComponentVerification m_Verification;

        public static ComponentVerification Verification
        {
            get
            {
                if (m_Verification == null)
                    m_Verification = new ComponentVerification();

                return m_Verification;
            }
        }

        public static bool ValidPiece(int itemID)
        {
            return ValidPiece(itemID, false);
        }

        public static bool ValidPiece(int itemID, bool roof)
        {
            itemID &= TileData.MaxItemValue;

            if (!roof && (TileData.ItemTable[itemID].Flags & TileFlag.Roof) != 0)
                return false;
            else if (roof && (TileData.ItemTable[itemID].Flags & TileFlag.Roof) == 0)
                return false;

            return Verification.IsItemValid(itemID);
        }

        private static int[] m_BlockIDs = new int[]
			{
				0x3EE, 0x709, 0x71E, 0x721,
				0x738, 0x750, 0x76C, 0x788,
				0x7A3, 0x7BA,

				// 9th Anniversary
				0x35D2, 0x3609,

				// Stygian Abyss
				0x4317, 0x4318
			};

        private static int[] m_StairSeqs = new int[]
			{
				0x3EF, 0x70A, 0x722, 0x739,
				0x751, 0x76D, 0x789, 0x7A4
			};

        private static int[] m_StairIDs = new int[]
			{
				0x71F, 0x736, 0x737, 0x749,
				0x7BB, 0x7BC
			};

        private static int[] m_9aStairs = new int[]
			{
				0x35D3, 0x35D4, 0x35D5, 0x35D6,
				0x360A, 0x360B, 0x360C, 0x360D
			};

        public static bool IsStairBlock(int id)
        {
            id &= 0x7FFF;
            int delta = -1;

            for (int i = 0; delta < 0 && i < m_BlockIDs.Length; ++i)
                delta = (m_BlockIDs[i] - id);

            return (delta == 0);
        }

        public static bool IsStair(int id, ref int dir)
        {
            id &= TileData.MaxItemValue;
            int delta;

            if (id >= 0x435A && id <= 0x4365) // Gargish Stairs
            {
                dir = (id - 0x435A) % 4;

                switch (dir)
                {
                    case 0: dir = 1; break;
                    case 1: dir = 2; break;
                    case 2: dir = 0; break;
                    case 3: dir = 3; break;
                }

                return true;
            }
            else if (id >= 0x35D3) // 9th Anniversary
            {
                delta = -1;

                for (int i = 0; delta < 0 && i < m_9aStairs.Length; ++i)
                {
                    delta = (m_9aStairs[i] - id);
                    dir = (i % 4) + (i % 2 == 0 ? 1 : -1);
                }

                return (delta == 0);
            }
            else
            {
                delta = -4;

                for (int i = 0; delta < -3 && i < m_StairSeqs.Length; ++i)
                    delta = (m_StairSeqs[i] - id);

                if (delta >= -3 && delta <= 0)
                {
                    dir = -delta;
                    return true;
                }

                delta = -1;

                for (int i = 0; delta < 0 && i < m_StairIDs.Length; ++i)
                {
                    delta = (m_StairIDs[i] - id);
                    dir = i % 4;
                }

                return (delta == 0);
            }
        }

        public static bool DeleteStairs(MultiComponentList mcl, int id, int x, int y, int z)
        {
            int ax = x + mcl.Center.X;
            int ay = y + mcl.Center.Y;

            if (ax < 0 || ay < 0 || ax >= mcl.Width || ay >= (mcl.Height - 1) || z < 7 || ((z - 7) % 5) != 0)
                return false;

            if (IsStairBlock(id))
            {
                StaticTile[] tiles = mcl.Tiles[ax][ay];

                for (int i = 0; i < tiles.Length; ++i)
                {
                    StaticTile tile = tiles[i];

                    if (tile.Z == (z + 5))
                    {
                        id = tile.ID;
                        z = tile.Z;

                        if (!IsStairBlock(id))
                            break;
                    }
                }
            }

            int dir = 0;

            if (!IsStair(id, ref dir))
                return false;

            int height = ((z - 7) % 20) / 5;

            int xStart, yStart;
            int xInc, yInc;

            switch (dir)
            {
                default:
                case 0: // North
                    {
                        xStart = x;
                        yStart = y + height;
                        xInc = 0;
                        yInc = -1;
                        break;
                    }
                case 1: // West
                    {
                        xStart = x + height;
                        yStart = y;
                        xInc = -1;
                        yInc = 0;
                        break;
                    }
                case 2: // South
                    {
                        xStart = x;
                        yStart = y - height;
                        xInc = 0;
                        yInc = 1;
                        break;
                    }
                case 3: // East
                    {
                        xStart = x - height;
                        yStart = y;
                        xInc = 1;
                        yInc = 0;
                        break;
                    }
            }

            int zStart = z - (height * 5);

            for (int i = 0; i < 4; ++i)
            {
                x = xStart + (i * xInc);
                y = yStart + (i * yInc);

                for (int j = 0; j <= i; ++j)
                    mcl.RemoveXYZH(x, y, zStart + (j * 5), 5);

                ax = x + mcl.Center.X;
                ay = y + mcl.Center.Y;

                if (ax >= 1 && ax < mcl.Width && ay >= 1 && ay < mcl.Height - 1)
                {
                    StaticTile[] tiles = mcl.Tiles[ax][ay];

                    bool hasBaseFloor = false;

                    for (int j = 0; !hasBaseFloor && j < tiles.Length; ++j)
                        hasBaseFloor = (tiles[j].Z == 7 && (tiles[j].ID & TileData.MaxItemValue) != 1);

                    if (!hasBaseFloor)
                        mcl.Add(0x31F4, x, y, 7);
                }
            }

            return true;
        }

        public static void Designer_Delete(NetState state, IEntity e, EncodedReader pvSrc)
        {
            Mobile from = state.Mobile;
            DesignContext context = DesignContext.Find(from);

            if (context != null)
            {
                /* Client chose to delete a component
                 *  - Read data detailing which component to delete
                 *  - Verify component is deletable
                 *  - Remove the component
                 *  - If needed, replace removed component with a dirt tile
                 *  - Update revision
                 */

                // Read data detailing which component to delete
                int itemID = pvSrc.ReadInt32();
                int x = pvSrc.ReadInt32();
                int y = pvSrc.ReadInt32();
                int z = pvSrc.ReadInt32();

                // Verify component is deletable
                DesignState design = context.Foundation.DesignState;
                MultiComponentList mcl = design.Components;

                int ax = x + mcl.Center.X;
                int ay = y + mcl.Center.Y;

                if (IsSignHanger(itemID) || (z == 0 && ax >= 0 && ax < mcl.Width && ay >= 0 && ay < (mcl.Height - 1)))
                {
                    /* Component is not deletable
                     *  - Resend design state
                     *  - Return without further processing
                     */

                    design.SendDetailedInfoTo(state);
                    return;
                }

                bool deleteStairs = DeleteStairs(mcl, itemID, x, y, z);

                // Remove the component
                if (!deleteStairs)
                    mcl.Remove(itemID, x, y, z);

                // If needed, replace removed component with a dirt tile
                if (ax >= 1 && ax < mcl.Width && ay >= 1 && ay < mcl.Height - 1)
                {
                    StaticTile[] tiles = mcl.Tiles[ax][ay];

                    bool hasBaseFloor = false;

                    for (int i = 0; !hasBaseFloor && i < tiles.Length; ++i)
                        hasBaseFloor = (tiles[i].Z == 7 && (tiles[i].ID & TileData.MaxItemValue) != 1);

                    if (!hasBaseFloor)
                    {
                        // Replace with a dirt tile
                        mcl.Add(0x31F4, x, y, 7);
                    }
                }

                if (deleteStairs)
                    design.SendDetailedInfoTo(state);

                // Update revision
                design.OnRevised();
            }
        }

        public static void Designer_Stairs(NetState state, IEntity e, EncodedReader pvSrc)
        {
            Mobile from = state.Mobile;
            DesignContext context = DesignContext.Find(from);

            if (context != null)
            {
                /* Client chose to add stairs
                 *  - Read data detailing stair type and location
                 *  - Validate stair multi ID
                 *  - Add the stairs
                 *     - Load data describing the stair components
                 *     - Insert described components
                 *  - Update revision
                 */

                // Read data detailing stair type and location
                int itemID = pvSrc.ReadInt32();
                int x = pvSrc.ReadInt32();
                int y = pvSrc.ReadInt32();

                // Validate stair multi ID
                DesignState design = context.Foundation.DesignState;

                if (!Verification.IsMultiValid(itemID))
                {
                    /* Specified multi ID is not a stair
                     *  - Resend design state
                     *  - Return without further processing
                     */

                    TraceValidity(state, itemID);
                    design.SendDetailedInfoTo(state);
                    return;
                }

                // Add the stairs
                MultiComponentList mcl = design.Components;

                // Add the stairs : Load data describing stair components
                MultiComponentList stairs = MultiData.GetComponents(itemID);

                // Add the stairs : Insert described components
                int z = GetLevelZ(context.Level, context.Foundation);

                for (int i = 0; i < stairs.List.Length; ++i)
                {
                    MultiTileEntry entry = stairs.List[i];

                    if ((entry.m_ItemID & TileData.MaxItemValue) != 1)
                        mcl.Add(entry.m_ItemID, x + entry.m_OffsetX, y + entry.m_OffsetY, z + entry.m_OffsetZ);
                }

                // Update revision
                design.OnRevised();
            }
        }

        private static void TraceValidity(NetState state, int itemID)
        {
            try
            {
                using (StreamWriter op = new StreamWriter("comp_val.log", true))
                    op.WriteLine("{0}\t{1}\tInvalid ItemID 0x{2:X4}", state, state.Mobile, itemID);
            }
            catch
            {
            }
        }

        public static void Designer_Build(NetState state, IEntity e, EncodedReader pvSrc)
        {
            Mobile from = state.Mobile;
            DesignContext context = DesignContext.Find(from);

            if (context != null)
            {
                /* Client chose to add a component
                 *  - Read data detailing component graphic and location
                 *  - Add component
                 *  - Update revision
                 */

                // Read data detailing component graphic and location
                int itemID = pvSrc.ReadInt32();
                int x = pvSrc.ReadInt32();
                int y = pvSrc.ReadInt32();

                // Add component
                DesignState design = context.Foundation.DesignState;

                if (from.AccessLevel < AccessLevel.GameMaster && !ValidPiece(itemID))
                {
                    TraceValidity(state, itemID);
                    design.SendDetailedInfoTo(state);
                    return;
                }

                MultiComponentList mcl = design.Components;

                int z = GetLevelZ(context.Level, context.Foundation);

                if ((y + mcl.Center.Y) == (mcl.Height - 1))
                    z = 0; // Tiles placed on the far-south of the house are at 0 Z

                mcl.Add(itemID, x, y, z);

                // Update revision
                design.OnRevised();
            }
        }

        public static void Designer_Close(NetState state, IEntity e, EncodedReader pvSrc)
        {
            Mobile from = state.Mobile;
            DesignContext context = DesignContext.Find(from);

            if (context != null)
            {
                /* Client closed his house design window
                 *  - Remove design context
                 *  - Notify the client that customization has ended
                 *  - Refresh client with current visable design state
                 *  - If a signpost is needed, add it
                 *  - Eject all from house
                 *  - Restore relocated entities
                 */

                // Remove design context
                DesignContext.Remove(from);

                // Notify the client that customization has ended
                from.Send(new EndHouseCustomization(context.Foundation));

                // Refresh client with current visible design state
                context.Foundation.SendInfoTo(state);
                context.Foundation.CurrentState.SendDetailedInfoTo(state);

                // If a signpost is needed, add it
                context.Foundation.CheckSignpost();

                // Eject all from house
                from.RevealingAction();

                foreach (Item item in context.Foundation.GetItems())
                    item.Location = context.Foundation.BanLocation;

                foreach (Mobile mobile in context.Foundation.GetMobiles())
                    mobile.Location = context.Foundation.BanLocation;

                // Restore relocated entities
                context.Foundation.RestoreRelocatedEntities();
            }
        }

        public static void Designer_Level(NetState state, IEntity e, EncodedReader pvSrc)
        {
            Mobile from = state.Mobile;
            DesignContext context = DesignContext.Find(from);

            if (context != null)
            {
                /* Client is moving to a new floor level
                 *  - Read data detailing the target level
                 *  - Validate target level
                 *  - Update design context with new level
                 *  - Teleport mobile to new level
                 *  - Update client
                 */

                // Read data detailing the target level
                int newLevel = pvSrc.ReadInt32();

                // Validate target level
                if (newLevel < 1 || newLevel > context.MaxLevels)
                    newLevel = 1;

                // Update design context with new level
                context.Level = newLevel;

                // Teleport mobile to new level
                from.Location = new Point3D(from.X, from.Y, context.Foundation.Z + GetLevelZ(newLevel, context.Foundation));

                // Update client
                context.Foundation.SendInfoTo(state);
            }
        }

        public static void QueryDesignDetails(NetState state, PacketReader pvSrc)
        {
            Mobile from = state.Mobile;
            DesignContext context = DesignContext.Find(from);

            HouseFoundation foundation = World.FindItem(pvSrc.ReadInt32()) as HouseFoundation;

            if (foundation != null && from.Map == foundation.Map && from.InRange(foundation.GetWorldLocation(), 24) && from.CanSee(foundation))
            {
                DesignState stateToSend;

                if (context != null && context.Foundation == foundation)
                    stateToSend = foundation.DesignState;
                else
                    stateToSend = foundation.CurrentState;

                stateToSend.SendDetailedInfoTo(state);
            }
        }

        public static void Designer_Roof(NetState state, IEntity e, EncodedReader pvSrc)
        {
            Mobile from = state.Mobile;
            DesignContext context = DesignContext.Find(from);

            if (context != null)
            {
                // Read data detailing component graphic and location
                int itemID = pvSrc.ReadInt32();
                int x = pvSrc.ReadInt32();
                int y = pvSrc.ReadInt32();
                int z = pvSrc.ReadInt32();

                // Add component
                DesignState design = context.Foundation.DesignState;

                if (from.AccessLevel < AccessLevel.GameMaster && !ValidPiece(itemID, true))
                {
                    TraceValidity(state, itemID);
                    design.SendDetailedInfoTo(state);
                    return;
                }

                MultiComponentList mcl = design.Components;

                if (z < -3 || z > 12 || z % 3 != 0)
                    z = -3;
                z += GetLevelZ(context.Level, context.Foundation);

                MultiTileEntry[] list = mcl.List;
                for (int i = 0; i < list.Length; i++)
                {
                    MultiTileEntry mte = list[i];

                    if (mte.m_OffsetX == x && mte.m_OffsetY == y && GetZLevel(mte.m_OffsetZ, context.Foundation) == context.Level && (TileData.ItemTable[mte.m_ItemID & TileData.MaxItemValue].Flags & TileFlag.Roof) != 0)
                        mcl.Remove(mte.m_ItemID, x, y, mte.m_OffsetZ);
                }

                mcl.Add(itemID, x, y, z);

                // Update revision
                design.OnRevised();
            }
        }

        public static void Designer_RoofDelete(NetState state, IEntity e, EncodedReader pvSrc)
        {
            Mobile from = state.Mobile;
            DesignContext context = DesignContext.Find(from);

            if (context != null)
            {
                // Read data detailing which component to delete
                int itemID = pvSrc.ReadInt32();
                int x = pvSrc.ReadInt32();
                int y = pvSrc.ReadInt32();
                int z = pvSrc.ReadInt32();

                // Verify component is deletable
                DesignState design = context.Foundation.DesignState;
                MultiComponentList mcl = design.Components;

                if ((TileData.ItemTable[itemID & TileData.MaxItemValue].Flags & TileFlag.Roof) == 0)
                {
                    design.SendDetailedInfoTo(state);
                    return;
                }

                mcl.Remove(itemID, x, y, z);

                design.OnRevised();
            }
        }
    }

    public class DesignState
    {
        private HouseFoundation m_Foundation;
        private MultiComponentList m_Components;
        private MultiTileEntry[] m_Fixtures;
        private int m_Revision;
        private Packet m_PacketCache;

        public Packet PacketCache
        {
            get { return m_PacketCache; }
            set
            {
                if (m_PacketCache == value)
                    return;

                if (m_PacketCache != null)
                    m_PacketCache.Release();

                m_PacketCache = value;
            }
        }

        public HouseFoundation Foundation { get { return m_Foundation; } }
        public MultiComponentList Components { get { return m_Components; } }
        public MultiTileEntry[] Fixtures { get { return m_Fixtures; } }
        public int Revision { get { return m_Revision; } set { m_Revision = value; } }

        public DesignState(HouseFoundation foundation, MultiComponentList components)
        {
            m_Foundation = foundation;
            m_Components = components;
            m_Fixtures = new MultiTileEntry[0];
        }

        public DesignState(DesignState toCopy)
        {
            m_Foundation = toCopy.m_Foundation;
            m_Components = new MultiComponentList(toCopy.m_Components);
            m_Revision = toCopy.m_Revision;
            m_Fixtures = new MultiTileEntry[toCopy.m_Fixtures.Length];

            for (int i = 0; i < m_Fixtures.Length; ++i)
                m_Fixtures[i] = toCopy.m_Fixtures[i];
        }

        public DesignState(HouseFoundation foundation, GenericReader reader)
        {
            m_Foundation = foundation;

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_Components = new MultiComponentList(reader);

                        int length = reader.ReadInt();

                        m_Fixtures = new MultiTileEntry[length];

                        for (int i = 0; i < length; ++i)
                        {
                            m_Fixtures[i].m_ItemID = reader.ReadUShort();
                            m_Fixtures[i].m_OffsetX = reader.ReadShort();
                            m_Fixtures[i].m_OffsetY = reader.ReadShort();
                            m_Fixtures[i].m_OffsetZ = reader.ReadShort();
                            m_Fixtures[i].m_Flags = reader.ReadInt();
                        }

                        m_Revision = reader.ReadInt();

                        break;
                    }
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write((int)0); // version

            m_Components.Serialize(writer);

            writer.Write((int)m_Fixtures.Length);

            for (int i = 0; i < m_Fixtures.Length; ++i)
            {
                MultiTileEntry ent = m_Fixtures[i];

                writer.Write((ushort)ent.m_ItemID);
                writer.Write((short)ent.m_OffsetX);
                writer.Write((short)ent.m_OffsetY);
                writer.Write((short)ent.m_OffsetZ);
                writer.Write((int)ent.m_Flags);
            }

            writer.Write((int)m_Revision);
        }

        public void OnRevised()
        {
            lock (this)
            {
                m_Revision = ++m_Foundation.LastRevision;

                if (m_PacketCache != null)
                    m_PacketCache.Release();

                m_PacketCache = null;
            }
        }

        public void SendGeneralInfoTo(NetState state)
        {
            if (state != null)
                state.Send(new DesignStateGeneral(m_Foundation, this));
        }

        public void SendDetailedInfoTo(NetState state)
        {
            if (state != null)
            {
                lock (this)
                {
                    if (m_PacketCache == null)
                        DesignStateDetailed.SendDetails(state, m_Foundation, this);
                    else
                        state.Send(m_PacketCache);
                }
            }
        }

        public void FreezeFixtures()
        {
            OnRevised();

            for (int i = 0; i < m_Fixtures.Length; ++i)
            {
                MultiTileEntry mte = m_Fixtures[i];

                m_Components.Add(mte.m_ItemID, mte.m_OffsetX, mte.m_OffsetY, mte.m_OffsetZ);
            }

            m_Fixtures = new MultiTileEntry[0];
        }

        public void MeltFixtures()
        {
            OnRevised();

            MultiTileEntry[] list = m_Components.List;
            int length = 0;

            for (int i = list.Length - 1; i >= 0; --i)
            {
                MultiTileEntry mte = list[i];

                if (IsFixture(mte.m_ItemID))
                    ++length;
            }

            m_Fixtures = new MultiTileEntry[length];

            for (int i = list.Length - 1; i >= 0; --i)
            {
                MultiTileEntry mte = list[i];

                if (IsFixture(mte.m_ItemID))
                {
                    m_Fixtures[--length] = mte;
                    m_Components.Remove(mte.m_ItemID, mte.m_OffsetX, mte.m_OffsetY, mte.m_OffsetZ);
                }
            }
        }

        public static bool IsFixture(int itemID)
        {
            itemID &= 0x7FFF;

            if (itemID >= 0x675 && itemID < 0x6F5)
                return true;
            else if (itemID >= 0x314 && itemID < 0x364)
                return true;
            else if (itemID >= 0x824 && itemID < 0x834)
                return true;
            else if (itemID >= 0x839 && itemID < 0x849)
                return true;
            else if (itemID >= 0x84C && itemID < 0x85C)
                return true;
            else if (itemID >= 0x866 && itemID < 0x876)
                return true;
            else if (itemID >= 0x0E8 && itemID < 0x0F8)
                return true;
            else if (itemID >= 0x1FED && itemID < 0x1FFD)
                return true;
            else if (itemID >= 0x181D && itemID < 0x1829)
                return true;
            else if (itemID >= 0x241F && itemID < 0x2421)
                return true;
            else if (itemID >= 0x2423 && itemID < 0x2425)
                return true;
            else if (itemID >= 0x2A05 && itemID < 0x2A1D)
                return true;
            else if (itemID >= 0x319C && itemID < 0x31B0)
                return true;
            else if (itemID == 0x2D46 || itemID == 0x2D48 || itemID == 0x2FE2 || itemID == 0x2FE4)	//ML doors begin here.  Note funkyness.
                return true;
            else if (itemID >= 0x2D63 && itemID < 0x2D70)
                return true;
            else if (itemID >= 0x319C && itemID < 0x31AF)
                return true;
            else if (itemID >= 0x367B && itemID < 0x369B)
                return true;
            // Stygian Abyss Doors
            else if (itemID >= 16539 && itemID <= 16546)
                return true;
            else if (itemID >= 16652 && itemID <= 16659)
                return true;
            else if (itemID >= 16834 && itemID <= 16841)
                return true;
            else if (itemID >= 16847 && itemID <= 16854)
                return true;
            else if (itemID >= 17262 && itemID <= 17277)
                return true;
            else if (itemID >= 18141 && itemID <= 18148)
                return true;
            else if (itemID >= 19746 && itemID <= 19753)
                return true;
            else if (itemID >= 20680 && itemID <= 20695)
                return true;
            else if (itemID >= 20802 && itemID <= 20809)
                return true;

            return false;
        }
    }

    public class ConfirmCommitGump : Gump
    {
        public override int TypeID { get { return 0x1F7; } }

        private HouseFoundation m_Foundation;

        public ConfirmCommitGump(Mobile from, HouseFoundation foundation, int bankBalance, int oldPrice, int newPrice)
            : base(50, 50)
        {
            m_Foundation = foundation;

            int cost = newPrice - oldPrice;
            bool canAfford = cost <= bankBalance;

            AddPage(0);

            AddBackground(0, 0, 320, 320, 5054);

            AddImageTiled(10, 10, 300, 20, 2624);
            AddImageTiled(10, 40, 300, 240, 2624);
            AddImageTiled(10, 290, 300, 20, 2624);

            AddAlphaRegion(10, 10, 300, 300);

            AddHtmlLocalized(10, 10, 300, 20, 1062060, 32736, false, false); // <CENTER>COMMIT DESIGN</CENTER>

            AddHtmlLocalized(10, 190, 150, 20, 1061902, 32736, false, false); // Bank Balance:
            AddLabel(170, 190, 55, bankBalance.ToString());

            AddHtmlLocalized(10, 215, 150, 20, 1061899, 1023, false, false); // Old Value:
            AddLabel(170, 215, 90, oldPrice.ToString());

            AddHtmlLocalized(10, 235, 150, 20, 1061900, 1023, false, false); // Cost To Commit:
            AddLabel(170, 235, 90, newPrice.ToString());

            AddButton(170, 290, 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(195, 290, 55, 20, 1011012, 32767, false, false); // CANCEL

            if (canAfford)
            {
                AddHtmlLocalized(10, 40, 300, 140, 1061898, 0x3FF, false, true);

                AddButton(10, 290, 4005, 4007, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 290, 55, 20, 1011036, 32767, false, false); // OKAY
            }
            else
            {
                AddHtmlLocalized(10, 40, 300, 140, 1061903, 0x7C00, false, true);
            }

            if (cost >= 0)
            {
                AddHtmlLocalized(10, 260, 150, 20, 1061901, 0x7C00, false, false); // Your Cost:
                AddLabel(170, 260, 40, cost.ToString());
            }
            else
            {
                AddHtmlLocalized(10, 260, 150, 20, 1062059, 0x3E0, false, false); // Your Refund:
                AddLabel(170, 260, 70, (-cost).ToString());
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                m_Foundation.EndConfirmCommit(sender.Mobile);
            }
        }
    }

    public class DesignContext
    {
        private HouseFoundation m_Foundation;
        private int m_Level;

        public HouseFoundation Foundation { get { return m_Foundation; } }
        public int Level { get { return m_Level; } set { m_Level = value; } }
        public int MaxLevels { get { return m_Foundation.MaxLevels; } }

        public DesignContext(HouseFoundation foundation)
        {
            m_Foundation = foundation;
            m_Level = 1;
        }

        private static Hashtable m_Table = new Hashtable();

        public static Hashtable Table { get { return m_Table; } }

        public static DesignContext Find(Mobile from)
        {
            if (from == null)
            {
                return null;
            }

            return (DesignContext)m_Table[from];
        }

        public static bool Check(Mobile m)
        {
            if (Find(m) != null)
            {
                m.SendLocalizedMessage(1062206); // You cannot do that while customizing a house.
                return false;
            }

            return true;
        }

        public static void Add(Mobile from, HouseFoundation foundation)
        {
            if (from == null)
            {
                return;
            }

            DesignContext c = new DesignContext(foundation);

            m_Table[from] = c;

            if (from is PlayerMobile)
            {
                ((PlayerMobile)from).DesignContext = c;
            }

            foundation.Customizer = from;

            from.Hidden = true;
            from.Location = new Point3D(foundation.X, foundation.Y, foundation.Z + 7);

            NetState state = from.NetState;

            if (state == null)
            {
                return;
            }

            List<Item> fixtures = foundation.Fixtures;

            for (int i = 0; fixtures != null && i < fixtures.Count; ++i)
            {
                Item item = (Item)fixtures[i];

                state.Send(item.RemovePacket);
            }

            if (foundation.Signpost != null)
            {
                state.Send(foundation.Signpost.RemovePacket);
            }

            if (foundation.SignHanger != null)
            {
                state.Send(foundation.SignHanger.RemovePacket);
            }

            if (foundation.Sign != null)
            {
                state.Send(foundation.Sign.RemovePacket);
            }
        }

        public static void Remove(Mobile from)
        {
            if (from == null)
                return;

            DesignContext context = (DesignContext)m_Table[from];

            m_Table.Remove(from);

            if (from is PlayerMobile)
            {
                ((PlayerMobile)from).DesignContext = null;
            }

            if (context == null)
                return;

            context.Foundation.Customizer = null;

            NetState state = from.NetState;

            if (state == null)
                return;

            List<Item> fixtures = context.Foundation.Fixtures;

            for (int i = 0; fixtures != null && i < fixtures.Count; ++i)
            {
                Item item = (Item)fixtures[i];

                item.SendInfoTo(state);
            }

            if (context.Foundation.Signpost != null)
                context.Foundation.Signpost.SendInfoTo(state);

            if (context.Foundation.SignHanger != null)
                context.Foundation.SignHanger.SendInfoTo(state);

            if (context.Foundation.Sign != null)
                context.Foundation.Sign.SendInfoTo(state);
        }
    }

    public class BeginHouseCustomization : Packet
    {
        public BeginHouseCustomization(HouseFoundation house)
            : base(0xBF)
        {
            EnsureCapacity(17);

            m_Stream.Write((short)0x20);
            m_Stream.Write((int)house.Serial);
            m_Stream.Write((byte)0x04);
            m_Stream.Write((ushort)0x0000);
            m_Stream.Write((ushort)0xFFFF);
            m_Stream.Write((ushort)0xFFFF);
            m_Stream.Write((byte)0xFF);
        }
    }

    public class EndHouseCustomization : Packet
    {
        public EndHouseCustomization(HouseFoundation house)
            : base(0xBF)
        {
            EnsureCapacity(17);

            m_Stream.Write((short)0x20);
            m_Stream.Write((int)house.Serial);
            m_Stream.Write((byte)0x05);
            m_Stream.Write((ushort)0x0000);
            m_Stream.Write((ushort)0xFFFF);
            m_Stream.Write((ushort)0xFFFF);
            m_Stream.Write((byte)0xFF);
        }
    }

    public class DesignStateGeneral : Packet
    {
        public DesignStateGeneral(HouseFoundation house, DesignState state)
            : base(0xBF)
        {
            EnsureCapacity(13);

            m_Stream.Write((short)0x1D);
            m_Stream.Write((int)house.Serial);
            m_Stream.Write((int)state.Revision);
        }
    }

    public sealed class DesignStateDetailed : Packet
    {
        public const int MaxItemsPerStairBuffer = 750;

        private static BufferPool m_PlaneBufferPool = new BufferPool("Housing Plane Buffers", 9, 0x400);
        private static BufferPool m_StairBufferPool = new BufferPool("Housing Stair Buffers", 6, MaxItemsPerStairBuffer * 5);
        private static BufferPool m_DeflatedBufferPool = new BufferPool("Housing Deflated Buffers", 1, 0x2000);

        private byte[][] m_PlaneBuffers;
        private byte[][] m_StairBuffers;

        private bool[] m_PlaneUsed = new bool[9];
        private byte[] m_PrimBuffer = new byte[4];

        public void Write(int value)
        {
            m_PrimBuffer[0] = (byte)(value >> 24);
            m_PrimBuffer[1] = (byte)(value >> 16);
            m_PrimBuffer[2] = (byte)(value >> 8);
            m_PrimBuffer[3] = (byte)value;

            m_Stream.UnderlyingStream.Write(m_PrimBuffer, 0, 4);
        }

        public void Write(short value)
        {
            m_PrimBuffer[0] = (byte)(value >> 8);
            m_PrimBuffer[1] = (byte)value;

            m_Stream.UnderlyingStream.Write(m_PrimBuffer, 0, 2);
        }

        public void Write(byte value)
        {
            m_Stream.UnderlyingStream.WriteByte(value);
        }

        public void Write(byte[] buffer, int offset, int size)
        {
            m_Stream.UnderlyingStream.Write(buffer, offset, size);
        }

        public static void Clear(byte[] buffer, int size)
        {
            for (int i = 0; i < size; ++i)
                buffer[i] = 0;
        }

        public DesignStateDetailed(int serial, int revision, int xMin, int yMin, int xMax, int yMax, MultiTileEntry[] tiles)
            : base(0xD8)
        {
            EnsureCapacity(17 + (tiles.Length * 5));

            Write((byte)0x03); // Compression Type
            Write((byte)0x00); // Unknown
            Write((int)serial);
            Write((int)revision);
            Write((short)tiles.Length);
            Write((short)0); // Buffer length : reserved
            Write((byte)0); // Plane count : reserved

            int totalLength = 1; // includes plane count

            int width = (xMax - xMin) + 1;
            int height = (yMax - yMin) + 1;

            m_PlaneBuffers = new byte[9][];

            lock (m_PlaneBufferPool)
                for (int i = 0; i < m_PlaneBuffers.Length; ++i)
                {
                    m_PlaneBuffers[i] = m_PlaneBufferPool.AcquireBuffer();
                }

            m_StairBuffers = new byte[6][];

            lock (m_StairBufferPool)
                for (int i = 0; i < m_StairBuffers.Length; ++i)
                {
                    m_StairBuffers[i] = m_StairBufferPool.AcquireBuffer();
                }

            Clear(m_PlaneBuffers[0], width * height * 2);

            for (int i = 0; i < 4; ++i)
            {
                Clear(m_PlaneBuffers[1 + i], (width - 1) * (height - 2) * 2);
                Clear(m_PlaneBuffers[5 + i], width * (height - 1) * 2);
            }

            int totalStairsUsed = 0;

            for (int i = 0; i < tiles.Length; ++i)
            {
                MultiTileEntry mte = tiles[i];
                int x = mte.m_OffsetX - xMin;
                int y = mte.m_OffsetY - yMin;
                int z = mte.m_OffsetZ;
                bool floor = (TileData.ItemTable[mte.m_ItemID & TileData.MaxItemValue].Height <= 0);
                int plane, size;

                switch (z)
                {
                    case 0: plane = 0; break;
                    case 7: plane = 1; break;
                    case 27: plane = 2; break;
                    case 47: plane = 3; break;
                    case 67: plane = 4; break;
                    default:
                        {
                            int stairBufferIndex = (totalStairsUsed / MaxItemsPerStairBuffer);
                            byte[] stairBuffer = m_StairBuffers[stairBufferIndex];

                            int byteIndex = (totalStairsUsed % MaxItemsPerStairBuffer) * 5;

                            stairBuffer[byteIndex++] = (byte)(mte.m_ItemID >> 8);
                            stairBuffer[byteIndex++] = (byte)mte.m_ItemID;

                            stairBuffer[byteIndex++] = (byte)mte.m_OffsetX;
                            stairBuffer[byteIndex++] = (byte)mte.m_OffsetY;
                            stairBuffer[byteIndex++] = (byte)mte.m_OffsetZ;

                            ++totalStairsUsed;

                            continue;
                        }
                }

                if (plane == 0)
                {
                    size = height;
                }
                else if (floor)
                {
                    size = height - 2;
                    x -= 1;
                    y -= 1;
                }
                else
                {
                    size = height - 1;
                    plane += 4;
                }

                int index = ((x * size) + y) * 2;

                if (x < 0 || y < 0 || y >= size || (index + 1) >= 0x400)
                {
                    int stairBufferIndex = (totalStairsUsed / MaxItemsPerStairBuffer);
                    byte[] stairBuffer = m_StairBuffers[stairBufferIndex];

                    int byteIndex = (totalStairsUsed % MaxItemsPerStairBuffer) * 5;

                    stairBuffer[byteIndex++] = (byte)(mte.m_ItemID >> 8);
                    stairBuffer[byteIndex++] = (byte)mte.m_ItemID;

                    stairBuffer[byteIndex++] = (byte)mte.m_OffsetX;
                    stairBuffer[byteIndex++] = (byte)mte.m_OffsetY;
                    stairBuffer[byteIndex++] = (byte)mte.m_OffsetZ;

                    ++totalStairsUsed;
                }
                else
                {
                    m_PlaneUsed[plane] = true;
                    m_PlaneBuffers[plane][index] = (byte)(mte.m_ItemID >> 8);
                    m_PlaneBuffers[plane][index + 1] = (byte)mte.m_ItemID;
                }
            }

            int planeCount = 0;

            byte[] m_DeflatedBuffer = null;
            lock (m_DeflatedBufferPool)
                m_DeflatedBuffer = m_DeflatedBufferPool.AcquireBuffer();

            for (int i = 0; i < m_PlaneBuffers.Length; ++i)
            {
                if (!m_PlaneUsed[i])
                {
                    m_PlaneBufferPool.ReleaseBuffer(m_PlaneBuffers[i]);
                    continue;
                }

                ++planeCount;

                int size = 0;

                if (i == 0)
                {
                    size = width * height * 2;
                }
                else if (i < 5)
                {
                    size = (width - 1) * (height - 2) * 2;
                }
                else
                {
                    size = width * (height - 1) * 2;
                }

                byte[] inflatedBuffer = m_PlaneBuffers[i];

                int deflatedLength = m_DeflatedBuffer.Length;
                ZLibError ce = Compression.Pack(m_DeflatedBuffer, ref deflatedLength, inflatedBuffer, size, ZLibQuality.Default);

                if (ce != ZLibError.Okay)
                {
                    Console.WriteLine("ZLib error: {0} (#{1})", ce, (int)ce);
                    deflatedLength = 0;
                    size = 0;
                }

                Write((byte)(0x20 | i));
                Write((byte)size);
                Write((byte)deflatedLength);
                Write((byte)(((size >> 4) & 0xF0) | ((deflatedLength >> 8) & 0xF)));
                Write(m_DeflatedBuffer, 0, deflatedLength);

                totalLength += 4 + deflatedLength;
                lock (m_PlaneBufferPool)
                    m_PlaneBufferPool.ReleaseBuffer(inflatedBuffer);
            }

            int totalStairBuffersUsed = (totalStairsUsed + (MaxItemsPerStairBuffer - 1)) / MaxItemsPerStairBuffer;

            for (int i = 0; i < totalStairBuffersUsed; ++i)
            {
                ++planeCount;

                int count = (totalStairsUsed - (i * MaxItemsPerStairBuffer));

                if (count > MaxItemsPerStairBuffer)
                {
                    count = MaxItemsPerStairBuffer;
                }

                int size = count * 5;

                byte[] inflatedBuffer = m_StairBuffers[i];

                int deflatedLength = m_DeflatedBuffer.Length;
                ZLibError ce = Compression.Pack(m_DeflatedBuffer, ref deflatedLength, inflatedBuffer, size, ZLibQuality.Default);

                if (ce != ZLibError.Okay)
                {
                    Console.WriteLine("ZLib error: {0} (#{1})", ce, (int)ce);
                    deflatedLength = 0;
                    size = 0;
                }

                Write((byte)(9 + i));
                Write((byte)size);
                Write((byte)deflatedLength);
                Write((byte)(((size >> 4) & 0xF0) | ((deflatedLength >> 8) & 0xF)));
                Write(m_DeflatedBuffer, 0, deflatedLength);

                totalLength += 4 + deflatedLength;
            }

            lock (m_StairBufferPool)
                for (int i = 0; i < m_StairBuffers.Length; ++i)
                {
                    m_StairBufferPool.ReleaseBuffer(m_StairBuffers[i]);
                }

            lock (m_DeflatedBufferPool)
                m_DeflatedBufferPool.ReleaseBuffer(m_DeflatedBuffer);

            m_Stream.Seek(15, System.IO.SeekOrigin.Begin);

            Write((short)totalLength); // Buffer length
            Write((byte)planeCount); // Plane count
        }

        private class SendQueueEntry
        {
            public NetState m_NetState;
            public int m_Serial, m_Revision;
            public int m_xMin, m_yMin, m_xMax, m_yMax;
            public DesignState m_Root;
            public MultiTileEntry[] m_Tiles;

            public SendQueueEntry(NetState ns, HouseFoundation foundation, DesignState state)
            {
                m_NetState = ns;
                m_Serial = foundation.Serial;
                m_Revision = state.Revision;
                m_Root = state;

                MultiComponentList mcl = state.Components;

                m_xMin = mcl.Min.X;
                m_yMin = mcl.Min.Y;
                m_xMax = mcl.Max.X;
                m_yMax = mcl.Max.Y;

                m_Tiles = mcl.List;
            }
        }

        private static Queue<SendQueueEntry> m_SendQueue;
        private static object m_SendQueueSyncRoot;
        private static AutoResetEvent m_Sync;
        private static Thread m_Thread;

        static DesignStateDetailed()
        {
            m_SendQueue = new Queue<SendQueueEntry>();
            m_SendQueueSyncRoot = ((ICollection)m_SendQueue).SyncRoot;
            m_Sync = new AutoResetEvent(false);

            m_Thread = new Thread(new ThreadStart(CompressionThread));
            m_Thread.Name = "Housing Compression Thread";
            m_Thread.Start();
        }

        public static void CompressionThread()
        {
            while (!Core.Closing)
            {
                m_Sync.WaitOne();

                int count;

                lock (m_SendQueueSyncRoot)
                    count = m_SendQueue.Count;

                while (count > 0)
                {
                    SendQueueEntry sqe = null;

                    lock (m_SendQueueSyncRoot)
                        sqe = m_SendQueue.Dequeue();

                    try
                    {
                        Packet p = null;

                        lock (sqe.m_Root)
                            p = sqe.m_Root.PacketCache;

                        if (p == null)
                        {
                            p = new DesignStateDetailed(sqe.m_Serial, sqe.m_Revision, sqe.m_xMin, sqe.m_yMin, sqe.m_xMax, sqe.m_yMax, sqe.m_Tiles);
                            p.SetStatic();

                            lock (sqe.m_Root)
                            {
                                if (sqe.m_Revision == sqe.m_Root.Revision)
                                    sqe.m_Root.PacketCache = p;
                            }
                        }

                        sqe.m_NetState.Send(p);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);

                        try
                        {
                            using (StreamWriter op = new StreamWriter("dsd_exceptions.txt", true))
                                op.WriteLine(e);
                        }
                        catch
                        {
                        }
                    }
                    finally
                    {
                        lock (m_SendQueueSyncRoot)
                            count = m_SendQueue.Count;
                    }

                    //sqe.m_NetState.Send( new DesignStateDetailed( sqe.m_Serial, sqe.m_Revision, sqe.m_xMin, sqe.m_yMin, sqe.m_xMax, sqe.m_yMax, sqe.m_Tiles ) );
                }
            }
        }

        public static void SendDetails(NetState ns, HouseFoundation house, DesignState state)
        {
            lock (m_SendQueueSyncRoot)
                m_SendQueue.Enqueue(new SendQueueEntry(ns, house, state));
            m_Sync.Set();
        }
    }
}