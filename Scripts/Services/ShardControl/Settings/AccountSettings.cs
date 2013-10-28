#region References
using System;

using Server;
using Server.Misc;
#endregion

namespace CustomsFramework.Systems.ShardControl
{
	public sealed class AccountSettings : BaseSettings
	{
		#region Properties
		[CommandProperty(AccessLevel.Administrator)]
		public int AccountsPerIP { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public int HousesPerAccount { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public int MaxHousesPerAccount { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public bool AutoAccountCreation { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public bool RestrictDeletion { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public TimeSpan DeleteDelay { get; set; }

		[CommandProperty(AccessLevel.Administrator)]
		public PasswordProtection PasswordProtection { get; set; }
		#endregion

		public AccountSettings(
			TimeSpan deleteDelay,
			int accountsPerIP = 1,
			int housesPerAccount = 2,
			int maxHousesPerAccount = 4,
			bool autoAccountCreation = true,
			bool restrictDeletion = true,
			PasswordProtection passwordProtection = PasswordProtection.NewCrypt)
		{
			AccountsPerIP = accountsPerIP;
			HousesPerAccount = housesPerAccount;
			MaxHousesPerAccount = maxHousesPerAccount;
			AutoAccountCreation = autoAccountCreation;
			RestrictDeletion = restrictDeletion;
			DeleteDelay = deleteDelay;
			PasswordProtection = passwordProtection;
		}

		public AccountSettings(GenericReader reader)
			: base(reader)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			int version = writer.WriteVersion(0);

			switch (version)
			{
				case 0:
					{
						writer.Write(AccountsPerIP);
						writer.Write(HousesPerAccount);
						writer.Write(MaxHousesPerAccount);
						writer.Write(AutoAccountCreation);
						writer.Write(RestrictDeletion);
						writer.Write(DeleteDelay);
						writer.Write((byte)PasswordProtection);
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
						AccountsPerIP = reader.ReadInt();
						HousesPerAccount = reader.ReadInt();
						MaxHousesPerAccount = reader.ReadInt();
						AutoAccountCreation = reader.ReadBool();
						RestrictDeletion = reader.ReadBool();
						DeleteDelay = reader.ReadTimeSpan();
						PasswordProtection = (PasswordProtection)reader.ReadByte();
					}
					break;
			}
		}

		public override string ToString()
		{
			return "Account Settings";
		}
	}
}