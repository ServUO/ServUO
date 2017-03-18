using System;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
    public class DecorativeShardShield : Item, IAddon
    {
        public override bool IsArtifact { get { return true; } }

        [Constructable]
        public DecorativeShardShield()
            : this(0x6380)
        {
        }

        [Constructable]
        public DecorativeShardShield(int itemID)
            : base(itemID)
        {
            this.Movable = false;
        }

        public DecorativeShardShield(Serial serial)
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
                DecorativeShardShieldDeed deed = new DecorativeShardShieldDeed();

                return deed;
            }
        }
        public bool FacingEast
        {
            get
            {
                return this.ItemID <= 0x639A;
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

    public class DecorativeShardShieldDeed : Item
    {
        public override int LabelNumber { get { return 1153729; } } // Deed for a Decorative Shard Shield

        [Constructable]
        public DecorativeShardShieldDeed()
            : base(0x14F0)
        {
            this.LootType = LootType.Blessed;
            this.Weight = 1.0;
        }

        public DecorativeShardShieldDeed(Serial serial)
            : base(serial)
        {
        }

        public static int GetSouthItemID(int south)
        {
            return south + 27;
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
            public const int Start = 25472;
            public const int End = 25498;
            private readonly DecorativeShardShieldDeed m_Shield;
            private readonly int m_Page;
            public InternalGump(DecorativeShardShieldDeed shield)
                : this(shield, 1)
            {
            }

            public InternalGump(DecorativeShardShieldDeed shield, int page)
                : base(150, 50)
            {
                this.m_Shield = shield;
                this.m_Page = page;

                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;

                this.AddPage(0);

                this.AddBackground(50, 89, 647, 505, 2600);
                this.AddLabel(103, 114, 0, @"Choose from the following:");

                int itemID = Start;


                for (int i = 0; i < 27; i++)
                {
                    if (8 >= i)
                    {
                        this.AddItem(82 + i * 65, 180, itemID);
                        this.AddTooltip(1104344 + i);
                        this.AddButton(92 + i * 65, 155, 0x845, 0x846, itemID, GumpButtonType.Reply, 0);
                        this.AddTooltip(1104344 + i);
                    }
                    else if (i > 8 && i < 18)
                    {

                        this.AddItem(82 + ((i - 9) * 65), 330, itemID);
                        this.AddTooltip(1104344 + i);
                        this.AddButton(92 + ((i - 9) * 65), 305, 0x845, 0x846, itemID, GumpButtonType.Reply, 0);
                        this.AddTooltip(1104344 + i);
                    }
                    else if (i >= 18 && 26 >= i)
                    {
                        this.AddItem(82 + ((i - 18) * 65), 480, itemID);
                        this.AddTooltip(1104344 + i);
                        this.AddButton(92 + ((i - 18) * 65), 455, 0x845, 0x846, itemID, GumpButtonType.Reply, 0);
                        this.AddTooltip(1104344 + i);
                    }

                    itemID++;
                }
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (this.m_Shield == null | this.m_Shield.Deleted)
                    return;

                Mobile m = sender.Mobile;

                if (info.ButtonID >= Start && info.ButtonID <= End)
                {
                    if ((info.ButtonID & 0x1) == 0 && info.ButtonID < 0x6380 || info.ButtonID >= 0x6380 && info.ButtonID <= 0x639A)
                    {
                        m.SendLocalizedMessage(1049780); // Where would you like to place this decoration?
                        m.Target = new InternalTarget(this.m_Shield, info.ButtonID);
                    }
                }
            }
        }

        private class InternalTarget : Target
        {
            private readonly DecorativeShardShieldDeed m_Shield;
            private readonly int m_ItemID;
            public InternalTarget(DecorativeShardShieldDeed shield, int itemID)
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
                                    DecorativeShardShield shield = null;

                                    if (north)
                                        shield = new DecorativeShardShield(GetSouthItemID(this.m_ItemID));
                                    else if (west)
                                        shield = new DecorativeShardShield(this.m_ItemID);

                                    house.Addons.Add(shield);

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
                private readonly DecorativeShardShieldDeed m_Shield;
                private readonly int m_ItemID;
                private readonly Point3D m_Location;
                private readonly BaseHouse m_House;
                public FacingGump(DecorativeShardShieldDeed shield, int itemID, Point3D location, BaseHouse house)
                    : base(150, 50)
                {
                    this.m_Shield = shield;
                    this.m_ItemID = itemID;
                    this.m_Location = location;
                    this.m_House = house;

                    this.AddBackground(0, 0, 300, 150, 0xA28);

                    this.AddItem(90, 30, itemID);
                    this.AddItem(180, 30, GetSouthItemID(itemID));

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

                    DecorativeShardShield shield = null;

                    if (info.ButtonID == (int)Buttons.East)
                        shield = new DecorativeShardShield(this.m_ItemID);

                    if (info.ButtonID == (int)Buttons.South)
                        shield = new DecorativeShardShield(GetSouthItemID(this.m_ItemID));

                    if (shield != null)
                    {
                        this.m_House.Addons.Add(shield);

                        shield.MoveToWorld(this.m_Location, sender.Mobile.Map);

                        this.m_Shield.Delete();
                    }
                }
            }
        }
    }
}