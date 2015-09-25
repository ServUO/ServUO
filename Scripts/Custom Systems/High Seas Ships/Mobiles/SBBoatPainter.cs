using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBBoatPainter : SBInfo
    {
        private readonly List<GenericBuyInfo> _buyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo _sellInfo = new InternalSellInfo();
        public SBBoatPainter()
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
                Add(new GenericBuyInfo("1041205", typeof(GreenBoatPaint), 6256, 20, 0xFAB, 1420));
                Add(new GenericBuyInfo("1041206", typeof(BlueBoatPaint), 6256, 20, 0xFAB, 1303));
                Add(new GenericBuyInfo("1041207", typeof(PurpleBoatPaint), 6256, 20, 0xFAB, 1230));
                Add(new GenericBuyInfo("1041208", typeof(BrownBoatPaint), 6256, 20, 0xFAB, 1501));
                Add(new GenericBuyInfo("1041209", typeof(MaronBoatPaint), 6256, 20, 0xFAB, 2013));
				Add(new GenericBuyInfo("1041209", typeof(RoseBoatPaint), 6256, 20, 0xFAB, 1619));
				Add(new GenericBuyInfo("1041209", typeof(RedBoatPaint), 6256, 20, 0xFAB, 1640));
				Add(new GenericBuyInfo("1041209", typeof(OliveBoatPaint), 6256, 20, 0xFAB, 2001));
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