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
            this.m_Rank = rank;
            this.m_Required = required;
            this.m_Title = title;
            this.m_MaxWearables = maxWearables;
        }

        public int Rank
        {
            get
            {
                return this.m_Rank;
            }
        }
        public int Required
        {
            get
            {
                return this.m_Required;
            }
        }
        public int MaxWearables
        {
            get
            {
                return this.m_MaxWearables;
            }
        }
        public TextDefinition Title
        {
            get
            {
                return this.m_Title;
            }
        }
    }
}