using System;
using System.Collections.Generic;
using Server.Engines.VendorSearching;
using Server.Mobiles;
using Server.ContextMenus;
using Server.Multis;
using Server.Gumps;

namespace Server.Items
{
    public class VendorSearchMap : MapItem
    {
        public readonly int TeleportCost = 1000;
        public readonly int DeleteDelayMinutes = 30;

        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerVendor Vendor { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Item SearchItem { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D SetLocation { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Map SetMap { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DeleteTime { get; set; }

        public int TimeRemaining { get { return DeleteTime <= DateTime.UtcNow ? 0 : (int)(DeleteTime - DateTime.UtcNow).TotalMinutes; } }

        public VendorSearchMap(PlayerVendor vendor, Item item)
            : base(vendor.Map)
        {
            Vendor = vendor;
            SearchItem = item;

            Hue = RecallRune.CalculateHue(vendor.Map, null, true);
            LootType = LootType.Blessed;

            Width = 400;
            Height = 400;

            Bounds = new Rectangle2D(vendor.X - 100, vendor.Y - 100, 200, 200);
            AddWorldPin(vendor.X, vendor.Y);            

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

            list.Add(1075269); // Destroyed when dropped
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
            list.Add(new TeleportEntry(from, this));
            list.Add(new OpenContainerEntry(from, this));
        }

        public bool CheckVendor()
        {
            return Vendor != null && Vendor.Alive && BaseHouse.FindHouseAt(Vendor) != null;
        }

        public Point3D GetVendorLocation(Mobile m)
        {
            if (CheckVendor())
            {
                BaseHouse h = BaseHouse.FindHouseAt(Vendor);

                if (h != null)
                {
                    m.SendLocalizedMessage(1070905); // Strong magics have redirected you to a safer location!
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
                : base(map.SetLocation == Point3D.Zero ? 1154558 : 1154636, -1) // Teleport To Vendor : Return to Previous Location
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
