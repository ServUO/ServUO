using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBSEBowyer : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBSEBowyer()
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
                this.Add(new GenericBuyInfo(typeof(Yumi), 53, 20, 0x27A5, 0));
                this.Add(new GenericBuyInfo(typeof(Fukiya), 20, 20, 0x27AA, 0));
                this.Add(new GenericBuyInfo(typeof(Nunchaku), 35, 20, 0x27AE, 0));
                this.Add(new GenericBuyInfo(typeof(FukiyaDarts), 3, 20, 0x2806, 0));
                this.Add(new GenericBuyInfo(typeof(Bokuto), 21, 20, 0x27A8, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(Yumi), 26);
                this.Add(typeof(Fukiya), 10);
                this.Add(typeof(Nunchaku), 17);
                this.Add(typeof(FukiyaDarts), 1);
                this.Add(typeof(Bokuto), 10);
            }
        }
    }
}