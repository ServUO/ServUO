using Server.Engines.Points;
using Server.Gumps;
using Server.Mobiles;

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