using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBSATanner : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBSATanner()
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
                this.Add(new GenericBuyInfo(typeof(Bag), 6, 20, 0xE76, 0));
                this.Add(new GenericBuyInfo(typeof(Pouch), 6, 20, 0xE79, 0));
                this.Add(new GenericBuyInfo(typeof(Backpack), 15, 20, 0x9B2, 0));
                this.Add(new GenericBuyInfo(typeof(Leather), 6, 20, 0x1081, 0));
                this.Add(new GenericBuyInfo(typeof(GargishDagger), 20, 20, 0x902, 0));
                this.Add(new GenericBuyInfo("1041279", typeof(TaxidermyKit), 100000, 20, 0x1EBA, 0));

                this.Add(new GenericBuyInfo(typeof(GargishLeatherArms), 73, 20, 0x301, 0));
                this.Add(new GenericBuyInfo(typeof(GargishLeatherArms), 78, 20, 0x302, 0));
                this.Add(new GenericBuyInfo(typeof(GargishLeatherChest), 86, 20, 0x303, 0));
                this.Add(new GenericBuyInfo(typeof(GargishLeatherChest), 91, 20, 0x304, 0));
                this.Add(new GenericBuyInfo(typeof(GargishLeatherKilt), 82, 20, 0x310, 0));
                this.Add(new GenericBuyInfo(typeof(GargishLeatherKilt), 92, 20, 0x311, 0));
                this.Add(new GenericBuyInfo(typeof(GargishLeatherLegs), 71, 20, 0x305, 0));

            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(Bag), 3);
                this.Add(typeof(Pouch), 3);
                this.Add(typeof(Backpack), 7);
                this.Add(typeof(Leather), 5);
                this.Add(typeof(GargishDagger), 10);

                this.Add(typeof(GargishLeatherArms), 36);
                this.Add(typeof(GargishLeatherArms), 39);
                this.Add(typeof(GargishLeatherChest), 43);
                this.Add(typeof(GargishLeatherChest), 45);
                this.Add(typeof(GargishLeatherKilt), 41);
                this.Add(typeof(GargishLeatherKilt), 46);
                this.Add(typeof(GargishLeatherLegs), 35);
            }
        }
    }
}