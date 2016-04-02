using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles 
{ 
    public class SBBaker : SBInfo 
    { 
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBBaker() 
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
                this.Add(new GenericBuyInfo(typeof(BreadLoaf), 6, 20, 0x103B, 0));
                this.Add(new GenericBuyInfo(typeof(BreadLoaf), 5, 20, 0x103C, 0));
                this.Add(new GenericBuyInfo(typeof(ApplePie), 7, 20, 0x1041, 0)); //OSI just has Pie, not Apple/Fruit/Meat
                this.Add(new GenericBuyInfo(typeof(Cake), 13, 20, 0x9E9, 0));
                this.Add(new GenericBuyInfo(typeof(Muffins), 3, 20, 0x9EA, 0));
                this.Add(new GenericBuyInfo(typeof(SackFlour), 3, 20, 0x1039, 0));
                this.Add(new GenericBuyInfo(typeof(FrenchBread), 5, 20, 0x98C, 0));
                this.Add(new GenericBuyInfo(typeof(Cookies), 3, 20, 0x160b, 0)); 
                this.Add(new GenericBuyInfo(typeof(CheesePizza), 8, 10, 0x1040, 0)); // OSI just has Pizza
                this.Add(new GenericBuyInfo(typeof(JarHoney), 3, 20, 0x9ec, 0)); 
                this.Add(new GenericBuyInfo(typeof(BowlFlour), 7, 20, 0xA1E, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo 
        { 
            public InternalSellInfo() 
            { 
                this.Add(typeof(BreadLoaf), 3); 
                this.Add(typeof(FrenchBread), 1); 
                this.Add(typeof(Cake), 5); 
                this.Add(typeof(Cookies), 3); 
                this.Add(typeof(Muffins), 2); 
                this.Add(typeof(CheesePizza), 4); 
                this.Add(typeof(ApplePie), 5); 
                this.Add(typeof(PeachCobbler), 5); 
                this.Add(typeof(Quiche), 6); 
                this.Add(typeof(Dough), 4); 
                this.Add(typeof(JarHoney), 1); 
                this.Add(typeof(Pitcher), 5);
                this.Add(typeof(SackFlour), 1); 
                this.Add(typeof(Eggs), 1); 
            }
        }
    }
}