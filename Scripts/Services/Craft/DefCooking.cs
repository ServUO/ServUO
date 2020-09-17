using Server.Items;
using System;

namespace Server.Engines.Craft
{
    #region Recipes
    public enum CookRecipes
    {
        // magical
        RotWormStew = 500,
        GingerbreadCookie = 599,

        DarkChocolateNutcracker = 600,
        MilkChocolateNutcracker = 601,
        WhiteChocolateNutcracker = 602,

        ThreeTieredCake = 603,
        BlackrockStew = 604,
        Hamburger = 605,
        HotDog = 606,
        Sausage = 607,

        GrilledSerpentSteak = 608,
        BBQDinoRibs = 609,
        WakuChicken = 610
    }
    #endregion

    public class DefCooking : CraftSystem
    {
        public override SkillName MainSkill => SkillName.Cooking;

        public override int GumpTitleNumber => 1044003;

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefCooking();

                return m_CraftSystem;
            }
        }

        public override CraftECA ECA => CraftECA.ChanceMinusSixtyToFourtyFive;

        public override double GetChanceAtMin(CraftItem item)
        {
            if (item.ItemType == typeof(GrapesOfWrath) ||
                item.ItemType == typeof(EnchantedApple))
            {
                return .5;
            }

            return 0.0; // 0%
        }

        private DefCooking()
            : base(1, 1, 1.25)// base( 1, 1, 1.5 )
        {
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

        public override void PlayCraftEffect(Mobile from)
        {
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

            #region Ingredients
            index = AddCraft(typeof(SackFlour), 1044495, 1024153, 0.0, 100.0, typeof(WheatSheaf), 1044489, 2, 1044490);
            SetNeedMill(index, true);

            index = AddCraft(typeof(Dough), 1044495, 1024157, 0.0, 100.0, typeof(SackFlourOpen), 1044468, 1, 1151092);
            AddRes(index, typeof(BaseBeverage), 1046458, 1, 1044253);

            index = AddCraft(typeof(SweetDough), 1044495, 1041340, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            AddRes(index, typeof(JarHoney), 1044472, 1, 1044253);

            index = AddCraft(typeof(CakeMix), 1044495, 1041002, 0.0, 100.0, typeof(SackFlourOpen), 1044468, 1, 1151092);
            AddRes(index, typeof(SweetDough), 1044475, 1, 1044253);

            index = AddCraft(typeof(CookieMix), 1044495, 1024159, 0.0, 100.0, typeof(JarHoney), 1044472, 1, 1044253);
            AddRes(index, typeof(SweetDough), 1044475, 1, 1044253);

            index = AddCraft(typeof(CocoaButter), 1044495, 1079998, 0.0, 100.0, typeof(CocoaPulp), 1080530, 1, 1044253);
            SetItemHue(index, 0x457);
            SetNeedOven(index, true);

            index = AddCraft(typeof(CocoaLiquor), 1044495, 1079999, 0.0, 100.0, typeof(CocoaPulp), 1080530, 1, 1044253);
            AddRes(index, typeof(EmptyPewterBowl), 1025629, 1, 1044253);
            SetItemHue(index, 0x46A);
            SetNeedOven(index, true);

            index = AddCraft(typeof(WheatWort), 1044495, 1150275, 30.0, 100.0, typeof(Bottle), 1023854, 1, 1044253);
            AddRes(index, typeof(BaseBeverage), 1046458, 1, 1044253);
            AddRes(index, typeof(SackFlourOpen), 1044468, 1, 1151092);
            SetItemHue(index, 1281);

            index = AddCraft(typeof(FishOilFlask), 1044495, 1150863, 60.0, 110.0, typeof(CoffeeGrounds), 1155735, 1, 1155734);
            AddRes(index, typeof(RawFishSteak), 1044476, 8, 1044253);
            SetUseAllRes(index, true);

            index = AddCraft(typeof(FreshSeasoning), 1044495, 1159200, 75.0, 125.0, typeof(DriedHerbs), 1023137, 1, 1044253);
            AddRes(index, typeof(Salt), 1159201, 3, 1044253);
            #endregion

            #region Preparations
            index = AddCraft(typeof(UnbakedQuiche), 1044496, 1041339, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            AddRes(index, typeof(Eggs), 1044477, 1, 1044253);

            // TODO: This must also support chicken and lamb legs
            index = AddCraft(typeof(UnbakedMeatPie), 1044496, 1041338, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            AddRes(index, typeof(RawRibs), 1044482, 1, 1044253);

            index = AddCraft(typeof(UncookedSausagePizza), 1044496, 1041337, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            AddRes(index, typeof(Sausage), 1044483, 1, 1044253);

            index = AddCraft(typeof(UncookedCheesePizza), 1044496, 1041341, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            AddRes(index, typeof(CheeseWheel), 1044486, 1, 1044253);

            index = AddCraft(typeof(UnbakedFruitPie), 1044496, 1041334, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            AddRes(index, typeof(Pear), 1044481, 1, 1044253);

            index = AddCraft(typeof(UnbakedPeachCobbler), 1044496, 1041335, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            AddRes(index, typeof(Peach), 1044480, 1, 1044253);

            index = AddCraft(typeof(UnbakedApplePie), 1044496, 1041336, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            AddRes(index, typeof(Apple), 1044479, 1, 1044253);

            index = AddCraft(typeof(UnbakedPumpkinPie), 1044496, 1041342, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            AddRes(index, typeof(Pumpkin), 1044484, 1, 1044253);

            index = AddCraft(typeof(GreenTea), 1044496, 1030316, 80.0, 130.0, typeof(GreenTeaBasket), 1030316, 1, 1044253);
            AddRes(index, typeof(BaseBeverage), 1046458, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(WasabiClumps), 1044496, 1029451, 70.0, 120.0, typeof(BaseBeverage), 1046458, 1, 1044253);
            AddRes(index, typeof(WoodenBowlOfPeas), 1025633, 3, 1044253);

            index = AddCraft(typeof(SushiRolls), 1044496, 1030303, 90.0, 120.0, typeof(BaseBeverage), 1046458, 1, 1044253);
            AddRes(index, typeof(RawFishSteak), 1044476, 10, 1044253);

            index = AddCraft(typeof(SushiPlatter), 1044496, 1030305, 90.0, 120.0, typeof(BaseBeverage), 1046458, 1, 1044253);
            AddRes(index, typeof(RawFishSteak), 1044476, 10, 1044253);

            index = AddCraft(typeof(TribalPaint), 1044496, 1040000, 55.0, 105.0, typeof(SackFlourOpen), 1044468, 1, 1151092);
            AddRes(index, typeof(TribalBerry), 1046460, 1, 1044253);

            index = AddCraft(typeof(EggBomb), 1044496, 1030249, 90.0, 120.0, typeof(Eggs), 1044477, 1, 1044253);
            AddRes(index, typeof(SackFlourOpen), 1044468, 3, 1151092);

            index = AddCraft(typeof(ParrotWafer), 1044496, 1032246, 37.5, 87.5, typeof(Dough), 1044469, 1, 1044253);
            AddRes(index, typeof(JarHoney), 1044472, 1, 1044253);
            AddRes(index, typeof(RawFishSteak), 1044476, 10, 1044253);

            index = AddCraft(typeof(PlantPigment), 1044496, 1112132, 75.0, 100.0, typeof(PlantClippings), 1112131, 1, 1044253);
            AddRes(index, typeof(Bottle), 1023854, 1, 1044253);
            SetRequireResTarget(index);

            index = AddCraft(typeof(NaturalDye), 1044496, 1112136, 65.0, 115.0, typeof(PlantPigment), 1112132, 1, 1044253);
            AddRes(index, typeof(ColorFixative), 1112135, 1, 1044253);
            SetRequireResTarget(index);

            index = AddCraft(typeof(ColorFixative), 1044496, 1112135, 75.0, 100.0, typeof(BaseBeverage), 1022503, 1, 1044253);
            AddRes(index, typeof(SilverSerpentVenom), 1112173, 1, 1044253);
            SetBeverageType(index, BeverageType.Wine);

            index = AddCraft(typeof(WoodPulp), 1044496, 1113136, 60.0, 100.0, typeof(BarkFragment), 1032687, 1, 1044253);
            AddRes(index, typeof(BaseBeverage), 1046458, 1, 1044253);

            index = AddCraft(typeof(Charcoal), 1044496, 1116303, 0.0, 50.0, typeof(Board), 1044041, 1, 1044351);
            SetUseAllRes(index, true);
            SetNeedHeat(index, true);

            index = AddCraft(typeof(Bait), 1044496, 1159164, 100.0, 140.0, typeof(SamuelsSecretSauce), 1116338, 1, 1044253);
            AddRes(index, typeof(RawFishSteak), 1044476, 50, 1044253);
            AddRes(index, typeof(SaltedSerpentSteaks), 1159163, 3, 1044253);
            AddCreateItem(index, CraftCharydbisBait);
            SetForceSuccess(index, 5);
            SetNeedHeat(index, true);
            #endregion

            #region Baking
            index = AddCraft(typeof(BreadLoaf), 1044497, 1024156, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(Cookies), 1044497, 1025643, 0.0, 100.0, typeof(CookieMix), 1044474, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(Cake), 1044497, 1022537, 0.0, 100.0, typeof(CakeMix), 1044471, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(Muffins), 1044497, 1022539, 0.0, 100.0, typeof(SweetDough), 1044475, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(Quiche), 1044497, 1041345, 0.0, 100.0, typeof(UnbakedQuiche), 1044518, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(MeatPie), 1044497, 1041347, 0.0, 100.0, typeof(UnbakedMeatPie), 1044519, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(SausagePizza), 1044497, 1044517, 0.0, 100.0, typeof(UncookedSausagePizza), 1044520, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(CheesePizza), 1044497, 1044516, 0.0, 100.0, typeof(UncookedCheesePizza), 1044521, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(FruitPie), 1044497, 1041346, 0.0, 100.0, typeof(UnbakedFruitPie), 1044522, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(PeachCobbler), 1044497, 1041344, 0.0, 100.0, typeof(UnbakedPeachCobbler), 1044523, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(ApplePie), 1044497, 1041343, 0.0, 100.0, typeof(UnbakedApplePie), 1044524, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(PumpkinPie), 1044497, 1041348, 0.0, 100.0, typeof(UnbakedPumpkinPie), 1046461, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(MisoSoup), 1044497, 1030317, 60.0, 110.0, typeof(RawFishSteak), 1044476, 1, 1044253);
            AddRes(index, typeof(BaseBeverage), 1046458, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(WhiteMisoSoup), 1044497, 1030318, 60.0, 110.0, typeof(RawFishSteak), 1044476, 1, 1044253);
            AddRes(index, typeof(BaseBeverage), 1046458, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(RedMisoSoup), 1044497, 1030319, 60.0, 110.0, typeof(RawFishSteak), 1044476, 1, 1044253);
            AddRes(index, typeof(BaseBeverage), 1046458, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(AwaseMisoSoup), 1044497, 1030320, 60.0, 110.0, typeof(RawFishSteak), 1044476, 1, 1044253);
            AddRes(index, typeof(BaseBeverage), 1046458, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(GingerBreadCookie), 1044497, 1031233, 35.0, 85.0, typeof(CookieMix), 1044474, 1, 1044253);
            AddRes(index, typeof(FreshGinger), 1031235, 1, 1044253);
            AddRecipe(index, (int)CookRecipes.GingerbreadCookie);
            SetNeedOven(index, true);

            index = AddCraft(typeof(ThreeTieredCake), 1044497, 1154465, 60.0, 110.0, typeof(CakeMix), 1044471, 3, 1044253);
            AddRecipe(index, (int)CookRecipes.ThreeTieredCake);
            SetNeedOven(index, true);
            #endregion

            #region Barbecue
            index = AddCraft(typeof(CookedBird), 1044498, 1022487, 0.0, 100.0, typeof(RawBird), 1044470, 1, 1044253);
            SetNeedHeat(index, true);
            SetUseAllRes(index, true);
            ForceNonExceptional(index);

            index = AddCraft(typeof(ChickenLeg), 1044498, 1025640, 0.0, 100.0, typeof(RawChickenLeg), 1044473, 1, 1044253);
            SetNeedHeat(index, true);
            SetUseAllRes(index, true);
            ForceNonExceptional(index);

            index = AddCraft(typeof(FishSteak), 1044498, 1022427, 0.0, 100.0, typeof(RawFishSteak), 1044476, 1, 1044253);
            SetNeedHeat(index, true);
            SetUseAllRes(index, true);
            ForceNonExceptional(index);

            index = AddCraft(typeof(FriedEggs), 1044498, 1022486, 0.0, 100.0, typeof(Eggs), 1044477, 1, 1044253);
            SetNeedHeat(index, true);
            SetUseAllRes(index, true);
            ForceNonExceptional(index);

            index = AddCraft(typeof(LambLeg), 1044498, 1025642, 0.0, 100.0, typeof(RawLambLeg), 1044478, 1, 1044253);
            SetNeedHeat(index, true);
            SetUseAllRes(index, true);
            ForceNonExceptional(index);

            index = AddCraft(typeof(Ribs), 1044498, 1022546, 0.0, 100.0, typeof(RawRibs), 1044485, 1, 1044253);
            SetNeedHeat(index, true);
            SetUseAllRes(index, true);
            ForceNonExceptional(index);

            index = AddCraft(typeof(BowlOfRotwormStew), 1044498, 1031706, 0.0, 100.0, typeof(RawRotwormMeat), 1031705, 1, 1044253);
            SetNeedHeat(index, true);
            SetUseAllRes(index, true);
            AddRecipe(index, (int)CookRecipes.RotWormStew);
            ForceNonExceptional(index);

            index = AddCraft(typeof(BowlOfBlackrockStew), 1044498, 1115752, 30.0, 70.0, typeof(BowlOfRotwormStew), 1031706, 1, 1044253);
            AddRes(index, typeof(SmallPieceofBlackrock), 1153836, 1, 1044253);
            SetNeedHeat(index, true);
            SetItemHue(index, 1954);
            AddRecipe(index, (int)CookRecipes.BlackrockStew);
            ForceNonExceptional(index);

            index = AddCraft(typeof(KhaldunTastyTreat), 1044498, 1158680, 60.0, 100.0, typeof(RawFishSteak), 1044476, 40, 1044253);
            SetUseAllRes(index, true);
            SetNeedHeat(index, true);

            index = AddCraft(typeof(Hamburger), 1044498, 1125202, 45.0, 95.0, typeof(BreadLoaf), 1024155, 1, 1044253);
            AddRes(index, typeof(RawRibs), 1044485, 1, 1044253);
            AddRes(index, typeof(Lettuce), 1023184, 1, 1044253);
            SetNeedHeat(index, true);
            AddRecipe(index, (int)CookRecipes.Hamburger);

            index = AddCraft(typeof(HotDog), 1044498, 1125200, 45.0, 95.0, typeof(BreadLoaf), 1024155, 1, 1044253);
            AddRes(index, typeof(Sausage), 1125198, 1, 1044253);
            SetNeedHeat(index, true);
            AddRecipe(index, (int)CookRecipes.HotDog);

            index = AddCraft(typeof(CookableSausage), 1044498, 1125198, 45.0, 95.0, typeof(Ham), 1022515, 1, 1044253);
            AddRes(index, typeof(DriedHerbs), 1023137, 1, 1044253);
            SetNeedHeat(index, true);
            AddRecipe(index, (int)CookRecipes.Sausage);

            index = AddCraft(typeof(GrilledSerpentSteak), 1044498, 1159197, 45.0, 95.0, typeof(RawSeaSerpentSteak), 1126043, 1, 1044253);
            AddRes(index, typeof(FreshSeasoning), 1159200, 1, 1044253);
            SetNeedHeat(index, true);
            SetUseAllRes(index, true);
            AddRecipe(index, (int)CookRecipes.GrilledSerpentSteak);

            index = AddCraft(typeof(BBQDinoRibs), 1044498, 1159198, 45.0, 95.0, typeof(RawDinoRibs), 1126045, 1, 1044253);
            AddRes(index, typeof(FreshSeasoning), 1159200, 1, 1044253);
            AddRes(index, typeof(SackOfSugar), 1079997, 1, 1044253);
            AddRes(index, typeof(SamuelsSecretSauce), 1116338, 1, 1044253);
            SetNeedHeat(index, true);
            SetUseAllRes(index, true);
            AddRecipe(index, (int)CookRecipes.BBQDinoRibs);

            index = AddCraft(typeof(WakuChicken), 1044498, 1159199, 45.0, 95.0, typeof(RawBird), 1044470, 1, 1044253);
            AddRes(index, typeof(DriedHerbs), 1023137, 1, 1044253);
            AddRes(index, typeof(SamuelsSecretSauce), 1116338, 1, 1044253);
            SetNeedHeat(index, true);
            SetUseAllRes(index, true);
            AddRecipe(index, (int)CookRecipes.WakuChicken);
            #endregion

            #region Enchanted
            index = AddCraft(typeof(FoodEngraver), 1073108, 1072951, 75.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            AddRes(index, typeof(JarHoney), 1044472, 1, 1044253);

            index = AddCraft(typeof(EnchantedApple), 1073108, 1072952, 60.0, 85.0, typeof(Apple), 1044479, 1, 1044253);
            AddRes(index, typeof(GreaterHealPotion), 1073467, 1, 1044253);
            ForceNonExceptional(index);

            index = AddCraft(typeof(GrapesOfWrath), 1073108, 1072953, 95.0, 120.0, typeof(Grapes), 1073468, 1, 1044253);
            AddRes(index, typeof(GreaterStrengthPotion), 1073466, 1, 1044253);
            ForceNonExceptional(index);

            index = AddCraft(typeof(FruitBowl), 1073108, 1072950, 55.0, 105.0, typeof(EmptyWoodenBowl), 1073472, 1, 1044253);
            AddRes(index, typeof(Pear), 1044481, 3, 1044253);
            AddRes(index, typeof(Apple), 1044479, 3, 1044253);
            AddRes(index, typeof(Banana), 1073470, 3, 1044253);
            #endregion

            #region Chocolatiering
            index = AddCraft(typeof(SweetCocoaButter), 1080001, 1156401, 15.0, 100.0, typeof(SackOfSugar), 1079997, 1, 1044253);
            AddRes(index, typeof(CocoaButter), 1079998, 1, 1044253);
            SetItemHue(index, 0x457);
            SetNeedOven(index, true);

            index = AddCraft(typeof(DarkChocolate), 1080001, 1079994, 15.0, 100.0, typeof(SackOfSugar), 1079997, 1, 1044253);
            AddRes(index, typeof(CocoaButter), 1079998, 1, 1044253);
            AddRes(index, typeof(CocoaLiquor), 1079999, 1, 1044253);
            SetItemHue(index, 0x465);

            index = AddCraft(typeof(MilkChocolate), 1080001, 1079995, 32.5, 107.5, typeof(SackOfSugar), 1079997, 1, 1044253);
            AddRes(index, typeof(CocoaButter), 1079998, 1, 1044253);
            AddRes(index, typeof(CocoaLiquor), 1079999, 1, 1044253);
            AddRes(index, typeof(BaseBeverage), 1022544, 1, 1044253);
            SetBeverageType(index, BeverageType.Milk);
            SetItemHue(index, 0x461);

            index = AddCraft(typeof(WhiteChocolate), 1080001, 1079996, 52.5, 127.5, typeof(SackOfSugar), 1079997, 1, 1044253);
            AddRes(index, typeof(CocoaButter), 1079998, 1, 1044253);
            AddRes(index, typeof(Vanilla), 1080000, 1, 1044253);
            AddRes(index, typeof(BaseBeverage), 1022544, 1, 1044253);
            SetBeverageType(index, BeverageType.Milk);
            SetItemHue(index, 0x47E);

            index = AddCraft(typeof(ChocolateNutcracker), 1080001, 1156390, 15.0, 100.0, typeof(SweetCocoaButter), 1156401, 1, 1044253);
            AddRes(index, typeof(SweetCocoaButter), 1124032, 1, 1156402);
            AddRes(index, typeof(CocoaLiquor), 1079999, 1, 1044253);
            AddRecipe(index, (int)CookRecipes.DarkChocolateNutcracker);
            SetData(index, ChocolateNutcracker.ChocolateType.Dark);

            index = AddCraft(typeof(ChocolateNutcracker), 1080001, 1156391, 32.5, 107.5, typeof(SweetCocoaButter), 1156401, 1, 1044253);
            AddRes(index, typeof(SweetCocoaButter), 1124032, 1, 1156402);
            AddRes(index, typeof(CocoaLiquor), 1079999, 1, 1044253);
            AddRecipe(index, (int)CookRecipes.MilkChocolateNutcracker);
            SetData(index, ChocolateNutcracker.ChocolateType.Milk);

            index = AddCraft(typeof(ChocolateNutcracker), 1080001, 1156392, 52.5, 127.5, typeof(SweetCocoaButter), 1156401, 1, 1044253);
            AddRes(index, typeof(SweetCocoaButter), 1124032, 1, 1156402);
            AddRes(index, typeof(CocoaLiquor), 1079999, 1, 1044253);
            AddRecipe(index, (int)CookRecipes.WhiteChocolateNutcracker);
            SetData(index, ChocolateNutcracker.ChocolateType.White);
            #endregion

            #region fish pies
            index = AddCraft(typeof(GreatBarracudaPie), 1116340, 1116214, 61.0, 110.0, typeof(GreatBarracudaSteak), 1116298, 1, 1044253);
            AddRes(index, typeof(MentoSeasoning), 1116299, 1, 1044253);
            AddRes(index, typeof(ZoogiFungus), 1029911, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(GiantKoiPie), 1116340, 1116216, 61.0, 110.0, typeof(GiantKoiSteak), 1044253, 1, 1044253);
            AddRes(index, typeof(MentoSeasoning), 1116299, 1, 1044253);
            AddRes(index, typeof(WoodenBowlOfPeas), 1025628, 1, 1044253);
            AddRes(index, typeof(Dough), 1024157, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(FireFishPie), 1116340, 1116217, 55.0, 105.0, typeof(FireFishSteak), 1116307, 1, 1044253);
            AddRes(index, typeof(Dough), 1024157, 1, 1044253);
            AddRes(index, typeof(Carrot), 1023191, 1, 1044253);
            AddRes(index, typeof(SamuelsSecretSauce), 1116338, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(StoneCrabPie), 1116340, 1116227, 55.0, 105.0, typeof(StoneCrabMeat), 1116317, 1, 1044253);
            AddRes(index, typeof(Dough), 1024157, 1, 1044253);
            AddRes(index, typeof(Cabbage), 1023195, 1, 1044253);
            AddRes(index, typeof(SamuelsSecretSauce), 1116338, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(BlueLobsterPie), 1116340, 1116228, 55.0, 105.0, typeof(BlueLobsterMeat), 1116318, 1, 1044253);
            AddRes(index, typeof(Dough), 1024157, 1, 1044253);
            AddRes(index, typeof(TribalBerry), 1040001, 1, 1044253);
            AddRes(index, typeof(SamuelsSecretSauce), 1116338, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(ReaperFishPie), 1116340, 1116218, 55.0, 105.0, typeof(ReaperFishSteak), 1116308, 1, 1044253);
            AddRes(index, typeof(Dough), 1024157, 1, 1044253);
            AddRes(index, typeof(Pumpkin), 1023178, 1, 1044253);
            AddRes(index, typeof(SamuelsSecretSauce), 1116338, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(CrystalFishPie), 1116340, 1116219, 55.0, 105.0, typeof(CrystalFishSteak), 1116309, 1, 1044253);
            AddRes(index, typeof(Dough), 1024157, 1, 1044253);
            AddRes(index, typeof(Apple), 1022512, 1, 1044253);
            AddRes(index, typeof(SamuelsSecretSauce), 1116338, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(BullFishPie), 1116340, 1116220, 55.0, 105.0, typeof(BullFishSteak), 1116310, 1, 1044253);
            AddRes(index, typeof(Dough), 1024157, 1, 1044253);
            AddRes(index, typeof(Squash), 1023186, 1, 1044253);
            AddRes(index, typeof(MentoSeasoning), 1116299, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(SummerDragonfishPie), 1116340, 1116221, 55.0, 105.0, typeof(SummerDragonfishSteak), 1116311, 1, 1044253);
            AddRes(index, typeof(Dough), 1024157, 1, 1044253);
            AddRes(index, typeof(Onion), 1023182, 1, 1044253);
            AddRes(index, typeof(MentoSeasoning), 1116299, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(FairySalmonPie), 1116340, 1116222, 55.0, 105.0, typeof(FairySalmonSteak), 1116312, 1, 1044253);
            AddRes(index, typeof(Dough), 1024157, 1, 1044253);
            AddRes(index, typeof(EarOfCorn), 1023199, 1, 1044253);
            AddRes(index, typeof(DarkTruffle), 1116300, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(LavaFishPie), 1116340, 1116223, 55.0, 105.0, typeof(LavaFishSteak), 1116313, 1, 1044253);
            AddRes(index, typeof(Dough), 1024157, 1, 1044253);
            AddRes(index, typeof(CheeseWheel), 1044486, 1, 1044253);
            AddRes(index, typeof(DarkTruffle), 1116300, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(AutumnDragonfishPie), 1116340, 1116224, 55.0, 105.0, typeof(AutumnDragonfishSteak), 1116314, 1, 1044253);
            AddRes(index, typeof(Dough), 1024157, 1, 1044253);
            AddRes(index, typeof(Pear), 1022452, 1, 1044253);
            AddRes(index, typeof(MentoSeasoning), 1116299, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(SpiderCrabPie), 1116340, 1116229, 55.0, 105.0, typeof(SpiderCrabMeat), 1116320, 1, 1044253);
            AddRes(index, typeof(Dough), 1024157, 1, 1044253);
            AddRes(index, typeof(Lettuce), 1023184, 1, 1044253);
            AddRes(index, typeof(MentoSeasoning), 1116299, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(YellowtailBarracudaPie), 1116340, 1116098, 55.0, 105.0, typeof(YellowtailBarracudaSteak), 1116301, 1, 1044253);
            AddRes(index, typeof(Dough), 1024157, 1, 1044253);
            AddRes(index, typeof(BaseBeverage), 1022503, 1, 1044253);
            AddRes(index, typeof(MentoSeasoning), 1116299, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(HolyMackerelPie), 1116340, 1116225, 55.0, 105.0, typeof(HolyMackerelSteak), 1116315, 1, 1044253);
            AddRes(index, typeof(Dough), 1024157, 1, 1044253);
            AddRes(index, typeof(JarHoney), 1022540, 1, 1044253);
            AddRes(index, typeof(MentoSeasoning), 1116299, 1, 1044253);
            SetNeedOven(index, true);

            index = AddCraft(typeof(UnicornFishPie), 1116340, 1116226, 55.0, 105.0, typeof(UnicornFishSteak), 1116316, 1, 1044253);
            AddRes(index, typeof(Dough), 1024157, 1, 1044253);
            AddRes(index, typeof(FreshGinger), 1031235, 1, 1044253);
            AddRes(index, typeof(MentoSeasoning), 1116299, 1, 1044253);
            SetNeedOven(index, true);
            #endregion

            #region Bevereages
            index = AddCraft(typeof(CoffeeMug), 1155736, 1155737, 0.0, 28.58, typeof(CoffeeGrounds), 1155735, 1, 1155734);
            AddRes(index, typeof(BaseBeverage), 1046458, 1, 1044253);
            SetBeverageType(index, BeverageType.Water);
            SetNeedMaker(index, true);
            ForceNonExceptional(index);

            index = AddCraft(typeof(BasketOfGreenTeaMug), 1155736, 1030315, 0.0, 28.58, typeof(GreenTeaBasket), 1155735, 1, 1044253);
            AddRes(index, typeof(BaseBeverage), 1046458, 1, 1044253);
            SetBeverageType(index, BeverageType.Water);
            SetNeedMaker(index, true);
            ForceNonExceptional(index);

            index = AddCraft(typeof(HotCocoaMug), 1155736, 1155738, 0.0, 28.58, typeof(CocoaLiquor), 1080007, 1, 1080006);
            AddRes(index, typeof(SackOfSugar), 1080003, 1, 1080002);
            AddRes(index, typeof(BaseBeverage), 1080011, 1, 1080010);
            SetBeverageType(index, BeverageType.Milk);
            SetNeedMaker(index, true);
            ForceNonExceptional(index);
            #endregion
        }

        private Item CraftCharydbisBait(Mobile m, CraftItem craftItem, ITool tool)
        {
            var bait = new Bait
            {
                Index = 35,
                UsesRemaining = 5
            };

            return bait;
        }
    }
}
