using Server.Engines.VvV;
using Server.Mobiles;
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

        public override void OnExit(Mobile m)
        {
            base.OnExit(m);

            if (!(m is PlayerMobile))
                return;

            if (Map == Map.Felucca && (IsPartOf("Britain") || IsPartOf("Moonglow") || IsPartOf("Britain") || IsPartOf("Jhelom") || IsPartOf("Yew") || IsPartOf("Minoc") || IsPartOf("Trinsic") ||
                IsPartOf("Skara Brae") || IsPartOf("Magincia") || IsPartOf("Vesper")))
            {
                if (m.HasGump(typeof(BattleWarningGump)))
                {
                    m.CloseGump(typeof(BattleWarningGump));
                }
            }
        }
    }
}