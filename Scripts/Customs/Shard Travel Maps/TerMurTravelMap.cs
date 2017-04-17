/******************* Shard Travel Maps *****************************************************************************************
 *
 *					(C) 2015, by Lokai
 *   
/*******************************************************************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify it under the terms of the GNU General Public License 
 *   as published by the Free Software Foundation; either version 2 of the License, or (at your option) any later version.
 *
 ******************************************************************************************************************************/
using System;
using System.Collections.Generic;
using Server.Network;
using Server.Spells;
using Server.Items;

namespace Server.Gumps
{
    public class TerMurTravelGump : Gump
    {
        private int m_Page;
        private Mobile m_From;
        private TerMurTravelMap m_TravelMap;

        // ..... IF YOU WANT TO CHARGE FOR USING THE MAP: Set CHARGE to true
        private const bool CHARGE = true;
        // ..... and MAKE SURE LENGTH OF payAmounts == LENGTH OF payTypes
        private int[] payAmounts = { 1, 100 };
        private Type[] payTypes = { typeof(BlackPearl), typeof(Gold) };
        // ..... and Describe the payment HERE.
        private const string PAYTHIS = "1 Black Pearl and 100 Gold";

        private string[] MapPages = new string[]
        {
            "", 
            "TerMur Locations"
        };

        private int m_Detail;

        public TerMurTravelGump(Mobile from, int page, int x, int y, TerMurTravelMap travelmap)
            : this(from, page, x, y, travelmap, 0, "")
        {
        }

        public TerMurTravelGump(Mobile from, int page, int x, int y, TerMurTravelMap travelmap, int detail)
            : this(from, page, x, y, travelmap, detail, "")
        {
        }

        public TerMurTravelGump(Mobile from, int page, int x, int y, TerMurTravelMap travelmap, int detail, string message)
            : base(x, y)
        {
            m_Page = page;
            m_From = from;
            m_TravelMap = travelmap;
            m_Detail = detail;

            bool GM = m_From.AccessLevel >= AccessLevel.GameMaster;
            GumpButtonType Reply = GumpButtonType.Reply;

            if (m_TravelMap == null) return;

            if (m_TravelMap.Entries == null || m_TravelMap.Entries.Count <= 0)
            {
                m_TravelMap.Entries = m_TravelMap.GetDefaultEntries();
            }

            if (m_TravelMap.Entries == null || m_TravelMap.Entries.Count <= 0)
            {
                from.SendMessage("No entries were found on this map.");
                m_From.CloseGump(typeof(TerMurTravelGump));
                return;
            }

            AddPage(0);
            AddBackground(0, 0, 600, 460, 3600); // Grey Stone Background
            AddBackground(20, 20, 560, 420, 3000); // Paper Background
            AddBackground(36, 28, 387, 387, 2620); // Black Background behind map

            switch (m_Page)
            {
                case 1: // TerMur Locations
                    AddImage(38, 30, 0x15DE);
                    break;
            }

            AddImage(34, 26, 0x15DF); // Border around the map

            AddLabel(206, 417, 0, MapPages[m_Page]);

            foreach (MapTravelEntry entry in m_TravelMap.Entries)
            {
                bool found = entry.Discovered;
                bool open = entry.Unlocked;
                if (entry.MapIndex == m_Page && (found || GM))
                {
                    AddButton(entry.XposButton, entry.YposButton, 1210, 1209, entry.Index, Reply, 0);
                    AddLabel(entry.XposLabel, entry.YposLabel, open ? 0x480 : found ? 0x40 : 0x20, entry.Name);
                }
            }

            if (m_Detail > 0)
            {
                MapTravelEntry entry = m_TravelMap.GetEntry(m_Detail);
                AddLabel(433, 64, 0, entry.Name);
                AddLabel(433, 84, 0, string.Format("X: {0}", entry.Destination.X));
                AddLabel(433, 104, 0, string.Format("Y: {0}", entry.Destination.Y));
                AddLabel(433, 124, 0, string.Format("Z: {0}", entry.Destination.Z));
                AddLabel(433, 144, 0, string.Format("Map: {0}", entry.Map));
                AddLabel(433, 164, entry.Unlocked ? 0x2A5 : 0x14D,
                    string.Format("Status: {0}", entry.Unlocked ? "Unlocked" : "Locked"));
                if (entry.Unlocked || GM)
                {
                    AddButton(434, 204, 4006, 4007, entry.Index + 10000, Reply, 0); // Teleport Now
                    AddLabel(480, 204, 0x2A5, "Teleport Now");
                }
                if (!entry.Unlocked && (entry.Discovered || GM))
                {
                    if (m_From.Map == entry.Map)
                    {
                        AddButton(434, 244, 4027, 4028, entry.Index + 30000, Reply, 0); // Explore This
                        AddLabel(480, 244, 0x2A5, "Explore This");
                    }
                }
            }
            if (message.Length > 0)
            {
                AddHtml(434, 284, 120, 170, message, false, false);
            }
            AddButton(561, 21, 22153, 22155, 4, Reply, 0); // Help Button
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            m_From.CloseGump(typeof(TerMurTravelGump));
            int id = info.ButtonID;
            Point3D p;
            Map map;

            if (id < 100)
            {
                if (id == 4)
                {
                    m_From.SendGump(new TerMurTravelGump(m_From, m_Page, X, Y, m_TravelMap));
                    m_From.SendGump(new MapTravelHelp(X, Y));
                }
            }
            else if (id < 10000)
            {
                m_From.SendGump(new TerMurTravelGump(m_From, m_Page, X, Y, m_TravelMap, id));
            }
            else if (id < 20000)
            {
                // Here begins the teleport

                string message = "";

                try
                {
                    id -= 10000;
                    bool NonGM = m_From.AccessLevel < AccessLevel.GameMaster;
                    MapTravelEntry entry = m_TravelMap.GetEntry(id);
                    if (entry == null) return;

                    p = entry.Destination;
                    map = entry.Map;

                    if (MapTravelHelp.CanTravel(m_From, map, p) && (entry.Unlocked || !NonGM) && m_TravelMap.Parent == m_From.Backpack)
                    {
                        if (NonGM && CHARGE) // Do not charge GMs - they might complain...
                        {
                            Container pack = m_From.Backpack;
                            if (pack == null || (pack.ConsumeTotal(payTypes, payAmounts) > 0))
                            {
                                if (pack == null) message = "Your pack is null???";
                                else message = string.Format("Using the map to teleport costs {0}.", PAYTHIS);
                                m_From.SendGump(new TerMurTravelGump(m_From, m_Page, X, Y, m_TravelMap, m_Detail, message));
                                return;
                            }
                            // Payment was successful if we reach here ...
                        }
                        m_From.MoveToWorld(p, map);
                        message = string.Format("{0} have been moved to X:{1}, Y:{2}, Z:{3}, Map: {4}",
                            CHARGE ? "You paid " + PAYTHIS + " and" : "You", p.X, p.Y, p.Z, map);
                    }
                }
                catch
                {
                    message = string.Format("Teleport failed.");
                }

                m_From.SendGump(new TerMurTravelGump(m_From, m_Page, X, Y, m_TravelMap, m_Detail, message));
            }
            else if (id >= 30000 && id < 40000)
            {
                id -= 30000;

                MapTravelEntry entry = m_TravelMap.GetEntry(id);
                if (entry == null) return;

                p = entry.Destination;
                map = entry.Map;

                string message = "";
                if (map != m_From.Map)
                {
                    message = "You must be on the same map to Explore there.";
                }
                else
                {
                    if (m_From.InRange(p, 7))
                    {
                        entry.Unlocked = true;
                        entry.Discovered = true; // We do this in case a GM unlocked the location before it was discovered.
                        message = string.Format("You have unlocked a new location: {0}.", entry.Name);
                    }
                    else
                    {
                        int distance = (int)m_From.GetDistanceToSqrt(p);
                        message = string.Format("That location is still approximately {0} paces to the {1}.",
                            distance, MapTravelHelp.GetDirection(m_From, p));
                    }
                }
                m_From.SendGump(new TerMurTravelGump(m_From, m_Page, X, Y, m_TravelMap, m_Detail, message));
            }

        }
    }

    public class TerMurTravelMap : Item
    {
        private List<MapTravelEntry> m_Entries;

        public List<MapTravelEntry> Entries
        {
            get { return m_Entries; }
            set { m_Entries = value; }
        }

        public MapTravelEntry GetEntry(int index)
        {
            foreach (MapTravelEntry entry in m_Entries)
            {
                if (entry.Index == index)
                    return entry;
            }
            return null;
        }

        public int EntryNum { get { return m_Entries.Count; } }

        public int Discovered
        {
            get
            {
                int count = 0;
                foreach (MapTravelEntry entry in m_Entries)
                {
                    if (entry.Discovered) count++;
                }
                return count;
            }
        }

        public int Unlocked
        {
            get
            {
                int count = 0;
                foreach (MapTravelEntry entry in m_Entries)
                {
                    if (entry.Unlocked) count++;
                }
                return count;
            }
        }

        [Constructable]
        public TerMurTravelMap()
            : base(0x14EB)
        {
            Name = "TerMur Travel Map";
            LootType = LootType.Blessed;
            m_Entries = GetDefaultEntries();
        }

        public override string DefaultName
        {
            get { return "TerMur Travel Map"; }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Parent == from.Backpack)
                from.SendGump(new TerMurTravelGump(from, 1, 50, 60, this));
            else
                from.SendMessage("That must be in your pack to use it.");
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            list.Add("Total Entries: {0}.\nEntries Discovered: {1}.\nEntries Unlocked: {2}.", m_Entries.Count, Discovered, Unlocked);
        }

        public TerMurTravelMap(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            if (m_Entries == null || m_Entries.Count <= 0)
                writer.Write(0);
            else
            {
                writer.Write(m_Entries.Count);
                foreach (MapTravelEntry entry in m_Entries)
                {
                    writer.Write(entry.Index);
                    writer.Write(entry.MapIndex);
                    writer.Write(entry.Name);
                    writer.Write(entry.Destination);
                    writer.Write(entry.Map);
                    writer.Write(entry.XposLabel);
                    writer.Write(entry.YposLabel);
                    writer.Write(entry.XposButton);
                    writer.Write(entry.YposButton);
                    writer.Write(entry.Unlocked);
                    writer.Write(entry.Discovered);
                }
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        int count = reader.ReadInt();
                        if (count > 0)
                        {
                            m_Entries = new List<MapTravelEntry>();
                            for (int i = 0; i < count; i++)
                            {
                                try
                                {
                                    m_Entries.Add(new MapTravelEntry(reader.ReadInt(), reader.ReadInt(),
                                        reader.ReadString(), reader.ReadPoint3D(),
                                        reader.ReadMap(), reader.ReadInt(), reader.ReadInt(), reader.ReadInt(),
                                        reader.ReadInt(), reader.ReadBool(), reader.ReadBool()));
                                }
                                catch
                                {
                                }
                            }
                            if (m_Entries.Count != count)
                            {
                                Console.WriteLine("There was an error reading the Map Travel Entries for a Travel Map.");
                            }
                        }
                        else
                        {
                            m_Entries = GetDefaultEntries();
                        }
                        break;
                    }
            }
        }

        public List<MapTravelEntry> GetDefaultEntries()
        {
            List<MapTravelEntry> entries = new List<MapTravelEntry>();
            Map Ter = Map.TerMur;

            // TerMur Locations
            entries.Add(new MapTravelEntry(100, 1, "Royal City", new Point3D(852, 3526, -43), Ter, 198, 233, 260, 240));
            entries.Add(new MapTravelEntry(101, 1, "Holy City", new Point3D(926, 3989, -36), Ter, 332, 358, 310, 361));
            entries.Add(new MapTravelEntry(102, 1, "Fisherman's Reach", new Point3D(612, 3038, 35),
                Ter, 191, 103, 172, 107));
            entries.Add(new MapTravelEntry(103, 1, "Tomb of Kings", new Point3D(997, 3843, -41),
                Ter, 222, 335, 309, 338));
            entries.Add(new MapTravelEntry(104, 1, "Eastern Refuge", new Point3D(1106, 3558, -40), Ter, 312, 247, 352, 266));
            entries.Add(new MapTravelEntry(105, 1, "Toxic Desert", new Point3D(1065, 2995, 74), Ter, 334, 73, 339, 91));

            return entries;
        }
    }

    public class TerMurTravelMapPiece : Item
    {
        private Map m_Map;
        private int m_Index;
        private string m_LocDesc;

        [CommandProperty(AccessLevel.GameMaster)]
        public Map MapSource
        {
            get { return m_Map; }
            set { m_Map = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Index
        {
            get { return m_Index; }
            set { m_Index = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool UnSet
        {
            get { return m_Index == 0; }
            set { m_Index = value ? 0 : -1; }
        }

        [Constructable]
        public TerMurTravelMapPiece()
            : this(Map.TerMur)
        {
        }

        [Constructable]
        public TerMurTravelMapPiece(Map map)
            : this(map, 0, "")
        {
        }

        [Constructable]
        public TerMurTravelMapPiece(Map map, int index, string description)
            : base(0x14ED)
        {
            m_Map = map;
            m_Index = index;
            m_LocDesc = description;
        }

        public override string DefaultName
        {
            get { return string.Format("a travel map fragment"); }
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);
            if (UnSet)
            {
                list.Add("Somewhere in {0}", m_Map);
            }
            else
            {
                list.Add("{0}", m_LocDesc);
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            Container pack = from.Backpack;
            if (pack == null) return;

            if (Parent != pack)
            {
                from.SendMessage("That must be in your pack to use it.");
                return;
            }

            if (from.Map != MapSource)
            {
                from.SendMessage("You must be in {0} to use this map fragment.", MapSource.Name);
                return;
            }

            TerMurTravelMap map = pack.FindItemByType<TerMurTravelMap>();
            if (map == null)
            {
                from.SendMessage("You must have a TerMur Travel Map in your pack to use this map fragment.");
                return;
            }

            if (UnSet)
            {
                List<MapTravelEntry> entries = new List<MapTravelEntry>();
                foreach (MapTravelEntry entry in map.Entries)
                {
                    if (entry.Map == MapSource && !entry.Discovered)
                        entries.Add(entry); // Add Locked entries that match the Map source
                }

                if (entries.Count > 0) // If one or more was found...
                {
                    int token = Utility.Random(entries.Count);
                    m_Index = entries[token].Index; // Pick a random entry and store the Index
                }
                else
                {
                    from.SendMessage("All locations on this map have been discovered.");
                }
            }

            if (m_Index > 0)
            {
                if (map.GetEntry(m_Index).Discovered)
                {
                    from.SendMessage("You have already discovered this location: {0}.", map.GetEntry(m_Index).Name);
                }
                map.GetEntry(m_Index).Discovered = true; // Mark the location as discovered (but not unlocked)
                from.SendMessage("You note the location of the {0}: {1} for future reference.",
                    MapNames[map.GetEntry(m_Index).MapIndex], map.GetEntry(m_Index).Name);
                from.SendMessage("The fragment crumbles to dust in your hands.");
                Delete();
            }
        }

        private string[] MapNames = new string[]
        {
            "", 
            "TerMur Locations"
        };

        public TerMurTravelMapPiece(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            // version 0
            writer.Write(m_Map);
            writer.Write(m_Index);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    m_Map = reader.ReadMap();
                    m_Index = reader.ReadInt();
                    break;
            }
        }
    }
}
