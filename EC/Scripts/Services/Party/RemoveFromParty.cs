using System;
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
            this.m_From = from;
            this.m_Target = target;
        }

        public override void OnClick()
        { 
            Party p = Party.Get(this.m_From);

            if (p == null || p.Leader != this.m_From || !p.Contains(this.m_Target))
                return;

            if (this.m_From == this.m_Target)
                this.m_From.SendLocalizedMessage(1005446); // You may only remove yourself from a party if you are not the leader.
            else
                p.Remove(this.m_Target);
        }
    }
}