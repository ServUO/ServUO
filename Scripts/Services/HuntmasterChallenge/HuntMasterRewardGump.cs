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

        private static List<CollectionItem> _Collections = new List<CollectionItem>
        {
            new CollectionItem(typeof(HarvestersBlade), 0x2D20, 1114096, 0, 1.0),
            new CollectionItem(typeof(HornOfPlenty), 18080, 1153503, 0, 1.0),
            new CollectionItem(typeof(HuntmastersRewardTitleDeed), 0x14EF, 0, 0, 1.0),
            new CollectionItem(typeof(RangersGuildSash), 0x1541, 0, 0, 1.0),
            new CollectionItem(typeof(GargishRangersGuildSash), 0x46B5, 0, 0, 1.0),
        };
	}
}