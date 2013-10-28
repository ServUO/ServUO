using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBPlateArmor : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBPlateArmor()
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
                this.Add(new GenericBuyInfo(typeof(PlateGorget), 104, 20, 0x1413, 0));
                this.Add(new GenericBuyInfo(typeof(PlateChest), 243, 20, 0x1415, 0));
                this.Add(new GenericBuyInfo(typeof(PlateLegs), 218, 20, 0x1411, 0));
                this.Add(new GenericBuyInfo(typeof(PlateArms), 188, 20, 0x1410, 0));
                this.Add(new GenericBuyInfo(typeof(PlateGloves), 155, 20, 0x1414, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(PlateArms), 94);
                this.Add(typeof(PlateChest), 121);
                this.Add(typeof(PlateGloves), 72);
                this.Add(typeof(PlateGorget), 52);
                this.Add(typeof(PlateLegs), 109);

                this.Add(typeof(FemalePlateChest), 113);
            }
        }
    }
}