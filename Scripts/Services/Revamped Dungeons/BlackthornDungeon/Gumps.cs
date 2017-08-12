using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using System.Linq;
using Server.Engines.Points;
using Server.Commands;

namespace Server.Engines.Blackthorn
{
	public class BlackthornRewardGump : BaseRewardGump
	{
        public BlackthornRewardGump(Mobile owner, PlayerMobile user)
            : base(owner, user, BlackthornRewards.Rewards, 1154516)
		{
		}

        public override double GetPoints(Mobile m)
        {
            return PointsSystem.Blackthorn.GetPoints(m);
        }

        public override void RemovePoints(double points)
        {
            PointsSystem.Blackthorn.DeductPoints(User, points);
        }
	}
}