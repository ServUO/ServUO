#region References
using System;
using System.Linq;
using System.Threading;

using Server.Accounting;
using Server.Engines.Help;
using Server.Network;
#endregion

namespace Server.Misc
{
	internal static class ServerConsole
	{
		private static readonly Func<string> _Listen = Console.ReadLine;

		private static string _Command;

		private static Timer _PollTimer;

		private static bool _HearConsole;

		public static void Initialize()
		{
			EventSink.ServerStarted += () =>
			{
				PollCommands();

				if (_HearConsole)
				{
					Console.WriteLine("Now listening to the whole shard.");
				}
			};

			EventSink.Speech += args =>
			{
				if (args.Mobile == null || !_HearConsole)
				{
					return;
				}

				try
				{
					if (args.Mobile.Region.Name.Length > 0)
					{
						Console.WriteLine(args.Mobile.Name + " (" + args.Mobile.Region.Name + "): " + args.Speech);
					}
					else
					{
						Console.WriteLine("" + args.Mobile.Name + ": " + args.Speech + "");
					}
				}
				catch
				{ }
			};
		}

		private static void PollCommands()
		{
			_PollTimer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromMilliseconds(100), ProcessCommand);

			_Listen.BeginInvoke(r => ProcessInput(_Listen.EndInvoke(r)), null);
		}

		private static void ProcessInput(string input)
		{
			if (!Core.Crashed && !Core.Closing)
			{
				Interlocked.Exchange(ref _Command, input);
			}
		}

		private static void ProcessCommand()
		{
			if (Core.Crashed || Core.Closing || World.Loading || World.Saving)
			{
				return;
			}

			if (String.IsNullOrEmpty(_Command))
			{
				return;
			}

			ProcessCommand(_Command);

			Interlocked.Exchange(ref _Command, String.Empty);

			_Listen.BeginInvoke(r => ProcessInput(_Listen.EndInvoke(r)), null);
		}

		private static PageEntry[] _Pages;

		private static void ProcessCommand(string input)
		{
			input = input.Trim();

			if (_Pages != null)
			{
				HandlePaging(input);
				return;
			}

			if (input.StartsWith("pages", StringComparison.OrdinalIgnoreCase))
			{
				HandlePaging(input.Substring(5).Trim());
				return;
			}

			if (input.StartsWith("bc", StringComparison.OrdinalIgnoreCase))
			{
				var sub = input.Substring(2).Trim();

				BroadcastMessage(AccessLevel.Player, 0x35, String.Format("[Admin] {0}", sub));

				Console.WriteLine("[World]: {0}", sub);
				return;
			}

			if (input.StartsWith("sc", StringComparison.OrdinalIgnoreCase))
			{
				var sub = input.Substring(2).Trim();

				BroadcastMessage(AccessLevel.Counselor, 0x32, String.Format("[Admin] {0}", sub));

				Console.WriteLine("[Staff]: {0}", sub);
				return;
			}

			if (input.StartsWith("ban", StringComparison.OrdinalIgnoreCase))
			{
				var sub = input.Substring(3).Trim();

				var states = NetState.Instances;

				if (states.Count == 0)
				{
					Console.WriteLine("There are no players online.");
					return;
				}

				var ns = states.FirstOrDefault(o => o.Account != null && o.Mobile != null && Insensitive.StartsWith(sub, o.Mobile.RawName));

				if (ns != null)
				{
					Console.WriteLine("[Ban]: {0}: Mobile: '{1}' Account: '{2}'", ns, ns.Mobile.RawName, ns.Account.Username);

					ns.Dispose();
				}

				return;
			}

			if (input.StartsWith("kick", StringComparison.OrdinalIgnoreCase))
			{
				var sub = input.Substring(4).Trim();

				var states = NetState.Instances;

				if (states.Count == 0)
				{
					Console.WriteLine("There are no players online.");
					return;
				}

				var ns = states.FirstOrDefault(o => o.Account != null && o.Mobile != null && Insensitive.StartsWith(sub, o.Mobile.RawName));

				if (ns != null)
				{
					Console.WriteLine("[Kick]: {0}: Mobile: '{1}' Account: '{2}'", ns, ns.Mobile.RawName, ns.Account.Username);

					ns.Dispose();
				}

				return;
			}

			switch (input.Trim())
			{
				case "crash":
					{
						Timer.DelayCall(() => { throw new Exception("Forced Crash"); });
					}
					break;
				case "shutdown":
					{
						AutoSave.Save();
						Core.Kill(false);
					}
					break;
				case "shutdown nosave":
					{
						Core.Kill(false);
					}
					break;
				case "restart":
					{
						AutoSave.Save();
						Core.Kill(true);
					}
					break;
				case "restart nosave":
					{
						Core.Kill(true);
					}
					break;
				case "online":
					{
						var states = NetState.Instances;

						if (states.Count == 0)
						{
							Console.WriteLine("There are no users online at this time.");
						}

						foreach (var t in states)
						{
							var a = t.Account as Account;

							if (a == null)
							{
								continue;
							}

							var m = t.Mobile;

							if (m != null)
							{
								Console.WriteLine("- Account: {0}, Name: {1}, IP: {2}", a.Username, m.Name, t);
							}
						}
					}
					break;
				case "save":
					AutoSave.Save();
					break;
				case "hear": // Credit to Zippy for the HearAll script!
					{
						_HearConsole = !_HearConsole;

						Console.WriteLine("{0} sending speech to the console.", _HearConsole ? "Now" : "No longer");
					}
					break;
				default:
					DisplayHelp();
					break;
			}
		}

		private static void DisplayHelp()
		{
			Console.WriteLine(" ");
			Console.WriteLine("Commands:");
			Console.WriteLine("crash           - Forces an exception to be thrown.");
			Console.WriteLine("save            - Performs a forced save.");
			Console.WriteLine("shutdown        - Performs a forced save then shuts down the server.");
			Console.WriteLine("shutdown nosave - Shuts down the server without saving.");
			Console.WriteLine("restart         - Sends a message to players informing them that the server is");
			Console.WriteLine("                  restarting, performs a forced save, then shuts down and");
			Console.WriteLine("                  restarts the server.");
			Console.WriteLine("restart nosave  - Restarts the server without saving.");
			Console.WriteLine("online          - Shows a list of every person online:");
			Console.WriteLine("                  Account, Char Name, IP.");
			Console.WriteLine("bc <message>    - Type this command and your message after it.");
			Console.WriteLine("                  It will then be sent to all players.");
			Console.WriteLine("sc <message>    - Type this command and your message after it.");
			Console.WriteLine("                  It will then be sent to all staff.");
			Console.WriteLine("hear            - Copies all local speech to this console:");
			Console.WriteLine("                  Char Name (Region name): Speech.");
			Console.WriteLine("ban <name>      - Kicks and bans the users account.");
			Console.WriteLine("kick <name>     - Kicks the user.");
			Console.WriteLine("pages           - Enter page mode to handle help requests.");
			Console.WriteLine("help|?          - Shows this list.");
			Console.WriteLine(" ");
		}

		private static void DisplayPagingHelp()
		{
			Console.WriteLine(" ");
			Console.WriteLine("Paging Commands:");
			Console.WriteLine("view <id>              - View sender message.");
			Console.WriteLine("remove <id>            - Remove without message.");
			Console.WriteLine("handle <id> <message>  - Remove with message.");
			Console.WriteLine("clear                  - Clears the page queue.");
			Console.WriteLine("exit                   - Exit page mode.");
			Console.WriteLine("help|?                 - Shows this list.");
			Console.WriteLine(" ");
		}

		private static void HandlePaging(string sub)
		{
			if (sub.StartsWith("help", StringComparison.OrdinalIgnoreCase) ||
				sub.StartsWith("?", StringComparison.OrdinalIgnoreCase))
			{
				DisplayPagingHelp();

				HandlePaging(String.Empty);
				return;
			}

			if (PageQueue.List.Count == 0)
			{
				Console.WriteLine("There are no pages in the queue.");

				if (_Pages != null)
				{
					_Pages = null;

					Console.WriteLine("[Pages]: Disabled page mode.");
				}

				return;
			}

			if (String.IsNullOrWhiteSpace(sub))
			{
				if (_Pages == null)
				{
					Console.WriteLine("[Pages]: Enabled page mode.");

					DisplayPagingHelp();
				}

				_Pages = PageQueue.List.Cast<PageEntry>().ToArray();

				const string format = "{0:D3}:\t{1}\t{2}";

				for (var i = 0; i < _Pages.Length; i++)
				{
					Console.WriteLine(format, i + 1, _Pages[i].Type, _Pages[i].Sender);
				}

				return;
			}

			if (sub.StartsWith("exit", StringComparison.OrdinalIgnoreCase))
			{
				if (_Pages != null)
				{
					_Pages = null;

					Console.WriteLine("[Pages]: Disabled page mode.");
				}

				return;
			}

			if (sub.StartsWith("clear", StringComparison.OrdinalIgnoreCase))
			{
				if (_Pages != null)
				{
					foreach (var page in _Pages)
					{
						PageQueue.Remove(page);
					}

					Console.WriteLine("[Pages]: Queue cleared.");

					Array.Clear(_Pages, 0, _Pages.Length);

					_Pages = null;

					Console.WriteLine("[Pages]: Disabled page mode.");
				}

				return;
			}

			if (sub.StartsWith("remove", StringComparison.OrdinalIgnoreCase))
			{
				string[] args;

				var page = FindPage(sub, out args);

				if (page == null)
				{
					Console.WriteLine("[Pages]: Invalid page entry.");
				}
				else
				{
					PageQueue.Remove(page);

					Console.WriteLine("[Pages]: Removed from queue.");
				}

				HandlePaging(String.Empty);
				return;
			}

			if (sub.StartsWith("handle", StringComparison.OrdinalIgnoreCase))
			{
				string[] args;

				var page = FindPage(sub, out args);

				if (page == null)
				{
					Console.WriteLine("[Pages]: Invalid page entry.");

					HandlePaging(String.Empty);
					return;
				}

				if (args.Length <= 0)
				{
					Console.WriteLine("[Pages]: Message required.");

					HandlePaging(String.Empty);
					return;
				}

				page.Sender.SendGump(new MessageSentGump(page.Sender, ServerList.ServerName, String.Join(" ", args)));

				Console.WriteLine("[Pages]: Message sent.");

				PageQueue.Remove(page);

				Console.WriteLine("[Pages]: Removed from queue.");

				HandlePaging(String.Empty);
				return;
			}

			if (sub.StartsWith("view", StringComparison.OrdinalIgnoreCase))
			{
				string[] args;

				var page = FindPage(sub, out args);

				if (page == null)
				{
					Console.WriteLine("[Pages]: Invalid page entry.");

					HandlePaging(String.Empty);
					return;
				}

				var idx = Array.IndexOf(_Pages, page) + 1;

				Console.WriteLine("[Pages]: {0:D3}:\t{1}\t{2}", idx, page.Type, page.Sender);

				if (!String.IsNullOrWhiteSpace(page.Message))
				{
					Console.WriteLine("[Pages]: {0}", page.Message);
				}
				else
				{
					Console.WriteLine("[Pages]: No message supplied.");
				}

				HandlePaging(String.Empty);
				return;
			}

			if (_Pages != null)
			{
				string[] args;

				var page = FindPage(sub, out args);

				if (page != null)
				{
					var idx = Array.IndexOf(_Pages, page) + 1;

					Console.WriteLine("[Pages]: {0:D3}:\t{1}\t{2}", idx, page.Type, page.Sender);

					if (!String.IsNullOrWhiteSpace(page.Message))
					{
						Console.WriteLine("[Pages]: {0}", page.Message);
					}
					else
					{
						Console.WriteLine("[Pages]: No message supplied.");
					}

					HandlePaging(String.Empty);
					return;
				}

				Array.Clear(_Pages, 0, _Pages.Length);

				_Pages = null;

				Console.WriteLine("[Pages]: Disabled page mode.");
			}
		}

		private static PageEntry FindPage(string sub, out string[] args)
		{
			args = sub.Split(' ');

			if (args.Length > 1)
			{
				sub = args[1];

				if (args.Length > 2)
				{
					args = args.Skip(2).ToArray();
				}
				else
				{
					args = args.Skip(1).ToArray();
				}
			}

			int id;

			if (Int32.TryParse(sub, out id) && --id >= 0 && id < _Pages.Length)
			{
				var page = _Pages[id];

				if (PageQueue.List.Contains(page))
				{
					return page;
				}
			}

			return null;
		}

		public static void BroadcastMessage(AccessLevel ac, int hue, string message)
		{
			World.Broadcast(hue, false, ac, message);
		}
	}
}
