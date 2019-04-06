using System;
using System.Collections;
using Server.Commands;

namespace Server.Items
{
    public class StealableArtifactsSpawner : Item
    { 
        private static readonly StealableEntry[] m_Entries = new StealableEntry[]
        {            
            // Doom - Artifact rarity 1
            new StealableEntry(Map.Malas, new Point3D(317, 56, -1), 72, 108, typeof(RockArtifact)),
            new StealableEntry(Map.Malas, new Point3D(360, 31, 8), 72, 108, typeof(SkullCandleArtifact)),
            new StealableEntry(Map.Malas, new Point3D(369, 372, -1), 72, 108, typeof(BottleArtifact)),
            new StealableEntry(Map.Malas, new Point3D(378, 372, 0), 72, 108, typeof(DamagedBooksArtifact)),
            // Doom - Artifact rarity 2
            new StealableEntry(Map.Malas, new Point3D(432, 16, -1), 144, 216, typeof(StretchedHideArtifact)),
            new StealableEntry(Map.Malas, new Point3D(489, 9, 0), 144, 216, typeof(BrazierArtifact)),
            // Doom - Artifact rarity 3
            new StealableEntry(Map.Malas, new Point3D(471, 96, -1), 288, 432, typeof(LampPostArtifact), GetLampPostHue()),
            new StealableEntry(Map.Malas, new Point3D(421, 198, 2), 288, 432, typeof(BooksNorthArtifact)),
            new StealableEntry(Map.Malas, new Point3D(431, 189, -1), 288, 432, typeof(BooksWestArtifact)),
            new StealableEntry(Map.Malas, new Point3D(435, 196, -1), 288, 432, typeof(BooksFaceDownArtifact)),
            // Doom - Artifact rarity 5
            new StealableEntry(Map.Malas, new Point3D(447, 9, 8), 1152, 1728, typeof(StuddedLeggingsArtifact)),
            new StealableEntry(Map.Malas, new Point3D(423, 28, 0), 1152, 1728, typeof(EggCaseArtifact)),
            new StealableEntry(Map.Malas, new Point3D(347, 44, 4), 1152, 1728, typeof(SkinnedGoatArtifact)),
            new StealableEntry(Map.Malas, new Point3D(497, 57, -1), 1152, 1728, typeof(GruesomeStandardArtifact)),
            new StealableEntry(Map.Malas, new Point3D(381, 375, 11), 1152, 1728, typeof(BloodyWaterArtifact)),
            new StealableEntry(Map.Malas, new Point3D(489, 369, 2), 1152, 1728, typeof(TarotCardsArtifact)),
            new StealableEntry(Map.Malas, new Point3D(497, 369, 5), 1152, 1728, typeof(BackpackArtifact)),
            // Doom - Artifact rarity 7
            new StealableEntry(Map.Malas, new Point3D(475, 23, 4), 4608, 6912, typeof(StuddedTunicArtifact)),
            new StealableEntry(Map.Malas, new Point3D(423, 28, 0), 4608, 6912, typeof(CocoonArtifact)),
            // Doom - Artifact rarity 8
            new StealableEntry(Map.Malas, new Point3D(354, 36, -1), 9216, 13824, typeof(SkinnedDeerArtifact)),
            #region New Doom Artifacts
			new StealableEntry(Map.Malas, new Point3D(274, 231, 0), 9216, 13824, typeof(HangingPlatemailTunic)),
            new StealableEntry(Map.Malas, new Point3D(445, 119, -1), 9216, 13824, typeof(HangingPlatemailLeggings)),
            new StealableEntry(Map.Malas, new Point3D(421, 197, -1), 9216, 13824, typeof(HangingPlatemailArms)),
			#endregion
            // Doom - Artifact rarity 9
            new StealableEntry(Map.Malas, new Point3D(433, 11, -1), 18432, 27648, typeof(SaddleArtifact)),
            new StealableEntry(Map.Malas, new Point3D(403, 31, 4), 18432, 27648, typeof(LeatherTunicArtifact)),
            #region New Doom Artifacts
			new StealableEntry(Map.Malas, new Point3D(378, 371, -1), 18432, 27648, typeof(ArtifactBookshelf)),
            new StealableEntry(Map.Malas, new Point3D(487, 364, -1), 18432, 27648, typeof(ArcaneTable)),
			#endregion
            // Doom - Artifact rarity 10
            new StealableEntry(Map.Malas, new Point3D(257, 70, -2), 36864, 55296, typeof(ZyronicClaw)),
            new StealableEntry(Map.Malas, new Point3D(354, 176, 7), 36864, 55296, typeof(TitansHammer)),
            new StealableEntry(Map.Malas, new Point3D(369, 389, -1), 36864, 55296, typeof(BladeOfTheRighteous)),
            new StealableEntry(Map.Malas, new Point3D(467, 92, 4), 36864, 55296, typeof(InquisitorsResolution)),
            // Doom - Artifact rarity 12
            new StealableEntry(Map.Malas, new Point3D(487, 364, -1), 147456, 221184, typeof(RuinedPaintingArtifact)),
            #region New Doom Artifacts
			new StealableEntry(Map.Malas, new Point3D(365, 7, -1), 147456, 221184, typeof(IncenseBurner)),
			#endregion

            // Yomotsu Mines - Artifact rarity 1
            new StealableEntry(Map.Malas, new Point3D(18, 110, -1), 72, 108, typeof(Basket1Artifact)),
            new StealableEntry(Map.Malas, new Point3D(66, 114, -1), 72, 108, typeof(Basket2Artifact)),
            // Yomotsu Mines - Artifact rarity 2
            new StealableEntry(Map.Malas, new Point3D(63, 12, 11), 144, 216, typeof(Basket4Artifact)),
            new StealableEntry(Map.Malas, new Point3D(5, 29, -1), 144, 216, typeof(Basket5NorthArtifact)),
            new StealableEntry(Map.Malas, new Point3D(30, 81, 3), 144, 216, typeof(Basket5WestArtifact)),
            // Yomotsu Mines - Artifact rarity 3
            new StealableEntry(Map.Malas, new Point3D(115, 7, -1), 288, 432, typeof(Urn1Artifact)),
            new StealableEntry(Map.Malas, new Point3D(85, 13, -1), 288, 432, typeof(Urn2Artifact)),
            new StealableEntry(Map.Malas, new Point3D(110, 53, -1), 288, 432, typeof(Sculpture1Artifact)),
            new StealableEntry(Map.Malas, new Point3D(108, 37, -1), 288, 432, typeof(Sculpture2Artifact)),
            new StealableEntry(Map.Malas, new Point3D(121, 14, -1), 288, 432, typeof(TeapotNorthArtifact)),
            new StealableEntry(Map.Malas, new Point3D(121, 115, -1), 288, 432, typeof(TeapotWestArtifact)),
            new StealableEntry(Map.Malas, new Point3D(84, 40, -1), 288, 432, typeof(TowerLanternArtifact)),
            // Yomotsu Mines - Artifact rarity 9
            new StealableEntry(Map.Malas, new Point3D(94, 7, -1), 18432, 27648, typeof(ManStatuetteSouthArtifact)),

            // Fan Dancer's Dojo - Artifact rarity 1
            new StealableEntry(Map.Malas, new Point3D(113, 640, -2), 72, 108, typeof(Basket3NorthArtifact)),
            new StealableEntry(Map.Malas, new Point3D(102, 355, -1), 72, 108, typeof(Basket3WestArtifact)),
            // Fan Dancer's Dojo - Artifact rarity 2
            new StealableEntry(Map.Malas, new Point3D(99, 370, -1), 144, 216, typeof(Basket6Artifact)),
            new StealableEntry(Map.Malas, new Point3D(100, 357, -1), 144, 216, typeof(ZenRock1Artifact)),
            // Fan Dancer's Dojo - Artifact rarity 3
            new StealableEntry(Map.Malas, new Point3D(73, 473, -1), 288, 432, typeof(FanNorthArtifact)),
            new StealableEntry(Map.Malas, new Point3D(99, 372, -1), 288, 432, typeof(FanWestArtifact)),
            new StealableEntry(Map.Malas, new Point3D(92, 326, -1), 288, 432, typeof(BowlsVerticalArtifact)),
            new StealableEntry(Map.Malas, new Point3D(97, 470, -1), 288, 432, typeof(ZenRock2Artifact)),
            new StealableEntry(Map.Malas, new Point3D(103, 691, -1), 288, 432, typeof(ZenRock3Artifact)),
            // Fan Dancer's Dojo - Artifact rarity 4
            new StealableEntry(Map.Malas, new Point3D(103, 336, 4), 576, 864, typeof(Painting1NorthArtifact)),
            new StealableEntry(Map.Malas, new Point3D(59, 381, 4), 576, 864, typeof(Painting1WestArtifact)),
            new StealableEntry(Map.Malas, new Point3D(84, 401, 2), 576, 864, typeof(Painting2NorthArtifact)),
            new StealableEntry(Map.Malas, new Point3D(59, 392, 2), 576, 864, typeof(Painting2WestArtifact)),
            new StealableEntry(Map.Malas, new Point3D(107, 483, -1), 576, 864, typeof(TripleFanNorthArtifact)),
            new StealableEntry(Map.Malas, new Point3D(50, 475, -1), 576, 864, typeof(TripleFanWestArtifact)),
            new StealableEntry(Map.Malas, new Point3D(107, 460, -1), 576, 864, typeof(BowlArtifact)),
            new StealableEntry(Map.Malas, new Point3D(90, 502, -1), 576, 864, typeof(CupsArtifact)),
            new StealableEntry(Map.Malas, new Point3D(107, 688, -1), 576, 864, typeof(BowlsHorizontalArtifact)),
            new StealableEntry(Map.Malas, new Point3D(112, 676, -1), 576, 864, typeof(SakeArtifact)),
            // Fan Dancer's Dojo - Artifact rarity 5
            new StealableEntry(Map.Malas, new Point3D(135, 614, -1), 1152, 1728, typeof(SwordDisplay1NorthArtifact)),
            new StealableEntry(Map.Malas, new Point3D(50, 482, -1), 1152, 1728, typeof(SwordDisplay1WestArtifact)),
            new StealableEntry(Map.Malas, new Point3D(119, 672, -1), 1152, 1728, typeof(Painting3Artifact)),
            // Fan Dancer's Dojo - Artifact rarity 6
            new StealableEntry(Map.Malas, new Point3D(90, 326, -1), 2304, 3456, typeof(Painting4NorthArtifact)),
            new StealableEntry(Map.Malas, new Point3D(99, 354, -1), 2304, 3456, typeof(Painting4WestArtifact)),
            new StealableEntry(Map.Malas, new Point3D(179, 652, -1), 2304, 3456, typeof(SwordDisplay2NorthArtifact)),
            new StealableEntry(Map.Malas, new Point3D(118, 627, -1), 2304, 3456, typeof(SwordDisplay2WestArtifact)),
            // Fan Dancer's Dojo - Artifact rarity 7
            new StealableEntry(Map.Malas, new Point3D(90, 483, -1), 4608, 6912, typeof(FlowersArtifact)),
            // Fan Dancer's Dojo - Artifact rarity 8
            new StealableEntry(Map.Malas, new Point3D(71, 562, -1), 9216, 13824, typeof(DolphinLeftArtifact)),
            new StealableEntry(Map.Malas, new Point3D(102, 677, -1), 9216, 13824, typeof(DolphinRightArtifact)),
            new StealableEntry(Map.Malas, new Point3D(61, 499, 0), 9216, 13824, typeof(SwordDisplay3SouthArtifact)),
            new StealableEntry(Map.Malas, new Point3D(182, 669, -1), 9216, 13824, typeof(SwordDisplay3EastArtifact)),
            new StealableEntry(Map.Malas, new Point3D(162, 647, -1), 9216, 13824, typeof(SwordDisplay4WestArtifact)),
            new StealableEntry(Map.Malas, new Point3D(124, 624, 0), 9216, 13824, typeof(Painting5NorthArtifact)),
            new StealableEntry(Map.Malas, new Point3D(146, 649, 2), 9216, 13824, typeof(Painting5WestArtifact)),
            // Fan Dancer's Dojo - Artifact rarity 9
            new StealableEntry(Map.Malas, new Point3D(100, 488, -1), 18432, 27648, typeof(SwordDisplay4NorthArtifact)),
            new StealableEntry(Map.Malas, new Point3D(175, 606, 0), 18432, 27648, typeof(SwordDisplay5NorthArtifact)),
            new StealableEntry(Map.Malas, new Point3D(157, 608, -1), 18432, 27648, typeof(SwordDisplay5WestArtifact)),
            new StealableEntry(Map.Malas, new Point3D(187, 643, 1), 18432, 27648, typeof(Painting6NorthArtifact)),
            new StealableEntry(Map.Malas, new Point3D(146, 623, 1), 18432, 27648, typeof(Painting6WestArtifact)),
            new StealableEntry(Map.Malas, new Point3D(178, 629, -1), 18432, 27648, typeof(ManStatuetteEastArtifact)),

            // Abyss - Artifact rarity 5
	        new StealableEntry(Map.TerMur, new Point3D(717, 416, 50), 1152, 1728, typeof(DyingPlantArtifact)),
	        new StealableEntry(Map.TerMur, new Point3D(951, 542, -14), 1152, 1728, typeof(LargePewterBowlArtifact)),
			new StealableEntry(Map.TerMur, new Point3D(877, 527, -13), 1152, 1728, typeof(CrownOfArcaneTemperament)),
			new StealableEntry(Map.TerMur, new Point3D(345, 621, 26), 1152, 1728, typeof(LightInTheVoid)),
			new StealableEntry(Map.TerMur, new Point3D(585, 853, -45), 1152, 1728, typeof(StaffOfResonance)),
			new StealableEntry(Map.TerMur, new Point3D(843, 665, 27), 1152, 1728, typeof(ValkyriesGlaive)),
	        // Abyss - Artifact rarity 6
	        new StealableEntry(Map.TerMur, new Point3D(785, 442, -15), 2304, 3456, typeof(LargeDyingPlantArtifact)),
	        new StealableEntry(Map.TerMur, new Point3D(849, 281, -6), 2304, 3456, typeof(GargishLuckTotemArtifact)),
	        new StealableEntry(Map.TerMur, new Point3D(916, 374, -6), 2304, 3456, typeof(BookOfTruthArtifact)),
	        new StealableEntry(Map.TerMur, new Point3D(669, 819, -108), 2304, 3456, typeof(GargishTraditionalVaseArtifact)),
	        new StealableEntry(Map.TerMur, new Point3D(715, 782, 27), 2304, 3456, typeof(GargishProtectiveTotemArtifact)),
	        // Abyss - Artifact rarity 7
	        new StealableEntry(Map.TerMur, new Point3D(368, 605, 26), 4608, 6912, typeof(GargishBentasVaseArtifact)),
	        new StealableEntry(Map.TerMur, new Point3D(670, 441, 50), 4608, 6912, typeof(GargishPortraitArtifact)),
	        new StealableEntry(Map.TerMur, new Point3D(555, 670, 55), 4608, 6912, typeof(GargishKnowledgeTotemArtifact)),
	        new StealableEntry(Map.TerMur, new Point3D(948, 393, 88), 4608, 6912, typeof(GargishMemorialStatueArtifact)),
	        // Abyss - Artifact rarity 8
	        new StealableEntry(Map.TerMur, new Point3D(926, 598, -5), 9216, 13824, typeof(PushmePullyuArtifact)),
	        // UnderWorld - Artifact rarity 3
	        new StealableEntry(Map.TerMur, new Point3D(1046, 1106, -63), 288, 432, typeof(MysteriousSupperArtifact)),
	        new StealableEntry(Map.TerMur, new Point3D(1239, 1019, -37), 288, 432, typeof(JugsOfGoblinRotgutArtifact)),
	        // UnderWorld - Artifact rarity 4
	        new StealableEntry(Map.TerMur, new Point3D(1015, 1013, -35), 576, 864, typeof(StolenBottlesOfLiquor1Artifact)), // [2a]
	        new StealableEntry(Map.TerMur, new Point3D(1015, 1029, -35), 576, 864, typeof(StolenBottlesOfLiquor2Artifact)), // [2b]
	        new StealableEntry(Map.TerMur, new Point3D(1210, 1035, -22), 576, 864, typeof(BottlesOfSpoiledWine1Artifact)), // [2]
	        new StealableEntry(Map.TerMur, new Point3D(1077, 975, -23), 576, 864, typeof(NaverysWeb1Artifact)), // [1]
	        new StealableEntry(Map.TerMur, new Point3D(1094, 990, -23), 576, 864, typeof(NaverysWeb2Artifact)), // [2]
	        // UnderWorld - Artifact rarity 5
	        new StealableEntry(Map.TerMur, new Point3D(1049, 1109, -65), 1152, 1728, typeof(BloodySpoonArtifact)),
            new StealableEntry(Map.TerMur, new Point3D(1047, 1108, -65), 1152, 1728, typeof(MysticsGuard)),
            new StealableEntry(Map.TerMur, new Point3D(1137, 1134, -38), 1152, 1728, typeof(RemnantsOfMeatLoafArtifact)),
	        new StealableEntry(Map.TerMur, new Point3D(1134, 1204, 7), 1152, 1728, typeof(HalfEatenSupperArtifact)),
	        new StealableEntry(Map.TerMur, new Point3D(1083, 983, -19), 1152, 1728, typeof(NaverysWeb3Artifact)), // [3]
	        new StealableEntry(Map.TerMur, new Point3D(1081, 992, -21), 1152, 1728, typeof(NaverysWeb4Artifact)), // [4]
	        new StealableEntry(Map.TerMur, new Point3D(1146, 1011, -52), 1152, 1728, typeof(NaverysWeb5Artifact)), // [2]
	        new StealableEntry(Map.TerMur, new Point3D(1119, 974, -41), 1152, 1728, typeof(NaverysWeb6Artifact)), // [1]
	        // UnderWorld - Artifact rarity 6
	        new StealableEntry(Map.TerMur, new Point3D(1015, 1018, -35), 2304, 3456, typeof(BatteredPanArtifact)),
	        new StealableEntry(Map.TerMur, new Point3D(1007, 975, -22), 2304, 3456, typeof(RustedPanArtifact)),
	        new StealableEntry(Map.TerMur, new Point3D(1188, 1015, -35), 2304, 3456, typeof(BottlesOfSpoiledWine2Artifact)),
	        // UnderWorld - Artifact rarity 7
	        new StealableEntry(Map.TerMur, new Point3D(1015, 1026, -35), 4608, 6912, typeof(StolenBottlesOfLiquor3Artifact)),
	        new StealableEntry(Map.TerMur, new Point3D(1226, 963, -22), 4608, 6912, typeof(BottlesOfSpoiledWine3Artifact)),
	        new StealableEntry(Map.TerMur, new Point3D(1089, 1126, -36), 4608, 6912, typeof(DriedUpInkWellArtifact)),
	        new StealableEntry(Map.TerMur, new Point3D(1227, 964, -29), 4608, 6912, typeof(FakeCopperIngotsArtifact)),
	        // UnderWorld - Artifact rarity 8
	        new StealableEntry(Map.TerMur, new Point3D(1031, 998, -38), 9216, 13824, typeof(StolenBottlesOfLiquor4Artifact)),
	        new StealableEntry(Map.TerMur, new Point3D(1017, 1150, -64), 9216, 13824, typeof(RottedOarsArtifact)),
	        new StealableEntry(Map.TerMur, new Point3D(1226, 966, -29), 9216, 13824, typeof(PricelessTreasureArtifact)),
	        // UnderWorld - Artifact rarity 9
	        new StealableEntry(Map.TerMur, new Point3D(1066, 1193, -36), 18432, 27648, typeof(TyballsFlaskStandArtifact)),
	        new StealableEntry(Map.TerMur, new Point3D(1131, 1128, -42), 18432, 27648, typeof(BlockAndTackleArtifact)),

            //Ararat Stealables (Exploring the Deep) - Artifact rarity 8
            new StealableEntry(Map.Trammel, new Point3D(6430, 1637, 0), 9216, 13824, typeof(SternAnchorOfBmvArarat)),
            new StealableEntry(Map.Trammel, new Point3D(6423, 1658, 0), 9216, 13824, typeof(ShipsBellOfBmvArarat)),
            new StealableEntry(Map.Trammel, new Point3D(6405, 1640, 0), 9216, 13824, typeof(FigureheadOfBmvArarat)),

            //TOL - Artifact rarity 11 - does not show rarity on item
            new StealableEntry(Map.TerMur, new Point3D(538, 1496, 40), 36864, 55296, typeof(StretchedDinosaurHide)),
            new StealableEntry(Map.TerMur, new Point3D(174, 1808, 80), 36864, 55296, typeof(CarvedMyrmydexGlyph)),
            new StealableEntry(Map.TerMur, new Point3D(630, 1689, 100), 36864, 55296, typeof(WakuOnASpit)),
            new StealableEntry(Map.TerMur, new Point3D(688, 2125, 45), 36864, 55296, typeof(SacredLavaRock)),
            new StealableEntry(Map.TerMur, new Point3D(393, 1838, 0), 36864, 55296, typeof(WhiteTigerFigurine)),
            new StealableEntry(Map.TerMur, new Point3D(421, 1571, 40), 36864, 55296, typeof(DragonTurtleHatchlingNet)),

            // Wrong - Artifact rarity 8
            new StealableEntry(Map.Trammel, new Point3D(5790, 579, 10), 18432, 27648, typeof(BlanketOfDarkness)),
            new StealableEntry(Map.Felucca, new Point3D(5790, 579, 10), 18432, 27648, typeof(BlanketOfDarkness)),
            new StealableEntry(Map.Trammel, new Point3D(5865, 559, 15), 18432, 27648, typeof(BlanketOfDarkness)),
            new StealableEntry(Map.Felucca, new Point3D(5865, 559, 15), 18432, 27648, typeof(BlanketOfDarkness)),
            new StealableEntry(Map.Trammel, new Point3D(5832, 576, 10), 18432, 27648, typeof(BlanketOfDarkness)),
            new StealableEntry(Map.Felucca, new Point3D(5832, 576, 10), 18432, 27648, typeof(BlanketOfDarkness)),

			// Wrong - Arfifact rarity 10
            new StealableEntry(Map.Trammel, new Point3D(5703, 521, 0), 36864, 55296, typeof(TortureRackSouth)),
            new StealableEntry(Map.Felucca, new Point3D(5703, 521, 0), 36864, 55296, typeof(TortureRackSouth)),
            new StealableEntry(Map.Trammel, new Point3D(5680, 537, 0), 46864, 55296, typeof(TortureRackEast)),
            new StealableEntry(Map.Felucca, new Point3D(5680, 537, 0), 46864, 55296, typeof(TortureRackEast)),

            // Bedlam - Artifact Rarity 8
            new StealableEntry(Map.Malas, new Point3D(168, 1609, 0), 9216, 13824, typeof(AcademicBooksArtifact)),
            new StealableEntry(Map.Malas, new Point3D(165, 1650, 0), 9216, 13824, typeof(AcademicBooksArtifact)),
            new StealableEntry(Map.Malas, new Point3D(85, 1644, 20), 9216, 13824, typeof(AcademicBooksArtifact)),
        };

        private static Type[] m_TypesOfEntries = null;
        private static StealableArtifactsSpawner m_Instance;
        private Timer m_RespawnTimer;
        private StealableInstance[] m_Artifacts;
        private Hashtable m_Table;
        public StealableArtifactsSpawner(Serial serial)
            : base(serial)
        {
            m_Instance = this;
        }

        private StealableArtifactsSpawner()
            : base(1)
        {
            this.Movable = false;

            this.m_Artifacts = new StealableInstance[m_Entries.Length];
            this.m_Table = new Hashtable(m_Entries.Length);

            for (int i = 0; i < m_Entries.Length; i++)
            {
                this.m_Artifacts[i] = new StealableInstance(m_Entries[i]);
            }

            this.m_RespawnTimer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromMinutes(15.0), new TimerCallback(CheckRespawn));
        }

        public static StealableEntry[] Entries
        {
            get
            {
                return m_Entries;
            }
        }
        public static Type[] TypesOfEntires
        {
            get
            {
                if (m_TypesOfEntries == null)
                {
                    m_TypesOfEntries = new Type[m_Entries.Length];

                    for (int i = 0; i < m_Entries.Length; i++)
                        m_TypesOfEntries[i] = m_Entries[i].Type;
                }

                return m_TypesOfEntries;
            }
        }
        public static StealableArtifactsSpawner Instance
        {
            get
            {
                return m_Instance;
            }
        }
        public override string DefaultName
        {
            get
            {
                return "Stealable Artifacts Spawner - Internal";
            }
        }
        public static void Initialize()
        {
            CommandSystem.Register("GenStealArties", AccessLevel.Administrator, new CommandEventHandler(GenStealArties_OnCommand));
            CommandSystem.Register("RemoveStealArties", AccessLevel.Administrator, new CommandEventHandler(RemoveStealArties_OnCommand));
            CommandSystem.Register("StealArtiesForceRespawn", AccessLevel.GameMaster, new CommandEventHandler(StealArtiesForceRespawn_OnCommand));
        }

        private static void StealArtiesForceRespawn_OnCommand(CommandEventArgs e)
        {
            if(Instance != null &&
                Instance.m_Artifacts != null)
            {
                foreach (StealableInstance instance in Instance.m_Artifacts)
                {
                    instance.ForceRespawn();
                }
            }
        }

        public static bool Create()
        {
            if (m_Instance != null && !m_Instance.Deleted)
                return false;

            m_Instance = new StealableArtifactsSpawner();
            return true;
        }

        public static bool Remove()
        {
            if (m_Instance == null)
                return false;

            m_Instance.Delete();
            m_Instance = null;
            return true;
        }

        public static StealableInstance GetStealableInstance(Item item)
        {
            if (Instance == null)
                return null;

            return (StealableInstance)Instance.m_Table[item];
        }

        public override void OnDelete()
        {
            base.OnDelete();

            if (this.m_RespawnTimer != null)
            {
                this.m_RespawnTimer.Stop();
                this.m_RespawnTimer = null;
            }

            foreach (StealableInstance si in this.m_Artifacts)
            {
                if (si.Item != null)
                    si.Item.Delete();
            }

            m_Instance = null;
        }

        public void CheckRespawn()
        {
            foreach (StealableInstance si in this.m_Artifacts)
            {
                si.CheckRespawn();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version

            writer.WriteEncodedInt(this.m_Artifacts.Length);

            for (int i = 0; i < this.m_Artifacts.Length; i++)
            {
                StealableInstance si = this.m_Artifacts[i];

                writer.Write((Item)si.Item);
                writer.WriteDeltaTime((DateTime)si.NextRespawn);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_Artifacts = new StealableInstance[m_Entries.Length];
            this.m_Table = new Hashtable(m_Entries.Length);

            int length = reader.ReadEncodedInt();

            for (int i = 0; i < length; i++)
            {
                Item item = reader.ReadItem();
                DateTime nextRespawn = reader.ReadDeltaTime();

                if (i < this.m_Artifacts.Length)
                {
                    StealableInstance si = new StealableInstance(m_Entries[i], item, nextRespawn);
                    this.m_Artifacts[i] = si;

                    if (si.Item != null)
                        this.m_Table[si.Item] = si;
                }
            }

            for (int i = length; i < m_Entries.Length; i++)
            {
                this.m_Artifacts[i] = new StealableInstance(m_Entries[i]);
            }

            this.m_RespawnTimer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromMinutes(15.0), new TimerCallback(CheckRespawn));
        }

        private static int GetLampPostHue()
        {
            if (0.9 > Utility.RandomDouble())
                return 0;

            return Utility.RandomList(0x455, 0x47E, 0x482, 0x486, 0x48F, 0x4F2, 0x58C, 0x66C);
        }

        [Usage("GenStealArties")]
        [Description("Generates the stealable artifacts spawner.")]
        private static void GenStealArties_OnCommand(CommandEventArgs args)
        {
            Mobile from = args.Mobile;

            if (Create())
                from.SendMessage("Stealable artifacts spawner generated.");
            else
                from.SendMessage("Stealable artifacts spawner already present.");
        }

        [Usage("RemoveStealArties")]
        [Description("Removes the stealable artifacts spawner and every not yet stolen stealable artifacts.")]
        private static void RemoveStealArties_OnCommand(CommandEventArgs args)
        {
            Mobile from = args.Mobile;

            if (Remove())
                from.SendMessage("Stealable artifacts spawner removed.");
            else
                from.SendMessage("Stealable artifacts spawner not present.");
        }

        public class StealableEntry
        {
            private readonly Map m_Map;
            private readonly Point3D m_Location;
            private readonly int m_MinDelay;
            private readonly int m_MaxDelay;
            private readonly Type m_Type;
            private readonly int m_Hue;
            public StealableEntry(Map map, Point3D location, int minDelay, int maxDelay, Type type)
                : this(map, location, minDelay, maxDelay, type, 0)
            {
            }

            public StealableEntry(Map map, Point3D location, int minDelay, int maxDelay, Type type, int hue)
            {
                this.m_Map = map;
                this.m_Location = location;
                this.m_MinDelay = minDelay;
                this.m_MaxDelay = maxDelay;
                this.m_Type = type;
                this.m_Hue = hue;
            }

            public Map Map
            {
                get
                {
                    return this.m_Map;
                }
            }
            public Point3D Location
            {
                get
                {
                    return this.m_Location;
                }
            }
            public int MinDelay
            {
                get
                {
                    return this.m_MinDelay;
                }
            }
            public int MaxDelay
            {
                get
                {
                    return this.m_MaxDelay;
                }
            }
            public Type Type
            {
                get
                {
                    return this.m_Type;
                }
            }
            public int Hue
            {
                get
                {
                    return this.m_Hue;
                }
            }
            public Item CreateInstance()
            {
                Item item = (Item)Activator.CreateInstance(this.m_Type);

                if (this.m_Hue > 0)
                    item.Hue = this.m_Hue;

                item.Movable = false;
                item.MoveToWorld(this.Location, this.Map);

                return item;
            }
        }

        public class StealableInstance
        {
            private readonly StealableEntry m_Entry;
            private Item m_Item;
            private DateTime m_NextRespawn;
            public StealableInstance(StealableEntry entry)
                : this(entry, null, DateTime.UtcNow)
            {
            }

            public StealableInstance(StealableEntry entry, Item item, DateTime nextRespawn)
            {
                this.m_Item = item;
                this.m_NextRespawn = nextRespawn;
                this.m_Entry = entry;
            }

            public StealableEntry Entry
            {
                get
                {
                    return this.m_Entry;
                }
            }
            public Item Item
            {
                get
                {
                    return this.m_Item;
                }
                set
                {
                    if (this.m_Item != null && value == null)
                    {
                        int delay = Utility.RandomMinMax(this.Entry.MinDelay, this.Entry.MaxDelay);
                        this.NextRespawn = DateTime.UtcNow + TimeSpan.FromMinutes(delay);
                    }

                    if (Instance != null)
                    {
                        if (this.m_Item != null)
                            Instance.m_Table.Remove(this.m_Item);

                        if (value != null)
                            Instance.m_Table[value] = this;
                    }

                    this.m_Item = value;
                }
            }
            public DateTime NextRespawn
            {
                get
                {
                    return this.m_NextRespawn;
                }
                set
                {
                    this.m_NextRespawn = value;
                }
            }
            public void CheckRespawn()
            {
                if (this.Item != null && (this.Item.Deleted || this.Item.Movable || this.Item.Parent != null))
                    this.Item = null;

                if (this.Item == null && DateTime.UtcNow >= this.NextRespawn)
                {
                    this.Item = this.Entry.CreateInstance();
                }
            }
            public void ForceRespawn()
            {
                if (this.Item != null && (this.Item.Deleted || this.Item.Movable || this.Item.Parent != null))
                    this.Item = null;

                if (this.Item == null)
                {
                    this.Item = this.Entry.CreateInstance();
                }
            }
        }
    }
}