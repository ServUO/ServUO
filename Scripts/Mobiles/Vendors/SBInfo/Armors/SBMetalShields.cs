using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBMetalShields : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBMetalShields()
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
                this.Add(new GenericBuyInfo(typeof(BronzeShield), 66, 20, 0x1B72, 0));
                this.Add(new GenericBuyInfo(typeof(Buckler), 50, 20, 0x1B73, 0));
                this.Add(new GenericBuyInfo(typeof(MetalKiteShield), 123, 20, 0x1B74, 0));
                this.Add(new GenericBuyInfo(typeof(HeaterShield), 231, 20, 0x1B76, 0));
                this.Add(new GenericBuyInfo(typeof(WoodenKiteShield), 70, 20, 0x1B78, 0));
                this.Add(new GenericBuyInfo(typeof(MetalShield), 121, 20, 0x1B7B, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(Buckler), 25);
                this.Add(typeof(BronzeShield), 33);
                this.Add(typeof(MetalShield), 60);
                this.Add(typeof(MetalKiteShield), 62);
                this.Add(typeof(HeaterShield), 115);
                this.Add(typeof(WoodenKiteShield), 35);
            }
        }
    }
}