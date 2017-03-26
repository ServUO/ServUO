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
using Server.Gumps;
using Server.Spells;

namespace Server
{
    public class MapTravelHelp : Gump
    {
        public MapTravelHelp(int x, int y)
            : base(x, y)
        {
            string message = @"As you explore areas around the world, complete quests, and defeat enemies, you will encounter fragments of the Travel Map.
					
Use these fragments to discover new map destinations which can be bound to the Map once you have explored them.

Shard Owners may want to include information about any costs they add to the use of the map in this help screen.";

            AddBackground(0, 0, 387, 387, 3000); // Paper Background
            AddImage(0, 0, 0x15DF); // Border
            AddHtml(30, 20, 347, 357, Color(message, 0x40), false, true);
        }

        public string Color(string text, int color)
        {
            return String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", color, text);
        }

        public static bool CanTravel(Mobile from, Map map, Point3D p)
        {
            bool NonGM = from.AccessLevel < AccessLevel.GameMaster;

            if (NonGM && Factions.Sigil.ExistsOn(from))
            {
                from.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
                return false;
            }
            if (NonGM && from.Criminal)
            {
                from.SendLocalizedMessage(1005561, "", 0x22);
                // Thou'rt a criminal and cannot escape so easily.
                return false;
            }
            if (NonGM && SpellHelper.CheckCombat(from))
            {
                from.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
                return false;
            }
            if (NonGM && Misc.WeightOverloading.IsOverloaded(from))
            {
                from.SendLocalizedMessage(502359, "", 0x22); // Thou art too encumbered to move.
                return false;
            }
            if (!map.CanSpawnMobile(p.X, p.Y, p.Z))
            {
                from.SendLocalizedMessage(501942); // That location is blocked.
                return false;
            }
            if (from.Holding != null)
            {
                from.SendLocalizedMessage(1071955); // You cannot teleport while dragging an object.
                return false;
            }

            return true;
        }

        public static string GetDirection(Mobile from, Point3D p)
        {
            Direction d = from.GetDirectionTo(p);
            string direction = "... You can't tell in what direction it lies.";
            switch (d)
            {
                case Direction.Down:
                    direction = "South East";
                    break;
                case Direction.East:
                    direction = "East";
                    break;
                case Direction.Left:
                    direction = "South West";
                    break;
                case Direction.North:
                    direction = "North";
                    break;
                case Direction.Right:
                    direction = "North East";
                    break;
                case Direction.South:
                    direction = "South";
                    break;
                case Direction.Up:
                    direction = "North West";
                    break;
                case Direction.West:
                    direction = "West";
                    break;
            }
            return direction;
        }
    }

    public class MapTravelEntry
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

        public MapTravelEntry(int index, int mapindex, string name, Point3D p, Map map, int xposlabel, int yposlabel,
            int xposbutton, int yposbutton)
            : this(index, mapindex, name, p, map, xposlabel, yposlabel, xposbutton,
                yposbutton, false, false)
        {
        }

        public MapTravelEntry(int index, int mapindex, string name, Point3D p, Map map, int xposlabel, int yposlabel,
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
}
