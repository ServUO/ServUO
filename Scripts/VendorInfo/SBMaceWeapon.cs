using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBMaceWeapon : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBMaceWeapon()
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
                this.Add(new GenericBuyInfo(typeof(HammerPick), 26, 20, 0x143D, 0));
                this.Add(new GenericBuyInfo(typeof(Club), 16, 20, 0x13B4, 0));
                this.Add(new GenericBuyInfo(typeof(Mace), 28, 20, 0xF5C, 0));
                this.Add(new GenericBuyInfo(typeof(Maul), 21, 20, 0x143B, 0));
                this.Add(new GenericBuyInfo(typeof(WarHammer), 25, 20, 0x1439, 0));
                this.Add(new GenericBuyInfo(typeof(WarMace), 31, 20, 0x1407, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(Club), 8);
                this.Add(typeof(HammerPick), 13);
                this.Add(typeof(Mace), 14);
                this.Add(typeof(Maul), 10);
                this.Add(typeof(WarHammer), 12);
                this.Add(typeof(WarMace), 15);
            }
        }
    }
}