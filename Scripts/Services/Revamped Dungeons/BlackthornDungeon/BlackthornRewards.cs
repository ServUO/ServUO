using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using Server.Items;

namespace Server.Engines.Blackthorn
{
    public static class BlackthornRewards
    {
        public static List<CollectionItem> Rewards { get; set; }
        
        public static void Initialize()
        {
            Rewards = new List<CollectionItem>();

            Rewards.Add(new CollectionItem(typeof(DecorativeShardShieldDeed), 0x14EF, 1153729, 0, 10));
            Rewards.Add(new CollectionItem(typeof(BlackthornPainting1), 0x4C63, 1023744, 0, 10));
            Rewards.Add(new CollectionItem(typeof(BlackthornPainting2), 0x4C65, 1023744, 0, 10));

            // RuneBeetleCarapaceBase
            Rewards.Add(new CollectionItem(typeof(GargishStoneChestBearingTheCrestOfBlackthorn), 0x286, 0, 0, 25));
            Rewards.Add(new CollectionItem(typeof(GargishPlatemailChestBearingTheCrestOfBlackthorn), 0x030A, 0, 0, 25));
            Rewards.Add(new CollectionItem(typeof(DragonBreastplateBearingTheCrestOfBlackthorn), 0x2641, 0, 0, 25));
            Rewards.Add(new CollectionItem(typeof(ChainmailTunicBearingTheCrestOfBlackthorn), 0x13BF, 0, 0, 25));
            Rewards.Add(new CollectionItem(typeof(RingmailTunicBearingTheCrestOfBlackthorn), 0x13EC, 0, 0, 25));
            Rewards.Add(new CollectionItem(typeof(PlatemailTunicBearingTheCrestOfBlackthorn), 0x1415, 0, 0, 25));

            // QuiverofInfinityBase
            Rewards.Add(new CollectionItem(typeof(GargishClothWingArmorBearingTheCrestOfBlackthorn), 0x45A4, 0, 0, 25));
            Rewards.Add(new CollectionItem(typeof(CloakBearingTheCrestOfBlackthorn), 0x1515, 0, 0, 25));

            // ShroudoftheCondemnedBase
            Rewards.Add(new CollectionItem(typeof(HoodedRobeBearingTheCrestOfBlackthorn1), 0x2683, 0, 0, 25));
            Rewards.Add(new CollectionItem(typeof(GargishFancyBearingTheCrestOfBlackthorn1), 0x4002, 0, 0, 25));
            Rewards.Add(new CollectionItem(typeof(GargishEpauletteBearingTheCrestOfBlackthorn1), 0x9986, 0, 0, 25));
            Rewards.Add(new CollectionItem(typeof(EpauletteBearingTheCrestOfBlackthorn1), 0x9985, 0, 0, 25));
            Rewards.Add(new CollectionItem(typeof(GargishRobeBearingTheCrestOfBlackthorn1), 0x4000, 0, 0, 25));
            Rewards.Add(new CollectionItem(typeof(PlainDressBearingTheCrestOfBlackthorn1), 0x1F01, 0, 0, 25));
            Rewards.Add(new CollectionItem(typeof(GildedDressBearingTheCrestOfBlackthorn1), 0x230E, 0, 0, 25));
            Rewards.Add(new CollectionItem(typeof(FancyDressBearingTheCrestOfBlackthorn1), 0x1F00, 0, 0, 25));
            Rewards.Add(new CollectionItem(typeof(FemaleKimonoBearingTheCrestOfBlackthorn1), 0x2783, 0, 0, 25));
            Rewards.Add(new CollectionItem(typeof(MaleKimonoBearingTheCrestOfBlackthorn1), 0x2782, 0, 0, 25));

            // Mystic's Garb Base
            Rewards.Add(new CollectionItem(typeof(HoodedRobeBearingTheCrestOfBlackthorn2), 0x2683, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(GargishFancyBearingTheCrestOfBlackthorn2), 0x4002, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(GargishEpauletteBearingTheCrestOfBlackthorn2), 0x9986, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(EpauletteBearingTheCrestOfBlackthorn2), 0x9985, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(GargishRobeBearingTheCrestOfBlackthorn2), 0x4000, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(PlainDressBearingTheCrestOfBlackthorn2), 0x1F01, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(GildedDressBearingTheCrestOfBlackthorn2), 0x230E, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(FancyDressBearingTheCrestOfBlackthorn2), 0x1F00, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(FemaleKimonoBearingTheCrestOfBlackthorn2), 0x2783, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(MaleKimonoBearingTheCrestOfBlackthorn2), 0x2782, 0, 0, 50));

            // NightEyesBase
            Rewards.Add(new CollectionItem(typeof(BandanaBearingTheCrestOfBlackthorn1), 0x1540, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(BascinetBearingTheCrestOfBlackthorn1), 0x140C, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(CircletBearingTheCrestOfBlackthorn1), 0x2B6E, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(FeatheredHatBearingTheCrestOfBlackthorn1), 0x171A, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(GargishEarringsBearingTheCrestOfBlackthorn1), 0x4213, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(GargishGlassesBearingTheCrestOfBlackthorn1), 0x4644, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(JesterHatBearingTheCrestOfBlackthorn1), 0x171C, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(NorseHelmBearingTheCrestOfBlackthorn1), 0x140E, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(PlateHelmBearingTheCrestOfBlackthorn1), 0x1412, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(RoyalCircletBearingTheCrestOfBlackthorn1), 0x2B6F, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(SkullcapBearingTheCrestOfBlackthorn1), 0x1544, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(TricorneHatBearingTheCrestOfBlackthorn1), 0x171B, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(WizardsHatBearingTheCrestOfBlackthorn1), 0x1718, 0, 0, 50));

            // MaceAndShieldBase
            Rewards.Add(new CollectionItem(typeof(BandanaBearingTheCrestOfBlackthorn2), 0x1540, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(BascinetBearingTheCrestOfBlackthorn2), 0x140C, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(CircletBearingTheCrestOfBlackthorn2), 0x2B6E, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(FeatheredHatBearingTheCrestOfBlackthorn2), 0x171A, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(GargishEarringsBearingTheCrestOfBlackthorn2), 0x4213, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(GargishGlassesBearingTheCrestOfBlackthorn2), 0x4644, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(JesterHatBearingTheCrestOfBlackthorn2), 0x171C, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(NorseHelmBearingTheCrestOfBlackthorn2), 0x140E, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(PlateHelmBearingTheCrestOfBlackthorn2), 0x1412, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(RoyalCircletBearingTheCrestOfBlackthorn2), 0x2B6F, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(SkullcapBearingTheCrestOfBlackthorn2), 0x1544, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(TricorneHatBearingTheCrestOfBlackthorn2), 0x171B, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(WizardsHatBearingTheCrestOfBlackthorn2), 0x1718, 0, 0, 50));

            // FoldedSteelBase
            Rewards.Add(new CollectionItem(typeof(BandanaBearingTheCrestOfBlackthorn3), 0x1540, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(BascinetBearingTheCrestOfBlackthorn3), 0x140C, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(CircletBearingTheCrestOfBlackthorn3), 0x2B6E, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(FeatheredHatBearingTheCrestOfBlackthorn3), 0x171A, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(GargishEarringsBearingTheCrestOfBlackthorn3), 0x4213, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(GargishGlassesBearingTheCrestOfBlackthorn3), 0x4644, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(JesterHatBearingTheCrestOfBlackthorn3), 0x171C, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(NorseHelmBearingTheCrestOfBlackthorn3), 0x140E, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(PlateHelmBearingTheCrestOfBlackthorn3), 0x1412, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(RoyalCircletBearingTheCrestOfBlackthorn3), 0x2B6F, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(SkullcapBearingTheCrestOfBlackthorn3), 0x1544, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(TricorneHatBearingTheCrestOfBlackthorn3), 0x171B, 0, 0, 50));
            Rewards.Add(new CollectionItem(typeof(WizardsHatBearingTheCrestOfBlackthorn3), 0x1718, 0, 0, 50));

            // Cloak of Silence Base
            Rewards.Add(new CollectionItem(typeof(HoodedRobeBearingTheCrestOfBlackthorn3), 0x2683, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(GargishFancyBearingTheCrestOfBlackthorn3), 0x4002, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(GargishEpauletteBearingTheCrestOfBlackthorn3), 0x9986, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(EpauletteBearingTheCrestOfBlackthorn3), 0x9985, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(GargishRobeBearingTheCrestOfBlackthorn3), 0x4000, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(PlainDressBearingTheCrestOfBlackthorn3), 0x1F01, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(GildedDressBearingTheCrestOfBlackthorn3), 0x230E, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(FancyDressBearingTheCrestOfBlackthorn3), 0x1F00, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(FemaleKimonoBearingTheCrestOfBlackthorn3), 0x2783, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(MaleKimonoBearingTheCrestOfBlackthorn3), 0x2782, 0, 0, 75));

            // Cloak of Power Base
            Rewards.Add(new CollectionItem(typeof(HoodedRobeBearingTheCrestOfBlackthorn4), 0x2683, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(GargishFancyBearingTheCrestOfBlackthorn4), 0x4002, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(GargishEpauletteBearingTheCrestOfBlackthorn4), 0x9986, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(EpauletteBearingTheCrestOfBlackthorn4), 0x9985, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(GargishRobeBearingTheCrestOfBlackthorn4), 0x4000, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(PlainDressBearingTheCrestOfBlackthorn4), 0x1F01, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(GildedDressBearingTheCrestOfBlackthorn4), 0x230E, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(FancyDressBearingTheCrestOfBlackthorn4), 0x1F00, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(FemaleKimonoBearingTheCrestOfBlackthorn4), 0x2783, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(MaleKimonoBearingTheCrestOfBlackthorn4), 0x2782, 0, 0, 75));

            // Cloak of Life Base
            Rewards.Add(new CollectionItem(typeof(HoodedRobeBearingTheCrestOfBlackthorn5), 0x2683, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(GargishFancyBearingTheCrestOfBlackthorn5), 0x4002, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(GargishEpauletteBearingTheCrestOfBlackthorn5), 0x9986, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(EpauletteBearingTheCrestOfBlackthorn5), 0x9985, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(GargishRobeBearingTheCrestOfBlackthorn5), 0x4000, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(PlainDressBearingTheCrestOfBlackthorn5), 0x1F01, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(GildedDressBearingTheCrestOfBlackthorn5), 0x230E, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(FancyDressBearingTheCrestOfBlackthorn5), 0x1F00, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(FemaleKimonoBearingTheCrestOfBlackthorn5), 0x2783, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(MaleKimonoBearingTheCrestOfBlackthorn5), 0x2782, 0, 0, 75));

            // Cloak of Death Base
            Rewards.Add(new CollectionItem(typeof(HoodedRobeBearingTheCrestOfBlackthorn6), 0x2683, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(GargishFancyBearingTheCrestOfBlackthorn6), 0x4002, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(GargishEpauletteBearingTheCrestOfBlackthorn6), 0x9986, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(EpauletteBearingTheCrestOfBlackthorn6), 0x9985, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(GargishRobeBearingTheCrestOfBlackthorn6), 0x4000, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(PlainDressBearingTheCrestOfBlackthorn6), 0x1F01, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(GildedDressBearingTheCrestOfBlackthorn6), 0x230E, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(FancyDressBearingTheCrestOfBlackthorn6), 0x1F00, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(FemaleKimonoBearingTheCrestOfBlackthorn6), 0x2783, 0, 0, 75));
            Rewards.Add(new CollectionItem(typeof(MaleKimonoBearingTheCrestOfBlackthorn6), 0x2782, 0, 0, 75));


            // Conjurer's Garb Base
            Rewards.Add(new CollectionItem(typeof(HoodedRobeBearingTheCrestOfBlackthorn7), 0x2683, 0, 0, 100));
            Rewards.Add(new CollectionItem(typeof(GargishFancyBearingTheCrestOfBlackthorn7), 0x4002, 0, 0, 100));
            Rewards.Add(new CollectionItem(typeof(GargishEpauletteBearingTheCrestOfBlackthorn7), 0x9986, 0, 0, 100));
            Rewards.Add(new CollectionItem(typeof(EpauletteBearingTheCrestOfBlackthorn7), 0x9985, 0, 0, 100));
            Rewards.Add(new CollectionItem(typeof(GargishRobeBearingTheCrestOfBlackthorn7), 0x4000, 0, 0, 100));
            Rewards.Add(new CollectionItem(typeof(PlainDressBearingTheCrestOfBlackthorn7), 0x1F01, 0, 0, 100));
            Rewards.Add(new CollectionItem(typeof(GildedDressBearingTheCrestOfBlackthorn7), 0x230E, 0, 0, 100));
            Rewards.Add(new CollectionItem(typeof(FancyDressBearingTheCrestOfBlackthorn7), 0x1F00, 0, 0, 100));
            Rewards.Add(new CollectionItem(typeof(FemaleKimonoBearingTheCrestOfBlackthorn7), 0x2783, 0, 0, 100));
            Rewards.Add(new CollectionItem(typeof(MaleKimonoBearingTheCrestOfBlackthorn7), 0x2782, 0, 0, 100));

            // RoyalBritannianBase
            Rewards.Add(new CollectionItem(typeof(DoubletBearingTheCrestOfBlackthorn), 0x1F7B, 0, 0, 100));
            Rewards.Add(new CollectionItem(typeof(GargishSashBearingTheCrestOfBlackthorn), 0x46B4, 0, 0, 100));
            Rewards.Add(new CollectionItem(typeof(SurcoatBearingTheCrestOfBlackthorn), 0x1FFD, 0, 0, 100));
            Rewards.Add(new CollectionItem(typeof(TunicBearingTheCrestOfBlackthorn), 0x1FA1, 0, 0, 100));

            // TangleBase
            Rewards.Add(new CollectionItem(typeof(GargishHalfApronBearingTheCrestOfBlackthorn1), 0x50D8, 0, 0, 100));
            Rewards.Add(new CollectionItem(typeof(LeatherNinjaBeltBearingTheCrestOfBlackthorn1), 0x2790, 0, 0, 100));
            Rewards.Add(new CollectionItem(typeof(ObiBearingTheCrestOfBlackthorn1), 0x27A0, 0, 0, 100));
            Rewards.Add(new CollectionItem(typeof(WoodlandBeltBearingTheCrestOfBlackthorn1), 0x2B68, 0, 0, 100));

            // CrimsonCinctureBase
            Rewards.Add(new CollectionItem(typeof(GargishHalfApronBearingTheCrestOfBlackthorn2), 0x50D8, 0, 0, 100));
            Rewards.Add(new CollectionItem(typeof(LeatherNinjaBeltBearingTheCrestOfBlackthorn2), 0x2790, 0, 0, 100));
            Rewards.Add(new CollectionItem(typeof(ObiBearingTheCrestOfBlackthorn2), 0x27A0, 0, 0, 100));
            Rewards.Add(new CollectionItem(typeof(WoodlandBeltBearingTheCrestOfBlackthorn2), 0x2B68, 0, 0, 100));
        }

        public static bool IsTokunoDyable(Type t)
        {
            return Rewards.FirstOrDefault(item => item.Type == t) != null;
        }
    }
}