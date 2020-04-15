using Server.ContextMenus;
using Server.Mobiles;

namespace Server.Engines.Points
{
    public class LoyaltyRating : ContextMenuEntry
    {
        private readonly PlayerMobile m_From;

        public LoyaltyRating(PlayerMobile from)
            : base(1049594)
        {
            m_From = from;
        }

        public override void OnClick()
        {
            if (m_From != null)
            {
                m_From.CloseGump(typeof(LoyaltyRatingGump));
                m_From.SendGump(new LoyaltyRatingGump(m_From));
            }
        }
    }
}
