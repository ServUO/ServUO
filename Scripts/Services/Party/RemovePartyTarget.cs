using Server.Targeting;

namespace Server.Engines.PartySystem
{
    public class RemovePartyTarget : Target
    {
        public RemovePartyTarget()
            : base(8, false, TargetFlags.None)
        {
        }

        protected override void OnTarget(Mobile from, object o)
        {
            if (o is Mobile)
            {
                Mobile m = (Mobile)o;
                Party p = Party.Get(from);

                if (p == null || p.Leader != from || !p.Contains(m))
                    return;

                if (from == m)
                    from.SendLocalizedMessage(1005446); // You may only remove yourself from a party if you are not the leader.
                else
                    p.Remove(m);
            }
        }
    }
}