using System;
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
            this.Movable = false;
        }

        public DecorativeShield(Serial serial)
            : base(serial)
        {
        }

        public override bool ForceShowProperties
        {
            get
            {
                return ObjectPropertyList.Enabled;
            }
        }
        public Item Deed
        { 
            get
            { 
                DecorativeShieldDeed deed = new DecorativeShieldDeed();
                deed.IsRewardItem = this.m_IsRewardItem;

                return deed;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return this.m_IsRewardItem;
            }
            set
            {
                this.m_IsRewardItem = value;
                this.InvalidateProperties();
            }
        }
        public bool FacingSouth
        {
            get
            {
                if (this.ItemID < 0x1582)
                    return (this.ItemID & 0x1) == 0;
				
                return this.ItemID <= 0x1585; 
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
			
            if (Core.ML && this.m_IsRewardItem)
                list.Add(1076220); // 4th Year Veteran Reward
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.Location, 2))
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
			
            writer.Write((bool)this.m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
			
            this.m_IsRewardItem = reader.ReadBool();
        }

        public bool CouldFit(IPoint3D p, Map map)
        { 
            if (map == null || !map.CanFit(p.X, p.Y, p.Z, this.ItemData.Height))
                return false;
				
            if (this.FacingSouth)
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
            this.LootType = LootType.Blessed;
            this.Weight = 1.0;
        }

        public DecorativeShieldDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1049771;
            }
        }// deed for a decorative shield wall hanging
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get
            {
                return this.m_IsRewardItem;
            }
            set
            {
                this.m_IsRewardItem = value;
                this.InvalidateProperties();
            }
        }
        public static int GetWestItemID(int east)
        {
            switch ( east )
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
			
            if (this.m_IsRewardItem)
                list.Add(1076220); // 4th Year Veteran Reward
        }

        public override void OnDoubleClick(Mobile from)
        { 
            if (this.m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
                return;
		
            if (this.IsChildOf(from.Backpack))
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
			
            writer.Write((bool)this.m_IsRewardItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
			
            this.m_IsRewardItem = reader.ReadBool();
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
                this.m_Shield = shield;
                this.m_Page = page;
				
                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;
				
                this.AddPage(0);

                this.AddBackground(25, 0, 500, 230, 0xA28);

                int itemID = Start;

                for (int i = 1; i <= 2; i++)
                {
                    this.AddPage(i);

                    for (int j = 0; j < 9 - i; j++)
                    {
                        this.AddItem(40 + j * 60, 70, itemID);
                        this.AddButton(60 + j * 60, 50, 0x845, 0x846, itemID, GumpButtonType.Reply, 0);

                        if (itemID < 0x1582)
                            itemID += 2;
                        else
                            itemID += 1;
                    }

                    switch ( i )
                    {
                        case 1:
                            this.AddButton(455, 198, 0x8B0, 0x8B0, 0, GumpButtonType.Page, 2);
                            break;
                        case 2:
                            this.AddButton(70, 198, 0x8AF, 0x8AF, 0, GumpButtonType.Page, 1);
                            break;	
                    }
                }
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (this.m_Shield == null | this.m_Shield.Deleted)
                    return;		
				
                Mobile m = sender.Mobile;	
			
                if (info.ButtonID >= Start && info.ButtonID <= End)
                {
                    if ((info.ButtonID & 0x1) == 0 && info.ButtonID < 0x1582 || info.ButtonID >= 0x1582 && info.ButtonID <= 0x1585)
                    {
                        m.SendLocalizedMessage(1049780); // Where would you like to place this decoration?
                        m.Target = new InternalTarget(this.m_Shield, info.ButtonID);
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
                this.m_Shield = shield;
                this.m_ItemID = itemID;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_Shield == null || this.m_Shield.Deleted)
                    return;
					
                if (this.m_Shield.IsChildOf(from.Backpack))
                {
                    BaseHouse house = BaseHouse.FindHouseAt(from);

                    if (house != null && house.IsOwner(from))
                    {
                        IPoint3D p = targeted as IPoint3D;
                        Map map = from.Map;
						
                        if (p == null || map == null)
                            return;
							
                        Point3D p3d = new Point3D(p);
                        ItemData id = TileData.ItemTable[this.m_ItemID & TileData.MaxItemValue];
						
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
                                    from.SendGump(new FacingGump(this.m_Shield, this.m_ItemID, p3d, house));
                                }
                                else if (north || west)
                                {
                                    DecorativeShield shield = null;
									
                                    if (north)
                                        shield = new DecorativeShield(this.m_ItemID);
                                    else if (west)
                                        shield = new DecorativeShield(GetWestItemID(this.m_ItemID));
										
                                    house.Addons.Add(shield);

                                    shield.IsRewardItem = this.m_Shield.IsRewardItem;
                                    shield.MoveToWorld(p3d, map);

                                    this.m_Shield.Delete();
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
                    this.m_Shield = shield;
                    this.m_ItemID = itemID;
                    this.m_Location = location;
                    this.m_House = house;
				
                    this.Closable = true;
                    this.Disposable = true;
                    this.Dragable = true;
                    this.Resizable = false;

                    this.AddPage(0);
                    this.AddBackground(0, 0, 300, 150, 0xA28);
					
                    this.AddItem(90, 30, GetWestItemID(itemID));
                    this.AddItem(180, 30, itemID);

                    this.AddButton(50, 35, 0x867, 0x869, (int)Buttons.East, GumpButtonType.Reply, 0);					
                    this.AddButton(145, 35, 0x867, 0x869, (int)Buttons.South, GumpButtonType.Reply, 0);
                }

                private enum Buttons
                {
                    Cancel,
                    South,
                    East
                }
                public override void OnResponse(NetState sender, RelayInfo info)
                {
                    if (this.m_Shield == null || this.m_Shield.Deleted || this.m_House == null)
                        return;		
					
                    DecorativeShield shield = null;	
				
                    if (info.ButtonID == (int)Buttons.East)
                        shield = new DecorativeShield(GetWestItemID(this.m_ItemID));
                    if (info.ButtonID == (int)Buttons.South)
                        shield = new DecorativeShield(this.m_ItemID);
						
                    if (shield != null)
                    {
                        this.m_House.Addons.Add(shield);

                        shield.IsRewardItem = this.m_Shield.IsRewardItem;
                        shield.MoveToWorld(this.m_Location, sender.Mobile.Map);

                        this.m_Shield.Delete();
                    }
                }
            }
        }
    }
}