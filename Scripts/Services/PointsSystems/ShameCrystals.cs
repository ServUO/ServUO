using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Engines.Quests;
using System.Collections.Generic;

namespace Server.Engines.Points
{
	public class ShameCrystals : PointsSystem
	{
		public override PointsType Loyalty { get { return PointsType.ShameCrystals; } }
		public override TextDefinition Name { get { return m_Name; } }
		public override bool AutoAdd { get { return true; } }
		public override double MaxPoints { get { return int.MaxValue; } }
		
		private TextDefinition m_Name = new TextDefinition(1151673);
		
		public ShameCrystals()
		{
		}
		
		public override void SendMessage(PlayerMobile from, double old, double points, bool quest)
		{
            from.SendLocalizedMessage(1151634, String.Format("{0}\t{1}\t{2}", ((int)points).ToString(), "Shame", ((int)old + points).ToString())); // You gain ~1_AMT~ dungeon points for ~2_NAME~. Your total is now ~3_TOTAL~.
		}
		
		public override TextDefinition GetTitle(PlayerMobile from)
		{
            return new TextDefinition(1123444);
		}
	}
}