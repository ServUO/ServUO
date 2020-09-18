using Server.Items;
using System;

namespace Server.Spells
{
    public class Reagent
    {
        private static readonly Type[] m_Types = 
        {
            typeof(BlackPearl),
            typeof(Bloodmoss),
            typeof(Garlic),
            typeof(Ginseng),
            typeof(MandrakeRoot),
            typeof(Nightshade),
            typeof(SulfurousAsh),
            typeof(SpidersSilk),
            typeof(BatWing),
            typeof(GraveDust),
            typeof(DaemonBlood),
            typeof(NoxCrystal),
            typeof(PigIron),
            typeof(Bone),
            typeof(DragonBlood),
            typeof(FertileDirt),
            typeof(DaemonBone),
        };
        public static Type BlackPearl
        {
            get
            {
                return m_Types[0];
            }
            set
            {
                m_Types[0] = value;
            }
        }
        public static Type Bloodmoss
        {
            get
            {
                return m_Types[1];
            }
            set
            {
                m_Types[1] = value;
            }
        }
        public static Type Garlic
        {
            get
            {
                return m_Types[2];
            }
            set
            {
                m_Types[2] = value;
            }
        }
        public static Type Ginseng
        {
            get
            {
                return m_Types[3];
            }
            set
            {
                m_Types[3] = value;
            }
        }
        public static Type MandrakeRoot
        {
            get
            {
                return m_Types[4];
            }
            set
            {
                m_Types[4] = value;
            }
        }
        public static Type Nightshade
        {
            get
            {
                return m_Types[5];
            }
            set
            {
                m_Types[5] = value;
            }
        }
        public static Type SulfurousAsh
        {
            get
            {
                return m_Types[6];
            }
            set
            {
                m_Types[6] = value;
            }
        }
        public static Type SpidersSilk
        {
            get
            {
                return m_Types[7];
            }
            set
            {
                m_Types[7] = value;
            }
        }
        public static Type BatWing
        {
            get
            {
                return m_Types[8];
            }
            set
            {
                m_Types[8] = value;
            }
        }
        public static Type GraveDust
        {
            get
            {
                return m_Types[9];
            }
            set
            {
                m_Types[9] = value;
            }
        }
        public static Type DaemonBlood
        {
            get
            {
                return m_Types[10];
            }
            set
            {
                m_Types[10] = value;
            }
        }
        public static Type NoxCrystal
        {
            get
            {
                return m_Types[11];
            }
            set
            {
                m_Types[11] = value;
            }
        }
        public static Type PigIron
        {
            get
            {
                return m_Types[12];
            }
            set
            {
                m_Types[12] = value;
            }
        }
        public static Type Bone
        {
            get
            {
                return m_Types[13];
            }
            set
            {
                m_Types[13] = value;
            }
        }
        public static Type DragonBlood
        {
            get
            {
                return m_Types[14];
            }
            set
            {
                m_Types[14] = value;
            }
        }
        public static Type FertileDirt
        {
            get
            {
                return m_Types[15];
            }
            set
            {
                m_Types[15] = value;
            }
        }
        public static Type DaemonBone
        {
            get
            {
                return m_Types[16];
            }
            set
            {
                m_Types[16] = value;
            }
        }
        public Type[] Types => m_Types;
    }
}