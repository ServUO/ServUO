using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class SBLeatherWorker : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo(typeof(Hides), 4, 999, 0x1078, 0, true));
                Add(new GenericBuyInfo(typeof(ThighBoots), 56, 10, 0x1711, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(Hides), 2);
                Add(typeof(ThighBoots), 28);
            }
        }
    }
}
