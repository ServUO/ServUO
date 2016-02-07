using System;

namespace Server.Engines.PartySystem
{
    public class PartyMemberInfo
    {
        private readonly Mobile m_Mobile;
        private bool m_CanLoot;
        public PartyMemberInfo(Mobile m)
        {
            this.m_Mobile = m;
            this.m_CanLoot = !Core.ML;
        }

        public Mobile Mobile
        {
            get
            {
                return this.m_Mobile;
            }
        }
        public bool CanLoot
        {
            get
            {
                return this.m_CanLoot;
            }
            set
            {
                this.m_CanLoot = value;
            }
        }
    }
}