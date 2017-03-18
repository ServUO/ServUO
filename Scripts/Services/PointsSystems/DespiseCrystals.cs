using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Engines.Quests;
using System.Collections.Generic;

namespace Server.Engines.Points
{
	public class DespiseCrystals : PointsSystem
	{
		public override PointsType Loyalty { get { return PointsType.DespiseCrystals; } }
		public override TextDefinition Name { get { return m_Name; } }
		public override bool AutoAdd { get { return true; } }
		public override double MaxPoints { get { return int.MaxValue; } }
		
		private TextDefinition m_Name = new TextDefinition(1151673);
		
		public DespiseCrystals()
		{
		}
		
		public override void SendMessage(PlayerMobile from, double old, double points, bool quest)
		{
			from.SendLocalizedMessage(1153423, ((int)points).ToString()); // You have gained ~1_AMT~ Dungeon Crystal Points of Despise.
		}
		
		public override TextDefinition GetTitle(PlayerMobile from)
		{
			return new TextDefinition(1123418);
		}
	}
}