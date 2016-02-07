using System;
using System.Collections.Generic;

namespace Server.Engines.VeteranRewards
{
    public class RewardCategory
    {
        private readonly int m_Name;
        private readonly string m_NameString;
        private readonly List<RewardEntry> m_Entries;
        public RewardCategory(int name)
        {
            this.m_Name = name;
            this.m_Entries = new List<RewardEntry>();
        }

        public RewardCategory(string name)
        {
            this.m_NameString = name;
            this.m_Entries = new List<RewardEntry>();
        }

        public int Name
        {
            get
            {
                return this.m_Name;
            }
        }
        public string NameString
        {
            get
            {
                return this.m_NameString;
            }
        }
        public List<RewardEntry> Entries
        {
            get
            {
                return this.m_Entries;
            }
        }
    }
}