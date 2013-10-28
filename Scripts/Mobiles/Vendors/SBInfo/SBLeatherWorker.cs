using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles 
{ 
    public class SBLeatherWorker : SBInfo 
    { 
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBLeatherWorker() 
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
                this.Add(new GenericBuyInfo(typeof(Hides), 4, 999, 0x1078, 0)); 
                this.Add(new GenericBuyInfo(typeof(ThighBoots), 56, 10, 0x1711, 0)); 
            }
        }

        public class InternalSellInfo : GenericSellInfo 
        { 
            public InternalSellInfo() 
            { 
                this.Add(typeof(Hides), 2); 
                this.Add(typeof(ThighBoots), 28); 
            }
        }
    }
}