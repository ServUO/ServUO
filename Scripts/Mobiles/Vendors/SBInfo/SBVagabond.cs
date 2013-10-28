using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles 
{ 
    public class SBVagabond : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBVagabond()
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
                this.Add(new GenericBuyInfo(typeof(GoldRing), 27, 20, 0x108A, 0));
                this.Add(new GenericBuyInfo(typeof(Necklace), 26, 20, 0x1085, 0));
                this.Add(new GenericBuyInfo(typeof(GoldNecklace), 27, 20, 0x1088, 0));
                this.Add(new GenericBuyInfo(typeof(GoldBeadNecklace), 27, 20, 0x1089, 0));
                this.Add(new GenericBuyInfo(typeof(Beads), 27, 20, 0x108B, 0));
                this.Add(new GenericBuyInfo(typeof(GoldBracelet), 27, 20, 0x1086, 0));
                this.Add(new GenericBuyInfo(typeof(GoldEarrings), 27, 20, 0x1087, 0));

                this.Add(new GenericBuyInfo(typeof(Board), 3, 20, 0x1BD7, 0));
                this.Add(new GenericBuyInfo(typeof(IronIngot), 6, 20, 0x1BF2, 0));

                this.Add(new GenericBuyInfo(typeof(StarSapphire), 125, 20, 0xF21, 0));
                this.Add(new GenericBuyInfo(typeof(Emerald), 100, 20, 0xF10, 0));
                this.Add(new GenericBuyInfo(typeof(Sapphire), 100, 20, 0xF19, 0));
                this.Add(new GenericBuyInfo(typeof(Ruby), 75, 20, 0xF13, 0));
                this.Add(new GenericBuyInfo(typeof(Citrine), 50, 20, 0xF15, 0));
                this.Add(new GenericBuyInfo(typeof(Amethyst), 100, 20, 0xF16, 0));
                this.Add(new GenericBuyInfo(typeof(Tourmaline), 75, 20, 0xF2D, 0));
                this.Add(new GenericBuyInfo(typeof(Amber), 50, 20, 0xF25, 0));
                this.Add(new GenericBuyInfo(typeof(Diamond), 200, 20, 0xF26, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(Board), 1);
                this.Add(typeof(IronIngot), 3);

                this.Add(typeof(Amber), 25);
                this.Add(typeof(Amethyst), 50);
                this.Add(typeof(Citrine), 25);
                this.Add(typeof(Diamond), 100);
                this.Add(typeof(Emerald), 50);
                this.Add(typeof(Ruby), 37);
                this.Add(typeof(Sapphire), 50);
                this.Add(typeof(StarSapphire), 62);
                this.Add(typeof(Tourmaline), 47);
                this.Add(typeof(GoldRing), 13);
                this.Add(typeof(SilverRing), 10);
                this.Add(typeof(Necklace), 13);
                this.Add(typeof(GoldNecklace), 13);
                this.Add(typeof(GoldBeadNecklace), 13);
                this.Add(typeof(SilverNecklace), 10);
                this.Add(typeof(SilverBeadNecklace), 10);
                this.Add(typeof(Beads), 13);
                this.Add(typeof(GoldBracelet), 13);
                this.Add(typeof(SilverBracelet), 10);
                this.Add(typeof(GoldEarrings), 13);
                this.Add(typeof(SilverEarrings), 10);
            }
        }
    }
}