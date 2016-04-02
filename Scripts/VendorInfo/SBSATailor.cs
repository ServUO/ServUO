using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBSATailor : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBSATailor()
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
                this.Add(new GenericBuyInfo(typeof(Cotton), 102, 20, 0xDF9, 0));
                this.Add(new GenericBuyInfo(typeof(Wool), 62, 20, 0xDF8, 0));
                this.Add(new GenericBuyInfo(typeof(Flax), 102, 20, 0x1A9C, 0));
                this.Add(new GenericBuyInfo(typeof(SpoolOfThread), 18, 20, 0xFA0, 0));
                this.Add(new GenericBuyInfo(typeof(SewingKit), 3, 20, 0xF9D, 0)); 
                this.Add(new GenericBuyInfo(typeof(Scissors), 11, 20, 0xF9F, 0));
                this.Add(new GenericBuyInfo(typeof(DyeTub), 8, 20, 0xFAB, 0)); 
                this.Add(new GenericBuyInfo(typeof(Dyes), 8, 20, 0xFA9, 0)); 
                
                this.Add(new GenericBuyInfo(typeof(GargishRobe), 32, 20, 0x4000, 0));
                this.Add(new GenericBuyInfo(typeof(GargishFancyRobe), 46, 20, 0x4002, 0));

                this.Add(new GenericBuyInfo(typeof(GargishClothArms), 61, 20, 0x403, 0));
                this.Add(new GenericBuyInfo(typeof(GargishClothArms), 60, 20, 0x404, 0));
                this.Add(new GenericBuyInfo(typeof(GargishClothChest), 80, 20, 0x405, 0));
                this.Add(new GenericBuyInfo(typeof(GargishClothChest), 85, 20, 0x406, 0));
                this.Add(new GenericBuyInfo(typeof(GargishClothLegs), 60, 20, 0x409, 0));
                this.Add(new GenericBuyInfo(typeof(GargishClothLegs), 65, 20, 0x40A, 0));
                this.Add(new GenericBuyInfo(typeof(GargishClothKilt), 61, 20, 0x407, 0));
                this.Add(new GenericBuyInfo(typeof(GargishClothKilt), 65, 20, 0x408, 0));

            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(Cotton), 51);
                this.Add(typeof(Wool), 31);
                this.Add(typeof(Flax), 51);
                this.Add(typeof(SpoolOfThread), 9);
                this.Add(typeof(SewingKit), 1);
                this.Add(typeof(Scissors), 6);
                this.Add(typeof(DyeTub), 4);
                this.Add(typeof(Dyes), 4);

                this.Add(typeof(GargishRobe), 16);
                this.Add(typeof(GargishFancyRobe), 23);
                this.Add(typeof(GargishClothArms), 30);
                this.Add(typeof(GargishClothArms), 30);
                this.Add(typeof(GargishClothChest), 40);
                this.Add(typeof(GargishClothChest), 42);
                this.Add(typeof(GargishClothLegs), 30);
                this.Add(typeof(GargishClothLegs), 32);
                this.Add(typeof(GargishClothKilt), 30);
                this.Add(typeof(GargishClothKilt), 32);
            }
        }
    }
}