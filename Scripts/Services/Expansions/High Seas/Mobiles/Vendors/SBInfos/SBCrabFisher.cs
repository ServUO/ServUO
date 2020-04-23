using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class SBCrabFisher : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo("empty lobster trap", typeof(LobsterTrap), 137, 500, 17615, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(LobsterTrap), 10);
                Add(typeof(AppleCrab), 10);
                Add(typeof(BlueCrab), 10);
                Add(typeof(DungeonessCrab), 10);
                Add(typeof(KingCrab), 10);
                Add(typeof(RockCrab), 10);
                Add(typeof(SnowCrab), 10);
                Add(typeof(StoneCrab), 250);
                Add(typeof(SpiderCrab), 250);
                Add(typeof(TunnelCrab), 2500);
                Add(typeof(VoidCrab), 2500);

                Add(typeof(CrustyLobster), 10);
                Add(typeof(FredLobster), 10);
                Add(typeof(HummerLobster), 10);
                Add(typeof(RockLobster), 10);
                Add(typeof(ShovelNoseLobster), 10);
                Add(typeof(SpineyLobster), 10);
                Add(typeof(BlueLobster), 250);
                Add(typeof(BloodLobster), 2500);
                Add(typeof(DreadLobster), 2500);
                Add(typeof(VoidLobster), 2500);

                Add(typeof(StoneCrabMeat), 100);
                Add(typeof(SpiderCrabMeat), 100);
                Add(typeof(BlueLobsterMeat), 100);
            }
        }
    }
}
