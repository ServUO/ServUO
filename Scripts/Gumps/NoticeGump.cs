using System;

namespace Server.Gumps
{
    public delegate void NoticeGumpCallback(Mobile from, object state);

    public class NoticeGump : Gump
    {
        private readonly NoticeGumpCallback m_Callback;
        private readonly object m_State;
        public NoticeGump(int header, int headerColor, object content, int contentColor, int width, int height, NoticeGumpCallback callback, object state)
            : base((640 - width) / 2, (480 - height) / 2)
        {
            this.m_Callback = callback;
            this.m_State = state;

            this.Closable = false;

            this.AddPage(0);

            this.AddBackground(0, 0, width, height, 5054);

            this.AddImageTiled(10, 10, width - 20, 20, 2624);
            this.AddAlphaRegion(10, 10, width - 20, 20);
            this.AddHtmlLocalized(10, 10, width - 20, 20, header, headerColor, false, false);

            this.AddImageTiled(10, 40, width - 20, height - 80, 2624);
            this.AddAlphaRegion(10, 40, width - 20, height - 80);

            if (content is int)
                this.AddHtmlLocalized(10, 40, width - 20, height - 80, (int)content, contentColor, false, true);
            else if (content is string)
                this.AddHtml(10, 40, width - 20, height - 80, String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", contentColor, content), false, true);

            this.AddImageTiled(10, height - 30, width - 20, 20, 2624);
            this.AddAlphaRegion(10, height - 30, width - 20, 20);
            this.AddButton(10, height - 30, 4005, 4007, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(40, height - 30, 120, 20, 1011036, 32767, false, false); // OKAY
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1 && this.m_Callback != null)
                this.m_Callback(sender.Mobile, this.m_State);
        }
    }
}