using System;
using System.Xml;

namespace Server.Regions
{
    public class TownRegion : GuardedRegion
    {
        public TownRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
        }
    }
}