using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class SBBoatPainter : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

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
