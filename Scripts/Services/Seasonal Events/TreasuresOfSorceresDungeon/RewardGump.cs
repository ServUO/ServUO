using Server.Engines.Points;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Engines.SorcerersDungeon
{
    public class SorcerersDungeonRewardGump : BaseRewardGump
    {
        public SorcerersDungeonRewardGump(Mobile owner, PlayerMobile user)
            : base(owner, user, SorcerersDungeonRewards.Rewards, 1158744)
        {
        }

        public override double GetPoints(Mobile m)
        {
            return PointsSystem.SorcerersDungeon.GetPoints(m);
        }

        public override void RemovePoints(double points)
        {
            PointsSystem.SorcerersDungeon.DeductPoints(User, points);
        }
    }
}
