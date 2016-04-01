using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBLeatherArmor : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBLeatherArmor()
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
                this.Add(new GenericBuyInfo(typeof(LeatherArms), 80, 20, 0x13CD, 0));
                this.Add(new GenericBuyInfo(typeof(LeatherChest), 101, 20, 0x13CC, 0));
                this.Add(new GenericBuyInfo(typeof(LeatherGloves), 60, 20, 0x13C6, 0));
                this.Add(new GenericBuyInfo(typeof(LeatherGorget), 74, 20, 0x13C7, 0));
                this.Add(new GenericBuyInfo(typeof(LeatherLegs), 80, 20, 0x13cb, 0));
                this.Add(new GenericBuyInfo(typeof(LeatherCap), 10, 20, 0x1DB9, 0));
                this.Add(new GenericBuyInfo(typeof(FemaleLeatherChest), 116, 20, 0x1C06, 0));
                this.Add(new GenericBuyInfo(typeof(LeatherBustierArms), 97, 20, 0x1C0A, 0));
                this.Add(new GenericBuyInfo(typeof(LeatherShorts), 86, 20, 0x1C00, 0));
                this.Add(new GenericBuyInfo(typeof(LeatherSkirt), 87, 20, 0x1C08, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(LeatherArms), 40);
                this.Add(typeof(LeatherChest), 52);
                this.Add(typeof(LeatherGloves), 30);
                this.Add(typeof(LeatherGorget), 37);
                this.Add(typeof(LeatherLegs), 40);
                this.Add(typeof(LeatherCap), 5);

                this.Add(typeof(FemaleLeatherChest), 18);
                this.Add(typeof(FemaleStuddedChest), 25);
                this.Add(typeof(LeatherShorts), 14);
                this.Add(typeof(LeatherSkirt), 11);
                this.Add(typeof(LeatherBustierArms), 11);
                this.Add(typeof(StuddedBustierArms), 27);
            }
        }
    }
}