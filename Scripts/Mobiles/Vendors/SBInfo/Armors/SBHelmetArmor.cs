using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBHelmetArmor : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBHelmetArmor()
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
                this.Add(new GenericBuyInfo(typeof(PlateHelm), 21, 20, 0x1412, 0));
                this.Add(new GenericBuyInfo(typeof(CloseHelm), 18, 20, 0x1408, 0));
                this.Add(new GenericBuyInfo(typeof(CloseHelm), 18, 20, 0x1409, 0));
                this.Add(new GenericBuyInfo(typeof(Helmet), 31, 20, 0x140A, 0));
                this.Add(new GenericBuyInfo(typeof(Helmet), 18, 20, 0x140B, 0));
                this.Add(new GenericBuyInfo(typeof(NorseHelm), 18, 20, 0x140E, 0));
                this.Add(new GenericBuyInfo(typeof(NorseHelm), 18, 20, 0x140F, 0));
                this.Add(new GenericBuyInfo(typeof(Bascinet), 18, 20, 0x140C, 0));
                this.Add(new GenericBuyInfo(typeof(PlateHelm), 21, 20, 0x1419, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(Bascinet), 9);
                this.Add(typeof(CloseHelm), 9);
                this.Add(typeof(Helmet), 9);
                this.Add(typeof(NorseHelm), 9);
                this.Add(typeof(PlateHelm), 10);
            }
        }
    }
}