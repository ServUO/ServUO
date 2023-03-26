using System;
using System.Diagnostics;

using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Misc
{
	public enum InvalidClientAction
	{
		Ignore,
		Warn,
		Annoy,
		LenientKick,
		Kick
	}

	public static class ClientVerification
	{
		[ConfigProperty("Client.InvalidClientResponse")]
		public static InvalidClientAction InvalidClientResponse { get => Config.GetEnum("Client.InvalidClientResponse", InvalidClientAction.LenientKick); set => Config.SetEnum("Client.InvalidClientResponse", value); }

		[ConfigProperty("Client.AgeLeniency")]
		public static TimeSpan AgeLeniency { get => TimeSpan.FromDays(Config.Get("Client.AgeLeniency", 10.0)); set => Config.Set("Client.AgeLeniency", value.TotalDays); }

		[ConfigProperty("Client.GameTimeLeniency")]
		public static TimeSpan GameTimeLeniency { get => TimeSpan.FromHours(Config.Get("Client.GameTimeLeniency", 25.0)); set => Config.Set("Client.GameTimeLeniency", value.TotalHours); }

		[ConfigProperty("Client.KickDelay")]
		public static TimeSpan KickDelay { get => TimeSpan.FromSeconds(Config.Get("Client.KickDelay", 20.0)); set => Config.Set("Client.KickDelay", value.TotalSeconds); }

		[ConfigProperty("Client.UpdateNote")]
		public static string UpdateNote { get => Config.Get("Client.UpdateNote", default(string)); set => Config.Set("Client.UpdateNote", value); }

		[ConfigProperty("Client.AllowRegular")]
		public static bool AllowRegular { get => Config.Get("Client.AllowRegular", true); set => Config.Set("Client.AllowRegular", value); }

		[ConfigProperty("Client.AllowUOTD")]
		public static bool AllowUOTD { get => Config.Get("Client.AllowUOTD", true); set => Config.Set("Client.AllowUOTD", value); }

		[ConfigProperty("Client.AllowGod")]
		public static bool AllowGod { get => Config.Get("Client.AllowGod", true); set => Config.Set("Client.AllowGod", value); }

		[ConfigProperty("Client.AllowEC")]
		public static bool AllowEC { get => Config.Get("Client.AllowEC", true); set => Config.Set("Client.AllowEC", value); }

		private static readonly ClientVersion DefaultRequiredVersion = ClientVersion.Zero;
		private static readonly ClientVersion DefaultRequiredVersionEC = new ClientVersion(67, 0, 59, 0, ClientType.SA);

		[ConfigProperty("Client.RequiredVersion")]
		public static ClientVersion Required { get => Config.Get("Client.RequiredVersion", DefaultRequiredVersion); set => Config.Set("Client.RequiredVersion", value); }

		[ConfigProperty("Client.RequiredVersionEC")]
		public static ClientVersion RequiredEC { get => Config.Get("Client.RequiredVersionEC", DefaultRequiredVersionEC); set => Config.Set("Client.RequiredVersionLimitEC", value); }

		[ConfigProperty("Client.RequiredVersionLimit")]
		public static ClientVersion RequiredLimit { get => Config.Get("Client.RequiredVersionLimit", ClientVersion.Zero); set => Config.Set("Client.RequiredVersionLimit", value); }

		[ConfigProperty("Client.RequiredVersionLimitEC")]
		public static ClientVersion RequiredLimitEC { get => Config.Get("Client.RequiredVersionLimitEC", ClientVersion.Zero); set => Config.Set("Client.RequiredVersionLimitEC", value); }

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
			if (Required > ClientVersion.Zero)
			{
				if (RequiredLimit > ClientVersion.Zero)
				{
					Utility.WriteLine(ConsoleColor.White, $"Restricting classic client version to {Required} - {RequiredLimit}");
				}
				else
				{
					Utility.WriteLine(ConsoleColor.White, $"Restricting classic client version to {Required}");
				}
			}

			if (RequiredEC > ClientVersion.Zero)
			{
				if (RequiredLimitEC > ClientVersion.Zero)
				{
					Utility.WriteLine(ConsoleColor.White, $"Restricting enhanced client version to {RequiredEC} - {RequiredLimitEC}");
				}
				else
				{
					Utility.WriteLine(ConsoleColor.White, $"Restricting enhanced client version to {RequiredEC}");
				}
			}
		}

		private static bool TestVersion(bool enhanced, ClientVersion ver, out ClientVersion min, out ClientVersion max)
		{
			min = enhanced ? RequiredEC : Required;
			max = enhanced ? RequiredLimitEC : RequiredLimit;

			if (max > ClientVersion.Zero)
			{
				return ver >= min && ver <= max;
			}

			return ver >= min;
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

			if (!TestVersion(false, version, out var min, out var max))
			{
				switch (InvalidClientResponse)
				{
					case InvalidClientAction.Warn:
					{
						if (max > ClientVersion.Zero)
						{
							if (min != max)
							{
								state.Mobile.SendMessage(0x22, $"Your client version is invalid.  Your client version must be {min} to {max}");
							}
							else
							{
								state.Mobile.SendMessage(0x22, $"Your client version is invalid.  Your client version must be {max}");
							}
						}
						else
						{
							state.Mobile.SendMessage(0x22, $"Your client version is invalid.  Your client version must be {min} or higher.");
						}

						break;
					}
					case InvalidClientAction.Annoy:
					{
						SendAnnoyGump(state.Mobile);
						break;
					}
					case InvalidClientAction.Kick:
					{
						if (max > ClientVersion.Zero)
						{
							if (min != max)
							{
								kickMessage = $"This server requires client version {min} to {max}.";
							}
							else
							{
								kickMessage = $"This server requires client version {min}.";
							}
						}
						else
						{
							kickMessage = $"This server requires client version {min} or higher.";
						}

						break;
					}
					case InvalidClientAction.LenientKick:
					{
						if (state.Mobile is PlayerMobile p && p.GameTime > GameTimeLeniency && state.Mobile.Age > AgeLeniency)
						{
							kickMessage = $"This server requires your client version be at least {min}.";
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

			if (!TestVersion(true, version, out var min, out var max))
			{
				switch (InvalidClientResponse)
				{
					case InvalidClientAction.Warn:
					{
						state.Mobile.SendMessage(0x22, $"Your client version is invalid.  Your client version must be at least {min}");
						break;
					}
					case InvalidClientAction.Annoy:
					{
						SendAnnoyGump(state.Mobile);
						break;
					}
					case InvalidClientAction.Kick:
					{
						if (min > ClientVersion.Zero)
						{
							kickMessage = $"This server requires your client version be at least {min}.";
						}

						break;
					}
					case InvalidClientAction.LenientKick:
					{
						if (state.Mobile is PlayerMobile p && p.GameTime > GameTimeLeniency && state.Mobile.Age > AgeLeniency)
						{
							kickMessage = $"This server requires your client version be at least {min}.";
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

					if (InvalidClientResponse == InvalidClientAction.LenientKick)
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
