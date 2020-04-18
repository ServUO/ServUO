using Server.Engines.Craft;
using Server.Engines.Points;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Fellowship
{
    public class FellowshipRewardGump : BaseRewardGump
    {
        public override int PointsName => 1159184;  // Your Fellowship Silver:
        public override int RewardLabel => 1159185;  // Would you like to buy something?

        public FellowshipRewardGump(Mobile owner, PlayerMobile user)
            : base(owner, user, FellowshipRewards.Rewards, 1159183)
        {
        }

        public override int GetYOffset(int id)
        {
            if (Index > 3)
                return 20;

            return base.GetYOffset(id);
        }

        public override double GetPoints(Mobile m)
        {
            return PointsSystem.FellowshipData.GetPoints(m);
        }

        public override void RemovePoints(double points)
        {
            PointsSystem.FellowshipData.DeductPoints(User, points);
        }

        public override void OnItemCreated(Item item)
        {
            item.InvalidateProperties();
        }

        public override void OnConfirmed(CollectionItem citem, int index)
        {
            Item item = null;

            if (citem.Type == typeof(RecipeScroll))
            {
                switch (index)
                {
                    default:
                    case 9: item = new RecipeScroll((int)TailorRecipe.CowlOfTheMaceAndShield); break;
                    case 10: item = new RecipeScroll((int)TailorRecipe.MagesHoodOfScholarlyInsight); break;
                    case 11: item = new RecipeScroll((int)TailorRecipe.ElegantCollarOfFortune); break;
                    case 12: item = new RecipeScroll((int)TailorRecipe.CrimsonDaggerBelt); break;
                    case 13: item = new RecipeScroll((int)TailorRecipe.CrimsonSwordBelt); break;
                    case 14: item = new RecipeScroll((int)TailorRecipe.CrimsonMaceBelt); break;
                }
            }

            if (item != null)
            {
                if (User.Backpack == null || !User.Backpack.TryDropItem(User, item, false))
                {
                    User.SendLocalizedMessage(1074361); // The reward could not be given.  Make sure you have room in your pack.
                    item.Delete();
                }
                else
                {
                    User.SendLocalizedMessage(1073621); // Your reward has been placed in your backpack.
                    User.PlaySound(0x5A7);
                }
            }
            else
            {
                base.OnConfirmed(citem, index);
            }

            PointsSystem.FellowshipData.DeductPoints(User, citem.Points);
        }
    }
}
