using System;

namespace Server.Factions
{
    public class SilverGivenEntry
    {
        public static readonly TimeSpan ExpirePeriod = TimeSpan.FromHours(3.0);
        private readonly Mobile m_GivenTo;
        private readonly DateTime m_TimeOfGift;
        public SilverGivenEntry(Mobile givenTo)
        {
            this.m_GivenTo = givenTo;
            this.m_TimeOfGift = DateTime.UtcNow;
        }

        public Mobile GivenTo
        {
            get
            {
                return this.m_GivenTo;
            }
        }
        public DateTime TimeOfGift
        {
            get
            {
                return this.m_TimeOfGift;
            }
        }
        public bool IsExpired
        {
            get
            {
                return (this.m_TimeOfGift + ExpirePeriod) < DateTime.UtcNow;
            }
        }
    }
}