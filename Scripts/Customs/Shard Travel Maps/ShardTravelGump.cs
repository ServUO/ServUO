/**************************** ShardTravelGump.cs *******************************************************************************
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
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Items;

namespace Server.Gumps
{
    public class ShardTravelGump : Gump
    {
        private int m_Page;
        private Mobile m_From;
        private ShardTravelMap m_TravelMap;
		
		// ..... IF YOU WANT TO CHARGE FOR USING THE MAP: Set CHARGE to true
		private const bool CHARGE = false;
		// ..... and MAKE SURE LENGTH OF payAmounts == LENGTH OF payTypes
		private int[] payAmounts = {1, 100};
		private Type[] payTypes = {typeof(BlackPearl), typeof(Gold)};
		// ..... and Describe the payment HERE.
		private const string PAYTHIS = "1 Black Pearl and 100 Gold";

        private string[] MapPages = new string[]
        {
            "", "Trammel Cities", "Trammel Dungeons", "Trammel Moongates", "Trammel Shrines",
            "Felucca Cities", "Felucca Dungeons", "Felucca Moongates", "Felucca Shrines",
            "Ilshenar Cities", "Ilshenar Dungeons", "Ilshenar Shrines",
            "Malas Locations",
            "Tokuno Moongates", "Tokuno Locations",
            "TerMur Locations"
        };

        private int m_Detail;

        public ShardTravelGump(Mobile from, int page, int x, int y, ShardTravelMap travelmap)
            : this(from, page, x, y, travelmap, 0, "")
        {
        }

        public ShardTravelGump(Mobile from, int page, int x, int y, ShardTravelMap travelmap, int detail)
            : this(from, page, x, y, travelmap, detail, "")
        {
        }

        public ShardTravelGump(Mobile from, int page, int x, int y, ShardTravelMap travelmap, int detail, string message)
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
                m_From.CloseGump(typeof (ShardTravelGump));
                return;
            }

            AddPage(0);
            AddBackground(0, 0, 600, 460, 3600); // Grey Stone Background
            AddBackground(20, 20, 560, 420, 3000); // Paper Background
            AddBackground(36, 28, 387, 387, 2620); // Black Background behind map

            switch (m_Page)
            {
                case 1: // Trammel Cities
                case 2: // Trammel Dungeons
                case 3: // Trammel Moongates
                case 4: // Trammel Shrines
                    AddImage(38, 30, 0x15DA);
                    break;
                case 5: // Felucca Cities
                case 6: // Felucca Dungeons
                case 7: // Felucca Moongates
                case 8: // Felucca Shrines
                    AddImage(38, 30, 0x15D9);
                    break;
                case 9: // Ilshenar Cities
                case 10: // Ilshenar Dungeons
                case 11: // Ilshenar Shrines
                    AddImage(38, 30, 0x15DB);
                    break;
                case 12: // Malas Locations
                    AddImage(38, 30, 0x15DC);
                    break;
                case 13: // Tokuno Moongates
                case 14: // Tokuno Locations
                    AddImage(38, 30, 0x15DD);
                    break;
                case 15: // TerMur Locations
                    AddImage(38, 30, 0x15DE);
                    break;
            }

            AddImage(34, 26, 0x15DF); // Border around the map

            AddLabel(206, 417, 0, MapPages[m_Page]);
            if (m_Page > 1)
                AddButton(21, 29, 0x15E3, 0x15E7, 2, Reply, 0); // Previous Page
            if (m_Page < 15)
                AddButton(427, 29, 0x15E1, 0x15E5, 3, Reply, 0); // Next Page

            foreach (ShardTravelEntry entry in m_TravelMap.Entries)
            {
                bool found = entry.Discovered;
                bool open = entry.Unlocked;
                if (entry.MapIndex == m_Page && (found || GM))
                {
                    AddButton(entry.XposButton, entry.YposButton, 1210, 1209, entry.Index, Reply, 0);
                    AddLabel(entry.XposLabel, entry.YposLabel, open ? 0x480: found? 0x40 : 0x20, entry.Name);
                }
            }

            if (m_Detail > 0)
            {
                ShardTravelEntry entry = m_TravelMap.GetEntry(m_Detail);
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
            m_From.CloseGump(typeof (ShardTravelGump));
            int id = info.ButtonID;
            Point3D p;
            Map map;

            if (id < 100)
            {
                if (id == 2)
                    m_From.SendGump(new ShardTravelGump(m_From, m_Page - 1, X, Y, m_TravelMap));
                if (id == 3)
                    m_From.SendGump(new ShardTravelGump(m_From, m_Page + 1, X, Y, m_TravelMap));
                if (id == 4)
				{
                    m_From.SendGump(new ShardTravelGump(m_From, m_Page, X, Y, m_TravelMap));
                    m_From.SendGump(new MapTravelHelp(X, Y));
				}
            }
            else if (id < 10000)
            {
                m_From.SendGump(new ShardTravelGump(m_From, m_Page, X, Y, m_TravelMap, id));
            }
            else if (id < 20000)
            {
                // Here begins the teleport

                string message = "";

                try
                {
                    id -= 10000;
                    bool NonGM = m_From.AccessLevel < AccessLevel.GameMaster;
                    ShardTravelEntry entry = m_TravelMap.GetEntry(id);
                    if (entry == null) return;

                    p = entry.Destination;
                    map = entry.Map;

                    if (NonGM && Factions.Sigil.ExistsOn(m_From))
                    {
                        m_From.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
                    }
                    else if (NonGM && map == Map.Felucca && m_From is PlayerMobile && ((PlayerMobile) m_From).Young)
                    {
                        m_From.SendLocalizedMessage(1049543);
                        // You decide against traveling to Felucca while you are still young.
                    }
                    else if (NonGM && m_From.Kills >= 5 && map != Map.Felucca)
                    {
                        m_From.SendLocalizedMessage(1019004); // You are not allowed to travel there.
                    }
                    else if (NonGM && m_From.Criminal)
                    {
                        m_From.SendLocalizedMessage(1005561, "", 0x22);
                        // Thou'rt a criminal and cannot escape so easily.
                    }
                    else if (NonGM && SpellHelper.CheckCombat(m_From))
                    {
                        m_From.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
                    }
                    else if (NonGM && Misc.WeightOverloading.IsOverloaded(m_From))
                    {
                        m_From.SendLocalizedMessage(502359, "", 0x22); // Thou art too encumbered to move.
                    }
                    else if (!map.CanSpawnMobile(p.X, p.Y, p.Z))
                    {
                        m_From.SendLocalizedMessage(501942); // That location is blocked.
                    }
                    else if (m_From.Holding != null)
                    {
                        m_From.SendLocalizedMessage(1071955); // You cannot teleport while dragging an object.
                    }
                    else if (entry.Unlocked || !NonGM)
                    {
						if (NonGM && CHARGE) // Do not charge GMs - they might complain...
						{
							Container pack = m_From.Backpack;
							if (pack == null || (pack.ConsumeTotal(payTypes, payAmounts) > 0))
							{
								if (pack == null) message = "Your pack is null???";
								else message = string.Format("Using the map to teleport costs {0}.", PAYTHIS);
								m_From.SendGump(new ShardTravelGump(m_From, m_Page, X, Y, m_TravelMap, m_Detail, message));
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

                m_From.SendGump(new ShardTravelGump(m_From, m_Page, X, Y, m_TravelMap, m_Detail, message));
            }
            else if (id >= 30000 && id < 40000)
            {
                id -= 30000;

                ShardTravelEntry entry = m_TravelMap.GetEntry(id);
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
                        int distance = (int) m_From.GetDistanceToSqrt(p);
                        message = string.Format("That location is still approximately {0} paces to the {1}.",
                            distance, MapTravelHelp.GetDirection(m_From, p));
                    }
                }
                m_From.SendGump(new ShardTravelGump(m_From, m_Page, X, Y, m_TravelMap, m_Detail, message));
            }

        }
    }

    public class ShardTravelEntry
    {
        private int m_Index;
        private int m_MapIndex;
        private string m_Name;
        private Point3D m_Destination;
        private Map m_Map;
        private int m_XposLabel;
        private int m_YposLabel;
        private int m_XposButton;
        private int m_YposButton;
        private bool m_Unlocked;
        private bool m_Discovered;

        public int Index
        {
            get { return m_Index; }
        }

        public int MapIndex
        {
            get { return m_MapIndex; }
        }

        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        public Point3D Destination
        {
            get { return m_Destination; }
            set { m_Destination = value; }
        }

        public Map Map
        {
            get { return m_Map; }
            set { m_Map = value; }
        }

        public int XposLabel
        {
            get { return m_XposLabel; }
            set { m_XposLabel = value; }
        }

        public int YposLabel
        {
            get { return m_YposLabel; }
            set { m_YposLabel = value; }
        }

        public int XposButton
        {
            get { return m_XposButton; }
            set { m_XposButton = value; }
        }

        public int YposButton
        {
            get { return m_YposButton; }
            set { m_YposButton = value; }
        }

        public bool Unlocked
        {
            get { return m_Unlocked; }
            set { m_Unlocked = value; }
        }

        public bool Discovered
        {
            get { return m_Discovered; }
            set { m_Discovered = value; }
        }

        public ShardTravelEntry(int index, int mapindex, string name, Point3D p, Map map, int xposlabel, int yposlabel,
            int xposbutton, int yposbutton)
            : this(index, mapindex, name, p, map, xposlabel, yposlabel, xposbutton,
                yposbutton, false, false)
        {
        }

        public ShardTravelEntry(int index, int mapindex, string name, Point3D p, Map map, int xposlabel, int yposlabel,
            int xposbutton,
            int yposbutton, bool unlocked, bool discovered)
        {
            m_Index = index;
            m_MapIndex = mapindex;
            m_Name = name;
            m_Destination = p;
            m_Map = map;
            m_XposLabel = xposlabel;
            m_YposLabel = yposlabel;
            m_XposButton = xposbutton;
            m_YposButton = yposbutton;
            m_Unlocked = unlocked;
            m_Discovered = discovered;
        }
    }

    public class ShardTravelMap : Item
    {
        private List<ShardTravelEntry> m_Entries;

        public List<ShardTravelEntry> Entries
        {
            get { return m_Entries; }
            set { m_Entries = value; }
        }

        public ShardTravelEntry GetEntry(int index)
        {
            foreach (ShardTravelEntry entry in m_Entries)
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
				foreach (ShardTravelEntry entry in m_Entries)
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
				foreach (ShardTravelEntry entry in m_Entries)
				{
					if (entry.Unlocked) count++;
				}
				return count;
			}
		}

        [Constructable]
        public ShardTravelMap() : base(0x14EB)
        {
            Name = "Shard Travel Map";
            LootType = LootType.Blessed;
            m_Entries = GetDefaultEntries();
        }

        public override string DefaultName
        {
            get { return "Shard Travel Map"; }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Parent == from.Backpack)
                from.SendGump(new ShardTravelGump(from, 1, 50, 60, this));
            else
                from.SendMessage("That must be in your pack to use it.");
        }

        public override void AddNameProperties(ObjectPropertyList list)
		{
			base.AddNameProperties(list);

            list.Add("Total Entries: {0}.\nEntries Discovered: {1}.\nEntries Unlocked: {2}.", m_Entries.Count, Discovered, Unlocked);
		}

        public ShardTravelMap(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int) 0); // version

            if (m_Entries == null || m_Entries.Count <= 0)
                writer.Write(0);
            else
            {
                writer.Write(m_Entries.Count);
                foreach (ShardTravelEntry entry in m_Entries)
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
                        m_Entries = new List<ShardTravelEntry>();
                        for (int i = 0; i < count; i++)
                        {
                            try
                            {
                                m_Entries.Add(new ShardTravelEntry(reader.ReadInt(), reader.ReadInt(),
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
                            Console.WriteLine("There was an error reading the Shard Travel Entries for a Travel Map.");
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

        public List<ShardTravelEntry> GetDefaultEntries()
        {
            List<ShardTravelEntry> entries = new List<ShardTravelEntry>();
            Map Tram = Map.Trammel;
            Map Fel = Map.Felucca;
            Map Ilsh = Map.Ilshenar;
            Map Mal = Map.Malas;
            Map Tok = Map.Tokuno;
            Map Ter = Map.TerMur;

            // Trammel Cities			
            entries.Add(new ShardTravelEntry(100, 1, "Britain", new Point3D(1434, 1699, 2), 
                Tram, 167, 172, 147, 176, false, true)); // ************************************************************ This location begins discovered but locked
            entries.Add(new ShardTravelEntry(101, 1, "Bucs Den", new Point3D(2705, 2162, 0), Tram, 219, 239, 234, 228));
            entries.Add(new ShardTravelEntry(102, 1, "Cove", new Point3D(2237, 1214, 0), Tram, 199, 146, 200, 135));
            entries.Add(new ShardTravelEntry(103, 1, "New Haven", new Point3D(3503, 2574, 14),
                Tram, 331, 264, 314, 268, true, true)); // ************************************************************ This location begins unlocked
            entries.Add(new ShardTravelEntry(104, 1, "Jhelom", new Point3D(1417, 3821, 0),
                Tram, 159, 387, 141, 391, false, true)); // ************************************************************ This location begins discovered but locked
            entries.Add(new ShardTravelEntry(105, 1, "New Magincia", new Point3D(3728, 2164, 20),
                Tram, 326, 219, 308, 223));
            entries.Add(new ShardTravelEntry(106, 1, "Minoc", new Point3D(2525, 582, 0),
                Tram, 251, 66, 232, 70, false, true)); // ************************************************************ This location begins discovered but locked
            entries.Add(new ShardTravelEntry(107, 1, "Moonglow", new Point3D(4471, 1177, 0),
                Tram, 345, 110, 364, 130, false, true)); // ************************************************************ This location begins discovered but locked
            entries.Add(new ShardTravelEntry(108, 1, "Nujel'm", new Point3D(3770, 1308, 0), Tram, 293, 151, 306, 139));
            entries.Add(new ShardTravelEntry(109, 1, "Serpents Hold", new Point3D(2895, 3479, 15),
                Tram, 268, 347, 250, 349));
            entries.Add(new ShardTravelEntry(110, 1, "Skara Brae", new Point3D(596, 2138, 0), Tram, 95, 224, 78, 228));
            entries.Add(new ShardTravelEntry(111, 1, "Trinsic", new Point3D(1823, 2821, 0),
                Tram, 199, 276, 182, 280, false, true)); // ************************************************************ This location begins discovered but locked
            entries.Add(new ShardTravelEntry(112, 1, "Vesper", new Point3D(2899, 676, 0),
                Tram, 239, 104, 247, 124, false, true)); // ************************************************************ This location begins discovered but locked
            entries.Add(new ShardTravelEntry(113, 1, "Yew", new Point3D(542, 985, 0),
                Tram, 93, 100, 78, 107, false, true)); // ************************************************************ This location begins discovered but locked

            // Felucca Cities
            entries.Add(new ShardTravelEntry(500, 5, "Britain", new Point3D(1434, 1699, 2),
                Fel, 167, 172, 147, 176, true, true)); // ************************************************************ This location begins unlocked
            entries.Add(new ShardTravelEntry(501, 5, "Bucs Den", new Point3D(2705, 2162, 0), Fel, 219, 239, 234, 228));
            entries.Add(new ShardTravelEntry(502, 5, "Cove", new Point3D(2237, 1214, 0), Fel, 199, 146, 200, 135));
            entries.Add(new ShardTravelEntry(503, 5, "Jhelom", new Point3D(1417, 3821, 0), Fel, 159, 387, 141, 391));
            entries.Add(new ShardTravelEntry(504, 5, "Magincia", new Point3D(3728, 2164, 20), Fel, 326, 219, 308, 223));
            entries.Add(new ShardTravelEntry(505, 5, "Minoc", new Point3D(2525, 582, 0), Fel, 251, 66, 232, 70));
            entries.Add(new ShardTravelEntry(506, 5, "Moonglow", new Point3D(4471, 1177, 0), Fel, 345, 110, 364, 130));
            entries.Add(new ShardTravelEntry(507, 5, "Nujel'm", new Point3D(3770, 1308, 0), Fel, 293, 151, 306, 139));
            entries.Add(new ShardTravelEntry(508, 5, "Ocllo", new Point3D(3620, 2611, 0), Fel, 331, 264, 314, 268));
            entries.Add(new ShardTravelEntry(509, 5, "Serpents Hold", new Point3D(2895, 3479, 15),
                Fel, 268, 347, 250, 349));
            entries.Add(new ShardTravelEntry(510, 5, "Skara Brae", new Point3D(596, 2138, 0), Fel, 95, 224, 78, 228));
            entries.Add(new ShardTravelEntry(511, 5, "Trinsic", new Point3D(1823, 2821, 0), Fel, 199, 276, 182, 280));
            entries.Add(new ShardTravelEntry(512, 5, "Vesper", new Point3D(2899, 676, 0), Fel, 239, 104, 247, 124));
            entries.Add(new ShardTravelEntry(513, 5, "Yew", new Point3D(542, 985, 0), Fel, 93, 100, 78, 107));

            // Trammel Dungeons
            entries.Add(new ShardTravelEntry(200, 2, "Covetous", new Point3D(2498, 921, 0), Tram, 239, 107, 223, 110));
            entries.Add(new ShardTravelEntry(201, 2, "Deceit", new Point3D(4111, 434, 5), Tram, 342, 47, 338, 65));
            entries.Add(new ShardTravelEntry(202, 2, "Despise", new Point3D(1301, 1080, 0), Tram, 85, 114, 131, 121));
            entries.Add(new ShardTravelEntry(203, 2, "Destard", new Point3D(1176, 2640, 2), Tram, 118, 289, 118, 273));
            entries.Add(new ShardTravelEntry(204, 2, "Fire", new Point3D(2923, 3409, 6), Tram, 221, 330, 247, 336));
            entries.Add(new ShardTravelEntry(205, 2, "Hythloth", new Point3D(4721, 3824, 0), Tram, 328, 377, 382, 377));
            entries.Add(new ShardTravelEntry(206, 2, "Ice", new Point3D(1999, 81, 4), Tram, 152, 33, 172, 33));
            entries.Add(new ShardTravelEntry(207, 2, "Sanctuary", new Point3D(759, 1642, 0), Tram, 94, 182, 91, 172));
            entries.Add(new ShardTravelEntry(208, 2, "Shame", new Point3D(511, 1565, 0), Tram, 46, 150, 70, 169));
            entries.Add(new ShardTravelEntry(209, 2, "Wrong", new Point3D(2043, 238, 10), Tram, 198, 53, 182, 55));

            // Felucca Dungeons
            entries.Add(new ShardTravelEntry(600, 6, "Covetous", new Point3D(2498, 921, 0), Fel, 239, 107, 223, 110));
            entries.Add(new ShardTravelEntry(601, 6, "Deceit", new Point3D(4111, 434, 5), Fel, 342, 47, 338, 65));
            entries.Add(new ShardTravelEntry(602, 6, "Despise", new Point3D(1301, 1080, 0), Fel, 85, 114, 131, 121));
            entries.Add(new ShardTravelEntry(603, 6, "Destard", new Point3D(1176, 2640, 2), Fel, 118, 289, 118, 273));
            entries.Add(new ShardTravelEntry(604, 6, "Fire", new Point3D(2923, 3409, 8), Fel, 221, 330, 247, 336));
            entries.Add(new ShardTravelEntry(605, 6, "Hythloth", new Point3D(4721, 3824, 0), Fel, 328, 377, 382, 377));
            entries.Add(new ShardTravelEntry(606, 6, "Ice", new Point3D(1999, 81, 4), Fel, 152, 33, 172, 33));
            entries.Add(new ShardTravelEntry(607, 6, "Sanctuary", new Point3D(759, 1642, 0), Fel, 94, 182, 91, 172));
            entries.Add(new ShardTravelEntry(608, 6, "Shame", new Point3D(511, 1565, 0), Fel, 46, 150, 70, 169));
            entries.Add(new ShardTravelEntry(609, 6, "Wrong", new Point3D(2043, 238, 10), Fel, 198, 53, 182, 55));

            // Trammel Moongates
            entries.Add(new ShardTravelEntry(300, 3, "Britain", new Point3D(1336, 1997, 5), Tram, 156, 197, 135, 206));
            entries.Add(new ShardTravelEntry(301, 3, "New Haven", new Point3D(3450, 2677, 25), Tram, 314, 248, 299, 268));
            entries.Add(new ShardTravelEntry(302, 3, "Jhelom", new Point3D(1499, 3771, 5), Tram, 159, 369, 144, 374));
            entries.Add(new ShardTravelEntry(303, 3, "Magincia", new Point3D(3563, 2139, 31), Tram, 299, 202, 299, 222));
            entries.Add(new ShardTravelEntry(304, 3, "Minoc", new Point3D(2701, 692, 5), Tram, 232, 80, 232, 100));
            entries.Add(new ShardTravelEntry(305, 3, "Moonglow", new Point3D(4467, 1283, 5), Tram, 317, 152, 367, 145));
            entries.Add(new ShardTravelEntry(306, 3, "Skara Brae", new Point3D(643, 2067, 5), Tram, 45, 231, 79, 218));
            entries.Add(new ShardTravelEntry(307, 3, "Trinsic", new Point3D(1828, 2948, -20), Tram, 188, 290, 172, 293));
            entries.Add(new ShardTravelEntry(308, 3, "Yew", new Point3D(771, 752, 5), Tram, 62, 81, 93, 99));

            // Felucca Moongates
            entries.Add(new ShardTravelEntry(700, 7, "Britain", new Point3D(1336, 1997, 5), Fel, 156, 197, 135, 206));
            entries.Add(new ShardTravelEntry(701, 7, "Buccaneer's Den", new Point3D(2711, 2234, 0),
                Fel, 245, 244, 245, 224));
            entries.Add(new ShardTravelEntry(702, 7, "Jhelom", new Point3D(1499, 3771, 5), Fel, 159, 369, 144, 374));
            entries.Add(new ShardTravelEntry(703, 7, "Magincia", new Point3D(3563, 2139, 31), Fel, 299, 202, 299, 222));
            entries.Add(new ShardTravelEntry(704, 7, "Minoc", new Point3D(2701, 692, 5), Fel, 232, 80, 232, 100));
            entries.Add(new ShardTravelEntry(705, 7, "Moonglow", new Point3D(4467, 1283, 5), Fel, 317, 152, 367, 145));
            entries.Add(new ShardTravelEntry(706, 7, "Skara Brae", new Point3D(643, 2067, 5), Fel, 45, 231, 79, 218));
            entries.Add(new ShardTravelEntry(707, 7, "Trinsic", new Point3D(1828, 2948, -20), Fel, 188, 290, 172, 293));
            entries.Add(new ShardTravelEntry(708, 7, "Yew", new Point3D(771, 752, 5), Fel, 62, 81, 93, 99));

            // Trammel Shrines
            entries.Add(new ShardTravelEntry(400, 4, "Compassion", new Point3D(1856, 872, 0), Tram, 146, 126, 179, 109));
            entries.Add(new ShardTravelEntry(401, 4, "Honesty", new Point3D(4217, 564, 36), Tram, 312, 91, 349, 79));
            entries.Add(new ShardTravelEntry(402, 4, "Honor", new Point3D(1730, 3528, 3), Tram, 119, 338, 156, 353));
            entries.Add(new ShardTravelEntry(403, 4, "Humility", new Point3D(4276, 3699, 0), Tram, 296, 356, 346, 365));
            entries.Add(new ShardTravelEntry(404, 4, "Justice", new Point3D(1301, 639, 16), Tram, 87, 52, 130, 72));
            entries.Add(new ShardTravelEntry(405, 4, "Sacrifice", new Point3D(3355, 299, 9), Tram, 224, 50, 277, 52));
            entries.Add(new ShardTravelEntry(406, 4, "Spirituality", new Point3D(1589, 2485, 5),
                Tram, 169, 250, 148, 252));
            entries.Add(new ShardTravelEntry(407, 4, "Valor", new Point3D(2496, 3932, 0), Tram, 236, 379, 221, 392));
            entries.Add(new ShardTravelEntry(408, 4, "Choas", new Point3D(1456, 854, 0), Tram, 105, 116, 141, 106));

            // Felucca Shrines
            entries.Add(new ShardTravelEntry(800, 8, "Compassion", new Point3D(1856, 872, 0), Fel, 146, 126, 179, 109));
            entries.Add(new ShardTravelEntry(801, 8, "Honesty", new Point3D(4217, 564, 36), Fel, 312, 91, 349, 79));
            entries.Add(new ShardTravelEntry(802, 8, "Honor", new Point3D(1730, 3528, 3), Fel, 119, 338, 156, 353));
            entries.Add(new ShardTravelEntry(803, 8, "Humility", new Point3D(4276, 3699, 0), Fel, 296, 356, 346, 365));
            entries.Add(new ShardTravelEntry(804, 8, "Justice", new Point3D(1301, 639, 16), Fel, 87, 52, 130, 72));
            entries.Add(new ShardTravelEntry(805, 8, "Sacrifice", new Point3D(3355, 299, 9), Fel, 224, 50, 277, 52));
            entries.Add(new ShardTravelEntry(806, 8, "Spirituality", new Point3D(1589, 2485, 5), Fel, 169, 250, 148, 252));
            entries.Add(new ShardTravelEntry(807, 8, "Valor", new Point3D(2496, 3932, 0), Fel, 236, 379, 221, 392));
            entries.Add(new ShardTravelEntry(808, 8, "Choas", new Point3D(1456, 854, 0), Fel, 105, 116, 141, 106));

            // Ilshenar Cities
            entries.Add(new ShardTravelEntry(900, 9, "Gargoyle City", new Point3D(763, 640, 0), Ilsh, 160, 150, 160, 170));
            entries.Add(new ShardTravelEntry(901, 9, "Lakeshire", new Point3D(1203, 1124, -25), Ilsh, 235, 285, 235, 305));
            entries.Add(new ShardTravelEntry(902, 9, "Mistas", new Point3D(819, 1130, -29), Ilsh, 160, 280, 160, 300));
            entries.Add(new ShardTravelEntry(903, 9, "Montor", new Point3D(1706, 205, 104), Ilsh, 310, 60, 310, 80));

            //Ilshenar Dungeons
            entries.Add(new ShardTravelEntry(1000, 10, "Ankh", new Point3D(576, 1150, -100), Ilsh, 96, 293, 126, 298));
            entries.Add(new ShardTravelEntry(1001, 10, "Blood", new Point3D(1747, 1171, -2), Ilsh, 314, 325, 323, 315));
            entries.Add(new ShardTravelEntry(1002, 10, "Exodus", new Point3D(854, 778, -80), Ilsh, 126, 203, 169, 208));
            entries.Add(new ShardTravelEntry(1003, 10, "Sorceror's Dungeon", new Point3D(548, 462, -53),
                Ilsh, 71, 113, 118, 130));
            entries.Add(new ShardTravelEntry(1004, 10, "Spectre", new Point3D(1363, 1033, -8), Ilsh, 275, 272, 258, 274));
            entries.Add(new ShardTravelEntry(1005, 10, "Spider Cave", new Point3D(1420, 913, -16),
                Ilsh, 272, 220, 270, 239));
            entries.Add(new ShardTravelEntry(1006, 10, "Wisp", new Point3D(651, 1302, -60), Ilsh, 104, 330, 135, 336));
            entries.Add(new ShardTravelEntry(1007, 10, "Rock", new Point3D(1787, 572, 69), Ilsh, 322, 169, 325, 157));
            entries.Add(new ShardTravelEntry(1008, 10, "Savage Village", new Point3D(1151, 659, -80),
                Ilsh, 200, 180, 235, 198));

            // Ilshenar Shrines
            entries.Add(new ShardTravelEntry(1100, 11, "Compassion", new Point3D(1215, 467, -13),
                Ilsh, 187, 104, 237, 123));
            entries.Add(new ShardTravelEntry(1101, 11, "Honesty", new Point3D(722, 1366, -60),
                Ilsh, 127, 325, 152, 345));
            entries.Add(new ShardTravelEntry(1102, 11, "Honor", new Point3D(744, 724, -28), Ilsh, 122, 179, 156, 195));
            entries.Add(new ShardTravelEntry(1103, 11, "Humility", new Point3D(281, 1016, 0), Ilsh, 65, 273, 77, 261));
            entries.Add(new ShardTravelEntry(1104, 11, "Justice", new Point3D(987, 1011, -32), Ilsh, 182, 234, 193, 252));
            entries.Add(new ShardTravelEntry(1105, 11, "Sacrifice", new Point3D(1174, 1286, -30),
                Ilsh, 205, 312, 224, 330));
            entries.Add(new ShardTravelEntry(1106, 11, "Spirituality", new Point3D(1532, 1340, -3),
                Ilsh, 295, 337, 278, 340));
            entries.Add(new ShardTravelEntry(1107, 11, "Valor", new Point3D(528, 216, -45), Ilsh, 81, 60, 117, 66));
            entries.Add(new ShardTravelEntry(1108, 11, "Choas", new Point3D(1721, 218, 96), Ilsh, 276, 60, 311, 73));

            // Malas Locations
            entries.Add(new ShardTravelEntry(1200, 12, "Luna", new Point3D(1015, 527, -65), Mal, 97, 119, 128, 124));
            entries.Add(new ShardTravelEntry(1201, 12, "Umbra", new Point3D(1997, 1386, -85), Mal, 268, 273, 309, 278));
            entries.Add(new ShardTravelEntry(1202, 12, "Orc Fort", new Point3D(912, 215, -90), Mal, 117, 241, 176, 247));
            entries.Add(new ShardTravelEntry(1203, 12, "Forgotten Pyramid", new Point3D(1853, 1797, -109), Mal, 162, 359, 273, 364));
            entries.Add(new ShardTravelEntry(1204, 12, "Doom", new Point3D(2368, 1267, -85), Mal, 340, 252, 374, 259));
            entries.Add(new ShardTravelEntry(1205, 12, "Labyrinth", new Point3D(1730, 981, -80), Mal, 203, 203, 260, 208)); 
            entries.Add(new ShardTravelEntry(1206, 12, "Grimswind Ruins", new Point3D(2203, 327, -90), Mal, 248, 78, 343, 83)); 

            // Tokuno Moongates
            entries.Add(new ShardTravelEntry(1300, 13, "Isamu-Jima", new Point3D(1169, 998, 41), Tok, 336, 293, 338, 283));
            entries.Add(new ShardTravelEntry(1301, 13, "Makoto-Jima", new Point3D(802, 1204, 25), Tok, 261, 339, 243, 343));
            entries.Add(new ShardTravelEntry(1302, 13, "Homare-Jima", new Point3D(270, 628, 15), Tok, 59, 199, 102, 186));

            // Tokuno Locations
            entries.Add(new ShardTravelEntry(1400, 14, "Crane Marsh", new Point3D(203, 985, 18), Tok, 56, 313, 86, 304));
            entries.Add(new ShardTravelEntry(1401, 14, "Fan Dancer's Dojo", new Point3D(970, 222, 23),
                Tok, 309, 75, 288, 78));
            entries.Add(new ShardTravelEntry(1402, 14, "Makoto Desert", new Point3D(724, 1050, 33), Tok, 163, 272, 200, 265));
            entries.Add(new ShardTravelEntry(1403, 14, "Makoto Zento", new Point3D(741, 1261, 30), Tok, 184, 350, 223, 354));
            entries.Add(new ShardTravelEntry(1404, 14, "Mt. Sho Castle", new Point3D(1234, 772, 3), Tok, 232, 202, 327, 208));
            entries.Add(new ShardTravelEntry(1405, 14, "Yomotsu Mine", new Point3D(257, 786, 63), Tok, 59, 237, 97, 230));
            entries.Add(new ShardTravelEntry(1406, 14, "Bushido Dojo", new Point3D(322, 430, 32), Tok, 80, 111, 117, 130));
            entries.Add(new ShardTravelEntry(1407, 14, "Citadel", new Point3D(1363, 705, 29), Tok, 338, 232, 382, 235));

            // TerMur Locations
            entries.Add(new ShardTravelEntry(1500, 15, "Royal City", new Point3D(852, 3526, -43), Ter, 198, 233, 260, 240));
            entries.Add(new ShardTravelEntry(1501, 15, "Holy City", new Point3D(926, 3989, -36), Ter, 332, 358, 310, 361));
            entries.Add(new ShardTravelEntry(1502, 15, "Fisherman's Reach", new Point3D(612, 3038, 35),
                Ter, 191, 103, 172, 107));
            entries.Add(new ShardTravelEntry(1503, 15, "Tomb of Kings", new Point3D(997, 3843, -41),
                Ter, 222, 335, 309, 338));
            entries.Add(new ShardTravelEntry(1504, 15, "Eastern Refuge", new Point3D(1106, 3558, -40), Ter, 312, 247, 352, 266));
            entries.Add(new ShardTravelEntry(1505, 15, "Toxic Desert", new Point3D(1065, 2995, 74), Ter, 334, 73, 339, 91));

            return entries;
        }
    }

    public class ShardTravelMapPiece : Item
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
        public ShardTravelMapPiece()
            : this(Map.Maps[Utility.Random(6)]) // BE AWARE THIS IS HARD-CODED FOR ALL 6 MAPS
        {
        }

        [Constructable]
        public ShardTravelMapPiece(Map map)
            : this(map, 0, "")
        {
        }

        [Constructable]
        public ShardTravelMapPiece(Map map, int index, string description)
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

            ShardTravelMap map = pack.FindItemByType<ShardTravelMap>();
            if (map == null)
            {
                from.SendMessage("You must have a Shard Travel Map in your pack to use this map fragment.");
                return;
            }

            if (UnSet)
            {
                List<ShardTravelEntry> entries = new List<ShardTravelEntry>();
                foreach (ShardTravelEntry entry in map.Entries)
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
            "Trammel City", "Trammel Dungeon", "Trammel Moongate", "Trammel Shrine", 
            "Felucca City", "Felucca Dungeon", "Felucca Moongate","Felucca Shrine", 
            "Ilshenar City", "Ilshenar Dungeon", "Ilshenar Shrine", 
            "Malas Location", 
            "Tokuno Moongate", "Tokuno Location", 
            "TerMur Location"
        };

        public ShardTravelMapPiece(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int) 0);

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
