using System;

namespace Server.Gumps
{
    public delegate void WarningGumpCallback(Mobile from, bool okay, object state);

    public class WarningGump : Gump
    {
        private readonly WarningGumpCallback m_Callback;
        private readonly object m_State;
        private readonly bool m_CancelButton;
        public WarningGump(TextDefinition header, int headerColor, TextDefinition content, int contentColor, int width, int height, WarningGumpCallback callback, object state)
            : this(header, headerColor, content, contentColor, width, height, callback, state, true)
        {
        }

        public WarningGump(TextDefinition header, int headerColor, TextDefinition content, int contentColor, int width, int height, WarningGumpCallback callback, object state, bool cancelButton, int ok = 1011036, int cancel = 1011012)
            : base((640 - width) / 2, (480 - height) / 2)
        {
            m_Callback = callback;
            m_State = state;
            m_CancelButton = cancelButton;

            Closable = false;

            AddPage(0);

            AddBackground(0, 0, width, height, 5054);

            AddImageTiled(10, 10, width - 20, 20, 2624);
            AddAlphaRegion(10, 10, width - 20, 20);

            if (header.Number > 0)
            {
                AddHtmlLocalized(10, 10, width - 20, 20, header.Number, headerColor, false, false);
            }
            else if (header.String != null)
            {
                AddHtml(10, 10, width - 20, height - 80, String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", headerColor, header.String), false, true);
            }

            AddImageTiled(10, 40, width - 20, height - 80, 2624);
            AddAlphaRegion(10, 40, width - 20, height - 80);

            if (content.Number > 0)
            {
                AddHtmlLocalized(10, 40, width - 20, height - 80, content.Number, contentColor, false, true);
            }
            else if (content.String != null)
            {
                AddHtml(10, 40, width - 20, height - 80, String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", contentColor, content.String), false, true);
            }

            AddImageTiled(10, height - 30, width - 20, 20, 2624);
            AddAlphaRegion(10, height - 30, width - 20, 20);

            AddButton(10, height - 30, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(40, height - 30, 170, 20, ok, 32767, false, false); // OKAY

            if (m_CancelButton)
            {
                AddButton(10 + ((width - 20) / 2), height - 30, 4005, 4007, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(40 + ((width - 20) / 2), height - 30, 170, 20, cancel, 32767, false, false); // CANCEL
            }
        }

        public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 1 && m_Callback != null)
                m_Callback(sender.Mobile, true, m_State);
            else if (m_Callback != null)
                m_Callback(sender.Mobile, false, m_State);
        }
    }
}