using Server.Commands;
using Server.Regions;

namespace Server
{
    public class WorldLocationInfo
    {
        public static void Initialize()
        {
            CommandSystem.Register("GetLocation", AccessLevel.Administrator, GetLocation_OnCommand);
        }

        private readonly string m_RegionName;
        private readonly Rectangle2D[] m_Bounds;

        public string RegionName => m_RegionName;
        public Rectangle2D[] Bounds => m_Bounds;

        public WorldLocationInfo(string regionName, params Rectangle2D[] bounds)
        {
            m_RegionName = regionName;
            m_Bounds = bounds;
        }

        public static WorldLocationInfo[][] Locations => m_Locations;
        private static readonly WorldLocationInfo[][] m_Locations = new[]
        {
            new[] // Felucca
			{
                new WorldLocationInfo("an island southeast of Britain", new Rectangle2D(1620, 1900, 594, 150),
                                                                        new Rectangle2D(1810, 2040, 200, 120),
                                                                        new Rectangle2D(1980, 2040, 250, 380)),

                new WorldLocationInfo("near Buccaneer's Den", new Rectangle2D(2560, 1900, 400, 500)),
                new WorldLocationInfo("a jungle near Serpent's Hold", new Rectangle2D(2750, 3320, 340, 340)),
                new WorldLocationInfo("a forest near Ocllo", new Rectangle2D(3330, 2350, 500, 600)),
                new WorldLocationInfo("a forest near Magincia", new Rectangle2D(3510, 2000, 310, 320)),
                new WorldLocationInfo("a desert near Nujel'm", new Rectangle2D(3480, 1020, 350, 410)),

                new WorldLocationInfo("the arctic island north of Moonglow", new Rectangle2D(3850, 160, 500, 600)),
                new WorldLocationInfo("a forest near Moonglow", new Rectangle2D(4250, 780, 500, 800)),
                new WorldLocationInfo("the isle of fire", new Rectangle2D(4080, 3060, 800, 940)),

                new WorldLocationInfo("an island north of Jhelom", new Rectangle2D(1030, 3330, 200, 400)),
                new WorldLocationInfo("the island of Jhelom", new Rectangle2D(1230, 3600, 300, 330)),
                new WorldLocationInfo("the island south of Jhelom", new Rectangle2D(1360, 3940, 200, 200)),

                new WorldLocationInfo("the island of Jhelom", new Rectangle2D(1230, 3600, 300, 330)),
                new WorldLocationInfo("somewhere on Dragon Island", new Rectangle2D(1020, 3080, 100, 150)),

                new WorldLocationInfo("somewhere on Amoeba Island", new Rectangle2D(2100, 3870, 120, 150)),
                new WorldLocationInfo("somewhere on Valor Island", new Rectangle2D(2380, 3870, 200, 200)),

                new WorldLocationInfo("near bog of desolation", new Rectangle2D(1950, 920, 160, 195)),
                new WorldLocationInfo("near compassion desert", new Rectangle2D(1800, 800, 242, 180)),

                new WorldLocationInfo("a forest northeast of Yew", new Rectangle2D(636, 0, 914, 744)),
                new WorldLocationInfo("a froest east of Yew", new Rectangle2D(506, 743, 756, 535)),

                new WorldLocationInfo("a forest west of Britain", new Rectangle2D(106, 1280, 1252, 750)),
                new WorldLocationInfo("a forest north of Britain", new Rectangle2D(1260, 0, 860, 1628)),
                new WorldLocationInfo("a forest south of Britain", new Rectangle2D(1078, 1660, 582, 370)),

                new WorldLocationInfo("a forest east of cove", new Rectangle2D(2210, 1030, 600, 240)),
                new WorldLocationInfo("a forest north of Minoc", new Rectangle2D(2300, 0, 500, 370)),
                new WorldLocationInfo("a forest east of Minoc", new Rectangle2D(2300, 370, 500, 230)),
                new WorldLocationInfo("a forest near Minoc and Vesper", new Rectangle2D(2230, 520, 620, 510)),

                new WorldLocationInfo("a forest north of Vesper", new Rectangle2D(2800, 0, 700, 832)),

                new WorldLocationInfo("a forest outside of Skara Brae", new Rectangle2D(700, 2030, 560, 490)),

                new WorldLocationInfo("a swamp west of Trinsic", new Rectangle2D(1070, 2720, 210, 350)),
                new WorldLocationInfo("a forest north of Trinsic", new Rectangle2D(1260, 2030, 540, 600)),
                new WorldLocationInfo("a jungle north of Trinsic", new Rectangle2D(1800, 2160, 400, 406)),
                new WorldLocationInfo("a forest west of Trinsic", new Rectangle2D(770, 2630, 1089, 100)),
                new WorldLocationInfo("a jungle west of Trinsic", new Rectangle2D(1070, 2720, 1167, 356)),
                new WorldLocationInfo("a forest south of Trinsic", new Rectangle2D(1800, 2720, 188, 290)),
                new WorldLocationInfo("a jungle south of Trinsic", new Rectangle2D(1070, 2820, 1174, 846)),

                new WorldLocationInfo("an unknown island in the Lost Lands", new Rectangle2D(5840, 2440, 45, 60)),

                new WorldLocationInfo("a frozen tundra of the Lost Lands", new Rectangle2D(5120, 2300, 500, 130),
                                                                           new Rectangle2D(5700, 2300, 440, 260)),

                new WorldLocationInfo("somewhere in the Lost Lands", new Rectangle2D(5120, 2300, 1020, 1800))
            },
            new[] // Trammel
			{
                new WorldLocationInfo("an island southeast of Britain", new Rectangle2D(1620, 1900, 594, 150),
                                                                        new Rectangle2D(1810, 2040, 200, 120),
                                                                        new Rectangle2D(1980, 2040, 250, 380)),

                new WorldLocationInfo("near Buccaneer's Den", new Rectangle2D(2560, 1900, 400, 500)),
                new WorldLocationInfo("a jungle near Serpent's Hold", new Rectangle2D(2750, 3320, 340, 340)),

                new WorldLocationInfo("a forest near New Haven", new Rectangle2D(3330, 2350, 500, 600)),

                new WorldLocationInfo("a forest near Magincia", new Rectangle2D(3510, 2000, 310, 320)),
                new WorldLocationInfo("a desert near Nujel'm", new Rectangle2D(3480, 1020, 350, 410)),

                new WorldLocationInfo("the arctic island north of Moonglow", new Rectangle2D(3850, 160, 500, 600)),
                new WorldLocationInfo("a forest near Moonglow", new Rectangle2D(4250, 780, 500, 800)),
                new WorldLocationInfo("the isle of fire", new Rectangle2D(4080, 3060, 800, 940)),

                new WorldLocationInfo("an island north of Jhelom", new Rectangle2D(1030, 3330, 200, 400)),
                new WorldLocationInfo("the island of Jhelom", new Rectangle2D(1230, 3600, 300, 330)),
                new WorldLocationInfo("the island south of Jhelom", new Rectangle2D(1360, 3940, 200, 200)),

                new WorldLocationInfo("the island of Jhelom", new Rectangle2D(1230, 3600, 300, 330)),
                new WorldLocationInfo("somewhere on Dragon Island", new Rectangle2D(1020, 3080, 100, 150)),

                new WorldLocationInfo("somewhere on Amoeba Island", new Rectangle2D(2100, 3870, 120, 150)),
                new WorldLocationInfo("somewhere on Valor Island", new Rectangle2D(2380, 3870, 200, 200)),

                new WorldLocationInfo("near bog of desolation", new Rectangle2D(1950, 920, 160, 195)),
                new WorldLocationInfo("near compassion desert", new Rectangle2D(1800, 800, 242, 180)),

                new WorldLocationInfo("a forest northeast of Yew", new Rectangle2D(636, 0, 914, 744)),
                new WorldLocationInfo("a froest east of Yew", new Rectangle2D(506, 743, 756, 535)),

                new WorldLocationInfo("a forest west of Britain", new Rectangle2D(106, 1280, 1252, 750)),
                new WorldLocationInfo("a forest north of Britain", new Rectangle2D(1260, 0, 860, 1628)),
                new WorldLocationInfo("a forest south of Britain", new Rectangle2D(1078, 1660, 582, 370)),

                new WorldLocationInfo("a forest east of cove", new Rectangle2D(2210, 1030, 600, 240)),
                new WorldLocationInfo("a forest north of Minoc", new Rectangle2D(2300, 0, 500, 370)),
                new WorldLocationInfo("a forest east of Minoc", new Rectangle2D(2300, 370, 500, 230)),
                new WorldLocationInfo("a forest near Minoc and Vesper", new Rectangle2D(2230, 520, 620, 510)),

                new WorldLocationInfo("a forest north of Vesper", new Rectangle2D(2800, 0, 700, 832)),

                new WorldLocationInfo("a forest outside of Skara Brae", new Rectangle2D(700, 2030, 560, 490)),

                new WorldLocationInfo("a swamp west of Trinsic", new Rectangle2D(1070, 2720, 210, 350)),
                new WorldLocationInfo("a forest north of Trinsic", new Rectangle2D(1260, 2030, 540, 600)),
                new WorldLocationInfo("a jungle north of Trinsic", new Rectangle2D(1800, 2160, 400, 406)),
                new WorldLocationInfo("a forest west of Trinsic", new Rectangle2D(770, 2630, 1089, 100)),
                new WorldLocationInfo("a jungle west of Trinsic", new Rectangle2D(1070, 2720, 1167, 356)),
                new WorldLocationInfo("a forest south of Trinsic", new Rectangle2D(1800, 2720, 188, 290)),
                new WorldLocationInfo("a jungle south of Trinsic", new Rectangle2D(1070, 2820, 1174, 846)),

                new WorldLocationInfo("an unknown island in the Lost Lands", new Rectangle2D(5840, 2440, 45, 60)),

                new WorldLocationInfo("a frozen tundra of the Lost Lands", new Rectangle2D(5120, 2300, 500, 130),
                                                                           new Rectangle2D(5700, 2300, 440, 260)),

                new WorldLocationInfo("somewhere in the Lost Lands", new Rectangle2D(5120, 2300, 1020, 1800))
            },
            new[] // Ilshenar
			{
                new WorldLocationInfo("somewhere in Ilshenar", new Rectangle2D(0, 0, 2300, 1598))
            },
            new[] // Malas
			{
                new WorldLocationInfo("somewhere in Malas", new Rectangle2D(0, 0, 2558, 2046))
            },
            new[] // Tokuno
			{
                new WorldLocationInfo("somewhere in Tokuno", new Rectangle2D(0, 0, 1446, 1446))
            },
            new[] // TerMur
			{
                new WorldLocationInfo("somewhere in TerMur", new Rectangle2D(270, 2754, 1000, 1339))
            },
        };

        public static string GetLocationString(IEntity e)
        {
            return GetLocationString(e.Location, e.Map);
        }

        public static string GetLocationString(Point3D p, Map map)
        {
            if (map == null)
                return "an unknown location";

            Region r = Region.Find(p, map);
		
            if (r != null && r.Name != null)
	    {
		if (r is TownRegion)
                    return string.Format("somewhere near {0}.", r.Name);
    
                if (r is DungeonRegion)
                    return string.Format("somewhere in dungeon {0}.", r.Name);
	    }
		
            int mapID = map.MapID;
		
            if (mapID < 0 || mapID >= m_Locations.Length)
                return "an unknown location";

            WorldLocationInfo[] infos = m_Locations[mapID];

            foreach (WorldLocationInfo info in infos)
            {
                foreach (Rectangle2D rec in info.m_Bounds)
                {
                    if (rec.Contains(p))
                        return info.m_RegionName;
                }
            }

            return "an unknown location";
        }

        public static void GetLocation_OnCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage(GetLocationString(e.Mobile.Location, e.Mobile.Map));
        }
    }
}
