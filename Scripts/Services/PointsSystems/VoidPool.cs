using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Engines.Quests;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Engines.VoidPool;

namespace Server.Engines.Points
{
	public class VoidPool : PointsSystem
	{
		public override PointsType Loyalty { get { return PointsType.VoidPool; } }
		public override TextDefinition Name { get { return m_Name; } }
		public override bool AutoAdd { get { return true; } }
		public override double MaxPoints { get { return int.MaxValue; } }

        private TextDefinition m_Name = new TextDefinition(1152733);
		
		public VoidPool()
		{
		}
		
		public override void SendMessage(PlayerMobile from, double old, double points, bool quest)
		{
			from.SendLocalizedMessage(1152649, String.Format("{0}\t{1}", from.Map.ToString(), points)); 
			// For your participation in the Battle for the Void Pool on ~1_FACET~, you have received ~2_POINTS~ reward points. Any reward points you have accumulated may be redeemed by visiting Vela in Cove.
		}
		
		public override TextDefinition GetTitle(PlayerMobile from)
		{
			return new TextDefinition(1152531); // The Void Pool
		}
	}

    public class VoidPoolInfo : ContextMenuEntry
    {
        private Mobile m_From;

        public VoidPoolInfo(Mobile from)
            : base(1152531, -1) // The Void Pool
        {
            m_From = from;
        }

        public override void OnClick()
        {
            Region r = Region.Find(m_From.Location, m_From.Map);

            if (m_From is PlayerMobile && r is VoidPoolRegion && ((VoidPoolRegion)r).Controller != null)
                m_From.SendGump(new VoidPoolGump(((VoidPoolRegion)r).Controller, m_From as PlayerMobile));
        }
    }
}