using Server;
using System;

namespace Server.Gumps
{
    public class BasicInfoGump : Gump
    {
        public BasicInfoGump(TextDefinition body)
            : this(body, null)
        {
        }

        public BasicInfoGump(TextDefinition body, TextDefinition title)
            : base(20, 20)
        {
            AddBackground(0, 0, 300, 450, 9200);

            if (title != null)
            {
                AddImageTiled(10, 10, 280, 20, 2702);
                AddImageTiled(10, 40, 280, 400, 2702);

                if (title.Number > 0)
                    AddHtmlLocalized(12, 10, 275, 20, title.Number, 0xFFFFFF, false, false);
                else if (title.String != null)
                    AddHtml(12, 10, 275, 20, String.Format("<BASEFONT COLOR=WHITE>{0}</BASEFONT>", title.String), false, false);

                if (body.Number > 0)
                    AddHtmlLocalized(12, 40, 275, 390, body.Number, 0xFFFFFF, false, false);
                else if (body.String != null)
                    AddHtml(12, 40, 275, 390, String.Format("<BASEFONT COLOR=WHITE>{0}</BASEFONT>", body.String), false, false);
            }
            else
            {
                AddImageTiled(10, 10, 280, 430, 2702);

                if (body.Number > 0)
                    AddHtmlLocalized(12, 10, 275, 425, (int)body, 0xFFFFFF, false, false);
                else if (body.String != null)
                    AddHtml(12, 10, 275, 425, String.Format("<BASEFONT COLOR=WHITE>{0}</BASEFONT>", body.String), false, false);
            }
        }
    }

    public class BasicConfirmGump<T> : Gump
    {
        private readonly Action<Mobile, T> m_ConfirmCallback;
        private readonly Action<Mobile, T> m_CancelCallback;
        private readonly T m_State;

        public BasicConfirmGump(TextDefinition warning, Action<Mobile, T> confirmcallback, T state, Action<Mobile, T> cancelcallback = null, int width = 290, int height = 95)
            : base(100, 100)
        {
            m_ConfirmCallback = confirmcallback;
            m_CancelCallback = cancelcallback;
            m_State = state;

            AddBackground(0, 0, width, height, 9200);
            AddImageTiled(5, 5, width - 10, height - 30, 2702);

            AddHtmlLocalized(40, height - 25, 100, 20, 1011012, 0xFFFF, false, false);
            AddButton(5, height - 25, 0xFB1, 0xFB2, 1, GumpButtonType.Reply, 0);

            AddHtml(225, height - 25, 100, 20, "<basefont color=#FFFFFF>OK", false, false);
            AddButton(190, height - 25, 0xFB1, 0xFB2, 2, GumpButtonType.Reply, 0);

            if (warning.Number > 0)
                AddHtmlLocalized(10, 10, width - 20, height - 50, warning.Number, 0xFFFF, false, false);
            else
                AddHtml(10, 10, width - 20, height - 50, String.Format("<BASEFONT COLOR=#FFFFFF>{0}</BASEFONT>", warning.String), false, true);
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            Mobile m = sender.Mobile;

            if (info.ButtonID == 1 && m_CancelCallback != null)
            {
                m_CancelCallback(m, m_State);
            }
            else if (info.ButtonID == 2 && m_ConfirmCallback != null)
            {
                m_ConfirmCallback(m, m_State);
            }
        }
    }
}
