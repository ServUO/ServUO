using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBSEFood : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBSEFood()
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
                this.Add(new GenericBuyInfo(typeof(Wasabi), 2, 20, 0x24E8, 0));
                this.Add(new GenericBuyInfo(typeof(Wasabi), 2, 20, 0x24E9, 0));
                this.Add(new GenericBuyInfo(typeof(BentoBox), 6, 20, 0x2836, 0));
                this.Add(new GenericBuyInfo(typeof(BentoBox), 6, 20, 0x2837, 0));
                this.Add(new GenericBuyInfo(typeof(GreenTeaBasket), 2, 20, 0x284B, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(Wasabi), 1);
                this.Add(typeof(BentoBox), 3);
                this.Add(typeof(GreenTeaBasket), 1);
            }
        }
    }
}