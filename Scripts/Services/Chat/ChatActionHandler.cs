using System;

namespace Server.Engines.Chat
{
    public delegate void OnChatAction(ChatUser from, Channel channel, string param);

    public class ChatActionHandler
    {
        private bool m_RequireConference;
        private OnChatAction m_Callback;

        public bool RequireConference { get { return m_RequireConference; } }
        public OnChatAction Callback { get { return m_Callback; } }

        public ChatActionHandler(bool requireConference, OnChatAction callback)
        {
            m_RequireConference = requireConference;
            m_Callback = callback;
        }
    }
}
