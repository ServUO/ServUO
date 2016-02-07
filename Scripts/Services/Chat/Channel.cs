using System;
using System.Collections.Generic;

namespace Server.Engines.Chat
{
    public class Channel
    {
        private static readonly List<Channel> m_Channels = new List<Channel>();
        private readonly List<ChatUser> m_Users;
        private readonly List<ChatUser> m_Banned;
        private readonly List<ChatUser> m_Moderators;
        private readonly List<ChatUser> m_Voices;
        private string m_Name;
        private string m_Password;
        private bool m_VoiceRestricted;
        private bool m_AlwaysAvailable;
        public Channel(string name)
        {
            this.m_Name = name;

            this.m_Users = new List<ChatUser>();
            this.m_Banned = new List<ChatUser>();
            this.m_Moderators = new List<ChatUser>();
            this.m_Voices = new List<ChatUser>();
        }

        public Channel(string name, string password)
            : this(name)
        {
            this.m_Password = password;
        }

        public static List<Channel> Channels
        {
            get
            {
                return m_Channels;
            }
        }
        public string Name
        {
            get
            {
                return this.m_Name;
            }
            set
            {
                this.SendCommand(ChatCommand.RemoveChannel, this.m_Name);
                this.m_Name = value;
                this.SendCommand(ChatCommand.AddChannel, this.m_Name);
                this.SendCommand(ChatCommand.JoinedChannel, this.m_Name);
            }
        }
        public string Password
        {
            get
            {
                return this.m_Password;
            }
            set
            {
                string newValue = null;

                if (value != null)
                {
                    newValue = value.Trim();

                    if (String.IsNullOrEmpty(newValue))
                        newValue = null;
                }

                this.m_Password = newValue;
            }
        }
        public bool VoiceRestricted
        {
            get
            {
                return this.m_VoiceRestricted;
            }
            set
            {
                this.m_VoiceRestricted = value;

                if (value)
                    this.SendMessage(56); // From now on, only moderators will have speaking privileges in this conference by default.
                else
                    this.SendMessage(55); // From now on, everyone in the conference will have speaking privileges by default.
            }
        }
        public bool AlwaysAvailable
        {
            get
            {
                return this.m_AlwaysAvailable;
            }
            set
            {
                this.m_AlwaysAvailable = value;
            }
        }
        public static void SendChannelsTo(ChatUser user)
        {
            for (int i = 0; i < m_Channels.Count; ++i)
            {
                Channel channel = m_Channels[i];

                if (!channel.IsBanned(user))
                    ChatSystem.SendCommandTo(user.Mobile, ChatCommand.AddChannel, channel.Name, "0");
            }
        }

        public static Channel AddChannel(string name)
        {
            return AddChannel(name, null);
        }

        public static Channel AddChannel(string name, string password)
        {
            Channel channel = FindChannelByName(name);

            if (channel == null)
            {
                channel = new Channel(name, password);
                m_Channels.Add(channel);
            }

            ChatUser.GlobalSendCommand(ChatCommand.AddChannel, name, "0") ; 

            return channel;
        }

        public static void RemoveChannel(string name)
        {
            RemoveChannel(FindChannelByName(name));
        }

        public static void RemoveChannel(Channel channel)
        {
            if (channel == null)
                return;

            if (m_Channels.Contains(channel) && channel.m_Users.Count == 0)
            {
                ChatUser.GlobalSendCommand(ChatCommand.RemoveChannel, channel.Name) ;

                channel.m_Moderators.Clear();
                channel.m_Voices.Clear();

                m_Channels.Remove(channel);
            }
        }

        public static Channel FindChannelByName(string name)
        {
            for (int i = 0; i < m_Channels.Count; ++i)
            {
                Channel channel = m_Channels[i];

                if (channel.m_Name == name)
                    return channel;
            }

            return null;
        }

        public static void Initialize()
        {
            AddStaticChannel("Newbie Help");
        }

        public static void AddStaticChannel(string name)
        {
            AddChannel(name).AlwaysAvailable = true;
        }

        public bool Contains(ChatUser user)
        {
            return this.m_Users.Contains(user);
        }

        public bool IsBanned(ChatUser user)
        {
            return this.m_Banned.Contains(user);
        }

        public bool CanTalk(ChatUser user)
        {
            return (!this.m_VoiceRestricted || this.m_Voices.Contains(user) || this.m_Moderators.Contains(user));
        }

        public bool IsModerator(ChatUser user)
        {
            return this.m_Moderators.Contains(user);
        }

        public bool IsVoiced(ChatUser user)
        {
            return this.m_Voices.Contains(user);
        }

        public bool ValidatePassword(string password)
        {
            return (this.m_Password == null || Insensitive.Equals(this.m_Password, password));
        }

        public bool ValidateModerator(ChatUser user)
        {
            if (user != null && !this.IsModerator(user))
            {
                user.SendMessage(29); // You must have operator status to do this.
                return false;
            }

            return true;
        }

        public bool ValidateAccess(ChatUser from, ChatUser target)
        {
            if (from != null && target != null && from.Mobile.AccessLevel < target.Mobile.AccessLevel)
            {
                from.Mobile.SendMessage("Your access level is too low to do this.");
                return false;
            }

            return true;
        }

        public bool AddUser(ChatUser user)
        {
            return this.AddUser(user, null);
        }

        public bool AddUser(ChatUser user, string password)
        {
            if (this.Contains(user))
            {
                user.SendMessage(46, this.m_Name); // You are already in the conference '%1'.
                return true;
            }
            else if (this.IsBanned(user))
            {
                user.SendMessage(64); // You have been banned from this conference.
                return false;
            }
            else if (!this.ValidatePassword(password))
            {
                user.SendMessage(34); // That is not the correct password.
                return false;
            }
            else
            {
                if (user.CurrentChannel != null)
                    user.CurrentChannel.RemoveUser(user); // Remove them from their current channel first

                ChatSystem.SendCommandTo(user.Mobile, ChatCommand.JoinedChannel, this.m_Name);

                this.SendCommand(ChatCommand.AddUserToChannel, user.GetColorCharacter() + user.Username);

                this.m_Users.Add(user);
                user.CurrentChannel = this;
				
                if (user.Mobile.AccessLevel >= AccessLevel.GameMaster || (!this.m_AlwaysAvailable && this.m_Users.Count == 1))
                    this.AddModerator(user);

                this.SendUsersTo(user);

                return true;
            }
        }

        public void RemoveUser(ChatUser user) 
        { 
            if (this.Contains(user))
            {
                this.m_Users.Remove(user);
                user.CurrentChannel = null;

                if (this.m_Moderators.Contains(user))
                    this.m_Moderators.Remove(user);

                if (this.m_Voices.Contains(user))
                    this.m_Voices.Remove(user);

                this.SendCommand(ChatCommand.RemoveUserFromChannel, user, user.Username);
                ChatSystem.SendCommandTo(user.Mobile, ChatCommand.LeaveChannel);

                if (this.m_Users.Count == 0 && !this.m_AlwaysAvailable)
                    RemoveChannel(this);
            }
        }

        public void AdBan(ChatUser user)
        {
            this.AddBan(user, null);
        }

        public void AddBan(ChatUser user, ChatUser moderator)
        {
            if (!this.ValidateModerator(moderator) || !this.ValidateAccess(moderator, user))
                return;

            if (!this.m_Banned.Contains(user))
                this.m_Banned.Add(user);

            this.Kick(user, moderator, true);
        }

        public void RemoveBan(ChatUser user)
        {
            if (this.m_Banned.Contains(user))
                this.m_Banned.Remove(user);
        }

        public void Kick(ChatUser user)
        {
            this.Kick(user, null);
        }

        public void Kick(ChatUser user, ChatUser moderator)
        {
            this.Kick(user, moderator, false);
        }

        public void Kick(ChatUser user, ChatUser moderator, bool wasBanned)
        {
            if (!this.ValidateModerator(moderator) || !this.ValidateAccess(moderator, user))
                return;

            if (this.Contains(user))
            {
                if (moderator != null)
                {
                    if (wasBanned)
                        user.SendMessage(63, moderator.Username); // %1, a conference moderator, has banned you from the conference.
                    else
                        user.SendMessage(45, moderator.Username); // %1, a conference moderator, has kicked you out of the conference.
                }

                this.RemoveUser(user);
                ChatSystem.SendCommandTo(user.Mobile, ChatCommand.AddUserToChannel, user.GetColorCharacter() + user.Username);

                this.SendMessage(44, user.Username) ; // %1 has been kicked out of the conference.
            }

            if (wasBanned && moderator != null)
                moderator.SendMessage(62, user.Username); // You are banning %1 from this conference.
        }

        public void AddVoiced(ChatUser user)
        {
            this.AddVoiced(user, null);
        }

        public void AddVoiced(ChatUser user, ChatUser moderator)
        {
            if (!this.ValidateModerator(moderator))
                return;

            if (!this.IsBanned(user) && !this.IsModerator(user) && !this.IsVoiced(user))
            {
                this.m_Voices.Add(user);

                if (moderator != null)
                    user.SendMessage(54, moderator.Username); // %1, a conference moderator, has granted you speaking priviledges in this conference.

                this.SendMessage(52, user, user.Username); // %1 now has speaking privileges in this conference.
                this.SendCommand(ChatCommand.AddUserToChannel, user, user.GetColorCharacter() + user.Username);
            }
        }

        public void RemoveVoiced(ChatUser user, ChatUser moderator)
        {
            if (!this.ValidateModerator(moderator) || !this.ValidateAccess(moderator, user))
                return;

            if (!this.IsModerator(user) && this.IsVoiced(user))
            {
                this.m_Voices.Remove(user);

                if (moderator != null)
                    user.SendMessage(53, moderator.Username); // %1, a conference moderator, has removed your speaking priviledges for this conference.

                this.SendMessage(51, user, user.Username); // %1 no longer has speaking privileges in this conference.
                this.SendCommand(ChatCommand.AddUserToChannel, user, user.GetColorCharacter() + user.Username);
            }
        }

        public void AddModerator(ChatUser user)
        {
            this.AddModerator(user, null);
        }

        public void AddModerator(ChatUser user, ChatUser moderator)
        {
            if (!this.ValidateModerator(moderator))
                return;

            if (this.IsBanned(user) || this.IsModerator(user))
                return;

            if (this.IsVoiced(user))
                this.m_Voices.Remove(user);

            this.m_Moderators.Add(user);

            if (moderator != null)
                user.SendMessage(50, moderator.Username); // %1 has made you a conference moderator.

            this.SendMessage(48, user, user.Username); // %1 is now a conference moderator.
            this.SendCommand(ChatCommand.AddUserToChannel, user.GetColorCharacter() + user.Username);
        }

        public void RemoveModerator(ChatUser user)
        {
            this.RemoveModerator(user, null);
        }

        public void RemoveModerator(ChatUser user, ChatUser moderator)
        {
            if (!this.ValidateModerator(moderator) || !this.ValidateAccess(moderator, user))
                return;

            if (this.IsModerator(user))
            {
                this.m_Moderators.Remove(user);

                if (moderator != null)
                    user.SendMessage(49, moderator.Username); // %1 has removed you from the list of conference moderators.

                this.SendMessage(47, user, user.Username); // %1 is no longer a conference moderator.
                this.SendCommand(ChatCommand.AddUserToChannel, user.GetColorCharacter() + user.Username);
            }
        }

        public void SendMessage(int number)
        {
            this.SendMessage(number, null, null, null);
        }

        public void SendMessage(int number, string param1)
        {
            this.SendMessage(number, null, param1, null);
        }

        public void SendMessage(int number, string param1, string param2)
        {
            this.SendMessage(number, null, param1, param2);
        }

        public void SendMessage(int number, ChatUser initiator)
        {
            this.SendMessage(number, initiator, null, null);
        }

        public void SendMessage(int number, ChatUser initiator, string param1)
        {
            this.SendMessage(number, initiator, param1, null);
        }

        public void SendMessage(int number, ChatUser initiator, string param1, string param2)
        {
            for (int i = 0; i < this.m_Users.Count; ++i)
            {
                ChatUser user = this.m_Users[i];

                if (user == initiator)
                    continue;

                if (user.CheckOnline())
                    user.SendMessage(number, param1, param2);
                else if (!this.Contains(user))
                    --i;
            }
        }

        public void SendIgnorableMessage(int number, ChatUser from, string param1, string param2)
        {
            for (int i = 0; i < this.m_Users.Count; ++i)
            {
                ChatUser user = this.m_Users[i];

                if (user.IsIgnored(from))
                    continue;

                if (user.CheckOnline())
                    user.SendMessage(number, from.Mobile, param1, param2);
                else if (!this.Contains(user))
                    --i;
            }
        }

        public void SendCommand(ChatCommand command)
        {
            this.SendCommand(command, null, null, null);
        }

        public void SendCommand(ChatCommand command, string param1)
        {
            this.SendCommand(command, null, param1, null);
        }

        public void SendCommand(ChatCommand command, string param1, string param2)
        {
            this.SendCommand(command, null, param1, param2);
        }

        public void SendCommand(ChatCommand command, ChatUser initiator)
        {
            this.SendCommand(command, initiator, null, null);
        }

        public void SendCommand(ChatCommand command, ChatUser initiator, string param1)
        {
            this.SendCommand(command, initiator, param1, null);
        }

        public void SendCommand(ChatCommand command, ChatUser initiator, string param1, string param2)
        {
            for (int i = 0; i < this.m_Users.Count; ++i)
            {
                ChatUser user = this.m_Users[i];

                if (user == initiator)
                    continue;

                if (user.CheckOnline())
                    ChatSystem.SendCommandTo(user.Mobile, command, param1, param2);
                else if (!this.Contains(user))
                    --i;
            }
        }

        public void SendUsersTo(ChatUser to)
        {
            for (int i = 0; i < this.m_Users.Count; ++i)
            {
                ChatUser user = this.m_Users[i];

                ChatSystem.SendCommandTo(to.Mobile, ChatCommand.AddUserToChannel, user.GetColorCharacter() + user.Username);
            }
        }
    }
}