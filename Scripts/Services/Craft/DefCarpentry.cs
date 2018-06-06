using System;
using Server.Items;

namespace Server.Engines.Craft
{
    #region Mondain's Legacy
    public enum CarpRecipes
    {
        // stuff
        WarriorStatueSouth = 100,
        WarriorStatueEast = 101,
        SquirrelStatueSouth = 102,
        SquirrelStatueEast = 103,
        AcidProofRope = 104,
        OrnateElvenChair = 105,
        ArcaneBookshelfSouth = 106,
        ArcaneBookshelfEast = 107,
        OrnateElvenChestSouth = 108,
        ElvenDresserSouth = 109,
        ElvenDresserEast = 110,
        FancyElvenArmoire = 111,
        ArcanistsWildStaff = 112,
        AncientWildStaff = 113,
        ThornedWildStaff = 114,
        HardenedWildStaff = 115,
        TallElvenBedSouth = 116,
        TallElvenBedEast = 117,
        StoneAnvilSouth = 118,
        StoneAnvilEast = 119,
        OrnateElvenChestEast = 120,

        // arties
        PhantomStaff = 150,
        IronwoodCrown = 151,
        BrambleCoat = 152,

        KotlBlackRod = 170,
        KotlAutomaton = 171,
    }
    #endregion

    public class DefCarpentry : CraftSystem
    {
        public override SkillName MainSkill
        {
            get
            {
                return SkillName.Carpentry;
            }
        }

        public override int GumpTitleNumber
        {
            get
            {
                return 1044004;
            }// <CENTER>CARPENTRY MENU</CENTER>
        }

        public override CraftECA ECA
        {
            get
            {
                return CraftECA.ChanceMinusSixtyToFourtyFive;
            }
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefCarpentry();

                return m_CraftSystem;
            }
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.5; // 50%
        }

        private DefCarpentry()
            : base(1, 1, 1.25)// base( 1, 1, 3.0 )
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
            // no animation
            //if ( from.Body.Type == BodyType.Human && !from.Mounted )
            //	from.Animate( 9, 5, 1, true, false, 0 );
            from.PlaySound(0x23D);
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

        private System.Collections.Generic.List<Type> _NoConsumeOfFailure = new System.Collections.Generic.List<Type>
        {
            typeof(StaffOfTheMagi), typeof(BlackrockMoonstone)
        };

        public override bool ConsumeOnFailure(Mobile from, Type resourceType, CraftItem craftItem)
        {
            if (_NoConsumeOfFailure.Contains(resourceType))
                return false;

            return base.ConsumeOnFailure(from, resourceType, craftItem);
        }

        public override void InitCraftList()
        {
            int index = -1;

            // Other
            AddCraft(typeof(BarrelStaves), 1044294, 1027857, 00.0, 25.0, typeof(Board), 1044041, 5, 1044351);
            AddCraft(typeof(BarrelLid), 1044294, 1027608, 11.0, 36.0, typeof(Board), 1044041, 4, 1044351);
            AddCraft(typeof(ShortMusicStandLeft), 1044294, 1044313, 78.9, 103.9, typeof(Board), 1044041, 15, 1044351);
            AddCraft(typeof(ShortMusicStandRight), 1044294, 1044314, 78.9, 103.9, typeof(Board), 1044041, 15, 1044351);
            AddCraft(typeof(TallMusicStandLeft), 1044294, 1044315, 81.5, 106.5, typeof(Board), 1044041, 20, 1044351);
            AddCraft(typeof(TallMusicStandRight), 1044294, 1044316, 81.5, 106.5, typeof(Board), 1044041, 20, 1044351);
            AddCraft(typeof(EasleSouth), 1044294, 1044317, 86.8, 111.8, typeof(Board), 1044041, 20, 1044351);
            AddCraft(typeof(EasleEast), 1044294, 1044318, 86.8, 111.8, typeof(Board), 1044041, 20, 1044351);
            AddCraft(typeof(EasleNorth), 1044294, 1044319, 86.8, 111.8, typeof(Board), 1044041, 20, 1044351);

            if (Core.SE)
            {
                index = AddCraft(typeof(RedHangingLantern), 1044294, 1029412, 65.0, 90.0, typeof(Board), 1044041, 5, 1044351);
                AddRes(index, typeof(BlankScroll), 1044377, 10, 1044378);
                SetNeededExpansion(index, Expansion.SE);

                index = AddCraft(typeof(WhiteHangingLantern), 1044294, 1029416, 65.0, 90.0, typeof(Board), 1044041, 5, 1044351);
                AddRes(index, typeof(BlankScroll), 1044377, 10, 1044378);
                SetNeededExpansion(index, Expansion.SE);

                index = AddCraft(typeof(ShojiScreen), 1044294, 1029423, 80.0, 105.0, typeof(Board), 1044041, 75, 1044351);
                AddSkill(index, SkillName.Tailoring, 50.0, 55.0);
                AddRes(index, typeof(Cloth), 1044286, 60, 1044287);
                SetNeededExpansion(index, Expansion.SE);

                index = AddCraft(typeof(BambooScreen), 1044294, 1029428, 80.0, 105.0, typeof(Board), 1044041, 75, 1044351);
                AddSkill(index, SkillName.Tailoring, 50.0, 55.0);
                AddRes(index, typeof(Cloth), 1044286, 60, 1044287);
                SetNeededExpansion(index, Expansion.SE);
            }

            if (Core.AOS)	//Duplicate Entries to preserve ordering depending on era
            {
                index = AddCraft(typeof(FishingPole), 1044294, 1023519, 68.4, 93.4, typeof(Board), 1044041, 5, 1044351); //This is in the categor of Other during AoS
                AddSkill(index, SkillName.Tailoring, 40.0, 45.0);
                AddRes(index, typeof(Cloth), 1044286, 5, 1044287);
            }

            #region Mondain's Legacy
            if (Core.ML)
            {
                index = AddCraft(typeof(WoodenContainerEngraver), 1044294, 1072153, 75.0, 100.0, typeof(Board), 1044041, 4, 1044351);
                AddRes(index, typeof(IronIngot), 1044036, 2, 1044037);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(RunedSwitch), 1044294, 1072896, 70.0, 120.0, typeof(Board), 1044041, 2, 1044351);
                AddRes(index, typeof(EnchantedSwitch), 1072893, 1, 1053098);
                AddRes(index, typeof(RunedPrism), 1073465, 1, 1053098);
                AddRes(index, typeof(JeweledFiligree), 1072894, 1, 1053098);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(ArcanistStatueSouthDeed), 1044294, 1072885, 0.0, 35.0, typeof(Board), 1044041, 250, 1044351);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(ArcanistStatueEastDeed), 1044294, 1072886, 0.0, 35.0, typeof(Board), 1044041, 250, 1044351);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(WarriorStatueSouthDeed), 1044294, 1072887, 0.0, 35.0, typeof(Board), 1044041, 250, 1044351);
                AddRecipe(index, (int)CarpRecipes.WarriorStatueSouth);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(WarriorStatueEastDeed), 1044294, 1072888, 0.0, 35.0, typeof(Board), 1044041, 250, 1044351);
                AddRecipe(index, (int)CarpRecipes.WarriorStatueEast);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(SquirrelStatueSouthDeed), 1044294, 1072884, 0.0, 35.0, typeof(Board), 1044041, 250, 1044351);
                AddRecipe(index, (int)CarpRecipes.SquirrelStatueSouth);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(SquirrelStatueEastDeed), 1044294, 1073398, 0.0, 35.0, typeof(Board), 1044041, 250, 1044351);
                AddRecipe(index, (int)CarpRecipes.SquirrelStatueEast);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(GiantReplicaAcorn), 1044294, 1072889, 80.0, 105.0, typeof(Board), 1044041, 35, 1044351);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(MountedDreadHorn), 1044294, 1032632, 90.0, 115.0, typeof(Board), 1044041, 50, 1044351);
                AddRes(index, typeof(PristineDreadHorn), 1032634, 1, 1053098);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(AcidProofRope), 1044294, 1074886, 80, 130.0, typeof(GreaterStrengthPotion), 1073466, 2, 1044253);
                AddRes(index, typeof(ProtectionScroll), 1044395, 1, 1053098);
                AddRes(index, typeof(SwitchItem), 1032127, 1, 1053098);
                AddRecipe(index, (int)CarpRecipes.AcidProofRope);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);
            }
            #endregion

            #region SA
            if (Core.SA)
            {
                index = AddCraft(typeof(GargishBanner), 1044294, 1095312, 94.7, 115.0, typeof(Board), 1044041, 50, 1044351);
                AddSkill(index, SkillName.Tailoring, 75.0, 105.0);
                AddRes(index, typeof(Cloth), 1044286, 50, 1044287);
                SetNeededExpansion(index, Expansion.SA);

                index = AddCraft(typeof(Incubator), 1044294, 1112479, 90.0, 115.0, typeof(Board), 1044041, 100, 1044351);
                SetNeededExpansion(index, Expansion.SA);

                index = AddCraft(typeof(ChickenCoop), 1044294, 1112570, 90.0, 115.0, typeof(Board), 1044041, 150, 1044351);
                SetNeededExpansion(index, Expansion.SA);

                index = AddCraft(typeof(ExodusSummoningAlter), 1044294, 1153502, 95.0, 120.0, typeof(Board), 1044041, 100, 1044351);
                AddSkill(index, SkillName.Magery, 75.0, 120.0);
                AddRes(index, typeof(Granite), 1044607, 10, 1044253);
                AddRes(index, typeof(SmallPieceofBlackrock), 1150016, 10, 1044253);
                AddRes(index, typeof(NexusCore), 1153501, 1, 1044253);                
            }
            #endregion

            #region TOL
            if (Core.TOL)
            {
                index = AddCraft(typeof(CraftableHouseItem), 1044294, 1155849, 42.1, 77.7, typeof(Board), 1044041, 5, 1044351);
                SetData(index, CraftableItemType.DarkWoodenSignHanger);
                SetDisplayID(index, 2967);
                SetNeededExpansion(index, Expansion.TOL);

                index = AddCraft(typeof(CraftableHouseItem), 1044294, 1155850, 42.1, 77.7, typeof(Board), 1044041, 5, 1044351);
                SetData(index, CraftableItemType.LightWoodenSignHanger);
                SetDisplayID(index, 2969);
                SetNeededExpansion(index, Expansion.TOL);
            }            
            #endregion

            // Furniture
            AddCraft(typeof(FootStool), 1044291, 1022910, 11.0, 36.0, typeof(Board), 1044041, 9, 1044351);
            AddCraft(typeof(Stool), 1044291, 1022602, 11.0, 36.0, typeof(Board), 1044041, 9, 1044351);
            AddCraft(typeof(BambooChair), 1044291, 1044300, 21.0, 46.0, typeof(Board), 1044041, 13, 1044351);
            AddCraft(typeof(WoodenChair), 1044291, 1044301, 21.0, 46.0, typeof(Board), 1044041, 13, 1044351);
            AddCraft(typeof(FancyWoodenChairCushion), 1044291, 1044302, 42.1, 67.1, typeof(Board), 1044041, 15, 1044351);
            AddCraft(typeof(WoodenChairCushion), 1044291, 1044303, 42.1, 67.1, typeof(Board), 1044041, 13, 1044351);
            AddCraft(typeof(WoodenBench), 1044291, 1022860, 52.6, 77.6, typeof(Board), 1044041, 17, 1044351);
            AddCraft(typeof(WoodenThrone), 1044291, 1044304, 52.6, 77.6, typeof(Board), 1044041, 17, 1044351);
            AddCraft(typeof(Throne), 1044291, 1044305, 73.6, 98.6, typeof(Board), 1044041, 19, 1044351);
            AddCraft(typeof(Nightstand), 1044291, 1044306, 42.1, 67.1, typeof(Board), 1044041, 17, 1044351);
            AddCraft(typeof(WritingTable), 1044291, 1022890, 63.1, 88.1, typeof(Board), 1044041, 17, 1044351);
            AddCraft(typeof(LargeTable), 1044291, 1044308, 84.2, 109.2, typeof(Board), 1044041, 27, 1044351);
            AddCraft(typeof(YewWoodTable), 1044291, 1044307, 63.1, 88.1, typeof(Board), 1044041, 23, 1044351);            

            if (Core.SE)
            {
                index = AddCraft(typeof(ElegantLowTable), 1044291, 1030265, 80.0, 105.0, typeof(Board), 1044041, 35, 1044351);
                SetNeededExpansion(index, Expansion.SE);

                index = AddCraft(typeof(PlainLowTable), 1044291, 1030266, 80.0, 105.0, typeof(Board), 1044041, 35, 1044351);
                SetNeededExpansion(index, Expansion.SE);
            }

            #region Mondain's Legacy
            if (Core.ML)
            {
                index = AddCraft(typeof(OrnateElvenTableSouthDeed), 1044291, 1072869, 85.0, 110.0, typeof(Board), 1044041, 60, 1044351);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(OrnateElvenTableEastDeed), 1044291, 1073384, 85.0, 110.0, typeof(Board), 1044041, 60, 1044351);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(FancyElvenTableSouthDeed), 1044291, 1073385, 80.0, 105.0, typeof(Board), 1044041, 50, 1044351);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(FancyElvenTableEastDeed), 1044291, 1073386, 80.0, 105.0, typeof(Board), 1044041, 50, 1044351);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(ElvenPodium), 1044291, 1073399, 80.0, 105.0, typeof(Board), 1044041, 20, 1044351);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(OrnateElvenChair), 1044291, 1072870, 80.0, 105.0, typeof(Board), 1044041, 30, 1044351);
                AddRecipe(index, (int)CarpRecipes.OrnateElvenChair);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(BigElvenChair), 1044291, 1072872, 85.0, 110.0, typeof(Board), 1044041, 40, 1044351);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(ElvenReadingChair), 1044291, 1072873, 80.0, 105.0, typeof(Board), 1044041, 30, 1044351);
                SetNeededExpansion(index, Expansion.ML);
            }

            if (Core.SA)
            {
                index = AddCraft(typeof(TerMurStyleChair), 1044291, 1095291, 85.0, 110.0, typeof(Board), 1044041, 40, 1044351);
                SetNeededExpansion(index, Expansion.SA);

                index = AddCraft(typeof(TerMurStyleTable), 1044291, 1095321, 75.0, 100.0, typeof(Board), 1044041, 50, 1044351);
                SetNeededExpansion(index, Expansion.SA);
            }

            index = AddCraft(typeof(UpholsteredChairDeed), 1044291, 1154173, 70.0, 110.0, typeof(Board), 1044041, 40, 1044351);
            AddSkill(index, SkillName.Tailoring, 55.0, 60.0);
            AddRes(index, typeof(Cloth), 1044286, 12, 1044287);
            SetNeededThemePack(index, ThemePack.Kings);
            #endregion

            // Containers
            AddCraft(typeof(WoodenBox), 1044292, 1023709, 21.0, 46.0, typeof(Board), 1044041, 10, 1044351);
            AddCraft(typeof(SmallCrate), 1044292, 1044309, 10.0, 35.0, typeof(Board), 1044041, 8, 1044351);
            AddCraft(typeof(MediumCrate), 1044292, 1044310, 31.0, 56.0, typeof(Board), 1044041, 15, 1044351);
            AddCraft(typeof(LargeCrate), 1044292, 1044311, 47.3, 72.3, typeof(Board), 1044041, 18, 1044351);
            AddCraft(typeof(WoodenChest), 1044292, 1023650, 73.6, 98.6, typeof(Board), 1044041, 20, 1044351);
            AddCraft(typeof(EmptyBookcase), 1044292, 1022718, 31.5, 56.5, typeof(Board), 1044041, 25, 1044351);
            AddCraft(typeof(FancyArmoire), 1044292, 1044312, 84.2, 109.2, typeof(Board), 1044041, 35, 1044351);
            AddCraft(typeof(Armoire), 1044292, 1022643, 84.2, 109.2, typeof(Board), 1044041, 35, 1044351);

            if (Core.SE)
            {
                index = AddCraft(typeof(PlainWoodenChest), 1044292, 1030251, 90.0, 115.0, typeof(Board), 1044041, 30, 1044351);
                SetNeededExpansion(index, Expansion.SE);

                index = AddCraft(typeof(OrnateWoodenChest), 1044292, 1030253, 90.0, 115.0, typeof(Board), 1044041, 30, 1044351);
                SetNeededExpansion(index, Expansion.SE);

                index = AddCraft(typeof(GildedWoodenChest), 1044292, 1030255, 90.0, 115.0, typeof(Board), 1044041, 30, 1044351);
                SetNeededExpansion(index, Expansion.SE);

                index = AddCraft(typeof(WoodenFootLocker), 1044292, 1030257, 90.0, 115.0, typeof(Board), 1044041, 30, 1044351);
                SetNeededExpansion(index, Expansion.SE);

                index = AddCraft(typeof(FinishedWoodenChest), 1044292, 1030259, 90.0, 115.0, typeof(Board), 1044041, 30, 1044351);
                SetNeededExpansion(index, Expansion.SE);

                index = AddCraft(typeof(TallCabinet), 1044292, 1030261, 90.0, 115.0, typeof(Board), 1044041, 35, 1044351);
                SetNeededExpansion(index, Expansion.SE);

                index = AddCraft(typeof(ShortCabinet), 1044292, 1030263, 90.0, 115.0, typeof(Board), 1044041, 35, 1044351);
                SetNeededExpansion(index, Expansion.SE);

                index = AddCraft(typeof(RedArmoire), 1044292, 1030328, 90.0, 115.0, typeof(Board), 1044041, 40, 1044351);
                SetNeededExpansion(index, Expansion.SE);

                index = AddCraft(typeof(ElegantArmoire), 1044292, 1030330, 90.0, 115.0, typeof(Board), 1044041, 40, 1044351);
                SetNeededExpansion(index, Expansion.SE);

                index = AddCraft(typeof(MapleArmoire), 1044292, 1030332, 90.0, 115.0, typeof(Board), 1044041, 40, 1044351);
                SetNeededExpansion(index, Expansion.SE);

                index = AddCraft(typeof(CherryArmoire), 1044292, 1030334, 90.0, 115.0, typeof(Board), 1044041, 40, 1044351);
                SetNeededExpansion(index, Expansion.SE);
            }

            index = AddCraft(typeof(Keg), 1044292, 1023711, 57.8, 82.8, typeof(BarrelStaves), 1044288, 3, 1044253);
            AddRes(index, typeof(BarrelHoops), 1044289, 1, 1044253);
            AddRes(index, typeof(BarrelLid), 1044251, 1, 1044253);
            ForceNonExceptional(index);

            #region Mondain's Legacy
            if (Core.ML)
            {
                index = AddCraft(typeof(ArcaneBookShelfDeedSouth), 1044292, 1072871, 94.7, 119.7, typeof(Board), 1044041, 80, 1044351);
                AddRecipe(index, (int)CarpRecipes.ArcaneBookshelfSouth);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(ArcaneBookShelfDeedEast), 1044292, 1073371, 94.7, 119.7, typeof(Board), 1044041, 80, 1044351);
                AddRecipe(index, (int)CarpRecipes.ArcaneBookshelfEast);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(OrnateElvenChestSouthDeed), 1044292, 1072862, 94.7, 119.7, typeof(Board), 1044041, 40, 1044351);
                AddRecipe(index, (int)CarpRecipes.OrnateElvenChestSouth);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(OrnateElvenChestEastDeed), 1044292, 1073383, 94.7, 119.7, typeof(Board), 1044041, 40, 1044351);
                AddRecipe(index, (int)CarpRecipes.OrnateElvenChestEast);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(ElvenWashBasinSouthWithDrawerDeed), 1044292, 1072865, 70.0, 95.0, typeof(Board), 1044041, 40, 1044351);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(ElvenWashBasinEastWithDrawerDeed), 1044292, 1073387, 70.0, 95.0, typeof(Board), 1044041, 40, 1044351);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(ElvenDresserDeedSouth), 1044292, 1072864, 75.0, 100.0, typeof(Board), 1044041, 45, 1044351);
                AddRecipe(index, (int)CarpRecipes.ElvenDresserSouth);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(ElvenDresserDeedEast), 1044292, 1073388, 75.0, 100.0, typeof(Board), 1044041, 45, 1044351);
                AddRecipe(index, (int)CarpRecipes.ElvenDresserEast);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(FancyElvenArmoire), 1044292, 1072866, 80.0, 105.0, typeof(Board), 1044041, 60, 1044351);
                AddRecipe(index, (int)CarpRecipes.FancyElvenArmoire);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(SimpleElvenArmoire), 1044292, 1073401, 80.0, 105.0, typeof(Board), 1044041, 60, 1044351);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(RarewoodChest), 1044292, 1073402, 80.0, 105.0, typeof(Board), 1044041, 30, 1044351);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(DecorativeBox), 1044292, 1073403, 80.0, 105.0, typeof(Board), 1044041, 25, 1044351);
                SetNeededExpansion(index, Expansion.ML);
            }
            #endregion

            index = AddCraft(typeof(AcademicBookCase), 1044292, 1071213, 60.0, 85.0, typeof(Board), 1044041, 25, 1044351);
            AddRes(index, typeof(AcademicBooksArtifact), 1071202, 1, 1044253);

            if (Core.SA)
            {
                index = AddCraft(typeof(GargishChest), 1044292, 1095293, 80.0, 105.0, typeof(Board), 1044041, 30, 1044351);
                SetNeededExpansion(index, Expansion.SA);
            }

            AddCraft(typeof(LiquorBarrel), 1044292, 1150816, 60.0, 90.0, typeof(Board), 1044041, 50, 1044351);

            // Weapons
            AddCraft(typeof(ShepherdsCrook), 1044566, 1023713, 78.9, 103.9, typeof(Board), 1044041, 7, 1044351);
            AddCraft(typeof(QuarterStaff), 1044566, 1023721, 73.6, 98.6, typeof(Board), 1044041, 6, 1044351);
            AddCraft(typeof(GnarledStaff), 1044566, 1025112, 78.9, 103.9, typeof(Board), 1044041, 7, 1044351);

            if (Core.SE)
            {
                index = AddCraft(typeof(Bokuto), 1044566, 1030227, 70.0, 95.0, typeof(Board), 1044041, 6, 1044351);
                SetNeededExpansion(index, Expansion.SE);

                index = AddCraft(typeof(Fukiya), 1044566, 1030229, 60.0, 85.0, typeof(Board), 1044041, 6, 1044351);
                SetNeededExpansion(index, Expansion.SE);

                index = AddCraft(typeof(Tetsubo), 1044566, 1030225, 80.0, 105.0, typeof(Board), 1044041, 10, 1044351);
                SetNeededExpansion(index, Expansion.SE);
            }

            #region Mondain's Legacy
            if (Core.ML)
            {
                index = AddCraft(typeof(WildStaff), 1044566, 1031557, 63.8, 113.8, typeof(Board), 1044041, 16, 1044351);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(PhantomStaff), 1044566, 1072919, 90.0, 130.0, typeof(Board), 1044041, 16, 1044351);
                AddRes(index, typeof(DiseasedBark), 1032683, 1, 1053098);
                AddRes(index, typeof(Putrefication), 1032678, 10, 1053098);
                AddRes(index, typeof(Taint), 1032679, 10, 1053098);
                AddRecipe(index, (int)CarpRecipes.PhantomStaff);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(ArcanistsWildStaff), 1044566, 1073549, 63.8, 113.8, typeof(Board), 1044041, 16, 1044351);
                AddRes(index, typeof(WhitePearl), 1026253, 1, 1053098);
                AddRecipe(index, (int)CarpRecipes.ArcanistsWildStaff);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(AncientWildStaff), 1044566, 1073550, 63.8, 113.8, typeof(Board), 1044041, 16, 1044351);
                AddRes(index, typeof(PerfectEmerald), 1026251, 1, 1053098);
                AddRecipe(index, (int)CarpRecipes.AncientWildStaff);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(ThornedWildStaff), 1044566, 1073551, 63.8, 113.8, typeof(Board), 1044041, 16, 1044351);
                AddRes(index, typeof(FireRuby), 1026254, 1, 1053098);
                AddRecipe(index, (int)CarpRecipes.ThornedWildStaff);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(HardenedWildStaff), 1044566, 1073552, 63.8, 113.8, typeof(Board), 1044041, 16, 1044351);
                AddRes(index, typeof(Turquoise), 1026250, 1, 1053098);
                AddRecipe(index, (int)CarpRecipes.HardenedWildStaff);
                SetNeededExpansion(index, Expansion.ML);
            }
            #endregion

            #region SA
            if (Core.SA)
            {
                index = AddCraft(typeof(SerpentStoneStaff), 1044566, 1095367, 63.8, 113.8, typeof(Board), 1044041, 16, 1044351);
                AddRes(index, typeof(EcruCitrine), 1026252, 1, 1053098);
                SetNeededExpansion(index, Expansion.SA);

                index = AddCraft(typeof(GargishGnarledStaff), 1044566, 1097488, 78.9, 128.9, typeof(Board), 1044041, 16, 1044351);
                AddRes(index, typeof(EcruCitrine), 1026252, 1, 1053098);
                SetNeededExpansion(index, Expansion.SA);
            }
            #endregion

            AddCraft(typeof(Club), 1044566, 1025043, 65.0, 90.0, typeof(Board), 1044041, 9, 1044351);
            AddCraft(typeof(BlackStaff), 1044566, 1023568, 81.5, 106.5, typeof(Board), 1044041, 9, 1044351);

            if (Core.TOL)
            {
                index = AddCraft(typeof(KotlBlackRod), 1044566, 1156990, 100.0, 160.0, typeof(Board), 1044041, 20, 1044351);
                AddRes(index, typeof(BlackrockMoonstone), 1156993, 1, 1156992);
                AddRes(index, typeof(StaffOfTheMagi), 1061600, 1, 1044253);
                AddRecipe(index, (int)CarpRecipes.KotlBlackRod);
                SetNeededExpansion(index, Expansion.TOL);

                /*index = AddCraft(typeof(GargishKotlBlackRod), 1044566, 1156994, 100.0, 160.0, typeof(Board), 1044041, 20, 1044351);
                AddRes(index, typeof(BlackrockMoonstone), 1156993, 1, 1156992);
                AddRes(index, typeof(StaffOfTheMagi), 1061600, 1, 1044253);
                AddRecipe(index, (int)CarpRecipes.KotlBlackRod);
                SetNeededExpansion(index, Expansion.TOL);
                */
            }

            // Armor
            AddCraft(typeof(WoodenShield), 1062760, 1027034, 52.6, 77.6, typeof(Board), 1044041, 9, 1044351);

            #region Mondain's Legacy
            if (Core.ML)
            {
                index = AddCraft(typeof(WoodlandChest), 1062760, 1031111, 90.0, 115.0, typeof(Board), 1044041, 20, 1044351);
                AddRes(index, typeof(BarkFragment), 1032687, 6, 1053098);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(WoodlandArms), 1062760, 1031116, 80.0, 105.0, typeof(Board), 1044041, 15, 1044351);
                AddRes(index, typeof(BarkFragment), 1032687, 4, 1053098);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(WoodlandGloves), 1062760, 1031114, 85.0, 110.0, typeof(Board), 1044041, 15, 1044351);
                AddRes(index, typeof(BarkFragment), 1032687, 4, 1053098);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(WoodlandLegs), 1062760, 1031115, 85.0, 110.0, typeof(Board), 1044041, 15, 1044351);
                AddRes(index, typeof(BarkFragment), 1032687, 4, 1053098);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(WoodlandGorget), 1062760, 1031113, 85.0, 110.0, typeof(Board), 1044041, 15, 1044351);
                AddRes(index, typeof(BarkFragment), 1032687, 4, 1053098);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(RavenHelm), 1062760, 1031121, 65.0, 115.0, typeof(Board), 1044041, 10, 1044351);
                AddRes(index, typeof(BarkFragment), 1032687, 4, 1053098);
                AddRes(index, typeof(Feather), 1027123, 25, 1053098);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(VultureHelm), 1062760, 1031122, 63.9, 113.9, typeof(Board), 1044041, 10, 1044351);
                AddRes(index, typeof(BarkFragment), 1032687, 4, 1053098);
                AddRes(index, typeof(Feather), 1027123, 25, 1053098);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(WingedHelm), 1062760, 1031123, 58.4, 108.4, typeof(Board), 1044041, 10, 1044351);
                AddRes(index, typeof(BarkFragment), 1032687, 4, 1053098);
                AddRes(index, typeof(Feather), 1027123, 60, 1053098);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(IronwoodCrown), 1062760, 1072924, 85.0, 120.0, typeof(Board), 1044041, 10, 1044351);
                AddRes(index, typeof(DiseasedBark), 1032683, 1, 1053098);
                AddRes(index, typeof(Corruption), 1032676, 10, 1053098);
                AddRes(index, typeof(Putrefication), 1032678, 10, 1053098);
                AddRecipe(index, (int)CarpRecipes.IronwoodCrown);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(BrambleCoat), 1062760, 1072925, 85.0, 120.0, typeof(Board), 1044041, 10, 1044351);
                AddRes(index, typeof(DiseasedBark), 1032683, 1, 1053098);
                AddRes(index, typeof(Taint), 1032679, 10, 1053098);
                AddRes(index, typeof(Scourge), 1032677, 10, 1053098);
                AddRecipe(index, (int)CarpRecipes.BrambleCoat);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(DarkwoodCrown), 1062760, 1073481, 85.0, 120.0, typeof(Board), 1044041, 10, 1044351);
                AddRes(index, typeof(LardOfParoxysmus), 1032681, 1, 1053098);
                AddRes(index, typeof(Blight), 1032675, 10, 1053098);
                AddRes(index, typeof(Taint), 1032679, 10, 1053098);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(DarkwoodChest), 1062760, 1073482, 85.0, 120.0, typeof(Board), 1044041, 20, 1044351);
                AddRes(index, typeof(DreadHornMane), 1032682, 1, 1053098);
                AddRes(index, typeof(Corruption), 1032676, 10, 1053098);
                AddRes(index, typeof(Muculent), 1032680, 10, 1053098);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(DarkwoodGorget), 1062760, 1073483, 85.0, 120.0, typeof(Board), 1044041, 15, 1044351);
                AddRes(index, typeof(DiseasedBark), 1032683, 1, 1053098);
                AddRes(index, typeof(Blight), 1032675, 10, 1053098);
                AddRes(index, typeof(Scourge), 1032677, 10, 1053098);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(DarkwoodLegs), 1062760, 1073484, 85.0, 120.0, typeof(Board), 1044041, 15, 1044351);
                AddRes(index, typeof(GrizzledBones), 1032684, 1, 1053098);
                AddRes(index, typeof(Corruption), 1032676, 10, 1053098);
                AddRes(index, typeof(Putrefication), 1072137, 10, 1053098);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(DarkwoodPauldrons), 1062760, 1073485, 85.0, 120.0, typeof(Board), 1044041, 15, 1044351);
                AddRes(index, typeof(EyeOfTheTravesty), 1032685, 1, 1053098);
                AddRes(index, typeof(Scourge), 1032677, 10, 1053098);
                AddRes(index, typeof(Taint), 1032679, 10, 1053098);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(DarkwoodGloves), 1062760, 1073486, 85.0, 120.0, typeof(Board), 1044041, 15, 1044351);
                AddRes(index, typeof(CapturedEssence), 1032686, 1, 1053098);
                AddRes(index, typeof(Putrefication), 1032678, 10, 1053098);
                AddRes(index, typeof(Muculent), 1032680, 10, 1053098);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);
            }
            #endregion

            #region SA
            index = AddCraft(typeof(GargishWoodenShield), 1062760, 1095768, 52.6, 77.6, typeof(Board), 1044041, 9, 1044351);
            SetNeededExpansion(index, Expansion.SA);
            #endregion

            // Instruments
            index = AddCraft(typeof(LapHarp), 1044293, 1023762, 63.1, 88.1, typeof(Board), 1044041, 20, 1044351);
            AddSkill(index, SkillName.Musicianship, 45.0, 50.0);
            AddRes(index, typeof(Cloth), 1044286, 10, 1044287);

            index = AddCraft(typeof(Harp), 1044293, 1023761, 78.9, 103.9, typeof(Board), 1044041, 35, 1044351);
            AddSkill(index, SkillName.Musicianship, 45.0, 50.0);
            AddRes(index, typeof(Cloth), 1044286, 15, 1044287);

            index = AddCraft(typeof(Drums), 1044293, 1023740, 57.8, 82.8, typeof(Board), 1044041, 20, 1044351);
            AddSkill(index, SkillName.Musicianship, 45.0, 50.0);
            AddRes(index, typeof(Cloth), 1044286, 10, 1044287);

            index = AddCraft(typeof(Lute), 1044293, 1023763, 68.4, 93.4, typeof(Board), 1044041, 25, 1044351);
            AddSkill(index, SkillName.Musicianship, 45.0, 50.0);
            AddRes(index, typeof(Cloth), 1044286, 10, 1044287);

            index = AddCraft(typeof(Tambourine), 1044293, 1023741, 57.8, 82.8, typeof(Board), 1044041, 15, 1044351);
            AddSkill(index, SkillName.Musicianship, 45.0, 50.0);
            AddRes(index, typeof(Cloth), 1044286, 10, 1044287);

            index = AddCraft(typeof(TambourineTassel), 1044293, 1044320, 57.8, 82.8, typeof(Board), 1044041, 15, 1044351);
            AddSkill(index, SkillName.Musicianship, 45.0, 50.0);
            AddRes(index, typeof(Cloth), 1044286, 15, 1044287);

            if (Core.SE)
            {
                index = AddCraft(typeof(BambooFlute), 1044293, 1030247, 80.0, 105.0, typeof(Board), 1044041, 15, 1044351);
                AddSkill(index, SkillName.Musicianship, 45.0, 50.0);
                SetNeededExpansion(index, Expansion.SE);
            }

            if (Core.SA)
            {
                index = AddCraft(typeof(AudChar), 1044293, 1095315, 78.9, 103.9, typeof(Board), 1044041, 35, 1044351);
                AddSkill(index, SkillName.Musicianship, 45.0, 50.0);
                AddRes(index, typeof(Granite), 1044514, 3, 1044513);
                SetNeededExpansion(index, Expansion.SA);

                index = AddCraft(typeof(SnakeCharmerFlute), 1044293, 1112174, 80.0, 105.0, typeof(Board), 1044041, 15, 1044351);
                AddSkill(index, SkillName.Musicianship, 45.0, 50.0);
                AddRes(index, typeof(LuminescentFungi), 1073475, 3, 1044253);
                SetNeededExpansion(index, Expansion.SA);
            }

            index = AddCraft(typeof(CelloDeed), 1044293, 1098390, 75.0, 105.0, typeof(Board), 1044041, 15, 1044351);
            AddSkill(index, SkillName.Musicianship, 45.0, 50.0);
            AddRes(index, typeof(Cloth), 1044286, 5, 1044287);
            SetNeededThemePack(index, ThemePack.Kings);

            index = AddCraft(typeof(WallMountedBellSouthDeed), 1044293, 1154162, 75.0, 105.0, typeof(Board), 1044041, 50, 1044351);
            AddSkill(index, SkillName.Musicianship, 45.0, 50.0);
            AddRes(index, typeof(IronIngot), 1044036, 50, 1044037);
            SetNeededThemePack(index, ThemePack.Kings);

            index = AddCraft(typeof(WallMountedBellEastDeed), 1044293, 1154163, 75.0, 105.0, typeof(Board), 1044041, 50, 1044351);
            AddSkill(index, SkillName.Musicianship, 45.0, 50.0);
            AddRes(index, typeof(IronIngot), 1044036, 50, 1044037);
            SetNeededThemePack(index, ThemePack.Kings);

            index = AddCraft(typeof(TrumpetDeed), 1044293, 1098388, 85.0, 105.0, typeof(Board), 1044041, 10, 1044351);
            AddSkill(index, SkillName.Musicianship, 45.0, 50.0);
            AddRes(index, typeof(IronIngot), 1044036, 15, 1044037);
            SetNeededThemePack(index, ThemePack.Kings);

            index = AddCraft(typeof(CowBellDeed), 1044293, 1098418, 85.0, 105.0, typeof(Board), 1044041, 10, 1044351);
            AddSkill(index, SkillName.Musicianship, 45.0, 50.0);
            AddRes(index, typeof(IronIngot), 1044036, 15, 1044037);
            SetNeededThemePack(index, ThemePack.Kings);

            // Misc
            if (Core.AOS)
            {
                AddCraft(typeof(PlayerBBEast), 1044290, 1062420, 85.0, 110.0, typeof(Board), 1044041, 50, 1044351);
                AddCraft(typeof(PlayerBBSouth), 1044290, 1062421, 85.0, 110.0, typeof(Board), 1044041, 50, 1044351);
            }

            #region Mondain's Legacy
            if (Core.ML)
            {
                index = AddCraft(typeof(ParrotPerchAddonDeed), 1044290, 1072617, 50.0, 85.0, typeof(Board), 1044041, 100, 1044351);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(ArcaneCircleDeed), 1044290, 1072703, 94.7, 119.7, typeof(Board), 1044041, 100, 1044351);
                AddRes(index, typeof(BlueDiamond), 1026255, 2, 1053098);
                AddRes(index, typeof(PerfectEmerald), 1026251, 2, 1053098);
                AddRes(index, typeof(FireRuby), 1026254, 2, 1053098);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

				index = AddCraft(typeof(TallElvenBedSouthDeed), 1044290, 1072858, 94.7, 119.7, typeof(Board), 1044041, 200, 1044351);
                AddSkill(index, SkillName.Tailoring, 75.0, 80.0);
                AddRes(index, typeof(Cloth), 1044286, 100, 1044287);
                AddRecipe(index, (int)CarpRecipes.TallElvenBedSouth);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(TallElvenBedEastDeed), 1044290, 1072859, 94.7, 119.7, typeof(Board), 1044041, 200, 1044351);
                AddSkill(index, SkillName.Tailoring, 75.0, 80.0);
                AddRes(index, typeof(Cloth), 1044286, 100, 1044287);
                AddRecipe(index, (int)CarpRecipes.TallElvenBedEast);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(ElvenBedSouthDeed), 1044290, 1072860, 94.7, 119.7, typeof(Board), 1044041, 100, 1044351);
                AddRes(index, typeof(Cloth), 1044286, 100, 1044287);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(ElvenBedEastDeed), 1044290, 1072861, 94.7, 119.7, typeof(Board), 1044041, 100, 1044351);
                AddRes(index, typeof(Cloth), 1044286, 100, 1044287);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(ElvenLoveseatSouthDeed), 1044290, 1072867, 80.0, 105.0, typeof(Board), 1044041, 50, 1044351);
                SetDisplayID(index, 0x2DDF);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(ElvenLoveseatEastDeed), 1044290, 1073372, 80.0, 105.0, typeof(Board), 1044041, 50, 1044351);
                SetDisplayID(index, 0x2DE0);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(AlchemistTableSouthDeed), 1044290, 1073396, 85.0, 110.0, typeof(Board), 1044041, 70, 1044351);
                SetDisplayID(index, 0x2DD4);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(AlchemistTableEastDeed), 1044290, 1073397, 85.0, 110.0, typeof(Board), 1044041, 70, 1044351);
                SetDisplayID(index, 0x2DD3);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);
            }
            #endregion

            index = AddCraft(typeof(SmallBedSouthDeed), 1044290, 1044321, 94.7, 119.8, typeof(Board), 1044041, 100, 1044351);
            AddSkill(index, SkillName.Tailoring, 75.0, 80.0);
            AddRes(index, typeof(Cloth), 1044286, 100, 1044287);

            index = AddCraft(typeof(SmallBedEastDeed), 1044290, 1044322, 94.7, 119.8, typeof(Board), 1044041, 100, 1044351);
            AddSkill(index, SkillName.Tailoring, 75.0, 80.0);
            AddRes(index, typeof(Cloth), 1044286, 100, 1044287);

            index = AddCraft(typeof(LargeBedSouthDeed), 1044290, 1044323, 94.7, 119.8, typeof(Board), 1044041, 150, 1044351);
            AddSkill(index, SkillName.Tailoring, 75.0, 80.0);
            AddRes(index, typeof(Cloth), 1044286, 150, 1044287);

            index = AddCraft(typeof(LargeBedEastDeed), 1044290, 1044324, 94.7, 119.8, typeof(Board), 1044041, 150, 1044351);
            AddSkill(index, SkillName.Tailoring, 75.0, 80.0);
            AddRes(index, typeof(Cloth), 1044286, 150, 1044287);

            AddCraft(typeof(DartBoardSouthDeed), 1044290, 1044325, 15.7, 40.7, typeof(Board), 1044041, 5, 1044351);
            AddCraft(typeof(DartBoardEastDeed), 1044290, 1044326, 15.7, 40.7, typeof(Board), 1044041, 5, 1044351);
            AddCraft(typeof(BallotBoxDeed), 1044290, 1044327, 47.3, 72.3, typeof(Board), 1044041, 5, 1044351);

            index = AddCraft(typeof(PentagramDeed), 1044290, 1044328, 100.0, 125.0, typeof(Board), 1044041, 100, 1044351);
            AddSkill(index, SkillName.Magery, 75.0, 80.0);
            AddRes(index, typeof(IronIngot), 1044036, 40, 1044037);

            index = AddCraft(typeof(AbbatoirDeed), 1044290, 1044329, 100.0, 125.0, typeof(Board), 1044041, 100, 1044351);
            AddSkill(index, SkillName.Magery, 50.0, 55.0);
            AddRes(index, typeof(IronIngot), 1044036, 40, 1044037);

            if (Core.SA)
            {
                index = AddCraft(typeof(GargishCouchEastDeed), 1044290, 1111776, 90.0, 115.0, typeof(Board), 1044041, 75, 1044351);
                SetNeededExpansion(index, Expansion.SA);

                index = AddCraft(typeof(GargishCouchSouthDeed), 1044290, 1111775, 90.0, 115.0, typeof(Board), 1044041, 75, 1044351);
                SetNeededExpansion(index, Expansion.SA);

                index = AddCraft(typeof(LongTableSouthDeed), 1044290, 1111781, 90.0, 115.0, typeof(Board), 1044041, 80, 1044351);
                SetNeededExpansion(index, Expansion.SA);

                index = AddCraft(typeof(LongTableEastDeed), 1044290, 1111782, 90.0, 115.0, typeof(Board), 1044041, 80, 1044351);
                SetNeededExpansion(index, Expansion.SA);
           
                index = AddCraft(typeof(TerMurDresserEastDeed), 1044290, 1111784, 90.0, 115.0, typeof(Board), 1044041, 60, 1044351);
                SetNeededExpansion(index, Expansion.SA);

                index = AddCraft(typeof(TerMurDresserSouthDeed), 1044290, 1111783, 90.0, 115.0, typeof(Board), 1044041, 60, 1044351);
                SetNeededExpansion(index, Expansion.SA);
            }

            index = AddCraft(typeof(RusticBenchSouthDeed), 1044290, 1150593, 94.7, 119.8, typeof(Board), 1044041, 35, 1044351);
            SetNeededThemePack(index, ThemePack.Rustic);

            index = AddCraft(typeof(RusticBenchEastDeed), 1044290, 1150594, 94.7, 119.8, typeof(Board), 1044041, 35, 1044351);
            SetNeededThemePack(index, ThemePack.Rustic);

            index = AddCraft(typeof(PlainWoodenShelfSouthDeed), 1044290, 1154160, 40.0, 90.0, typeof(Board), 1044041, 15, 1044351);
            SetNeededThemePack(index, ThemePack.Kings);

            index = AddCraft(typeof(PlainWoodenShelfEastDeed), 1044290, 1154161, 40.0, 90.0, typeof(Board), 1044041, 15, 1044351);
            SetNeededThemePack(index, ThemePack.Kings);

            index = AddCraft(typeof(FancyWoodenShelfSouthDeed), 1044290, 1154158, 40.0, 90.0, typeof(Board), 1044041, 15, 1044351);
            SetNeededThemePack(index, ThemePack.Kings);

            index = AddCraft(typeof(FancyWoodenShelfEastDeed), 1044290, 1154159, 40.0, 90.0, typeof(Board), 1044041, 15, 1044351);
            SetNeededThemePack(index, ThemePack.Kings);

            index = AddCraft(typeof(FancyLoveseatSouthDeed), 1044290, 1154137, 70.0, 120.0, typeof(Board), 1044041, 80, 1044351);
            AddSkill(index, SkillName.Tailoring, 55.0, 60.0);
            AddRes(index, typeof(Cloth), 1044286, 24, 1044287);
            SetNeededThemePack(index, ThemePack.Kings);

            index = AddCraft(typeof(FancyLoveseatEastDeed), 1044290, 1154138, 70.0, 120.0, typeof(Board), 1044041, 80, 1044351);
            AddSkill(index, SkillName.Tailoring, 55.0, 60.0);
            AddRes(index, typeof(Cloth), 1044286, 24, 1044287);
            SetNeededThemePack(index, ThemePack.Kings);

            index = AddCraft(typeof(FancyCouchSouthDeed), 1044290, 1154139, 70.0, 120.0, typeof(Board), 1044041, 80, 1044351);
            AddSkill(index, SkillName.Tailoring, 55.0, 60.0);
            AddRes(index, typeof(Cloth), 1044286, 48, 1044287);
            SetNeededThemePack(index, ThemePack.Kings);

            index = AddCraft(typeof(FancyCouchEastDeed), 1044290, 1154140, 70.0, 120.0, typeof(Board), 1044041, 80, 1044351);
            AddSkill(index, SkillName.Tailoring, 55.0, 60.0);
            AddRes(index, typeof(Cloth), 1044286, 48, 1044287);
            SetNeededThemePack(index, ThemePack.Kings);

            index = AddCraft(typeof(PlushLoveseatSouthDeed), 1044290, 1154135, 70.0, 120.0, typeof(Board), 1044041, 80, 1044351);
            AddSkill(index, SkillName.Tailoring, 55.0, 60.0);
            AddRes(index, typeof(Cloth), 1044286, 24, 1044287);
            SetNeededThemePack(index, ThemePack.Kings);

            index = AddCraft(typeof(PlushLoveseatEastDeed), 1044290, 1154136, 70.0, 120.0, typeof(Board), 1044041, 80, 1044351);
            AddSkill(index, SkillName.Tailoring, 55.0, 60.0);
            AddRes(index, typeof(Cloth), 1044286, 24, 1044287);
            SetNeededThemePack(index, ThemePack.Kings);

            index = AddCraft(typeof(PlantTapestrySouthDeed), 1044290, 1154146, 85.0, 110.0, typeof(Board), 1044041, 12, 1044351);
            AddSkill(index, SkillName.Tailoring, 75.0, 80.0);
            AddRes(index, typeof(Cloth), 1044286, 50, 1044287);
            SetNeededThemePack(index, ThemePack.Kings);

            index = AddCraft(typeof(PlantTapestryEastDeed), 1044290, 1154147, 85.0, 110.0, typeof(Board), 1044041, 12, 1044351);
            AddSkill(index, SkillName.Tailoring, 75.0, 80.0);
            AddRes(index, typeof(Cloth), 1044286, 50, 1044287);
            SetNeededThemePack(index, ThemePack.Kings);

            index = AddCraft(typeof(MetalTableSouthDeed), 1044290, 1154154, 80.0, 105.0, typeof(Board), 1044041, 20, 1044351);
            AddSkill(index, SkillName.Tinkering, 75.0, 80.0);
            AddRes(index, typeof(IronIngot), 1044036, 15, 1044037);
            SetNeededThemePack(index, ThemePack.Kings);

            index = AddCraft(typeof(MetalTableEastDeed), 1044290, 1154155, 80.0, 105.0, typeof(Board), 1044041, 20, 1044351);
            AddSkill(index, SkillName.Tinkering, 75.0, 80.0);
            AddRes(index, typeof(IronIngot), 1044036, 15, 1044037);
            SetNeededThemePack(index, ThemePack.Kings);

            index = AddCraft(typeof(LongMetalTableSouthDeed), 1044290, 1154164, 80.0, 105.0, typeof(Board), 1044041, 40, 1044351);
            AddSkill(index, SkillName.Tinkering, 75.0, 80.0);
            AddRes(index, typeof(IronIngot), 1044036, 30, 1044037);
            SetNeededThemePack(index, ThemePack.Kings);

            index = AddCraft(typeof(LongMetalTableEastDeed), 1044290, 1154165, 80.0, 105.0, typeof(Board), 1044041, 40, 1044351);
            AddSkill(index, SkillName.Tinkering, 75.0, 80.0);
            AddRes(index, typeof(IronIngot), 1044036, 30, 1044037);
            SetNeededThemePack(index, ThemePack.Kings);

            index = AddCraft(typeof(WoodenTableSouthDeed), 1044290, 1154156, 80.0, 105.0, typeof(Board), 1044041, 20, 1044351);
            SetNeededThemePack(index, ThemePack.Kings);

            index = AddCraft(typeof(WoodenTableEastDeed), 1044290, 1154157, 80.0, 105.0, typeof(Board), 1044041, 20, 1044351);
            SetNeededThemePack(index, ThemePack.Kings);

            index = AddCraft(typeof(LongWoodenTableSouthDeed), 1044290, 1154166, 80.0, 105.0, typeof(Board), 1044041, 80, 1044351);
            SetNeededThemePack(index, ThemePack.Kings);

            index = AddCraft(typeof(LongWoodenTableEastDeed), 1044290, 1154167, 80.0, 105.0, typeof(Board), 1044041, 80, 1044351);
            SetNeededThemePack(index, ThemePack.Kings);

            index = AddCraft(typeof(SmallDisplayCaseSouthDeed), 1044290, 1155842, 95.0, 120.0, typeof(Board), 1044041, 40, 1044351);
            AddSkill(index, SkillName.Tinkering, 75.0, 80.0);
            AddRes(index, typeof(IronIngot), 1044036, 10, 1044037);
            SetNeededExpansion(index, Expansion.TOL);

            index = AddCraft(typeof(SmallDisplayCaseEastDeed), 1044290, 1155843, 95.0, 120.0, typeof(Board), 1044041, 40, 1044351);
            AddSkill(index, SkillName.Tinkering, 75.0, 80.0);
            AddRes(index, typeof(IronIngot), 1044036, 10, 1044037);
            SetNeededExpansion(index, Expansion.TOL);

            index = AddCraft(typeof(FancyLoveseatNorthDeed), 1044290, 1156560, 70.0, 120.0, typeof(Board), 1044041, 80, 1044351);
            AddSkill(index, SkillName.Tailoring, 55.0, 60.0);
            AddRes(index, typeof(Cloth), 1044286, 48, 1044287);
            SetNeededThemePack(index, ThemePack.Kings);

            index = AddCraft(typeof(FancyLoveseatWestDeed), 1044290, 1156561, 70.0, 120.0, typeof(Board), 1044041, 80, 1044351);
            AddSkill(index, SkillName.Tailoring, 55.0, 60.0);
            AddRes(index, typeof(Cloth), 1044286, 48, 1044287);
            SetNeededThemePack(index, ThemePack.Kings);

            index = AddCraft(typeof(FancyCouchNorthDeed), 1044290, 1156582, 70.0, 120.0, typeof(Board), 1044041, 80, 1044351);
            AddSkill(index, SkillName.Tailoring, 55.0, 60.0);
            AddRes(index, typeof(Cloth), 1044286, 48, 1044287);
            SetNeededThemePack(index, ThemePack.Kings);

            index = AddCraft(typeof(FancyCouchWestDeed), 1044290, 1156583, 70.0, 120.0, typeof(Board), 1044041, 80, 1044351);
            AddSkill(index, SkillName.Tailoring, 55.0, 60.0);
            AddRes(index, typeof(Cloth), 1044286, 48, 1044287);
            SetNeededThemePack(index, ThemePack.Kings);

            // Tailoring and Cooking
            index = AddCraft(typeof(DressformFront), 1044298, 1044339, 63.1, 88.1, typeof(Board), 1044041, 25, 1044351);
            AddSkill(index, SkillName.Tailoring, 65.0, 70.0);
            AddRes(index, typeof(Cloth), 1044286, 10, 1044287);

            index = AddCraft(typeof(DressformSide), 1044298, 1044340, 63.1, 88.1, typeof(Board), 1044041, 25, 1044351);
            AddSkill(index, SkillName.Tailoring, 65.0, 70.0);
            AddRes(index, typeof(Cloth), 1044286, 10, 1044287);

            #region Mondain's Legacy
            if (Core.ML)
            {
                index = AddCraft(typeof(ElvenSpinningwheelEastDeed), 1044298, 1073393, 75.0, 100.0, typeof(Board), 1044041, 60, 1044351);
                AddSkill(index, SkillName.Tailoring, 65.0, 85.0);
                AddRes(index, typeof(Cloth), 1044286, 40, 1044287);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(ElvenSpinningwheelSouthDeed), 1044298, 1072878, 75.0, 100.0, typeof(Board), 1044041, 60, 1044351);
                AddSkill(index, SkillName.Tailoring, 65.0, 85.0);
                AddRes(index, typeof(Cloth), 1044286, 40, 1044287);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(ElvenStoveSouthDeed), 1044298, 1073394, 85.0, 110.0, typeof(Board), 1044041, 80, 1044351);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);

                index = AddCraft(typeof(ElvenStoveEastDeed), 1044298, 1073395, 85.0, 110.0, typeof(Board), 1044041, 80, 1044351);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);
            }
            #endregion

            index = AddCraft(typeof(SpinningwheelEastDeed), 1044298, 1044341, 73.6, 98.6, typeof(Board), 1044041, 75, 1044351);
            AddSkill(index, SkillName.Tailoring, 65.0, 70.0);
            AddRes(index, typeof(Cloth), 1044286, 25, 1044287);

            index = AddCraft(typeof(SpinningwheelSouthDeed), 1044298, 1044342, 73.6, 98.6, typeof(Board), 1044041, 75, 1044351);
            AddSkill(index, SkillName.Tailoring, 65.0, 70.0);
            AddRes(index, typeof(Cloth), 1044286, 25, 1044287);

            index = AddCraft(typeof(LoomEastDeed), 1044298, 1044343, 84.2, 109.2, typeof(Board), 1044041, 85, 1044351);
            AddSkill(index, SkillName.Tailoring, 65.0, 70.0);
            AddRes(index, typeof(Cloth), 1044286, 25, 1044287);

            index = AddCraft(typeof(LoomSouthDeed), 1044298, 1044344, 84.2, 109.2, typeof(Board), 1044041, 85, 1044351);
            AddSkill(index, SkillName.Tailoring, 65.0, 70.0);
            AddRes(index, typeof(Cloth), 1044286, 25, 1044287);            

            index = AddCraft(typeof(StoneOvenEastDeed), 1044298, 1044345, 68.4, 93.4, typeof(Board), 1044041, 85, 1044351);
            AddSkill(index, SkillName.Tinkering, 50.0, 55.0);
            AddRes(index, typeof(IronIngot), 1044036, 125, 1044037);

            index = AddCraft(typeof(StoneOvenSouthDeed), 1044298, 1044346, 68.4, 93.4, typeof(Board), 1044041, 85, 1044351);
            AddSkill(index, SkillName.Tinkering, 50.0, 55.0);
            AddRes(index, typeof(IronIngot), 1044036, 125, 1044037);

            index = AddCraft(typeof(FlourMillEastDeed), 1044298, 1044347, 94.7, 119.7, typeof(Board), 1044041, 100, 1044351);
            AddSkill(index, SkillName.Tinkering, 50.0, 55.0);
            AddRes(index, typeof(IronIngot), 1044036, 50, 1044037);

            index = AddCraft(typeof(FlourMillSouthDeed), 1044298, 1044348, 94.7, 119.7, typeof(Board), 1044041, 100, 1044351);
            AddSkill(index, SkillName.Tinkering, 50.0, 55.0);
            AddRes(index, typeof(IronIngot), 1044036, 50, 1044037);

            AddCraft(typeof(WaterTroughEastDeed), 1044298, 1044349, 94.7, 119.7, typeof(Board), 1044041, 150, 1044351);
            AddCraft(typeof(WaterTroughSouthDeed), 1044298, 1044350, 94.7, 119.7, typeof(Board), 1044041, 150, 1044351);

            // Anvils and Forges
            #region Mondain's Legacy
            if (Core.ML)
            {
                index = AddCraft(typeof(ElvenForgeDeed), 1111809, 1072875, 94.7, 119.7, typeof(Board), 1044041, 200, 1044351);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.ML);
            }
            #endregion

            #region SA
            if (Core.SA)
            {
                index = AddCraft(typeof(SoulForgeDeed), 1111809, 1095827, 100.0, 200.0, typeof(Board), 1044041, 150, 1044351);
                AddSkill(index, SkillName.Imbuing, 75.0, 80.0);
                AddRes(index, typeof(IronIngot), 1044036, 150, 1044037);
                AddRes(index, typeof(RelicFragment), 1031699, 1, 1044253);
                ForceNonExceptional(index);
                SetNeededExpansion(index, Expansion.SA);
            }
            #endregion

            index = AddCraft(typeof(SmallForgeDeed), 1111809, 1044330, 73.6, 98.6, typeof(Board), 1044041, 5, 1044351);
            AddSkill(index, SkillName.Blacksmith, 75.0, 80.0);
            AddRes(index, typeof(IronIngot), 1044036, 75, 1044037);

            index = AddCraft(typeof(LargeForgeEastDeed), 1111809, 1044331, 78.9, 103.9, typeof(Board), 1044041, 5, 1044351);
            AddSkill(index, SkillName.Blacksmith, 80.0, 85.0);
            AddRes(index, typeof(IronIngot), 1044036, 100, 1044037);

            index = AddCraft(typeof(LargeForgeSouthDeed), 1111809, 1044332, 78.9, 103.9, typeof(Board), 1044041, 5, 1044351);
            AddSkill(index, SkillName.Blacksmith, 80.0, 85.0);
            AddRes(index, typeof(IronIngot), 1044036, 100, 1044037);

            index = AddCraft(typeof(AnvilEastDeed), 1111809, 1044333, 73.6, 98.6, typeof(Board), 1044041, 5, 1044351);
            AddSkill(index, SkillName.Blacksmith, 75.0, 80.0);
            AddRes(index, typeof(IronIngot), 1044036, 150, 1044037);

            index = AddCraft(typeof(AnvilSouthDeed), 1111809, 1044334, 73.6, 98.6, typeof(Board), 1044041, 5, 1044351);
            AddSkill(index, SkillName.Blacksmith, 75.0, 80.0);
            AddRes(index, typeof(IronIngot), 1044036, 150, 1044037);

            // Training
            index = AddCraft(typeof(TrainingDummyEastDeed), 1044297, 1044335, 68.4, 93.4, typeof(Board), 1044041, 55, 1044351);
            AddSkill(index, SkillName.Tailoring, 50.0, 55.0);
            AddRes(index, typeof(Cloth), 1044286, 60, 1044287);

            index = AddCraft(typeof(TrainingDummySouthDeed), 1044297, 1044336, 68.4, 93.4, typeof(Board), 1044041, 55, 1044351);
            AddSkill(index, SkillName.Tailoring, 50.0, 55.0);
            AddRes(index, typeof(Cloth), 1044286, 60, 1044287);

            index = AddCraft(typeof(PickpocketDipEastDeed), 1044297, 1044337, 73.6, 98.6, typeof(Board), 1044041, 65, 1044351);
            AddSkill(index, SkillName.Tailoring, 50.0, 55.0);
            AddRes(index, typeof(Cloth), 1044286, 60, 1044287);

            index = AddCraft(typeof(PickpocketDipSouthDeed), 1044297, 1044338, 73.6, 98.6, typeof(Board), 1044041, 65, 1044351);
            AddSkill(index, SkillName.Tailoring, 50.0, 55.0);
            AddRes(index, typeof(Cloth), 1044286, 60, 1044287);

            MarkOption = true;
            Repair = Core.AOS;
            CanEnhance = Core.ML;

            SetSubRes(typeof(Board), 1072643);

            // Add every material you want the player to be able to choose from
            // This will override the overridable material	TODO: Verify the required skill amount
            AddSubRes(typeof(Board), 1072643, 00.0, 1044041, 1072652);
            AddSubRes(typeof(OakBoard), 1072644, 65.0, 1044041, 1072652);
            AddSubRes(typeof(AshBoard), 1072645, 80.0, 1044041, 1072652);
            AddSubRes(typeof(YewBoard), 1072646, 95.0, 1044041, 1072652);
            AddSubRes(typeof(HeartwoodBoard), 1072647, 100.0, 1044041, 1072652);
            AddSubRes(typeof(BloodwoodBoard), 1072648, 100.0, 1044041, 1072652);
            AddSubRes(typeof(FrostwoodBoard), 1072649, 100.0, 1044041, 1072652);
        }
    }
}
