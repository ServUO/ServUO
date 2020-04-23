using System.Collections.Generic;

namespace Server.Mobiles
{
    public class SBAnimalTrainer : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new AnimalBuyInfo(1, typeof(Cat), 132, 10, 201, 0));
                Add(new AnimalBuyInfo(1, typeof(Dog), 170, 10, 217, 0));
                Add(new AnimalBuyInfo(1, typeof(Horse), 550, 10, 204, 0));
                Add(new AnimalBuyInfo(1, typeof(PackHorse), 631, 10, 291, 0));
                Add(new AnimalBuyInfo(1, typeof(PackLlama), 565, 10, 292, 0));
                Add(new AnimalBuyInfo(1, typeof(Rabbit), 106, 10, 205, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
        }
    }
}
