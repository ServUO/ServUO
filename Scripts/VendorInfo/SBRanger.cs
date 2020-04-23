using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class SBRanger : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new AnimalBuyInfo(1, typeof(Cat), 138, 20, 201, 0));
                Add(new AnimalBuyInfo(1, typeof(Dog), 181, 20, 217, 0));
                Add(new AnimalBuyInfo(1, typeof(PackLlama), 491, 20, 292, 0));
                Add(new AnimalBuyInfo(1, typeof(PackHorse), 606, 20, 291, 0));
                Add(new GenericBuyInfo(typeof(Bandage), 5, 20, 0xE21, 0, true));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
        }
    }
}
