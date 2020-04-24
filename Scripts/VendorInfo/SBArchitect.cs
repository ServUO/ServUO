using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class SBArchitect : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo("1041280", typeof(InteriorDecorator), 10001, 20, 0xFC1, 0));
                Add(new GenericBuyInfo("1060651", typeof(HousePlacementTool), 627, 20, 0x14F6, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(InteriorDecorator), 5000);

                Add(typeof(HousePlacementTool), 301);
            }
        }
    }
}
