#region Header
// **********
// ServUO - ConsoleCommands.cs
// **********
#endregion

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
		private static bool _HearConsole;
		private static PageEntry[] _List;

		public static bool Paging { get; set; }

		public static void Initialize()
		{
			EventSink.ServerStarted += () =>
			{
				ThreadPool.QueueUserWorkItem(ConsoleListen);

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
						Console.WriteLine("" + args.Mobile.Name + " (" + args.Mobile.Region.Name + "): " + args.Speech + "");
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

		public static void ConsoleListen(Object stateInfo)
		{
			if (Core.Crashed)
			{
				return;
			}

			if (!Paging)
			{
				try
				{
					Next(Console.ReadLine());
				}
				catch
				{ }
			}
		}

		public static void PageResp(object obj)
		{
			if (Core.Crashed)
			{
				return;
			}

			Paging = true;

			var objects = (object[])obj;
			var w = (int)objects[0];
			var pag = (int)objects[1];
			int paG;

			if (w == 1)
			{
				up:
				try
				{
					paG = Convert.ToInt32(Console.ReadLine());
				}
				catch
				{
					Console.WriteLine("Thats not a number,try again.");
					goto up;
				}

				Console.WriteLine("Type your response");

				ThreadPool.QueueUserWorkItem(PageResp, new object[] {2, paG});
			}
			else
			{
				var resp = Console.ReadLine();
				var list = PageQueue.List;

				_List = (PageEntry[])list.ToArray(typeof(PageEntry));

				if (_List.Length > 0)
				{
					if (pag > _List.Length)
					{
						Console.WriteLine("Error: Not a valid page number");
					}
					else
					{
						for (var i = 0; i < _List.Length; ++i)
						{
							var e = _List[i];

							if (i != pag)
							{
								continue;
							}

							e.Sender.SendGump(new MessageSentGump(e.Sender, "Admin", resp));
							PageQueue.Remove(e);

							Console.WriteLine("Message Sent...");
						}
					}
				}
				else
				{
					Console.WriteLine("There are no pages to display.");
				}
			}

			Paging = false;

			ThreadPool.QueueUserWorkItem(ConsoleListen);
		}

		public static void BroadcastMessage(AccessLevel ac, int hue, string message)
		{
			foreach (var m in NetState.Instances.Select(state => state.Mobile).Where(m => m != null && m.AccessLevel >= ac))
			{
				m.SendMessage(hue, message);
			}
		}

		public static void Next(string input)
		{
			if (Core.Crashed)
			{
				return;
			}

			input = input.ToLower();

			if (input.StartsWith("bc"))
			{
				var sub = input.Replace("bc", "");

				BroadcastMessage(AccessLevel.Player, 0x35, String.Format("[Admin] {0}", sub));
				Console.WriteLine("Players will see: {0}", sub);
			}
			else if (input.StartsWith("sc"))
			{
				var sub = input.Replace("staff", "");

				BroadcastMessage(AccessLevel.Counselor, 0x32, String.Format("[Admin] {0}", sub));
				Console.WriteLine("Staff will see: {0}", sub);
			}
			else if (input.StartsWith("ban"))
			{
				var sub = input.Replace("ban", "");
				var states = NetState.Instances;

				if (states.Count == 0)
				{
					Console.WriteLine("There are no players online.");
				}

				foreach (var t in states)
				{
					var a = t.Account as Account;

					if (a == null)
					{
						continue;
					}

					var m = t.Mobile;

					if (m == null)
					{
						continue;
					}

					sub = sub.ToLower();

					if (m.Name.ToLower() != sub.Trim())
					{
						continue;
					}

					var m_ns = m.NetState;

					Console.WriteLine("Mobile name: '{0}' Account name: '{1}'", m.Name, a.Username);

					a.Banned = true;
					m_ns.Dispose();

					Console.WriteLine("Banning complete.");
				}
			}
			else if (input.StartsWith("kick"))
			{
				var sub = input.Replace("kick", "");
				var states = NetState.Instances;

				if (states.Count == 0)
				{
					Console.WriteLine("There are no players online.");
				}

				foreach (var t in states)
				{
					var a = t.Account as Account;

					if (a == null)
					{
						continue;
					}

					var m = t.Mobile;

					if (m == null)
					{
						continue;
					}

					sub = sub.ToLower();

					if (m.Name.ToLower() != sub.Trim())
					{
						continue;
					}

					var m_ns = m.NetState;

					Console.WriteLine("Mobile name: '{0}' Account name: '{1}'", m.Name, a.Username);

					m_ns.Dispose();

					Console.WriteLine("Kicking complete.");
				}
			}
			else
			{
				switch (input.Trim())
				{
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
						World.Save();
						break;
					case "hear": //credit to Zippy for the HearAll script!
					{
						_HearConsole = !_HearConsole;

						Console.WriteLine(
							_HearConsole ? "Now sending all speech to the console." : "No longer sending speech to the console.");
					}
						break;
					case "pages":
					{
						Paging = true;

						var list = PageQueue.List;
						PageEntry e;

						for (var i = 0; i < list.Count;)
						{
							e = (PageEntry)list[i];

							if (e.Sender.Deleted || e.Sender.NetState == null)
							{
								e.AddResponse(e.Sender, "[Logout]");
								PageQueue.Remove(e);
							}
							else
							{
								++i;
							}
						}

						_List = (PageEntry[])list.ToArray(typeof(PageEntry));

						if (_List.Length > 0)
						{
							for (var i = 0; i < _List.Length; ++i)
							{
								e = _List[i];

								var type = PageQueue.GetPageTypeName(e.Type);

								Console.WriteLine("--------------Page Number: " + i + " --------------------");
								Console.WriteLine("Player   :" + e.Sender.Name);
								Console.WriteLine("Catagory :" + type);
								Console.WriteLine("Message  :" + e.Message);
							}

							Console.WriteLine("Type the number of the page to respond to.");

							ThreadPool.QueueUserWorkItem(PageResp, new object[] {1, 2});
						}
						else
						{
							Console.WriteLine("No pages to display.");

							Paging = false;
						}
					}
						break;
					//case "help":
					//case "list":
					default:
					{
						Console.WriteLine(" ");
						Console.WriteLine("Commands:");
						Console.WriteLine("save            - Performs a save.");
						Console.WriteLine("shutdown        - Performs a save, then shuts down the server");
						Console.WriteLine("shutdown nosave - Shuts down the server without saving");
						Console.WriteLine("restart         - Performs a save, then restarts the server");
						Console.WriteLine("restart nosave  - Restarts the server without saving");
						Console.WriteLine("online          - Shows a list of every person online:");
						Console.WriteLine("                      Account, Char Name, IP");
						Console.WriteLine("bc <message>    - Sends a message to all players");
						Console.WriteLine("sc <message>    - Sends a message to all staff");
						Console.WriteLine("hear            - Forwards all local speech to this console:");
						Console.WriteLine("                      Char Name (Region name): Speech");
						Console.WriteLine("pages           - Manage help pages");
						Console.WriteLine("ban <name>      - Kicks and bans the user");
						Console.WriteLine("kick <name>     - Kicks the user");
						Console.WriteLine("list or help    - Shows this list");
						Console.WriteLine(" ");
					}
						break;
				}
			}

			if (!Paging)
			{
				ThreadPool.QueueUserWorkItem(ConsoleListen);
			}
		}
	}
}