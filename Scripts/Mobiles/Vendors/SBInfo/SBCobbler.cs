using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles 
{ 
    public class SBCobbler : SBInfo 
    { 
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBCobbler() 
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
                this.Add(new GenericBuyInfo(typeof(ThighBoots), 15, 20, 0x1711, Utility.RandomNeutralHue())); 
                this.Add(new GenericBuyInfo(typeof(Shoes), 8, 20, 0x170f, Utility.RandomNeutralHue())); 
                this.Add(new GenericBuyInfo(typeof(Boots), 10, 20, 0x170b, Utility.RandomNeutralHue()));
                this.Add(new GenericBuyInfo(typeof(Sandals), 5, 20, 0x170d, Utility.RandomNeutralHue())); 
            }
        }

        public class InternalSellInfo : GenericSellInfo 
        { 
            public InternalSellInfo() 
            { 
                this.Add(typeof(Shoes), 4); 
                this.Add(typeof(Boots), 5); 
                this.Add(typeof(ThighBoots), 7); 
                this.Add(typeof(Sandals), 2); 
            }
        }
    }
}