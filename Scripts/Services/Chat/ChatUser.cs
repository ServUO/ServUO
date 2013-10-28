using System;
using System.Collections.Generic;
using Server.Accounting;

namespace Server.Engines.Chat
{
    public class ChatUser
    {
        public const char NormalColorCharacter = '0';
        public const char ModeratorColorCharacter = '1';
        public const char VoicedColorCharacter = '2';
        private static readonly List<ChatUser> m_Users = new List<ChatUser>();
        private static readonly Dictionary<Mobile, ChatUser> m_Table = new Dictionary<Mobile, ChatUser>();
        private readonly Mobile m_Mobile;
        private readonly List<ChatUser> m_Ignored;
        private readonly List<ChatUser> m_Ignoring;
        private Channel m_Channel;
        private bool m_Anonymous;
        private bool m_IgnorePrivateMessage;
        public ChatUser(Mobile m)
        {
            this.m_Mobile = m;
            this.m_Ignored = new List<ChatUser>();
            this.m_Ignoring = new List<ChatUser>();
        }

        public Mobile Mobile
        {
            get
            {
                return this.m_Mobile;
            }
        }
        public List<ChatUser> Ignored
        {
            get
            {
                return this.m_Ignored;
            }
        }
        public List<ChatUser> Ignoring
        {
            get
            {
                return this.m_Ignoring;
            }
        }
        public string Username
        {
            get
            {
                Account acct = this.m_Mobile.Account as Account;

                if (acct != null)
                    return acct.GetTag("ChatName");

                return null;
            }
            set
            {
                Account acct = this.m_Mobile.Account as Account;

                if (acct != null)
                    acct.SetTag("ChatName", value);
            }
        }
        public Channel CurrentChannel
        {
            get
            {
                return this.m_Channel;
            }
            set
            {
                this.m_Channel = value;
            }
        }
        public bool IsOnline
        {
            get
            {
                return (this.m_Mobile.NetState != null);
            }
        }
        public bool Anonymous
        {
            get
            {
                return this.m_Anonymous;
            }
            set
            {
                this.m_Anonymous = value;
            }
        }
        public bool IgnorePrivateMessage
        {
            get
            {
                return this.m_IgnorePrivateMessage;
            }
            set
            {
                this.m_IgnorePrivateMessage = value;
            }
        }
        public bool IsModerator
        {
            get
            {
                return (this.m_Channel != null && this.m_Channel.IsModerator(this));
            }
        }
        public static ChatUser AddChatUser(Mobile from)
        {
            ChatUser user = GetChatUser(from);

            if (user == null)
            {
                user = new ChatUser(from);

                m_Users.Add(user);
                m_Table[from] = user;

                Channel.SendChannelsTo(user);

                List<Channel> list = Channel.Channels;

                for (int i = 0; i < list.Count; ++i)
                {
                    Channel c = list[i];

                    if (c.AddUser(user))
                        break;
                }
                //ChatSystem.SendCommandTo( user.m_Mobile, ChatCommand.AddUserToChannel, user.GetColorCharacter() + user.Username );
            }

            return user;
        }

        public static void RemoveChatUser(ChatUser user)
        {
            if (user == null)
                return;

            for (int i = 0; i < user.m_Ignoring.Count; ++i)
                user.m_Ignoring[i].RemoveIgnored(user);

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
            for (int i = 0; i < m_Users.Count; ++i)
            {
                ChatUser user = m_Users[i];

                if (user.Username == username)
                    return user;
            }

            return null;
        }

        public static void GlobalSendCommand(ChatCommand command)
        {
            GlobalSendCommand(command, null, null, null);
        }

        public static void GlobalSendCommand(ChatCommand command, string param1)
        {
            GlobalSendCommand(command, null, param1, null);
        }

        public static void GlobalSendCommand(ChatCommand command, string param1, string param2)
        {
            GlobalSendCommand(command, null, param1, param2);
        }

        public static void GlobalSendCommand(ChatCommand command, ChatUser initiator)
        {
            GlobalSendCommand(command, initiator, null, null);
        }

        public static void GlobalSendCommand(ChatCommand command, ChatUser initiator, string param1)
        {
            GlobalSendCommand(command, initiator, param1, null);
        }

        public static void GlobalSendCommand(ChatCommand command, ChatUser initiator, string param1, string param2)
        {
            for (int i = 0; i < m_Users.Count; ++i)
            {
                ChatUser user = m_Users[i];

                if (user == initiator)
                    continue;

                if (user.CheckOnline())
                    ChatSystem.SendCommandTo(user.m_Mobile, command, param1, param2);
            }
        }

        public char GetColorCharacter()
        {
            if (this.m_Channel != null && this.m_Channel.IsModerator(this))
                return ModeratorColorCharacter;

            if (this.m_Channel != null && this.m_Channel.IsVoiced(this))
                return VoicedColorCharacter;

            return NormalColorCharacter;
        }

        public bool CheckOnline()
        {
            if (this.IsOnline)
                return true;

            RemoveChatUser(this);
            return false;
        }

        public void SendMessage(int number)
        {
            this.SendMessage(number, null, null);
        }

        public void SendMessage(int number, string param1)
        {
            this.SendMessage(number, param1, null);
        }

        public void SendMessage(int number, string param1, string param2)
        {
            if (this.m_Mobile.NetState != null)
                this.m_Mobile.Send(new ChatMessagePacket(this.m_Mobile, number, param1, param2));
        }

        public void SendMessage(int number, Mobile from, string param1, string param2)
        {
            if (this.m_Mobile.NetState != null)
                this.m_Mobile.Send(new ChatMessagePacket(from, number, param1, param2));
        }

        public bool IsIgnored(ChatUser check)
        {
            return this.m_Ignored.Contains(check);
        }

        public void AddIgnored(ChatUser user)
        {
            if (this.IsIgnored(user))
            {
                this.SendMessage(22, user.Username); // You are already ignoring %1.
            }
            else
            {
                this.m_Ignored.Add(user);
                user.m_Ignoring.Add(this);

                this.SendMessage(23, user.Username); // You are now ignoring %1.
            }
        }

        public void RemoveIgnored(ChatUser user)
        {
            if (this.IsIgnored(user))
            {
                this.m_Ignored.Remove(user);
                user.m_Ignoring.Remove(this);

                this.SendMessage(24, user.Username); // You are no longer ignoring %1.

                if (this.m_Ignored.Count == 0)
                    this.SendMessage(26); // You are no longer ignoring anyone.
            }
            else
            {
                this.SendMessage(25, user.Username); // You are not ignoring %1.
            }
        }
    }
}