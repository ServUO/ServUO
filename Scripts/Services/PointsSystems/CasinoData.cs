using System;
using Server;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Engines.ResortAndCasino;

namespace Server.Engines.Points
{
    public class CasinoData : PointsSystem
    {
		public override PointsType Loyalty { get { return PointsType.CasinoData; } }
		public override TextDefinition Name { get { return m_Name; } }
		public override bool AutoAdd { get { return true; } }
		public override double MaxPoints { get { return int.MaxValue; } }
        public override bool ShowOnLoyaltyGump { get { return false; } }

        public static readonly int ChipCost = 100;

        private TextDefinition m_Name = new TextDefinition(1153485); // Fortune's Fire Resort & Casino

        public CasinoData()
		{
		}
		
		public override void SendMessage(PlayerMobile from, double old, double points, bool quest)
		{
            //from.SendLocalizedMessage(1153189, ((int)points).ToString());
		}
		
		public override TextDefinition GetTitle(PlayerMobile from)
		{
            return new TextDefinition(1153485); // Fortune's Fire Resort & Casino
		}
    }
}