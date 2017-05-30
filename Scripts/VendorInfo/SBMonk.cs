using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBMonk : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBMonk()
        {
        }

        public override IShopSellInfo SellInfo
        {
            get
            {
                return m_SellInfo;
            }
        }
        public override List<GenericBuyInfo> BuyInfo
        {
            get
            {
                return m_BuyInfo;
            }
        }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                if (Core.AOS)
                    Add(new GenericBuyInfo(typeof(MonkRobe), 136, 20, 0x2687, 0x21E));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
            }
        }
    }
}