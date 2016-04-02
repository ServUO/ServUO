using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public abstract class SBInfo
    {
        public SBInfo()
        {
        }

        public abstract IShopSellInfo SellInfo { get; }
        public abstract List<GenericBuyInfo> BuyInfo { get; }
    }
}