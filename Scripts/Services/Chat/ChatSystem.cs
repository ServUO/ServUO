using System;

using Server.Network;

namespace Server.Engines.Chat
{
	public class ChatSystem
	{
		[ConfigProperty("Chat.Enabled")]
		public static bool Enabled { get => Config.Get("Chat.Enabled", true); set => Config.Set("Chat.Enabled", value); }

		[ConfigProperty("Chat.AllowCreateChannels")]
		public static bool AllowCreateChannels { get => Config.Get("Chat.AllowCreateChannels", true); set => Config.Set("Chat.AllowCreateChannels", value); }

		[ConfigProperty("Chat.DefaultChannel")]
		public static string DefaultChannel { get => Config.Get("Chat.DefaultChannel", "Help"); set => Config.Set("Chat.DefaultChannel", value); }

		[ConfigProperty("Chat.MessageInterval")]
		public static long ChatDelay { get => Config.Get("Chat.MessageInterval", 5000); set => Config.Set("Chat.MessageInterval", value); }

		[ConfigProperty("Chat.Logging")]
		public static bool LoggingEnabled { get => Config.Get("Chat.Logging", false); set => Config.Set("Chat.Logging", value); }

		public static void Configure()
		{
			PacketHandlers.Register(0xB5, 0x40, true, OpenChatWindowRequest);
			PacketHandlers.Register(0xB3, 0, true, ChatAction);
		}

		public static void OpenChatWindowRequest(NetState state, PacketReader pvSrc)
		{
			var from = state.Mobile;

			if (!Enabled)
			{
				from.SendMessage("The chat system has been disabled.");
				return;
			}

			var chatName = from.Name;

			SendCommandTo(from, ChatCommand.OpenChatWindow, chatName);

			ChatUser.AddChatUser(from);
		}

		public static void ChatAction(NetState state, PacketReader pvSrc)
		{
			if (!Enabled)
			{
				return;
			}

			try
			{
				var from = state.Mobile;
				var user = ChatUser.GetChatUser(from);

				if (user == null)
				{
					return;
				}

				var lang = pvSrc.ReadStringSafe(4);
				var actionId = pvSrc.ReadInt16();
				var param = pvSrc.ReadUnicodeString();

				var handler = ChatActionHandlers.GetHandler(actionId);

				if (handler != null)
				{
					var channel = user.CurrentChannel;

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
					Console.WriteLine($"Client: {state}: Unknown chat action 0x{actionId:X}: {param}");
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
			{
				to.Send(new ChatMessagePacket(null, (int)type + 20, param1, param2));
			}
		}
	}
}
