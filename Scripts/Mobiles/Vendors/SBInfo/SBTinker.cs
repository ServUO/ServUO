using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles 
{ 
    public class SBTinker : SBInfo 
    { 
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBTinker() 
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
                this.Add(new GenericBuyInfo(typeof(Clock), 22, 20, 0x104B, 0));
                this.Add(new GenericBuyInfo(typeof(Nails), 3, 20, 0x102E, 0));
                this.Add(new GenericBuyInfo(typeof(ClockParts), 3, 20, 0x104F, 0));
                this.Add(new GenericBuyInfo(typeof(AxleGears), 3, 20, 0x1051, 0));
                this.Add(new GenericBuyInfo(typeof(Gears), 2, 20, 0x1053, 0));
                this.Add(new GenericBuyInfo(typeof(Hinge), 2, 20, 0x1055, 0));

                this.Add(new GenericBuyInfo(typeof(Sextant), 13, 20, 0x1057, 0));
                this.Add(new GenericBuyInfo(typeof(SextantParts), 5, 20, 0x1059, 0));
                this.Add(new GenericBuyInfo(typeof(Axle), 2, 20, 0x105B, 0));
                this.Add(new GenericBuyInfo(typeof(Springs), 3, 20, 0x105D, 0));

                this.Add(new GenericBuyInfo("1024111", typeof(Key), 8, 20, 0x100F, 0));
                this.Add(new GenericBuyInfo("1024112", typeof(Key), 8, 20, 0x1010, 0));
                this.Add(new GenericBuyInfo("1024115", typeof(Key), 8, 20, 0x1013, 0));
                this.Add(new GenericBuyInfo(typeof(KeyRing), 8, 20, 0x1010, 0));
                this.Add(new GenericBuyInfo(typeof(Lockpick), 12, 20, 0x14FC, 0));

                this.Add(new GenericBuyInfo(typeof(TinkersTools), 7, 20, 0x1EBC, 0));
                this.Add(new GenericBuyInfo(typeof(Board), 3, 20, 0x1BD7, 0));
                this.Add(new GenericBuyInfo(typeof(IronIngot), 5, 16, 0x1BF2, 0));
                this.Add(new GenericBuyInfo(typeof(SewingKit), 3, 20, 0xF9D, 0));

                this.Add(new GenericBuyInfo(typeof(DrawKnife), 10, 20, 0x10E4, 0));
                this.Add(new GenericBuyInfo(typeof(Froe), 10, 20, 0x10E5, 0));
                this.Add(new GenericBuyInfo(typeof(Scorp), 10, 20, 0x10E7, 0));
                this.Add(new GenericBuyInfo(typeof(Inshave), 10, 20, 0x10E6, 0));

                this.Add(new GenericBuyInfo(typeof(ButcherKnife), 13, 20, 0x13F6, 0));

                this.Add(new GenericBuyInfo(typeof(Scissors), 11, 20, 0xF9F, 0));

                this.Add(new GenericBuyInfo(typeof(Tongs), 13, 14, 0xFBB, 0));

                this.Add(new GenericBuyInfo(typeof(DovetailSaw), 12, 20, 0x1028, 0));
                this.Add(new GenericBuyInfo(typeof(Saw), 15, 20, 0x1034, 0));

                this.Add(new GenericBuyInfo(typeof(Hammer), 17, 20, 0x102A, 0));
                this.Add(new GenericBuyInfo(typeof(SmithHammer), 23, 20, 0x13E3, 0));
                // TODO: Sledgehammer

                this.Add(new GenericBuyInfo(typeof(Shovel), 12, 20, 0xF39, 0));

                this.Add(new GenericBuyInfo(typeof(MouldingPlane), 11, 20, 0x102C, 0));
                this.Add(new GenericBuyInfo(typeof(JointingPlane), 10, 20, 0x1030, 0));
                this.Add(new GenericBuyInfo(typeof(SmoothingPlane), 11, 20, 0x1032, 0));

                this.Add(new GenericBuyInfo(typeof(Pickaxe), 25, 20, 0xE86, 0));

                this.Add(new GenericBuyInfo(typeof(Drums), 21, 20, 0x0E9C, 0));
                this.Add(new GenericBuyInfo(typeof(Tambourine), 21, 20, 0x0E9E, 0));
                this.Add(new GenericBuyInfo(typeof(LapHarp), 21, 20, 0x0EB2, 0));
                this.Add(new GenericBuyInfo(typeof(Lute), 21, 20, 0x0EB3, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo 
        { 
            public InternalSellInfo() 
            { 
                this.Add(typeof(Drums), 10);
                this.Add(typeof(Tambourine), 10);
                this.Add(typeof(LapHarp), 10);
                this.Add(typeof(Lute), 10);

                this.Add(typeof(Shovel), 6);
                this.Add(typeof(SewingKit), 1);
                this.Add(typeof(Scissors), 6);
                this.Add(typeof(Tongs), 7);
                this.Add(typeof(Key), 1);

                this.Add(typeof(DovetailSaw), 6);
                this.Add(typeof(MouldingPlane), 6);
                this.Add(typeof(Nails), 1);
                this.Add(typeof(JointingPlane), 6);
                this.Add(typeof(SmoothingPlane), 6);
                this.Add(typeof(Saw), 7);

                this.Add(typeof(Clock), 11);
                this.Add(typeof(ClockParts), 1);
                this.Add(typeof(AxleGears), 1);
                this.Add(typeof(Gears), 1);
                this.Add(typeof(Hinge), 1);
                this.Add(typeof(Sextant), 6);
                this.Add(typeof(SextantParts), 2);
                this.Add(typeof(Axle), 1);
                this.Add(typeof(Springs), 1);

                this.Add(typeof(DrawKnife), 5);
                this.Add(typeof(Froe), 5);
                this.Add(typeof(Inshave), 5);
                this.Add(typeof(Scorp), 5);

                this.Add(typeof(Lockpick), 6);
                this.Add(typeof(TinkerTools), 3);

                this.Add(typeof(Board), 1);
                this.Add(typeof(Log), 1);

                this.Add(typeof(Pickaxe), 16);
                this.Add(typeof(Hammer), 3);
                this.Add(typeof(SmithHammer), 11);
                this.Add(typeof(ButcherKnife), 6);
            }
        }
    }
}