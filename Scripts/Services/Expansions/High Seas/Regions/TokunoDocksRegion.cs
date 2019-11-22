using System;
using System.Xml;

using Server;
using Server.Mobiles;

namespace Server.Regions
{
    public class TokunoDocksRegion : GuardedRegion
    {
        public static TokunoDocksRegion Instance { get { return m_Region; } }
        private static TokunoDocksRegion m_Region;

        public TokunoDocksRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
            m_Region = this;
        }

        public override bool AllowHousing(Mobile from, Point3D p)
        {
            return false;
        }
    }
}
