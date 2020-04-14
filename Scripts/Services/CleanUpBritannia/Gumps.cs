using Server.Engines.Points;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.CleanUpBritannia
{
    public class CleanUpBritanniaRewardGump : BaseRewardGump
    {
        public CleanUpBritanniaRewardGump(Mobile owner, PlayerMobile user)
            : base(owner, user, CleanUpBritanniaRewards.Rewards, 1151316)
        {
        }

        public override int GetYOffset(int id)
        {
            if (Index > 80)
                return 20;

            return base.GetYOffset(id);
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
            if (item is ScrollOfAlacrity)
            {
                ((ScrollOfAlacrity)item).Skill = (SkillName)Utility.Random(SkillInfo.Table.Length);
            }

            item.InvalidateProperties();
        }
    }
}