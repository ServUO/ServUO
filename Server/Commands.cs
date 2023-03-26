#region References
using System;
using System.Collections.Generic;

using Server.Network;
#endregion

namespace Server.Commands
{
	public delegate void CommandEventHandler(CommandEventArgs e);

	public class CommandEventArgs : EventArgs
	{
		public Mobile Mobile { get; }

		public string Command { get; }

		public string ArgString { get; }
		public string[] Arguments { get; }

		public int Length => Arguments.Length;

		public string GetString(int index)
		{
			if (index < 0 || index >= Arguments.Length)
			{
				return "";
			}

			return Arguments[index];
		}

		public int GetInt32(int index)
		{
			if (index < 0 || index >= Arguments.Length)
			{
				return 0;
			}

			return Utility.ToInt32(Arguments[index]);
		}

		public bool GetBoolean(int index)
		{
			if (index < 0 || index >= Arguments.Length)
			{
				return false;
			}

			return Utility.ToBoolean(Arguments[index]);
		}

		public double GetDouble(int index)
		{
			if (index < 0 || index >= Arguments.Length)
			{
				return 0.0;
			}

			return Utility.ToDouble(Arguments[index]);
		}

		public TimeSpan GetTimeSpan(int index)
		{
			if (index < 0 || index >= Arguments.Length)
			{
				return TimeSpan.Zero;
			}

			return Utility.ToTimeSpan(Arguments[index]);
		}

		public Serial GetSerial(int index)
		{
			if (index < 0 || index >= Arguments.Length)
			{
				return Serial.Zero;
			}

			return Utility.ToSerial(Arguments[index]);
		}

		public CommandEventArgs(Mobile mobile, string command, string argString, string[] arguments)
		{
			Mobile = mobile;
			Command = command;
			ArgString = argString;
			Arguments = arguments;
		}
	}

	public class CommandEntry : IComparable
	{
		public string Command { get; }

		public CommandEventHandler Handler { get; }

		public AccessLevel AccessLevel { get; }

		public CommandEntry(string command, CommandEventHandler handler, AccessLevel accessLevel)
		{
			Command = command;
			Handler = handler;
			AccessLevel = accessLevel;
		}

		public int CompareTo(object obj)
		{
			if (obj == this)
			{
				return 0;
			}
			else if (obj == null)
			{
				return 1;
			}

			var e = obj as CommandEntry;

			if (e == null)
			{
				throw new ArgumentException();
			}

			return Insensitive.Compare(Command, e.Command);
		}
	}

	public static class CommandSystem
	{
		public static event Action<Mobile, string> OnCommandFail;

		public static string Prefix { get; set; } = "[";

		public static string[] Split(string value)
		{
			var array = value.ToCharArray();
			var list = new List<string>();

			int start = 0, end = 0;

			while (start < array.Length)
			{
				var c = array[start];

				if (c == '"')
				{
					++start;
					end = start;

					while (end < array.Length)
					{
						if (array[end] != '"' || array[end - 1] == '\\')
						{
							++end;
						}
						else
						{
							break;
						}
					}

					list.Add(value.Substring(start, end - start));

					start = end + 2;
				}
				else if (c != ' ')
				{
					end = start;

					while (end < array.Length)
					{
						if (array[end] != ' ')
						{
							++end;
						}
						else
						{
							break;
						}
					}

					list.Add(value.Substring(start, end - start));

					start = end + 1;
				}
				else
				{
					++start;
				}
			}

			return list.ToArray();
		}

		public static Dictionary<string, CommandEntry> Entries { get; private set; }

		static CommandSystem()
		{
			Entries = new Dictionary<string, CommandEntry>(StringComparer.OrdinalIgnoreCase);
		}

		public static void Register(string command, AccessLevel access, CommandEventHandler handler)
		{
			Entries[command] = new CommandEntry(command, handler, access);
		}

		public static AccessLevel BadCommandIgnoreLevel { get; set; } = AccessLevel.Player;

		public static bool Handle(Mobile from, string text)
		{
			return Handle(from, text, MessageType.Regular);
		}

		public static bool Handle(Mobile from, string text, MessageType type)
		{
			if (text.StartsWith(Prefix) || type == MessageType.Command)
			{
				if (type != MessageType.Command)
				{
					text = text.Substring(Prefix.Length);
				}

				var indexOf = text.IndexOf(' ');

				string command;
				string[] args;
				string argString;

				if (indexOf >= 0)
				{
					argString = text.Substring(indexOf + 1);

					command = text.Substring(0, indexOf);
					args = Split(argString);
				}
				else
				{
					argString = "";
					command = text.ToLower();
					args = new string[0];
				}

				Entries.TryGetValue(command, out var entry);

				if (entry != null)
				{
					if (from == null || from.AccessLevel >= entry.AccessLevel)
					{
						if (entry.Handler != null)
						{
							var e = new CommandEventArgs(from, command, argString, args);
							entry.Handler(e);
							EventSink.InvokeCommand(e);
						}
					}
					else
					{
						if (from.AccessLevel <= BadCommandIgnoreLevel)
						{
							return false;
						}

						from.SendMessage("You do not have access to that command.");
					}
				}
				else if (from != null)
				{
					if (from.AccessLevel <= BadCommandIgnoreLevel)
					{
						return false;
					}

					from.SendMessage("That is not a valid command.");

					OnCommandFail?.Invoke(from, text);
				}

				return true;
			}

			return false;
		}
	}
}
