#region References
using System;
using System.Collections;
using System.Diagnostics;
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
			if(Console.Out == null ||
				Console.In == null ||
				Console.Error == null)
			{
				return;
			}

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
			if (!Paging)
			{
				try
				{
					Next(Console.ReadLine());
				}
				catch (Exception) { }
			}
		}

		public static void PageResp(object obj)
		{
			Paging = true;

			var objects = (object[])obj;
			int w = (int)objects[0];
			int pag = (int)objects[1];
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
				string resp = Console.ReadLine();
				ArrayList list = PageQueue.List;

				_List = (PageEntry[])list.ToArray(typeof(PageEntry));

				if (_List.Length > 0)
				{
					if (pag > _List.Length)
					{
						Console.WriteLine("Error: Not a valid page number");
					}
					else
					{
						for (int i = 0; i < _List.Length; ++i)
						{
							PageEntry e = _List[i];

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
			foreach (Mobile m in NetState.Instances.Select(state => state.Mobile).Where(m => m != null && m.AccessLevel >= ac))
			{
				m.SendMessage(hue, message);
			}
		}

		public static void Next(string input)
		{
			input = input.ToLower();

			if (input.StartsWith("bc"))
			{
				string sub = input.Replace("bc", "");

				BroadcastMessage(AccessLevel.Player, 0x35, String.Format("[Admin] {0}", sub));
				Console.WriteLine("Players will see: {0}", sub);
			}
			else if (input.StartsWith("sc"))
			{
				string sub = input.Replace("staff", "");

				BroadcastMessage(AccessLevel.Counselor, 0x32, String.Format("[Admin] {0}", sub));
				Console.WriteLine("Staff will see: {0}", sub);
			}
			else if (input.StartsWith("ban"))
			{
				string sub = input.Replace("ban", "");
				var states = NetState.Instances;

				if (states.Count == 0)
				{
					Console.WriteLine("There are no players online.");
				}

				foreach (NetState t in states)
				{
					Account a = t.Account as Account;

					if (a == null)
					{
						continue;
					}

					Mobile m = t.Mobile;

					if (m == null)
					{
						continue;
					}

					sub = sub.ToLower();

					if (m.Name.ToLower() != sub.Trim())
					{
						continue;
					}

					NetState m_ns = m.NetState;

					Console.WriteLine("Mobile name: '{0}' Account name: '{1}'", m.Name, a.Username);

					a.Banned = true;
					m_ns.Dispose();

					Console.WriteLine("Banning complete.");
				}
			}
			else if (input.StartsWith("kick"))
			{
				string sub = input.Replace("kick", "");
				var states = NetState.Instances;

				if (states.Count == 0)
				{
					Console.WriteLine("There are no players online.");
				}

				foreach (NetState t in states)
				{
					Account a = t.Account as Account;

					if (a == null)
					{
						continue;
					}

					Mobile m = t.Mobile;

					if (m == null)
					{
						continue;
					}

					sub = sub.ToLower();

					if (m.Name.ToLower() != sub.Trim())
					{
						continue;
					}

					NetState m_ns = m.NetState;

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
							Core.Process.Kill();
						}
						break;
					case "shutdown nosave":
						Core.Process.Kill();
						break;
					case "restart":
						{
							BroadcastMessage(AccessLevel.Player, 0x35, String.Format("[Server] We are restarting..."));
							AutoSave.Save();
							Process.Start(Core.ExePath, Core.Arguments);
							Core.Process.Kill();
						}
						break;
					case "restart nosave":
						{
							Process.Start(Core.ExePath, Core.Arguments);
							Core.Process.Kill();
						}
						break;
					case "online":
						{
							var states = NetState.Instances;

							if (states.Count == 0)
							{
								Console.WriteLine("There are no users online at this time.");
							}

							foreach (NetState t in states)
							{
								Account a = t.Account as Account;

								if (a == null)
								{
									continue;
								}

								Mobile m = t.Mobile;

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

							ArrayList list = PageQueue.List;
							PageEntry e;

							for (int i = 0; i < list.Count;)
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
								for (int i = 0; i < _List.Length; ++i)
								{
									e = _List[i];

									string type = PageQueue.GetPageTypeName(e.Type);

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
							Console.WriteLine("save            - Performs a forced save.");
							Console.WriteLine("shutdown        - Performs a forced save then shuts down the server.");
							Console.WriteLine("shutdown nosave - Shuts down the server without saving.");
							Console.WriteLine("restart         - Sends a message to players informing them that the server is");
							Console.WriteLine("                      restarting, performs a forced save, then shuts down and");
							Console.WriteLine("                      restarts the server.");
							Console.WriteLine("restart nosave  - Restarts the server without saving.");
							Console.WriteLine("online          - Shows a list of every person online:");
							Console.WriteLine("                      Account, Char Name, IP.");
							Console.WriteLine("bc <message>    - Type this command and your message after it. It will then be");
							Console.WriteLine("                      sent to all players.");
							Console.WriteLine("sc <message>    - Type this command and your message after it.It will then be ");
							Console.WriteLine("                      sent to all staff.");
							Console.WriteLine("hear            - Copies all local speech to this console:");
							Console.WriteLine("                      Char Name (Region name): Speech.");
							Console.WriteLine("pages           - Shows all the pages in the page queue,you type the page");
							Console.WriteLine("                      number ,then you type your response to the player.");
							Console.WriteLine("ban <playername>- Kicks and bans the users account.");
							Console.WriteLine("kick <playername>- Kicks the user.");
							Console.WriteLine("list or help    - Shows this list.");
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