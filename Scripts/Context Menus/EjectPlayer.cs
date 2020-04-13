using System;
using Server.Multis;

namespace Server.ContextMenus
{
    public class EjectPlayerEntry : ContextMenuEntry
    {
        private readonly Mobile m_From;
        private readonly Mobile m_Target;
        private readonly BaseHouse m_TargetHouse;
		
        public EjectPlayerEntry(Mobile from, Mobile target)
            : base(6206, 12)
        {
            m_From = from;
            m_Target = target;
            m_TargetHouse = BaseHouse.FindHouseAt(m_Target);
        }

        public override void OnClick()
        { 
            if (!m_From.Alive || m_TargetHouse.Deleted || !m_TargetHouse.IsFriend(m_From))
                return;

            if (m_Target is Mobile)
            {
                m_TargetHouse.Kick(m_From, (Mobile)m_Target);
            }
        }
    }
}