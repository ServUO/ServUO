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
                return m_SellInfo;
            }
        }
        public override List<GenericBuyInfo> BuyInfo
        {
            get
            {
                return m_BuyInfo;
            }
        }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo(typeof(BronzeShield), 66, 20, 0x1B72, 0));
                Add(new GenericBuyInfo(typeof(Buckler), 50, 20, 0x1B73, 0));
                Add(new GenericBuyInfo(typeof(MetalKiteShield), 123, 20, 0x1B74, 0));
                Add(new GenericBuyInfo(typeof(HeaterShield), 231, 20, 0x1B76, 0));
                Add(new GenericBuyInfo(typeof(WoodenKiteShield), 70, 20, 0x1B78, 0));
                Add(new GenericBuyInfo(typeof(MetalShield), 121, 20, 0x1B7B, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(Buckler), 25);
                Add(typeof(BronzeShield), 33);
                Add(typeof(MetalShield), 60);
                Add(typeof(MetalKiteShield), 62);
                Add(typeof(HeaterShield), 115);
                Add(typeof(WoodenKiteShield), 35);
            }
        }
    }
}