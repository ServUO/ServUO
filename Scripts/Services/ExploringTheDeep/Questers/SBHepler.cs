using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles 
{ 
    public class SBHepler : SBInfo 
    { 
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBHepler() 
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
                this.Add(new GenericBuyInfo("1154215", typeof(SpecialSalvageHook), 1900, 10, 0x14F9, 2654));
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