using System;

namespace Server.Engines.Chat
{
    public enum ChatCommand
    {
        /// <summary>
        /// Add a channel to top list.
        /// </summary>
        AddChannel = 0x3E8,
        /// <summary>
        /// Remove channel from top list.
        /// </summary>
        RemoveChannel = 0x3E9,
        /// <summary>
        /// Queries for a new chat nickname.
        /// </summary>
        AskNewNickname = 0x3EB,
        /// <summary>
        /// Closes the chat window.
        /// </summary>
        CloseChatWindow = 0x3EC,
        /// <summary>
        /// Opens the chat window.
        /// </summary>
        OpenChatWindow = 0x3ED,
        /// <summary>
        /// Add a user to current channel.
        /// </summary>
        AddUserToChannel = 0x3EE,
        /// <summary>
        /// Remove a user from current channel.
        /// </summary>
        RemoveUserFromChannel = 0x3EF,
        /// <summary>
        /// Send a message putting generic conference name at top when player leaves a channel.
        /// </summary>
        LeaveChannel = 0x3F0,
        /// <summary>
        /// Send a message putting Channel name at top and telling player he joined the channel.
        /// </summary>
        JoinedChannel = 0x3F1
    }
}