using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBRealEstateBroker : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBRealEstateBroker()
        {
        }

        public override IShopSellInfo SellInfo
        {
            get
            {
                return this.m_SellInfo;
            }
        }
        public override List<GenericBuyInfo> BuyInfo
        {
            get
            {
                return this.m_BuyInfo;
            }
        }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                this.Add(new GenericBuyInfo(typeof(BlankScroll), 5, 20, 0x0E34, 0));
                this.Add(new GenericBuyInfo(typeof(ScribesPen), 8, 20, 0xFBF, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(ScribesPen), 4);
                this.Add(typeof(BlankScroll), 2);
            }
        }
    }
}