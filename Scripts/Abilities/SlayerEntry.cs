using System;

namespace Server.Items
{
    public class SlayerEntry
    {
        private static readonly int[] m_AosTitles = new int[]
        {
            1060479, // undead slayer
            1060470, // orc slayer
            1060480, // troll slayer
            1060468, // ogre slayer
            1060472, // repond slayer
            1060462, // dragon slayer
            1060478, // terathan slayer
            1060475, // snake slayer
            1060467, // lizardman slayer
            1060473, // reptile slayer
            1060460, // demon slayer
            1060466, // gargoyle slayer
            1017396, // Balron Damnation
            1060461, // demon slayer
            1060469, // ophidian slayer
            1060477, // spider slayer
            1060474, // scorpion slayer
            1060458, // arachnid slayer
            1060465, // fire elemental slayer
            1060481, // water elemental slayer
            1060457, // air elemental slayer
            1060471, // poison elemental slayer
            1060463, // earth elemental slayer
            1060459, // blood elemental slayer
            1060476, // snow elemental slayer
            1060464, // elemental slayer
            1070855, // fey slayer
            1156240, // dinosaur slayer
            1156241, // myrmidex slayer
            1156126, // Eodon Slayer
            1156347  // Eodon Tribe Slayer
        };

        private readonly SlayerName m_Name;
        private readonly Type[] m_Types;
        private SlayerGroup m_Group;
        public SlayerEntry(SlayerName name, params Type[] types)
        {
            m_Name = name;
            m_Types = types;
        }

        public SlayerGroup Group
        {
            get
            {
                return m_Group;
            }
            set
            {
                m_Group = value;
            }
        }

        public SlayerName Name => m_Name;

        public Type[] Types => m_Types;

        public int Title
        {
            get
            {
                int[] titles = m_AosTitles;

                return titles[(int)m_Name - 1];
            }
        }

        public bool Slays(Mobile m)
        {

            if (m.SpecialSlayerMechanics)
            {
                if (m.SlayerVulnerabilities.Contains(m_Name.ToString()))
                    return true;
                else
                    return false;
            }

            Type t = m.GetType();

            for (int i = 0; i < m_Types.Length; ++i)
            {
                if (m_Types[i].IsAssignableFrom(t))
                    return true;
            }

            return false;
        }
    }
}