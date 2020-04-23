using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class SBRealEstateBroker : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo(typeof(BlankScroll), 5, 20, 0x0E34, 0));
                Add(new GenericBuyInfo(typeof(ScribesPen), 8, 20, 0xFBF, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(ScribesPen), 4);
                Add(typeof(BlankScroll), 2);
            }
        }
    }
}
