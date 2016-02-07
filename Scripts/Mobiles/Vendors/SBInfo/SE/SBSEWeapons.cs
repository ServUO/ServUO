using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBSEWeapons : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBSEWeapons()
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
                this.Add(new GenericBuyInfo(typeof(NoDachi), 82, 20, 0x27A2, 0));
                this.Add(new GenericBuyInfo(typeof(Tessen), 83, 20, 0x27A3, 0));
                this.Add(new GenericBuyInfo(typeof(Wakizashi), 38, 20, 0x27A4, 0));
                this.Add(new GenericBuyInfo(typeof(Tetsubo), 43, 20, 0x27A6, 0));
                this.Add(new GenericBuyInfo(typeof(Lajatang), 108, 20, 0x27A7, 0));
                this.Add(new GenericBuyInfo(typeof(Daisho), 66, 20, 0x27A9, 0));
                this.Add(new GenericBuyInfo(typeof(Tekagi), 55, 20, 0x27AB, 0));
                this.Add(new GenericBuyInfo(typeof(Shuriken), 18, 20, 0x27AC, 0));
                this.Add(new GenericBuyInfo(typeof(Kama), 61, 20, 0x27AD, 0));
                this.Add(new GenericBuyInfo(typeof(Sai), 56, 20, 0x27AF, 0));		
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(NoDachi), 41);
                this.Add(typeof(Tessen), 41);
                this.Add(typeof(Wakizashi), 19);
                this.Add(typeof(Tetsubo), 21);
                this.Add(typeof(Lajatang), 54);
                this.Add(typeof(Daisho), 33);
                this.Add(typeof(Tekagi), 22);
                this.Add(typeof(Shuriken), 9);
                this.Add(typeof(Kama), 30);
                this.Add(typeof(Sai), 28);
            }
        }
    }
}