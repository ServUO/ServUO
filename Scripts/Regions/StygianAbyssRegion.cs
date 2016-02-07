using System;
using System.Xml;
using Server.Spells.Chivalry;
using Server.Spells.Seventh;
using Server.Spells.Sixth;

namespace Server.Regions
{
    public class StygianAbyssRegion : BaseRegion
    {
        public StygianAbyssRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        { 
        }

        public override bool AllowHousing(Mobile from, Point3D p)
        {
            return false;
        }

        public override bool OnBeginSpellCast(Mobile m, ISpell s)
        {
            if ((s is GateTravelSpell || s is MarkSpell) && m.IsPlayer())
            {
                m.SendLocalizedMessage(501802); // Thy spell doth not appear to work...
				
                return false;
            }
			
            return base.OnBeginSpellCast(m, s);
        }
    }
}