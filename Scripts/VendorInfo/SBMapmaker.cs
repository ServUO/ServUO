using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class SBMapmaker : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo(typeof(BlankMap), 5, 40, 0x14EC, 0));
                Add(new GenericBuyInfo(typeof(MapmakersPen), 8, 20, 0x0FBF, 0));
                Add(new GenericBuyInfo(typeof(BlankScroll), 12, 40, 0xEF3, 0));

                for (int i = 0; i < PresetMapEntry.Table.Length; ++i)
                    Add(new PresetMapBuyInfo(PresetMapEntry.Table[i], Utility.RandomMinMax(7, 10), 20));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(BlankScroll), 6);
                Add(typeof(MapmakersPen), 4);
                Add(typeof(BlankMap), 2);
                Add(typeof(CityMap), 3);
                Add(typeof(LocalMap), 3);
                Add(typeof(WorldMap), 3);
                Add(typeof(PresetMapEntry), 3);
                //TODO: Buy back maps that the mapmaker sells!!!
            }
        }
    }
}
