using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBHealer : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBHealer()
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
                this.Add(new GenericBuyInfo(typeof(Bandage), 5, 20, 0xE21, 0));
                this.Add(new GenericBuyInfo(typeof(LesserHealPotion), 15, 20, 0xF0C, 0));
                this.Add(new GenericBuyInfo(typeof(Ginseng), 3, 20, 0xF85, 0));
                this.Add(new GenericBuyInfo(typeof(Garlic), 3, 20, 0xF84, 0));
                this.Add(new GenericBuyInfo(typeof(RefreshPotion), 15, 20, 0xF0B, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(Bandage), 1);
                this.Add(typeof(LesserHealPotion), 7);
                this.Add(typeof(RefreshPotion), 7);
                this.Add(typeof(Garlic), 2);
                this.Add(typeof(Ginseng), 2);
            }
        }
    }
}