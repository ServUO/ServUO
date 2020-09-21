using System.Collections.Generic;
using System.Linq;

namespace Server.Engines.Chat
{
    public class ChatUser
    {
        private readonly Mobile m_Mobile;
        private Channel m_Channel;

        public ChatUser(Mobile m)
        {
            m_Mobile = m;
        }

        public Mobile Mobile => m_Mobile;

        public string Username => string.Format("<{0}>{1}", m_Mobile.Serial.Value, m_Mobile.Name);

        public Channel CurrentChannel { get { return m_Channel; } set { m_Channel = value; } }

        public bool IsOnline => (m_Mobile.NetState != null);

        public long NextMessage { get; set; }

        public const char NormalColorCharacter = '0';

        public char GetColorCharacter()
        {
            return NormalColorCharacter;
        }

        public bool CheckOnline()
        {
            if (IsOnline)
                return true;

            RemoveChatUser(this);
            return false;
        }

        public void SendMessage(int number, string param1 = null, string param2 = null)
        {
            SendMessage(number, m_Mobile, param1, param2);
        }

        public void SendMessage(int number, Mobile from, string param1, string param2)
        {
            if (m_Mobile.NetState != null)
                m_Mobile.Send(new ChatMessagePacket(from, number, param1, param2));
        }

        private static readonly List<ChatUser> m_Users = new List<ChatUser>();
        private static readonly Dictionary<Mobile, ChatUser> m_Table = new Dictionary<Mobile, ChatUser>();

        public static ChatUser AddChatUser(Mobile from)
        {
            ChatUser user = GetChatUser(from);

            if (user == null)
            {
                user = new ChatUser(from);

                m_Users.Add(user);
                m_Table[from] = user;

                Channel.SendChannelsTo(user);
            }

            return user;
        }

        public static void RemoveChatUser(ChatUser user)
        {
            if (user == null)
                return;

            if (m_Users.Contains(user))
            {
                ChatSystem.SendCommandTo(user.Mobile, ChatCommand.CloseChatWindow);

                if (user.m_Channel != null)
                    user.m_Channel.RemoveUser(user);

                m_Users.Remove(user);
                m_Table.Remove(user.m_Mobile);
            }
        }

        public static void RemoveChatUser(Mobile from)
        {
            ChatUser user = GetChatUser(from);

            RemoveChatUser(user);
        }

        public static ChatUser GetChatUser(Mobile from)
        {
            ChatUser c;
            m_Table.TryGetValue(from, out c);
            return c;
        }

        public static ChatUser GetChatUser(string username)
        {
            return m_Users.FirstOrDefault(user => user.Username == username);
        }

        public static void GlobalSendCommand(ChatCommand command, string param1 = null, string param2 = null)
        {
            GlobalSendCommand(command, null, param1, param2);
        }

        public static void GlobalSendCommand(ChatCommand command, ChatUser initiator, string param1 = null, string param2 = null)
        {
            foreach (ChatUser user in m_Users.ToArray())
            {
                if (user == initiator)
                    continue;

                if (user.CheckOnline())
                    ChatSystem.SendCommandTo(user.m_Mobile, command, param1, param2);
            }
        }
    }
}
