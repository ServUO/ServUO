using System;
using Server.Targeting;

namespace Server.Engines.PartySystem
{
    public class AddPartyTarget : Target
    {
        public AddPartyTarget(Mobile from)
            : base(8, false, TargetFlags.None)
        {
            from.SendLocalizedMessage(1005454); // Who would you like to add to your party?
        }

        protected override void OnTarget(Mobile from, object o)
        {
            if (o is Mobile)
            {
                Mobile m = (Mobile)o;
                Party p = Party.Get(from);
                Party mp = Party.Get(m);

                if (from == m)
                    from.SendLocalizedMessage(1005439); // You cannot add yourself to a party.
                else if (p != null && p.Leader != from)
                    from.SendLocalizedMessage(1005453); // You may only add members to the party if you are the leader.
                else if (m.Party is Mobile)
                    return;
                else if (p != null && (p.Members.Count + p.Candidates.Count) >= Party.Capacity)
                    from.SendLocalizedMessage(1008095); // You may only have 10 in your party (this includes candidates).
                else if (!m.Player && m.Body.IsHuman)
                    m.SayTo(from, 1005443); // Nay, I would rather stay here and watch a nail rust.
                else if (!m.Player)
                    from.SendLocalizedMessage(1005444); // The creature ignores your offer.
                else if (mp != null && mp == p)
                    from.SendLocalizedMessage(1005440); // This person is already in your party!
                else if (mp != null)
                    from.SendLocalizedMessage(1005441); // This person is already in a party!
                else
                    Party.Invite(from, m);
            }
            else
            {
                from.SendLocalizedMessage(1005442); // You may only add living things to your party!
            }
        }
    }
}