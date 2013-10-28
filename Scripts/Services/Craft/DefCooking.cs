using System;
using Server.Items;

namespace Server.Engines.Craft
{
    #region Recipes
    public enum CookRecipes
    {
        // magical
        RotWormStew = 500
    }
    #endregion

    public class DefCooking : CraftSystem
    {
        public override SkillName MainSkill
        {
            get
            {
                return SkillName.Cooking;
            }
        }

        public override int GumpTitleNumber
        {
            get
            {
                return 1044003;
            }// <CENTER>COOKING MENU</CENTER>
        }

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

        public override CraftECA ECA
        {
            get
            {
                return CraftECA.ChanceMinusSixtyToFourtyFive;
            }
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.0; // 0%
        }

        private DefCooking()
            : base(1, 1, 1.25)// base( 1, 1, 1.5 )
        {
        }

        public override int CanCraft(Mobile from, BaseTool tool, Type itemType)
        {
            if (tool == null || tool.Deleted || tool.UsesRemaining < 0)
                return 1044038; // You have worn out your tool!
            else if (!BaseTool.CheckAccessible(tool, from))
                return 1044263; // The tool must be on your person to use.

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

            /* Begin Ingredients */
            index = this.AddCraft(typeof(SackFlour), 1044495, 1024153, 0.0, 100.0, typeof(WheatSheaf), 1044489, 2, 1044490);
            this.SetNeedMill(index, true);

            index = this.AddCraft(typeof(Dough), 1044495, 1024157, 0.0, 100.0, typeof(SackFlour), 1044468, 1, 1044253);
            this.AddRes(index, typeof(BaseBeverage), 1046458, 1, 1044253);

            index = this.AddCraft(typeof(SweetDough), 1044495, 1041340, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            this.AddRes(index, typeof(JarHoney), 1044472, 1, 1044253);

            index = this.AddCraft(typeof(CakeMix), 1044495, 1041002, 0.0, 100.0, typeof(SackFlour), 1044468, 1, 1044253);
            this.AddRes(index, typeof(SweetDough), 1044475, 1, 1044253);

            index = this.AddCraft(typeof(CookieMix), 1044495, 1024159, 0.0, 100.0, typeof(JarHoney), 1044472, 1, 1044253);
            this.AddRes(index, typeof(SweetDough), 1044475, 1, 1044253);

            if (Core.ML)
            {
                index = this.AddCraft(typeof(CocoaButter), 1044495, 1079998, 0.0, 100.0, typeof(CocoaPulp), 1080530, 1, 1044253);
                this.SetItemHue(index, 0x457);
                this.SetNeededExpansion(index, Expansion.ML);
                this.SetNeedOven(index, true);

                index = this.AddCraft(typeof(CocoaLiquor), 1044495, 1079999, 0.0, 100.0, typeof(CocoaPulp), 1080530, 1, 1044253);
                this.AddRes(index, typeof(EmptyPewterBowl), 1025629, 1, 1044253);
                this.SetItemHue(index, 0x46A);
                this.SetNeededExpansion(index, Expansion.ML);
                this.SetNeedOven(index, true);
            }
            /* End Ingredients */

            /* Begin Preparations */
            index = this.AddCraft(typeof(UnbakedQuiche), 1044496, 1041339, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            this.AddRes(index, typeof(Eggs), 1044477, 1, 1044253);

            // TODO: This must also support chicken and lamb legs
            index = this.AddCraft(typeof(UnbakedMeatPie), 1044496, 1041338, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            this.AddRes(index, typeof(RawRibs), 1044482, 1, 1044253);

            index = this.AddCraft(typeof(UncookedSausagePizza), 1044496, 1041337, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            this.AddRes(index, typeof(Sausage), 1044483, 1, 1044253);

            index = this.AddCraft(typeof(UncookedCheesePizza), 1044496, 1041341, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            this.AddRes(index, typeof(CheeseWheel), 1044486, 1, 1044253);

            index = this.AddCraft(typeof(UnbakedFruitPie), 1044496, 1041334, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            this.AddRes(index, typeof(Pear), 1044481, 1, 1044253);

            index = this.AddCraft(typeof(UnbakedPeachCobbler), 1044496, 1041335, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            this.AddRes(index, typeof(Peach), 1044480, 1, 1044253);

            index = this.AddCraft(typeof(UnbakedApplePie), 1044496, 1041336, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            this.AddRes(index, typeof(Apple), 1044479, 1, 1044253);

            index = this.AddCraft(typeof(UnbakedPumpkinPie), 1044496, 1041342, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            this.AddRes(index, typeof(Pumpkin), 1044484, 1, 1044253);

            index = this.AddCraft(typeof(WoodPulp), 1044496, 1113136, 60.0, 100.0, typeof(BarkFragment), 1032687, 1, 1044253);
            this.AddRes(index, typeof(BaseBeverage), 1046458, 1, 1044253);

            if (Core.SE)
            {
                index = this.AddCraft(typeof(GreenTea), 1044496, 1030315, 80.0, 130.0, typeof(GreenTeaBasket), 1030316, 1, 1044253);
                this.AddRes(index, typeof(BaseBeverage), 1046458, 1, 1044253);
                this.SetNeededExpansion(index, Expansion.SE);
                this.SetNeedOven(index, true);

                index = this.AddCraft(typeof(WasabiClumps), 1044496, 1029451, 70.0, 120.0, typeof(BaseBeverage), 1046458, 1, 1044253);
                this.AddRes(index, typeof(WoodenBowlOfPeas), 1025633, 3, 1044253);
                this.SetNeededExpansion(index, Expansion.SE);

                index = this.AddCraft(typeof(SushiRolls), 1044496, 1030303, 90.0, 120.0, typeof(BaseBeverage), 1046458, 1, 1044253);
                this.AddRes(index, typeof(RawFishSteak), 1044476, 10, 1044253);
                this.SetNeededExpansion(index, Expansion.SE);

                index = this.AddCraft(typeof(SushiPlatter), 1044496, 1030305, 90.0, 120.0, typeof(BaseBeverage), 1046458, 1, 1044253);
                this.AddRes(index, typeof(RawFishSteak), 1044476, 10, 1044253);
                this.SetNeededExpansion(index, Expansion.SE);
            }

            index = this.AddCraft(typeof(TribalPaint), 1044496, 1040000, Core.ML ? 55.0 : 80.0, Core.ML ? 105.0 : 80.0, typeof(SackFlour), 1044468, 1, 1044253);
            this.AddRes(index, typeof(TribalBerry), 1046460, 1, 1044253);

            if (Core.SE)
            {
                index = this.AddCraft(typeof(EggBomb), 1044496, 1030249, 90.0, 120.0, typeof(Eggs), 1044477, 1, 1044253);
                this.AddRes(index, typeof(SackFlour), 1044468, 3, 1044253);
                this.SetNeededExpansion(index, Expansion.SE);
            }

            #region Mondain's Legacy
            if (Core.ML)
            {
                index = this.AddCraft(typeof(ParrotWafer), 1044496, 1032246, 37.5, 87.5, typeof(Dough), 1044469, 1, 1044253);
                this.AddRes(index, typeof(JarHoney), 1044472, 1, 1044253);
                this.AddRes(index, typeof(RawFishSteak), 1044476, 10, 1044253);
                this.SetNeededExpansion(index, Expansion.ML);
            }
            #endregion
            /* End Preparations */

            /* Begin Baking */
            index = this.AddCraft(typeof(BreadLoaf), 1044497, 1024156, 0.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
            this.SetNeedOven(index, true);

            index = this.AddCraft(typeof(Cookies), 1044497, 1025643, 0.0, 100.0, typeof(CookieMix), 1044474, 1, 1044253);
            this.SetNeedOven(index, true);

            index = this.AddCraft(typeof(Cake), 1044497, 1022537, 0.0, 100.0, typeof(CakeMix), 1044471, 1, 1044253);
            this.SetNeedOven(index, true);

            index = this.AddCraft(typeof(Muffins), 1044497, 1022539, 0.0, 100.0, typeof(SweetDough), 1044475, 1, 1044253);
            this.SetNeedOven(index, true);

            index = this.AddCraft(typeof(Quiche), 1044497, 1041345, 0.0, 100.0, typeof(UnbakedQuiche), 1044518, 1, 1044253);
            this.SetNeedOven(index, true);

            index = this.AddCraft(typeof(MeatPie), 1044497, 1041347, 0.0, 100.0, typeof(UnbakedMeatPie), 1044519, 1, 1044253);
            this.SetNeedOven(index, true);

            index = this.AddCraft(typeof(SausagePizza), 1044497, 1044517, 0.0, 100.0, typeof(UncookedSausagePizza), 1044520, 1, 1044253);
            this.SetNeedOven(index, true);

            index = this.AddCraft(typeof(CheesePizza), 1044497, 1044516, 0.0, 100.0, typeof(UncookedCheesePizza), 1044521, 1, 1044253);
            this.SetNeedOven(index, true);

            index = this.AddCraft(typeof(FruitPie), 1044497, 1041346, 0.0, 100.0, typeof(UnbakedFruitPie), 1044522, 1, 1044253);
            this.SetNeedOven(index, true);

            index = this.AddCraft(typeof(PeachCobbler), 1044497, 1041344, 0.0, 100.0, typeof(UnbakedPeachCobbler), 1044523, 1, 1044253);
            this.SetNeedOven(index, true);

            index = this.AddCraft(typeof(ApplePie), 1044497, 1041343, 0.0, 100.0, typeof(UnbakedApplePie), 1044524, 1, 1044253);
            this.SetNeedOven(index, true);

            index = this.AddCraft(typeof(PumpkinPie), 1044497, 1041348, 0.0, 100.0, typeof(UnbakedPumpkinPie), 1046461, 1, 1044253);
            this.SetNeedOven(index, true);

            if (Core.SE)
            {
                index = this.AddCraft(typeof(MisoSoup), 1044497, 1030317, 60.0, 110.0, typeof(RawFishSteak), 1044476, 1, 1044253);
                this.AddRes(index, typeof(BaseBeverage), 1046458, 1, 1044253);
                this.SetNeededExpansion(index, Expansion.SE);
                this.SetNeedOven(index, true);

                index = this.AddCraft(typeof(WhiteMisoSoup), 1044497, 1030318, 60.0, 110.0, typeof(RawFishSteak), 1044476, 1, 1044253);
                this.AddRes(index, typeof(BaseBeverage), 1046458, 1, 1044253);
                this.SetNeededExpansion(index, Expansion.SE);
                this.SetNeedOven(index, true);

                index = this.AddCraft(typeof(RedMisoSoup), 1044497, 1030319, 60.0, 110.0, typeof(RawFishSteak), 1044476, 1, 1044253);
                this.AddRes(index, typeof(BaseBeverage), 1046458, 1, 1044253);
                this.SetNeededExpansion(index, Expansion.SE);
                this.SetNeedOven(index, true);

                index = this.AddCraft(typeof(AwaseMisoSoup), 1044497, 1030320, 60.0, 110.0, typeof(RawFishSteak), 1044476, 1, 1044253);
                this.AddRes(index, typeof(BaseBeverage), 1046458, 1, 1044253);
                this.SetNeededExpansion(index, Expansion.SE);
                this.SetNeedOven(index, true);
            }
            /* End Baking */

            /* Begin Barbecue */
            index = this.AddCraft(typeof(CookedBird), 1044498, 1022487, 0.0, 100.0, typeof(RawBird), 1044470, 1, 1044253);
            this.SetNeedHeat(index, true);
            this.SetUseAllRes(index, true);

            index = this.AddCraft(typeof(ChickenLeg), 1044498, 1025640, 0.0, 100.0, typeof(RawChickenLeg), 1044473, 1, 1044253);
            this.SetNeedHeat(index, true);
            this.SetUseAllRes(index, true);

            index = this.AddCraft(typeof(FishSteak), 1044498, 1022427, 0.0, 100.0, typeof(RawFishSteak), 1044476, 1, 1044253);
            this.SetNeedHeat(index, true);
            this.SetUseAllRes(index, true);

            index = this.AddCraft(typeof(FriedEggs), 1044498, 1022486, 0.0, 100.0, typeof(Eggs), 1044477, 1, 1044253);
            this.SetNeedHeat(index, true);
            this.SetUseAllRes(index, true);

            index = this.AddCraft(typeof(LambLeg), 1044498, 1025642, 0.0, 100.0, typeof(RawLambLeg), 1044478, 1, 1044253);
            this.SetNeedHeat(index, true);
            this.SetUseAllRes(index, true);

            index = this.AddCraft(typeof(Ribs), 1044498, 1022546, 0.0, 100.0, typeof(RawRibs), 1044485, 1, 1044253);
            this.SetNeedHeat(index, true);
            this.SetUseAllRes(index, true);
            /* End Barbecue */

            /* Begin Chocolatiering */
            if (Core.ML)
            {
                index = this.AddCraft(typeof(DarkChocolate), 1080001, 1079994, 15.0, 100.0, typeof(SackOfSugar), 1079997, 1, 1044253);
                this.AddRes(index, typeof(CocoaButter), 1079998, 1, 1044253);
                this.AddRes(index, typeof(CocoaLiquor), 1079999, 1, 1044253);
                this.SetItemHue(index, 0x465);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(MilkChocolate), 1080001, 1079995, 32.5, 107.5, typeof(SackOfSugar), 1079997, 1, 1044253);
                this.AddRes(index, typeof(CocoaButter), 1079998, 1, 1044253);
                this.AddRes(index, typeof(CocoaLiquor), 1079999, 1, 1044253);
                this.AddRes(index, typeof(BaseBeverage), 1022544, 1, 1044253);
                this.SetBeverageType(index, BeverageType.Milk);
                this.SetItemHue(index, 0x461);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(WhiteChocolate), 1080001, 1079996, 52.5, 127.5, typeof(SackOfSugar), 1079997, 1, 1044253);
                this.AddRes(index, typeof(CocoaButter), 1079998, 1, 1044253);
                this.AddRes(index, typeof(Vanilla), 1080000, 1, 1044253);
                this.AddRes(index, typeof(BaseBeverage), 1022544, 1, 1044253);
                this.SetBeverageType(index, BeverageType.Milk);
                this.SetItemHue(index, 0x47E);
                this.SetNeededExpansion(index, Expansion.ML);
            }
            /* End Chocolatiering */

            #region Mondain's Legacy
            /* Begin Enchanted */
            if (Core.ML)
            {
                index = this.AddCraft(typeof(FoodEngraver), 1073108, 1072951, 75.0, 100.0, typeof(Dough), 1044469, 1, 1044253);
                this.AddRes(index, typeof(JarHoney), 1044472, 1, 1044253);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(EnchantedApple), 1073108, 1072952, 60.0, 110.0, typeof(Apple), 1044479, 1, 1044253);
                this.AddRes(index, typeof(GreaterHealPotion), 1073467, 1, 1044253);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(WrathGrapes), 1073108, 1072953, 95.0, 145.0, typeof(Grapes), 1073468, 1, 1044253);
                this.AddRes(index, typeof(GreaterStrengthPotion), 1073466, 1, 1044253);
                this.SetNeededExpansion(index, Expansion.ML);

                index = this.AddCraft(typeof(FruitBowl), 1073108, 1072950, 55.0, 105.0, typeof(EmptyWoodenBowl), 1073472, 1, 1044253);
                this.AddRes(index, typeof(Pear), 1044481, 3, 1044253);
                this.AddRes(index, typeof(Apple), 1044479, 3, 1044253);
                this.AddRes(index, typeof(Banana), 1073470, 3, 1044253);
                this.SetNeededExpansion(index, Expansion.ML);
            }
            /* End Enchanted */
            #endregion

            #region Stygian Abyss
            /* Plant Pigments*/

            if (Core.SA)
            {
                index = this.AddCraft(typeof(PlantPigment), "Plant Pigments", 1112132, 33.0, 83.0, typeof(PlantClippings), 1112131, 1, 1044253);
                this.AddRes(index, typeof(Bottle), 1023854, 1, 1044253);
                this.SetNeededExpansion(index, Expansion.SA);

                index = this.AddCraft(typeof(NaturalDye), "Plant Pigments", 1112136, 75.0, 100.0, typeof(PlantPigment), 1112132, 1, 1044253);
                this.AddRes(index, typeof(ColorFixative), 1112135, 1, 1044253);
                this.SetNeededExpansion(index, Expansion.SA);

                index = this.AddCraft(typeof(ColorFixative), "Plant Pigments", 1112135, 75.0, 100.0, typeof(SilverSerpentVenom), 1112173, 1, 1044253);
                this.AddRes(index, typeof(BaseBeverage), 1044476, 1, 1044253);//TODO correct Consumption of BaseBeverage...
                this.SetNeededExpansion(index, Expansion.SA);
            }
            #endregion
        }
    }
}