using System;
using Server;

namespace Server.Engines.Plants
{
	public enum PlantType
	{
		CampionFlowers,
		Poppies,
		Snowdrops,
		Bulrushes,
		Lilies,
		PampasGrass,
		Rushes,
		ElephantEarPlant,
		Fern,
		PonytailPalm,
		SmallPalm,
		CenturyPlant,
		WaterPlant,
		SnakePlant,
		PricklyPearCactus,
		BarrelCactus,
		TribarrelCactus,
		CommonGreenBonsai,
		CommonPinkBonsai,
		UncommonGreenBonsai,
		UncommonPinkBonsai,
		RareGreenBonsai,
		RarePinkBonsai,
		ExceptionalBonsai,
		ExoticBonsai,
		Cactus,
		FlaxFlowers,
		FoxgloveFlowers,
		HopsEast,
		OrfluerFlowers,
		CypressTwisted,
		HedgeShort,
		JuniperBush,
		SnowdropPatch,
		Cattails,
		PoppyPatch,
		SpiderTree,
		WaterLily,
		CypressStraight,
		HedgeTall,
		HopsSouth,
		SugarCanes,
		CocoaTree, 
        Vanilla
	}

	public enum PlantCategory
	{
		Default,
		Common		= 1063335, //
		Uncommon	= 1063336, //
		Rare		= 1063337, // Bonsai
		Exceptional	= 1063341, //
		Exotic		= 1063342, //
		Peculiar	= 1080528,
		Fragrant	= 1080529
	}

	public class PlantTypeInfo
	{
		private static PlantTypeInfo[] m_Table = new PlantTypeInfo[]
		{
			new PlantTypeInfo( 0xC83, 0, 0,			PlantType.CampionFlowers,		false, true, true, true,		PlantCategory.Default ),
			new PlantTypeInfo( 0xC86, 0, 0,			PlantType.Poppies,				false, true, true, true,		PlantCategory.Default ),
			new PlantTypeInfo( 0xC88, 0, 10,		PlantType.Snowdrops,			false, true, true, true,		PlantCategory.Default ),
			new PlantTypeInfo( 0xC94, -15, 0,		PlantType.Bulrushes,			false, true, true, true,		PlantCategory.Default ),
			new PlantTypeInfo( 0xC8B, 0, 0,			PlantType.Lilies,				false, true, true, true,		PlantCategory.Default ),
			new PlantTypeInfo( 0xCA5, -8, 0,		PlantType.PampasGrass,			false, true, true, true,		PlantCategory.Default ),
			new PlantTypeInfo( 0xCA7, -10, 0,		PlantType.Rushes,				false, true, true, true,		PlantCategory.Default ),
			new PlantTypeInfo( 0xC97, -20, 0,		PlantType.ElephantEarPlant,		true, false, true, true,		PlantCategory.Default ),
			new PlantTypeInfo( 0xC9F, -20, 0,		PlantType.Fern,					false, false, true, true,		PlantCategory.Default ),
			new PlantTypeInfo( 0xCA6, -16, -5,		PlantType.PonytailPalm,			false, false, true, true,		PlantCategory.Default ),
			new PlantTypeInfo( 0xC9C, -5, -10,		PlantType.SmallPalm,			false, false, true, true,		PlantCategory.Default ),
			new PlantTypeInfo( 0xD31, 0, -27,		PlantType.CenturyPlant,			true, false, true, true,		PlantCategory.Default ),
			new PlantTypeInfo( 0xD04, 0, 10,		PlantType.WaterPlant,			true, false, true, true,		PlantCategory.Default ),
			new PlantTypeInfo( 0xCA9, 0, 0,			PlantType.SnakePlant,			true, false, true, true,		PlantCategory.Default ),
			new PlantTypeInfo( 0xD2C, 0, 10,		PlantType.PricklyPearCactus,	false, false, true, true,		PlantCategory.Default ),
			new PlantTypeInfo( 0xD26, 0, 10,		PlantType.BarrelCactus,			false, false, true, true,		PlantCategory.Default ),
			new PlantTypeInfo( 0xD27, 0, 10,		PlantType.TribarrelCactus,		false, false, true, true,		PlantCategory.Default ),
			new PlantTypeInfo( 0x28DC, -5, 5,		PlantType.CommonGreenBonsai,	true, false, false, false,		PlantCategory.Common ),
			new PlantTypeInfo( 0x28DF, -5, 5,		PlantType.CommonPinkBonsai,		true, false, false, false,		PlantCategory.Common ),
			new PlantTypeInfo( 0x28DD, -5, 5,		PlantType.UncommonGreenBonsai,	true, false, false, false,		PlantCategory.Uncommon ),
			new PlantTypeInfo( 0x28E0, -5, 5,		PlantType.UncommonPinkBonsai,	true, false, false, false,		PlantCategory.Uncommon ),
			new PlantTypeInfo( 0x28DE, -5, 5,		PlantType.RareGreenBonsai,		true, false, false, false,		PlantCategory.Rare ),
			new PlantTypeInfo( 0x28E1, -5, 5,		PlantType.RarePinkBonsai,		true, false, false, false,		PlantCategory.Rare ),
			new PlantTypeInfo( 0x28E2, -5, 5,		PlantType.ExceptionalBonsai,	true, false, false, false,		PlantCategory.Exceptional ),
			new PlantTypeInfo( 0x28E3, -5, 5,		PlantType.ExoticBonsai,			true, false, false, false,		PlantCategory.Exotic ),
			new PlantTypeInfo( 0x0D25, 0, 0,		PlantType.Cactus,				false, false, false, false,		PlantCategory.Peculiar ),
			new PlantTypeInfo( 0x1A9A, 5, 10,		PlantType.FlaxFlowers,			false, true, false, false,		PlantCategory.Peculiar ),
			new PlantTypeInfo( 0x0C84, 0, 0,		PlantType.FoxgloveFlowers,		false, true, false, false,		PlantCategory.Peculiar ),
			new PlantTypeInfo( 0x1A9F, 5, -25,		PlantType.HopsEast,				false, false, false, false,		PlantCategory.Peculiar ),
			new PlantTypeInfo( 0x0CC1, 0, 0,		PlantType.OrfluerFlowers,		false, true, false, false,		PlantCategory.Peculiar ),
			new PlantTypeInfo( 0x0CFE, -45, -30,	PlantType.CypressTwisted,		false, false, false, false,		PlantCategory.Peculiar ),
			new PlantTypeInfo( 0x0C8F, 0, 0,		PlantType.HedgeShort,			false, false, false, false,		PlantCategory.Peculiar ),
			new PlantTypeInfo( 0x0CC8, 0, 0,		PlantType.JuniperBush,			true, false, false, false,		PlantCategory.Peculiar ),
			new PlantTypeInfo( 0x0C8E, -20, 0,		PlantType.SnowdropPatch,		false, true, false, false,		PlantCategory.Peculiar ),
			new PlantTypeInfo( 0x0CB7, 0, 0,		PlantType.Cattails,				false, false, false, false,		PlantCategory.Peculiar ),
			new PlantTypeInfo( 0x0CBE, -20, 0,		PlantType.PoppyPatch,			false, true, false, false,		PlantCategory.Peculiar ),
			new PlantTypeInfo( 0x0CC9, 0, 0,		PlantType.SpiderTree,			false, false, false, false,		PlantCategory.Peculiar ),
			new PlantTypeInfo( 0x0DC1, -5, 15,		PlantType.WaterLily,			false, true, false, false,		PlantCategory.Peculiar ),
			new PlantTypeInfo( 0x0CFB, -45, -30,	PlantType.CypressStraight,		false, false, false, false,		PlantCategory.Peculiar ),
			new PlantTypeInfo( 0x0DB8, 0, -20,		PlantType.HedgeTall,			false, false, false, false,		PlantCategory.Peculiar ),
			new PlantTypeInfo( 0x1AA1, 10, -25,		PlantType.HopsSouth,			false, false, false, false,		PlantCategory.Peculiar ),
			new PlantTypeInfo( 0x246C, -25, -20,	PlantType.SugarCanes,			false, false, false, false,		PlantCategory.Peculiar,		1114898, 1114898, 1094702, 1094703, 1095221, 1113715 ),
			new PlantTypeInfo( 0xC9E, -40, -30,		PlantType.CocoaTree,			false, false, false, true,		PlantCategory.Fragrant,		1080536, 1080536, 1080534, 1080531, 1080533, 1113716 ),
            new PlantTypeInfo( 0x4B8C, 5, 10,		PlantType.Vanilla,			    false, true, false, true,		PlantCategory.Peculiar )
		};

		public static PlantTypeInfo GetInfo( PlantType plantType )
		{
			int index = (int)plantType;

			if ( index >= 0 && index < m_Table.Length )
				return m_Table[index];
			else
				return m_Table[0];
		}

		public static PlantType RandomFirstGeneration()
		{
			switch ( Utility.Random( 3 ) )
			{
				case 0: return PlantType.CampionFlowers;
				case 1: return PlantType.Fern;
				default: return PlantType.TribarrelCactus;
			}
		}

		public static PlantType RandomPeculiarGroupOne()
		{
			switch ( Utility.Random( 6 ) )
			{
				case 0: return PlantType.Cactus;
				case 1: return PlantType.FlaxFlowers;
				case 2: return PlantType.FoxgloveFlowers;
				case 3: return PlantType.HopsEast;
				case 4: return PlantType.CocoaTree;
				default: return PlantType.OrfluerFlowers;
			}
		}

		public static PlantType RandomPeculiarGroupTwo()
		{
			switch ( Utility.Random( 5 ) )
			{
				case 0:	return PlantType.CypressTwisted;
				case 1: return PlantType.HedgeShort;
				case 2: return PlantType.JuniperBush;
				case 3: return PlantType.CocoaTree;
				default: return PlantType.SnowdropPatch;
			}
		}

		public static PlantType RandomPeculiarGroupThree()
		{
			switch ( Utility.Random( 5 ) )
			{
				case 0:	return PlantType.Cattails;
				case 1: return PlantType.PoppyPatch;
				case 2: return PlantType.SpiderTree;
				case 3: return PlantType.CocoaTree;
				default: return PlantType.WaterLily;
			}
		}

		public static PlantType RandomPeculiarGroupFour()
		{
			switch ( Utility.Random( 5 ) )
			{
				case 0:	return PlantType.CypressStraight;
				case 1: return PlantType.HedgeTall;
				case 2: return PlantType.HopsSouth;
				case 3: return PlantType.CocoaTree;
				default: return PlantType.SugarCanes;
			}
		}

		public static PlantType RandomBonsai( double increaseRatio )
		{
			/* Chances of each plant type are equal to the chances of the previous plant type * increaseRatio:
			 * E.g.:
			 *  chances_of_uncommon = chances_of_common * increaseRatio
			 *  chances_of_rare = chances_of_uncommon * increaseRatio
			 *  ...
			 *
			 * If increaseRatio < 1 -> rare plants are actually rarer than the others
			 * If increaseRatio > 1 -> rare plants are actually more common than the others (it might be the case with certain monsters)
			 *
			 * If a plant type (common, uncommon, ...) has 2 different colors, they have the same chances:
			 *  chances_of_green_common = chances_of_pink_common = chances_of_common / 2
			 *  ...
			 */

			double k1 = increaseRatio >= 0.0 ? increaseRatio : 0.0;
			double k2 = k1 * k1;
			double k3 = k2 * k1;
			double k4 = k3 * k1;

			double exp1 = k1 + 1.0;
			double exp2 = k2 + exp1;
			double exp3 = k3 + exp2;
			double exp4 = k4 + exp3;

			double rand = Utility.RandomDouble();

			if ( rand < 0.5 / exp4 )
				return PlantType.CommonGreenBonsai;
			else if ( rand < 1.0 / exp4 )
				return PlantType.CommonPinkBonsai;
			else if ( rand < (k1 * 0.5 + 1.0) / exp4 )
				return PlantType.UncommonGreenBonsai;
			else if ( rand < exp1 / exp4 )
				return PlantType.UncommonPinkBonsai;
			else if ( rand < (k2 * 0.5 + exp1) / exp4 )
				return PlantType.RareGreenBonsai;
			else if ( rand < exp2 / exp4 )
				return PlantType.RarePinkBonsai;
			else if ( rand < exp3 / exp4 )
				return PlantType.ExceptionalBonsai;
			else
				return PlantType.ExoticBonsai;
		}

		public static bool IsCrossable( PlantType plantType )
		{
			return GetInfo( plantType ).Crossable;
		}

		public static PlantType Cross( PlantType first, PlantType second )
		{
			if ( !IsCrossable( first ) || !IsCrossable( second ) )
				return PlantType.CampionFlowers;

			int firstIndex = (int)first;
			int secondIndex = (int)second;

			if ( firstIndex + 1 == secondIndex || firstIndex == secondIndex + 1 )
				return Utility.RandomBool() ? first : second;
			else
				return (PlantType)( (firstIndex + secondIndex) / 2 );
		}

		public static bool CanReproduce( PlantType plantType )
		{
			return GetInfo( plantType ).Reproduces;
		}

		public int GetPlantLabelSeed( PlantHueInfo hueInfo )
		{
			if ( m_PlantLabelSeed != -1 )
				return m_PlantLabelSeed;

			return hueInfo.IsBright() ? 1061887 : 1061888; // a ~1_val~ of ~2_val~ dirt with a ~3_val~ [bright] ~4_val~ ~5_val~ ~6_val~
		}

		public int GetPlantLabelPlant( PlantHueInfo hueInfo )
		{
			if ( m_PlantLabelPlant != -1 )
				return m_PlantLabelPlant;

			if ( m_ContainsPlant )
				return hueInfo.IsBright() ? 1060832 : 1060831; // a ~1_val~ of ~2_val~ dirt with a ~3_val~ [bright] ~4_val~ ~5_val~
			else
				return hueInfo.IsBright() ? 1061887 : 1061888; // a ~1_val~ of ~2_val~ dirt with a ~3_val~ [bright] ~4_val~ ~5_val~ ~6_val~
		}

		public int GetPlantLabelFullGrown( PlantHueInfo hueInfo )
		{
			if ( m_PlantLabelFullGrown != -1 )
				return m_PlantLabelFullGrown;

			if ( m_ContainsPlant )
				return hueInfo.IsBright() ? 1061891 : 1061889; // a ~1_HEALTH~ [bright] ~2_COLOR~ ~3_NAME~
			else
				return hueInfo.IsBright() ? 1061892 : 1061890; // a ~1_HEALTH~ [bright] ~2_COLOR~ ~3_NAME~ plant
		}

		public int GetPlantLabelDecorative( PlantHueInfo hueInfo )
		{
			if ( m_PlantLabelDecorative != -1 )
				return m_PlantLabelDecorative;

			return hueInfo.IsBright() ? 1074267 : 1070973; // a decorative [bright] ~1_COLOR~ ~2_TYPE~
		}

		public int GetSeedLabel( PlantHueInfo hueInfo )
		{
			if ( m_SeedLabel != -1 )
				return m_SeedLabel;

			return hueInfo.IsBright() ? 1061918 : 1061917; // [bright] ~1_COLOR~ ~2_TYPE~ seed
		}

		public int GetSeedLabelPlural( PlantHueInfo hueInfo )
		{
			if ( m_SeedLabelPlural != -1 )
				return m_SeedLabelPlural;

			return hueInfo.IsBright() ? 1113493 : 1113492; // ~1_amount~ [bright] ~2_color~ ~3_type~ seeds
		}

		private int m_ItemID;
		private int m_OffsetX;
		private int m_OffsetY;
		private PlantType m_PlantType;
		private bool m_ContainsPlant;
		private bool m_Flowery;
		private bool m_Crossable;
		private bool m_Reproduces;
		private PlantCategory m_PlantCategory;

		// Cliloc overrides
		private int m_PlantLabelSeed;
		private int m_PlantLabelPlant;
		private int m_PlantLabelFullGrown;
		private int m_PlantLabelDecorative;
		private int m_SeedLabel;
		private int m_SeedLabelPlural;

		public int ItemID { get { return m_ItemID; } }
		public int OffsetX { get { return m_OffsetX; } }
		public int OffsetY { get { return m_OffsetY; } }
		public PlantType PlantType { get { return m_PlantType; } }
		public PlantCategory PlantCategory { get { return m_PlantCategory; } }
		public int Name { get { return ( m_ItemID < 0x4000 ) ? 1020000 + m_ItemID : 1078872 + m_ItemID; } }

		public bool ContainsPlant { get { return m_ContainsPlant; } }
		public bool Flowery { get { return m_Flowery; } }
		public bool Crossable { get { return m_Crossable; } }
		public bool Reproduces { get { return m_Reproduces; } }

		private PlantTypeInfo( int itemID, int offsetX, int offsetY, PlantType plantType, bool containsPlant, bool flowery, bool crossable, bool reproduces, PlantCategory plantCategory )
			: this( itemID, offsetX, offsetY, plantType, containsPlant, flowery, crossable, reproduces, plantCategory, -1, -1, -1, -1, -1, -1 )
		{
		}

		private PlantTypeInfo( int itemID, int offsetX, int offsetY, PlantType plantType, bool containsPlant, bool flowery, bool crossable, bool reproduces, PlantCategory plantCategory, int plantLabelSeed, int plantLabelPlant, int plantLabelFullGrown, int plantLabelDecorative, int seedLabel, int seedLabelPlural )
		{
			m_ItemID = itemID;
			m_OffsetX = offsetX;
			m_OffsetY = offsetY;
			m_PlantType = plantType;
			m_ContainsPlant = containsPlant;
			m_Flowery = flowery;
			m_Crossable = crossable;
			m_Reproduces = reproduces;
			m_PlantCategory = plantCategory;
			m_PlantLabelSeed = plantLabelSeed;
			m_PlantLabelPlant = plantLabelPlant;
			m_PlantLabelFullGrown = plantLabelFullGrown;
			m_PlantLabelDecorative = plantLabelDecorative;
			m_SeedLabel = seedLabel;
			m_SeedLabelPlural = seedLabelPlural;
		}
	}
}