#region References
using System;

using Server;
#endregion

namespace CustomsFramework.Systems.ShardControl
{
	public sealed class ClientSettings : BaseSettings
	{
		#region Properties
		[CommandProperty(AccessLevel.Administrator)]
		public bool AutoDetectClient { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public string ClientPath { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public OldClientResponse OldClientResponse { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public ClientVersion RequiredClientVersion { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public bool AllowRegular { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public bool AllowUOTD { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public bool AllowGod { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public TimeSpan AgeLeniency { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public TimeSpan GameTimeLeniency { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public TimeSpan KickDelay { get; set; }
		#endregion

		public ClientSettings(
			TimeSpan ageLeniency,
			TimeSpan gameTimeLeniency,
			TimeSpan kickDelay,
			bool autoDetectClient = false,
			string clientPath = null,
			OldClientResponse oldClientResponse = OldClientResponse.LenientKick,
			ClientVersion requiredVersion = null,
			bool allowRegular = true,
			bool allowUOTD = true,
			bool allowGod = true)
		{
			AutoDetectClient = autoDetectClient;
			ClientPath = clientPath;
			OldClientResponse = oldClientResponse;
			RequiredClientVersion = requiredVersion;
			AllowRegular = allowRegular;
			AllowUOTD = allowUOTD;
			AllowGod = allowGod;
			AgeLeniency = ageLeniency;
			GameTimeLeniency = gameTimeLeniency;
			KickDelay = kickDelay;
		}

		public ClientSettings(GenericReader reader)
			: base(reader)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			int version = writer.WriteVersion(0);

			switch (version)
			{
				case 0:
					{
						writer.Write(AutoDetectClient);
						writer.Write(ClientPath);
						writer.Write((byte)OldClientResponse);

						writer.Write(RequiredClientVersion.Major);
						writer.Write(RequiredClientVersion.Minor);
						writer.Write(RequiredClientVersion.Revision);
						writer.Write(RequiredClientVersion.Patch);

						writer.Write(AllowRegular);
						writer.Write(AllowUOTD);
						writer.Write(AllowGod);
						writer.Write(AgeLeniency);
						writer.Write(GameTimeLeniency);
						writer.Write(KickDelay);
					}
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					{
						AutoDetectClient = reader.ReadBool();
						ClientPath = reader.ReadString();
						OldClientResponse = (OldClientResponse)reader.ReadByte();

						RequiredClientVersion = new ClientVersion(reader.ReadInt(), reader.ReadInt(), reader.ReadInt(), reader.ReadInt());

						AllowRegular = reader.ReadBool();
						AllowUOTD = reader.ReadBool();
						AllowGod = reader.ReadBool();
						AgeLeniency = reader.ReadTimeSpan();
						GameTimeLeniency = reader.ReadTimeSpan();
						KickDelay = reader.ReadTimeSpan();
					}
					break;
			}
		}

		public override string ToString()
		{
			return "Client Settings";
		}
	}
}