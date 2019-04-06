using System;

namespace Server.Factions
{
    public class RankDefinition
    {
        private readonly int m_Rank;
        private readonly int m_Required;
        private readonly int m_MaxWearables;
        private readonly TextDefinition m_Title;

        public RankDefinition(int rank, int required, int maxWearables, TextDefinition title)
        {
            m_Rank = rank;
            m_Required = required;
            m_Title = title;
            m_MaxWearables = maxWearables;
        }

        public int Rank
        {
            get
            {
                return m_Rank;
            }
        }
        public int Required
        {
            get
            {
                return m_Required;
            }
        }
        public int MaxWearables
        {
            get
            {
                return m_MaxWearables;
            }
        }
        public TextDefinition Title
        {
            get
            {
                return m_Title;
            }
        }
    }
}