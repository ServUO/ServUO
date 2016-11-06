using System;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public class DragonHead : Item, IAddon
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public DragonHead()
            : this(0x2234)
        {
        }

        [Constructable]
        public DragonHead(int itemID)
            : base(itemID)
        {
            this.Movable = false;
        }

        public DragonHead(Serial serial)
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
                DragonHeadDeed deed = new DragonHeadDeed();

                return deed;
            }
        }
        public bool FacingEast
        {
            get
            {
                return this.ItemID == 0x2235;
            }
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
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }

        public bool CouldFit(IPoint3D p, Map map)
        {
            if (map == null || !map.CanFit(p.X, p.Y, p.Z, this.ItemData.Height))
                return false;

            if (this.FacingEast)
                return BaseAddon.IsWall(p.X - 1, p.Y, p.Z, map); // west wall                
            else
                return BaseAddon.IsWall(p.X, p.Y - 1, p.Z, map); // north wall
        }
    }

    public class DragonHeadDeed : Item
    {
        public override int LabelNumber { get { return 1028756; } } // dragon head

        [Constructable]
        public DragonHeadDeed()
            : base(0x14F0)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 1.0;
        }

        public DragonHeadDeed(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
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
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }

        private class InternalGump : Gump
        {            
            private readonly DragonHeadDeed m_Head;
            
            public InternalGump(DragonHeadDeed head)
                : base(150, 50)
            {
                this.m_Head = head;

                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;

                this.AddPage(0);

                this.AddBackground(12, 13, 271, 318, 9200);                
                this.AddImageTiled(20, 23, 252, 300, 2624);
                this.AddImageTiled(22, 53, 250, 12, 5055);
                this.AddImageTiled(22, 292, 250, 12, 5055);
                this.AddAlphaRegion(20, 23, 252, 300);

                this.AddButton(22, 294, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(58, 296, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL

                this.AddHtmlLocalized(23, 31, 273, 20, 1080392, 0x7FFF, false, false); // Select your choice from the menu below.

                this.AddButton(30, 77, 2117, 2118, 0x2235, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(56, 74, 150, 20, 1080207, 0x7FFF, false, false); // Dragon Head (East)
                this.AddButton(30, 106, 2117, 2118, 0x2234, GumpButtonType.Reply, 0);
                this.AddHtmlLocalized(56, 103, 150, 20, 1080208, 0x7FFF, false, false); // Dragon Head (South)
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (this.m_Head == null | this.m_Head.Deleted)
                    return;

                Mobile m = sender.Mobile;

                if (info.ButtonID == 0x2235 || info.ButtonID == 0x2234)
                    m.Target = new InternalTarget(this.m_Head, info.ButtonID);
            }
        }

        private class InternalTarget : Target
        {
            private readonly DragonHeadDeed m_Head;
            private readonly int m_ItemID;

            public InternalTarget(DragonHeadDeed head, int itemID)
                : base(-1, true, TargetFlags.None)
            {
                this.m_Head = head;
                this.m_ItemID = itemID;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (this.m_Head == null || this.m_Head.Deleted)
                    return;

                if (this.m_Head.IsChildOf(from.Backpack))
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
                                    from.SendGump(new FacingGump(this.m_Head, this.m_ItemID, p3d, house));
                                }
                                else if (north || west)
                                {
                                    DragonHead head = null;

                                    if (north)
                                        head = new DragonHead(0x2234);
                                    else if (west)
                                        head = new DragonHead(this.m_ItemID);

                                    house.Addons.Add(head);

                                    head.MoveToWorld(p3d, map);

                                    this.m_Head.Delete();
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
                private readonly DragonHeadDeed m_Head;
                private readonly int m_ItemID;
                private readonly Point3D m_Location;
                private readonly BaseHouse m_House;
                public FacingGump(DragonHeadDeed head, int itemID, Point3D location, BaseHouse house)
                    : base(150, 50)
                {
                    this.m_Head = head;
                    this.m_ItemID = itemID;
                    this.m_Location = location;
                    this.m_House = house;

                    this.AddBackground(0, 0, 300, 150, 0xA28);

                    this.AddItem(90, 30, itemID);
                    this.AddItem(180, 30, 0x2234);

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
                    if (this.m_Head == null || this.m_Head.Deleted || this.m_House == null)
                        return;

                    DragonHead head = null;

                    if (info.ButtonID == (int)Buttons.East)
                        head = new DragonHead(this.m_ItemID);

                    if (info.ButtonID == (int)Buttons.South)
                        head = new DragonHead(0x2234);

                    if (head != null)
                    {
                        this.m_House.Addons.Add(head);

                        head.MoveToWorld(this.m_Location, sender.Mobile.Map);

                        this.m_Head.Delete();
                    }
                }
            }
        }
    }
}