using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBStavesWeapon : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBStavesWeapon()
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
                this.Add(new GenericBuyInfo(typeof(BlackStaff), 22, 20, 0xDF1, 0));
                this.Add(new GenericBuyInfo(typeof(GnarledStaff), 16, 20, 0x13F8, 0));
                this.Add(new GenericBuyInfo(typeof(QuarterStaff), 19, 20, 0xE89, 0));
                this.Add(new GenericBuyInfo(typeof(ShepherdsCrook), 20, 20, 0xE81, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(BlackStaff), 11);
                this.Add(typeof(GnarledStaff), 8);
                this.Add(typeof(QuarterStaff), 9);
                this.Add(typeof(ShepherdsCrook), 10);
            }
        }
    }
}