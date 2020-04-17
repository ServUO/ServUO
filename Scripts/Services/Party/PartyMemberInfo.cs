namespace Server.Engines.PartySystem
{
    public class PartyMemberInfo
    {
        private readonly Mobile m_Mobile;
        private bool m_CanLoot;
        public PartyMemberInfo(Mobile m)
        {
            m_Mobile = m;
            m_CanLoot = false;
        }

        public Mobile Mobile => m_Mobile;
        public bool CanLoot
        {
            get
            {
                return m_CanLoot;
            }
            set
            {
                m_CanLoot = value;
            }
        }
    }
}
