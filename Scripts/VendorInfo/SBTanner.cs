using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class SBTanner : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo(typeof(LeatherGorget), 31, 20, 0x13C7, 0));
                Add(new GenericBuyInfo(typeof(LeatherCap), 10, 20, 0x1DB9, 0));
                Add(new GenericBuyInfo(typeof(LeatherArms), 37, 20, 0x13CD, 0));
                Add(new GenericBuyInfo(typeof(LeatherChest), 47, 20, 0x13CC, 0));
                Add(new GenericBuyInfo(typeof(LeatherLegs), 36, 20, 0x13CB, 0));
                Add(new GenericBuyInfo(typeof(LeatherGloves), 31, 20, 0x13C6, 0));

                Add(new GenericBuyInfo(typeof(StuddedGorget), 50, 20, 0x13D6, 0));
                Add(new GenericBuyInfo(typeof(StuddedArms), 57, 20, 0x13DC, 0));
                Add(new GenericBuyInfo(typeof(StuddedChest), 75, 20, 0x13DB, 0));
                Add(new GenericBuyInfo(typeof(StuddedLegs), 67, 20, 0x13DA, 0));
                Add(new GenericBuyInfo(typeof(StuddedGloves), 45, 20, 0x13D5, 0));

                Add(new GenericBuyInfo(typeof(FemaleStuddedChest), 62, 20, 0x1C02, 0));
                Add(new GenericBuyInfo(typeof(FemalePlateChest), 207, 20, 0x1C04, 0));
                Add(new GenericBuyInfo(typeof(FemaleLeatherChest), 36, 20, 0x1C06, 0));
                Add(new GenericBuyInfo(typeof(LeatherShorts), 28, 20, 0x1C00, 0));
                Add(new GenericBuyInfo(typeof(LeatherSkirt), 25, 20, 0x1C08, 0));
                Add(new GenericBuyInfo(typeof(LeatherBustierArms), 25, 20, 0x1C0A, 0));
                Add(new GenericBuyInfo(typeof(LeatherBustierArms), 30, 20, 0x1C0B, 0));
                Add(new GenericBuyInfo(typeof(StuddedBustierArms), 50, 20, 0x1C0C, 0));
                Add(new GenericBuyInfo(typeof(StuddedBustierArms), 47, 20, 0x1C0D, 0));

                Add(new GenericBuyInfo(typeof(Bag), 6, 20, 0xE76, 0));
                Add(new GenericBuyInfo(typeof(Pouch), 6, 20, 0xE79, 0));
                Add(new GenericBuyInfo(typeof(Backpack), 15, 20, 0x9B2, 0));
                Add(new GenericBuyInfo(typeof(Leather), 6, 20, 0x1081, 0, true));

                Add(new GenericBuyInfo(typeof(SkinningKnife), 15, 20, 0xEC4, 0));

                Add(new GenericBuyInfo("1041279", typeof(TaxidermyKit), 100000, 20, 0x1EBA, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(Bag), 3);
                Add(typeof(Pouch), 3);
                Add(typeof(Backpack), 7);

                Add(typeof(Leather), 5);

                Add(typeof(SkinningKnife), 7);

                Add(typeof(LeatherArms), 18);
                Add(typeof(LeatherChest), 23);
                Add(typeof(LeatherGloves), 15);
                Add(typeof(LeatherGorget), 15);
                Add(typeof(LeatherLegs), 18);
                Add(typeof(LeatherCap), 5);

                Add(typeof(StuddedArms), 43);
                Add(typeof(StuddedChest), 37);
                Add(typeof(StuddedGloves), 39);
                Add(typeof(StuddedGorget), 22);
                Add(typeof(StuddedLegs), 33);

                Add(typeof(FemaleStuddedChest), 31);
                Add(typeof(StuddedBustierArms), 23);
                Add(typeof(FemalePlateChest), 103);
                Add(typeof(FemaleLeatherChest), 18);
                Add(typeof(LeatherBustierArms), 12);
                Add(typeof(LeatherShorts), 14);
                Add(typeof(LeatherSkirt), 12);
            }
        }
    }
}
