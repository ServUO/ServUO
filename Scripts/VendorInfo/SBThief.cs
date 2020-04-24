using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class SBThief : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo(typeof(Backpack), 15, 20, 0x9B2, 0));
                Add(new GenericBuyInfo(typeof(Pouch), 6, 20, 0xE79, 0));
                Add(new GenericBuyInfo(typeof(Torch), 8, 20, 0xF6B, 0));
                Add(new GenericBuyInfo(typeof(Lantern), 2, 20, 0xA25, 0));
                //Add( new GenericBuyInfo( typeof( OilFlask ), 8, 20, 0x####, 0 ) );
                Add(new GenericBuyInfo(typeof(Lockpick), 12, 20, 0x14FC, 0, true));
                Add(new GenericBuyInfo(typeof(WoodenBox), 14, 20, 0x9AA, 0));
                Add(new GenericBuyInfo(typeof(Key), 2, 20, 0x100E, 0));
                Add(new GenericBuyInfo(typeof(HairDye), 37, 20, 0xEFF, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(Backpack), 7);
                Add(typeof(Pouch), 3);
                Add(typeof(Torch), 3);
                Add(typeof(Lantern), 1);
                //Add( typeof( OilFlask ), 4 );
                Add(typeof(Lockpick), 6);
                Add(typeof(WoodenBox), 7);
                Add(typeof(HairDye), 19);
            }
        }
    }
}
