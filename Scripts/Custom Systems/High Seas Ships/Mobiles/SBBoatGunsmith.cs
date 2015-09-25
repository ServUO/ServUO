using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBBoatGunsmith : SBInfo
    {
        private readonly List<GenericBuyInfo> _buyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo _sellInfo = new InternalSellInfo();
        public SBBoatGunsmith()
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
                Add(new GenericBuyInfo("1041205", typeof(LightShipCannonball), 800, 20, 0xE74, 0));
                Add(new GenericBuyInfo("1041206", typeof(IronShipCannonball), 1000, 20, 0xE74, 0));
                Add(new GenericBuyInfo("1041207", typeof(ExplodingShipCannonball), 1000, 20, 0xE74, 46));
                Add(new GenericBuyInfo("1041208", typeof(FieryShipCannonball), 1000, 20, 0xE74, 33));
                Add(new GenericBuyInfo("1041209", typeof(ShipGrapeShot), 1000, 20, 0xE74, 0));
				Add(new GenericBuyInfo("1041209", typeof(LightCannonDeed), 10000, 20, 0x14F0, 0x488));
				Add(new GenericBuyInfo("1041209", typeof(HeavyCannonDeed), 20000, 20, 0x14F0, 0x488));
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