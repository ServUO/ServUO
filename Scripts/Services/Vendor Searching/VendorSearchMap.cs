using System;
using System.Collections.Generic;
using Server.Engines.VendorSearching;
using Server.Mobiles;
using Server.ContextMenus;
using Server.Multis;
using Server.Gumps;
using Server.Engines.Auction;

namespace Server.Items
{
    public class VendorSearchMap : MapItem
    {
        public readonly int TeleportCost = 1000;
        public readonly int DeleteDelayMinutes = 30;

        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerVendor Vendor { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public AuctionSafe AuctionSafe { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsAuction { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Item SearchItem { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D SetLocation { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Map SetMap { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DeleteTime { get; set; }

        public int TimeRemaining { get { return DeleteTime <= DateTime.UtcNow ? 0 : (int)(DeleteTime - DateTime.UtcNow).TotalMinutes; } }

        public VendorSearchMap(Item item, bool auction)
            : base(item.Map)
        {
            LootType = LootType.Blessed;
            Hue = RecallRune.CalculateHue(item.Map, null, true);

            IsAuction = auction;
            SearchItem = item;

            Point3D p;

            if (IsAuction)
            {
                AuctionSafe = Auction.Auctions.Find(x => x.AuctionItem == item).Safe;
                p = AuctionSafe.Location;
            }
            else
            {
                Vendor = item.RootParentEntity as PlayerVendor;
                p = Vendor.Location;
            }            

            Width = 300;
            Height = 300;
            var size = item.Map == Map.Tokuno ? 300 : item.Map == Map.TerMur ? 200 : 600;

            Bounds = new Rectangle2D(p.X - size / 2, p.Y - size / 2, size, size);
            AddWorldPin(p.X, p.Y);            

            DeleteTime = DateTime.UtcNow + TimeSpan.FromMinutes(DeleteDelayMinutes);
            Timer.DelayCall(TimeSpan.FromMinutes(DeleteDelayMinutes), Delete);
        }

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            from.SendLocalizedMessage(500424); // You destroyed the item.
            Delete();

            return true;
        }

        public override bool AllowSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
        {
            return false;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            string[] name = Name();

            list.Add(1154559, string.Format("{0}\t{1}", name[0], name[1])); // Map to Vendor ~1_Name~: ~2_Shop~
        }

        public new string[] Name()
        {
            string[] array = new string[2];

            string Name = "Unknown";
            string Shop = "Unknown";

            if (IsAuction)
            {
                if (SearchItem != null)
                {
                    if (AuctionSafe != null)
                    {
                        BaseHouse house = BaseHouse.FindHouseAt(AuctionSafe);

                        if (house != null)
                            Name = house.Sign.GetName();
                    }

                    Shop = (SearchItem.LabelNumber != 0) ? string.Format("#{0}", SearchItem.LabelNumber) : SearchItem.Name;
                }                
            }
            else
            {
                if (Vendor != null)
                {
                    Name = Vendor.Name;
                    Shop = Vendor.ShopName;
                }
            }

            return new string[] { Name, Shop };
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            string[] coord = GetCoords();

            if (SetLocation == Point3D.Zero)
                list.Add(1154639, string.Format("{0}\t{1}", coord[0], coord[1])); //  Vendor Located at ~1_loc~ (~2_facet~)
            else
                list.Add(1154638, string.Format("{0}\t{1}", coord[0], coord[1])); //  Return to ~1_loc~ (~2_facet~)                

            if (!IsSale())
            {
                list.Add(1154700); // Item no longer for sale.
            }

            list.Add(1075269); // Destroyed when dropped
        }

        public bool IsSale()
        {
            return SearchItem != null && (AuctionSafe != null && AuctionSafe.CheckAuctionItem(SearchItem) || Vendor != null && Vendor.GetVendorItem(SearchItem) != null);
        }

        public string[] GetCoords()
        {
            string[] array = new string[2];

            Point3D loc = Point3D.Zero;
            Map locmap = Map.Internal;

            if (SetLocation != Point3D.Zero)
            {
                loc = SetLocation;
                locmap = SetMap;
            }
            else if (AuctionSafe != null)
            {
                loc = AuctionSafe.Location;
                locmap = AuctionSafe.Map;
            }
            else if (Vendor != null)
            {
                loc = Vendor.Location;
                locmap = Vendor.Map;
            }

            if (loc != Point3D.Zero && locmap != Map.Internal)
            {
                int x = loc.X;
                int y = loc.Y;
                int z = loc.Z;
                Map map = locmap;

                int xLong = 0, yLat = 0;
                int xMins = 0, yMins = 0;
                bool xEast = false, ySouth = false;

                if (Sextant.Format(new Point3D(x, y, z), map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth))
                {
                    return new string[] { string.Format("{0}o {1}'{2}, {3}o {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W"), map.ToString() };
                }
            }

            return new string[] { "an unknown location", "Unknown" };
        }

        public void OnBeforeTravel(Mobile from)
        {
            if (SetLocation != Point3D.Zero)
            {
                Delete();
            }
            else
            {
                Banker.Withdraw(from, TeleportCost);
                from.SendLocalizedMessage(1060398, TeleportCost.ToString()); // ~1_AMOUNT~ gold has been withdrawn from your bank box.

                SetLocation = from.Location;
                SetMap = from.Map;
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            list.Add(new OpenMapEntry(from, this));

            if (SetLocation == Point3D.Zero)
                list.Add(new TeleportEntry(from, this));
            else
                list.Add(new ReturnTeleportEntry(from, this));

            list.Add(new OpenContainerEntry(from, this));
        }

        public Point3D GetLocation(Mobile m)
        {
            BaseHouse h = null;

            if (SetLocation != Point3D.Zero)
            {
                h = BaseHouse.FindHouseAt(SetLocation, SetMap, 16);
            }
            else if (IsAuction)
            {
                if (AuctionSafe != null)
                {
                    h = BaseHouse.FindHouseAt(AuctionSafe);                    
                }
            }
            else
            {
                if (Vendor != null)
                {
                    h = BaseHouse.FindHouseAt(Vendor);
                }
            }

            if (h != null)
            {
                m.SendLocalizedMessage(1070905); // Strong magics have redirected you to a safer location!
                return h.BanLocation;
            }

            return SetLocation != Point3D.Zero ? SetLocation : Point3D.Zero;
        }

        public Map GetMap()
        {
            if (SetLocation != Point3D.Zero)
                return SetMap;

            Map map = null;

            if (IsAuction)
            {
                if (AuctionSafe != null)
                {
                    map = AuctionSafe.Map;
                }
            }
            else
            {
                if (Vendor != null)
                {
                    map = Vendor.Map;
                }
            }

            return map;
        }

        public class OpenMapEntry : ContextMenuEntry
        {
            public VendorSearchMap VendorMap { get; set; }
            public Mobile Clicker { get; set; }

            public OpenMapEntry(Mobile from, VendorSearchMap map)
                : base(3006150, 1) // Open Map
            {
                VendorMap = map;
                Clicker = from;
            }

            public override void OnClick()
            {
                VendorMap.DisplayTo(Clicker);
            }
        }

        public class TeleportEntry : ContextMenuEntry
        {
            private VendorSearchMap VendorMap { get; set; }
            private Mobile Clicker { get; set; }

            public TeleportEntry(Mobile from, VendorSearchMap map)
                : base(1154558, -1) // Teleport To Vendor
            {
                VendorMap = map;
                Clicker = from;
                Enabled = VendorMap.IsSale();
            }

            public override void OnClick()
            {
                if (VendorMap.IsSale())
                {
                    BaseGump.SendGump(new ConfirmTeleportGump(VendorMap, (PlayerMobile)Clicker));
                }
                else
                {
                    Clicker.SendLocalizedMessage(1154700); // Item no longer for sale.
                }
            }
        }

        public class ReturnTeleportEntry : ContextMenuEntry
        {
            private VendorSearchMap VendorMap { get; set; }
            private Mobile Clicker { get; set; }

            public ReturnTeleportEntry(Mobile from, VendorSearchMap map)
                : base(1154636, -1) // Return to Previous Location
            {
                VendorMap = map;
                Clicker = from;
            }

            public override void OnClick()
            {
                if (Clicker is PlayerMobile)
                {
                    BaseGump.SendGump(new ConfirmTeleportGump(VendorMap, (PlayerMobile)Clicker));
                }
            }
        }

        public class OpenContainerEntry : ContextMenuEntry
        {
            private VendorSearchMap VendorMap { get; set; }
            private Mobile Clicker { get; set; }
            private Container Container { get; set; }

            public OpenContainerEntry(Mobile from, VendorSearchMap map)
                : base(1154699, -1) // Open Container Containing Item
            {
                VendorMap = map;
                Clicker = from;

                if (VendorMap.SearchItem != null)
                    Container = VendorMap.SearchItem.ParentEntity as Container;

                Enabled = IsAccessible();
            }

            private bool IsAccessible()
            {
                if (Container == null || VendorMap.IsAuction)
                    return false;

                if (!Container.IsAccessibleTo(Clicker))
                    return false;

                if (!Clicker.InRange(Container.GetWorldLocation(), 18))
                    return false;

                return true;
            }

            public override void OnClick()
            {
                RecurseOpen(Container, Clicker);
            }

            private static void RecurseOpen(Container c, Mobile from)
            {
                if (c.Parent is Container parent)
                    RecurseOpen(parent, from);

                c.DisplayTo(from);
            }
        }

        public VendorSearchMap(Serial serial)
            : base(serial)
		{
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Delete();
        }
    }
}
