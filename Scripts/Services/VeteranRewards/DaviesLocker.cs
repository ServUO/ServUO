using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Multis;
using Server.Network;
using Server.Targeting;
using System;
using System.Collections.Generic;

namespace Server.Engines.VeteranRewards
{
    public class DaviesLockerAddon : BaseAddon, ISecurable
    {
        public override BaseAddonDeed Deed => new DaviesLockerAddonDeed(Entries);

        public List<DaviesLockerEntry> Entries { get; private set; } = new List<DaviesLockerEntry>();

        [CommandProperty(AccessLevel.GameMaster)]
        public bool South { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level { get; set; }

        [Constructable]
        public DaviesLockerAddon(bool south, List<DaviesLockerEntry> list)
        {
            South = south;
            Entries = list;
            Level = SecureLevel.CoOwners;

            if (south)
            {
                AddComponent(new DaviesLockerComponent(19455), 0, 0, 0);
                AddComponent(new DaviesLockerComponent(19456), 1, 0, 0);
                AddComponent(new DaviesLockerComponent(19457), 2, 0, 0);
                AddComponent(new DaviesLockerComponent(19453), 0, 1, 0);
                AddComponent(new DaviesLockerComponent(19452), 1, 1, 0);
                AddComponent(new DaviesLockerComponent(19454), 2, 1, 0);
            }
            else
            {
                AddComponent(new DaviesLockerComponent(19449), 0, 0, 0);
                AddComponent(new DaviesLockerComponent(19448), 1, 0, 0);
                AddComponent(new DaviesLockerComponent(19450), 0, 1, 0);
                AddComponent(new DaviesLockerComponent(19447), 1, 1, 0);
                AddComponent(new DaviesLockerComponent(19451), 0, 2, 0);
                AddComponent(new DaviesLockerComponent(19446), 1, 2, 0);
            }
        }

        public override void OnComponentUsed(AddonComponent component, Mobile from)
        {
            if (from.InRange(component.Location, 2))
            {
                if (CanUse(from))
                {
                    from.CloseGump(typeof(DaviesLockerGump));
                    from.SendGump(new DaviesLockerGump(from, this));
                }
                else
                {
                    from.SendLocalizedMessage(503301, "", 0x22); // You don't have permission to do that.
                }
            }
        }

        public bool CanUse(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            return house != null && house.HasSecureAccess(from, Level);
        }

        public void TryAddEntry(Item item, Mobile from)
        {
            if (!CanUse(from) || item == null)
                return;

            if (!CheckRange(from))
                from.SendLocalizedMessage(3000268); // that is too far away.
            else if (!(item is TreasureMap || item is SOS || item is MessageInABottle))
                from.SendLocalizedMessage(1153564); // That is not a treasure map or message in a bottle.
            else if (!item.IsChildOf(from.Backpack))
                from.SendLocalizedMessage(1054107); // This item must be in your backpack.
            else if (Entries.Count >= 500)
                from.SendLocalizedMessage(1153565); // The locker is full
            else
            {
                DaviesLockerEntry entry = null;

                if (item is TreasureMap)
                    entry = new TreasureMapEntry((TreasureMap)item);
                else if (item is SOS)
                    entry = new SOSEntry((SOS)item);
                else if (item is MessageInABottle)
                    entry = new SOSEntry((MessageInABottle)item);

                if (entry != null)
                {
                    Entries.Add(entry);
                    from.CloseGump(typeof(DaviesLockerGump));
                    from.SendGump(new DaviesLockerGump(from, this));

                    item.Delete();

                    UpdateProperties();
                }
            }
        }

        private bool CheckRange(Mobile m)
        {
            if (Components == null || m.Map != Map)
                return false;

            foreach (AddonComponent c in Components)
            {
                if (m.InRange(c.Location, 2))
                    return true;
            }

            return false;
        }

        public DaviesLockerAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version

            writer.Write(South);
            writer.Write((int)Level);

            writer.Write(Entries.Count);
            foreach (DaviesLockerEntry entry in Entries)
            {
                if (entry is SOSEntry)
                    writer.Write(0);
                else if (entry is TreasureMapEntry)
                    writer.Write(1);
                else
                {
                    writer.Write(2);
                    continue;
                }

                entry.Serialize(writer);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Entries = new List<DaviesLockerEntry>();

            South = reader.ReadBool();
            Level = (SecureLevel)reader.ReadInt();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                switch (reader.ReadInt())
                {
                    case 0:
                        Entries.Add(new SOSEntry(reader));
                        break;
                    case 1:
                        Entries.Add(new TreasureMapEntry(reader));
                        break;
                    case 2: break;
                }
            }
        }

        public class DaviesLockerComponent : LocalizedAddonComponent
        {
            public override bool ForceShowProperties => true;

            public DaviesLockerComponent(int id)
                : base(id, 1153534) // Davies' Locker
            {
            }

            public override bool OnDragDrop(Mobile from, Item dropped)
            {
                if (Addon is DaviesLockerAddon && (dropped is SOS || dropped is TreasureMap))
                    ((DaviesLockerAddon)Addon).TryAddEntry(dropped as Item, from);

                return false;
            }

            public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
            {
                base.GetContextMenuEntries(from, list);

                if (Addon is DaviesLockerAddon)
                    SetSecureLevelEntry.AddTo(from, (DaviesLockerAddon)Addon, list);
            }

            public override void GetProperties(ObjectPropertyList list)
            {
                base.GetProperties(list);

                if (Addon is DaviesLockerAddon)
                {
                    list.Add(1153648, ((DaviesLockerAddon)Addon).Entries.Count.ToString());
                }
            }

            public DaviesLockerComponent(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write(0); // Version
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadInt();
            }
        }
    }

    public class DaviesLockerAddonDeed : BaseAddonDeed, IRewardOption
    {
        public override BaseAddon Addon => new DaviesLockerAddon(South, Entries);
        public override int LabelNumber => 1153535;  // Deed to Davies' Locker

        public List<DaviesLockerEntry> Entries { get; private set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool South { get; private set; }

        [Constructable]
        public DaviesLockerAddonDeed()
            : this(null)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(RewardOptionGump));
                from.SendGump(new RewardOptionGump(this));
            }
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
        }

        public DaviesLockerAddonDeed(List<DaviesLockerEntry> list)
        {
            if (list == null)
                Entries = new List<DaviesLockerEntry>();
            else
                Entries = list;

            LootType = LootType.Blessed;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1076224); // 8th Year Veteran Reward	

            list.Add(1153648, Entries.Count.ToString()); // ~1_COUNT~ maps
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add(0, 1153587); // Long edge facing south 
            list.Add(1, 1153588); // Long edge facing east
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            South = choice == 0;

            if (!Deleted)
                base.OnDoubleClick(from);
        }

        public DaviesLockerAddonDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version

            writer.Write(South);

            writer.Write(Entries.Count);
            foreach (DaviesLockerEntry entry in Entries)
            {
                if (entry is SOSEntry)
                    writer.Write(0);
                else if (entry is TreasureMapEntry)
                    writer.Write(1);
                else
                {
                    writer.Write(2);
                    continue;
                }

                entry.Serialize(writer);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Entries = new List<DaviesLockerEntry>();
            South = reader.ReadBool();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                switch (reader.ReadInt())
                {
                    case 0:
                        Entries.Add(new SOSEntry(reader));
                        break;
                    case 1:
                        Entries.Add(new TreasureMapEntry(reader));
                        break;
                    case 2: break;
                }
            }
        }
    }

    public class DaviesLockerEntry
    {
        public Map Map { get; set; }
        public Point3D Location { get; set; }
        public int Level { get; set; }
        public bool QuestItem { get; set; }

        public DaviesLockerEntry(Map map, Point3D location, int level)
        {
            Map = map;
            Location = location;
            Level = level;
        }

        public DaviesLockerEntry(GenericReader reader)
        {
            int v = reader.ReadInt();

            switch (v)
            {
                case 1:
                    QuestItem = reader.ReadBool();
                    goto case 0;
                case 0:
                    Map = reader.ReadMap();
                    Location = reader.ReadPoint3D();
                    Level = reader.ReadInt();
                    break;
            }
        }

        public virtual void Serialize(GenericWriter writer)
        {
            writer.Write(1);

            writer.Write(QuestItem);

            writer.Write(Map);
            writer.Write(Location);
            writer.Write(Level);
        }
    }

    public class SOSEntry : DaviesLockerEntry
    {
        public bool IsAncient { get; set; }
        public int MessageIndex { get; set; }
        public bool Opened { get; set; }

        public SOSEntry(MessageInABottle mib)
            : base(mib.TargetMap, Point3D.Zero, mib.Level)
        {
            Opened = false;
            IsAncient = false;
            MessageIndex = -1;

            if (mib is SaltySeaMIB)
                QuestItem = true;
        }

        public SOSEntry(SOS sos)
            : base(sos.TargetMap, sos.TargetLocation, sos.Level)
        {
            Opened = true;
            IsAncient = sos.IsAncient;
            MessageIndex = sos.MessageIndex;

            if (sos is SaltySeaSOS)
                QuestItem = true;
        }

        public SOSEntry(GenericReader reader)
            : base(reader)
        {
            int v = reader.ReadInt();

            IsAncient = reader.ReadBool();
            MessageIndex = reader.ReadInt();
            Opened = reader.ReadBool();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(IsAncient);
            writer.Write(MessageIndex);
            writer.Write(Opened);
        }
    }

    public class TreasureMapEntry : DaviesLockerEntry
    {
        public bool Completed { get; set; }
        public Mobile CompletedBy { get; set; }
        public Mobile Decoder { get; set; }
        public DateTime NextReset { get; set; }
        public TreasurePackage Package { get; set; }

        public TreasureMapEntry(TreasureMap map)
            : base(map.Facet, new Point3D(map.ChestLocation.X, map.ChestLocation.Y, 0), map.Level)
        {
            Completed = map.Completed;
            CompletedBy = map.CompletedBy;
            Decoder = map.Decoder;
            NextReset = map.NextReset;
            Package = map.Package;

            if (map is HiddenTreasuresTreasureMap)
                QuestItem = true;
        }

        public TreasureMapEntry(GenericReader reader) : base(reader)
        {
            int v = reader.ReadInt();

            switch (v)
            {
                case 1:
                    Package = (TreasurePackage)reader.ReadInt();
                    goto case 0;
                case 0:
                    Completed = reader.ReadBool();
                    CompletedBy = reader.ReadMobile();
                    Decoder = reader.ReadMobile();
                    NextReset = reader.ReadDateTime();
                    break;
            }

            if (v == 0)
            {
                Package = (TreasurePackage)Utility.RandomMinMax(0, 4);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write((int)Package);

            writer.Write(Completed);
            writer.Write(CompletedBy);
            writer.Write(Decoder);
            writer.Write(NextReset);
        }
    }

    public class DaviesLockerGump : Gump
    {
        private readonly int Blue = 0x431F;
        private readonly int AquaGreen = 0x43F8;
        private readonly int Yellow = 0x7FF0;

        private readonly List<DaviesLockerEntry> m_List = new List<DaviesLockerEntry>();
        private int m_Page;
        private readonly DaviesLockerAddon m_Addon;

        public DaviesLockerGump(Mobile from, DaviesLockerAddon addon)
            : this(from, addon, 0)
        {
        }

        public DaviesLockerGump(Mobile from, DaviesLockerAddon addon, int page)
            : base(100, 100)
        {
            if (addon == null || addon.Deleted)
                return;

            AddImage(0, 0, 0x5C1);
            m_List = addon.Entries;
            m_Addon = addon;

            AddHtmlLocalized(10, 10, 580, 20, 1153552, AquaGreen, false, false); // <DIV ALIGN="CENTER">Davies' Locker</DIV>

            AddHtmlLocalized(35, 40, 35, 20, 1153554, Blue, false, false); // <DIV ALIGN="CENTER">Get</DIV>
            AddHtmlLocalized(78, 40, 110, 20, 1153555, Blue, false, false); // <DIV ALIGN="CENTER">Facet</DIV>
            AddHtmlLocalized(198, 40, 110, 20, 1153556, Blue, false, false); // <DIV ALIGN="CENTER">Level</DIV>
            AddHtmlLocalized(373, 40, 90, 20, 1153557, Blue, false, false); // <DIV ALIGN="CENTER">Coords</DIV>
            AddHtmlLocalized(473, 40, 110, 20, 1153558, Blue, false, false); // <DIV ALIGN="CENTER">Status</DIV>

            int perPage = 10;
            int totalPages = (int)Math.Ceiling(m_List.Count / 10.0);

            if (totalPages < 1)
                totalPages = 1;

            if (page < 0)
                page = 0;

            if (page + 1 > totalPages)
                page = totalPages - 1;

            m_Page = page;

            int start = page * perPage;

            AddHtmlLocalized(35, 430, 280, 20, 1153560, string.Format("{0}@{1}", m_List.Count, "500"), Blue, false, false); // Maps: ~1_NUM~ of ~2_MAX~
            AddHtmlLocalized(35, 450, 280, 20, 1153561, string.Format("{0}@{1}", (page + 1).ToString(), (totalPages).ToString()), Blue, false, false); // Page ~1_CUR~ of ~2_MAX~

            AddHtmlLocalized(390, 430, 100, 20, 1153553, Yellow, false, false); // <DIV ALIGN="CENTER">ADD MAPS</DIV>
            AddButton(350, 430, 4011, 4013, 3, GumpButtonType.Reply, 0);

            AddHtmlLocalized(390, 450, 100, 20, 1153562, Yellow, false, false); // <DIV ALIGN="CENTER">PAGE</DIV>
            AddButton(350, 450, 4014, 4016, 1, GumpButtonType.Reply, 0);
            AddButton(500, 450, 4005, 4007, 2, GumpButtonType.Reply, 0);

            AddButton(330, 453, 5603, 5607, 11, GumpButtonType.Reply, 0);
            AddButton(534, 453, 5601, 5605, 12, GumpButtonType.Reply, 0);

            int y = 73;
            int index = 0;

            for (int i = start; i >= 0 && i < m_List.Count && index < perPage; i++)
            {
                DaviesLockerEntry entry = m_List[i];

                if (entry == null)
                    continue;

                if (addon.CanUse(from))
                    AddButton(45, y + 4, 1209, 1210, 1000 + i, GumpButtonType.Reply, 0);

                AddHtmlLocalized(78, y, 110, 20, GetFacet(entry), Yellow, false, false);

                if (TreasureMapInfo.NewSystem && entry is TreasureMapEntry)
                {
                    AddHtmlLocalized(174, y, 110, 20, GetPackage((TreasureMapEntry)entry), Yellow, false, false);
                    AddHtmlLocalized(268, y, 110, 20, GetLevel((TreasureMapEntry)entry), Yellow, false, false);
                }
                else
                {
                    AddHtmlLocalized(268, y, 110, 20, GetLevel(entry), Yellow, false, false);
                }

                if ((entry is TreasureMapEntry && ((TreasureMapEntry)entry).Decoder == null) || (entry is SOSEntry && !((SOSEntry)entry).Opened))
                    AddHtmlLocalized(373, y, 90, 20, 1153569, Yellow, false, false); // Unknown
                else
                    AddHtmlLocalized(373, y, 90, 20, 1060847, GetLocation(entry), Yellow, false, false); // ~1_val~ ~2_val~

                AddHtmlLocalized(473, y, 100, 20, GetStatus(entry), Yellow, false, false);

                y += 35;
                index++;
            }
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            Mobile from = state.Mobile;

            if (!m_Addon.CanUse(from))
                return;

            switch (info.ButtonID)
            {
                case 0: return;
                case 1: // PAGE BACK
                    m_Page--;
                    break;
                case 2: // PAGE FORWARD
                    m_Page++;
                    break;
                case 3: // ADD MAPS
                    {
                        from.Target = new InternalTarget(from, m_Addon, m_Page);
                        from.SendLocalizedMessage(1153563); // Target maps in your backpack or a sub-container to add them to the Locker. When done, press ESC.
                        return;
                    }
                case 11:
                    m_Page = m_Page - 11;
                    break;
                case 12:
                    m_Page = m_Page + 11;
                    break;
                default:
                    {
                        int index = info.ButtonID - 1000;

                        if (index >= 0 && index < m_List.Count)
                        {
                            DaviesLockerEntry entry = m_List[index];

                            if (entry != null)
                                ConstructEntry(from, entry);
                        }
                    }
                    break;
            }

            from.SendGump(new DaviesLockerGump(from, m_Addon, m_Page));
        }

        public string GetLocation(DaviesLockerEntry e)
        {
            string loc;

            // Location labels
            int xLong = 0, yLat = 0;
            int xMins = 0, yMins = 0;
            bool xEast = false, ySouth = false;

            if (Sextant.Format(e.Location, e.Map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth))
            {
                loc = string.Format("{0}@{1}", yLat + (ySouth ? "S" : "N"), xLong + (xEast ? "E" : "W"));
            }
            else
            {
                loc = string.Format("{0}@{1}", e.Location.X.ToString(), e.Location.Y.ToString());
            }

            return loc;
        }

        private int GetPackage(TreasureMapEntry entry)
        {
            switch (entry.Package)
            {
                default:
                case TreasurePackage.Artisan: return 1159000;
                case TreasurePackage.Assassin: return 1158998;
                case TreasurePackage.Mage: return 1158997;
                case TreasurePackage.Ranger: return 1159002;
                case TreasurePackage.Warrior: return 1158999;
            }
        }

        private int GetLevel(TreasureMapEntry entry)
        {
            return 1158992 + Math.Min(4, entry.Level);
        }

        private int GetFacet(DaviesLockerEntry entry)
        {
            Map map = entry.Map;

            if (map == Map.Felucca)
                return 1012001; // Felucca
            else if (map == Map.Trammel)
                return 1012000; // Trammel
            else if (map == Map.Ilshenar)
                return 1012002; // Ilshenar
            else if (map == Map.Malas)
                return 1060643; // Malas
            else if (map == Map.Tokuno)
                return 1063258; // Tokuno Islands
            else if (map == Map.TerMur)
                return 1112178; // Ter Mur
            else
                return 1074235; // Unknown
        }

        private void ConstructEntry(Mobile from, DaviesLockerEntry entry)
        {
            Item item = null;

            if (entry is SOSEntry)
                item = Construct((SOSEntry)entry);
            else if (entry is TreasureMapEntry)
                item = Construct((TreasureMapEntry)entry);

            if (item != null)
            {
                Container pack = from.Backpack;

                if (pack == null || !pack.TryDropItem(from, item, false))
                    item.Delete();
                else
                {
                    if (m_List.Contains(entry))
                    {
                        m_List.Remove(entry);
                        m_Addon.UpdateProperties();
                    }
                }
            }
        }

        private Item Construct(SOSEntry entry)
        {
            if (entry == null)
                return null;

            if (entry.Opened)
            {
                SOS sos;

                if (entry.QuestItem)
                    sos = new SaltySeaSOS(entry.Map, entry.Level);
                else
                    sos = new SOS(entry.Map, entry.Level);

                sos.MessageIndex = entry.MessageIndex;
                sos.TargetLocation = entry.Location;
                return sos;
            }
            else
            {
                MessageInABottle mib;

                if (entry.QuestItem)
                    mib = new SaltySeaMIB(entry.Map, entry.Level);
                else
                    mib = new MessageInABottle(entry.Map, entry.Level);

                return mib;
            }
        }

        private TreasureMap Construct(TreasureMapEntry entry)
        {
            if (entry == null)
                return null;

            TreasureMap map;

            if (entry.QuestItem)
                map = new HiddenTreasuresTreasureMap(entry.Level, entry.Map, new Point2D(entry.Location.X, entry.Location.Y));
            else
            {
                map = new TreasureMap
                {
                    Facet = entry.Map,
                    Level = entry.Level,
                    Package = entry.Package,
                    ChestLocation = new Point2D(entry.Location.X, entry.Location.Y)
                };
            }

            bool eodon = map.TreasureFacet == TreasureFacet.Eodon;

            map.Completed = entry.Completed;
            map.CompletedBy = entry.CompletedBy;
            map.Decoder = entry.Decoder;
            map.NextReset = entry.NextReset;

            map.Width = 300;
            map.Height = 300;
            int x = entry.Location.X;
            int y = entry.Location.Y;
            Map facet = entry.Map;

            map.GetWidthAndHeight(facet, out int width, out int height);

            int x1 = x - Utility.RandomMinMax(width / 4, (width / 4) * 3);
            int y1 = y - Utility.RandomMinMax(height / 4, (height / 4) * 3);

            if (x1 < 0) x1 = 0;
            if (y1 < 0) y1 = 0;

            map.AdjustMap(facet, out int x2, out int y2, x1, y1, width, height, eodon);

            x1 = x2 - width;
            y1 = y2 - height;

            map.Bounds = new Rectangle2D(x1, y1, width, height);
            map.Protected = true;

            map.AddWorldPin(x, y);

            return map;
        }

        private int GetLevel(DaviesLockerEntry entry)
        {
            if (entry is SOSEntry)
                return 1153568; // S-O-S
            else if (entry is TreasureMapEntry)
                return 1153572 + entry.Level;

            return 1153569; // Unknown
        }

        private int GetStatus(DaviesLockerEntry entry)
        {
            if (entry is SOSEntry sosEntry)
            {
                if (!sosEntry.Opened)
                    return 1153570; // Unopened

                if (sosEntry.IsAncient)
                    return 1153572; // Ancient

                return 1153571; // Opened
            }
            else if (entry is TreasureMapEntry mapEntry)
            {
                if (mapEntry.Completed)
                    return 1153582; // Completed
                else if (mapEntry.Decoder != null)
                    return 1153581; // Decoded
                else
                    return 1153580; // Not Decoded
            }

            return 1153569; // Unknown
        }

        private class InternalTarget : Target
        {
            private readonly DaviesLockerAddon m_Addon;
            private readonly int m_Page;

            public InternalTarget(Mobile from, DaviesLockerAddon addon, int page)
                : base(-1, false, TargetFlags.None)
            {
                m_Addon = addon;
                m_Page = page;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Addon != null && !m_Addon.Deleted && targeted is Item)
                {
                    m_Addon.TryAddEntry(targeted as Item, from);
                    from.Target = new InternalTarget(from, m_Addon, m_Page);
                }
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                if (m_Addon != null && !m_Addon.Deleted)
                {
                    from.CloseGump(typeof(DaviesLockerGump));
                    from.SendGump(new DaviesLockerGump(from, m_Addon, m_Page));
                }
            }
        }
    }
}
