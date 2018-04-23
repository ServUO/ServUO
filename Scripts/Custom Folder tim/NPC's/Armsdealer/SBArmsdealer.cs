using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBArmsdealer : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBArmsdealer()
        {
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
		Add( new GenericBuyInfo( typeof( FireNeedle ), 50000, 1, 0x1401, 0 ) );
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(FireNeedle), 15000);
                Add(typeof(EarthShaker), 15000);
                Add(typeof(WaveCrusher), 15000);
                Add(typeof(StormBringer), 15000);
                Add(typeof(TheImpaler), 20000);
                Add(typeof(TheCleaver), 10000);
            }
        }
    }
}