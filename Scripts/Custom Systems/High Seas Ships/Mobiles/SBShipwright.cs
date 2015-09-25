using System;
using System.Collections.Generic;
using Server.Multis;

namespace Server.Mobiles
{
    public class SBNewShipwright : SBInfo
    {
        private readonly List<GenericBuyInfo> _buyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo _sellInfo = new InternalSellInfo();
        public SBNewShipwright()
        {
        }

        public override IShopSellInfo SellInfo
        {
            get
            {
                return _sellInfo;
            }
        }
        public override List<GenericBuyInfo> BuyInfo
        {
            get
            {
                return _buyInfo;
            }
        }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo("1041205", typeof(NewSmallBoatDeed), 10177, 20, 0x14F2, 0));
				Add(new GenericBuyInfo("1041205", typeof(NewSmallDragonBoatDeed), 15157, 20, 0x14F2, 0));
				Add(new GenericBuyInfo("1041205", typeof(NewMediumBoatDeed), 17157, 20, 0x14F2, 0));
				Add(new GenericBuyInfo("1041205", typeof(NewMediumDragonBoatDeed), 19257, 20, 0x14F2, 0));
				Add(new GenericBuyInfo("1041205", typeof(NewLargeBoatDeed), 29257, 20, 0x14F2, 0));
				Add(new GenericBuyInfo("1041205", typeof(NewLargeDragonBoatDeed), 35257, 20, 0x14F2, 0));
                Add(new GenericBuyInfo("1041206", typeof(TokunoGalleonDeed), 200177, 20, 0x14F2, 0));
                Add(new GenericBuyInfo("1041207", typeof(GargoyleGalleonDeed), 201552, 20, 0x14F2, 0));
                Add(new GenericBuyInfo("1041208", typeof(OrcGalleonDeed), 201552, 20, 0x14F2, 0));
                Add(new GenericBuyInfo("1041209", typeof(BritainGalleonDeed), 202927, 20, 0x14F2, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                //You technically CAN sell them back, *BUT* the vendors do not carry enough money to buy with
            }
        }
    }
}