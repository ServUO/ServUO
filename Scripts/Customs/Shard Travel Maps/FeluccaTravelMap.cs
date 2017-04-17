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
    public class FeluccaTravelGump : Gump
    {
        private int m_Page;
        private Mobile m_From;
        private FeluccaTravelMap m_TravelMap;

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
            "Felucca Cities", "Felucca Dungeons", "Felucca Moongates", "Felucca Shrines"
        };

        private int m_Detail;

        public FeluccaTravelGump(Mobile from, int page, int x, int y, FeluccaTravelMap travelmap)
            : this(from, page, x, y, travelmap, 0, "")
        {
        }

        public FeluccaTravelGump(Mobile from, int page, int x, int y, FeluccaTravelMap travelmap, int detail)
            : this(from, page, x, y, travelmap, detail, "")
        {
        }

        public FeluccaTravelGump(Mobile from, int page, int x, int y, FeluccaTravelMap travelmap, int detail, string message)
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
                m_From.CloseGump(typeof(FeluccaTravelGump));
                return;
            }

            AddPage(0);
            AddBackground(0, 0, 600, 460, 3600); // Grey Stone Background
            AddBackground(20, 20, 560, 420, 3000); // Paper Background
            AddBackground(36, 28, 387, 387, 2620); // Black Background behind map

            switch (m_Page)
            {
                case 1: // Felucca Cities
                case 2: // Felucca Dungeons
                case 3: // Felucca Moongates
                case 4: // Felucca Shrines
                    AddImage(38, 30, 0x15D9);
                    break;
            }

            AddImage(34, 26, 0x15DF); // Border around the map

            AddLabel(206, 417, 0, MapPages[m_Page]);
            if (m_Page > 1)
                AddButton(21, 29, 0x15E3, 0x15E7, 2, Reply, 0); // Previous Page
            if (m_Page < 4)
                AddButton(427, 29, 0x15E1, 0x15E5, 3, Reply, 0); // Next Page

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
            m_From.CloseGump(typeof(FeluccaTravelGump));
            int id = info.ButtonID;
            Point3D p;
            Map map;

            if (id < 100)
            {
                if (id == 2)
                    m_From.SendGump(new FeluccaTravelGump(m_From, m_Page - 1, X, Y, m_TravelMap));
                if (id == 3)
                    m_From.SendGump(new FeluccaTravelGump(m_From, m_Page + 1, X, Y, m_TravelMap));
                if (id == 4)
                {
                    m_From.SendGump(new FeluccaTravelGump(m_From, m_Page, X, Y, m_TravelMap));
                    m_From.SendGump(new MapTravelHelp(X, Y));
                }
            }
            else if (id < 10000)
            {
                m_From.SendGump(new FeluccaTravelGump(m_From, m_Page, X, Y, m_TravelMap, id));
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
                                m_From.SendGump(new FeluccaTravelGump(m_From, m_Page, X, Y, m_TravelMap, m_Detail,
                                    message));
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

                m_From.SendGump(new FeluccaTravelGump(m_From, m_Page, X, Y, m_TravelMap, m_Detail, message));
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
                m_From.SendGump(new FeluccaTravelGump(m_From, m_Page, X, Y, m_TravelMap, m_Detail, message));
            }

        }
    }

    public class FeluccaTravelMap : Item
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
        public FeluccaTravelMap()
            : base(0x14EB)
        {
            Name = "Felucca Travel Map";
            LootType = LootType.Blessed;
            m_Entries = GetDefaultEntries();
        }

        public override string DefaultName
        {
            get { return "Felucca Travel Map"; }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Parent == from.Backpack)
                from.SendGump(new FeluccaTravelGump(from, 1, 50, 60, this));
            else
                from.SendMessage("That must be in your pack to use it.");
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            list.Add("Total Entries: {0}.\nEntries Discovered: {1}.\nEntries Unlocked: {2}.", m_Entries.Count, Discovered, Unlocked);
        }

        public FeluccaTravelMap(Serial serial)
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
            Map Fel = Map.Felucca;

            // Felucca Cities
            entries.Add(new MapTravelEntry(100, 1, "Britain", new Point3D(1434, 1699, 2),
                Fel, 167, 172, 147, 176, true, true)); // ************************************************************ This location begins unlocked
            entries.Add(new MapTravelEntry(101, 1, "Bucs Den", new Point3D(2705, 2162, 0), Fel, 219, 239, 234, 228));
            entries.Add(new MapTravelEntry(102, 1, "Cove", new Point3D(2237, 1214, 0), Fel, 199, 146, 200, 135));
            entries.Add(new MapTravelEntry(103, 1, "Jhelom", new Point3D(1417, 3821, 0), Fel, 159, 387, 141, 391));
            entries.Add(new MapTravelEntry(104, 1, "Magincia", new Point3D(3728, 2164, 20), Fel, 326, 219, 308, 223));
            entries.Add(new MapTravelEntry(105, 1, "Minoc", new Point3D(2525, 582, 0), Fel, 251, 66, 232, 70));
            entries.Add(new MapTravelEntry(106, 1, "Moonglow", new Point3D(4471, 1177, 0), Fel, 345, 110, 364, 130));
            entries.Add(new MapTravelEntry(107, 1, "Nujel'm", new Point3D(3770, 1308, 0), Fel, 293, 151, 306, 139));
            entries.Add(new MapTravelEntry(108, 1, "Ocllo", new Point3D(3620, 2611, 0), Fel, 331, 264, 314, 268));
            entries.Add(new MapTravelEntry(109, 1, "Serpents Hold", new Point3D(2895, 3479, 15),
                Fel, 268, 347, 250, 349));
            entries.Add(new MapTravelEntry(110, 1, "Skara Brae", new Point3D(596, 2138, 0), Fel, 95, 224, 78, 228));
            entries.Add(new MapTravelEntry(111, 1, "Trinsic", new Point3D(1823, 2821, 0), Fel, 199, 276, 182, 280));
            entries.Add(new MapTravelEntry(112, 1, "Vesper", new Point3D(2899, 676, 0), Fel, 239, 104, 247, 124));
            entries.Add(new MapTravelEntry(113, 1, "Yew", new Point3D(542, 985, 0), Fel, 93, 100, 78, 107));

            // Felucca Dungeons
            entries.Add(new MapTravelEntry(200, 2, "Covetous", new Point3D(2498, 921, 0), Fel, 239, 107, 223, 110));
            entries.Add(new MapTravelEntry(201, 2, "Deceit", new Point3D(4111, 434, 5), Fel, 342, 47, 338, 65));
            entries.Add(new MapTravelEntry(202, 2, "Despise", new Point3D(1301, 1080, 0), Fel, 85, 114, 131, 121));
            entries.Add(new MapTravelEntry(203, 2, "Destard", new Point3D(1176, 2640, 2), Fel, 118, 289, 118, 273));
            entries.Add(new MapTravelEntry(204, 2, "Fire", new Point3D(2923, 3409, 8), Fel, 221, 330, 247, 336));
            entries.Add(new MapTravelEntry(205, 2, "Hythloth", new Point3D(4721, 3824, 0), Fel, 328, 377, 382, 377));
            entries.Add(new MapTravelEntry(206, 2, "Ice", new Point3D(1999, 81, 4), Fel, 152, 33, 172, 33));
            entries.Add(new MapTravelEntry(207, 2, "Sanctuary", new Point3D(759, 1642, 0), Fel, 94, 182, 91, 172));
            entries.Add(new MapTravelEntry(208, 2, "Shame", new Point3D(511, 1565, 0), Fel, 46, 150, 70, 169));
            entries.Add(new MapTravelEntry(209, 2, "Wrong", new Point3D(2043, 238, 10), Fel, 198, 53, 182, 55));

            // Felucca Moongates
            entries.Add(new MapTravelEntry(300, 3, "Britain", new Point3D(1336, 1997, 5), Fel, 156, 197, 135, 206));
            entries.Add(new MapTravelEntry(301, 3, "Buccaneer's Den", new Point3D(2711, 2234, 0),
                Fel, 245, 244, 245, 224));
            entries.Add(new MapTravelEntry(302, 3, "Jhelom", new Point3D(1499, 3771, 5), Fel, 159, 369, 144, 374));
            entries.Add(new MapTravelEntry(303, 3, "Magincia", new Point3D(3563, 2139, 31), Fel, 299, 202, 299, 222));
            entries.Add(new MapTravelEntry(304, 3, "Minoc", new Point3D(2701, 692, 5), Fel, 232, 80, 232, 100));
            entries.Add(new MapTravelEntry(305, 3, "Moonglow", new Point3D(4467, 1283, 5), Fel, 317, 152, 367, 145));
            entries.Add(new MapTravelEntry(306, 3, "Skara Brae", new Point3D(643, 2067, 5), Fel, 45, 231, 79, 218));
            entries.Add(new MapTravelEntry(307, 3, "Trinsic", new Point3D(1828, 2948, -20), Fel, 188, 290, 172, 293));
            entries.Add(new MapTravelEntry(308, 3, "Yew", new Point3D(771, 752, 5), Fel, 62, 81, 93, 99));

            // Felucca Shrines
            entries.Add(new MapTravelEntry(400, 4, "Compassion", new Point3D(1856, 872, 0), Fel, 146, 126, 179, 109));
            entries.Add(new MapTravelEntry(401, 4, "Honesty", new Point3D(4217, 564, 36), Fel, 312, 91, 349, 79));
            entries.Add(new MapTravelEntry(402, 4, "Honor", new Point3D(1730, 3528, 3), Fel, 119, 338, 156, 353));
            entries.Add(new MapTravelEntry(403, 4, "Humility", new Point3D(4276, 3699, 0), Fel, 296, 356, 346, 365));
            entries.Add(new MapTravelEntry(404, 4, "Justice", new Point3D(1301, 639, 16), Fel, 87, 52, 130, 72));
            entries.Add(new MapTravelEntry(405, 4, "Sacrifice", new Point3D(3355, 299, 9), Fel, 224, 50, 277, 52));
            entries.Add(new MapTravelEntry(406, 4, "Spirituality", new Point3D(1589, 2485, 5), Fel, 169, 250, 148, 252));
            entries.Add(new MapTravelEntry(407, 4, "Valor", new Point3D(2496, 3932, 0), Fel, 236, 379, 221, 392));
            entries.Add(new MapTravelEntry(408, 4, "Choas", new Point3D(1456, 854, 0), Fel, 105, 116, 141, 106));

            return entries;
        }
    }

    public class FeluccaTravelMapPiece : Item
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
        public FeluccaTravelMapPiece()
            : this(Map.Felucca)
        {
        }

        [Constructable]
        public FeluccaTravelMapPiece(Map map)
            : this(map, 0, "")
        {
        }

        [Constructable]
        public FeluccaTravelMapPiece(Map map, int index, string description)
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

            FeluccaTravelMap map = pack.FindItemByType<FeluccaTravelMap>();
            if (map == null)
            {
                from.SendMessage("You must have a Felucca Travel Map in your pack to use this map fragment.");
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
            "Felucca City", "Felucca Dungeon", "Felucca Moongate","Felucca Shrine"
        };

        public FeluccaTravelMapPiece(Serial serial)
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
