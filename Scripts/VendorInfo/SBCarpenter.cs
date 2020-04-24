using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class SBCarpenter : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

        public override IShopSellInfo SellInfo => m_SellInfo;
        public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo(typeof(Nails), 3, 20, 0x102E, 0));
                Add(new GenericBuyInfo(typeof(Axle), 2, 20, 0x105B, 0, true));
                Add(new GenericBuyInfo(typeof(Board), 3, 20, 0x1BD7, 0, true));
                Add(new GenericBuyInfo(typeof(DrawKnife), 10, 20, 0x10E4, 0));
                Add(new GenericBuyInfo(typeof(Froe), 10, 20, 0x10E5, 0));
                Add(new GenericBuyInfo(typeof(Scorp), 10, 20, 0x10E7, 0));
                Add(new GenericBuyInfo(typeof(Inshave), 10, 20, 0x10E6, 0));
                Add(new GenericBuyInfo(typeof(DovetailSaw), 12, 20, 0x1028, 0));
                Add(new GenericBuyInfo(typeof(Saw), 15, 20, 0x1034, 0));
                Add(new GenericBuyInfo(typeof(Hammer), 17, 20, 0x102A, 0));
                Add(new GenericBuyInfo(typeof(MouldingPlane), 11, 20, 0x102C, 0));
                Add(new GenericBuyInfo(typeof(SmoothingPlane), 10, 20, 0x1032, 0));
                Add(new GenericBuyInfo(typeof(JointingPlane), 11, 20, 0x1030, 0));
                Add(new GenericBuyInfo(typeof(Drums), 21, 20, 0xE9C, 0));
                Add(new GenericBuyInfo(typeof(Tambourine), 21, 20, 0xE9D, 0));
                Add(new GenericBuyInfo(typeof(LapHarp), 21, 20, 0xEB2, 0));
                Add(new GenericBuyInfo(typeof(Lute), 21, 20, 0xEB3, 0));

                Add(new GenericBuyInfo("1154004", typeof(SolventFlask), 50, 500, 7192, 2969, true));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(WoodenBox), 7);
                Add(typeof(SmallCrate), 5);
                Add(typeof(MediumCrate), 6);
                Add(typeof(LargeCrate), 7);
                Add(typeof(WoodenChest), 15);

                Add(typeof(LargeTable), 10);
                Add(typeof(Nightstand), 7);
                Add(typeof(YewWoodTable), 10);

                Add(typeof(Throne), 24);
                Add(typeof(WoodenThrone), 6);
                Add(typeof(Stool), 6);
                Add(typeof(FootStool), 6);

                Add(typeof(FancyWoodenChairCushion), 12);
                Add(typeof(WoodenChairCushion), 10);
                Add(typeof(WoodenChair), 8);
                Add(typeof(BambooChair), 6);
                Add(typeof(WoodenBench), 6);

                Add(typeof(Saw), 9);
                Add(typeof(Scorp), 6);
                Add(typeof(SmoothingPlane), 6);
                Add(typeof(DrawKnife), 6);
                Add(typeof(Froe), 6);
                Add(typeof(Hammer), 14);
                Add(typeof(Inshave), 6);
                Add(typeof(JointingPlane), 6);
                Add(typeof(MouldingPlane), 6);
                Add(typeof(DovetailSaw), 7);
                Add(typeof(Board), 2);
                Add(typeof(Axle), 1);

                Add(typeof(Club), 13);

                Add(typeof(Lute), 10);
                Add(typeof(LapHarp), 10);
                Add(typeof(Tambourine), 10);
                Add(typeof(Drums), 10);

                Add(typeof(Log), 1);
            }
        }
    }
}
