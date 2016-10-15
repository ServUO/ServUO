using System;
using Server;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Engines.ResortAndCasino;

namespace Server.Engines.Points
{
    public class BlackthornData : PointsSystem
    {
        public override PointsType Loyalty { get { return PointsType.Blackthorn; } }
        public override TextDefinition Name { get { return m_Name; } }
        public override bool AutoAdd { get { return true; } }
        public override double MaxPoints { get { return int.MaxValue; } }
        public override bool ShowOnLoyaltyGump { get { return false; } }

        private TextDefinition m_Name = null;

        public BlackthornData()
        {
        }

        public override void SendMessage(PlayerMobile from, double old, double points, bool quest)
        {
            from.SendLocalizedMessage(1154518, ((int)points).ToString()); // You have turned in ~1_COUNT~ artifacts bearing the crest of Minax.            
        }
    }
}