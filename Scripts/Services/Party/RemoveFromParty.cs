using Server.Engines.PartySystem;

namespace Server.ContextMenus
{
    public class RemoveFromPartyEntry : ContextMenuEntry
    {
        private readonly Mobile m_From;
        private readonly Mobile m_Target;
        public RemoveFromPartyEntry(Mobile from, Mobile target)
            : base(0198, 12)
        {
            m_From = from;
            m_Target = target;
        }

        public override void OnClick()
        {
            Party p = Party.Get(m_From);

            if (p == null || p.Leader != m_From || !p.Contains(m_Target))
                return;

            if (m_From == m_Target)
                m_From.SendLocalizedMessage(1005446); // You may only remove yourself from a party if you are not the leader.
            else
                p.Remove(m_Target);
        }
    }
}