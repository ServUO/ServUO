using Server.ContextMenus;
using Server.Engines.Auction;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using System.Collections.Generic;

namespace Server.Items
{
    public class AuctionMap : MapItem
    {
        public readonly int TeleportCost = 1000;
        public readonly int DeleteDelayMinutes = 30;

        [CommandProperty(AccessLevel.GameMaster)]
        public IAuctionItem AuctionSafe { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Item AuctionItem { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D SafeLocation { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Map SafeMap { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public string HouseName { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D SetLocation { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public Map SetMap { get; set; }

        public AuctionMap(IAuctionItem auctionsafe)
            : base(auctionsafe.Map)
        {
            AuctionSafe = auctionsafe;
            AuctionItem = auctionsafe.Auction.AuctionItem;
            SafeLocation = auctionsafe.Location;
            SafeMap = auctionsafe.Map;

            BaseHouse house = GetHouse();

            if (house != null)
            {
                HouseName = house.Sign.GetName();
            }
            else
            {
                HouseName = "Unknown";
            }

            Hue = RecallRune.CalculateHue(auctionsafe.Map, null, true);
            LootType = LootType.Blessed;

            Width = 400;
            Height = 400;

            Bounds = new Rectangle2D(auctionsafe.X - 100, auctionsafe.Y - 100, 200, 200);
            AddWorldPin(auctionsafe.X, auctionsafe.Y);
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
            if (AuctionItem == null)
            {
                list.Add(1156474, string.Format("{0}\t{1}", HouseName, "Unknown")); // Map to Auction ~1_ITEMNAME~: ~2_HOUSE~
            }
            else if (AuctionItem.LabelNumber != 0)
            {
                list.Add(1156474, string.Format("{0}\t#{1}", HouseName, AuctionItem.LabelNumber)); // Map to Auction ~1_ITEMNAME~: ~2_HOUSE~
            }
            else
            {
                list.Add(1156474, string.Format("{0}\t{1}", HouseName, AuctionItem.Name)); // Map to Auction ~1_ITEMNAME~: ~2_HOUSE~
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            string[] coord = GetCoords();

            list.Add(1154639, string.Format("{0}\t{1}", coord[0], coord[1])); //  Vendor Located at ~1_loc~ (~2_facet~)

            if (!CheckItem())
            {
                list.Add(1154700); // Item no longer for sale.
            }

            list.Add(1075269); // Destroyed when dropped
        }

        public string[] GetCoords()
        {
            string[] array = new string[2];

            Point3D loc = Point3D.Zero;
            Map locmap = Map.Internal;

            if (SetLocation != Point3D.Zero && SetMap != Map.Internal)
            {
                loc = SetLocation;
                locmap = SetMap;
            }
            else if (SafeLocation != Point3D.Zero && SafeMap != Map.Internal)
            {
                loc = SafeLocation;
                locmap = SafeMap;
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
                    return new string[] { string.Format("{0}° {1}'{2}, {3}° {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W"), map.ToString() };
                }
            }

            return new string[] { "an unknown location", "Unknown" };
        }

        public BaseHouse GetHouse()
        {
            if (AuctionSafe != null)
                return BaseHouse.FindHouseAt(AuctionSafe);

            return null;
        }

        public void OnBeforeTravel(Mobile from)
        {
            Banker.Withdraw(from, TeleportCost);
            from.SendLocalizedMessage(1060398, TeleportCost.ToString()); // ~1_AMOUNT~ gold has been withdrawn from your bank box.

            if (SetLocation != Point3D.Zero)
            {
                SetLocation = Point3D.Zero;
                SetMap = null;
            }
            else
            {
                SetLocation = from.Location;
                SetMap = from.Map;
            }

            InvalidateProperties();
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            list.Add(new OpenMapEntry(from, this));
            list.Add(new TeleportEntry(from, this));
        }

        public bool CheckItem()
        {
            return GetHouse() != null && AuctionItem != null && AuctionSafe.CheckAuctionItem(AuctionItem);
        }

        public Point3D GetLocation(Mobile m)
        {
            BaseHouse h = null;

            if (SetLocation != Point3D.Zero)
            {
                h = BaseHouse.FindHouseAt(SetLocation, SetMap, 16);
            }
            else if (AuctionSafe != null)
            {
                h = BaseHouse.FindHouseAt(AuctionSafe);
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

            if (AuctionSafe != null)
            {
                return AuctionSafe.Map;
            }

            return null;
        }

        public class OpenMapEntry : ContextMenuEntry
        {
            public AuctionMap VendorMap { get; set; }
            public Mobile Clicker { get; set; }

            public OpenMapEntry(Mobile from, AuctionMap map)
                : base(3006150, -1) // Open Map
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
            public AuctionMap VendorMap { get; set; }
            public Mobile Clicker { get; set; }

            public TeleportEntry(Mobile from, AuctionMap map)
                : base(1154558, -1) // Teleport To Vendor
            {
                VendorMap = map;
                Clicker = from;

                Enabled = map.CheckItem();
            }

            public override void OnClick()
            {
                if (Clicker is PlayerMobile)
                {
                    BaseGump.SendGump(new ConfirmTeleportGump(VendorMap, (PlayerMobile)Clicker));
                }
            }
        }

        public AuctionMap(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(AuctionSafe as Item);
            writer.Write(AuctionItem);
            writer.Write(SafeLocation);
            writer.Write(SafeMap);
            writer.Write(HouseName);
            writer.Write(SetLocation);
            writer.Write(SetMap);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            AuctionSafe = reader.ReadItem() as IAuctionItem;
            AuctionItem = reader.ReadItem();
            SafeLocation = reader.ReadPoint3D();
            SafeMap = reader.ReadMap();
            HouseName = reader.ReadString();
            SetLocation = reader.ReadPoint3D();
            SetMap = reader.ReadMap();
        }

        public class ConfirmTeleportGump : BaseGump
        {
            public AuctionMap AuctionMap { get; set; }

            public ConfirmTeleportGump(AuctionMap map, PlayerMobile pm)
                : base(pm, 10, 10)
            {
                AuctionMap = map;
            }

            public override void AddGumpLayout()
            {
                AddPage(0);

                AddBackground(0, 0, 414, 214, 0x7752);

                AddHtmlLocalized(27, 47, 380, 80, 1156475, string.Format("@{0}@{1}@{2}", AuctionMap.TeleportCost.ToString(), AuctionMap.HouseName, AuctionMap.DeleteDelayMinutes.ToString()), 0xFFFF, false, false); // Please select 'Accept' if you would like to pay ~1_cost~ gold to teleport to auction house ~2_name~. For this price you will also be able to teleport back to this location within the next ~3_minutes~ minutes.

                AddButton(7, 167, 0x7747, 0x7747, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(47, 167, 100, 40, 1150300, 0x4E73, false, false); // CANCEL

                AddButton(377, 167, 0x7746, 0x7746, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(267, 167, 100, 40, 1114514, "#1150299", 0xFFFF, false, false); // // <DIV ALIGN=RIGHT>~1_TOKEN~</DIV>
            }

            public override void OnResponse(RelayInfo info)
            {
                switch (info.ButtonID)
                {
                    default: break;
                    case 1:
                        {
                            if (Banker.GetBalance(User) < AuctionMap.TeleportCost)
                            {
                                User.SendLocalizedMessage(1154672); // You cannot afford to teleport to the vendor.
                            }
                            else if (!AuctionMap.CheckItem())
                            {
                                User.SendLocalizedMessage(1154643); // That item is no longer for sale.
                            }
                            else if (AuctionMap.SetLocation != Point3D.Zero && (!Utility.InRange(AuctionMap.SetLocation, User.Location, 100) || AuctionMap.SetMap != User.Map))
                            {
                                User.SendLocalizedMessage(501035); // You cannot teleport from here to the destination.
                            }
                            else
                            {
                                new Spells.Fourth.RecallSpell(User, AuctionMap, AuctionMap).Cast();
                            }

                            break;
                        }
                }
            }
        }
    }
}
