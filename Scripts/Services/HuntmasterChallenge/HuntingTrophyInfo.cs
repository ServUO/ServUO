using Server.Mobiles;
using System;
using System.Collections.Generic;

namespace Server.Engines.HuntsmasterChallenge
{
    public enum HuntType
    {
        GrizzlyBear,
        GrayWolf,
        Cougar,
        Turkey,
        Bull,
        Boar,
        Walrus,
        Alligator,
        Eagle,
        MyrmidexLarvae,
        Najasaurus,
        Anchisaur,
        Allosaurus,
        Dimetrosaur,
        Saurosaurus,
        Tiger,
        MyrmidexDrone,
        Triceratops,
        Lion,
        WhiteTiger,
        BlackTiger,
        //Publish 102 added:
        Raptor,
        SeaSerpent,
        Scorpion
    }

    public enum MeasuredBy
    {
        Weight,
        Length,
        Wingspan
    }

    [PropertyObject]
    public class HuntingTrophyInfo
    {
        private static readonly List<HuntingTrophyInfo> m_Infos = new List<HuntingTrophyInfo>();
        public static List<HuntingTrophyInfo> Infos => m_Infos;

        public static void Configure()
        {
            m_Infos.Add(new HuntingTrophyInfo(HuntType.GrizzlyBear, typeof(GrizzlyBear), 0x9A26, 1015242, 1123486, 400, 790, MeasuredBy.Weight, false));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.GrayWolf, typeof(GreyWolf), 0x9A28, 1018118, 1123488, 50, 99, MeasuredBy.Weight, false));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Cougar, typeof(Cougar), 0x9A2A, 1029603, 1123490, 100, 220, MeasuredBy.Weight, false));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Turkey, typeof(Turkey), 0x9A2C, 1155714, 1123492, 10, 24, MeasuredBy.Weight, false));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Bull, typeof(Bull), 0x9A2E, 1072495, 1123494, 1100, 2200, MeasuredBy.Weight, false));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Boar, typeof(Boar), 0x9A30, 1155715, 1123496, 100, 400, MeasuredBy.Weight, false));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Walrus, typeof(Walrus), 0x9A32, 1155716, 1123498, 1200, 3700, MeasuredBy.Weight, false));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Alligator, typeof(Alligator), 0x9A34, 1155717, 1123500, 15, 30, MeasuredBy.Length, false));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Eagle, typeof(Eagle), 0x9A36, 1072461, 1123502, 10, 20, MeasuredBy.Wingspan, false));

            // Pub 91 Additions
            m_Infos.Add(new HuntingTrophyInfo(HuntType.MyrmidexLarvae, typeof(MyrmidexLarvae), 0x9C00, 0x9C04, 1156276, 1123960, 20, 40, MeasuredBy.Weight, true));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Najasaurus, typeof(Najasaurus), 0x9C02, 0x9C06, 1156283, 1123962, 200, 400, MeasuredBy.Weight, true));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Anchisaur, typeof(Anchisaur), 0x9C08, 1156284, 1123968, 200, 400, MeasuredBy.Weight, false));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Allosaurus, typeof(Allosaurus), 0x9C0A, 1156280, 1123970, 5000, 12000, MeasuredBy.Weight, false));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Dimetrosaur, typeof(Dimetrosaur), 0x9C0C, 1156279, 1123972, 200, 400, MeasuredBy.Weight, false));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Saurosaurus, typeof(Saurosaurus), 0x9C0E, 1156289, 1123974, 1500, 2000, MeasuredBy.Weight, false));

            m_Infos.Add(new HuntingTrophyInfo(HuntType.MyrmidexDrone, typeof(MyrmidexDrone), 0x9DA6, 1156134, 1124382, 100, 200, MeasuredBy.Weight, false));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Tiger, typeof(WildTiger), 0x9DA4, 1156286, 1124380, 350, 700, MeasuredBy.Weight, false));

            m_Infos.Add(new HuntingTrophyInfo(HuntType.Triceratops, typeof(Triceratops), 0x9F2C, 0x9F2B, 1124731, 1124771, 10000, 15000, MeasuredBy.Weight, false));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Lion, typeof(Lion), 0x9F2E, 0x9F2D, 1124736, 1124773, 350, 700, MeasuredBy.Weight, false));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.WhiteTiger, typeof(WildWhiteTiger), 0x9F30, 0x9F2F, 1156286, 1124775, 350, 700, MeasuredBy.Weight, false));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.BlackTiger, typeof(WildBlackTiger), 0x9F32, 0x9F31, 1156286, 1124777, 350, 700, MeasuredBy.Weight, false));

            //Publish 102
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Raptor, typeof(Raptor), 0xA20E, 0xA20D, 1095923, 1125508, 400, 800, MeasuredBy.Weight, false));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.SeaSerpent, typeof(SeaSerpent), 0xA20C, 0xA20C, 1018242, 1125508, 200, 600, MeasuredBy.Weight, false));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Scorpion, typeof(Scorpion), 0xA210, 0xA20F, 1029657, 1125508, 250, 500, MeasuredBy.Weight, false));
        }

        private readonly HuntType m_HuntType;
        private readonly Type m_CreatureType;
        private readonly MeasuredBy m_MeasuredBy;
        private readonly int m_SouthID, m_EastID, m_MinMeasurement, m_MaxMeasurement;
        private readonly TextDefinition m_Species, m_TrophyName;
        private readonly bool m_Complex;

        [CommandProperty(AccessLevel.GameMaster)]
        public HuntType HuntType => m_HuntType;

        [CommandProperty(AccessLevel.GameMaster)]
        public Type CreatureType => m_CreatureType;

        [CommandProperty(AccessLevel.GameMaster)]
        public MeasuredBy MeasuredBy => m_MeasuredBy;

        [CommandProperty(AccessLevel.GameMaster)]
        public int SouthID => m_SouthID;

        [CommandProperty(AccessLevel.GameMaster)]
        public int EastID => m_EastID;

        [CommandProperty(AccessLevel.GameMaster)]
        public TextDefinition Species => m_Species;

        [CommandProperty(AccessLevel.GameMaster)]
        public TextDefinition TrophyName => m_TrophyName;

        [CommandProperty(AccessLevel.GameMaster)]
        public int MinMeasurement => m_MinMeasurement;

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxMeasurement => m_MaxMeasurement;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Complex => m_Complex;

        public HuntingTrophyInfo(HuntType type, Type creatureType, int southID, TextDefinition species, TextDefinition trophyName, int minMeasurement, int maxMeasurement, MeasuredBy measuredBy, bool complex)
            : this(type, creatureType, southID, southID + 1, species, trophyName, minMeasurement, maxMeasurement, measuredBy, complex)
        {
        }

        public HuntingTrophyInfo(HuntType type, Type creatureType, int southID, int eastID, TextDefinition species, TextDefinition trophyName, int minMeasurement, int maxMeasurement, MeasuredBy measuredBy, bool complex)
        {
            m_HuntType = type;
            m_CreatureType = creatureType;
            m_MeasuredBy = measuredBy;
            m_SouthID = southID;
            m_EastID = eastID;
            m_Species = species;
            m_TrophyName = trophyName;
            m_MinMeasurement = minMeasurement;
            m_MaxMeasurement = maxMeasurement;
            m_Complex = complex;
        }

        public static HuntingTrophyInfo GetInfo(HuntType type)
        {
            foreach (HuntingTrophyInfo info in m_Infos)
            {
                if (info.HuntType == type)
                    return info;
            }

            return null;
        }

        public static int CheckInfo(int number)
        {
            for (int i = 0; i < Infos.Count; i++)
            {
                HuntingTrophyInfo info = Infos[i];

                if (info.Species.Number == number)
                {
                    return i;
                }
            }

            return 0;
        }
    }
}
