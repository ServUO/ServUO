using System;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Services.Loyalty_System
{
    public class LoyaltyRatingGump : Gump
    {
        private readonly PlayerMobile m_From;

        public LoyaltyRatingGump(PlayerMobile from)
            : base(0, 0)
        {
            m_From = from;

            Closable = true;
            Dragable = true;
            AddPage(0);
            AddBackground(10, 200, 300, 400, 9380);
            AddLabel(40, 230, 0, "Queen's Loyalty");
            AddLabel(60, 250, 33, GetQueenTitle(from));
            AddLabel(180, 250, 33, String.Format("[ {0} ]", from.Exp));
            AddLabel(40, 300, 0, String.Format("Fame: {0}", from.Fame));
            AddLabel(40, 320, 0, String.Format("Karma: {0}", from.Karma));
        }

        private static string GetQueenTitle(PlayerMobile from)
        {
            if (from.Exp >= 10000)
                return "Noble of TerMur";
            if (from.Exp >= 2000)
                return "Citizen of TerMur";
            if (from.Exp > 0)
                return "Friend of TerMur";
            
            return "Enemy of TerMur";
        }
    }
}
