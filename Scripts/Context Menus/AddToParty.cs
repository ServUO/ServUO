using System;
using Server.Engines.PartySystem;

namespace Server.ContextMenus
{
    public class AddToPartyEntry : ContextMenuEntry
    {
        private readonly Mobile m_From;
        private readonly Mobile m_Target;
        public AddToPartyEntry(Mobile from, Mobile target)
            : base(0197, 12)
        {
            this.m_From = from;
            this.m_Target = target;
        }

        public override void OnClick()
        { 
            Party p = Party.Get(this.m_From);
            Party mp = Party.Get(this.m_Target);

            if (this.m_From == this.m_Target)
                this.m_From.SendLocalizedMessage(1005439); // You cannot add yourself to a party.
            else if (p != null && p.Leader != this.m_From)
                this.m_From.SendLocalizedMessage(1005453); // You may only add members to the party if you are the leader.
            else if (p != null && (p.Members.Count + p.Candidates.Count) >= Party.Capacity)
                this.m_From.SendLocalizedMessage(1008095); // You may only have 10 in your party (this includes candidates).
            else if (!this.m_Target.Player)
                this.m_From.SendLocalizedMessage(1005444); // The creature ignores your offer.
            else if (mp != null && mp == p)
                this.m_From.SendLocalizedMessage(1005440); // This person is already in your party!
            else if (mp != null)
                this.m_From.SendLocalizedMessage(1005441); // This person is already in a party!
            else
                Party.Invite(this.m_From, this.m_Target);
        }
    }
}