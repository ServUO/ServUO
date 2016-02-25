using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBMystic : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBMystic()
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
                this.Add(new GenericBuyInfo(typeof(PurgeMagicScroll), 18, 10, 0x2DA0, 0));
                this.Add(new GenericBuyInfo(typeof(EnchantScroll), 23, 10, 0x2DA1, 0));
                this.Add(new GenericBuyInfo(typeof(SleepScroll), 28, 10, 0x2DA2, 0));
                this.Add(new GenericBuyInfo(typeof(EagleStrikeScroll), 33, 10, 0x2DA3, 0));
                this.Add(new GenericBuyInfo(typeof(AnimatedWeaponScroll), 38, 10, 0x2DA4, 0));
                this.Add(new GenericBuyInfo(typeof(StoneFormScroll), 43, 10, 0x2DA5, 0));
                this.Add(new GenericBuyInfo(typeof(MysticBook), 18, 10, 0x2D9D, 0));
                this.Add(new GenericBuyInfo(typeof(ScribesPen), 8, 10, 0xFBF, 0));
                this.Add(new GenericBuyInfo(typeof(BlankScroll), 5, 20, 0x0E34, 0));
                this.Add(new GenericBuyInfo(typeof(RecallRune), 15, 10, 0x1F14, 0));

                this.Add(new GenericBuyInfo(typeof(RefreshPotion), 15, 10, 0xF0B, 0));
                this.Add(new GenericBuyInfo(typeof(AgilityPotion), 15, 10, 0xF08, 0));
                this.Add(new GenericBuyInfo(typeof(NightSightPotion), 15, 10, 0xF06, 0));
                this.Add(new GenericBuyInfo(typeof(LesserHealPotion), 15, 10, 0xF0C, 0));
                this.Add(new GenericBuyInfo(typeof(StrengthPotion), 15, 10, 0xF09, 0));
                this.Add(new GenericBuyInfo(typeof(LesserPoisonPotion), 15, 10, 0xF0A, 0));
                this.Add(new GenericBuyInfo(typeof(LesserCurePotion), 15, 10, 0xF07, 0));
                this.Add(new GenericBuyInfo(typeof(LesserExplosionPotion), 21, 10, 0xF0D, 0));

                this.Add(new GenericBuyInfo(typeof(BlackPearl), 5, 20, 0xF7A, 0));
                this.Add(new GenericBuyInfo(typeof(Bloodmoss), 5, 20, 0xF7B, 0));
                this.Add(new GenericBuyInfo(typeof(Garlic), 3, 20, 0xF84, 0));
                this.Add(new GenericBuyInfo(typeof(Ginseng), 3, 20, 0xF85, 0));
                this.Add(new GenericBuyInfo(typeof(MandrakeRoot), 3, 20, 0xF86, 0));
                this.Add(new GenericBuyInfo(typeof(Nightshade), 3, 20, 0xF88, 0));
                this.Add(new GenericBuyInfo(typeof(SulfurousAsh), 3, 20, 0xF8C, 0));
                this.Add(new GenericBuyInfo(typeof(SpidersSilk), 3, 20, 0xF8D, 0));


                this.Add(new GenericBuyInfo(typeof(Bone), 3, 20, 0xf7e, 0));
                this.Add(new GenericBuyInfo(typeof(FertileDirt), 3, 20, 0xF81, 0));
                this.Add(new GenericBuyInfo(typeof(NetherBoltScroll), 8, 20, 0x2D9E, 0));
                this.Add(new GenericBuyInfo(typeof(HealingStoneScroll), 13, 20, 0x2D9F, 0));
            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                this.Add(typeof(PurgeMagicScroll), 9);
                this.Add(typeof(EnchantScroll), 11);
                this.Add(typeof(SleepScroll), 14);
                this.Add(typeof(EagleStrikeScroll), 16);
                this.Add(typeof(AnimatedWeaponScroll), 19);
                this.Add(typeof(StoneFormScroll), 21);
                this.Add(typeof(MysticBook), 9);
                this.Add(typeof(RecallRune), 13);

                this.Add(typeof(BlackPearl), 3); 
                this.Add(typeof(Bloodmoss), 4); 
                this.Add(typeof(MandrakeRoot), 2); 
                this.Add(typeof(Garlic), 2); 
                this.Add(typeof(Ginseng), 2); 
                this.Add(typeof(Nightshade), 2); 
                this.Add(typeof(SpidersSilk), 2); 
                this.Add(typeof(SulfurousAsh), 2); 
                
                this.Add(typeof(NetherBoltScroll), 4);
                this.Add(typeof(HealingStoneScroll), 6);
            }
        }
    }
}