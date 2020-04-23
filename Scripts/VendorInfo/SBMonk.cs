using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class SBMonk : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo(typeof(MonkRobe), 136, 20, 0x2687, 0x21E));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
        }
    }
}
