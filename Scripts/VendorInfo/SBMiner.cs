using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBMiner : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBMiner()
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
                Add(new GenericBuyInfo(typeof(Bag), 6, 20, 0xE76, 0));
                Add(new GenericBuyInfo(typeof(Candle), 6, 10, 0xA28, 0));
                Add(new GenericBuyInfo(typeof(Torch), 8, 10, 0xF6B, 0));
                Add(new GenericBuyInfo(typeof(Lantern), 2, 10, 0xA25, 0));
                //Add( new GenericBuyInfo( typeof( OilFlask ), 8, 10, 0x####, 0 ) );
                Add(new GenericBuyInfo(typeof(Pickaxe), 25, 10, 0xE86, 0));
                Add(new GenericBuyInfo(typeof(Shovel), 12, 10, 0xF39, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(Pickaxe), 12);
                Add(typeof(Shovel), 6);
                Add(typeof(Lantern), 1);
                //Add( typeof( OilFlask ), 4 );
                Add(typeof(Torch), 3);
                Add(typeof(Bag), 3);
                Add(typeof(Candle), 3);
            }
        }
    }
}