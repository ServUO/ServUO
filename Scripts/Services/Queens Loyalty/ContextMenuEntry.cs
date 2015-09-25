/*using Server;
using System;
using Server.Mobiles;
using Server.ContextMenus;
using Server.Gumps;

namespace Server.Engines.QueensLoyalty
{
    public class QueensLoyaltyEntry : ContextMenuEntry
    {
        private Mobile m_From;

        public QueensLoyaltyEntry(Mobile from) : base(1049594, -1)
        {
            m_From = from;
        }

        public override void OnClick()
        {
            if(m_From is PlayerMobile)
                m_From.SendGump(new LoyaltyGump(m_From as PlayerMobile));
        }
    }
}*/