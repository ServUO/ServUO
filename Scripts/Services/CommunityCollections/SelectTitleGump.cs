using Server.Mobiles;

namespace Server.Gumps
{
    public class SelectTitleGump : Gump
    {
        private readonly PlayerMobile m_From;
        private readonly int m_Page;
        public SelectTitleGump(PlayerMobile from, int page)
            : base(50, 50)
        {
            m_From = from;
            m_Page = page;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            AddBackground(0, 0, 270, 120, 0x13BE);
            AddBackground(10, 10, 250, 100, 0xBB8);

            AddHtmlLocalized(20, 15, 230, 20, 1073994, 0x1, false, false); // Your title will be:

            if (page > -1 && page < from.RewardTitles.Count)
            {
                if (from.RewardTitles[page] is int)
                    AddHtmlLocalized(20, 35, 230, 40, (int)from.RewardTitles[page], 0x32, true, false);
                else if (from.RewardTitles[page] is string)
                    AddHtml(20, 35, 230, 40, string.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", 0x32, (string)from.RewardTitles[page]), true, false);
            }
            else
                AddHtmlLocalized(20, 35, 230, 40, 1073995, 0x32, true, false);

            AddHtmlLocalized(55, 80, 75, 20, 1073996, 0x0, false, false); // ACCEPT
            AddHtmlLocalized(170, 80, 75, 20, 1073997, 0x0, false, false); // NEXT

            AddButton(20, 80, 0xFA5, 0xFA7, (int)Buttons.Accept, GumpButtonType.Reply, 0);
            AddButton(135, 80, 0xFA5, 0xFA7, (int)Buttons.Next, GumpButtonType.Reply, 0);
        }

        private enum Buttons
        {
            Cancel,
            Next,
            Accept,
        }
        public override void OnResponse(Network.NetState state, RelayInfo info)
        {
            switch (info.ButtonID)
            {
                case (int)Buttons.Accept:
                    m_From.SelectRewardTitle(m_Page);
                    break;
                case (int)Buttons.Next:
                    if (m_Page == m_From.RewardTitles.Count - 1)
                        m_From.SendGump(new SelectTitleGump(m_From, -1));
                    else if (m_Page < m_From.RewardTitles.Count - 1 && m_Page > -1)
                        m_From.SendGump(new SelectTitleGump(m_From, m_Page + 1));
                    else
                        m_From.SendGump(new SelectTitleGump(m_From, 0));

                    break;
            }
        }
    }
}