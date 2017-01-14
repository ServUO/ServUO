using System;
using Server;
using Server.Mobiles;

namespace Server.Regions
{
    public class GravewaterLakeRegion : Region
    {
        private static Rectangle2D[] m_Bounds = new Rectangle2D[]
        {
            new Rectangle2D(1440, 1527, 423, 219),
            new Rectangle2D(1381, 1565, 215, 224)
        };

        public static void Initialize()
        {
            Region reg = new GravewaterLakeRegion();
            reg.Register();
        }

        public GravewaterLakeRegion() : base("Gravewater Lake", Map.Malas, Region.DefaultPriority, m_Bounds)
        {
        }
    }
}