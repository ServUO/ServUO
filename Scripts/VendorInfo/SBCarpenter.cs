using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBCarpenter : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBCarpenter()
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
                this.Add(new GenericBuyInfo(typeof(Nails), 3, 20, 0x102E, 0));
                this.Add(new GenericBuyInfo(typeof(Axle), 2, 20, 0x105B, 0));
                this.Add(new GenericBuyInfo(typeof(Board), 3, 20, 0x1BD7, 0));
                this.Add(new GenericBuyInfo(typeof(DrawKnife), 10, 20, 0x10E4, 0));
                this.Add(new GenericBuyInfo(typeof(Froe), 10, 20, 0x10E5, 0));
                this.Add(new GenericBuyInfo(typeof(Scorp), 10, 20, 0x10E7, 0));
                this.Add(new GenericBuyInfo(typeof(Inshave), 10, 20, 0x10E6, 0));
                this.Add(new GenericBuyInfo(typeof(DovetailSaw), 12, 20, 0x1028, 0));
                this.Add(new GenericBuyInfo(typeof(Saw), 15, 20, 0x1034, 0));
                this.Add(new GenericBuyInfo(typeof(Hammer), 17, 20, 0x102A, 0));
                this.Add(new GenericBuyInfo(typeof(MouldingPlane), 11, 20, 0x102C, 0));
                this.Add(new GenericBuyInfo(typeof(SmoothingPlane), 10, 20, 0x1032, 0));
                this.Add(new GenericBuyInfo(typeof(JointingPlane), 11, 20, 0x1030, 0));
                this.Add(new GenericBuyInfo(typeof(Drums), 21, 20, 0xE9C, 0));
                this.Add(new GenericBuyInfo(typeof(Tambourine), 21, 20, 0xE9D, 0));
                this.Add(new GenericBuyInfo(typeof(LapHarp), 21, 20, 0xEB2, 0));
                this.Add(new GenericBuyInfo(typeof(Lute), 21, 20, 0xEB3, 0));

                Add(new GenericBuyInfo("1154004", typeof(SolventFlask), 50, 500, 7192, 2969));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(WoodenBox), 7);
                this.Add(typeof(SmallCrate), 5);
                this.Add(typeof(MediumCrate), 6);
                this.Add(typeof(LargeCrate), 7);
                this.Add(typeof(WoodenChest), 15);
              
                this.Add(typeof(LargeTable), 10);
                this.Add(typeof(Nightstand), 7);
                this.Add(typeof(YewWoodTable), 10);

                this.Add(typeof(Throne), 24);
                this.Add(typeof(WoodenThrone), 6);
                this.Add(typeof(Stool), 6);
                this.Add(typeof(FootStool), 6);

                this.Add(typeof(FancyWoodenChairCushion), 12);
                this.Add(typeof(WoodenChairCushion), 10);
                this.Add(typeof(WoodenChair), 8);
                this.Add(typeof(BambooChair), 6);
                this.Add(typeof(WoodenBench), 6);

                this.Add(typeof(Saw), 9);
                this.Add(typeof(Scorp), 6);
                this.Add(typeof(SmoothingPlane), 6);
                this.Add(typeof(DrawKnife), 6);
                this.Add(typeof(Froe), 6);
                this.Add(typeof(Hammer), 14);
                this.Add(typeof(Inshave), 6);
                this.Add(typeof(JointingPlane), 6);
                this.Add(typeof(MouldingPlane), 6);
                this.Add(typeof(DovetailSaw), 7);
                this.Add(typeof(Board), 2);
                this.Add(typeof(Axle), 1);

                this.Add(typeof(Club), 13);

                this.Add(typeof(Lute), 10);
                this.Add(typeof(LapHarp), 10);
                this.Add(typeof(Tambourine), 10);
                this.Add(typeof(Drums), 10);

                this.Add(typeof(Log), 1);
            }
        }
    }
}