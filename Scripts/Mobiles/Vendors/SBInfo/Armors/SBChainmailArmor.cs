using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBChainmailArmor : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBChainmailArmor()
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
                this.Add(new GenericBuyInfo(typeof(ChainCoif), 17, 20, 0x13BB, 0));
                this.Add(new GenericBuyInfo(typeof(ChainChest), 143, 20, 0x13BF, 0));
                this.Add(new GenericBuyInfo(typeof(ChainLegs), 149, 20, 0x13BE, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(ChainCoif), 6);
                this.Add(typeof(ChainChest), 71);
                this.Add(typeof(ChainLegs), 74);
            }
        }
    }
}