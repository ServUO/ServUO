using System;
using Server;
using Server.Mobiles;
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
        private static List<HuntingTrophyInfo> m_Infos = new List<HuntingTrophyInfo>();
        public static List<HuntingTrophyInfo> Infos { get { return m_Infos; } }

        public static void Configure()
        {
            m_Infos.Add(new HuntingTrophyInfo(HuntType.GrizzlyBear, typeof(GrizzlyBear),    0x9A26, new TextDefinition(1015242), 400, 800,  MeasuredBy.Weight, false));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.GrayWolf,    typeof(GreyWolf),       0x9A28, new TextDefinition(1029681), 70,  190,  MeasuredBy.Weight, false));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Cougar,      typeof(Cougar),         0x9A2A, new TextDefinition(1029603), 50,  140,  MeasuredBy.Weight, false));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Turkey,      typeof(Turkey),         0x9A2C, new TextDefinition(1155714), 10,  55,   MeasuredBy.Weight, false));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Bull,        typeof(Bull),           0x9A2E, new TextDefinition(1072495), 500, 1000, MeasuredBy.Weight, false, true));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Boar,        typeof(Boar),           0x9A30, new TextDefinition(1155715), 100, 400,  MeasuredBy.Weight, false, true));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Walrus,      typeof(Walrus),         0x9A32, new TextDefinition(1155716), 600, 1500, MeasuredBy.Weight, false, true));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Alligator,   typeof(Alligator),      0x9A34, new TextDefinition(1155717), 5,   15,   MeasuredBy.Length, false, true));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Eagle,       typeof(Eagle),          0x9A36, new TextDefinition(1072461), 5,   15,   MeasuredBy.Wingspan, false, true));
        
            // Pub 91 Additions
            m_Infos.Add(new HuntingTrophyInfo(HuntType.MyrmidexLarvae,  typeof(MyrmidexLarvae), 0x9C00, 0x9C04, new TextDefinition(1156276), 200, 400, MeasuredBy.Weight, true));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Najasaurus,      typeof(Najasaurus),     0x9C02, 0x9C06, new TextDefinition(1156283), 400, 800, MeasuredBy.Weight, true));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Anchisaur,       typeof(Anchisaur),      0x9C08, new TextDefinition(1156284), 400, 800, MeasuredBy.Weight, false, true));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Allosaurus,      typeof(Allosaurus),     0x9C0A, new TextDefinition(1156280), 400, 800, MeasuredBy.Weight, false, true));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Dimetrosaur,     typeof(Dimetrosaur),    0x9C0C, new TextDefinition(1156279), 400, 800, MeasuredBy.Weight, false, true));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Saurosaurus,     typeof(Saurosaurus),    0x9C0E, new TextDefinition(1156289), 400, 800, MeasuredBy.Weight, false, true));

            m_Infos.Add(new HuntingTrophyInfo(HuntType.MyrmidexDrone,   typeof(MyrmidexDrone),  0x9DA6, new TextDefinition(1156134), 300, 600, MeasuredBy.Weight, false, true));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Tiger,           typeof(WildTiger),      0x9DA4, new TextDefinition(1156286), 400, 800, MeasuredBy.Weight, false, true));

            m_Infos.Add(new HuntingTrophyInfo(HuntType.Triceratops,     typeof(Triceratops),    0x9F2C, 0x9F2B, new TextDefinition(1124731), 400, 800, MeasuredBy.Weight, false, true));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Lion,            typeof(Lion),           0x9F2E, 0x9F2D, new TextDefinition(1124736), 400, 800, MeasuredBy.Weight, false, true));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.WhiteTiger,      typeof(WildWhiteTiger), 0x9F30, 0x9F2F, new TextDefinition(1156286), 400, 800, MeasuredBy.Weight, false, true));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.BlackTiger,      typeof(WildBlackTiger), 0x9F32, 0x9F31, new TextDefinition(1156286), 400, 800, MeasuredBy.Weight, false, true));
            
            //Publish 102 added: Please check code 
			m_Infos.Add(new HuntingTrophyInfo(HuntType.Raptor,          typeof(Raptor),         0xA20E, 0xA20D, new TextDefinition(1095923),  400, 800, MeasuredBy.Weight, false));
			m_Infos.Add(new HuntingTrophyInfo(HuntType.SeaSerpent,      typeof(SeaSerpent),     0xA20C, 0xA20C, new TextDefinition(1018242),   200, 600, MeasuredBy.Weight, false));
			m_Infos.Add(new HuntingTrophyInfo(HuntType.Scorpion,        typeof(Scorpion),       0xA210, 0xA20F, new TextDefinition(1028420),   100, 500, MeasuredBy.Weight, false));
        }

        private HuntType m_HuntType;
        private Type m_CreatureType;
        private MeasuredBy m_MeasuredBy;
        private int m_SouthID;
        private int m_EastID;
        private TextDefinition m_Species;
        private int m_MinMeasurement;
        private int m_MaxMeasurement;
        private bool m_Complex;
        private bool m_RequiresWall;

        [CommandProperty(AccessLevel.GameMaster)]
        public HuntType HuntType { get { return m_HuntType; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Type CreatureType { get { return m_CreatureType; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public MeasuredBy MeasuredBy { get { return m_MeasuredBy; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SouthID { get { return m_SouthID; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int EastID { get { return m_EastID; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public TextDefinition Species { get { return m_Species; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MinMeasurement { get { return m_MinMeasurement; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxMeasurement { get { return m_MaxMeasurement; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Complex { get { return m_Complex; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool RequiresWall { get { return m_RequiresWall; } }

        public HuntingTrophyInfo(HuntType type, Type creatureType, int southID, TextDefinition species, int minMeasurement, int maxMeasurement, MeasuredBy measuredBy, bool complex, bool requiresWall = false)
            : this(type, creatureType, southID, southID + 1, species, minMeasurement, maxMeasurement, measuredBy, complex, requiresWall)
        {
        }

        public HuntingTrophyInfo(HuntType type, Type creatureType, int southID, int eastID, TextDefinition species, int minMeasurement, int maxMeasurement, MeasuredBy measuredBy, bool complex, bool requiresWall = false)
        {
            m_HuntType = type;
            m_CreatureType = creatureType;
            m_MeasuredBy = measuredBy;
            m_SouthID = southID;
            m_EastID = eastID;
            m_Species = species;
            m_MinMeasurement = minMeasurement;
            m_MaxMeasurement = maxMeasurement;
            m_Complex = complex;
            m_RequiresWall = requiresWall;
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
            for (int i = 0; i < HuntingTrophyInfo.Infos.Count; i++)
            {
                var info = HuntingTrophyInfo.Infos[i];

                if (info.Species.Number == number)
                {
                    return i;
                }
            }

            return 0;
        }
    }
}
