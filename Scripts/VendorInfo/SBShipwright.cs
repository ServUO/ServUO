using Server.Items;
using Server.Multis;
using Server.Regions;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class SBShipwright : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo;
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBShipwright(Mobile m)
        {
            if (m != null)
            {
                m_BuyInfo = new InternalBuyInfo(m);
            }
        }

        public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo(Mobile m)
            {
                Add(new GenericBuyInfo("1041205", typeof(SmallBoatDeed), 10177, 20, 0x14F2, 0));
                Add(new GenericBuyInfo("1041206", typeof(SmallDragonBoatDeed), 10177, 20, 0x14F2, 0));
                Add(new GenericBuyInfo("1041207", typeof(MediumBoatDeed), 11552, 20, 0x14F2, 0));
                Add(new GenericBuyInfo("1041208", typeof(MediumDragonBoatDeed), 11552, 20, 0x14F2, 0));
                Add(new GenericBuyInfo("1041209", typeof(LargeBoatDeed), 12927, 20, 0x14F2, 0));
                Add(new GenericBuyInfo("1041210", typeof(LargeDragonBoatDeed), 12927, 20, 0x14F2, 0));

                if (m.Region is SeaMarketRegion || m.Region is TokunoDocksRegion)
                {
                    Add(new GenericBuyInfo("1116740", typeof(TokunoGalleonDeed), 150002, 20, 0x14F2, 0));
                    Add(new GenericBuyInfo("1116739", typeof(GargishGalleonDeed), 200002, 20, 0x14F2, 0));
                    Add(new GenericBuyInfo("1116491", typeof(RowBoatDeed), 6252, 20, 0x14F2, 0));
                }

                Add(new GenericBuyInfo(typeof(Spyglass), 3, 20, 0x14F5, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                //You technically CAN sell them back, *BUT* the vendors do not carry enough money to buy with
                Add(typeof(Spyglass), 1);
            }
        }
    }
}
