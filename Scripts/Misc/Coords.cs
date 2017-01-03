using System;
using Server.Items;

namespace Server
{
    public static class Coords
    {
        public static string ToCoordinates(Point3D location, Map map)
        {
            int xLong = 0, yLat = 0, xMins = 0, yMins = 0;
            bool xEast = false, ySouth = false;

            bool valid = Sextant.Format(location, map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth);

            return valid ? String.Format("{0}° {1}'{2}, {3}° {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W") : "unknown";
        }
    }
}