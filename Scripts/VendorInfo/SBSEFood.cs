using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class SBSEFood : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo(typeof(Wasabi), 2, 20, 0x24E8, 0));
                Add(new GenericBuyInfo(typeof(Wasabi), 2, 20, 0x24E9, 0));
                Add(new GenericBuyInfo(typeof(BentoBox), 6, 20, 0x2836, 0));
                Add(new GenericBuyInfo(typeof(BentoBox), 6, 20, 0x2837, 0));
                Add(new GenericBuyInfo(typeof(GreenTeaBasket), 2, 1000, 0x284B, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(Wasabi), 1);
                Add(typeof(BentoBox), 3);
                Add(typeof(GreenTeaBasket), 1);
            }
        }
    }
}
