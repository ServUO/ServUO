using System;

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
        SugarCanes
    }

    public class PlantTypeInfo
    {
        private static readonly PlantTypeInfo[] m_Table = new PlantTypeInfo[]
        {
            new PlantTypeInfo(0xC83, 0, 0, PlantType.CampionFlowers, false, true, true),
            new PlantTypeInfo(0xC86, 0, 0, PlantType.Poppies, false, true, true),
            new PlantTypeInfo(0xC88, 0, 10,	PlantType.Snowdrops, false, true, true),
            new PlantTypeInfo(0xC94, -15, 0,	PlantType.Bulrushes, false, true, true),
            new PlantTypeInfo(0xC8B, 0, 0, PlantType.Lilies, false, true, true),
            new PlantTypeInfo(0xCA5, -8, 0,	PlantType.PampasGrass, false, true, true),
            new PlantTypeInfo(0xCA7, -10, 0,	PlantType.Rushes, false, true, true),
            new PlantTypeInfo(0xC97, -20, 0,	PlantType.ElephantEarPlant, true, false, true),
            new PlantTypeInfo(0xC9F, -20, 0,	PlantType.Fern, false, false, true),
            new PlantTypeInfo(0xCA6, -16, -5,	PlantType.PonytailPalm, false, false, true),
            new PlantTypeInfo(0xC9C, -5, -10,	PlantType.SmallPalm, false, false, true),
            new PlantTypeInfo(0xD31, 0, -27,	PlantType.CenturyPlant, true, false, true),
            new PlantTypeInfo(0xD04, 0, 10,	PlantType.WaterPlant, true, false, true),
            new PlantTypeInfo(0xCA9, 0, 0, PlantType.SnakePlant, true, false, true),
            new PlantTypeInfo(0xD2C, 0, 10,	PlantType.PricklyPearCactus,	false, false, true),
            new PlantTypeInfo(0xD26, 0, 10,	PlantType.BarrelCactus, false, false, true),
            new PlantTypeInfo(0xD27, 0, 10,	PlantType.TribarrelCactus, false, false, true),
            new PlantTypeInfo(0x28DC, -5, 5,	PlantType.CommonGreenBonsai,	true, false, false),
            new PlantTypeInfo(0x28DF, -5, 5,	PlantType.CommonPinkBonsai, true, false, false),
            new PlantTypeInfo(0x28DD, -5, 5,	PlantType.UncommonGreenBonsai,	true, false, false),
            new PlantTypeInfo(0x28E0, -5, 5,	PlantType.UncommonPinkBonsai,	true, false, false),
            new PlantTypeInfo(0x28DE, -5, 5,	PlantType.RareGreenBonsai, true, false, false),
            new PlantTypeInfo(0x28E1, -5, 5,	PlantType.RarePinkBonsai, true, false, false),
            new PlantTypeInfo(0x28E2, -5, 5,	PlantType.ExceptionalBonsai,	true, false, false),
            new PlantTypeInfo(0x28E3, -5, 5,	PlantType.ExoticBonsai, true, false, false),
            new PlantTypeInfo(0x0D25, 0, 0,	PlantType.Cactus, false, false, false),
            new PlantTypeInfo(0x1A9A, 5, 10, PlantType.FlaxFlowers, false, true, false),
            new PlantTypeInfo(0x0C84, 0, 0, PlantType.FoxgloveFlowers, false, true, false),
            new PlantTypeInfo(0x1A9F, 5, -25,	PlantType.HopsEast, false, false, false),
            new PlantTypeInfo(0x0CC1, 0, 0, PlantType.OrfluerFlowers, false, true, false),
            new PlantTypeInfo(0x0CFE, -45, -30, PlantType.CypressTwisted, false, false, false),
            new PlantTypeInfo(0x0C8F, 0, 0, PlantType.HedgeShort, false, false, false),
            new PlantTypeInfo(0x0CC8, 0, 0,	PlantType.JuniperBush, true, false, false),
            new PlantTypeInfo(0x0C8E, -20, 0,	PlantType.SnowdropPatch, false, true, false),
            new PlantTypeInfo(0x0CB7, 0, 0,	PlantType.Cattails, false, false, false),
            new PlantTypeInfo(0x0CBE, -20, 0, PlantType.PoppyPatch, false, true, false),
            new PlantTypeInfo(0x0CC9, 0, 0,	PlantType.SpiderTree, false, false, false),
            new PlantTypeInfo(0x0DC1, -5, 15,	PlantType.WaterLily, false, true, false),
            new PlantTypeInfo(0x0CFB, -45, -30,	PlantType.CypressStraight, false, false, false),
            new PlantTypeInfo(0x0DB8, 0, -20, PlantType.HedgeTall, false, false, false),
            new PlantTypeInfo(0x1AA1, 10, -25,	PlantType.HopsSouth, false, false, false),
            new PlantTypeInfo(0x246C, -25, -20,	PlantType.SugarCanes, false, false, false)
        };
        private readonly int m_ItemID;
        private readonly int m_OffsetX;
        private readonly int m_OffsetY;
        private readonly PlantType m_PlantType;
        private readonly bool m_ContainsPlant;
        private readonly bool m_Flowery;
        private readonly bool m_Crossable;
        private PlantTypeInfo(int itemID, int offsetX, int offsetY, PlantType plantType, bool containsPlant, bool flowery, bool crossable)
        {
            this.m_ItemID = itemID;
            this.m_OffsetX = offsetX;
            this.m_OffsetY = offsetY;
            this.m_PlantType = plantType;
            this.m_ContainsPlant = containsPlant;
            this.m_Flowery = flowery;
            this.m_Crossable = crossable;
        }

        public int ItemID
        {
            get
            {
                return this.m_ItemID;
            }
        }
        public int OffsetX
        {
            get
            {
                return this.m_OffsetX;
            }
        }
        public int OffsetY
        {
            get
            {
                return this.m_OffsetY;
            }
        }
        public PlantType PlantType
        {
            get
            {
                return this.m_PlantType;
            }
        }
        public int Name
        {
            get
            {
                return this.m_ItemID < 0x4000 ? 1020000 + this.m_ItemID : 1078872 + this.m_ItemID;
            }
        }
        public bool ContainsPlant
        {
            get
            {
                return this.m_ContainsPlant;
            }
        }
        public bool Flowery
        {
            get
            {
                return this.m_Flowery;
            }
        }
        public bool Crossable
        {
            get
            {
                return this.m_Crossable;
            }
        }
        public static PlantTypeInfo GetInfo(PlantType plantType)
        {
            int index = (int)plantType;

            if (index >= 0 && index < m_Table.Length)
                return m_Table[index];
            else
                return m_Table[0];
        }

        public static PlantType RandomFirstGeneration()
        {
            switch ( Utility.Random(3) )
            {
                case 0:
                    return PlantType.CampionFlowers;
                case 1:
                    return PlantType.Fern;
                default:
                    return PlantType.TribarrelCactus;
            }
        }

        public static PlantType RandomPeculiarGroupOne()
        {
            switch ( Utility.Random(5) )
            {
                case 0:
                    return PlantType.Cactus;
                case 1:
                    return PlantType.FlaxFlowers;
                case 2:
                    return PlantType.FoxgloveFlowers;
                case 3:
                    return PlantType.HopsEast;
                default:
                    return PlantType.OrfluerFlowers;
            }
        }

        public static PlantType RandomPeculiarGroupTwo()
        {
            switch ( Utility.Random(4) )
            {
                case 0:
                    return PlantType.CypressTwisted; 
                case 1:
                    return PlantType.HedgeShort;
                case 2:
                    return PlantType.JuniperBush;
                default:
                    return PlantType.SnowdropPatch;
            }
        }

        public static PlantType RandomPeculiarGroupThree()
        {
            switch ( Utility.Random(4) )
            {
                case 0:
                    return PlantType.Cattails;
                case 1:
                    return PlantType.PoppyPatch;
                case 2:
                    return PlantType.SpiderTree;
                default:
                    return PlantType.WaterLily;
            }
        }

        public static PlantType RandomPeculiarGroupFour()
        {
            switch ( Utility.Random(4) )
            {
                case 0:
                    return PlantType.CypressStraight;
                case 1:
                    return PlantType.HedgeTall;
                case 2:
                    return PlantType.HopsSouth;
                default:
                    return PlantType.SugarCanes;
            }
        }

        public static PlantType RandomBonsai(double increaseRatio)
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

            if (rand < 0.5 / exp4)
                return PlantType.CommonGreenBonsai;
            else if (rand < 1.0 / exp4)
                return PlantType.CommonPinkBonsai;
            else if (rand < (k1 * 0.5 + 1.0) / exp4)
                return PlantType.UncommonGreenBonsai;
            else if (rand < exp1 / exp4)
                return PlantType.UncommonPinkBonsai;
            else if (rand < (k2 * 0.5 + exp1) / exp4)
                return PlantType.RareGreenBonsai;
            else if (rand < exp2 / exp4)
                return PlantType.RarePinkBonsai;
            else if (rand < exp3 / exp4)
                return PlantType.ExceptionalBonsai;
            else
                return PlantType.ExoticBonsai;
        }

        public static bool IsCrossable(PlantType plantType)
        {
            return GetInfo(plantType).Crossable;
        }

        public static PlantType Cross(PlantType first, PlantType second)
        {
            if (!IsCrossable(first) || !IsCrossable(second))
                return PlantType.CampionFlowers;

            int firstIndex = (int)first;
            int secondIndex = (int)second;

            if (firstIndex + 1 == secondIndex || firstIndex == secondIndex + 1)
                return Utility.RandomBool() ? first : second;
            else
                return (PlantType)((firstIndex + secondIndex) / 2);
        }

        public static int GetBonsaiTitle(PlantType plantType)
        {
            switch ( plantType )
            {
                case PlantType.Cactus:
                case PlantType.FlaxFlowers:
                case PlantType.FoxgloveFlowers:
                case PlantType.HopsEast:
                case PlantType.OrfluerFlowers:
                case PlantType.CypressTwisted:
                case PlantType.HedgeShort:
                case PlantType.JuniperBush:
                case PlantType.SnowdropPatch:
                case PlantType.Cattails:
                case PlantType.PoppyPatch:
                case PlantType.SpiderTree:
                case PlantType.WaterLily:
                case PlantType.CypressStraight:
                case PlantType.HedgeTall:
                case PlantType.HopsSouth:
                case PlantType.SugarCanes:
                    return 1080528; // peculiar

                case PlantType.CommonGreenBonsai:
                case PlantType.CommonPinkBonsai:
                    return 1063335; // common

                case PlantType.UncommonGreenBonsai:
                case PlantType.UncommonPinkBonsai:
                    return 1063336; // uncommon

                case PlantType.RareGreenBonsai:
                case PlantType.RarePinkBonsai:
                    return 1063337; // rare

                case PlantType.ExceptionalBonsai:
                    return 1063341; // exceptional

                case PlantType.ExoticBonsai:
                    return 1063342; // exotic

                default:
                    return 0;
            }
        }
    }
}