using System;

namespace Server.Engines.VeteranRewards
{
    public class RewardList
    {
        private readonly TimeSpan m_Age;
        private readonly RewardEntry[] m_Entries;
        public RewardList(TimeSpan interval, int index, RewardEntry[] entries)
        {
            this.m_Age = TimeSpan.FromDays(interval.TotalDays * index);
            this.m_Entries = entries;

            for (int i = 0; i < entries.Length; ++i)
                entries[i].List = this;
        }

        public TimeSpan Age
        {
            get
            {
                return this.m_Age;
            }
        }
        public RewardEntry[] Entries
        {
            get
            {
                return this.m_Entries;
            }
        }
    }
}