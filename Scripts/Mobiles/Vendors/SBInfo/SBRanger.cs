using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBRanger : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBRanger()
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
                this.Add(new AnimalBuyInfo(1, typeof(Cat), 138, 20, 201, 0));
                this.Add(new AnimalBuyInfo(1, typeof(Dog), 181, 20, 217, 0));
                this.Add(new AnimalBuyInfo(1, typeof(PackLlama), 491, 20, 292, 0));
                this.Add(new AnimalBuyInfo(1, typeof(PackHorse), 606, 20, 291, 0));
                this.Add(new GenericBuyInfo(typeof(Bandage), 5, 20, 0xE21, 0));
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