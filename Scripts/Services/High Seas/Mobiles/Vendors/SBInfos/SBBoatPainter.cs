using System;
using System.Collections.Generic;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
    public class SBBoatPainter : SBInfo
    {
        private List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private IShopSellInfo m_SellInfo = new InternalSellInfo();

        public SBBoatPainter()
        {
        }

        public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
        public override List<GenericBuyInfo> BuyInfo { get { return m_BuyInfo; } }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo("Boat Paint", typeof(BoatPaint), 6256, 20, 4011, 276, new object[] { 276 }));
                Add(new GenericBuyInfo("Boat Paint", typeof(BoatPaint), 6256, 20, 4011, 396, new object[] { 396 }));
                Add(new GenericBuyInfo("Boat Paint", typeof(BoatPaint), 6256, 20, 4011, 516, new object[] { 516 }));
                Add(new GenericBuyInfo("Boat Paint", typeof(BoatPaint), 6256, 20, 4011, 1900, new object[] { 1900 }));
                Add(new GenericBuyInfo("Boat Paint", typeof(BoatPaint), 6256, 20, 4011, 251, new object[] { 251 }));
                Add(new GenericBuyInfo("Boat Paint", typeof(BoatPaint), 6256, 20, 4011, 246, new object[] { 246 }));
                Add(new GenericBuyInfo("Boat Paint", typeof(BoatPaint), 6256, 20, 4011, 2213, new object[] { 2213 }));
                Add(new GenericBuyInfo("Boat Paint", typeof(BoatPaint), 6256, 20, 4011, 36, new object[] { 36 }));
                Add(new GenericBuyInfo("Boat Paint Remover", typeof(BoatPaintRemover), 6256, 20, 4011, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(LobsterTrap), 10);
            }
        }
    }
}