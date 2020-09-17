using Server.Engines.VeteranRewards;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public class DecorativeShield : Item, IAddon, IRewardItem
    {
        private bool m_IsRewardItem;
        [Constructable]
        public DecorativeShield()
            : this(0x156C)
        {
        }

        [Constructable]
        public DecorativeShield(int itemID)
            : base(itemID)
        {
            Movable = false;
        }

        public DecorativeShield(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties => true;

        public Item Deed
        {
            get
            {
                DecorativeShieldDeed deed = new DecorativeShieldDeed
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
                if (ItemID < 0x1582)
                    return (ItemID & 0x1) == 0;

                return ItemID <= 0x1585;
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
            if (from.InRange(Location, 2))
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

    public class DecorativeShieldDeed : Item, IRewardItem
    {
        private bool m_IsRewardItem;
        [Constructable]
        public DecorativeShieldDeed()
            : base(0x14F0)
        {
            LootType = LootType.Blessed;
            Weight = 1.0;
        }

        public DecorativeShieldDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1049771;// deed for a decorative shield wall hanging
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
        public static int GetWestItemID(int east)
        {
            switch (east)
            {
                case 0x1582:
                    return 0x1635;
                case 0x1583:
                    return 0x1634;
                case 0x1584:
                    return 0x1637;
                case 0x1585:
                    return 0x1636;
                default:
                    return east + 1;
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
                from.CloseGump(typeof(InternalGump));
                from.SendGump(new InternalGump(this));
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
            public const int Start = 0x156C;
            public const int End = 0x1585;
            private readonly DecorativeShieldDeed m_Shield;
            private readonly int m_Page;
            public InternalGump(DecorativeShieldDeed shield)
                : this(shield, 1)
            {
            }

            public InternalGump(DecorativeShieldDeed shield, int page)
                : base(150, 50)
            {
                m_Shield = shield;
                m_Page = page;

                Closable = true;
                Disposable = true;
                Dragable = true;
                Resizable = false;

                AddPage(0);

                AddBackground(25, 0, 500, 230, 0xA28);

                int itemID = Start;

                for (int i = 1; i <= 2; i++)
                {
                    AddPage(i);

                    for (int j = 0; j < 9 - i; j++)
                    {
                        AddItem(40 + j * 60, 70, itemID);
                        AddButton(60 + j * 60, 50, 0x845, 0x846, itemID, GumpButtonType.Reply, 0);

                        if (itemID < 0x1582)
                            itemID += 2;
                        else
                            itemID += 1;
                    }

                    switch (i)
                    {
                        case 1:
                            AddButton(455, 198, 0x8B0, 0x8B0, 0, GumpButtonType.Page, 2);
                            break;
                        case 2:
                            AddButton(70, 198, 0x8AF, 0x8AF, 0, GumpButtonType.Page, 1);
                            break;
                    }
                }
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (m_Shield == null || m_Shield.Deleted)
                    return;

                Mobile m = sender.Mobile;

                if (info.ButtonID >= Start && info.ButtonID <= End)
                {
                    if ((info.ButtonID & 0x1) == 0 && info.ButtonID < 0x1582 || info.ButtonID >= 0x1582 && info.ButtonID <= 0x1585)
                    {
                        m.SendLocalizedMessage(1049780); // Where would you like to place this decoration?
                        m.Target = new InternalTarget(m_Shield, info.ButtonID);
                    }
                }
            }
        }

        private class InternalTarget : Target
        {
            private readonly DecorativeShieldDeed m_Shield;
            private readonly int m_ItemID;
            public InternalTarget(DecorativeShieldDeed shield, int itemID)
                : base(-1, true, TargetFlags.None)
            {
                m_Shield = shield;
                m_ItemID = itemID;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Shield == null || m_Shield.Deleted)
                    return;

                if (m_Shield.IsChildOf(from.Backpack))
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
                                    from.SendGump(new FacingGump(m_Shield, m_ItemID, p3d, house));
                                }
                                else if (north || west)
                                {
                                    DecorativeShield shield = null;

                                    if (north)
                                        shield = new DecorativeShield(m_ItemID);
                                    else if (west)
                                        shield = new DecorativeShield(GetWestItemID(m_ItemID));

                                    house.Addons[shield] = from;

                                    shield.IsRewardItem = m_Shield.IsRewardItem;
                                    shield.MoveToWorld(p3d, map);

                                    m_Shield.Delete();
                                }
                                else
                                    from.SendLocalizedMessage(1049781); // This decoration must be placed next to a wall.		
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
                private readonly DecorativeShieldDeed m_Shield;
                private readonly int m_ItemID;
                private readonly Point3D m_Location;
                private readonly BaseHouse m_House;
                public FacingGump(DecorativeShieldDeed shield, int itemID, Point3D location, BaseHouse house)
                    : base(150, 50)
                {
                    m_Shield = shield;
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

                    AddButton(50, 35, 0x867, 0x869, (int)Buttons.East, GumpButtonType.Reply, 0);
                    AddButton(145, 35, 0x867, 0x869, (int)Buttons.South, GumpButtonType.Reply, 0);
                }

                private enum Buttons
                {
                    Cancel,
                    South,
                    East
                }
                public override void OnResponse(NetState sender, RelayInfo info)
                {
                    if (m_Shield == null || m_Shield.Deleted || m_House == null)
                        return;

                    DecorativeShield shield = null;

                    if (info.ButtonID == (int)Buttons.East)
                        shield = new DecorativeShield(GetWestItemID(m_ItemID));
                    if (info.ButtonID == (int)Buttons.South)
                        shield = new DecorativeShield(m_ItemID);

                    if (shield != null)
                    {
                        m_House.Addons[shield] = sender.Mobile;

                        shield.IsRewardItem = m_Shield.IsRewardItem;
                        shield.MoveToWorld(m_Location, sender.Mobile.Map);

                        m_Shield.Delete();
                    }
                }
            }
        }
    }
}
