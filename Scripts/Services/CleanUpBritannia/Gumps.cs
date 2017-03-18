using Server;
using System;
using Server.Mobiles;
using Server.Gumps;
using Server.Engines.Points;

namespace Server.Engines.CleanUpBritannia
{
    public class CleanUpBritanniaRewardGump : BaseRewardGump
    {
        public override int YDist
        {
            get
            {
                if (Index > 80)
                    return 20;

                return base.YDist;
            }
        }

        public CleanUpBritanniaRewardGump(Mobile owner, PlayerMobile user)
            : base(owner, user, CleanUpBritanniaRewards.Rewards, 1151316)
        {
        }

        public override int GetPoints(Mobile m)
        {
            return (int)PointsSystem.CleanUpBritannia.GetPoints(m);
        }

        public override void OnConfirmed(CollectionItem citem, int index)
        {
            base.OnConfirmed(citem, index);

            PointsSystem.CleanUpBritannia.DeductPoints(User, citem.Points);
        }
    }
}