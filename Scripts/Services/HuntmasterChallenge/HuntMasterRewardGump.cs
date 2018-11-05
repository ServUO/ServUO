using Server;
using System;
using Server.Mobiles;
using Server.Gumps;
using System.Collections.Generic;
using Server.Items;

namespace Server.Engines.HuntsmasterChallenge
{
	public class HuntmasterRewardGump : BaseRewardGump
	{
        public HuntmasterRewardGump(Mobile huntmaster, PlayerMobile pm)
            : base(huntmaster, pm, _Collections, 1155726) // Huntmaster's Challenge
        {
        }

        public override double GetPoints(Mobile m)
        {
            if (HuntingSystem.Instance == null)
                return 0.0;

            var sys = HuntingSystem.Instance;

            if (sys.UnclaimedWinners.ContainsKey(m))
            {
                if (sys.UnclaimedWinners[User] <= 0)
                {
                    sys.UnclaimedWinners.Remove(User);
                }
                else
                {
                    return sys.UnclaimedWinners[m];
                }
            }

            return 0.0;
        }

        public override void RemovePoints(double points)
        {
            if (HuntingSystem.Instance == null)
                return;

            var sys = HuntingSystem.Instance;

            if (sys.UnclaimedWinners.ContainsKey(User))
            {
                sys.UnclaimedWinners[User]--;

                if (sys.UnclaimedWinners[User] <= 0)
                {
                    sys.UnclaimedWinners.Remove(User);
                }
            }
        }

        public override void OnConfirmed(CollectionItem citem, int index)
        {
            if (citem.Type == typeof(RecipeScroll))
            {
                Item item;

                switch (citem.Tooltip)
                {
                    default:
                    case 1158769: item = new RecipeScroll(605); break;
                    case 1158770: item = new RecipeScroll(606); break;
                    case 1158771: item = new RecipeScroll(607); break;
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
                        OnItemCreated(item);

                        User.SendLocalizedMessage(1073621); // Your reward has been placed in your backpack.
                        RemovePoints(citem.Points);
                        User.PlaySound(0x5A8);
                    }
                }
            }
            else
            {
                base.OnConfirmed(citem, index);
            }
        }

        private static List<CollectionItem> _Collections = new List<CollectionItem>
        {
            new CollectionItem(typeof(HarvestersBlade), 0x2D20, 1114096, 0, 1.0),
            new CollectionItem(typeof(HornOfPlenty), 18080, 1153503, 0, 1.0),
            new CollectionItem(typeof(HuntmastersRewardTitleDeed), 0x14EF, 0, 0, 1.0),
            new CollectionItem(typeof(RangersGuildSash), 0x1541, 0, 0, 1.0),
            new CollectionItem(typeof(GargishRangersGuildSash), 0x46B5, 0, 0, 1.0),
            new CollectionItem(typeof(RecipeScroll), 0x2831, 1158769, 0, 1.0), // hamburger recipe
            new CollectionItem(typeof(RecipeScroll), 0x2831, 1158770, 0, 1.0), // hot dog recipe
            new CollectionItem(typeof(RecipeScroll), 0x2831, 1158771, 0, 1.0), // sausage recipe
            new CollectionItem(typeof(MinersSatchel), 0xA272, 1158773, 0, 1.0),
            new CollectionItem(typeof(LumbjacksSatchel), 0xA274, 1158772, 0, 1.0),
            new CollectionItem(typeof(HarvestersAxe), 0x1443, 1158774, 0, 1.0),
        };
	}
}