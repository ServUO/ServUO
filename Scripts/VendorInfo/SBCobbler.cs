using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class SBCobbler : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo(typeof(ThighBoots), 15, 20, 0x1711, Utility.RandomNeutralHue()));
                Add(new GenericBuyInfo(typeof(Shoes), 8, 20, 0x170f, Utility.RandomNeutralHue()));
                Add(new GenericBuyInfo(typeof(Boots), 10, 20, 0x170b, Utility.RandomNeutralHue()));
                Add(new GenericBuyInfo(typeof(Sandals), 5, 20, 0x170d, Utility.RandomNeutralHue()));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(Shoes), 4);
                Add(typeof(Boots), 5);
                Add(typeof(ThighBoots), 7);
                Add(typeof(Sandals), 2);
            }
        }
    }
}
