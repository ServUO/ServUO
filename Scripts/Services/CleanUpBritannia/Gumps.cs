using Server;
using System;
using Server.Mobiles;
using Server.Gumps;
using Server.Items;
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

        public override double GetPoints(Mobile m)
        {
            return PointsSystem.CleanUpBritannia.GetPoints(m);
        }

        public override void RemovePoints(double points)
        {
            PointsSystem.CleanUpBritannia.DeductPoints(User, points);
        }

        public override void OnItemCreated(Item item)
        {
            if (item is ScrollofAlacrity)
            {
                ((ScrollofAlacrity)item).Skill = (SkillName)Utility.Random(SkillInfo.Table.Length);
            }

            item.InvalidateProperties();
        }
    }
}