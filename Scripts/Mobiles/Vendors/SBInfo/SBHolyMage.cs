using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBHolyMage : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBHolyMage()
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
                this.Add(new GenericBuyInfo(typeof(Spellbook), 18, 10, 0xEFA, 0));
                this.Add(new GenericBuyInfo(typeof(ScribesPen), 8, 10, 0xFBF, 0));
                this.Add(new GenericBuyInfo(typeof(BlankScroll), 5, 20, 0x0E34, 0));

                this.Add(new GenericBuyInfo("1041072", typeof(MagicWizardsHat), 11, 10, 0x1718, Utility.RandomDyedHue()));

                this.Add(new GenericBuyInfo(typeof(RecallRune), 15, 10, 0x1f14, 0));

                this.Add(new GenericBuyInfo(typeof(RefreshPotion), 15, 20, 0xF0B, 0));
                this.Add(new GenericBuyInfo(typeof(AgilityPotion), 15, 20, 0xF08, 0));
                this.Add(new GenericBuyInfo(typeof(NightSightPotion), 15, 20, 0xF06, 0)); 
                this.Add(new GenericBuyInfo(typeof(LesserHealPotion), 15, 20, 0xF0C, 0));
                this.Add(new GenericBuyInfo(typeof(StrengthPotion), 15, 20, 0xF09, 0));
                this.Add(new GenericBuyInfo(typeof(LesserCurePotion), 15, 20, 0xF07, 0));

                this.Add(new GenericBuyInfo(typeof(BlackPearl), 5, 20, 0xF7A, 0));
                this.Add(new GenericBuyInfo(typeof(Bloodmoss), 5, 20, 0xF7B, 0));
                this.Add(new GenericBuyInfo(typeof(Garlic), 3, 20, 0xF84, 0));
                this.Add(new GenericBuyInfo(typeof(Ginseng), 3, 20, 0xF85, 0));
                this.Add(new GenericBuyInfo(typeof(MandrakeRoot), 3, 20, 0xF86, 0));
                this.Add(new GenericBuyInfo(typeof(Nightshade), 3, 20, 0xF88, 0));
                this.Add(new GenericBuyInfo(typeof(SpidersSilk), 3, 20, 0xF8D, 0));
                this.Add(new GenericBuyInfo(typeof(SulfurousAsh), 3, 20, 0xF8C, 0));

                Type[] types = Loot.RegularScrollTypes;

                for (int i = 0; i < types.Length && i < 8; ++i)
                {
                    int itemID = 0x1F2E + i;

                    if (i == 6)
                        itemID = 0x1F2D;
                    else if (i > 6)
                        --itemID;

                    this.Add(new GenericBuyInfo(types[i], 12 + ((i / 8) * 10), 20, itemID, 0));
                }
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
                this.Add(typeof(RecallRune), 8);
                this.Add(typeof(Spellbook), 9);
                this.Add(typeof(BlankScroll), 3);

                this.Add(typeof(NightSightPotion), 7);
                this.Add(typeof(AgilityPotion), 7);
                this.Add(typeof(StrengthPotion), 7);
                this.Add(typeof(RefreshPotion), 7);
                this.Add(typeof(LesserCurePotion), 7);
                this.Add(typeof(LesserHealPotion), 7);

                Type[] types = Loot.RegularScrollTypes;

                for (int i = 0; i < types.Length; ++i)
                    this.Add(types[i], ((i / 8) + 2) * 2);
            }
        }
    }
}