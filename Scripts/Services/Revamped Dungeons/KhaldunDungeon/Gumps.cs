using Server;
using System;
using Server.Mobiles;
using Server.Gumps;
using Server.Engines.Points;

namespace Server.Engines.Khaldun
{
	public class KhaldunRewardGump : BaseRewardGump
	{
        public KhaldunRewardGump(Mobile owner, PlayerMobile user)
            : base(owner, user, KhaldunRewards.Rewards, 1158744)
		{
		}

        public override double GetPoints(Mobile m)
        {
            return PointsSystem.Khaldun.GetPoints(m);
        }

        public override void RemovePoints(double points)
        {
            PointsSystem.Khaldun.DeductPoints(User, points);
        }
	}
}
