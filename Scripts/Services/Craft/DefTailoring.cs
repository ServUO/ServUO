using Server.Items;
using System;
using System.Collections.Generic;

namespace Server.Engines.Craft
{
    public enum TailorRecipe
    {
        ElvenQuiver = 501,
        QuiverOfFire = 502,
        QuiverOfIce = 503,
        QuiverOfBlight = 504,
        QuiverOfLightning = 505,

        SongWovenMantle = 550,
        SpellWovenBritches = 551,
        StitchersMittens = 552,

        JesterShoes = 560,
        ChefsToque = 561,
        GuildedKilt = 562,
        CheckeredKilt = 563,
        FancyKilt = 564,
        FloweredDress = 565,
        EveningGown = 566,

        TigerPeltChest = 570,
        TigerPeltCollar = 571,
        TigerPeltHelm = 572,
        TigerPeltLegs = 573,
        TigerPeltShorts = 574,
        TigerPeltBustier = 575,
        TigerPeltLongSkirt = 576,
        TigerPeltSkirt = 577,

        DragonTurtleHideArms = 580,
        DragonTurtleHideChest = 581,
        DragonTurtleHideHelm = 582,
        DragonTurtleHideLegs = 583,
        DragonTurtleHideBustier = 584,

        // doom
        CuffsOfTheArchmage = 585,

        KrampusMinionHat = 586,
        KrampusMinionBoots = 587,
        KrampusMinionTalons = 588,

        MaceBelt = 1100,
        SwordBelt = 1101,
        DaggerBelt = 1102,
        ElegantCollar = 1103,
        CrimsonMaceBelt = 1104,
        CrimsonSwordBelt = 1105,
        CrimsonDaggerBelt = 1106,
        ElegantCollarOfFortune = 1107,
        AssassinsCowl = 1108,
        MagesHood = 1109,
        CowlOfTheMaceAndShield = 1110,
        MagesHoodOfScholarlyInsight = 1111,

    }

    public class DefTailoring : CraftSystem
    {
        #region Statics
        private static readonly Type[] m_TailorColorables = new Type[]
   		{
            typeof(GozaMatEastDeed), typeof(GozaMatSouthDeed),
            typeof(SquareGozaMatEastDeed), typeof(SquareGozaMatSouthDeed),
            typeof(BrocadeGozaMatEastDeed), typeof(BrocadeGozaMatSouthDeed),
            typeof(BrocadeSquareGozaMatEastDeed), typeof(BrocadeSquareGozaMatSouthDeed),
            typeof(SquareGozaMatDeed)
   		};

        private static readonly Type[] m_TailorClothNonColorables = new Type[]
        {
            typeof(DeerMask), typeof(BearMask), typeof(OrcMask), typeof(TribalMask), typeof(HornedTribalMask), typeof(CuffsOfTheArchmage)
        };

        // singleton instance
        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefTailoring();

                return m_CraftSystem;
            }
        }
        #endregion

        #region Constructor
        private DefTailoring()
            : base(1, 1, 1.25)// base( 1, 1, 4.5 )
        {
        }

        #endregion

        #region Overrides
        public override SkillName MainSkill => SkillName.Tailoring;

        public override int GumpTitleNumber => 1044005;

        public override CraftECA ECA => CraftECA.ChanceMinusSixtyToFourtyFive;

        public override double GetChanceAtMin(CraftItem item)
        {
            if (item.NameNumber == 1157348 || item.NameNumber == 1159225 || item.NameNumber == 1159213 || item.NameNumber == 1159212 ||
                item.NameNumber == 1159211 || item.NameNumber == 1159228 || item.NameNumber == 1159229)
                return 0.05; // 5%

            return 0.5; // 50%
        }

        public override int CanCraft(Mobile from, ITool tool, Type itemType)
        {
            int num = 0;

            if (tool == null || tool.Deleted || tool.UsesRemaining <= 0)
                return 1044038; // You have worn out your tool!
            else if (!tool.CheckAccessible(from, ref num))
                return num; // The tool must be on your person to use.

            return 0;
        }

        public override bool RetainsColorFrom(CraftItem item, Type type)
        {
            if (type != typeof(Cloth) && type != typeof(UncutCloth) && type != typeof(AbyssalCloth))
                return false;

            type = item.ItemType;

            bool contains = false;

            for (int i = 0; !contains && i < m_TailorColorables.Length; ++i)
                contains = (m_TailorColorables[i] == type);

            return contains;
        }

        public override bool RetainsColorFromException(CraftItem item, Type type)
        {
            if (item == null || type == null)
                return false;

            if (type != typeof(Cloth) && type != typeof(UncutCloth) && type != typeof(AbyssalCloth))
                return false;

            bool contains = false;

            for (int i = 0; !contains && i < m_TailorClothNonColorables.Length; ++i)
                contains = (m_TailorClothNonColorables[i] == item.ItemType);

            return contains;
        }

        public override void PlayCraftEffect(Mobile from)
        {
            from.PlaySound(0x248);
        }

        public override int PlayEndingEffect(Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item)
        {
            if (toolBroken)
                from.SendLocalizedMessage(1044038); // You have worn out your tool

            if (failed)
            {
                if (lostMaterial)
                    return 1044043; // You failed to create the item, and some of your materials are lost.
                else
                    return 1044157; // You failed to create the item, but no materials were lost.
            }
            else
            {
                if (quality == 0)
                    return 502785; // You were barely able to make this item.  It's quality is below average.
                else if (makersMark && quality == 2)
                    return 1044156; // You create an exceptional quality item and affix your maker's mark.
                else if (quality == 2)
                    return 1044155; // You create an exceptional quality item.
                else
                    return 1044154; // You create the item.
            }
        }

        public override void InitCraftList()
        {
            int index = -1;

            #region Materials
            index = AddCraft(typeof(CutUpCloth), 1044457, 1044458, 0.0, 0.0, typeof(BoltOfCloth), 1044453, 1, 1044253);
            AddCraftAction(index, CutUpCloth);

            index = AddCraft(typeof(CombineCloth), 1044457, 1044459, 0.0, 0.0, typeof(Cloth), 1044455, 1, 1044253);
            AddCraftAction(index, CombineCloth);

            index = AddCraft(typeof(PowderCharge), 1044457, 1116160, 0.0, 50.0, typeof(Cloth), 1044455, 1, 1044253);
            AddRes(index, typeof(BlackPowder), 1095826, 4, 1044253);
            SetUseAllRes(index, true);

            index = AddCraft(typeof(AbyssalCloth), 1044457, 1113350, 110.0, 160.0, typeof(Cloth), 1044455, 50, 1044253);
            AddRes(index, typeof(CrystallineBlackrock), 1077568, 1, 1044253);
            SetItemHue(index, 2075);
            #endregion

            #region Hats
            AddCraft(typeof(SkullCap), 1011375, 1025444, 0.0, 25.0, typeof(Cloth), 1044455, 2, 1044287);
            AddCraft(typeof(Bandana), 1011375, 1025440, 0.0, 25.0, typeof(Cloth), 1044455, 2, 1044287);
            AddCraft(typeof(FloppyHat), 1011375, 1025907, 6.2, 31.2, typeof(Cloth), 1044455, 11, 1044287);
            AddCraft(typeof(Cap), 1011375, 1025909, 6.2, 31.2, typeof(Cloth), 1044455, 11, 1044287);
            AddCraft(typeof(WideBrimHat), 1011375, 1025908, 6.2, 31.2, typeof(Cloth), 1044455, 12, 1044287);
            AddCraft(typeof(StrawHat), 1011375, 1025911, 6.2, 31.2, typeof(Cloth), 1044455, 10, 1044287);
            AddCraft(typeof(TallStrawHat), 1011375, 1025910, 6.7, 31.7, typeof(Cloth), 1044455, 13, 1044287);
            AddCraft(typeof(WizardsHat), 1011375, 1025912, 7.2, 32.2, typeof(Cloth), 1044455, 15, 1044287);
            AddCraft(typeof(Bonnet), 1011375, 1025913, 6.2, 31.2, typeof(Cloth), 1044455, 11, 1044287);
            AddCraft(typeof(FeatheredHat), 1011375, 1025914, 6.2, 31.2, typeof(Cloth), 1044455, 12, 1044287);
            AddCraft(typeof(TricorneHat), 1011375, 1025915, 6.2, 31.2, typeof(Cloth), 1044455, 12, 1044287);
            AddCraft(typeof(JesterHat), 1011375, 1025916, 7.2, 32.2, typeof(Cloth), 1044455, 15, 1044287);

            AddCraft(typeof(FlowerGarland), 1011375, 1028965, 10.0, 35.0, typeof(Cloth), 1044455, 5, 1044287);

            AddCraft(typeof(ClothNinjaHood), 1011375, 1030202, 80.0, 105.0, typeof(Cloth), 1044455, 13, 1044287);

            AddCraft(typeof(Kasa), 1011375, 1030211, 60.0, 85.0, typeof(Cloth), 1044455, 12, 1044287);

            AddCraft(typeof(OrcMask), 1011375, 1025147, 75.0, 100.0, typeof(Cloth), 1044455, 12, 1044287);
            AddCraft(typeof(BearMask), 1011375, 1025445, 77.5, 102.5, typeof(Cloth), 1044455, 15, 1044287);
            AddCraft(typeof(DeerMask), 1011375, 1025447, 77.5, 102.5, typeof(Cloth), 1044455, 15, 1044287);
            AddCraft(typeof(TribalMask), 1011375, 1025449, 82.5, 107.5, typeof(Cloth), 1044455, 12, 1044287);
            AddCraft(typeof(HornedTribalMask), 1011375, 1025451, 82.5, 107.5, typeof(Cloth), 1044455, 12, 1044287);

            index = AddCraft(typeof(ChefsToque), 1011375, 1109618, 6.2, 21.2, typeof(Cloth), 1044455, 11, 1044287);
            AddRecipe(index, (int)TailorRecipe.ChefsToque);

            index = AddCraft(typeof(KrampusMinionHat), 1011375, 1125639, 100.0, 500.0, typeof(Cloth), 1044455, 8, 1044287);
            AddRecipe(index, (int)TailorRecipe.KrampusMinionHat);

            index = AddCraft(typeof(AssassinsCowl), 1011375, 1126024, 90.0, 110.0, typeof(Cloth), 1044455, 5, 1044287);
            AddRes(index, typeof(Leather), 1044462, 5, 1044463);
            AddRes(index, typeof(VileTentacles), 1113333, 5, 1044253);
            AddRecipe(index, (int)TailorRecipe.AssassinsCowl);

            index = AddCraft(typeof(MagesHood), 1011375, 1159227, 90.0, 110.0, typeof(Cloth), 1044455, 5, 1044287);
            AddRes(index, typeof(Leather), 1044462, 5, 1044463);
            AddRes(index, typeof(VoidCore), 1113334, 5, 1044253);
            AddRecipe(index, (int)TailorRecipe.MagesHood);

            index = AddCraft(typeof(CowlOfTheMaceAndShield), 1011375, 1159228, 120.0, 215.0, typeof(Cloth), 1044455, 5, 1044287);
            AddRes(index, typeof(Leather), 1044462, 5, 1044463);
            AddRes(index, typeof(MaceAndShieldGlasses), 1073381, 1, 1044253);
            AddRes(index, typeof(VileTentacles), 1113333, 10, 1044253);
            AddRecipe(index, (int)TailorRecipe.CowlOfTheMaceAndShield);
            ForceExceptional(index);

            index = AddCraft(typeof(MagesHoodOfScholarlyInsight), 1011375, 1159229, 120.0, 215.0, typeof(Cloth), 1044455, 5, 1044287);
            AddRes(index, typeof(Leather), 1044462, 5, 1044463);
            AddRes(index, typeof(TheScholarsHalo), 1157354, 1, 1044253);
            AddRes(index, typeof(VoidCore), 1113334, 10, 1044253);
            AddRecipe(index, (int)TailorRecipe.MagesHoodOfScholarlyInsight);
            ForceExceptional(index);

            #endregion

            #region Shirts/Pants
            AddCraft(typeof(Doublet), 1111747, 1028059, 0, 25.0, typeof(Cloth), 1044455, 8, 1044287);
            AddCraft(typeof(Shirt), 1111747, 1025399, 20.7, 45.7, typeof(Cloth), 1044455, 8, 1044287);
            AddCraft(typeof(FancyShirt), 1111747, 1027933, 24.8, 49.8, typeof(Cloth), 1044455, 8, 1044287);
            AddCraft(typeof(Tunic), 1111747, 1028097, 00.0, 25.0, typeof(Cloth), 1044455, 12, 1044287);
            AddCraft(typeof(Surcoat), 1111747, 1028189, 8.2, 33.2, typeof(Cloth), 1044455, 14, 1044287);
            AddCraft(typeof(PlainDress), 1111747, 1027937, 12.4, 37.4, typeof(Cloth), 1044455, 10, 1044287);
            AddCraft(typeof(FancyDress), 1111747, 1027935, 33.1, 58.1, typeof(Cloth), 1044455, 12, 1044287);
            AddCraft(typeof(Cloak), 1111747, 1025397, 41.4, 66.4, typeof(Cloth), 1044455, 14, 1044287);
            AddCraft(typeof(Robe), 1111747, 1027939, 53.9, 78.9, typeof(Cloth), 1044455, 16, 1044287);
            AddCraft(typeof(JesterSuit), 1111747, 1028095, 8.2, 33.2, typeof(Cloth), 1044455, 24, 1044287);

            AddCraft(typeof(FurCape), 1111747, 1028969, 35.0, 60.0, typeof(Cloth), 1044455, 13, 1044287);
            AddCraft(typeof(GildedDress), 1111747, 1028973, 37.5, 62.5, typeof(Cloth), 1044455, 16, 1044287);
            AddCraft(typeof(FormalShirt), 1111747, 1028975, 26.0, 51.0, typeof(Cloth), 1044455, 16, 1044287);

            index = AddCraft(typeof(ClothNinjaJacket), 1111747, 1030207, 75.0, 100.0, typeof(Cloth), 1044455, 12, 1044287);

            index = AddCraft(typeof(Kamishimo), 1111747, 1030212, 75.0, 100.0, typeof(Cloth), 1044455, 15, 1044287);

            index = AddCraft(typeof(HakamaShita), 1111747, 1030215, 40.0, 65.0, typeof(Cloth), 1044455, 14, 1044287);

            index = AddCraft(typeof(MaleKimono), 1111747, 1030189, 50.0, 75.0, typeof(Cloth), 1044455, 16, 1044287);

            index = AddCraft(typeof(FemaleKimono), 1111747, 1030190, 50.0, 75.0, typeof(Cloth), 1044455, 16, 1044287);

            index = AddCraft(typeof(JinBaori), 1111747, 1030220, 30.0, 55.0, typeof(Cloth), 1044455, 12, 1044287);

            AddCraft(typeof(ShortPants), 1111747, 1025422, 24.8, 49.8, typeof(Cloth), 1044455, 6, 1044287);
            AddCraft(typeof(LongPants), 1111747, 1025433, 24.8, 49.8, typeof(Cloth), 1044455, 8, 1044287);
            AddCraft(typeof(Kilt), 1111747, 1025431, 20.7, 45.7, typeof(Cloth), 1044455, 8, 1044287);
            AddCraft(typeof(Skirt), 1111747, 1025398, 29.0, 54.0, typeof(Cloth), 1044455, 10, 1044287);

            AddCraft(typeof(FurSarong), 1111747, 1028971, 35.0, 60.0, typeof(Cloth), 1044455, 12, 1044287);

            index = AddCraft(typeof(Hakama), 1111747, 1030213, 50.0, 75.0, typeof(Cloth), 1044455, 16, 1044287);

            index = AddCraft(typeof(TattsukeHakama), 1111747, 1030214, 50.0, 75.0, typeof(Cloth), 1044455, 16, 1044287);

            index = AddCraft(typeof(ElvenShirt), 1111747, 1032661, 80.0, 105.0, typeof(Cloth), 1044455, 10, 1044287);

            index = AddCraft(typeof(ElvenDarkShirt), 1111747, 1032662, 80.0, 105.0, typeof(Cloth), 1044455, 10, 1044287);

            index = AddCraft(typeof(ElvenPants), 1111747, 1032665, 80.0, 105.0, typeof(Cloth), 1044455, 12, 1044287);

            index = AddCraft(typeof(MaleElvenRobe), 1111747, 1032659, 80.0, 105.0, typeof(Cloth), 1044455, 30, 1044287);

            index = AddCraft(typeof(FemaleElvenRobe), 1111747, 1032660, 80.0, 105.0, typeof(Cloth), 1044455, 30, 1044287);

            index = AddCraft(typeof(WoodlandBelt), 1111747, 1032639, 80.0, 105.0, typeof(Cloth), 1044455, 10, 1044287);

            index = AddCraft(typeof(GargishRobe), 1111747, 1095256, 53.9, 78.9, typeof(Cloth), 1044455, 16, 1044287);

            index = AddCraft(typeof(GargishFancyRobe), 1111747, 1095258, 53.9, 78.9, typeof(Cloth), 1044455, 16, 1044287);

            index = AddCraft(typeof(RobeofRite), 1111747, 1153510, 101.5, 120.0, typeof(Leather), 1044462, 6, 1044253);
            AddRes(index, typeof(FireRuby), 1032695, 1, 1044253);
            AddRes(index, typeof(GoldDust), 1098337, 5, 1044253);
            AddRes(index, typeof(AbyssalCloth), 1113350, 6, 1044253);
            ForceNonExceptional(index);

            index = AddCraft(typeof(GuildedKilt), 1111747, 1109619, 82.8, 97.8, typeof(Cloth), 1044455, 8, 1044287);
            AddRecipe(index, (int)TailorRecipe.GuildedKilt);

            index = AddCraft(typeof(CheckeredKilt), 1111747, 1109620, 41.4, 56.4, typeof(Cloth), 1044455, 8, 1044287);
            AddRecipe(index, (int)TailorRecipe.CheckeredKilt);

            index = AddCraft(typeof(FancyKilt), 1111747, 1109621, 20.7, 25.7, typeof(Cloth), 1044455, 8, 1044287);
            AddRecipe(index, (int)TailorRecipe.FancyKilt);

            index = AddCraft(typeof(FloweredDress), 1111747, 1109622, 75.0, 90.0, typeof(Cloth), 1044455, 18, 1044287);
            AddRecipe(index, (int)TailorRecipe.FloweredDress);

            index = AddCraft(typeof(EveningGown), 1111747, 1109625, 75, 90.0, typeof(Cloth), 1044455, 18, 1044287);
            AddRecipe(index, (int)TailorRecipe.EveningGown);

            #endregion

            #region Misc
            AddCraft(typeof(BodySash), 1015283, 1025441, 4.1, 29.1, typeof(Cloth), 1044455, 4, 1044287);
            AddCraft(typeof(HalfApron), 1015283, 1025435, 20.7, 45.7, typeof(Cloth), 1044455, 6, 1044287);
            AddCraft(typeof(FullApron), 1015283, 1025437, 29.0, 54.0, typeof(Cloth), 1044455, 10, 1044287);

            AddCraft(typeof(Obi), 1015283, 1030219, 20.0, 45.0, typeof(Cloth), 1044455, 6, 1044287);

            index = AddCraft(typeof(ElvenQuiver), 1015283, 1032657, 65.0, 115.0, typeof(Leather), 1044462, 28, 1044463);
            AddRecipe(index, (int)TailorRecipe.ElvenQuiver);

            index = AddCraft(typeof(QuiverOfFire), 1015283, 1073109, 65.0, 115.0, typeof(Leather), 1044462, 28, 1044463);
            AddRes(index, typeof(FireRuby), 1032695, 15, 1042081);
            AddRecipe(index, (int)TailorRecipe.QuiverOfFire);

            index = AddCraft(typeof(QuiverOfIce), 1015283, 1073110, 65.0, 115.0, typeof(Leather), 1044462, 28, 1044463);
            AddRes(index, typeof(WhitePearl), 1032694, 15, 1042081);
            AddRecipe(index, (int)TailorRecipe.QuiverOfIce);

            index = AddCraft(typeof(QuiverOfBlight), 1015283, 1073111, 65.0, 115.0, typeof(Leather), 1044462, 28, 1044463);
            AddRes(index, typeof(Blight), 1032675, 10, 1042081);
            AddRecipe(index, (int)TailorRecipe.QuiverOfBlight);

            index = AddCraft(typeof(QuiverOfLightning), 1015283, 1073112, 65.0, 115.0, typeof(Leather), 1044462, 28, 1044463);
            AddRes(index, typeof(Corruption), 1032676, 10, 1042081);
            AddRecipe(index, (int)TailorRecipe.QuiverOfLightning);

            index = AddCraft(typeof(LeatherContainerEngraver), 1015283, 1072152, 75.0, 100.0, typeof(Bone), 1049064, 1, 1049063);
            AddRes(index, typeof(Leather), 1044462, 6, 1044463);
            AddRes(index, typeof(SpoolOfThread), 1073462, 2, 1073463);
            AddRes(index, typeof(Dyes), 1024009, 1, 1044253);

            AddCraft(typeof(GargoyleHalfApron), 1015283, 1099568, 20.7, 45.7, typeof(Cloth), 1044455, 6, 1044287);
            AddCraft(typeof(GargishSash), 1015283, 1115388, 4.1, 29.1, typeof(Cloth), 1044455, 4, 1044287);

            AddCraft(typeof(OilCloth), 1015283, 1041498, 74.6, 99.6, typeof(Cloth), 1044455, 1, 1044287);
            AddCraft(typeof(GozaMatEastDeed), 1015283, 1030404, 55.0, 80.0, typeof(Cloth), 1044455, 25, 1044287);
            AddCraft(typeof(GozaMatSouthDeed), 1015283, 1030405, 55.0, 80.0, typeof(Cloth), 1044455, 25, 1044287);
            AddCraft(typeof(SquareGozaMatEastDeed), 1015283, 1030407, 55.0, 80.0, typeof(Cloth), 1044455, 25, 1044287);
            AddCraft(typeof(SquareGozaMatSouthDeed), 1015283, 1030406, 55.0, 80.0, typeof(Cloth), 1044455, 25, 1044287);
            AddCraft(typeof(BrocadeGozaMatEastDeed), 1015283, 1030408, 55.0, 80.0, typeof(Cloth), 1044455, 25, 1044287);
            AddCraft(typeof(BrocadeGozaMatSouthDeed), 1015283, 1030409, 55.0, 80.0, typeof(Cloth), 1044455, 25, 1044287);
            AddCraft(typeof(BrocadeSquareGozaMatEastDeed), 1015283, 1030411, 55.0, 80.0, typeof(Cloth), 1044455, 25, 1044287);
            AddCraft(typeof(BrocadeSquareGozaMatSouthDeed), 1015283, 1030410, 55.0, 80.0, typeof(Cloth), 1044455, 25, 1044287);
            AddCraft(typeof(SquareGozaMatDeed), 1015283, 1113621, 55.0, 80.0, typeof(Cloth), 1044455, 25, 1044287);

            index = AddCraft(typeof(MaceBelt), 1015283, 1126020, 90.0, 110.0, typeof(Cloth), 1044455, 5, 1044287);
            AddRes(index, typeof(Leather), 1044462, 5, 1044463);
            AddRes(index, typeof(Lodestone), 1113332, 5, 1044253);
            AddRecipe(index, (int)TailorRecipe.MaceBelt);

            index = AddCraft(typeof(SwordBelt), 1015283, 1126021, 90.0, 110.0, typeof(Cloth), 1044455, 5, 1044287);
            AddRes(index, typeof(Leather), 1044462, 5, 1044463);
            AddRes(index, typeof(Lodestone), 1113332, 5, 1044253);
            AddRecipe(index, (int)TailorRecipe.SwordBelt);

            index = AddCraft(typeof(DaggerBelt), 1015283, 1159210, 90.0, 110.0, typeof(Cloth), 1044455, 5, 1044287);
            AddRes(index, typeof(Leather), 1044462, 5, 1044463);
            AddRes(index, typeof(Lodestone), 1113332, 5, 1044253);
            AddRecipe(index, (int)TailorRecipe.DaggerBelt);

            index = AddCraft(typeof(ElegantCollar), 1015283, 1159224, 90.0, 110.0, typeof(Cloth), 1044455, 5, 1044287);
            AddRes(index, typeof(Leather), 1044462, 5, 1044463);
            AddRes(index, typeof(FeyWings), 1113332, 5, 1044253);
            AddRecipe(index, (int)TailorRecipe.ElegantCollar);

            index = AddCraft(typeof(CrimsonMaceBelt), 1015283, 1159211, 120.0, 215.0, typeof(Cloth), 1044455, 5, 1044287);
            AddRes(index, typeof(Leather), 1044462, 5, 1044463);
            AddRes(index, typeof(CrimsonCincture), 1075043, 1, 1044253);
            AddRes(index, typeof(Lodestone), 1113348, 10, 1044253);
            AddRecipe(index, (int)TailorRecipe.CrimsonMaceBelt);
            ForceExceptional(index);

            index = AddCraft(typeof(CrimsonSwordBelt), 1015283, 1159212, 120.0, 215.0, typeof(Cloth), 1044455, 5, 1044287);
            AddRes(index, typeof(Leather), 1044462, 5, 1044463);
            AddRes(index, typeof(CrimsonCincture), 1075043, 1, 1044253);
            AddRes(index, typeof(Lodestone), 1113348, 10, 1044253);
            AddRecipe(index, (int)TailorRecipe.CrimsonSwordBelt);
            ForceExceptional(index);

            index = AddCraft(typeof(CrimsonDaggerBelt), 1015283, 1159213, 120.0, 215.0, typeof(Cloth), 1044455, 5, 1044287);
            AddRes(index, typeof(Leather), 1044462, 5, 1044463);
            AddRes(index, typeof(CrimsonCincture), 1075043, 1, 1044253);
            AddRes(index, typeof(Lodestone), 1113348, 10, 1044253);
            AddRecipe(index, (int)TailorRecipe.CrimsonDaggerBelt);
            ForceExceptional(index);

            index = AddCraft(typeof(ElegantCollarOfFortune), 1015283, 1159225, 120.0, 215.0, typeof(Cloth), 1044455, 5, 1044287);
            AddRes(index, typeof(Leather), 1044462, 5, 1044463);
            AddRes(index, typeof(LeurociansMempoOfFortune), 1071460, 1, 1044253);
            AddRes(index, typeof(FeyWings), 1113332, 10, 1044253);
            AddRecipe(index, (int)TailorRecipe.ElegantCollarOfFortune);
            ForceExceptional(index);

            #endregion

            #region Footwear

            index = AddCraft(typeof(ElvenBoots), 1015288, 1072902, 80.0, 105.0, typeof(Leather), 1044462, 15, 1044463);

            AddCraft(typeof(FurBoots), 1015288, 1028967, 50.0, 75.0, typeof(Cloth), 1044455, 12, 1044287);

            AddCraft(typeof(NinjaTabi), 1015288, 1030210, 70.0, 95.0, typeof(Cloth), 1044455, 10, 1044287);

            AddCraft(typeof(SamuraiTabi), 1015288, 1030209, 20.0, 45.0, typeof(Cloth), 1044455, 6, 1044287);

            AddCraft(typeof(Sandals), 1015288, 1025901, 12.4, 37.4, typeof(Leather), 1044462, 4, 1044463);
            AddCraft(typeof(Shoes), 1015288, 1025904, 16.5, 41.5, typeof(Leather), 1044462, 6, 1044463);
            AddCraft(typeof(Boots), 1015288, 1025899, 33.1, 58.1, typeof(Leather), 1044462, 8, 1044463);
            AddCraft(typeof(ThighBoots), 1015288, 1025906, 41.4, 66.4, typeof(Leather), 1044462, 10, 1044463);

            AddCraft(typeof(LeatherTalons), 1015288, 1095728, 40.4, 65.4, typeof(Leather), 1044462, 6, 1044453);

            index = AddCraft(typeof(JesterShoes), 1015288, 1109617, 20.0, 35.0, typeof(Cloth), 1044455, 6, 1044287);
            AddRecipe(index, (int)TailorRecipe.JesterShoes);

            index = AddCraft(typeof(KrampusMinionBoots), 1015288, 1125637, 100.0, 500.0, typeof(Leather), 1044462, 6, 1044463);
            AddRes(index, typeof(Cloth), 1044455, 4, 1044287);
            AddRecipe(index, (int)TailorRecipe.KrampusMinionBoots);

            index = AddCraft(typeof(KrampusMinionTalons), 1015288, 1125644, 100.0, 500.0, typeof(Leather), 1044462, 6, 1044463);
            AddRes(index, typeof(Cloth), 1044455, 4, 1044287);
            AddRecipe(index, (int)TailorRecipe.KrampusMinionTalons);

            #endregion

            #region Leather Armor

            index = AddCraft(typeof(SpellWovenBritches), 1015293, 1072929, 92.5, 117.5, typeof(Leather), 1044462, 15, 1044463);
            AddRes(index, typeof(EyeOfTheTravesty), 1032685, 1, 1044253);
            AddRes(index, typeof(Putrefaction), 1032678, 10, 1044253);
            AddRes(index, typeof(Scourge), 1032677, 10, 1044253);
            AddRecipe(index, (int)TailorRecipe.SpellWovenBritches);
            ForceNonExceptional(index);

            index = AddCraft(typeof(SongWovenMantle), 1015293, 1072931, 92.5, 117.5, typeof(Leather), 1044462, 15, 1044463);
            AddRes(index, typeof(EyeOfTheTravesty), 1032685, 1, 1044253);
            AddRes(index, typeof(Blight), 1032675, 10, 1044253);
            AddRes(index, typeof(Muculent), 1032680, 10, 1044253);
            AddRecipe(index, (int)TailorRecipe.SongWovenMantle);
            ForceNonExceptional(index);

            index = AddCraft(typeof(StitchersMittens), 1015293, 1072932, 92.5, 117.5, typeof(Leather), 1044462, 15, 1044463);
            AddRes(index, typeof(CapturedEssence), 1032686, 1, 1044253);
            AddRes(index, typeof(Corruption), 1032676, 10, 1044253);
            AddRes(index, typeof(Taint), 1032679, 10, 1044253);
            AddRecipe(index, (int)TailorRecipe.StitchersMittens);
            ForceNonExceptional(index);

            AddCraft(typeof(LeatherGorget), 1015293, 1025063, 53.9, 78.9, typeof(Leather), 1044462, 4, 1044463);
            AddCraft(typeof(LeatherCap), 1015293, 1027609, 6.2, 31.2, typeof(Leather), 1044462, 2, 1044463);
            AddCraft(typeof(LeatherGloves), 1015293, 1025062, 51.8, 76.8, typeof(Leather), 1044462, 3, 1044463);
            AddCraft(typeof(LeatherArms), 1015293, 1025061, 53.9, 78.9, typeof(Leather), 1044462, 4, 1044463);
            AddCraft(typeof(LeatherLegs), 1015293, 1025067, 66.3, 91.3, typeof(Leather), 1044462, 10, 1044463);
            AddCraft(typeof(LeatherChest), 1015293, 1025068, 70.5, 95.5, typeof(Leather), 1044462, 12, 1044463);

            index = AddCraft(typeof(LeatherJingasa), 1015293, 1030177, 45.0, 70.0, typeof(Leather), 1044462, 4, 1044463);

            index = AddCraft(typeof(LeatherMempo), 1015293, 1030181, 80.0, 105.0, typeof(Leather), 1044462, 8, 1044463);

            index = AddCraft(typeof(LeatherDo), 1015293, 1030182, 75.0, 100.0, typeof(Leather), 1044462, 12, 1044463);

            index = AddCraft(typeof(LeatherHiroSode), 1015293, 1030185, 55.0, 80.0, typeof(Leather), 1044462, 5, 1044463);

            index = AddCraft(typeof(LeatherSuneate), 1015293, 1030193, 68.0, 93.0, typeof(Leather), 1044462, 12, 1044463);

            index = AddCraft(typeof(LeatherHaidate), 1015293, 1030197, 68.0, 93.0, typeof(Leather), 1044462, 12, 1044463);

            index = AddCraft(typeof(LeatherNinjaPants), 1015293, 1030204, 80.0, 105.0, typeof(Leather), 1044462, 13, 1044463);

            index = AddCraft(typeof(LeatherNinjaJacket), 1015293, 1030206, 85.0, 110.0, typeof(Leather), 1044462, 13, 1044463);

            index = AddCraft(typeof(LeatherNinjaBelt), 1015293, 1030203, 50.0, 75.0, typeof(Leather), 1044462, 5, 1044463);

            index = AddCraft(typeof(LeatherNinjaMitts), 1015293, 1030205, 65.0, 90.0, typeof(Leather), 1044462, 12, 1044463);

            index = AddCraft(typeof(LeatherNinjaHood), 1015293, 1030201, 90.0, 115.0, typeof(Leather), 1044462, 14, 1044463);

            index = AddCraft(typeof(LeafChest), 1015293, 1032667, 75.0, 100.0, typeof(Leather), 1044462, 15, 1044463);

            index = AddCraft(typeof(LeafArms), 1015293, 1032670, 60.0, 85.0, typeof(Leather), 1044462, 12, 1044463);

            index = AddCraft(typeof(LeafGloves), 1015293, 1032668, 60.0, 85.0, typeof(Leather), 1044462, 10, 1044463);

            index = AddCraft(typeof(LeafLegs), 1015293, 1032671, 75.0, 100.0, typeof(Leather), 1044462, 15, 1044463);

            index = AddCraft(typeof(LeafGorget), 1015293, 1032669, 65.0, 90.0, typeof(Leather), 1044462, 12, 1044463);

            index = AddCraft(typeof(LeafTonlet), 1015293, 1032672, 70.0, 95.0, typeof(Leather), 1044462, 12, 1044463);

            index = AddCraft(typeof(GargishLeatherArms), 1015293, 1095327, 53.9, 78.9, typeof(Leather), 1044462, 8, 1044463);

            index = AddCraft(typeof(GargishLeatherChest), 1015293, 1095329, 70.5, 95.5, typeof(Leather), 1044462, 8, 1044463);

            index = AddCraft(typeof(GargishLeatherLegs), 1015293, 1095333, 66.3, 91.3, typeof(Leather), 1044462, 10, 1044463);

            index = AddCraft(typeof(GargishLeatherKilt), 1015293, 1095331, 58.0, 83.0, typeof(Leather), 1044462, 6, 1044463);

            index = AddCraft(typeof(FemaleGargishLeatherArms), 1015293, 1095327, 53.9, 78.9, typeof(Leather), 1044462, 8, 1044463);

            index = AddCraft(typeof(FemaleGargishLeatherChest), 1015293, 1095329, 70.5, 95.5, typeof(Leather), 1044462, 8, 1044463);

            index = AddCraft(typeof(FemaleGargishLeatherLegs), 1015293, 1095333, 66.3, 91.3, typeof(Leather), 1044462, 10, 1044463);

            index = AddCraft(typeof(FemaleGargishLeatherKilt), 1015293, 1095331, 58.0, 83.0, typeof(Leather), 1044462, 6, 1044463);

            index = AddCraft(typeof(GargishLeatherWingArmor), 1015293, 1096662, 65.0, 90.0, typeof(Leather), 1044462, 12, 1044463);

            index = AddCraft(typeof(TigerPeltChest), 1015293, 1109626, 90.0, 115.0, typeof(Leather), 1044462, 8, 1044463);
            AddRes(index, typeof(TigerPelt), 1123908, 4, 1044253);
            AddRecipe(index, (int)TailorRecipe.TigerPeltChest);

            index = AddCraft(typeof(TigerPeltLegs), 1015293, 1109628, 90.0, 115.0, typeof(Leather), 1044462, 8, 1044463);
            AddRes(index, typeof(TigerPelt), 1123908, 4, 1044253);
            AddRecipe(index, (int)TailorRecipe.TigerPeltLegs);

            index = AddCraft(typeof(TigerPeltShorts), 1015293, 1109629, 90.0, 115.0, typeof(Leather), 1044462, 4, 1044463);
            AddRes(index, typeof(TigerPelt), 1123908, 2, 1044253);
            AddRecipe(index, (int)TailorRecipe.TigerPeltShorts);

            index = AddCraft(typeof(TigerPeltHelm), 1015293, 1109632, 90.0, 115.0, typeof(Leather), 1044462, 2, 1044463);
            AddRes(index, typeof(TigerPelt), 1123908, 1, 1044253);
            AddRecipe(index, (int)TailorRecipe.TigerPeltHelm);

            index = AddCraft(typeof(TigerPeltCollar), 1015293, 1109633, 90.0, 115.0, typeof(Leather), 1044462, 2, 1044463);
            AddRes(index, typeof(TigerPelt), 1123908, 1, 1044253);
            AddRecipe(index, (int)TailorRecipe.TigerPeltCollar);

            index = AddCraft(typeof(DragonTurtleHideChest), 1015293, 1109634, 101.5, 116.5, typeof(Leather), 1044462, 8, 1044463);
            AddRes(index, typeof(DragonTurtleScute), 1123910, 2, 1044253);
            AddRecipe(index, (int)TailorRecipe.DragonTurtleHideChest);

            index = AddCraft(typeof(DragonTurtleHideLegs), 1015293, 1109636, 101.5, 116.5, typeof(Leather), 1044462, 8, 1044463);
            AddRes(index, typeof(DragonTurtleScute), 1123910, 4, 1044253);
            AddRecipe(index, (int)TailorRecipe.DragonTurtleHideLegs);

            index = AddCraft(typeof(DragonTurtleHideHelm), 1015293, 1109637, 101.5, 116.5, typeof(Leather), 1044462, 2, 1044463);
            AddRes(index, typeof(DragonTurtleScute), 1123910, 1, 1044253);
            AddRecipe(index, (int)TailorRecipe.DragonTurtleHideHelm);

            index = AddCraft(typeof(DragonTurtleHideArms), 1015293, 1109638, 101.5, 116.5, typeof(Leather), 1044462, 4, 1044463);
            AddRes(index, typeof(DragonTurtleScute), 1123910, 2, 1044253);
            AddRecipe(index, (int)TailorRecipe.DragonTurtleHideArms);

            #endregion

            #region Cloth Armor
            index = AddCraft(typeof(GargishClothArmsArmor), 1111748, 1021027, 87.1, 137.1, typeof(Cloth), 1044455, 8, 1044287);

            index = AddCraft(typeof(GargishClothChestArmor), 1111748, 1021029, 94.0, 144.0, typeof(Cloth), 1044455, 8, 1044287);

            index = AddCraft(typeof(GargishClothLegsArmor), 1111748, 1021033, 91.2, 141.2, typeof(Cloth), 1044455, 10, 1044287);

            index = AddCraft(typeof(GargishClothKiltArmor), 1111748, 1021031, 82.9, 132.9, typeof(Cloth), 1044455, 6, 1044287);

            index = AddCraft(typeof(FemaleGargishClothArmsArmor), 1111748, 1021027, 87.1, 137.1, typeof(Cloth), 1044455, 8, 1044287);

            index = AddCraft(typeof(FemaleGargishClothChestArmor), 1111748, 1021029, 94.0, 144.0, typeof(Cloth), 1044455, 8, 1044287);

            index = AddCraft(typeof(FemaleGargishClothLegsArmor), 1111748, 1021033, 91.2, 141.2, typeof(Cloth), 1044455, 10, 1044287);

            index = AddCraft(typeof(FemaleGargishClothKiltArmor), 1111748, 1021031, 82.9, 132.9, typeof(Cloth), 1044455, 6, 1044287);

            index = AddCraft(typeof(GargishClothWingArmor), 1111748, 1115393, 65.0, 90.0, typeof(Cloth), 1044455, 12, 1044287);
            #endregion

            #region Studded Armor
            AddCraft(typeof(StuddedGorget), 1015300, 1025078, 78.8, 103.8, typeof(Leather), 1044462, 6, 1044463);
            AddCraft(typeof(StuddedGloves), 1015300, 1025077, 82.9, 107.9, typeof(Leather), 1044462, 8, 1044463);
            AddCraft(typeof(StuddedArms), 1015300, 1025076, 87.1, 112.1, typeof(Leather), 1044462, 10, 1044463);
            AddCraft(typeof(StuddedLegs), 1015300, 1025082, 91.2, 116.2, typeof(Leather), 1044462, 12, 1044463);
            AddCraft(typeof(StuddedChest), 1015300, 1025083, 94.0, 119.0, typeof(Leather), 1044462, 14, 1044463);

            index = AddCraft(typeof(StuddedMempo), 1015300, 1030216, 80.0, 105.0, typeof(Leather), 1044462, 8, 1044463);

            index = AddCraft(typeof(StuddedDo), 1015300, 1030183, 95.0, 120.0, typeof(Leather), 1044462, 14, 1044463);

            index = AddCraft(typeof(StuddedHiroSode), 1015300, 1030186, 85.0, 110.0, typeof(Leather), 1044462, 8, 1044463);

            index = AddCraft(typeof(StuddedSuneate), 1015300, 1030194, 92.0, 117.0, typeof(Leather), 1044462, 14, 1044463);

            index = AddCraft(typeof(StuddedHaidate), 1015300, 1030198, 92.0, 117.0, typeof(Leather), 1044462, 14, 1044463);

            index = AddCraft(typeof(HideChest), 1015300, 1032651, 85.0, 110.0, typeof(Leather), 1044462, 15, 1044463);

            index = AddCraft(typeof(HidePauldrons), 1015300, 1032654, 75.0, 100.0, typeof(Leather), 1044462, 12, 1044463);

            index = AddCraft(typeof(HideGloves), 1015300, 1032652, 75.0, 100.0, typeof(Leather), 1044462, 10, 1044463);

            index = AddCraft(typeof(HidePants), 1015300, 1032655, 92.0, 117.0, typeof(Leather), 1044462, 15, 1044463);

            index = AddCraft(typeof(HideGorget), 1015300, 1032653, 90.0, 115.0, typeof(Leather), 1044462, 12, 1044463);

            #endregion

            #region Female Armor
            AddCraft(typeof(LeatherShorts), 1015306, 1027168, 62.2, 87.2, typeof(Leather), 1044462, 8, 1044463);
            AddCraft(typeof(LeatherSkirt), 1015306, 1027176, 58.0, 83.0, typeof(Leather), 1044462, 6, 1044463);
            AddCraft(typeof(LeatherBustierArms), 1015306, 1027178, 58.0, 83.0, typeof(Leather), 1044462, 6, 1044463);
            AddCraft(typeof(StuddedBustierArms), 1015306, 1027180, 82.9, 107.9, typeof(Leather), 1044462, 8, 1044463);
            AddCraft(typeof(FemaleLeatherChest), 1015306, 1027174, 62.2, 87.2, typeof(Leather), 1044462, 8, 1044463);
            AddCraft(typeof(FemaleStuddedChest), 1015306, 1027170, 87.1, 112.1, typeof(Leather), 1044462, 10, 1044463);

            index = AddCraft(typeof(TigerPeltBustier), 1015306, 1109627, 90.0, 115.0, typeof(Leather), 1044462, 6, 1044463);
            AddRes(index, typeof(TigerPelt), 1123908, 3, 1044253);
            AddRecipe(index, (int)TailorRecipe.TigerPeltBustier);

            index = AddCraft(typeof(TigerPeltLongSkirt), 1015306, 1109630, 90.0, 115.0, typeof(Leather), 1044462, 4, 1044463);
            AddRes(index, typeof(TigerPelt), 1123908, 2, 1044253);
            AddRecipe(index, (int)TailorRecipe.TigerPeltLongSkirt);

            index = AddCraft(typeof(TigerPeltSkirt), 1015306, 1109631, 90.0, 115.0, typeof(Leather), 1044462, 4, 1044463);
            AddRes(index, typeof(TigerPelt), 1123908, 2, 1044253);
            AddRecipe(index, (int)TailorRecipe.TigerPeltSkirt);

            index = AddCraft(typeof(DragonTurtleHideBustier), 1015306, 1109635, 101.5, 116.5, typeof(Leather), 1044462, 6, 1044463);
            AddRes(index, typeof(DragonTurtleScute), 1123910, 3, 1044253);
            AddRecipe(index, (int)TailorRecipe.DragonTurtleHideBustier);

            #endregion

            #region Bone Armor
            index = AddCraft(typeof(BoneHelm), 1049149, 1025206, 85.0, 110.0, typeof(Leather), 1044462, 4, 1044463);
            AddRes(index, typeof(Bone), 1049064, 2, 1049063);

            index = AddCraft(typeof(BoneGloves), 1049149, 1025205, 89.0, 114.0, typeof(Leather), 1044462, 6, 1044463);
            AddRes(index, typeof(Bone), 1049064, 2, 1049063);

            index = AddCraft(typeof(BoneArms), 1049149, 1025203, 92.0, 117.0, typeof(Leather), 1044462, 8, 1044463);
            AddRes(index, typeof(Bone), 1049064, 4, 1049063);

            index = AddCraft(typeof(BoneLegs), 1049149, 1025202, 95.0, 120.0, typeof(Leather), 1044462, 10, 1044463);
            AddRes(index, typeof(Bone), 1049064, 6, 1049063);

            index = AddCraft(typeof(BoneChest), 1049149, 1025199, 96.0, 121.0, typeof(Leather), 1044462, 12, 1044463);
            AddRes(index, typeof(Bone), 1049064, 10, 1049063);

            index = AddCraft(typeof(OrcHelm), 1049149, 1027947, 90.0, 115.0, typeof(Leather), 1044462, 6, 1044463);
            AddRes(index, typeof(Bone), 1049064, 4, 1049063);

            index = AddCraft(typeof(CuffsOfTheArchmage), 1049149, 1157348, 120.0, 120.1, typeof(Cloth), 1044455, 8, 1044287);
            AddRes(index, typeof(MidnightBracers), 1061093, 1, 1044253);
            AddRes(index, typeof(BloodOfTheDarkFather), 1157343, 5, 1044253);
            AddRes(index, typeof(DarkSapphire), 1032690, 4, 1044253);
            ForceNonExceptional(index);
            AddRecipe(index, (int)TailorRecipe.CuffsOfTheArchmage);
            #endregion

            // Set the overridable material
            SetSubRes(typeof(Leather), 1049150);

            // Add every material you want the player to be able to choose from
            // This will override the overridable material
            AddSubRes(typeof(Leather), 1049150, 0.0, 1044462, 1049312);
            AddSubRes(typeof(SpinedLeather), 1049151, 65.0, 1044462, 1049312);
            AddSubRes(typeof(HornedLeather), 1049152, 80.0, 1044462, 1049312);
            AddSubRes(typeof(BarbedLeather), 1049153, 99.0, 1044462, 1049312);

            MarkOption = true;
            Repair = true;
            CanEnhance = true;
            CanAlter = true;
        } 
        #endregion

        private void CutUpCloth(Mobile m, CraftItem craftItem, ITool tool)
        {
            PlayCraftEffect(m);

            Timer.DelayCall(TimeSpan.FromSeconds(Delay), () =>
                {
                    if (m.Backpack == null)
                    {
                        m.SendGump(new CraftGump(m, this, tool, null));
                    }

                    Dictionary<int, int> bolts = new Dictionary<int, int>();
                    List<Item> toConsume = new List<Item>();
                    object num = null;
                    Container pack = m.Backpack;

                    foreach (Item item in pack.Items)
                    {
                        if (item.GetType() == typeof(BoltOfCloth))
                        {
                            if (!bolts.ContainsKey(item.Hue))
                            {
                                toConsume.Add(item);
                                bolts[item.Hue] = item.Amount;
                            }
                            else
                            {
                                toConsume.Add(item);
                                bolts[item.Hue] += item.Amount;
                            }
                        }
                    }

                    if (bolts.Count == 0)
                    {
                        num = 1044253; // You don't have the components needed to make that.
                    }
                    else
                    {
                        foreach (Item item in toConsume)
                        {
                            item.Delete();
                        }

                        foreach (KeyValuePair<int, int> kvp in bolts)
                        {
                            UncutCloth cloth = new UncutCloth(kvp.Value * 50)
                            {
                                Hue = kvp.Key
                            };

                            DropItem(m, cloth, tool);
                        }
                    }

                    if (tool != null)
                    {
                        tool.UsesRemaining--;

                        if (tool.UsesRemaining <= 0 && !tool.Deleted)
                        {
                            tool.Delete();
                            m.SendLocalizedMessage(1044038);
                        }
                        else
                        {
                            m.SendGump(new CraftGump(m, this, tool, num));
                        }
                    }

                    ColUtility.Free(toConsume);
                    bolts.Clear();
                });
        }

        private void CombineCloth(Mobile m, CraftItem craftItem, ITool tool)
        {
            PlayCraftEffect(m);

            Timer.DelayCall(TimeSpan.FromSeconds(Delay), () =>
                {
                    if (m.Backpack == null)
                    {
                        m.SendGump(new CraftGump(m, this, tool, null));
                    }

                    Container pack = m.Backpack;

                    Dictionary<int, int> cloth = new Dictionary<int, int>();
                    List<Item> toConsume = new List<Item>();
                    object num = null;

                    foreach (Item item in pack.Items)
                    {
                        Type t = item.GetType();

                        if (t == typeof(UncutCloth) || t == typeof(Cloth) || t == typeof(CutUpCloth))
                        {
                            if (!cloth.ContainsKey(item.Hue))
                            {
                                toConsume.Add(item);
                                cloth[item.Hue] = item.Amount;
                            }
                            else
                            {
                                toConsume.Add(item);
                                cloth[item.Hue] += item.Amount;
                            }
                        }
                    }

                    if (cloth.Count == 0)
                    {
                        num = 1044253; // You don't have the components needed to make that.
                    }
                    else
                    {
                        foreach (Item item in toConsume)
                        {
                            item.Delete();
                        }

                        foreach (KeyValuePair<int, int> kvp in cloth)
                        {
                            UncutCloth c = new UncutCloth(kvp.Value)
                            {
                                Hue = kvp.Key
                            };

                            DropItem(m, c, tool);
                        }
                    }

                    if (tool != null)
                    {
                        tool.UsesRemaining--;

                        if (tool.UsesRemaining <= 0 && !tool.Deleted)
                        {
                            tool.Delete();
                            m.SendLocalizedMessage(1044038);
                        }
                        else
                        {
                            m.SendGump(new CraftGump(m, this, tool, num));
                        }
                    }

                    ColUtility.Free(toConsume);
                    cloth.Clear();
                });
        }

        private void DropItem(Mobile from, Item item, ITool tool)
        {
            if (tool is Item && ((Item)tool).Parent is Container)
            {
                Container cntnr = (Container)((Item)tool).Parent;

                if (!cntnr.TryDropItem(from, item, false))
                {
                    if (cntnr != from.Backpack)
                        from.AddToBackpack(item);
                    else
                        item.MoveToWorld(from.Location, from.Map);
                }
            }
            else
            {
                from.AddToBackpack(item);
            }
        }
    }
}
