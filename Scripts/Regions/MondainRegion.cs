using Server.Spells.Chivalry;
using Server.Spells.Fourth;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using System.Xml;

namespace Server.Regions
{
    public class MondainRegion : DungeonRegion
    {
        public MondainRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
        }

        public override bool OnBeginSpellCast(Mobile m, ISpell s)
        {
            if (m.IsPlayer())
            {
                if (s is MarkSpell)
                {
                    m.SendLocalizedMessage(501802); // Thy spell doth not appear to work...
                    return false;
                }
                else if (s is GateTravelSpell || s is RecallSpell || s is SacredJourneySpell)
                {
                    m.SendLocalizedMessage(501035); // You cannot teleport from here to the destination.
                    return false;
                }
            }

            return base.OnBeginSpellCast(m, s);
        }
    }
}