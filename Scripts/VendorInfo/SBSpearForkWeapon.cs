using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBSpearForkWeapon : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBSpearForkWeapon()
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
                this.Add(new GenericBuyInfo(typeof(Pitchfork), 19, 20, 0xE87, 0));
                this.Add(new GenericBuyInfo(typeof(ShortSpear), 23, 20, 0x1403, 0));
                this.Add(new GenericBuyInfo(typeof(Spear), 31, 20, 0xF62, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(Spear), 15);
                this.Add(typeof(Pitchfork), 9);
                this.Add(typeof(ShortSpear), 11);
            }
        }
    }
}