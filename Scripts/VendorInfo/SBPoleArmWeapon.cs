using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class SBPoleArmWeapon : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo(typeof(Bardiche), 60, 20, 0xF4D, 0));
                Add(new GenericBuyInfo(typeof(Halberd), 42, 20, 0x143E, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(Bardiche), 30);
                Add(typeof(Halberd), 21);
            }
        }
    }
}
