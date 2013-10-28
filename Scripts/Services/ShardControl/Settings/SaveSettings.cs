#region References
using System;
using System.Collections.Generic;

using Server;
#endregion

namespace CustomsFramework.Systems.ShardControl
{
	[PropertyObject]
	public sealed class SaveSettings : BaseSettings
	{
		public enum CompressionLevel
		{
			None = 0,
			Fast = 1,
			Low = 2,
			Normal = 3,
			High = 4,
			Ultra = 5,
		}

		#region Properties
		private SaveStrategy _SaveStrategy;

		[CommandProperty(AccessLevel.Administrator)]
		public bool SavesEnabled { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public AccessLevel SaveAccessLevel { get; set; }

		[CommandProperty(AccessLevel.Administrator, true)]
		public SaveStrategy SaveStrategy
		{
			get { return _SaveStrategy; }
			set
			{
				if (!Core.MultiProcessor && !(value is StandardSaveStrategy))
				{
					_SaveStrategy = new StandardSaveStrategy();
				}
				else
				{
					if (Core.ProcessorCount == 2 && (value is DualSaveStrategy || value is DynamicSaveStrategy))
					{
						_SaveStrategy = value;
					}
					else if (Core.ProcessorCount > 2)
					{
						_SaveStrategy = value;
					}
				}
			}
		}

		// Create a method to verify proper delay order
		[CommandProperty(AccessLevel.Administrator, true)]
		public List<TimeSpan> WarningDelays { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public bool AllowBackgroundWrite { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public TimeSpan SaveDelay { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public int NoIOHour { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public bool EnableEmergencyBackups { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public int EmergencyBackupHour { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public CompressionLevel Compression { get; set; }
		#endregion

		public SaveSettings(
			SaveStrategy saveStrategy,
			List<TimeSpan> warningDelays,
			TimeSpan saveDelay,
			bool savesEnabled = true,
			AccessLevel saveAccessLevel = AccessLevel.Administrator,
			bool allowBackgroundWrite = false,
			int noIOHour = -1,
			bool enableEmergencyBackups = true,
			int emergencyBackupHour = 3,
			CompressionLevel compressionLevel = CompressionLevel.Normal)
		{
			SavesEnabled = savesEnabled;
			SaveAccessLevel = saveAccessLevel;
			_SaveStrategy = saveStrategy;
			AllowBackgroundWrite = allowBackgroundWrite;
			SaveDelay = saveDelay;
			WarningDelays = warningDelays;
			NoIOHour = noIOHour;
			EnableEmergencyBackups = enableEmergencyBackups;
			EmergencyBackupHour = emergencyBackupHour;
			Compression = compressionLevel;
		}

		public override void Serialize(GenericWriter writer)
		{
			int version = writer.WriteVersion(0);

			switch (version)
			{
				case 0:
					{
						writer.Write(SavesEnabled);
						writer.Write((byte)SaveAccessLevel);
						writer.Write((byte)_SaveStrategy.GetSaveType());
						writer.Write(AllowBackgroundWrite);
						writer.Write(SaveDelay);

						writer.Write(WarningDelays.Count);

						foreach (TimeSpan t in WarningDelays)
						{
							writer.Write(t);
						}

						writer.Write(NoIOHour);

						writer.Write(EnableEmergencyBackups);
						writer.Write(EmergencyBackupHour);
						writer.Write((byte)Compression);
					}
					break;
			}
		}

		public SaveSettings(GenericReader reader)
			: base(reader)
		{ }

		public override void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					{
						SavesEnabled = reader.ReadBool();
						SaveAccessLevel = (AccessLevel)reader.ReadByte();
						_SaveStrategy = ((SaveStrategyTypes)reader.ReadByte()).GetSaveStrategy();
						AllowBackgroundWrite = reader.ReadBool();
						SaveDelay = reader.ReadTimeSpan();
						WarningDelays = new List<TimeSpan>();

						int count = reader.ReadInt();

						for (int i = 0; i < count; i++)
						{
							WarningDelays.Add(reader.ReadTimeSpan());
						}

						NoIOHour = reader.ReadInt();

						EnableEmergencyBackups = reader.ReadBool();
						EmergencyBackupHour = reader.ReadInt();
						Compression = (CompressionLevel)reader.ReadByte();
						break;
					}
			}
		}

		public override string ToString()
		{
			return "Save Settings";
		}
	}
}