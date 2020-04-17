using Server.Gumps;
using Server.Network;

namespace Server.Engines.Help
{
    public class PagePromptGump : Gump
    {
        private readonly Mobile m_From;
        private readonly PageType m_Type;
        public PagePromptGump(Mobile from, PageType type)
            : base(0, 0)
        {
            m_From = from;
            m_Type = type;

            from.CloseGump(typeof(PagePromptGump));

            AddBackground(50, 50, 540, 350, 2600);

            AddPage(0);

            AddHtmlLocalized(264, 80, 200, 24, 1062524, false, false); // Enter Description
            AddHtmlLocalized(120, 108, 420, 48, 1062638, false, false); // Please enter a brief description (up to 200 characters) of your problem:

            AddBackground(100, 148, 440, 200, 3500);
            AddTextEntry(120, 168, 400, 200, 1153, 0, "");

            AddButton(175, 355, 2074, 2075, 1, GumpButtonType.Reply, 0); // Okay
            AddButton(405, 355, 2073, 2072, 0, GumpButtonType.Reply, 0); // Cancel
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 0)
            {
                m_From.SendLocalizedMessage(501235, "", 0x35); // Help request aborted.
            }
            else
            {
                TextRelay entry = info.GetTextEntry(0);
                string text = (entry == null ? "" : entry.Text.Trim());

                if (text.Length == 0)
                {
                    m_From.SendMessage(0x35, "You must enter a description.");
                    m_From.SendGump(new PagePromptGump(m_From, m_Type));
                }
                else
                {
                    m_From.SendLocalizedMessage(501234, "", 0x35); /* The next available Counselor/Game Master will respond as soon as possible.
                    * Please check your Journal for messages every few minutes.
                    */

                    PageQueue.Enqueue(new PageEntry(m_From, text, m_Type));
                }
            }
        }
    }
}