using Server.Network;
using System;

namespace Server.Engines.Chat
{
    public class ChatSystem
    {
        public static readonly bool Enabled = Config.Get("Chat.Enabled", true);
        public static readonly bool AllowCreateChannels = Config.Get("Chat.AllowCreateChannels", true);
        public static readonly string DefaultChannel = "Help";

        public static readonly long ChatDelay = 5000;

        public static void Initialize()
        {
            PacketHandlers.Register(0xB5, 0x40, true, OpenChatWindowRequest);
            PacketHandlers.Register(0xB3, 0, true, ChatAction);
        }

        public static void OpenChatWindowRequest(NetState state, PacketReader pvSrc)
        {
            Mobile from = state.Mobile;

            if (!Enabled)
            {
                from.SendMessage("The chat system has been disabled.");
                return;
            }

            string chatName = from.Name;

            SendCommandTo(from, ChatCommand.OpenChatWindow, chatName);
            ChatUser.AddChatUser(from);
        }

        public static void ChatAction(NetState state, PacketReader pvSrc)
        {
            if (!Enabled)
                return;

            try
            {
                Mobile from = state.Mobile;
                ChatUser user = ChatUser.GetChatUser(from);

                if (user == null)
                    return;

                string lang = pvSrc.ReadStringSafe(4);
                short actionId = pvSrc.ReadInt16();
                string param = pvSrc.ReadUnicodeString();

                ChatActionHandler handler = ChatActionHandlers.GetHandler(actionId);

                if (handler != null)
                {
                    Channel channel = user.CurrentChannel;

                    if (handler.RequireConference && channel == null)
                    {
                        /* You must be in a conference to do this.
                         * To join a conference, select one from the Conference menu.
                         */
                        user.SendMessage(31);
                    }
                    else
                    {
                        handler.Callback(user, channel, param);
                    }
                }
                else
                {
                    Console.WriteLine("Client: {0}: Unknown chat action 0x{1:X}: {2}", state, actionId, param);
                }
            }
            catch (Exception e)
            {
                Diagnostics.ExceptionLogging.LogException(e);
            }
        }

        public static void SendCommandTo(Mobile to, ChatCommand type, string param1 = null, string param2 = null)
        {
            if (to != null)
                to.Send(new ChatMessagePacket(null, (int)type + 20, param1, param2));
        }
    }
}
