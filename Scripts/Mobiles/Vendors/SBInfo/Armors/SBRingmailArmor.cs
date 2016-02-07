using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBRingmailArmor : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBRingmailArmor()
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
                this.Add(new GenericBuyInfo(typeof(RingmailChest), 121, 20, 0x13ec, 0));
                this.Add(new GenericBuyInfo(typeof(RingmailLegs), 90, 20, 0x13F0, 0));
                this.Add(new GenericBuyInfo(typeof(RingmailArms), 85, 20, 0x13EE, 0));
                this.Add(new GenericBuyInfo(typeof(RingmailGloves), 93, 20, 0x13eb, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(RingmailArms), 42);
                this.Add(typeof(RingmailChest), 60);
                this.Add(typeof(RingmailGloves), 26);
                this.Add(typeof(RingmailLegs), 45);
            }
        }
    }
}