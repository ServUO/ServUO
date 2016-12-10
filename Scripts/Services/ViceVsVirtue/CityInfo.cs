using Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.VvV
{
    public class CityInfo
    {
        public static Dictionary<VvVCity, CityInfo> Infos { get; set; }

        public static void Configure()
        {
            Infos = new Dictionary<VvVCity, CityInfo>();

            Infos[VvVCity.Britain] = new CityInfo(
                "Britain",
                new Point3D[] { new Point3D(1453, 1554, 30), new Point3D(1462, 1650, 10), new Point3D(1514, 1640, 20), new Point3D(1490, 1740, 0) },
                new Point3D[] { new Point3D(1592, 1680, 10), new Point3D(1423, 1625, 20), new Point3D(1461, 1734, 0) },
                new Rectangle2D(1441, 1695, 20, 20),
                new Point3D(1450, 1694, 0));

            Infos[VvVCity.Jhelom] = new CityInfo(
                "Jhelom",
                new Point3D[] { new Point3D(1323, 3672, 0), new Point3D(1450, 3712, 8), new Point3D(1448, 3781, 0) },
                new Point3D[] { new Point3D(1336, 3769, 0), new Point3D(1401, 3839, 0), new Point3D(1423, 3803, 0) },
                new Rectangle2D(1311, 3758, 20, 20),
                new Point3D(1329, 3772, 0));

            Infos[VvVCity.Minoc] = new CityInfo(
                "Minoc",
                new Point3D[] { new Point3D(2507, 475, 15), new Point3D(2418, 497, 15), new Point3D(2467, 535, 15), new Point3D(2507, 593, 0) },
                new Point3D[] { new Point3D(2476, 417, 15), new Point3D(2491, 485, 15), new Point3D(2517, 561, 0) },
                new Rectangle2D(2504, 542, 20, 20),
                new Point3D(2501, 560, 0));

            Infos[VvVCity.Moonglow] = new CityInfo(
                "Moonglow",
                new Point3D[] { new Point3D(4479, 1122, 0), new Point3D(4413, 1136, 6), new Point3D(4419, 111, 0), new Point3D(4387, 1107, 0) },
                new Point3D[] { new Point3D(4436, 1089, 0), new Point3D(4421, 1157, 0), new Point3D(4471, 1142, 0) },
                new Rectangle2D(4476, 1168, 8, 8),
                new Point3D(4468, 1176, 0));

            Infos[VvVCity.Ocllo] = new CityInfo(
                "Ocllo",
                new Point3D[] { new Point3D(3672, 2476, 6), new Point3D(3611, 2581, 0), new Point3D(3635, 2655, -2) },
                new Point3D[] { new Point3D(3660, 2484, 0), new Point3D(3650, 2540, 0), new Point3D(3652, 2646, 0) },
                new Rectangle2D(3614, 2593, 10, 10),
                new Point3D(3697, 2522, 0));

            Infos[VvVCity.SkaraBrae] = new CityInfo(
                "Skara Brae",
                new Point3D[] { new Point3D(651, 2163, 6), new Point3D(624, 2142, 0), new Point3D(574, 2152, 0), new Point3D(640, 2247, -2) },
                new Point3D[] { new Point3D(635, 2142, 0), new Point3D(582, 2246, 0), new Point3D(575, 2199, 0) },
                new Rectangle2D(572, 2158, 15, 15),
                new Point3D(601, 2150, 0));

            Infos[VvVCity.Trinsic] = new CityInfo(
                "Trinsic",
                new Point3D[] { new Point3D(1846, 2688, 0), new Point3D(1824, 2685, 0), new Point3D(1845, 2744, 0), new Point3D(1903, 2715, 20) },
                new Point3D[] { new Point3D(1976, 2701, 0), new Point3D(2047, 2853, 0), new Point3D(1832, 2814, 0) },
                new Rectangle2D(1905, 2684, 12, 12),
                new Point3D(1908, 2691, 0));

            Infos[VvVCity.Yew] = new CityInfo(
                "Yew",
                new Point3D[] { new Point3D(453, 963, 0), new Point3D(334, 965, 0), new Point3D(613, 880, 0), new Point3D(640, 938, 6) },
                new Point3D[] { new Point3D(631, 883, 0), new Point3D(650, 976, 0), new Point3D(299, 1010, 0) },
                new Rectangle2D(628, 857, 12, 12),
                new Point3D(638, 858, 0));
        }

        public string Name { get; set; }

        public Point3D[] SigilLocs { get; set; }
        public Point3D[] AltarLocs { get; set; }
        public Point3D TraderLoc { get; set; }
        public Rectangle2D PriestLocation { get; set; }

        public Region Region
        {
            get
            {
                Region r = Region.Regions.FirstOrDefault(reg => reg.Name == Name && reg.Map == Map.Felucca);

                if (r == null)
                    Console.WriteLine("WARNING: Region for {0} not found.", Name);

                return r;
            }
        }

        public CityInfo(string name, Point3D[] sigillocs, Point3D[] altarlocs, Rectangle2D priestlocs, Point3D traderloc)
        {
            Name = name;
            SigilLocs = sigillocs;
            AltarLocs = altarlocs;
            TraderLoc = traderloc;
            PriestLocation = priestlocs;
        }
    }
}