using System;
using System.Collections;

namespace Server.Engines.PartySystem
{
    public class DeclineTimer : Timer
    {
        private static readonly Hashtable m_Table = new Hashtable();
        private readonly Mobile m_Mobile;
        private readonly Mobile m_Leader;
        private DeclineTimer(Mobile m, Mobile leader)
            : base(TimeSpan.FromSeconds(30.0))
        {
            this.m_Mobile = m;
            this.m_Leader = leader;
        }

        public static void Start(Mobile m, Mobile leader)
        {
            DeclineTimer t = (DeclineTimer)m_Table[m];

            if (t != null)
                t.Stop();

            m_Table[m] = t = new DeclineTimer(m, leader);
            t.Start();
        }

        protected override void OnTick()
        {
            m_Table.Remove(this.m_Mobile);

            if (this.m_Mobile.Party == this.m_Leader && PartyCommands.Handler != null)
                PartyCommands.Handler.OnDecline(this.m_Mobile, this.m_Leader);
        }
    }
}