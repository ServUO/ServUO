using System;
using System.Linq;
using System.Collections.Generic;
using Server.Gumps;
using Server.Engines.VendorSearhing;
using Server.Mobiles;
using Server.ContextMenus;
using Server.Multis;

namespace Server.Items
{
    public class VendorSearchMap : MapItem
    {
        public PlayerVendor Vendor { get; set; }
        public Item SearchItem { get; set; }
        public Point3D SetLocation { get; set; }
        public Map SetMap { get; set; }

        public DateTime DeleteTime { get; set; }

        public int TimeRemaining { get { return DeleteTime <= DateTime.UtcNow ? 0 : (int)(DeleteTime - DateTime.UtcNow).TotalMinutes; } }

        public VendorSearchMap(PlayerVendor vendor, Item item) : base(vendor.Map)
        {
            Vendor = vendor;
            SearchItem = item;

            LootType = LootType.Blessed;

            Width = 400;
            Height = 400;

            Bounds = new Rectangle2D(vendor.X - 100, vendor.Y - 100, 200, 200);
            AddWorldPin(vendor.X, vendor.Y);

            if (vendor.Map == Map.Malas)
                Hue = 1102;
            else if (vendor.Map == Map.Trammel)
                Hue = 50;
            else if (vendor.Map == Map.Tokuno)
                Hue = 1154;

            DeleteTime = DateTime.UtcNow + TimeSpan.FromMinutes(30);
            Timer.DelayCall(TimeSpan.FromMinutes(30), Delete);
        }

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            from.SendLocalizedMessage(1150512, "map"); // You destroyed the ~1_ITEMNAME~.
            Delete();

            return true;
        }

        public override bool AllowSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
        {
            return false;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (Vendor != null && Vendor.Map != null && Vendor.Map != Map.Internal)
                list.Add(1154559, String.Format("{0}\t{1}", Vendor.Name, Vendor.ShopName)); // Map to Vendor ~1_Name~: ~2_Shop~
            else
                base.AddNameProperties(list);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Vendor != null && Vendor.Map != null && Vendor.Map != Map.Internal)
                list.Add(1154639, String.Format("{0}\t{1}", GetCoords(), Vendor.Map.ToString())); //  Vendor Located at ~1_loc~ (~2_facet~)

            list.Add(1075269); // destroyed when dropped
        }

        public string GetCoords()
        {
            if (Vendor != null && Vendor.Map != null && Vendor.Map != Map.Internal)
            {
                int x = Vendor.X;
                int y = Vendor.Y;

                int xLong = 0, yLat = 0;
                int xMins = 0, yMins = 0;
                bool xEast = false, ySouth = false;

                if (Sextant.Format(new Point3D(x, y, Vendor.Map.GetAverageZ(x, y)), Vendor.Map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth))
                {
                    return String.Format("{0}° {1}'{2}, {3}° {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W");
                }
            }

            return "Unknown";
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            list.Add(new OpenMapEntry(from, this));
            list.Add(new TeleportEntry(from, this));
            list.Add(new OpenContainerEntry(from, this));
        }

        public bool CheckVendor()
        {
            return Vendor != null && Vendor.Alive && BaseHouse.FindHouseAt(Vendor) != null;
        }

        public Point3D GetVendorLocation()
        {
            if (CheckVendor())
            {
                BaseHouse h = BaseHouse.FindHouseAt(Vendor);

                if (h != null)
                {
                    return h.BanLocation;
                }
            }

            return Point3D.Zero;
        }

        public Map GetVendorMap()
        {
            if (CheckVendor())
            {
                BaseHouse h = BaseHouse.FindHouseAt(Vendor);

                if (h != null)
                {
                    return h.Map;
                }
            }

            return null;
        }

        public class OpenMapEntry : ContextMenuEntry
        {
            public VendorSearchMap VendorMap { get; set; }
            public Mobile Clicker { get; set; }

            public OpenMapEntry(Mobile from, VendorSearchMap map)
                : base(3006150, -1) // open map
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
            public VendorSearchMap VendorMap { get; set; }
            public Mobile Clicker { get; set; }

            public TeleportEntry(Mobile from, VendorSearchMap map)
                : base(map.SetLocation == Point3D.Zero ? 1154558 : 1154636, -1) // teleport to vendor : return to previous location
            {
                VendorMap = map;
                Clicker = from;
            }

            public override void OnClick()
            {
                Clicker.SendGump(new ConfirmTeleportGump(VendorMap, Clicker));
            }
        }

        public class OpenContainerEntry : ContextMenuEntry
        {
            public VendorSearchMap VendorMap { get; set; }
            public Mobile Clicker { get; set; }

            public OpenContainerEntry(Mobile from, VendorSearchMap map)
                : base(1154699, -1) // Open Container Containing Item
            {
                VendorMap = map;
                Clicker = from;

                BaseHouse h1 = BaseHouse.FindHouseAt(VendorMap.Vendor);
                BaseHouse h2 = BaseHouse.FindHouseAt(Clicker);

                Enabled = h1 != null && h1 == h2;
            }

            public override void OnClick()
            {
                BaseHouse h1 = BaseHouse.FindHouseAt(VendorMap.Vendor);
                BaseHouse h2 = BaseHouse.FindHouseAt(Clicker);

                if (h1 != null && h1 == h2)
                {
                    Container c = VendorMap.SearchItem.ParentEntity as Container;

                    if (c != null)
                        c.DisplayTo(Clicker);
                }
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