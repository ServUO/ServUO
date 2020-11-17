using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Spells;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;

namespace Server.Multis
{
    public enum FoundationType
    {
        Stone,
        DarkWood,
        LightWood,
        Dungeon,
        Brick,
        ElvenGrey,
        ElvenNatural,
        Crystal,
        Shadow,
        SimpleMarble,
        PlainMarble,
        OrnateMarble,
        GargishGreenMarble,
        GargishTwoToneStone,
        Gothic,
        Brick1,
        Brick2
    }

    public class HouseFoundation : BaseHouse
    {
        private DesignState m_Current; // State which is currently visible.
        private DesignState m_Design;  // State of current design.
        private DesignState m_Backup;  // State at last user backup.        

        // Graphic type of this foundation.
        public FoundationType Type { get; set; }

        // Latest revision number.
        public int LastRevision { get; set; }

        // List of fixtures (teleporters and doors) associated with this house.
        public List<Item> Fixtures { get; set; }

        // Item hanging the sign.
        public Item SignHanger { get; set; }

        // Item supporting the hanger.
        public Item Signpost { get; set; }

        // ItemID number of the chosen signpost.
        public int SignpostGraphic { get; set; }

        // Who is currently customizing this -or- null if not customizing.
        public Mobile Customizer { get; set; }

        public override bool IsActive => Customizer == null;

        public virtual int CustomizationCost => 0;

        public bool IsFixture(Item item)
        {
            return (Fixtures != null && Fixtures.Contains(item));
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

        public DesignState CurrentState
        {
            get
            {
                if (m_Current == null)
                    SetInitialState();

                return m_Current;
            }
            set
            {
                m_Current = value;
            }
        }

        public DesignState DesignState
        {
            get
            {
                if (m_Design == null)
                    SetInitialState();

                return m_Design;
            }
            set
            {
                m_Design = value;
            }
        }

        public DesignState BackupState
        {
            get
            {
                if (m_Backup == null)
                    SetInitialState();

                return m_Backup;
            }
            set
            {
                m_Backup = value;
            }
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

            if (SignHanger != null)
                SignHanger.Delete();

            if (Signpost != null)
                Signpost.Delete();

            if (Fixtures == null)
                return;

            for (int i = 0; i < Fixtures.Count; ++i)
            {
                Item item = Fixtures[i];

                if (item != null)
                    item.Delete();
            }

            Fixtures.Clear();
        }

        public override void OnLocationChange(Point3D oldLocation)
        {
            base.OnLocationChange(oldLocation);

            int x = Location.X - oldLocation.X;
            int y = Location.Y - oldLocation.Y;
            int z = Location.Z - oldLocation.Z;

            if (SignHanger != null)
                SignHanger.MoveToWorld(new Point3D(SignHanger.X + x, SignHanger.Y + y, SignHanger.Z + z), Map);

            if (Signpost != null)
                Signpost.MoveToWorld(new Point3D(Signpost.X + x, Signpost.Y + y, Signpost.Z + z), Map);

            if (Fixtures == null)
                return;

            for (int i = 0; i < Fixtures.Count; ++i)
            {
                Item item = Fixtures[i];

                if (Doors.Contains(item))
                    continue;

                item.MoveToWorld(new Point3D(item.X + x, item.Y + y, item.Z + z), Map);
            }
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            if (SignHanger != null)
                SignHanger.Map = Map;

            if (Signpost != null)
                Signpost.Map = Map;

            if (Fixtures == null)
                return;

            for (int i = 0; i < Fixtures.Count; ++i)
                Fixtures[i].Map = Map;
        }

        public void ClearFixtures(Mobile from)
        {
            if (Fixtures == null)
                return;

            for (int i = 0; i < Fixtures.Count; ++i)
            {
                Fixtures[i].Delete();
                Doors.Remove(Fixtures[i]);
            }

            Fixtures.Clear();
        }

        public void AddFixtures(Mobile from, MultiTileEntry[] list)
        {
            if (Fixtures == null)
                Fixtures = new List<Item>();

            for (int i = 0; i < list.Length; ++i)
            {
                MultiTileEntry mte = list[i];
                int itemID = mte.m_ItemID;

                if (itemID >= 0x181D && itemID < 0x1829)
                {
                    HouseTeleporter tp = new HouseTeleporter(itemID);

                    AddFixture(tp, mte);
                }
                else
                {
                    BaseDoor door = AddDoor(from, itemID, mte.m_OffsetX, mte.m_OffsetY, mte.m_OffsetZ);

                    if (door != null)
                    {
                        Fixtures.Add(door);
                    }
                }
            }

            for (int i = 0; i < Fixtures.Count; ++i)
            {
                Item fixture = Fixtures[i];

                if (fixture is HouseTeleporter)
                {
                    HouseTeleporter tp = (HouseTeleporter)fixture;

                    for (int j = 1; j <= Fixtures.Count; ++j)
                    {
                        HouseTeleporter check = Fixtures[(i + j) % Fixtures.Count] as HouseTeleporter;

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

                    for (int j = i + 1; j < Fixtures.Count; ++j)
                    {
                        BaseHouseDoor check = Fixtures[j] as BaseHouseDoor;

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
            Fixtures.Add(item);
            item.MoveToWorld(new Point3D(X + mte.m_OffsetX, Y + mte.m_OffsetY, Z + mte.m_OffsetZ), Map);
        }

        public static void GetFoundationGraphics(FoundationType type, out int east, out int south, out int post, out int corner)
        {
            switch (type)
            {
                default:
                case FoundationType.DarkWood:
                    corner = 0x0014;
                    east = 0x0015;
                    south = 0x0016;
                    post = 0x0017;
                    break;
                case FoundationType.LightWood:
                    corner = 0x00BD;
                    east = 0x00BE;
                    south = 0x00BF;
                    post = 0x00C0;
                    break;
                case FoundationType.Dungeon:
                    corner = 0x02FD;
                    east = 0x02FF;
                    south = 0x02FE;
                    post = 0x0300;
                    break;
                case FoundationType.Brick:
                    corner = 0x0041;
                    east = 0x0043;
                    south = 0x0042;
                    post = 0x0044;
                    break;
                case FoundationType.Stone:
                    corner = 0x0065;
                    east = 0x0064;
                    south = 0x0063;
                    post = 0x0066;
                    break;
                case FoundationType.ElvenGrey:
                    corner = 0x2DF7;
                    east = 0x2DF9;
                    south = 0x2DFA;
                    post = 0x2DF8;
                    break;
                case FoundationType.ElvenNatural:
                    corner = 0x2DFB;
                    east = 0x2DFD;
                    south = 0x2DFE;
                    post = 0x2DFC;
                    break;
                case FoundationType.Crystal:
                    corner = 0x3672;
                    east = 0x3671;
                    south = 0x3670;
                    post = 0x3673;
                    break;
                case FoundationType.Shadow:
                    corner = 0x3676;
                    east = 0x3675;
                    south = 0x3674;
                    post = 0x3677;
                    break;
                case FoundationType.SimpleMarble:
                    corner = 0x2BC7;
                    east = 0x2CEF;
                    south = 0x2CF0;
                    post = 0x2BC8;
                    break;
                case FoundationType.PlainMarble:
                    corner = 0x2DC3;
                    east = 0x2DCF;
                    south = 0x2DD0;
                    post = 0x2DC6;
                    break;
                case FoundationType.OrnateMarble:
                    corner = 0x2BAD;
                    east = 0x2BB9;
                    south = 0x2BBA;
                    post = 0x2BB0;
                    break;
                case FoundationType.GargishGreenMarble:
                    corner = 0x41A6;
                    east = 0x41A8;
                    south = 0x41A7;
                    post = 0x419E;
                    break;
                case FoundationType.GargishTwoToneStone:
                    corner = 0x415C;
                    east = 0x4166;
                    south = 0x4167;
                    post = 0x415F;
                    break;
                case FoundationType.Gothic:
                    corner = 0x4B08;
                    east = 0x4B00;
                    south = 0x4AFA;
                    post = 0x4B06;
                    break;
                case FoundationType.Brick1:
                    corner = 0x9ABE;
                    east = 0x9AC0;
                    south = 0x9ABF;
                    post = 0x9AC1;
                    break;
                case FoundationType.Brick2:
                    corner = 0x9BD0;
                    east = 0x9BD2;
                    south = 0x9BD1;
                    post = 0x9BD3;
                    break;
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

            ApplyFoundation(Type, mcl);

            for (int x = 1; x < mcl.Width; ++x)
                mcl.Add(0x751, x - xCenter, y - yCenter, 0);

            return mcl;
        }

        public override Rectangle2D[] Area
        {
            get
            {
                MultiComponentList mcl = Components;

                return new[] { new Rectangle2D(mcl.Min.X, mcl.Min.Y, mcl.Width, mcl.Height) };
            }
        }

        public override Point3D BaseBanLocation => new Point3D(Components.Min.X, Components.Height - 1 - Components.Center.Y, 0);

        public void CheckSignpost()
        {
            MultiComponentList mcl = Components;

            int x = mcl.Min.X;
            int y = mcl.Height - 2 - mcl.Center.Y;

            if (CheckWall(mcl, x, y))
            {
                if (Signpost != null)
                    Signpost.Delete();

                Signpost = null;
            }
            else if (Signpost == null)
            {
                Signpost = new Static(SignpostGraphic);
                Signpost.MoveToWorld(new Point3D(X + x, Y + y, Z + 7), Map);
            }
            else
            {
                Signpost.ItemID = SignpostGraphic;
                Signpost.MoveToWorld(new Point3D(X + x, Y + y, Z + 7), Map);
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
            SignpostGraphic = 9;

            Fixtures = new List<Item>();

            int x = Components.Min.X;
            int y = Components.Height - 1 - Components.Center.Y;

            SignHanger = new Static(0xB98);
            SignHanger.MoveToWorld(new Point3D(X + x, Y + y, Z + 7), Map);

            CheckSignpost();

            SetSign(x, y, 7);
        }

        public HouseFoundation(Serial serial)
            : base(serial)
        {
        }

        public override bool IsStairArea(IPoint3D p, out bool frontStairs)
        {
            if (p.Y >= Sign.Y)
            {
                frontStairs = true;
                return true;
            }

            frontStairs = false;
            return false;
        }

        public void BeginCustomize(Mobile m)
        {
            if (!m.CheckAlive())
            {
                return;
            }

            if (SpellHelper.CheckCombat(m))
            {
                m.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
                return;
            }

            RelocateEntities();

            foreach (Item item in GetItems())
            {
                item.Location = BanLocation;
            }

            foreach (Mobile mobile in GetMobiles())
            {
                if (mobile is Mannequin || mobile is Steward)
                {
                    Mannequin.ForceRedeed(mobile, this);
                }
                else if (mobile != m)
                {
                    mobile.Location = BanLocation;
                }
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
            writer.Write(5); // version

            writer.Write(Signpost);
            writer.Write(SignpostGraphic);

            writer.Write((int)Type);

            writer.Write(SignHanger);

            writer.Write(LastRevision);
            writer.Write(Fixtures, true);

            CurrentState.Serialize(writer);
            DesignState.Serialize(writer);
            BackupState.Serialize(writer);

            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            switch (version)
            {
                case 5:
                case 4:
                    {
                        Signpost = reader.ReadItem();
                        SignpostGraphic = reader.ReadInt();

                        goto case 3;
                    }
                case 3:
                    {
                        Type = (FoundationType)reader.ReadInt();

                        goto case 2;
                    }
                case 2:
                    {
                        SignHanger = reader.ReadItem();

                        goto case 1;
                    }
                case 1:
                case 0:
                    {
                        if (version < 3)
                            Type = FoundationType.Stone;

                        if (version < 4)
                            SignpostGraphic = 9;

                        LastRevision = reader.ReadInt();
                        Fixtures = reader.ReadStrongItemList();

                        m_Current = new DesignState(this, reader);
                        m_Design = new DesignState(this, reader);
                        m_Backup = new DesignState(this, reader);

                        break;
                    }
            }

            if (LastRevision == 0)
            {
                OnPlacement();
            }

            base.Deserialize(reader);
        }

        public bool IsHiddenToCustomizer(Mobile m, Item item)
        {
            // Always visible if *this* house, equipped, or contained.
            if (item == this || item.Parent != null)
                return false;

            // Always hidden if uneditable fixture.
            if (item == Signpost || item == SignHanger || item == Sign || IsFixture(item))
                return true;

            // Always hidden if *not* contained within *this* house region.
            // Note: Will hide other houses and their contents.
            if (Region != null && !Region.Contains(item.Location))
                return true;

            return false;
        }

        public static void Initialize()
        {
            EventSink.MultiDesign += QueryDesignDetails;
            PacketHandlers.RegisterExtended(0x1E, true, QueryDesignDetails);

            PacketHandlers.RegisterEncoded(0x02, true, Designer_Backup);
            PacketHandlers.RegisterEncoded(0x03, true, Designer_Restore);
            PacketHandlers.RegisterEncoded(0x04, true, Designer_Commit);
            PacketHandlers.RegisterEncoded(0x05, true, Designer_Delete);
            PacketHandlers.RegisterEncoded(0x06, true, Designer_Build);
            PacketHandlers.RegisterEncoded(0x0A, true, Designer_Action); // WTF does this do?
            PacketHandlers.RegisterEncoded(0x0C, true, Designer_Close);
            PacketHandlers.RegisterEncoded(0x0D, true, Designer_Stairs);
            PacketHandlers.RegisterEncoded(0x0E, true, Designer_Sync);
            PacketHandlers.RegisterEncoded(0x0F, true, Designer_Action); // WTF does this do?
            PacketHandlers.RegisterEncoded(0x10, true, Designer_Clear);
            PacketHandlers.RegisterEncoded(0x12, true, Designer_Level);

            PacketHandlers.RegisterEncoded(0x13, true, Designer_Roof); // Samurai Empire roof
            PacketHandlers.RegisterEncoded(0x14, true, Designer_RoofDelete); // Samurai Empire roof

            PacketHandlers.RegisterEncoded(0x1A, true, Designer_Revert);

            EventSink.Speech += EventSink_Speech;
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

        public static void Designer_Action(NetState state, IEntity e, EncodedReader pvSrc)
        {
            //pvSrc.Trace(state);
            // TODO: What does this do?
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

        #region Enhanced Client
        /// <summary>
        /// for EC, the details cache gets fucked up when prior to MoveToWorld (null map).
        /// So, we clear the cache on the current design state AFTER MoveToWorld, which at this point, is the only
        /// state. We still need to send details, regardless of client type, so the cache is set correctly for EC
        /// </summary>
        public void OnPlacement()
        {
            if (Deleted)
                return;

            DesignState designState = CurrentState;
            designState.OnRevised();

            Delta(ItemDelta.Update);
        }
        #endregion

        public void EndConfirmCommit(Mobile from)
        {
            int oldPrice = Price;
            int newPrice = oldPrice + CustomizationCost + ((DesignState.Components.List.Length - (CurrentState.Components.List.Length + CurrentState.Fixtures.Length)) * 500);
            int cost = newPrice - oldPrice;


            if (!Deleted)
            {
                // Temporary Fix. We should be booting a client out of customization mode in the delete handler.
                if (from.AccessLevel >= AccessLevel.GameMaster && cost != 0)
                {
                    from.SendMessage("{0} gold would have been {1} your bank if you were not a GM.", cost.ToString(), ((cost > 0) ? "withdrawn from" : "deposited into"));
                }
                else
                {
                    if (cost > 0)
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
                    else if (cost < 0)
                    {
                        if (Banker.Deposit(from, -cost))
                            from.SendLocalizedMessage(1060397, (-cost).ToString()); // ~1_AMOUNT~ gold has been deposited into your bank box.
                        else
                            return;
                    }
                }
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
            Price = newPrice - CustomizationCost;

            // Remove design context
            DesignContext.Remove(from);

            // Notify the client that customization has ended
            from.Send(new EndHouseCustomization(this));

            // Notify the core that the foundation has changed and should be resent to all clients
            Delta(ItemDelta.Update);
            ProcessDelta();
            CurrentState.SendDetailedInfoTo(from.NetState, false);

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
                int newPrice = oldPrice + context.Foundation.CustomizationCost + ((context.Foundation.DesignState.Components.List.Length - (context.Foundation.CurrentState.Components.List.Length + context.Foundation.Fixtures.Count)) * 500);
                int bankBalance = Banker.GetBalance(from);

                from.SendGump(new ConfirmCommitGump(from, context.Foundation, bankBalance, oldPrice, newPrice));
            }
        }

        public int MaxLevels
        {
            get
            {
                MultiComponentList mcl = Components;

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

        public static ComponentVerification Verification => m_Verification ?? (m_Verification = new ComponentVerification());

        public static bool ValidPiece(int itemID)
        {
            return ValidPiece(itemID, false);
        }

        public static bool ValidPiece(int itemID, bool roof)
        {
            itemID &= TileData.MaxItemValue;

            if (!roof && (TileData.ItemTable[itemID].Flags & TileFlag.Roof) != 0)
                return false;
            if (roof && (TileData.ItemTable[itemID].Flags & TileFlag.Roof) == 0)
                return false;

            return Verification.IsItemValid(itemID);
        }

        /* Stair block IDs
        * (sorted ascending)
        */
        private static readonly int[] m_BlockIDs =
        {
            0x3EE, 0x709, 0x71E, 0x721,
            0x738, 0x750, 0x76C, 0x788,
            0x7A3, 0x7BA, 0x35D2, 0x3609,
            0x4317, 0x4318, 0x4B07, 0x7807,
            0x9AEA, 0x9B4F
        };

        /* Stair sequence IDs
        * (sorted ascending)
        * Use this for stairs in the proper N,W,S,E sequence
        */
        private static readonly int[] m_StairSeqs =
        {
            0x3EF, 0x70A, 0x722, 0x739,
            0x751, 0x76D, 0x789, 0x7A4,
            0x9AEB, 0x9B50
        };

        /* Other stair IDs
        * Listed in order: north, west, south, east
        * Use this for stairs not in the proper sequence
        */
        private static readonly int[] m_StairIDs =
        {
            0x71F,  0x736,  0x737,  0x749,
            0x35D4, 0x35D3, 0x35D6, 0x35D5,
            0x360B, 0x360A, 0x360D, 0x360C,
            0x4360, 0x435E, 0x435F, 0x4361,
            0x435C, 0x435A, 0x435B, 0x435D,
            0x4364, 0x4362, 0x4363, 0x4365,
            0x4B05, 0x4B04, 0x4B34, 0x4B33,
            0x7809, 0x7808, 0x780A, 0x780B,
            0x7BB,  0x7BC, -1,      -1
        };

        private static readonly int[] m_CornerIDs =
        {
            0x749, 0x74A, 0x74B, 0x74C,
            0x4366, 0x4367, 0x4368, 0x4369,
            0x436A, 0x436B, 0x436C, 0x436D,
            0x4B01, 0x4B02, 0x4B03, 0x4B32,
            0x780C, 0x708D, 0x708E, 0x708F
        };

        public static bool IsStairBlock(int id)
        {
            int delta = -1;

            for (int i = 0; delta < 0 && i < m_BlockIDs.Length; ++i)
                delta = (m_BlockIDs[i] - id);

            return (delta == 0);
        }

        public static bool IsStair(int id)
        {
            return m_StairSeqs.Any(seq => id >= seq && id <= seq + 8) || m_StairIDs.Any(stairID => stairID == id) || m_CornerIDs.Any(cornerID => cornerID == id);
        }

        public static bool IsStair(int id, ref int dir)
        {
            //dir n=0 w=1 s=2 e=3
            int delta = -4;

            for (int i = 0; delta < -3 && i < m_StairSeqs.Length; ++i)
                delta = (m_StairSeqs[i] - id);

            if (delta >= -3 && delta <= 0)
            {
                dir = -delta;
                return true;
            }

            for (int i = 0; i < m_StairIDs.Length; ++i)
            {
                if (m_StairIDs[i] == id)
                {
                    dir = i % 4;
                    return true;
                }
            }

            return false;
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
                        hasBaseFloor = (tiles[j].Z == 7 && tiles[j].ID != 1);

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

                if (z == 0 && ax >= 0 && ax < mcl.Width && ay >= 0 && ay < (mcl.Height - 1))
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
                        hasBaseFloor = (tiles[i].Z == 7 && tiles[i].ID != 1);

                    if (!hasBaseFloor)
                    {
                        // Replace with a dirt tile
                        mcl.Add(0x31F4, x, y, 7);
                    }
                }

                // Update revision
                design.OnRevised();

                // Resend design state
                if (deleteStairs)
                    design.SendDetailedInfoTo(state);
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

                if (itemID >= 7668 && itemID <= 7675)
                {
                    int idOffset = itemID <= 7671 ? 101 : 0;
                    int[][] list = { };

                    switch (itemID)
                    {
                        case 7668: list = _StairsSouth; break;
                        case 7669: list = _StairsWest; break;
                        case 7670: list = _StairsNorth; break;
                        case 7671: list = _StairsEast; break;
                        case 7672: list = _StairsSouth; break;
                        case 7673: list = _StairsWest; break;
                        case 7674: list = _StairsNorth; break;
                        case 7675: list = _StairsEast; break;
                    }

                    for (int i = 0; i < list.Length; i++)
                    {
                        if (list[i][0] != 1)
                        {
                            mcl.Add(list[i][0] - idOffset, x + list[i][1], y + list[i][2], z + list[i][3]);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < stairs.List.Length; ++i)
                    {
                        MultiTileEntry entry = stairs.List[i];

                        if (entry.m_ItemID != 1)
                        {
                            mcl.Add(entry.m_ItemID, x + entry.m_OffsetX, y + entry.m_OffsetY, z + entry.m_OffsetZ);
                        }
                    }
                }

                // Update revision
                design.OnRevised();
            }
        }

        #region TOL Stair Components cannot be found in MultiData
        private static readonly int[][] _StairsSouth =
        {
            new[] { 0x9B4F, 0,  -3, 0  },
            new[] { 0x9B4F, 0,  -3, 5  },
            new[] { 0x9B4F, 0,  -3, 10 },
            new[] { 0x9B50, 0,  -3, 15 },
            new[] { 0x9B4F, 0,  -2, 0  },
            new[] { 0x9B4F, 0,  -2, 5  },
            new[] { 0x9B50, 0,  -2, 10 },
            new[] { 0x9B4F, 0,  -1, 0  },
            new[] { 0x9B50, 0,  -1, 5  },
            new[] { 0x0001, 0 , 0, 0   },
            new[] { 0x9B50, 0 , 0, 0   }
        };

        private static readonly int[][] _StairsWest =
        {
            new[] { 0x0001, 0,  0, 0   },
            new[] { 0x9B53, 0,  0, 0   },
            new[] { 0x9B4F, 1,  0, 0   },
            new[] { 0x9B53, 1,  0, 5   },
            new[] { 0x9B4F, 2,  0, 0   },
            new[] { 0x9B4F, 2,  0, 5   },
            new[] { 0x9B53, 2,  0, 10  },
            new[] { 0x9B4F, 3,  0, 0   },
            new[] { 0x9B4F, 3,  0, 5   },
            new[] { 0x9B4F, 3, 0, 10   },
            new[] { 0x9B53, 3 , 0, 15  }
        };

        private static readonly int[][] _StairsNorth =
        {
            new[] { 0x0001, 0,  0, 0  },
            new[] { 0x9B52, 0,  0, 0  },
            new[] { 0x9B4F, 0,  1, 0  },
            new[] { 0x9B52, 0,  1, 5  },
            new[] { 0x9B4F, 0,  2, 0  },
            new[] { 0x9B4F, 0,  2, 5  },
            new[] { 0x9B52, 0,  2, 10 },
            new[] { 0x9B4F, 0,  3, 0  },
            new[] { 0x9B4F, 0,  3, 5  },
            new[] { 0x9B4F, 0,  3, 10 },
            new[] { 0x9B52, 0,  3, 15 }
        };

        private static readonly int[][] _StairsEast =
        {
            new[] { 0x9B4F, -3, 0, 0  },
            new[] { 0x9B4F, -3, 0, 5  },
            new[] { 0x9B4F, -3, 0, 10 },
            new[] { 0x9B51, -3, 0, 15 },
            new[] { 0x9B4F, -2, 0, 0  },
            new[] { 0x9B4F, -2, 0, 5  },
            new[] { 0x9B51, -2, 0, 10 },
            new[] { 0x9B4F, -1, 0, 0  },
            new[] { 0x9B51, -1, 0, 5  },
            new[] { 0x0001, 0, 0, 0   },
            new[] { 0x9B51, 0, 0, 0   }
        };
        #endregion

        private static void TraceValidity(NetState state, int itemID)
        {
            try
            {
                using (StreamWriter op = new StreamWriter("comp_val.log", true))
                    op.WriteLine("{0}\t{1}\tInvalid ItemID 0x{2:X4}", state, state.Mobile, itemID);
            }
            catch (Exception e)
            {
                Diagnostics.ExceptionLogging.LogException(e);
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
                *  - Refresh client with current visible design state
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
                context.Foundation.CurrentState.SendDetailedInfoTo(state, false);

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
                *
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
            BaseMulti multi = World.FindItem(pvSrc.ReadInt32()) as BaseMulti;

            if (multi != null)
            {
                EventSink.InvokeMultiDesignQuery(new MultiDesignQueryEventArgs(state, multi));
            }
        }

        public static void QueryDesignDetails(MultiDesignQueryEventArgs e)
        {
            QueryDesignDetails(e.State, e.Multi);
        }

        public static void QueryDesignDetails(NetState state, BaseMulti multi)
        {
            Mobile from = state.Mobile;
            DesignContext context = DesignContext.Find(from);

            HouseFoundation foundation = multi as HouseFoundation;

            if (foundation != null && from.Map == foundation.Map)
            {
                int range = foundation.GetUpdateRange(from);

                if (Utility.InRange(from.Location, foundation.GetWorldLocation(), range) && from.CanSee(foundation))
                {
                    DesignState stateToSend;

                    if (context != null && context.Foundation == foundation)
                        stateToSend = foundation.DesignState;
                    else
                        stateToSend = foundation.CurrentState;

                    stateToSend.SendDetailedInfoTo(state);
                }
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

    public interface IDesignState
    {
        Packet PacketCache { get; set; }
        int Revision { get; set; }
        MultiComponentList Components { get; set; }
        MultiTileEntry[] Fixtures { get; set; }
    }

    public class DesignState : IDesignState
    {
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

        public HouseFoundation Foundation { get; }
        public MultiComponentList Components { get; set; }
        public MultiTileEntry[] Fixtures { get; set; }
        public int Revision { get; set; }

        public DesignState(HouseFoundation foundation, MultiComponentList components)
        {
            Foundation = foundation;
            Components = components;
            Fixtures = new MultiTileEntry[0];
        }

        public DesignState(DesignState toCopy)
        {
            Foundation = toCopy.Foundation;
            Components = new MultiComponentList(toCopy.Components);
            Revision = toCopy.Revision;
            Fixtures = new MultiTileEntry[toCopy.Fixtures.Length];

            for (int i = 0; i < Fixtures.Length; ++i)
                Fixtures[i] = toCopy.Fixtures[i];
        }

        public DesignState(HouseFoundation foundation, GenericReader reader)
        {
            Foundation = foundation;

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                case 0:
                    {
                        Components = new MultiComponentList(reader);

                        int length = reader.ReadInt();

                        Fixtures = new MultiTileEntry[length];

                        for (int i = 0; i < length; ++i)
                        {
                            Fixtures[i].m_ItemID = reader.ReadUShort();
                            Fixtures[i].m_OffsetX = reader.ReadShort();
                            Fixtures[i].m_OffsetY = reader.ReadShort();
                            Fixtures[i].m_OffsetZ = reader.ReadShort();

                            if (version > 0)
                                Fixtures[i].m_Flags = (TileFlag)reader.ReadULong();
                            else
                                Fixtures[i].m_Flags = (TileFlag)reader.ReadUInt();
                        }

                        Revision = reader.ReadInt();

                        break;
                    }
            }
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write(1); // version

            Components.Serialize(writer);

            writer.Write(Fixtures.Length);

            for (int i = 0; i < Fixtures.Length; ++i)
            {
                MultiTileEntry ent = Fixtures[i];

                writer.Write(ent.m_ItemID);
                writer.Write(ent.m_OffsetX);
                writer.Write(ent.m_OffsetY);
                writer.Write(ent.m_OffsetZ);

                writer.Write((ulong)ent.m_Flags);
            }

            writer.Write(Revision);
        }

        public void OnRevised()
        {
            lock (this)
            {
                Revision = ++Foundation.LastRevision;

                if (m_PacketCache != null)
                    m_PacketCache.Release();

                m_PacketCache = null;
            }
        }

        public void SendGeneralInfoTo(NetState state)
        {
            if (state != null)
                state.Send(new DesignStateGeneral(Foundation, this));
        }

        public void SendDetailedInfoTo(NetState state, bool response = true)
        {
            if (state != null)
            {
                lock (this)
                {
                    if (m_PacketCache == null)
                        DesignStateDetailed.SendDetails(state, Foundation, this, response);
                    else
                        state.Send(m_PacketCache);
                }
            }
        }

        public void FreezeFixtures()
        {
            OnRevised();

            for (int i = 0; i < Fixtures.Length; ++i)
            {
                MultiTileEntry mte = Fixtures[i];

                Components.Add(mte.m_ItemID, mte.m_OffsetX, mte.m_OffsetY, mte.m_OffsetZ);
            }

            Fixtures = new MultiTileEntry[0];
        }

        public void MeltFixtures()
        {
            OnRevised();

            MultiTileEntry[] list = Components.List;
            int length = 0;

            for (int i = list.Length - 1; i >= 0; --i)
            {
                MultiTileEntry mte = list[i];

                if (IsFixture(mte.m_ItemID))
                    ++length;
            }

            Fixtures = new MultiTileEntry[length];

            for (int i = list.Length - 1; i >= 0; --i)
            {
                MultiTileEntry mte = list[i];

                if (IsFixture(mte.m_ItemID))
                {
                    Fixtures[--length] = mte;
                    Components.Remove(mte.m_ItemID, mte.m_OffsetX, mte.m_OffsetY, mte.m_OffsetZ);
                }
            }
        }

        public static bool IsFixture(int itemID)
        {
            if (itemID >= 0x675 && itemID < 0x6F5)
                return true;
            if (itemID >= 0x314 && itemID < 0x364)
                return true;
            if (itemID >= 0x824 && itemID < 0x834)
                return true;
            if (itemID >= 0x839 && itemID < 0x849)
                return true;
            if (itemID >= 0x84C && itemID < 0x85C)
                return true;
            if (itemID >= 0x866 && itemID < 0x876)
                return true;
            if (itemID >= 0x0E8 && itemID < 0x0F8)
                return true;
            if (itemID >= 0x1FED && itemID < 0x1FFD)
                return true;
            if (itemID >= 0x181D && itemID < 0x1829)
                return true;
            if (itemID >= 0x241F && itemID < 0x2421)
                return true;
            if (itemID >= 0x2423 && itemID < 0x2425)
                return true;
            if (itemID >= 0x2A05 && itemID < 0x2A1D)
                return true;
            if (itemID >= 0x319C && itemID < 0x31B0)
                return true;
            // ML doors
            if (itemID == 0x2D46 || itemID == 0x2D48 || itemID == 0x2FE2 || itemID == 0x2FE4)
                return true;
            if (itemID >= 0x2D63 && itemID < 0x2D70)
                return true;
            if (itemID >= 0x319C && itemID < 0x31AF)
                return true;
            if (itemID >= 0x367B && itemID < 0x369B)
                return true;
            // SA doors
            if (itemID >= 0x409B && itemID < 0x40A3)
                return true;
            if (itemID >= 0x410C && itemID < 0x4114)
                return true;
            if (itemID >= 0x41C2 && itemID < 0x41CA)
                return true;
            if (itemID >= 0x41CF && itemID < 0x41D7)
                return true;
            if (itemID >= 0x436E && itemID < 0x437E)
                return true;
            if (itemID >= 0x46DD && itemID < 0x46E5)
                return true;
            if (itemID >= 0x4D22 && itemID < 0x4D2A)
                return true;
            if (itemID >= 0x50C8 && itemID < 0x50D8)
                return true;
            if (itemID >= 0x5142 && itemID < 0x514A)
                return true;
            // TOL doors
            if (itemID >= 0x9AD7 && itemID < 0x9AE7)
                return true;
            if (itemID >= 0x9B3C && itemID < 0x9B4C)
                return true;

            return false;
        }
    }

    public class ConfirmCommitGump : Gump
    {
        private readonly HouseFoundation m_Foundation;

        public ConfirmCommitGump(Mobile from, HouseFoundation foundation, int bankBalance, int oldPrice, int newPrice)
            : base(50, 50)
        {
            m_Foundation = foundation;

            AddPage(0);

            AddBackground(0, 0, 320, 320, 5054);

            AddImageTiled(10, 10, 300, 20, 2624);
            AddImageTiled(10, 40, 300, 240, 2624);
            AddImageTiled(10, 290, 300, 20, 2624);

            AddAlphaRegion(10, 10, 300, 300);

            AddHtmlLocalized(10, 10, 300, 20, 1062060, 32736, false, false); // <CENTER>COMMIT DESIGN</CENTER>

            AddHtmlLocalized(10, 40, 300, 140, (newPrice - oldPrice) <= bankBalance ? 1061898 : 1061903, 1023, false, true);

            AddHtmlLocalized(10, 190, 150, 20, 1061902, 32736, false, false); // Bank Balance:
            AddLabel(170, 190, 55, bankBalance.ToString());

            AddHtmlLocalized(10, 215, 150, 20, 1061899, 1023, false, false); // Old Value:
            AddLabel(170, 215, 90, oldPrice.ToString());

            AddHtmlLocalized(10, 235, 150, 20, 1061900, 1023, false, false); // Cost To Commit:
            AddLabel(170, 235, 90, newPrice.ToString());

            if (newPrice - oldPrice < 0)
            {
                AddHtmlLocalized(10, 260, 150, 20, 1062059, 992, false, false); // Your Refund:
                AddLabel(170, 260, 70, (oldPrice - newPrice).ToString());
            }
            else
            {
                AddHtmlLocalized(10, 260, 150, 20, 1061901, 31744, false, false); // Your Cost:
                AddLabel(170, 260, 40, (newPrice - oldPrice).ToString());
            }

            AddButton(10, 290, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(45, 290, 55, 20, 1011036, 32767, false, false); // OKAY

            AddButton(170, 290, 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(195, 290, 55, 20, 1011012, 32767, false, false); // CANCEL
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1)
                m_Foundation.EndConfirmCommit(sender.Mobile);
        }
    }

    public class DesignContext
    {
        public HouseFoundation Foundation { get; }

        public int Level { get; set; }

        public int MaxLevels => Foundation.MaxLevels;

        public DesignContext(HouseFoundation foundation)
        {
            Foundation = foundation;
            Level = 1;
        }

        private static readonly Dictionary<Mobile, DesignContext> Table = new Dictionary<Mobile, DesignContext>();

        public static DesignContext Find(Mobile from)
        {
            if (from == null)
                return null;

            DesignContext d;
            Table.TryGetValue(from, out d);

            return d;
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
                return;

            DesignContext c = new DesignContext(foundation);

            Table[from] = c;

            if (from is PlayerMobile)
                ((PlayerMobile)from).DesignContext = c;

            foundation.Customizer = from;

            from.Hidden = true;
            from.Location = new Point3D(foundation.X, foundation.Y, foundation.Z + 7);

            NetState state = from.NetState;

            if (state == null)
                return;

            foundation.Fixtures.ForEach(x => state.Send(x.RemovePacket));

            if (foundation.Signpost != null)
                state.Send(foundation.Signpost.RemovePacket);

            if (foundation.SignHanger != null)
                state.Send(foundation.SignHanger.RemovePacket);

            if (foundation.Sign != null)
                state.Send(foundation.Sign.RemovePacket);
        }

        public static void Remove(Mobile from)
        {
            DesignContext context = Find(from);

            if (context == null)
                return;

            Table.Remove(from);

            if (from is PlayerMobile)
                ((PlayerMobile)from).DesignContext = null;

            context.Foundation.Customizer = null;

            NetState state = from.NetState;

            if (state == null)
                return;

            context.Foundation.Fixtures.ForEach(x => x.SendInfoTo(state));

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
            m_Stream.Write(house.Serial);
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
            m_Stream.Write(house.Serial);
            m_Stream.Write((byte)0x05);
            m_Stream.Write((ushort)0x0000);
            m_Stream.Write((ushort)0xFFFF);
            m_Stream.Write((ushort)0xFFFF);
            m_Stream.Write((byte)0xFF);
        }
    }

    public sealed class DesignStateGeneral : Packet
    {
        public DesignStateGeneral(BaseMulti multi, IDesignState state)
            : base(0xBF)
        {
            EnsureCapacity(13);

            m_Stream.Write((short)0x1D);
            m_Stream.Write(multi.Serial);
            m_Stream.Write(state.Revision);
        }
    }

    public sealed class DesignStateDetailed : Packet
    {
        public const int MaxItemsPerStairBuffer = 750;

        private static readonly BufferPool m_PlaneBufferPool = new BufferPool("Housing Plane Buffers", 9, 0x2000);
        private static readonly BufferPool m_StairBufferPool = new BufferPool("Housing Stair Buffers", 6, MaxItemsPerStairBuffer * 5);
        private static readonly BufferPool m_DeflatedBufferPool = new BufferPool("Housing Deflated Buffers", 1, 0x2000);

        private readonly bool[] m_PlaneUsed = new bool[9];
        private readonly byte[] m_PrimBuffer = new byte[4];

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

        public DesignStateDetailed(int serial, int revision, bool response, int xMin, int yMin, int xMax, int yMax, MultiTileEntry[] tiles)
            : base(0xD8)
        {
            EnsureCapacity(17 + (tiles.Length * 5));

            Write((byte)0x03); // Compression Type
            Write((byte)(response ? 0x01 : 0x00)); // Enable Response (0x00 or 0x01)
            Write(serial); // Serial
            Write(revision); // Revision Number
            Write((short)tiles.Length); // Tile Length
            Write((short)0); // Buffer length : reserved
            Write((byte)0); // Plane count : reserved

            int totalLength = 1; // includes plane count

            int width = (xMax - xMin) + 1;
            int height = (yMax - yMin) + 1;

            var mPlaneBuffers = new byte[9][];

            lock (m_PlaneBufferPool)
                for (int i = 0; i < mPlaneBuffers.Length; ++i)
                    mPlaneBuffers[i] = m_PlaneBufferPool.AcquireBuffer();

            var mStairBuffers = new byte[6][];

            lock (m_StairBufferPool)
                for (int i = 0; i < mStairBuffers.Length; ++i)
                    mStairBuffers[i] = m_StairBufferPool.AcquireBuffer();

            Clear(mPlaneBuffers[0], width * height * 2);

            for (int i = 0; i < 4; ++i)
            {
                Clear(mPlaneBuffers[1 + i], (width - 1) * (height - 2) * 2);
                Clear(mPlaneBuffers[5 + i], width * (height - 1) * 2);
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
                            byte[] stairBuffer = mStairBuffers[stairBufferIndex];

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
                    byte[] stairBuffer = mStairBuffers[stairBufferIndex];

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
                    mPlaneBuffers[plane][index] = (byte)(mte.m_ItemID >> 8);
                    mPlaneBuffers[plane][index + 1] = (byte)mte.m_ItemID;
                }
            }

            int planeCount = 0;

            byte[] m_DeflatedBuffer = null;
            lock (m_DeflatedBufferPool)
                m_DeflatedBuffer = m_DeflatedBufferPool.AcquireBuffer();

            for (int i = 0; i < mPlaneBuffers.Length; ++i)
            {
                if (!m_PlaneUsed[i])
                {
                    m_PlaneBufferPool.ReleaseBuffer(mPlaneBuffers[i]);
                    continue;
                }

                ++planeCount;

                int size = 0;

                if (i == 0)
                    size = width * height * 2;
                else if (i < 5)
                    size = (width - 1) * (height - 2) * 2;
                else
                    size = width * (height - 1) * 2;

                byte[] inflatedBuffer = mPlaneBuffers[i];

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
                    count = MaxItemsPerStairBuffer;

                int size = count * 5;

                byte[] inflatedBuffer = mStairBuffers[i];

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
                for (int i = 0; i < mStairBuffers.Length; ++i)
                    m_StairBufferPool.ReleaseBuffer(mStairBuffers[i]);

            lock (m_DeflatedBufferPool)
                m_DeflatedBufferPool.ReleaseBuffer(m_DeflatedBuffer);

            m_Stream.Seek(15, SeekOrigin.Begin);

            Write((short)totalLength); // Buffer length
            Write((byte)planeCount); // Plane count
        }

        private class SendQueueEntry
        {
            public NetState State;
            public int Serial, Revision;
            public int xMin, yMin, xMax, yMax;
            public IDesignState Root;
            public MultiTileEntry[] Tiles;
            public bool EnableResponse;

            public SendQueueEntry(NetState ns, BaseMulti multi, IDesignState state, bool response)
            {
                State = ns;
                Serial = multi.Serial;
                Revision = state.Revision;
                Root = state;
                EnableResponse = response;

                MultiComponentList mcl = state.Components;

                xMin = mcl.Min.X;
                yMin = mcl.Min.Y;
                xMax = mcl.Max.X;
                yMax = mcl.Max.Y;

                Tiles = mcl.List;
            }
        }

        private static readonly Queue<SendQueueEntry> m_SendQueue;
        private static readonly object m_SendQueueSyncRoot;
        private static readonly AutoResetEvent m_Sync;

        static DesignStateDetailed()
        {
            m_SendQueue = new Queue<SendQueueEntry>();
            m_SendQueueSyncRoot = ((ICollection)m_SendQueue).SyncRoot;
            m_Sync = new AutoResetEvent(false);

            var mThread = new Thread(CompressionThread)
            {
                Name = "Housing Compression Thread"
            };
            mThread.Start();
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

                        lock (sqe.Root)
                            p = sqe.Root.PacketCache;

                        if (p == null)
                        {
                            p = new DesignStateDetailed(sqe.Serial, sqe.Revision, sqe.EnableResponse, sqe.xMin, sqe.yMin, sqe.xMax, sqe.yMax, sqe.Tiles);
                            p.SetStatic();

                            lock (sqe.Root)
                            {
                                if (sqe.Revision == sqe.Root.Revision)
                                    sqe.Root.PacketCache = p;
                            }
                        }

                        sqe.State.Send(p);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);

                        try
                        {
                            using (StreamWriter op = new StreamWriter("dsd_exceptions.txt", true))
                                op.WriteLine(e);
                        }
                        catch (Exception ex)
                        {
                            Diagnostics.ExceptionLogging.LogException(ex);
                        }
                    }
                    finally
                    {
                        lock (m_SendQueueSyncRoot)
                            count = m_SendQueue.Count;
                    }
                }
            }
        }

        public static void SendDetails(NetState ns, BaseMulti multi, IDesignState state, bool response)
        {
            lock (m_SendQueueSyncRoot)
                m_SendQueue.Enqueue(new SendQueueEntry(ns, multi, state, response));
            m_Sync.Set();
        }
    }
}
