using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBPoleArmWeapon : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBPoleArmWeapon()
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
                this.Add(new GenericBuyInfo(typeof(Bardiche), 60, 20, 0xF4D, 0));
                this.Add(new GenericBuyInfo(typeof(Halberd), 42, 20, 0x143E, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(Bardiche), 30);
                this.Add(typeof(Halberd), 21);
            }
        }
    }
}