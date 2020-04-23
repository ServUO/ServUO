using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class SBSELeatherArmor : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo(typeof(LeatherJingasa), 11, 20, 0x2776, 0));
                Add(new GenericBuyInfo(typeof(LeatherDo), 87, 20, 0x277B, 0));
                Add(new GenericBuyInfo(typeof(LeatherHiroSode), 49, 20, 0x277E, 0));
                Add(new GenericBuyInfo(typeof(LeatherSuneate), 55, 20, 0x2786, 0));
                Add(new GenericBuyInfo(typeof(LeatherHaidate), 54, 20, 0x278A, 0));
                Add(new GenericBuyInfo(typeof(LeatherNinjaPants), 49, 20, 0x2791, 0));
                Add(new GenericBuyInfo(typeof(LeatherNinjaJacket), 51, 20, 0x2793, 0));

                Add(new GenericBuyInfo(typeof(StuddedMempo), 61, 20, 0x279D, 0));
                Add(new GenericBuyInfo(typeof(StuddedDo), 130, 20, 0x277C, 0));
                Add(new GenericBuyInfo(typeof(StuddedHiroSode), 73, 20, 0x277F, 0));
                Add(new GenericBuyInfo(typeof(StuddedSuneate), 78, 20, 0x2787, 0));
                Add(new GenericBuyInfo(typeof(StuddedHaidate), 76, 20, 0x278B, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(LeatherJingasa), 5);
                Add(typeof(LeatherDo), 42);
                Add(typeof(LeatherHiroSode), 23);
                Add(typeof(LeatherSuneate), 26);
                Add(typeof(LeatherHaidate), 28);
                Add(typeof(LeatherNinjaPants), 25);
                Add(typeof(LeatherNinjaJacket), 26);
                Add(typeof(StuddedMempo), 28);
                Add(typeof(StuddedDo), 66);
                Add(typeof(StuddedHiroSode), 32);
                Add(typeof(StuddedSuneate), 40);
                Add(typeof(StuddedHaidate), 37);
            }
        }
    }
}
