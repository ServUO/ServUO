using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBBeekeeper : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBBeekeeper()
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
                this.Add(new GenericBuyInfo(typeof(JarHoney), 3, 20, 0x9EC, 0));
                this.Add(new GenericBuyInfo(typeof(Beeswax), 2, 20, 0x1422, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(JarHoney), 1);
                this.Add(typeof(Beeswax), 1);
            }
        }
    }
}