using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBMage : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBMage()
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
				
                if (Core.AOS)
                    this.Add(new GenericBuyInfo(typeof(NecromancerSpellbook), 115, 10, 0x2253, 0));
				
                this.Add(new GenericBuyInfo(typeof(ScribesPen), 8, 10, 0xFBF, 0));

                this.Add(new GenericBuyInfo(typeof(BlankScroll), 5, 20, 0x0E34, 0));

                this.Add(new GenericBuyInfo("1041072", typeof(MagicWizardsHat), 11, 10, 0x1718, Utility.RandomDyedHue()));

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
                this.Add(new GenericBuyInfo(typeof(SpidersSilk), 3, 20, 0xF8D, 0));
                this.Add(new GenericBuyInfo(typeof(SulfurousAsh), 3, 20, 0xF8C, 0));

                if (Core.AOS)
                {
                    this.Add(new GenericBuyInfo(typeof(BatWing), 3, 999, 0xF78, 0));
                    this.Add(new GenericBuyInfo(typeof(DaemonBlood), 6, 999, 0xF7D, 0));
                    this.Add(new GenericBuyInfo(typeof(PigIron), 5, 999, 0xF8A, 0));
                    this.Add(new GenericBuyInfo(typeof(NoxCrystal), 6, 999, 0xF8E, 0));
                    this.Add(new GenericBuyInfo(typeof(GraveDust), 3, 999, 0xF8F, 0));
                }

                Type[] types = Loot.RegularScrollTypes;

                int circles = 3;

                for (int i = 0; i < circles * 8 && i < types.Length; ++i)
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
                this.Add(typeof(WizardsHat), 15);
                this.Add(typeof(BlackPearl), 3); 
                this.Add(typeof(Bloodmoss), 4); 
                this.Add(typeof(MandrakeRoot), 2); 
                this.Add(typeof(Garlic), 2); 
                this.Add(typeof(Ginseng), 2); 
                this.Add(typeof(Nightshade), 2); 
                this.Add(typeof(SpidersSilk), 2); 
                this.Add(typeof(SulfurousAsh), 2); 

                if (Core.AOS)
                {
                    this.Add(typeof(BatWing), 1);
                    this.Add(typeof(DaemonBlood), 3);
                    this.Add(typeof(PigIron), 2);
                    this.Add(typeof(NoxCrystal), 3);
                    this.Add(typeof(GraveDust), 1);
                }

                this.Add(typeof(RecallRune), 13);
                this.Add(typeof(Spellbook), 25);

                Type[] types = Loot.RegularScrollTypes;

                for (int i = 0; i < types.Length; ++i)
                    this.Add(types[i], ((i / 8) + 2) * 2);

                if (Core.SE)
                { 
                    this.Add(typeof(ExorcismScroll), 3);
                    this.Add(typeof(AnimateDeadScroll), 8);
                    this.Add(typeof(BloodOathScroll), 8);
                    this.Add(typeof(CorpseSkinScroll), 8);
                    this.Add(typeof(CurseWeaponScroll), 8);
                    this.Add(typeof(EvilOmenScroll), 8);
                    this.Add(typeof(PainSpikeScroll), 8);
                    this.Add(typeof(SummonFamiliarScroll), 8);
                    this.Add(typeof(HorrificBeastScroll), 8);
                    this.Add(typeof(MindRotScroll), 10);
                    this.Add(typeof(PoisonStrikeScroll), 10);
                    this.Add(typeof(WraithFormScroll), 15);
                    this.Add(typeof(LichFormScroll), 16);
                    this.Add(typeof(StrangleScroll), 16);
                    this.Add(typeof(WitherScroll), 16);
                    this.Add(typeof(VampiricEmbraceScroll), 20);
                    this.Add(typeof(VengefulSpiritScroll), 20);
                }
            }
        }
    }
}