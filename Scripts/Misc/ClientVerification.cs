using System;
using System.Diagnostics;

using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Misc
{
	public enum OutdatedClientAction
	{
		Ignore,
		Warn,
		Annoy,
		LenientKick,
		Kick
	}

	public class ClientVerification
	{
		public static OutdatedClientAction OldClientResponse { get => Config.GetEnum("Client.OldClientResponse", OutdatedClientAction.LenientKick); set => Config.SetEnum("Client.OldClientResponse", value); }

		public static TimeSpan AgeLeniency { get => TimeSpan.FromDays(Config.Get("Client.AgeLeniency", 10.0)); set => Config.Set("Client.AgeLeniency", value.TotalDays); }
		public static TimeSpan GameTimeLeniency { get => TimeSpan.FromHours(Config.Get("Client.GameTimeLeniency", 25.0)); set => Config.Set("Client.GameTimeLeniency", value.TotalHours); }
		public static TimeSpan KickDelay { get => TimeSpan.FromSeconds(Config.Get("Client.KickDelay", 20.0)); set => Config.Set("Client.KickDelay", value.TotalSeconds); }

		public static string UpdateNote { get => Config.Get("Client.UpdateNote", default(string)); set => Config.Set("Client.UpdateNote", value); }

		public static bool AllowRegular { get => Config.Get("Client.AllowRegular", true); set => Config.Set("Client.AllowRegular", value); }
		public static bool AllowUOTD { get => Config.Get("Client.AllowUOTD", true); set => Config.Set("Client.AllowUOTD", value); }
		public static bool AllowGod { get => Config.Get("Client.AllowGod", true); set => Config.Set("Client.AllowGod", value); }
		public static bool AllowEC { get => Config.Get("Client.AllowEC", true); set => Config.Set("Client.AllowEC", value); }

		private static readonly ClientVersion DefaultRequiredVersion = ClientVersion.Zero;
		private static readonly ClientVersion DefaultRequiredVersionEC = new ClientVersion(67, 0, 59, 0, ClientType.SA);

		public static ClientVersion Required { get => Config.Get("Client.RequiredVersion", DefaultRequiredVersion); set => Config.Set("Client.RequiredVersion", value); }
		public static ClientVersion RequiredEC { get => Config.Get("Client.RequiredVersionEC", DefaultRequiredVersionEC); set => Config.Set("Client.RequiredVersionEC", value); }

		static ClientVerification()
		{
			var path = Core.FindDataFile("client.exe");

			if (path != null)
			{
				var info = FileVersionInfo.GetVersionInfo(path);

				if (info.FileMajorPart != 0 || info.FileMinorPart != 0 || info.FileBuildPart != 0 || info.FilePrivatePart != 0)
				{
					DefaultRequiredVersion = new ClientVersion(info.FileMajorPart, info.FileMinorPart, info.FileBuildPart, info.FilePrivatePart);
				}
			}
		}

		public static void Configure()
		{
			EventSink.ClientVersionReceived += EventSink_ClientVersionReceived;
			EventSink.ClientTypeReceived += EventSink_ClientTypeReceived;
		}

		public static void Initialize()
		{
			if (Required != ClientVersion.Zero)
			{
				Utility.WriteLine(ConsoleColor.White, $"Restricting classic client version to {Required}.");
			}

			if (RequiredEC != ClientVersion.Zero)
			{
				Utility.WriteLine(ConsoleColor.White, $"Restricting enhanced client version to {RequiredEC}.");
			}
		}

		private static void EventSink_ClientVersionReceived(ClientVersionReceivedArgs e)
		{
			var state = e.State;

			if (state == null || state.IsEnhancedClient || state.Mobile == null || state.Mobile.IsStaff())
			{
				return;
			}

			string kickMessage = null;

			var version = e.Version;
			var required = Required;

			if (required != ClientVersion.Zero && version < required)
			{
				switch (OldClientResponse)
				{
					case OutdatedClientAction.Warn:
					{
						state.Mobile.SendMessage(0x22, "Your client is out of date. Please update your client.");
						state.Mobile.SendMessage(0x22, $"Your client version must be at least {required}");
						break;
					}
					case OutdatedClientAction.Annoy:
					{
						SendAnnoyGump(state.Mobile);
						break;
					}
					case OutdatedClientAction.Kick:
					{
						kickMessage = $"This server requires your client version be at least {required}.";
						break;
					}
					case OutdatedClientAction.LenientKick:
					{
						if (state.Mobile is PlayerMobile p && p.GameTime > GameTimeLeniency && state.Mobile.Age > AgeLeniency)
						{
							kickMessage = $"This server requires your client version be at least {required}.";
						}
						else
						{
							SendAnnoyGump(state.Mobile);
						}

						break;
					}
				}
			}
			
			if (kickMessage == null && (!AllowGod || !AllowRegular || !AllowUOTD))
			{
				if (!AllowGod && version.Type == ClientType.God)
				{
					kickMessage = "This server does not allow god clients to connect.";
				}
				else if (!AllowRegular && version.Type == ClientType.Regular)
				{
					kickMessage = "This server does not allow regular clients to connect.";
				}
				else if (!AllowUOTD && state.IsUOTDClient)
				{
					kickMessage = "This server does not allow UO:TD clients to connect.";
				}

				if (!AllowGod && !AllowRegular && !AllowUOTD && !AllowEC)
				{
					kickMessage = "This server does not allow any clients to connect.";
				}
				else if (!AllowGod && !AllowRegular && !AllowUOTD && AllowEC)
				{
					kickMessage = "This server requires you to use the enhanced client.";
				}
				else if (AllowGod && !AllowRegular && !AllowUOTD && version.Type != ClientType.God)
				{
					kickMessage = "This server requires you to use the god client.";
				}
				else if (kickMessage != null)
				{
					if (AllowRegular)
					{
						kickMessage += " You can use regular clients.";
					}

					if (AllowUOTD)
					{
						kickMessage += " You can use UO:TD clients.";
					}

					if (AllowGod)
					{
						kickMessage += " You can use god clients.";
					}

					if (AllowEC)
					{
						kickMessage += " You can use enhanced clients.";
					}
				}
			}

			ProcessAction(state, kickMessage);
		}

		private static void EventSink_ClientTypeReceived(ClientTypeReceivedArgs e)
		{
			var state = e.State;

			if (state == null || !state.IsEnhancedClient || state.Mobile == null || state.Mobile.IsStaff())
			{
				return;
			}

			string kickMessage = null;

			var version = state.Version;
			var required = RequiredEC;

			if (required != ClientVersion.Zero && version < required)
			{
				switch (OldClientResponse)
				{
					case OutdatedClientAction.Warn:
					{
						state.Mobile.SendMessage(0x22, "Your client is out of date. Please update your client.");
						state.Mobile.SendMessage(0x22, $"Your client version must be at least {required}");
						break;
					}
					case OutdatedClientAction.Annoy:
					{
						SendAnnoyGump(state.Mobile);
						break;
					}
					case OutdatedClientAction.Kick:
					{
						kickMessage = $"This server requires your client version be at least {required}.";
						break;
					}
					case OutdatedClientAction.LenientKick:
					{
						if (state.Mobile is PlayerMobile p && p.GameTime > GameTimeLeniency && state.Mobile.Age > AgeLeniency)
						{
							kickMessage = $"This server requires your client version be at least {required}.";
						}
						else
						{
							SendAnnoyGump(state.Mobile);
						}

						break;
					}
				}
			}

			if (kickMessage == null && !AllowEC)
			{
				if (!AllowGod && version.Type == ClientType.God)
				{
					kickMessage = "This server does not allow god clients to connect.";
				}
				else if (!AllowRegular && version.Type == ClientType.Regular)
				{
					kickMessage = "This server does not allow regular clients to connect.";
				}
				else if (!AllowUOTD && state.IsUOTDClient)
				{
					kickMessage = "This server does not allow UO:TD clients to connect.";
				}

				if (!AllowGod && !AllowRegular && !AllowUOTD)
				{
					kickMessage = "This server does not allow any clients to connect.";
				}
				else if (AllowGod && !AllowRegular && !AllowUOTD && version.Type != ClientType.God)
				{
					kickMessage = "This server requires you to use the god client.";
				}
				else if (kickMessage != null)
				{
					if (AllowRegular)
					{
						kickMessage += " You can use regular clients.";
					}

					if (AllowUOTD)
					{
						kickMessage += " You can use UO:TD clients.";
					}

					if (AllowGod)
					{
						kickMessage += " You can use god clients.";
					}

					if (AllowEC)
					{
						kickMessage += " You can use enhanced clients.";
					}
				}
			}

			ProcessAction(state, kickMessage);
		}

		private static void ProcessAction(NetState state, string message)
		{
			if (message == null)
			{
				return;
			}

			state.Mobile.SendMessage(0x22, message);

			if (KickDelay > TimeSpan.Zero)
			{
				state.Mobile.SendMessage(0x22, $"You will be disconnected in {KickDelay.TotalSeconds:N0} seconds.");
			}

			Timer.DelayCall(KickDelay, ns =>
			{
				if (ns.Socket != null)
				{
					if (ns.Version < Required)
					{
						Utility.WriteLine(ConsoleColor.Red, $"Client: {ns}: Outdated version: {ns.Version}");
					}
					else
					{
						Utility.WriteLine(ConsoleColor.Red, $"Client: {state}: Disallowed client: {ns.Version.Type}");
					}

					ns.Dispose();
				}
			}, state);
		}

		private static void SendAnnoyGump(Mobile m)
		{
			if (m.NetState != null && m.NetState.Version < Required)
			{
				var msg = $"Your client is out of date.<br> This server recommends that your client version be at least {Required}.<br> You are currently using version {m.NetState.Version}.<br><br> {UpdateNote}";

				var g = new WarningGump(1060637, 30720, msg, 0xFFC000, 480, 360, (mob, selection, o) =>
				{
					m.SendMessage(0x22, "You will be reminded of this again.");

					if (OldClientResponse == OutdatedClientAction.LenientKick)
					{
						m.SendMessage(0x22, $"Outdated clients will be kicked after {AgeLeniency.TotalDays} days of character age and {GameTimeLeniency.TotalHours} hours of play time");
					}

					Timer.DelayCall(TimeSpan.FromMinutes(Utility.Random(5, 15)), SendAnnoyGump, m);
				}, null, false)
				{
					Dragable = false,
					Closable = false,
					Resizable = false
				};

				m.SendGump(g);
			}
		}
	}
}
