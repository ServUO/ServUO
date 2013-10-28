#region Header
// **********
// ServUO - BaseService.cs
// **********
#endregion

#region References
using Server;
using Server.Gumps;
#endregion

namespace CustomsFramework
{
	public class BaseService : SaveData, ICustomsEntity, ISerializable
	{
		public BaseService()
		{ }

		public BaseService(CustomSerial serial)
			: base(serial)
		{ }

		public override string Name { get { return @"Base Service"; } }
		public virtual string Description { get { return @"Base Service, inherit from this class and override the interface items."; } }
		public virtual string Version { get { return "1.0"; } }
		public virtual AccessLevel EditLevel { get { return AccessLevel.Developer; } }
		public virtual Gump SettingsGump { get { return null; } }

		public override string ToString()
		{
			return Name;
		}

		public override void Prep()
		{ }

		public override void Delete()
		{ }

		public override void Serialize(GenericWriter writer)
		{
			writer.WriteVersion(0);
			//Version 0
		}

		public override void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
					{
						break;
					}
			}
		}
	}
}