using System.Xml;

namespace Server.Regions
{
    public class TokunoDocksRegion : GuardedRegion
    {
        public static TokunoDocksRegion Instance => m_Region;
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
