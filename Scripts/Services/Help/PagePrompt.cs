using Server.Prompts;

namespace Server.Engines.Help
{
    public class PagePrompt : Prompt
    {
        private readonly PageType m_Type;
        public PagePrompt(PageType type)
        {
            m_Type = type;
        }

        public override void OnCancel(Mobile from)
        {
            from.SendLocalizedMessage(501235, "", 0x35); // Help request aborted.
        }

        public override void OnResponse(Mobile from, string text)
        {
            from.SendLocalizedMessage(501234, "", 0x35); /* The next available Counselor/Game Master will respond as soon as possible.
            * Please check your Journal for messages every few minutes.
            */

            PageQueue.Enqueue(new PageEntry(from, text, m_Type));
        }
    }
}