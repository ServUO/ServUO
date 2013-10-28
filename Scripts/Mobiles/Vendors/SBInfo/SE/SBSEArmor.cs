using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBSEArmor : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBSEArmor()
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
                this.Add(new GenericBuyInfo(typeof(PlateHatsuburi), 76, 20, 0x2775, 0));
                this.Add(new GenericBuyInfo(typeof(HeavyPlateJingasa), 76, 20, 0x2777, 0));
                this.Add(new GenericBuyInfo(typeof(DecorativePlateKabuto), 95, 20, 0x2778, 0));
                this.Add(new GenericBuyInfo(typeof(PlateDo), 310, 20, 0x277D, 0));
                this.Add(new GenericBuyInfo(typeof(PlateHiroSode), 222, 20, 0x2780, 0));
                this.Add(new GenericBuyInfo(typeof(PlateSuneate), 224, 20, 0x2788, 0));
                this.Add(new GenericBuyInfo(typeof(PlateHaidate), 235, 20, 0x278D, 0));
                this.Add(new GenericBuyInfo(typeof(ChainHatsuburi), 76, 20, 0x2774, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(PlateHatsuburi), 38);
                this.Add(typeof(HeavyPlateJingasa), 38);
                this.Add(typeof(DecorativePlateKabuto), 47);
                this.Add(typeof(PlateDo), 155);
                this.Add(typeof(PlateHiroSode), 111);
                this.Add(typeof(PlateSuneate), 112);
                this.Add(typeof(PlateHaidate), 117);
                this.Add(typeof(ChainHatsuburi), 38);
            }
        }
    }
}