using System;
using Server;
using Server.Mobiles;

namespace Server.Regions
{
    public class TokunoDocksRegion : GuardedRegion
    {
        public static TokunoDocksRegion Instance { get { return m_Region; } }
        private static TokunoDocksRegion m_Region;

        private static Rectangle2D[] m_Bounds = new Rectangle2D[]
        {
            new Rectangle2D(650, 1350, 100, 50)
        };

        public TokunoDocksRegion() : base("Tokuno Docks", Map.Tokuno, Region.DefaultPriority, m_Bounds)
        {
            m_Region = this;
        }
    }
}