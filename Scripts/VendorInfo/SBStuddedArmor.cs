using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class SBStuddedArmor : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo(typeof(StuddedArms), 87, 20, 0x13DC, 0));
                Add(new GenericBuyInfo(typeof(StuddedChest), 128, 20, 0x13DB, 0));
                Add(new GenericBuyInfo(typeof(StuddedGloves), 79, 20, 0x13D5, 0));
                Add(new GenericBuyInfo(typeof(StuddedGorget), 73, 20, 0x13D6, 0));
                Add(new GenericBuyInfo(typeof(StuddedLegs), 103, 20, 0x13DA, 0));
                Add(new GenericBuyInfo(typeof(FemaleStuddedChest), 142, 20, 0x1C02, 0));
                Add(new GenericBuyInfo(typeof(StuddedBustierArms), 120, 20, 0x1c0c, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(StuddedArms), 43);
                Add(typeof(StuddedChest), 64);
                Add(typeof(StuddedGloves), 39);
                Add(typeof(StuddedGorget), 36);
                Add(typeof(StuddedLegs), 51);
                Add(typeof(FemaleStuddedChest), 71);
                Add(typeof(StuddedBustierArms), 60);
            }
        }
    }
}
