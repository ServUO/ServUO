using System;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.SkillHandlers
{
    public class ImbuingDefinition
    {
        private object m_Attribute;
        private int m_AttributeName;
        private int m_Weight;
        private Type m_PrimaryRes;
        private Type m_GemRes;
        private Type m_SpecialRes;
        private int m_PrimaryName;
        private int m_GemName;
        private int m_SpecialName;
        private int m_MaxIntensity;
        private int m_IncAmount;
        private int m_Description;

        public object Attribute { get { return m_Attribute; } }
        public int AttributeName { get { return m_AttributeName; } }
        public int Weight { get { return m_Weight; } }
        public Type PrimaryRes { get { return m_PrimaryRes; } }
        public Type GemRes { get { return m_GemRes; } }
        public Type SpecialRes { get { return m_SpecialRes; } }
        public int PrimaryName { get { return m_PrimaryName; } }
        public int GemName { get { return m_GemName; } }
        public int SpecialName { get { return m_SpecialName; } }
        public int MaxIntensity { get { return m_MaxIntensity; } }
        public int IncAmount { get { return m_IncAmount; } }
        public int Description { get { return m_Description; } }

        public bool Melee { get; set; }
        public bool Ranged { get; set; }
        public bool Armor { get; set; }
        public bool Shield { get; set; }
        public bool Jewels { get; set; }

        public ImbuingDefinition(object attribute, int attributeName, int weight, Type pRes, Type gRes, Type spRes, int mInt, int inc, int desc, bool melee = false, bool ranged = false, bool armor = false, bool shield = false, bool jewels = false)
        {
            m_Attribute = attribute;
            m_AttributeName = attributeName;
            m_Weight = weight;
            m_PrimaryRes = pRes;
            m_GemRes = gRes;
            m_SpecialRes = spRes;

            m_PrimaryName = GetLocalization(pRes);
            m_GemName = GetLocalization(gRes);
            m_SpecialName = GetLocalization(spRes);

            m_MaxIntensity = mInt;
            m_IncAmount = inc;
            m_Description = desc;

            Melee = melee;
            Ranged = ranged;
            Armor = armor;
            Shield = shield;
            Jewels = jewels;
        }

        public int GetLocalization(Type type)
        {
            if (type == null)
                return 0;

            if (type == typeof(Tourmaline)) return 1023864;
            if (type == typeof(Ruby)) return 1023859;
            if (type == typeof(Diamond)) return 1023878;
            if (type == typeof(Sapphire)) return 1023857;
            if (type == typeof(Citrine)) return 1023861;
            if (type == typeof(Emerald)) return 1023856;
            if (type == typeof(StarSapphire)) return 1023855;
            if (type == typeof(Amethyst)) return 1023862;

            if (type == typeof(RelicFragment)) return 1031699;
            if (type == typeof(EnchantedEssence)) return 1031698;
            if (type == typeof(MagicalResidue)) return 1031697;

            if (type == typeof(DarkSapphire)) return 1032690;
            if (type == typeof(Turquoise)) return 1032691;
            if (type == typeof(PerfectEmerald)) return 1032692;
            if (type == typeof(EcruCitrine)) return 1032693;
            if (type == typeof(WhitePearl)) return 1032694;
            if (type == typeof(FireRuby)) return 1032695;
            if (type == typeof(BlueDiamond)) return 1032696;
            if (type == typeof(BrilliantAmber)) return 1032697;

            if (type == typeof(ParasiticPlant)) return 1032688;
            if (type == typeof(LuminescentFungi)) return 1032689;

            if (LocBuffer == null)
                LocBuffer = new Dictionary<Type, int>();

            if (LocBuffer.ContainsKey(type))
                return LocBuffer[type];
 
            Item item = Loot.Construct(type);

            if (item != null)
            {
                LocBuffer[type] = item.LabelNumber;
                item.Delete();

                return LocBuffer[type]; ;
            }

            if(type != null)
                Console.WriteLine("Warning, missing name cliloc for type {0}.", type.Name);
            return -1;
        }

        public Dictionary<Type, int> LocBuffer { get; set; }
    }
}