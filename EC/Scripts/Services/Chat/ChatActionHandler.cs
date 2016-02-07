using System;

namespace Server.Engines.Chat
{
    public delegate void OnChatAction(ChatUser from, Channel channel, string param);

    public class ChatActionHandler
    {
        private readonly bool m_RequireModerator;
        private readonly bool m_RequireConference;
        private readonly OnChatAction m_Callback;
        public ChatActionHandler(bool requireModerator, bool requireConference, OnChatAction callback)
        {
            this.m_RequireModerator = requireModerator;
            this.m_RequireConference = requireConference;
            this.m_Callback = callback;
        }

        public bool RequireModerator
        {
            get
            {
                return this.m_RequireModerator;
            }
        }
        public bool RequireConference
        {
            get
            {
                return this.m_RequireConference;
            }
        }
        public OnChatAction Callback
        {
            get
            {
                return this.m_Callback;
            }
        }
    }
}