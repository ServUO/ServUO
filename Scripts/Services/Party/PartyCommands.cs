namespace Server.Engines.PartySystem
{
    public class PartyCommandHandlers : PartyCommands
    {
        public static void Initialize()
        {
            Handler = new PartyCommandHandlers();
        }

        public override void OnAdd(Mobile from)
        {
            Party p = Party.Get(from);

            if (p != null && p.Leader != from)
                from.SendLocalizedMessage(1005453); // You may only add members to the party if you are the leader.
            else if (p != null && (p.Members.Count + p.Candidates.Count) >= Party.Capacity)
                from.SendLocalizedMessage(1008095); // You may only have 10 in your party (this includes candidates).
            else
                from.Target = new AddPartyTarget(from);
        }

        public override void OnRemove(Mobile from, Mobile target)
        {
            Party p = Party.Get(from);

            if (p == null)
            {
                from.SendLocalizedMessage(3000211); // You are not in a party.
                return;
            }

            if (p.Leader == from && target == null)
            {
                from.SendLocalizedMessage(1005455); // Who would you like to remove from your party?
                from.Target = new RemovePartyTarget();
            }
            else if ((p.Leader == from || from == target) && p.Contains(target))
            {
                p.Remove(target);
            }
        }

        public override void OnPrivateMessage(Mobile from, Mobile target, string text)
        {
            if (text.Length > 128 || (text = text.Trim()).Length == 0)
                return;

            Party p = Party.Get(from);

            if (p != null && p.Contains(target))
                p.SendPrivateMessage(from, target, text);
            else
                from.SendLocalizedMessage(3000211); // You are not in a party.
        }

        public override void OnPublicMessage(Mobile from, string text)
        {
            if (text.Length > 128 || (text = text.Trim()).Length == 0)
                return;

            Party p = Party.Get(from);

            if (p != null)
                p.SendPublicMessage(from, text);
            else
                from.SendLocalizedMessage(3000211); // You are not in a party.
        }

        public override void OnSetCanLoot(Mobile from, bool canLoot)
        {
            Party p = Party.Get(from);

            if (p == null)
            {
                from.SendLocalizedMessage(3000211); // You are not in a party.
            }
            else
            {
                PartyMemberInfo mi = p[from];

                if (mi != null)
                {
                    mi.CanLoot = canLoot;

                    if (canLoot)
                        from.SendLocalizedMessage(1005447); // You have chosen to allow your party to loot your corpse.
                    else
                        from.SendLocalizedMessage(1005448); // You have chosen to prevent your party from looting your corpse.
                }
            }
        }

        public override void OnAccept(Mobile from, Mobile sentLeader)
        {
            Mobile leader = from.Party as Mobile;
            from.Party = null;

            Party p = Party.Get(leader);

            if (leader == null || p == null || !p.Candidates.Contains(from))
                from.SendLocalizedMessage(3000222); // No one has invited you to be in a party.
            else if ((p.Members.Count + p.Candidates.Count) <= Party.Capacity)
                p.OnAccept(from);
        }

        public override void OnDecline(Mobile from, Mobile sentLeader)
        {
            Mobile leader = from.Party as Mobile;
            from.Party = null;

            Party p = Party.Get(leader);

            if (leader == null || p == null || !p.Candidates.Contains(from))
                from.SendLocalizedMessage(3000222); // No one has invited you to be in a party.
            else
                p.OnDecline(from, leader);
        }
    }
}