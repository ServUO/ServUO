using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class SBGardener : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo(typeof(Hoe), 17, 20, 0xE86, 2524));
                Add(new GenericBuyInfo(typeof(GardeningContract), 10156, 500, 0x14F0, 0));
                Add(new GenericBuyInfo("1060834", typeof(Engines.Plants.PlantBowl), 2, 20, 0x15FD, 0));
                Add(new BeverageBuyInfo(typeof(Pitcher), BeverageType.Water, 11, 20, 0x1F9D, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(Hoe), 8);
                Add(typeof(Pitcher), 5);
                Add(typeof(Engines.Plants.PlantBowl), 1);
            }
        }
    }
}
