using System;
using System.Collections.Generic;
using System.IO;

namespace Server.Engines.Chat
{
	public class ChatLogging
	{
		public static bool Enabled { get => Config.Get("Chat.Logging", false); set => Config.Set("Chat.Logging", value); }

		private static readonly Dictionary<string, StreamWriter> m_OutputPerChannel = new Dictionary<string, StreamWriter>();

		private static readonly StreamWriter m_Output;

		static ChatLogging()
		{
			var directory = Path.Combine(Core.BaseDirectory, "Logs", "Chat");

			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			try
			{
				var path = Path.Combine(directory, $"{DateTime.UtcNow.ToLongDateString()}.log");

				m_Output = new StreamWriter(path, true)
				{
					AutoFlush = true
				};

				m_Output.WriteLine("##############################");
				m_Output.WriteLine($"Log started on {DateTime.UtcNow}");
				m_Output.WriteLine();
			}
			catch
			{ }
		}

		public static void WriteLine(string channel, string format, params object[] args)
		{
			WriteLine(channel, String.Format(format, args));
		}

		public static void WriteLine(string channel, string text)
		{
			if (!Enabled)
			{
				return;
			}

			try
			{
				m_Output.WriteLine($"{DateTime.UtcNow}: [{channel}] {text}");

				if (!m_OutputPerChannel.TryGetValue(channel, out var channelOutput))
				{
					var path = Path.Combine("Logs", "Chat", "Channels");

					if (!Directory.Exists(path))
					{
						Directory.CreateDirectory(path);
					}

					path = Path.Combine(path, $"{channel}.log");

					m_OutputPerChannel[channel] = channelOutput = new StreamWriter(path, true)
					{
						AutoFlush = true
					};
				}

				channelOutput.WriteLine($"{DateTime.UtcNow}: {text}");
			}
			catch
			{ }
		}

		public static void LogMessage(string channel, string username, string message)
		{
			WriteLine(channel, $"{username} says: {message}");
		}

		public static void LogCreateChannel(string channel)
		{
			WriteLine(channel, "************** Channel was created.");
		}

		public static void LogRemoveChannel(string channel)
		{
			WriteLine(channel, "************** Channel was removed.");
		}

		public static void LogJoin(string channel, string username)
		{
			WriteLine(channel, $"{username} joined the channel.");
		}

		public static void LogLeave(string channel, string username)
		{
			WriteLine(channel, $"{username} left the channel.");

			if (m_OutputPerChannel.TryGetValue(channel, out var channelOutput))
			{
				m_OutputPerChannel.Remove(channel);

				channelOutput.Dispose();
			}
		}

		public static void Log(string channel, string message)
		{
			WriteLine(channel, message);
		}
	}
}
