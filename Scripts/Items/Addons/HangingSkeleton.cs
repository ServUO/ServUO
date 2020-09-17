using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public class HangingSkeleton : Item, IAddon, IRewardItem
    {
        private bool m_IsRewardItem;
        [Constructable]
        public HangingSkeleton()
            : this(0x1596)
        {
        }

        [Constructable]
        public HangingSkeleton(int itemID)
            : base(itemID)
        {
            LootType = LootType.Blessed;
            Movable = false;
        }

        public HangingSkeleton(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties => true;

        public Item Deed
        {
            get
            {
                HangingSkeletonDeed deed = new HangingSkeletonDeed
                {
                    IsRewardItem = m_IsRewardItem
                };

                return deed;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return m_IsRewardItem;
            }
            set
            {
                m_IsRewardItem = value;
                InvalidateProperties();
            }
        }
        public bool FacingSouth
        {
            get
            {
                if (ItemID == 0x1A03 || ItemID == 0x1A05 || ItemID == 0x1A09 ||
                    ItemID == 0x1B1E || ItemID == 0x1B7F)
                    return true;

                return false;
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_IsRewardItem)
                list.Add(1076220); // 4th Year Veteran Reward
        }

        void IChopable.OnChop(Mobile user)
        {
            OnDoubleClick(user);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(Location, 3))
            {
                BaseHouse house = BaseHouse.FindHouseAt(this);

                if (house != null && house.IsOwner(from))
                {
                    from.CloseGump(typeof(RewardDemolitionGump));
                    from.SendGump(new RewardDemolitionGump(this, 1049783)); // Do you wish to re-deed this decoration?
                }
                else
                    from.SendLocalizedMessage(1049784); // You can only re-deed this decoration if you are the house owner or originally placed the decoration.
            }
            else
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write(m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_IsRewardItem = reader.ReadBool();
        }

        public bool CouldFit(IPoint3D p, Map map)
        {
            if (map == null || !map.CanFit(p.X, p.Y, p.Z, ItemData.Height))
                return false;

            if (FacingSouth)
                return BaseAddon.IsWall(p.X, p.Y - 1, p.Z, map); // north wall
            else
                return BaseAddon.IsWall(p.X - 1, p.Y, p.Z, map); // west wall
        }
    }

    public class HangingSkeletonDeed : Item, IRewardItem
    {
        private bool m_IsRewardItem;
        [Constructable]
        public HangingSkeletonDeed()
            : base(0x14F0)
        {
            LootType = LootType.Blessed;
            Weight = 1.0;
        }

        public HangingSkeletonDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1049772;// deed for a hanging skeleton decoration
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return m_IsRewardItem;
            }
            set
            {
                m_IsRewardItem = value;
                InvalidateProperties();
            }
        }
        public static int GetWestItemID(int south)
        {
            switch (south)
            {
                case 0x1B1E:
                    return 0x1B1D;
                case 0x1B7F:
                    return 0x1B7C;
                default:
                    return south + 1;
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_IsRewardItem)
                list.Add(1076220); // 4th Year Veteran Reward
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
                return;

            if (IsChildOf(from.Backpack))
            {
                BaseHouse house = BaseHouse.FindHouseAt(from);

                if (house != null && house.IsOwner(from))
                {
                    from.CloseGump(typeof(InternalGump));
                    from.SendGump(new InternalGump(this));
                }
                else
                    from.SendLocalizedMessage(502092); // You must be in your house to do this.
            }
            else
                from.SendLocalizedMessage(1042038); // You must have the object in your backpack to use it.          	
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.Write(m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            m_IsRewardItem = reader.ReadBool();
        }

        private class InternalGump : Gump
        {
            private readonly HangingSkeletonDeed m_Skeleton;
            public InternalGump(HangingSkeletonDeed skeleton)
                : base(100, 200)
            {
                m_Skeleton = skeleton;

                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                AddPage(0);

                AddBackground(25, 0, 500, 230, 0xA28);

                AddPage(1);

                AddItem(130, 70, 0x1A03);
                AddButton(150, 50, 0x845, 0x846, 0x1A03, GumpButtonType.Reply, 0);

                AddItem(190, 70, 0x1A05);
                AddButton(210, 50, 0x845, 0x846, 0x1A05, GumpButtonType.Reply, 0);

                AddItem(250, 70, 0x1A09);
                AddButton(270, 50, 0x845, 0x846, 0x1A09, GumpButtonType.Reply, 0);

                AddItem(310, 70, 0x1B1E);
                AddButton(330, 50, 0x845, 0x846, 0x1B1E, GumpButtonType.Reply, 0);

                AddItem(370, 70, 0x1B7F);
                AddButton(390, 50, 0x845, 0x846, 0x1B7F, GumpButtonType.Reply, 0);
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (m_Skeleton == null || m_Skeleton.Deleted)
                    return;

                Mobile m = sender.Mobile;

                if (info.ButtonID == 0x1A03 || info.ButtonID == 0x1A05 || info.ButtonID == 0x1A09 ||
                    info.ButtonID == 0x1B1E || info.ButtonID == 0x1B7F)
                {
                    m.SendLocalizedMessage(1049780); // Where would you like to place this decoration?
                    m.Target = new InternalTarget(m_Skeleton, info.ButtonID);
                }
            }
        }

        private class InternalTarget : Target
        {
            private readonly HangingSkeletonDeed m_Skeleton;
            private readonly int m_ItemID;
            public InternalTarget(HangingSkeletonDeed banner, int itemID)
                : base(-1, true, TargetFlags.None)
            {
                m_Skeleton = banner;
                m_ItemID = itemID;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Skeleton == null || m_Skeleton.Deleted)
                    return;

                if (m_Skeleton.IsChildOf(from.Backpack))
                {
                    BaseHouse house = BaseHouse.FindHouseAt(from);

                    if (house != null && house.IsOwner(from))
                    {
                        IPoint3D p = targeted as IPoint3D;
                        Map map = from.Map;

                        if (p == null || map == null)
                            return;

                        Point3D p3d = new Point3D(p);
                        ItemData id = TileData.ItemTable[m_ItemID & TileData.MaxItemValue];

                        if (map.CanFit(p3d, id.Height))
                        {
                            house = BaseHouse.FindHouseAt(p3d, map, id.Height);

                            if (house != null && house.IsOwner(from))
                            {
                                bool north = BaseAddon.IsWall(p3d.X, p3d.Y - 1, p3d.Z, map);
                                bool west = BaseAddon.IsWall(p3d.X - 1, p3d.Y, p3d.Z, map);

                                if (north && west)
                                {
                                    from.CloseGump(typeof(FacingGump));
                                    from.SendGump(new FacingGump(m_Skeleton, m_ItemID, p3d, house));
                                }
                                else if (north || west)
                                {
                                    HangingSkeleton banner = null;

                                    if (north)
                                        banner = new HangingSkeleton(m_ItemID);
                                    else if (west)
                                        banner = new HangingSkeleton(GetWestItemID(m_ItemID));

                                    house.Addons[banner] = from;

                                    banner.IsRewardItem = m_Skeleton.IsRewardItem;
                                    banner.MoveToWorld(p3d, map);

                                    m_Skeleton.Delete();
                                }
                                else
                                    from.SendLocalizedMessage(1042039); // The banner must be placed next to a wall.								
                            }
                            else
                                from.SendLocalizedMessage(1042036); // That location is not in your house.
                        }
                        else
                            from.SendLocalizedMessage(500269); // You cannot build that there.		
                    }
                    else
                        from.SendLocalizedMessage(502092); // You must be in your house to do this.
                }
                else
                    from.SendLocalizedMessage(1042038); // You must have the object in your backpack to use it.       
            }

            private class FacingGump : Gump
            {
                private readonly HangingSkeletonDeed m_Skeleton;
                private readonly int m_ItemID;
                private readonly Point3D m_Location;
                private readonly BaseHouse m_House;
                public FacingGump(HangingSkeletonDeed banner, int itemID, Point3D location, BaseHouse house)
                    : base(150, 50)
                {
                    m_Skeleton = banner;
                    m_ItemID = itemID;
                    m_Location = location;
                    m_House = house;

                    Closable = true;
                    Disposable = true;
                    Dragable = true;
                    Resizable = false;

                    AddPage(0);

                    AddBackground(0, 0, 300, 150, 0xA28);

                    AddItem(90, 30, GetWestItemID(itemID));
                    AddItem(180, 30, itemID);

                    AddButton(50, 35, 0x868, 0x869, (int)Buttons.East, GumpButtonType.Reply, 0);
                    AddButton(145, 35, 0x868, 0x869, (int)Buttons.South, GumpButtonType.Reply, 0);
                }

                private enum Buttons
                {
                    Cancel,
                    South,
                    East
                }
                public override void OnResponse(NetState sender, RelayInfo info)
                {
                    if (m_Skeleton == null || m_Skeleton.Deleted || m_House == null)
                        return;

                    HangingSkeleton banner = null;

                    if (info.ButtonID == (int)Buttons.East)
                        banner = new HangingSkeleton(GetWestItemID(m_ItemID));
                    if (info.ButtonID == (int)Buttons.South)
                        banner = new HangingSkeleton(m_ItemID);

                    if (banner != null)
                    {
                        m_House.Addons[banner] = sender.Mobile;

                        banner.IsRewardItem = m_Skeleton.IsRewardItem;
                        banner.MoveToWorld(m_Location, sender.Mobile.Map);

                        m_Skeleton.Delete();
                    }
                }
            }
        }
    }
}
