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
        MyrmidexDrone,
        Najasaurus,
        Anchisaur,
        Allosaurus,
        Dimetrosaur,
        Saurosaurus
    }

    public enum MeasuredBy
    {
        Weight,
        Length,
        Wingspan
    }

    public class HuntingTrophyInfo
    {
        private static List<HuntingTrophyInfo> m_Infos = new List<HuntingTrophyInfo>();
        public static List<HuntingTrophyInfo> Infos { get { return m_Infos; } }

        public static void Initialize()
        {
            m_Infos.Add(new HuntingTrophyInfo(HuntType.GrizzlyBear, typeof(GrizzlyBear),    0x9A26, new TextDefinition(1015242), 400, 800,  MeasuredBy.Weight));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.GrayWolf,    typeof(GreyWolf),       0x9A28, new TextDefinition(1029681), 70,  190,  MeasuredBy.Weight));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Cougar,      typeof(Cougar),         0x9A2A, new TextDefinition(1029603), 50,  140,  MeasuredBy.Weight));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Turkey,      typeof(Turkey),         0x9A2C, new TextDefinition(1155714), 10,  55,   MeasuredBy.Weight));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Bull,        typeof(Bull),           0x9A2E, new TextDefinition(1072495), 500, 1000, MeasuredBy.Weight));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Boar,        typeof(Boar),           0x9A30, new TextDefinition(1155715), 100, 400,  MeasuredBy.Weight));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Walrus,      typeof(Walrus),         0x9A32, new TextDefinition(1155716), 600, 1500, MeasuredBy.Weight));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Alligator,   typeof(Alligator),      0x9A34, new TextDefinition(1155717), 5,   15,   MeasuredBy.Length));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Eagle,       typeof(Eagle),          0x9A36, new TextDefinition(1072461), 5,   15,   MeasuredBy.Wingspan));
        
            // Pub 91 Additions - Not Yet yo
            /*m_Infos.Add(new HuntingTrophyInfo(HuntType.MyrmidexDrone,   typeof(MyrmidexDrone),  0x9C00, new TextDefinition(1156134), 400, 800, MeasuredBy.Weight, true));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Najasaurus,      typeof(Najasaurus),     0x9C02, new TextDefinition(1156283), 400, 800, MeasuredBy.Weight, true));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Anchisaur,       typeof(Anchisaur),      0x9C08, new TextDefinition(1156284), 400, 800, MeasuredBy.Weight));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Allosaurus,      typeof(Allosaurus),     0x9C0A, new TextDefinition(1156280), 400, 800, MeasuredBy.Weight));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Dimetrosaur,     typeof(Dimetrosaur),    0x9C0C, new TextDefinition(1156279), 400, 800, MeasuredBy.Weight));
            m_Infos.Add(new HuntingTrophyInfo(HuntType.Saurosaurus,     typeof(Saurosaurus),    0x9C0E, new TextDefinition(1156289), 400, 800, MeasuredBy.Weight));*/
        }

        private HuntType m_HuntType;
        private Type m_CreatureType;
        private MeasuredBy m_MeasuredBy;
        private int m_SouthID;
        private TextDefinition m_Species;
        private int m_MinMeasurement;
        private int m_MaxMeasurement;
        private bool m_Complex;

        public HuntType HuntType { get { return m_HuntType; } }
        public Type CreatureType { get { return m_CreatureType; } }
        public MeasuredBy MeasuredBy { get { return m_MeasuredBy; } }
        public int SouthID { get { return m_SouthID; } }
        public TextDefinition Species { get { return m_Species; } }
        public int MinMeasurement { get { return m_MinMeasurement; } }
        public int MaxMeasurement { get { return m_MaxMeasurement; } }
        public bool Complex { get { return m_Complex; } }

        public HuntingTrophyInfo(HuntType type, Type creatureType, int id, TextDefinition species, int minMeasurement, int maxMeasurement, MeasuredBy measuredBy, bool complex = false)
        {
            m_HuntType = type;
            m_CreatureType = creatureType;
            m_MeasuredBy = measuredBy;
            m_SouthID = id;
            m_Species = species;
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
    }
}