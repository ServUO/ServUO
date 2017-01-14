using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBKnifeWeapon : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBKnifeWeapon()
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
                this.Add(new GenericBuyInfo(typeof(ButcherKnife), 14, 20, 0x13F6, 0));
                this.Add(new GenericBuyInfo(typeof(Dagger), 21, 20, 0xF52, 0));
                this.Add(new GenericBuyInfo(typeof(Cleaver), 15, 20, 0xEC3, 0));
                this.Add(new GenericBuyInfo(typeof(SkinningKnife), 14, 20, 0xEC4, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(ButcherKnife), 7);
                this.Add(typeof(Cleaver), 7);
                this.Add(typeof(Dagger), 10);
                this.Add(typeof(SkinningKnife), 7);
            }
        }
    }
}