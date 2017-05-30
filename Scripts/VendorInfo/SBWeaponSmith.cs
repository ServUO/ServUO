using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles 
{ 
    public class SBWeaponSmith : SBInfo 
    { 
        private readonly List<GenericBuyInfo> m_BuyInfo = new InternalBuyInfo();
        private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();
        public SBWeaponSmith() 
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
                Add(new GenericBuyInfo(typeof(BlackStaff), 22, 20, 0xDF1, 0));
                Add(new GenericBuyInfo(typeof(Club), 16, 20, 0x13B4, 0));
                Add(new GenericBuyInfo(typeof(GnarledStaff), 16, 20, 0x13F8, 0));
                Add(new GenericBuyInfo(typeof(Mace), 28, 20, 0xF5C, 0));
                Add(new GenericBuyInfo(typeof(Maul), 21, 20, 0x143B, 0));
                Add(new GenericBuyInfo(typeof(QuarterStaff), 19, 20, 0xE89, 0));
                Add(new GenericBuyInfo(typeof(ShepherdsCrook), 20, 20, 0xE81, 0));
                Add(new GenericBuyInfo(typeof(SmithHammer), 21, 20, 0x13E3, 0));
                Add(new GenericBuyInfo(typeof(ShortSpear), 23, 20, 0x1403, 0));
                Add(new GenericBuyInfo(typeof(Spear), 31, 20, 0xF62, 0));
                Add(new GenericBuyInfo(typeof(WarHammer), 25, 20, 0x1439, 0));
                Add(new GenericBuyInfo(typeof(WarMace), 31, 20, 0x1407, 0));

                if (Core.AOS)
                {
                    Add(new GenericBuyInfo(typeof(Scepter), 39, 20, 0x26BC, 0));
                    Add(new GenericBuyInfo(typeof(BladedStaff), 40, 20, 0x26BD, 0));
                }

                Add(new GenericBuyInfo(typeof(Hatchet), 25, 20, 0xF44, 0));
                Add(new GenericBuyInfo(typeof(Hatchet), 27, 20, 0xF43, 0));
                Add(new GenericBuyInfo(typeof(WarFork), 32, 20, 0x1405, 0));

                switch ( Utility.Random(3)) 
                { 
                    case 0:
                        {
                            Add(new GenericBuyInfo(typeof(ExecutionersAxe), 30, 20, 0xF45, 0));
                            Add(new GenericBuyInfo(typeof(Bardiche), 60, 20, 0xF4D, 0));
                            Add(new GenericBuyInfo(typeof(BattleAxe), 26, 20, 0xF47, 0));
                            Add(new GenericBuyInfo(typeof(TwoHandedAxe), 32, 20, 0x1443, 0));

                            Add(new GenericBuyInfo(typeof(Bow), 35, 20, 0x13B2, 0));

                            Add(new GenericBuyInfo(typeof(ButcherKnife), 14, 20, 0x13F6, 0));

                            Add(new GenericBuyInfo(typeof(Crossbow), 46, 20, 0xF50, 0));
                            Add(new GenericBuyInfo(typeof(HeavyCrossbow), 55, 20, 0x13FD, 0));

                            Add(new GenericBuyInfo(typeof(Cutlass), 24, 20, 0x1441, 0));
                            Add(new GenericBuyInfo(typeof(Dagger), 21, 20, 0xF52, 0));
                            Add(new GenericBuyInfo(typeof(Halberd), 42, 20, 0x143E, 0));

                            Add(new GenericBuyInfo(typeof(HammerPick), 26, 20, 0x143D, 0));

                            Add(new GenericBuyInfo(typeof(Katana), 33, 20, 0x13FF, 0));
                            Add(new GenericBuyInfo(typeof(Kryss), 32, 20, 0x1401, 0));
                            Add(new GenericBuyInfo(typeof(Broadsword), 35, 20, 0xF5E, 0));
                            Add(new GenericBuyInfo(typeof(Longsword), 55, 20, 0xF61, 0));
                            Add(new GenericBuyInfo(typeof(ThinLongsword), 27, 20, 0x13B8, 0));
                            Add(new GenericBuyInfo(typeof(VikingSword), 55, 20, 0x13B9, 0));

                            Add(new GenericBuyInfo(typeof(Cleaver), 15, 20, 0xEC3, 0));
                            Add(new GenericBuyInfo(typeof(Axe), 40, 20, 0xF49, 0));
                            Add(new GenericBuyInfo(typeof(DoubleAxe), 52, 20, 0xF4B, 0));
                            Add(new GenericBuyInfo(typeof(Pickaxe), 22, 20, 0xE86, 0));

                            Add(new GenericBuyInfo(typeof(Pitchfork), 19, 20, 0xE87, 0));

                            Add(new GenericBuyInfo(typeof(Scimitar), 36, 20, 0x13B6, 0));

                            Add(new GenericBuyInfo(typeof(SkinningKnife), 14, 20, 0xEC4, 0));

                            Add(new GenericBuyInfo(typeof(LargeBattleAxe), 33, 20, 0x13FB, 0));
                            Add(new GenericBuyInfo(typeof(WarAxe), 29, 20, 0x13B0, 0));

                            if (Core.AOS)
                            {
                                Add(new GenericBuyInfo(typeof(BoneHarvester), 35, 20, 0x26BB, 0));
                                Add(new GenericBuyInfo(typeof(CrescentBlade), 37, 20, 0x26C1, 0));
                                Add(new GenericBuyInfo(typeof(DoubleBladedStaff), 35, 20, 0x26BF, 0));
                                Add(new GenericBuyInfo(typeof(Lance), 34, 20, 0x26C0, 0));
                                Add(new GenericBuyInfo(typeof(Pike), 39, 20, 0x26BE, 0));
                                Add(new GenericBuyInfo(typeof(Scythe), 39, 20, 0x26BA, 0));
                                Add(new GenericBuyInfo(typeof(CompositeBow), 50, 20, 0x26C2, 0));
                                Add(new GenericBuyInfo(typeof(RepeatingCrossbow), 57, 20, 0x26C3, 0));
                            }

                            break;
                        }
                }
            }
        }

        public class InternalSellInfo : GenericSellInfo 
        { 
            public InternalSellInfo() 
            { 
                Add(typeof(BattleAxe), 13);
                Add(typeof(DoubleAxe), 26);
                Add(typeof(ExecutionersAxe), 15);
                Add(typeof(LargeBattleAxe), 16);
                Add(typeof(Pickaxe), 11);
                Add(typeof(TwoHandedAxe), 16);
                Add(typeof(WarAxe), 14);
                Add(typeof(Axe), 20);

                Add(typeof(Bardiche), 30);
                Add(typeof(Halberd), 21);

                Add(typeof(ButcherKnife), 7);
                Add(typeof(Cleaver), 7);
                Add(typeof(Dagger), 10);
                Add(typeof(SkinningKnife), 7);

                Add(typeof(Club), 8);
                Add(typeof(HammerPick), 13);
                Add(typeof(Mace), 14);
                Add(typeof(Maul), 10);
                Add(typeof(WarHammer), 12);
                Add(typeof(WarMace), 15);

                Add(typeof(HeavyCrossbow), 27);
                Add(typeof(Bow), 17);
                Add(typeof(Crossbow), 23); 

                if (Core.AOS)
                {
                    Add(typeof(CompositeBow), 25);
                    Add(typeof(RepeatingCrossbow), 28);
                    Add(typeof(Scepter), 20);
                    Add(typeof(BladedStaff), 20);
                    Add(typeof(Scythe), 19);
                    Add(typeof(BoneHarvester), 17);
                    Add(typeof(Scepter), 18);
                    Add(typeof(BladedStaff), 16);
                    Add(typeof(Pike), 19);
                    Add(typeof(DoubleBladedStaff), 17);
                    Add(typeof(Lance), 17);
                    Add(typeof(CrescentBlade), 18);
                }

                Add(typeof(Spear), 15);
                Add(typeof(Pitchfork), 9);
                Add(typeof(ShortSpear), 11);

                Add(typeof(BlackStaff), 11);
                Add(typeof(GnarledStaff), 8);
                Add(typeof(QuarterStaff), 9);
                Add(typeof(ShepherdsCrook), 10);

                Add(typeof(SmithHammer), 10);

                Add(typeof(Broadsword), 17);
                Add(typeof(Cutlass), 12);
                Add(typeof(Katana), 16);
                Add(typeof(Kryss), 16);
                Add(typeof(Longsword), 27);
                Add(typeof(Scimitar), 18);
                Add(typeof(ThinLongsword), 13);
                Add(typeof(VikingSword), 27);

                Add(typeof(Hatchet), 13);
                Add(typeof(WarFork), 16);
            }
        }
    }
}