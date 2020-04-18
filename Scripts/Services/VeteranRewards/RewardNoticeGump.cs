using Server.Gumps;
using Server.Network;

namespace Server.Engines.VeteranRewards
{
    public class RewardNoticeGump : Gump
    {
        private readonly Mobile m_From;
        public RewardNoticeGump(Mobile from)
            : base(0, 0)
        {
            m_From = from;

            from.CloseGump(typeof(RewardNoticeGump));

            AddPage(0);

            AddBackground(10, 10, 500, 135, 2600);

            /* You have reward items available.
            * Click 'ok' below to get the selection menu or 'cancel' to be prompted upon your next login.
            */
            AddHtmlLocalized(52, 35, 420, 55, 1006046, true, true);

            AddButton(60, 95, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(95, 96, 150, 35, 1006044, false, false); // Ok

            AddButton(285, 95, 4017, 4019, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(320, 96, 150, 35, 1006045, false, false); // Cancel
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1)
                m_From.SendGump(new RewardChoiceGump(m_From));
        }
    }
}