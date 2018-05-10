using System;
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
            this.LootType = LootType.Blessed;
            this.Movable = false;
        }

        public HangingSkeleton(Serial serial)
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
                HangingSkeletonDeed deed = new HangingSkeletonDeed();
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
                if (this.ItemID == 0x1A03 || this.ItemID == 0x1A05 || this.ItemID == 0x1A09 ||
                    this.ItemID == 0x1B1E || this.ItemID == 0x1B7F)
                    return true; 

                return false;
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
            if (from.InRange(this.Location, 3))
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

    public class HangingSkeletonDeed : Item, IRewardItem
    {
        private bool m_IsRewardItem;
        [Constructable]
        public HangingSkeletonDeed()
            : base(0x14F0)
        { 
            this.LootType = LootType.Blessed;
            this.Weight = 1.0;
        }

        public HangingSkeletonDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1049772;
            }
        }// deed for a hanging skeleton decoration
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
        public static int GetWestItemID(int south)
        {
            switch ( south )
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
			
            if (this.m_IsRewardItem)
                list.Add(1076220); // 4th Year Veteran Reward
        }

        public override void OnDoubleClick(Mobile from)
        { 
            if (this.m_IsRewardItem && !RewardSystem.CheckIsUsableBy(from, this, null))
                return;
		
            if (this.IsChildOf(from.Backpack))
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
            private readonly HangingSkeletonDeed m_Skeleton;
            public InternalGump(HangingSkeletonDeed skeleton)
                : base(100, 200)
            {
                this.m_Skeleton = skeleton;
				
                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;
				
                this.AddPage(0);

                this.AddBackground(25, 0, 500, 230, 0xA28);

                this.AddPage(1);

                this.AddItem(130, 70, 0x1A03);
                this.AddButton(150, 50, 0x845, 0x846, 0x1A03, GumpButtonType.Reply, 0);

                this.AddItem(190, 70, 0x1A05);
                this.AddButton(210, 50, 0x845, 0x846, 0x1A05, GumpButtonType.Reply, 0);

                this.AddItem(250, 70, 0x1A09);
                this.AddButton(270, 50, 0x845, 0x846, 0x1A09, GumpButtonType.Reply, 0);

                this.AddItem(310, 70, 0x1B1E);
                this.AddButton(330, 50, 0x845, 0x846, 0x1B1E, GumpButtonType.Reply, 0);

                this.AddItem(370, 70, 0x1B7F);
                this.AddButton(390, 50, 0x845, 0x846, 0x1B7F, GumpButtonType.Reply, 0);
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (this.m_Skeleton == null || this.m_Skeleton.Deleted)
                    return;		
				
                Mobile m = sender.Mobile;	
			
                if (info.ButtonID == 0x1A03 || info.ButtonID == 0x1A05 || info.ButtonID == 0x1A09 ||
                    info.ButtonID == 0x1B1E || info.ButtonID == 0x1B7F)
                {
                    m.SendLocalizedMessage(1049780); // Where would you like to place this decoration?
                    m.Target = new InternalTarget(this.m_Skeleton, info.ButtonID);
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
                this.m_Skeleton = banner;
                this.m_ItemID = itemID;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_Skeleton == null || this.m_Skeleton.Deleted)
                    return;
					
                if (this.m_Skeleton.IsChildOf(from.Backpack))
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
                                    from.SendGump(new FacingGump(this.m_Skeleton, this.m_ItemID, p3d, house));
                                }
                                else if (north || west)
                                {
                                    HangingSkeleton banner = null;
									
                                    if (north)
                                        banner = new HangingSkeleton(this.m_ItemID);
                                    else if (west)
                                        banner = new HangingSkeleton(GetWestItemID(this.m_ItemID));

                                    house.Addons[banner] = from;

                                    banner.IsRewardItem = this.m_Skeleton.IsRewardItem;
                                    banner.MoveToWorld(p3d, map);

                                    this.m_Skeleton.Delete();
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
                    this.m_Skeleton = banner;
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

                    this.AddButton(50, 35, 0x868, 0x869, (int)Buttons.East, GumpButtonType.Reply, 0);
                    this.AddButton(145, 35, 0x868, 0x869, (int)Buttons.South, GumpButtonType.Reply, 0);
                }

                private enum Buttons
                {
                    Cancel,
                    South,
                    East
                }
                public override void OnResponse(NetState sender, RelayInfo info)
                {
                    if (this.m_Skeleton == null || this.m_Skeleton.Deleted || this.m_House == null)
                        return;		
					
                    HangingSkeleton banner = null;	
				
                    if (info.ButtonID == (int)Buttons.East)
                        banner = new HangingSkeleton(GetWestItemID(this.m_ItemID));
                    if (info.ButtonID == (int)Buttons.South)
                        banner = new HangingSkeleton(this.m_ItemID);
						
                    if (banner != null)
                    {
                        m_House.Addons[banner] = sender.Mobile;

                        banner.IsRewardItem = this.m_Skeleton.IsRewardItem;
                        banner.MoveToWorld(this.m_Location, sender.Mobile.Map);

                        this.m_Skeleton.Delete();
                    }
                }
            }
        }
    }
}