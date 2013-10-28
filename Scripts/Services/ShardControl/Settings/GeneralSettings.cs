#region References
using Server;
#endregion

namespace CustomsFramework.Systems.ShardControl
{
	public sealed class GeneralSettings : BaseSettings
	{
		#region Properties
		[CommandProperty(AccessLevel.Owner)]
		public string ShardName { get; set; }

		[CommandProperty(AccessLevel.Owner)]
		public bool AutoDetect { get; set; }

		[CommandProperty(AccessLevel.Owner)]
		public string Address { get; set; }

		[CommandProperty(AccessLevel.Owner)]
		public int Port { get; set; }

		[CommandProperty(AccessLevel.Owner)]
		public Expansion Expansion { get; set; }

		[CommandProperty(AccessLevel.Owner)]
		public AccessLevel MaxPlayerLevel { get; set; }

		[CommandProperty(AccessLevel.Owner)]
		public AccessLevel LowestStaffLevel { get; set; }

		[CommandProperty(AccessLevel.Owner)]
		public AccessLevel LowestOwnerLevel { get; set; }
		#endregion

		public GeneralSettings(
			string shardName = "My Shard",
			bool autoDetect = true,
			string address = null,
			int port = 2593,
			Expansion expansion = Expansion.SA,
			AccessLevel maxPlayerLevel = AccessLevel.VIP,
			AccessLevel lowestStaffLevel = AccessLevel.Counselor,
			AccessLevel lowestOwnerLevel = AccessLevel.CoOwner)
		{
			ShardName = shardName;
			AutoDetect = autoDetect;
			Address = address;
			Port = port;
			Expansion = expansion;
			MaxPlayerLevel = maxPlayerLevel;
			LowestStaffLevel = lowestStaffLevel;
			LowestOwnerLevel = lowestOwnerLevel;
		}

		public GeneralSettings(GenericReader reader)
			: base(reader)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			int version = writer.WriteVersion(1);

			switch (version)
			{
				case 1:
					{
						writer.Write((int)MaxPlayerLevel);
						writer.Write((int)LowestStaffLevel);
						writer.Write((int)LowestOwnerLevel);
					}
					goto case 0;
				case 0:
					{
						writer.Write(ShardName);
						writer.Write(AutoDetect);
						writer.Write(Address);
						writer.Write(Port);
						writer.Write((byte)Expansion);
					}
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();

			switch (version)
			{
				case 1:
					{
						MaxPlayerLevel = (AccessLevel)reader.ReadInt();
						LowestStaffLevel = (AccessLevel)reader.ReadInt();
						LowestOwnerLevel = (AccessLevel)reader.ReadInt();
					}
					goto case 0;
				case 0:
					{
						ShardName = reader.ReadString();
						AutoDetect = reader.ReadBool();
						Address = reader.ReadString();
						Port = reader.ReadInt();
						Expansion = (Expansion)reader.ReadByte();
					}
						break;
			}
		}

		public override string ToString()
		{
			return "General Settings";
		}
	}
}