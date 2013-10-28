using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBStuddedArmor : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBStuddedArmor()
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
                this.Add(new GenericBuyInfo(typeof(StuddedArms), 87, 20, 0x13DC, 0));
                this.Add(new GenericBuyInfo(typeof(StuddedChest), 128, 20, 0x13DB, 0));
                this.Add(new GenericBuyInfo(typeof(StuddedGloves), 79, 20, 0x13D5, 0));
                this.Add(new GenericBuyInfo(typeof(StuddedGorget), 73, 20, 0x13D6, 0));
                this.Add(new GenericBuyInfo(typeof(StuddedLegs), 103, 20, 0x13DA, 0));
                this.Add(new GenericBuyInfo(typeof(FemaleStuddedChest), 142, 20, 0x1C02, 0));
                this.Add(new GenericBuyInfo(typeof(StuddedBustierArms), 120, 20, 0x1c0c, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(StuddedArms), 43);
                this.Add(typeof(StuddedChest), 64);
                this.Add(typeof(StuddedGloves), 39);
                this.Add(typeof(StuddedGorget), 36);
                this.Add(typeof(StuddedLegs), 51);
                this.Add(typeof(FemaleStuddedChest), 71);
                this.Add(typeof(StuddedBustierArms), 60);
            }
        }
    }
}