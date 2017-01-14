using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBGlassblower : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBGlassblower()
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
                this.Add(new GenericBuyInfo(typeof(RefreshPotion), 15, 10, 0xF0B, 0));
                this.Add(new GenericBuyInfo(typeof(AgilityPotion), 15, 10, 0xF08, 0));
                this.Add(new GenericBuyInfo(typeof(NightSightPotion), 15, 10, 0xF06, 0));
                this.Add(new GenericBuyInfo(typeof(LesserHealPotion), 15, 10, 0xF0C, 0));
                this.Add(new GenericBuyInfo(typeof(StrengthPotion), 15, 10, 0xF09, 0));
                this.Add(new GenericBuyInfo(typeof(LesserPoisonPotion), 15, 10, 0xF0A, 0));
                this.Add(new GenericBuyInfo(typeof(LesserCurePotion), 15, 10, 0xF07, 0));
                this.Add(new GenericBuyInfo(typeof(LesserExplosionPotion), 21, 10, 0xF0D, 0));

                this.Add(new GenericBuyInfo(typeof(MortarPestle), 8, 10, 0xE9B, 0));

                this.Add(new GenericBuyInfo(typeof(BlackPearl), 5, 20, 0xF7A, 0));
                this.Add(new GenericBuyInfo(typeof(Bloodmoss), 5, 20, 0xF7B, 0));
                this.Add(new GenericBuyInfo(typeof(Garlic), 3, 20, 0xF84, 0));
                this.Add(new GenericBuyInfo(typeof(Ginseng), 3, 20, 0xF85, 0));
                this.Add(new GenericBuyInfo(typeof(MandrakeRoot), 3, 20, 0xF86, 0));
                this.Add(new GenericBuyInfo(typeof(Nightshade), 3, 20, 0xF88, 0));
                this.Add(new GenericBuyInfo(typeof(SpidersSilk), 3, 20, 0xF8D, 0));
                this.Add(new GenericBuyInfo(typeof(SulfurousAsh), 3, 20, 0xF8C, 0));

                this.Add(new GenericBuyInfo(typeof(Bottle), 5, 100, 0xF0E, 0)); 

                this.Add(new GenericBuyInfo(typeof(HeatingStand), 2, 100, 0x1849, 0)); 

                this.Add(new GenericBuyInfo("Crafting Glass With Glassblowing", typeof(GlassblowingBook), 10637, 30, 0xFF4, 0));
                this.Add(new GenericBuyInfo("Finding Glass-Quality Sand", typeof(SandMiningBook), 10637, 30, 0xFF4, 0));
                this.Add(new GenericBuyInfo("1044608", typeof(Blowpipe), 21, 100, 0xE8A, 0x3B9));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(BlackPearl), 3); 
                this.Add(typeof(Bloodmoss), 3); 
                this.Add(typeof(MandrakeRoot), 2); 
                this.Add(typeof(Garlic), 2); 
                this.Add(typeof(Ginseng), 2); 
                this.Add(typeof(Nightshade), 2); 
                this.Add(typeof(SpidersSilk), 2); 
                this.Add(typeof(SulfurousAsh), 2); 
                this.Add(typeof(Bottle), 3);
                this.Add(typeof(MortarPestle), 4);

                this.Add(typeof(NightSightPotion), 7);
                this.Add(typeof(AgilityPotion), 7);
                this.Add(typeof(StrengthPotion), 7);
                this.Add(typeof(RefreshPotion), 7);
                this.Add(typeof(LesserCurePotion), 7);
                this.Add(typeof(LesserHealPotion), 7);
                this.Add(typeof(LesserPoisonPotion), 7);
                this.Add(typeof(LesserExplosionPotion), 10);

                this.Add(typeof(GlassblowingBook), 5000);
                this.Add(typeof(SandMiningBook), 5000);
                this.Add(typeof(Blowpipe), 10);
            }
        }
    }
}