using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class SBRingmailArmor : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo(typeof(RingmailChest), 121, 20, 0x13ec, 0));
                Add(new GenericBuyInfo(typeof(RingmailLegs), 90, 20, 0x13F0, 0));
                Add(new GenericBuyInfo(typeof(RingmailArms), 85, 20, 0x13EE, 0));
                Add(new GenericBuyInfo(typeof(RingmailGloves), 93, 20, 0x13eb, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(RingmailArms), 42);
                Add(typeof(RingmailChest), 60);
                Add(typeof(RingmailGloves), 26);
                Add(typeof(RingmailLegs), 45);
            }
        }
    }
}
