using System;
using System.Collections.Generic;

namespace Server.Misc
{
    /*
    * This system prevents the inability for server staff to
    * access their server due to data overflows during login.
    *
    * Whenever a staff character's NetState is disposed right after
    * the login process, the character is moved to and logged out
    * at a "safe" alternative.
    *
    * The location the character was moved from will be reported
    * to the player upon the next successful login.
    *
    * This system does not affect non-staff players.
    */
    public static class PreventInaccess
    {
        public static readonly bool Enabled = true;
        private static readonly LocationInfo[] m_Destinations = new LocationInfo[]
        {
            new LocationInfo(new Point3D(5275, 1163, 0), Map.Felucca), // Jail
            new LocationInfo(new Point3D(5275, 1163, 0), Map.Trammel),
            new LocationInfo(new Point3D(5445, 1153, 0), Map.Felucca), // Green acres
            new LocationInfo(new Point3D(5445, 1153, 0), Map.Trammel)
        };
        private static Dictionary<Mobile, LocationInfo> m_MoveHistory;
        public static void Initialize()
        {
            m_MoveHistory = new Dictionary<Mobile, LocationInfo>();

            if (Enabled)
                EventSink.Login += new LoginEventHandler(OnLogin);
        }

        public static void OnLogin(LoginEventArgs e)
        {
            Mobile from = e.Mobile;

            if (from == null || from.IsPlayer())
                return;

            if (HasDisconnected(from))
            {
                if (!m_MoveHistory.ContainsKey(from))
                    m_MoveHistory[from] = new LocationInfo(from.Location, from.Map);

                LocationInfo dest = GetRandomDestination();

                from.Location = dest.Location;
                from.Map = dest.Map;
            }
            else if (m_MoveHistory.ContainsKey(from))
            {
                LocationInfo orig = m_MoveHistory[from];
                from.SendMessage("Your character was moved from {0} ({1}) due to a detected client crash.", orig.Location, orig.Map);

                m_MoveHistory.Remove(from);
            }
        }

        private static bool HasDisconnected(Mobile m)
        {
            return (m.NetState == null || m.NetState.Socket == null);
        }

        private static LocationInfo GetRandomDestination()
        {
            return m_Destinations[Utility.Random(m_Destinations.Length)];
        }

        private class LocationInfo
        {
            private readonly Point3D m_Location;
            private readonly Map m_Map;
            public LocationInfo(Point3D loc, Map map)
            {
                this.m_Location = loc;
                this.m_Map = map;
            }

            public Point3D Location
            {
                get
                {
                    return this.m_Location;
                }
            }
            public Map Map
            {
                get
                {
                    return this.m_Map;
                }
            }
        }
    }
}