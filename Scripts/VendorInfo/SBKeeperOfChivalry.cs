using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class SBKeeperOfChivalry : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo(typeof(BookOfChivalry), 140, 20, 0x2252, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
        }
    }
}
