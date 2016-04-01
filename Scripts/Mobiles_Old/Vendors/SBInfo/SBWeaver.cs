using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles 
{ 
    public class SBWeaver : SBInfo 
    { 
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBWeaver() 
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
                this.Add(new GenericBuyInfo(typeof(Dyes), 8, 20, 0xFA9, 0)); 
                this.Add(new GenericBuyInfo(typeof(DyeTub), 8, 20, 0xFAB, 0)); 

                this.Add(new GenericBuyInfo(typeof(UncutCloth), 3, 20, 0x1761, 0)); 
                this.Add(new GenericBuyInfo(typeof(UncutCloth), 3, 20, 0x1762, 0)); 
                this.Add(new GenericBuyInfo(typeof(UncutCloth), 3, 20, 0x1763, 0)); 
                this.Add(new GenericBuyInfo(typeof(UncutCloth), 3, 20, 0x1764, 0)); 

                this.Add(new GenericBuyInfo(typeof(BoltOfCloth), 100, 20, 0xf9B, 0)); 
                this.Add(new GenericBuyInfo(typeof(BoltOfCloth), 100, 20, 0xf9C, 0)); 
                this.Add(new GenericBuyInfo(typeof(BoltOfCloth), 100, 20, 0xf96, 0)); 
                this.Add(new GenericBuyInfo(typeof(BoltOfCloth), 100, 20, 0xf97, 0)); 

                this.Add(new GenericBuyInfo(typeof(DarkYarn), 18, 20, 0xE1D, 0));
                this.Add(new GenericBuyInfo(typeof(LightYarn), 18, 20, 0xE1E, 0));
                this.Add(new GenericBuyInfo(typeof(LightYarnUnraveled), 18, 20, 0xE1F, 0));

                this.Add(new GenericBuyInfo(typeof(Scissors), 11, 20, 0xF9F, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo 
        { 
            public InternalSellInfo() 
            { 
                this.Add(typeof(Scissors), 6); 
                this.Add(typeof(Dyes), 4); 
                this.Add(typeof(DyeTub), 4); 
                this.Add(typeof(UncutCloth), 1);
                this.Add(typeof(BoltOfCloth), 50); 
                this.Add(typeof(LightYarnUnraveled), 9);
                this.Add(typeof(LightYarn), 9);
                this.Add(typeof(DarkYarn), 9);
            }
        }
    }
}