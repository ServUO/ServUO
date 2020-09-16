using Server.Spells.Chivalry;
using Server.Spells.Fourth;
using System;

namespace Server.Engines.InstancedPeerless
{
    public class InstanceRegion : Region
    {
        private readonly PeerlessInstance m_Owner;

        public PeerlessInstance Owner => m_Owner;

        public static void Initialize()
        {
            EventSink.Logout += EventSink_Logout;
        }

        public override bool OnBeginSpellCast(Mobile m, ISpell s)
        {
            if (s is RecallSpell || s is SacredJourneySpell)
            {
                m.SendLocalizedMessage(501802); // Thy spell doth not appear to work...
                return false;
            }

            return base.OnBeginSpellCast(m, s);
        }

        public InstanceRegion(PeerlessInstance instance)
            : base(null, instance.Map, Find(instance.EntranceLocation, instance.Map), instance.RegionBounds)
        {
            m_Owner = instance;

            Register();
        }

        public override TimeSpan GetLogoutDelay(Mobile m)
        {
            return TimeSpan.FromMinutes(10.0);
        }

        private static void EventSink_Logout(LogoutEventArgs e)
        {
            Mobile from = e.Mobile;

            InstanceRegion region = from.Region as InstanceRegion;

            if (region != null)
                region.Owner.Kick(from);
        }

        public override void OnExit(Mobile m)
        {
            m_Owner.RemoveFighter(m);
        }
    }
}
