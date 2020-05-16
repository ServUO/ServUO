using System.Xml;

namespace Server.Regions
{
    public class TombOfKingsRegion : BaseRegion
    {
        public TombOfKingsRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
        }

        public override void AlterLightLevel(Mobile m, ref int global, ref int personal)
        {
            global = 0;
        }
    }
}