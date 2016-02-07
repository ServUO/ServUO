using System;

namespace Server.Engines.Chat
{
    public class ChatActionHandlers
    {
        private static ChatActionHandler[] m_Handlers;
        static ChatActionHandlers()
        {
            m_Handlers = new ChatActionHandler[0x100];

            Register(0x41, true, true, new OnChatAction(ChangeChannelPassword));

            Register(0x58, false, false, new OnChatAction(LeaveChat));

            Register(0x61, false, true, new OnChatAction(ChannelMessage));
            Register(0x62, false, false, new OnChatAction(JoinChannel));
            Register(0x63, false, false, new OnChatAction(JoinNewChannel));
            Register(0x64, true, true, new OnChatAction(RenameChannel));
            Register(0x65, false, false, new OnChatAction(PrivateMessage));
            Register(0x66, false, false, new OnChatAction(AddIgnore));
            Register(0x67, false, false, new OnChatAction(RemoveIgnore));
            Register(0x68, false, false, new OnChatAction(ToggleIgnore));
            Register(0x69, true, true, new OnChatAction(AddVoice));
            Register(0x6A, true, true, new OnChatAction(RemoveVoice));
            Register(0x6B, true, true, new OnChatAction(ToggleVoice));
            Register(0x6C, true, true, new OnChatAction(AddModerator));
            Register(0x6D, true, true, new OnChatAction(RemoveModerator));
            Register(0x6E, true, true, new OnChatAction(ToggleModerator));
            Register(0x6F, false, false, new OnChatAction(AllowPrivateMessages));
            Register(0x70, false, false, new OnChatAction(DisallowPrivateMessages));
            Register(0x71, false, false, new OnChatAction(TogglePrivateMessages));
            Register(0x72, false, false, new OnChatAction(ShowCharacterName));
            Register(0x73, false, false, new OnChatAction(HideCharacterName));
            Register(0x74, false, false, new OnChatAction(ToggleCharacterName));
            Register(0x75, false, false, new OnChatAction(QueryWhoIs));
            Register(0x76, true, true, new OnChatAction(Kick));
            Register(0x77, true, true, new OnChatAction(EnableDefaultVoice));
            Register(0x78, true, true, new OnChatAction(DisableDefaultVoice));
            Register(0x79, true, true, new OnChatAction(ToggleDefaultVoice));
            Register(0x7A, false, true, new OnChatAction(EmoteMessage));
        }

        public static void Register(int actionID, bool requireModerator, bool requireConference, OnChatAction callback)
        {
            if (actionID >= 0 && actionID < m_Handlers.Length)
                m_Handlers[actionID] = new ChatActionHandler(requireModerator, requireConference, callback);
        }

        public static ChatActionHandler GetHandler(int actionID)
        {
            if (actionID >= 0 && actionID < m_Handlers.Length)
                return m_Handlers[actionID];

            return null;
        }

        public static void ChannelMessage(ChatUser from, Channel channel, string param)
        {
            if (channel.CanTalk(from))
                channel.SendIgnorableMessage(57, from, from.GetColorCharacter() + from.Username, param); // %1: %2
            else
                from.SendMessage(36); // The moderator of this conference has not given you speaking priviledges.
        }

        public static void EmoteMessage(ChatUser from, Channel channel, string param)
        {
            if (channel.CanTalk(from))
                channel.SendIgnorableMessage(58, from, from.GetColorCharacter() + from.Username, param); // %1 %2
            else
                from.SendMessage(36); // The moderator of this conference has not given you speaking priviledges.
        }

        public static void PrivateMessage(ChatUser from, Channel channel, string param)
        {
            int indexOf = param.IndexOf(' ');

            string name = param.Substring(0, indexOf);
            string text = param.Substring(indexOf + 1);

            ChatUser target = ChatSystem.SearchForUser(from, name);

            if (target == null)
                return;

            if (target.IsIgnored(from))
                from.SendMessage(35, target.Username); // %1 has chosen to ignore you. None of your messages to them will get through.
            else if (target.IgnorePrivateMessage)
                from.SendMessage(42, target.Username); // %1 has chosen to not receive private messages at the moment.
            else
                target.SendMessage(59, from.Mobile, from.GetColorCharacter() + from.Username, text); // [%1]: %2
        }

        public static void LeaveChat(ChatUser from, Channel channel, string param)
        {
            ChatUser.RemoveChatUser(from);
        }

        public static void ChangeChannelPassword(ChatUser from, Channel channel, string param)
        {
            channel.Password = param;
            from.SendMessage(60); // The password to the conference has been changed.
        }

        public static void AllowPrivateMessages(ChatUser from, Channel channel, string param)
        {
            from.IgnorePrivateMessage = false;
            from.SendMessage(37); // You can now receive private messages.
        }

        public static void DisallowPrivateMessages(ChatUser from, Channel channel, string param)
        {
            from.IgnorePrivateMessage = true;
            from.SendMessage(38); /* You will no longer receive private messages.
            * Those who send you a message will be notified that you are blocking incoming messages.
            */
        }

        public static void TogglePrivateMessages(ChatUser from, Channel channel, string param)
        {
            from.IgnorePrivateMessage = !from.IgnorePrivateMessage;
            from.SendMessage(from.IgnorePrivateMessage ? 38 : 37); // See above for messages
        }

        public static void ShowCharacterName(ChatUser from, Channel channel, string param)
        {
            from.Anonymous = false;
            from.SendMessage(39); // You are now showing your character name to any players who inquire with the whois command.
        }

        public static void HideCharacterName(ChatUser from, Channel channel, string param)
        {
            from.Anonymous = true;
            from.SendMessage(40); // You are no longer showing your character name to any players who inquire with the whois command.
        }

        public static void ToggleCharacterName(ChatUser from, Channel channel, string param)
        {
            from.Anonymous = !from.Anonymous;
            from.SendMessage(from.Anonymous ? 40 : 39); // See above for messages
        }

        public static void JoinChannel(ChatUser from, Channel channel, string param)
        {
            string name;
            string password = null;

            int start = param.IndexOf('\"');

            if (start >= 0)
            {
                int end = param.IndexOf('\"', ++start);

                if (end >= 0)
                {
                    name = param.Substring(start, end - start);
                    password = param.Substring(++end);
                }
                else
                {
                    name = param.Substring(start);
                }
            }
            else
            {
                int indexOf = param.IndexOf(' ');

                if (indexOf >= 0)
                {
                    name = param.Substring(0, indexOf++);
                    password = param.Substring(indexOf);
                }
                else
                {
                    name = param;
                }
            }

            if (password != null)
                password = password.Trim();

            if (password != null && password.Length == 0)
                password = null;

            Channel joined = Channel.FindChannelByName(name);

            if (joined == null)
                from.SendMessage(33, name); // There is no conference named '%1'.
            else
                joined.AddUser(from, password);
        }

        public static void JoinNewChannel(ChatUser from, Channel channel, string param)
        {
            if ((param = param.Trim()).Length == 0)
                return;

            string name;
            string password = null;

            int start = param.IndexOf('{');

            if (start >= 0)
            {
                name = param.Substring(0, start++);

                int end = param.IndexOf('}', start);

                if (end >= start)
                    password = param.Substring(start, end - start);
            }
            else
            {
                name = param;
            }

            if (password != null)
                password = password.Trim();

            if (password != null && password.Length == 0)
                password = null;

            Channel.AddChannel(name, password).AddUser(from, password);
        }

        public static void AddIgnore(ChatUser from, Channel channel, string param)
        {
            ChatUser target = ChatSystem.SearchForUser(from, param);

            if (target == null)
                return;

            from.AddIgnored(target);
        }

        public static void RemoveIgnore(ChatUser from, Channel channel, string param)
        {
            ChatUser target = ChatSystem.SearchForUser(from, param);

            if (target == null)
                return;

            from.RemoveIgnored(target);
        }

        public static void ToggleIgnore(ChatUser from, Channel channel, string param)
        {
            ChatUser target = ChatSystem.SearchForUser(from, param);

            if (target == null)
                return;

            if (from.IsIgnored(target))
                from.RemoveIgnored(target);
            else
                from.AddIgnored(target);
        }

        public static void AddVoice(ChatUser from, Channel channel, string param)
        {
            ChatUser target = ChatSystem.SearchForUser(from, param);

            if (target != null)
                channel.AddVoiced(target, from);
        }

        public static void RemoveVoice(ChatUser from, Channel channel, string param)
        {
            ChatUser target = ChatSystem.SearchForUser(from, param);

            if (target != null)
                channel.RemoveVoiced(target, from);
        }

        public static void ToggleVoice(ChatUser from, Channel channel, string param)
        {
            ChatUser target = ChatSystem.SearchForUser(from, param);

            if (target == null)
                return;

            if (channel.IsVoiced(target))
                channel.RemoveVoiced(target, from);
            else
                channel.AddVoiced(target, from);
        }

        public static void AddModerator(ChatUser from, Channel channel, string param)
        {
            ChatUser target = ChatSystem.SearchForUser(from, param);

            if (target != null)
                channel.AddModerator(target, from);
        }

        public static void RemoveModerator(ChatUser from, Channel channel, string param)
        {
            ChatUser target = ChatSystem.SearchForUser(from, param);

            if (target != null)
                channel.RemoveModerator(target, from);
        }

        public static void ToggleModerator(ChatUser from, Channel channel, string param)
        {
            ChatUser target = ChatSystem.SearchForUser(from, param);

            if (target == null)
                return;

            if (channel.IsModerator(target))
                channel.RemoveModerator(target, from);
            else
                channel.AddModerator(target, from);
        }

        public static void RenameChannel(ChatUser from, Channel channel, string param)
        {
            channel.Name = param;
        }

        public static void QueryWhoIs(ChatUser from, Channel channel, string param)
        {
            ChatUser target = ChatSystem.SearchForUser(from, param);

            if (target == null)
                return;

            if (target.Anonymous)
                from.SendMessage(41, target.Username); // %1 is remaining anonymous.
            else
                from.SendMessage(43, target.Username, target.Mobile.Name); // %2 is known in the lands of Britannia as %2.
        }

        public static void Kick(ChatUser from, Channel channel, string param)
        {
            ChatUser target = ChatSystem.SearchForUser(from, param);

            if (target != null)
                channel.Kick(target, from);
        }

        public static void EnableDefaultVoice(ChatUser from, Channel channel, string param)
        {
            channel.VoiceRestricted = false;
        }

        public static void DisableDefaultVoice(ChatUser from, Channel channel, string param)
        {
            channel.VoiceRestricted = true;
        }

        public static void ToggleDefaultVoice(ChatUser from, Channel channel, string param)
        {
            channel.VoiceRestricted = !channel.VoiceRestricted;
        }
    }
}