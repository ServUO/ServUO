/* Copyright (C) 2013 Ian Karlinsey
 * 
 * UltimeLive is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * UltimaLive is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with UltimaLive.  If not, see <http://www.gnu.org/licenses/>. 
 */

using Server;
using Server.Network;
using System.Collections.Generic;

namespace UltimaLive
{
    public sealed class MapRegistry
    {
        public struct MapDefinition
        {
            public int FileIndex;
            public Point2D Dimensions;
            public Point2D WrapAroundDimensions;
            public MapDefinition(int index, Point2D dimension, Point2D wraparound)
            {
                FileIndex = index;
                Dimensions = dimension;
                WrapAroundDimensions = wraparound;
            }
        }
        public static readonly Dictionary<int, MapDefinition> Definitions = new Dictionary<int, MapDefinition>();
        public static readonly Dictionary<int, List<int>> MapAssociations = new Dictionary<int, List<int>>();

        public static void AddMapDefinition(int index, int associated, Point2D dimensions, Point2D wrapDimensions)
        {
            if (!Definitions.ContainsKey(index))
            {
                Definitions.Add(index, new MapDefinition(associated, dimensions, wrapDimensions));
                if (MapAssociations.ContainsKey(associated))
                {
                    MapAssociations[associated].Add(index);
                }
                else
                {
                    MapAssociations[associated] = new List<int>() { index };
                }
                //m_Definitions.Add(index, dimensions, wrapDimensions); //Felucca
            }
        }

        public static void Configure()
        {
            AddMapDefinition(0, 0, new Point2D(7168, 4096), new Point2D(5120, 4096));
            AddMapDefinition(1, 1, new Point2D(7168, 4096), new Point2D(5120, 4096));
            AddMapDefinition(2, 2, new Point2D(2304, 1600), new Point2D(2304, 1600)); //Ilshenar
            AddMapDefinition(3, 3, new Point2D(2560, 2048), new Point2D(2560, 2048)); //Malas
            AddMapDefinition(4, 4, new Point2D(1448, 1448), new Point2D(1448, 1448)); //Tokuno
            AddMapDefinition(5, 5, new Point2D(1280, 4096), new Point2D(1280, 4096)); //TerMur
            //those are sample maps that use same original map...
            /*AddMapDefinition(32, 0, new Point2D(7168, 4096), new Point2D(5120, 4096));
            AddMapDefinition(33, 0, new Point2D(7168, 4096), new Point2D(5120, 4096));
            AddMapDefinition(34, 1, new Point2D(7168, 4096), new Point2D(5120, 4096));*/
            EventSink.ServerList += new ServerListEventHandler(EventSink_OnServerList);
            EventSink.MapChange += new MapChangeEventHandler(EventSink_MapChange);
            EventSink.Login += new LoginEventHandler(EventSink_Login);
        }

        private static void EventSink_OnServerList(ServerListEventArgs args)
        {
            args.State.Send(new LiveLoginComplete());
            args.State.Send(new MapDefinitions());
        }

        private static void EventSink_MapChange(MapChangeEventArgs args)
        {
            Mobile m = args.Mob;
            if (m != null && m.Map != null)//crash protection
            {
                m.Send(new QueryClientHash(m));
            }
        }

        private static void EventSink_Login(LoginEventArgs args)
        {
            Mobile m = args.Mobile;
            if (m != null && m.Map != null)//crash protection
            {
                m.Send(new QueryClientHash(m));
            }
        }
    }
}
