using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    public class SBSAWeapons : SBInfo
    {
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBSAWeapons()
        {
        }

        public override IShopSellInfo SellInfo
        {
            get
            {
                return m_SellInfo;
            }
        }
        public override List<GenericBuyInfo> BuyInfo
        {
            get
            {
                return m_BuyInfo;
            }
        }

        public class InternalBuyInfo : List<GenericBuyInfo>
        {
            public InternalBuyInfo()
            {
                Add(new GenericBuyInfo(typeof(DualShortAxes), 83, 20, 0x8FD, 0));
                Add(new GenericBuyInfo(typeof(BloodBlade), 47, 20, 0x8FE, 0));
                Add(new GenericBuyInfo(typeof(Boomerang), 28, 20, 0x8FF, 0));
                Add(new GenericBuyInfo(typeof(Cyclone), 47, 20, 0x901, 0));
                Add(new GenericBuyInfo(typeof(GargishDagger), 20, 20, 0x902, 0));
                Add(new GenericBuyInfo(typeof(DiscMace), 62, 20, 0x903, 0));
                Add(new GenericBuyInfo(typeof(GlassStaff), 11, 20, 0x905, 0));
                Add(new GenericBuyInfo(typeof(SerpentStoneStaff), 30, 20, 0x906, 0));
                Add(new GenericBuyInfo(typeof(Shortblade), 57, 20, 0x907, 0));
                Add(new GenericBuyInfo(typeof(SoulGlaive), 68, 20, 0x090A, 0));
                Add(new GenericBuyInfo(typeof(GargishTalwar), 63, 20, 0x908, 0));
                Add(new GenericBuyInfo(typeof(GargishCleaver), 8, 20, 0x48AE, 0));
                Add(new GenericBuyInfo(typeof(GargishBattleAxe), 22, 20, 0x48B0, 0));
                Add(new GenericBuyInfo(typeof(GargishAxe), 23, 20, 0x48B2, 0));
                Add(new GenericBuyInfo(typeof(GargishBardiche), 47, 20, 0x48B4, 0));
                Add(new GenericBuyInfo(typeof(GargishButcherKnife), 8, 20, 0x48B6, 0));
                Add(new GenericBuyInfo(typeof(GargishGnarledStaff), 11, 20, 0x48B8, 0));
                Add(new GenericBuyInfo(typeof(GargishKatana), 17, 20, 0x48BA, 0));
                Add(new GenericBuyInfo(typeof(GargishKryss), 15, 20, 0x48BC, 0));
                Add(new GenericBuyInfo(typeof(GargishWarFork), 15, 20, 0x48BE, 0));
                Add(new GenericBuyInfo(typeof(GargishWarHammer), 16, 20, 0x48C0, 0));
                Add(new GenericBuyInfo(typeof(GargishMaul), 13, 20, 0x48C2, 0));
                Add(new GenericBuyInfo(typeof(GargishScythe), 31, 20, 0x48C4, 0));
                Add(new GenericBuyInfo(typeof(GargishBoneHarvester), 30, 20, 0x48C6, 0));
                Add(new GenericBuyInfo(typeof(GargishPike), 31, 20, 0x48C8, 0));
                Add(new GenericBuyInfo(typeof(GargishLance), 40, 20, 0x48CA, 0));
                Add(new GenericBuyInfo(typeof(GargishTessen), 23, 20, 0x48CC, 0));
                Add(new GenericBuyInfo(typeof(GargishTekagi), 18, 20, 0x48CE, 0));
                Add(new GenericBuyInfo(typeof(GargishDaisho), 23, 20, 0x48D0, 0));
                Add(new GenericBuyInfo(typeof(GlassSword), 28, 20, 0x90C, 0));

            }
        }

        public class InternalSellInfo : GenericSellInfo
        {
            public InternalSellInfo()
            {
                Add(typeof(DualShortAxes), 41);
                Add(typeof(BloodBlade), 23);
                Add(typeof(Boomerang), 14);
                Add(typeof(Cyclone), 23);
                Add(typeof(GargishDagger), 10);
                Add(typeof(DiscMace), 31);
                Add(typeof(GlassStaff), 5);
                Add(typeof(SerpentStoneStaff), 15);
                Add(typeof(Shortblade), 28);
                Add(typeof(SoulGlaive), 34);
                Add(typeof(GargishTalwar), 31);
                Add(typeof(GargishCleaver), 4);
                Add(typeof(GargishBattleAxe), 11);
                Add(typeof(GargishAxe), 11);
                Add(typeof(GargishBardiche), 23);
                Add(typeof(GargishButcherKnife), 4);
                Add(typeof(GargishGnarledStaff), 5);
                Add(typeof(GargishKatana), 8);
                Add(typeof(GargishKryss), 7);
                Add(typeof(GargishWarFork), 7);
                Add(typeof(GargishWarHammer), 8);
                Add(typeof(GargishMaul), 6);
                Add(typeof(GargishScythe), 15);
                Add(typeof(GargishBoneHarvester), 15);
                Add(typeof(GargishPike), 15);
                Add(typeof(GargishLance), 20);
                Add(typeof(GargishTessen), 11);
                Add(typeof(GargishTekagi), 9);
                Add(typeof(GargishDaisho), 11);
                Add(typeof(GlassSword), 14);
            }
        }
    }
}