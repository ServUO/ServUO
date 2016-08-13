using System;
using Server;
using Server.Mobiles;
using Server.Multis;
using System.Collections.Generic;
using Server.Network;
using Server.Targeting;
using Server.Items;
using Server.Gumps;
using Server.ContextMenus;

namespace Server.Engines.VeteranRewards
{
    public class DaviesLockerAddon : BaseAddon, ISecurable
    {
        public override BaseAddonDeed Deed { get { return new DaviesLockerAddonDeed(m_South, m_Entries); } }

        private List<DaviesLockerEntry> m_Entries = new List<DaviesLockerEntry>();
        public List<DaviesLockerEntry> Entries { get { return m_Entries; } }

        private bool m_South;
        private SecureLevel m_Level;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool South
        {
            get { return m_South; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }

        [Constructable]
        public DaviesLockerAddon(bool south, List<DaviesLockerEntry> list)
        {
            m_South = south;
            m_Entries = list;
            m_Level = SecureLevel.CoOwners;

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

        /*public override void OnChop(Mobile from)
        {
            if (m_Entries.Count == 0)
                base.OnChop(from);
            else
                from.SendMessage("You cannot re-deed this addon unless it is emtpy!");
        }*/

        public override void OnComponentUsed(AddonComponent component, Mobile from)
        {
            if (from.InRange(component.Location, 2) && CanUse(from))
            {
                from.CloseGump(typeof(DaviesLockerGump));
                from.SendGump(new DaviesLockerGump(from, this));
            }
        }

        public bool CanUse(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            return house != null && house.HasSecureAccess(from, m_Level);
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
            else if (m_Entries.Count >= 500)
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
                    m_Entries.Add(entry);
                    from.CloseGump(typeof(DaviesLockerGump));
                    from.SendGump(new DaviesLockerGump(from, this));

                    item.Delete();

                    InvalidateProperties();
                }
            }
        }

        private bool CheckRange(Mobile m)
        {
            if (Components == null || m.Map != this.Map)
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

            writer.Write(m_South);
            writer.Write((int)m_Level);

            writer.Write(m_Entries.Count);
            foreach (DaviesLockerEntry entry in m_Entries)
            {
                if (entry is SOSEntry)
                    writer.Write((int)0);
                else if (entry is TreasureMapEntry)
                    writer.Write((int)1);
                else
                {
                    writer.Write((int)2);
                    continue;
                }

                entry.Serialize(writer);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Entries = new List<DaviesLockerEntry>();

            m_South = reader.ReadBool();
            m_Level = (SecureLevel)reader.ReadInt();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                switch (reader.ReadInt())
                {
                    case 0:
                        m_Entries.Add(new SOSEntry(reader));
                        break;
                    case 1:
                        m_Entries.Add(new TreasureMapEntry(reader));
                        break;
                    case 2: break;
                }
            }
        }

        public class DaviesLockerComponent : LocalizedAddonComponent
        {
            public DaviesLockerComponent(int id)
                : base(id, 1153534) // Davies' Locker
            {
            }

            public override bool OnDragDrop(Mobile from, Item dropped)
            {
                if (this.Addon is DaviesLockerAddon && (dropped is SOS || dropped is TreasureMap))
                    ((DaviesLockerAddon)this.Addon).TryAddEntry(dropped as Item, from);

                return false;
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

    public class DaviesLockerAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new DaviesLockerAddon(m_South, m_Entries); } }
        public override int LabelNumber { get { return 1153535; } } // deed to davies' locker

        private List<DaviesLockerEntry> m_Entries;
        public List<DaviesLockerEntry> Entries { get { return m_Entries; } }

        private bool m_South;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool South
        {
            get { return m_South; }
        }

        [Constructable]
        public DaviesLockerAddonDeed() : this(true, null)
        {
        }

        [Constructable]
        public DaviesLockerAddonDeed(bool south) : this(south, null)
        {
        }

        [Constructable]
        public DaviesLockerAddonDeed(bool south, List<DaviesLockerEntry> list)
        {
            m_South = south;

            if (list == null)
                m_Entries = new List<DaviesLockerEntry>();
            else
                m_Entries = list;

            LootType = LootType.Blessed;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(m_South ? "South" : "East");
            list.Add(1153648, m_Entries.Count.ToString()); // ~1_COUNT~ maps
        }

        public DaviesLockerAddonDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version

            writer.Write(m_South);

            writer.Write(m_Entries.Count);
            foreach (DaviesLockerEntry entry in m_Entries)
            {
                if (entry is SOSEntry)
                    writer.Write((int)0);
                else if (entry is TreasureMapEntry)
                    writer.Write((int)1);
                else
                {
                    writer.Write((int)2);
                    continue;
                }

                entry.Serialize(writer);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Entries = new List<DaviesLockerEntry>();
            m_South = reader.ReadBool();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                switch (reader.ReadInt())
                {
                    case 0:
                        m_Entries.Add(new SOSEntry(reader));
                        break;
                    case 1:
                        m_Entries.Add(new TreasureMapEntry(reader));
                        break;
                    case 2: break;
                }
            }
        }
    }

    public class DaviesLockerEntry
    {
        private Map m_Map;
        private Point3D m_Location;
        private int m_Level;
        private bool m_QuestItem;

        public Map Map { get { return m_Map; } }
        public Point3D Location { get { return m_Location; } }
        public int Level { get { return m_Level; } }
        public bool QuestItem { get { return m_QuestItem; } set { m_QuestItem = value; } }

        public DaviesLockerEntry(Map map, Point3D location, int level)
        {
            m_Map = map;
            m_Location = location;
            m_Level = level;
        }

        public DaviesLockerEntry(GenericReader reader)
        {
            int v = reader.ReadInt();

            m_Map = reader.ReadMap();
            m_Location = reader.ReadPoint3D();
            m_Level = reader.ReadInt();
        }

        public virtual void Serialize(GenericWriter writer)
        {
            writer.Write((int)0);

            writer.Write(m_Map);
            writer.Write(m_Location);
            writer.Write(m_Level);
        }
    }

    public class SOSEntry : DaviesLockerEntry
    {
        private bool m_IsAncient;
        private int m_MessageIndex;
        private bool m_Opened;

        public bool IsAncient { get { return m_IsAncient; } }
        public int MessageIndex { get { return m_MessageIndex; } }
        public bool Opened { get { return m_Opened; } }

        public SOSEntry(MessageInABottle mib) : base(mib.TargetMap, Point3D.Zero, mib.Level)
        {
            m_Opened = false;
            m_IsAncient = false;
            m_MessageIndex = -1;
        }

        public SOSEntry(SOS sos) : base(sos.TargetMap, sos.TargetLocation, sos.Level)
        {
            m_Opened = true;
            m_IsAncient = sos.IsAncient;
            m_MessageIndex = sos.MessageIndex;
        }

        public SOSEntry(GenericReader reader) : base(reader)
        {
            int v = reader.ReadInt();

            m_IsAncient = reader.ReadBool();
            m_MessageIndex = reader.ReadInt();
            m_Opened = reader.ReadBool();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(m_IsAncient);
            writer.Write(m_MessageIndex);
            writer.Write(m_Opened);
        }
    }

    public class TreasureMapEntry : DaviesLockerEntry
    {
        private bool m_Completed;
        private Mobile m_CompletedBy;
        private Mobile m_Decoder;
        private DateTime m_NextReset;

        public bool Completed { get { return m_Completed; } }
        public Mobile CompletedBy { get { return m_CompletedBy; } }
        public Mobile Decoder { get { return m_Decoder; } }
        public DateTime NextReset { get { return m_NextReset; } }

        public TreasureMapEntry(TreasureMap map) : base(map.Facet, new Point3D(map.ChestLocation.X, map.ChestLocation.Y, 0), map.Level)
        {
            m_Completed = map.Completed;
            m_CompletedBy = map.CompletedBy;
            m_Decoder = map.Decoder;
            m_NextReset = map.NextReset;
        }

        public TreasureMapEntry(GenericReader reader) : base(reader)
        {
            int v = reader.ReadInt();

            m_Completed = reader.ReadBool();
            m_CompletedBy = reader.ReadMobile();
            m_Decoder = reader.ReadMobile();
            m_NextReset = reader.ReadDateTime();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(m_Completed);
            writer.Write(m_CompletedBy);
            writer.Write(m_Decoder);
            writer.Write(m_NextReset);
        }
    }

    public class DaviesLockerGump : Gump
    {
        private readonly int Blue = 0x99FF;
        private readonly int AquaGreen = 0x03EF;
        private readonly int Yellow = 0xFFE0;

        private List<DaviesLockerEntry> m_List = new List<DaviesLockerEntry>();
        private int m_Page;
        private DaviesLockerAddon m_Addon;

        public DaviesLockerGump(Mobile from, DaviesLockerAddon addon) : this(from, addon, 0)
        {
        }

        public DaviesLockerGump(Mobile from, DaviesLockerAddon addon, int page) : base(50, 50)
        {
            if (addon == null || addon.Deleted)
                return;

            AddImage(0, 0, 0x5C1);
            m_List = addon.Entries;
            m_Addon = addon;

            AddHtmlLocalized(0, 10, 600, 20, 1153552, AquaGreen, false, false); // <DIV ALIGN="CENTER">Davies' Locker</DIV>

            AddHtmlLocalized(30, 35, 40, 20, 1153554, Blue, false, false); // <DIV ALIGN="CENTER">Get</DIV>
            AddHtmlLocalized(75, 35, 120, 20, 1153555, Blue, false, false); // <DIV ALIGN="CENTER">Facet</DIV>
            AddHtmlLocalized(195, 35, 120, 20, 1153556, Blue, false, false); // <DIV ALIGN="CENTER">Level</DIV>
            AddHtmlLocalized(315, 35, 120, 20, 1153557, Blue, false, false); // <DIV ALIGN="CENTER">Coords</DIV>
            AddHtmlLocalized(435, 35, 120, 20, 1153558, Blue, false, false); // <DIV ALIGN="CENTER">Status</DIV>

            int perPage = 10;
            int totalPages = (int)Math.Ceiling((double)m_List.Count / 10.0);

            if (totalPages < 1) totalPages = 1;

            if(page < 0) page = 0;
            if(page + 1 > totalPages) page = totalPages - 1;
            m_Page = page;

            int start = page * perPage;

            AddHtmlLocalized(40, 428, 200, 20, 1153560, String.Format("{0}\t{1}", m_List.Count, "500"), Blue, false, false); // Maps: ~1_NUM~ of ~2_MAX~
            AddHtmlLocalized(40, 450, 200, 20, 1153561, String.Format("{0}\t{1}", (page + 1).ToString(), (totalPages).ToString()), Blue, false, false); // Page ~1_CUR~ of ~2_MAX~

            AddHtmlLocalized(380, 427, 72, 20, 1153553, Yellow, false, false); // <DIV ALIGN="CENTER">ADD MAPS</DIV>
            AddButton(340, 428, 4011, 4013, 1, GumpButtonType.Reply, 0); 

            AddHtmlLocalized(377, 450, 40, 20, 1153562, Yellow, false, false); // <DIV ALIGN="CENTER">PAGE</DIV>
            AddButton(340, 450, 4014, 4016, 2, GumpButtonType.Reply, 0);
            AddButton(502, 450, 4005, 4007, 3, GumpButtonType.Reply, 0);
            AddImage(320, 455, 5603);
            AddImage(537, 455, 5601);

            int y = 72;
            int index = 0;

            for (int i = start; i >= 0 && i < m_List.Count && index < perPage; i++)
            {
                DaviesLockerEntry entry = m_List[i];

                if (entry == null)
                    continue;

                if(addon.CanUse(from))
                    AddButton(45, y + 3, 1209, 1210, 5 + i, GumpButtonType.Reply, 0);

                AddHtml(80, y, 100, 20, String.Format("<basefont color=yellow>{0}", entry.Map.ToString()), false, false);
                AddHtmlLocalized(205, y, 220, 20, GetLevel(entry), Yellow, false, false);

                if ((entry is TreasureMapEntry && ((TreasureMapEntry)entry).Decoder == null) || (entry is SOSEntry && !((SOSEntry)entry).Opened))
                    AddHtmlLocalized(325, y, 100, 20, 1153569, Yellow, false, false); // Unknown
                else
                    AddHtmlLocalized(325, y, 100, 20, 1060847, String.Format("{0}\t{1}", entry.Location.X.ToString(), entry.Location.Y.ToString()), Yellow, false, false); // ~1_val~ ~2_val~
                
                AddHtmlLocalized(440, y, 100, 20, GetStatus(entry), Yellow, false, false);

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
                case 1: // ADD MAPS
                    {
                        from.Target = new InternalTarget(from, m_Addon, m_Page);
                        from.SendLocalizedMessage(1153563); // Target maps in your backpack or a sub-container to add them to the Locker. When done, press ESC.
                        return;
                    }
                case 2: // PAGE BACK
                    m_Page--;
                    break;
                case 3: // PAGE FORWARD
                    m_Page++;
                    break;
                default:
                    {
                        int index = info.ButtonID - 5;

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
                        m_List.Remove(entry);
                    //TODO: Message?
                }
            }
        }

        private Item Construct(SOSEntry entry)
        {
            if (entry == null)
                return null;

            if (entry.Opened)
            {
                SOS sos = new SOS(entry.Map, entry.Level);

                sos.MessageIndex = entry.MessageIndex;
                sos.TargetLocation = entry.Location;
                return sos;
            }
            else
            {
                return new MessageInABottle(entry.Map, entry.Level);
            }
        }

        private TreasureMap Construct(TreasureMapEntry entry)
        {
            if (entry == null)
                return null;

            TreasureMap map = new TreasureMap();
            map.ChestLocation = new Point2D(entry.Location.X, entry.Location.Y);

            map.Level = entry.Level;
            map.Facet = entry.Map;

            map.Completed = entry.Completed;
            map.CompletedBy = entry.CompletedBy;
            map.Decoder = entry.Decoder;
            map.NextReset = entry.NextReset;

            map.Width = 300;
            map.Height = 300;
            int x = entry.Location.X;
            int y = entry.Location.Y;
            int width, height;
            Map facet = entry.Map;

            map.GetWidthAndHeight(facet, out width, out height);

            int x1 = x - Utility.RandomMinMax(width / 4, (width / 4) * 3);
            int y1 = y - Utility.RandomMinMax(height / 4, (height / 4) * 3);

            if (x1 < 0) x1 = 0;
            if (y1 < 0) y1 = 0;

            int x2, y2;

            map.AdjustMap(facet, out x2, out y2, x1, y1, width, height);

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
                return 1153568;             // S-O-S
            else if(entry is TreasureMapEntry)
                return 1153572 + entry.Level;

            return 1153569; // Unknown
        }

        private int GetStatus(DaviesLockerEntry entry)
        {
            if (entry is SOSEntry)
            {
                SOSEntry sosEntry = (SOSEntry)entry;

                if (!sosEntry.Opened)
                    return 1153570; // Unopened

                if (sosEntry.IsAncient)
                    return 1153572; // Ancient

                return 1153571;     // Opened
            }
            else if (entry is TreasureMapEntry)
            {
                TreasureMapEntry mapEntry = (TreasureMapEntry)entry;

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
            private DaviesLockerAddon m_Addon;
            private int m_Page;

            public InternalTarget(Mobile from, DaviesLockerAddon addon, int page) : base(-1, false, TargetFlags.None)
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