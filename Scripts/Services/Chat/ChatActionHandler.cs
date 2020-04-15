namespace Server.Engines.Chat
{
    public delegate void OnChatAction(ChatUser from, Channel channel, string param);

    public class ChatActionHandler
    {
        private readonly bool m_RequireConference;
        private readonly OnChatAction m_Callback;

        public bool RequireConference => m_RequireConference;
        public OnChatAction Callback => m_Callback;

        public ChatActionHandler(bool requireConference, OnChatAction callback)
        {
            m_RequireConference = requireConference;
            m_Callback = callback;
        }
    }
}
