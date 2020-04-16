using Server.Spells.Chivalry;
using Server.Spells.Fourth;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using System.Xml;

namespace Server.Regions
{
    public class GreenAcres : BaseRegion
    {
        public GreenAcres(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
        }

        public override bool AllowHousing(Mobile from, Point3D p)
        {
            if (from.IsPlayer())
                return false;

            return base.AllowHousing(from, p);
        }

        public override bool OnBeginSpellCast(Mobile m, ISpell s)
        {
            if ((s is GateTravelSpell || s is RecallSpell || s is MarkSpell || s is SacredJourneySpell) && m.IsPlayer())
            {
                m.SendMessage("You cannot cast that spell here.");
                return false;
            }

            return base.OnBeginSpellCast(m, s);
        }
    }
}