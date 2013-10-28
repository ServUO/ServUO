using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles 
{ 
    public class SBHerbalist : SBInfo 
    { 
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBHerbalist() 
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
                this.Add(new GenericBuyInfo(typeof(Ginseng), 3, 20, 0xF85, 0)); 
                this.Add(new GenericBuyInfo(typeof(Garlic), 3, 20, 0xF84, 0)); 
                this.Add(new GenericBuyInfo(typeof(MandrakeRoot), 3, 20, 0xF86, 0)); 
                this.Add(new GenericBuyInfo(typeof(Nightshade), 3, 20, 0xF88, 0)); 
                this.Add(new GenericBuyInfo(typeof(Bloodmoss), 5, 20, 0xF7B, 0)); 
                this.Add(new GenericBuyInfo(typeof(MortarPestle), 8, 20, 0xE9B, 0));
                this.Add(new GenericBuyInfo(typeof(Bottle), 5, 20, 0xF0E, 0)); 
            }
        }

        public class InternalSellInfo : GenericSellInfo 
        { 
            public InternalSellInfo() 
            { 
                this.Add(typeof(Bloodmoss), 3); 
                this.Add(typeof(MandrakeRoot), 2); 
                this.Add(typeof(Garlic), 2); 
                this.Add(typeof(Ginseng), 2); 
                this.Add(typeof(Nightshade), 2); 
                this.Add(typeof(Bottle), 3); 
                this.Add(typeof(MortarPestle), 4); 
            }
        }
    }
}