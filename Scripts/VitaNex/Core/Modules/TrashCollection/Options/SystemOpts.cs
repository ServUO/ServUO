#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using Server;
using Server.Commands;
using Server.Mobiles;

using VitaNex.SuperGumps;
#endregion

namespace VitaNex.Modules.TrashCollection
{
	public sealed class TrashCollectionOptions : CoreModuleOptions
	{
		private string _AdminCommand;

		private string _ProfilesCommand;

		public TrashCollectionOptions()
			: base(typeof(TrashCollection))
		{
			AdminCommand = "TrashAdmin";
			ProfilesCommand = "TrashProfiles";
			DateFormat = "m/d/y";
			DailyLimit = 500;
			UseTrashedProps = true;
			GiveBonusTokens = false;
		}

		public TrashCollectionOptions(GenericReader reader)
			: base(reader)
		{ }

		[CommandProperty(TrashCollection.Access)]
		public string AdminCommand
		{
			get => _AdminCommand;
			set => CommandUtility.Replace(_AdminCommand, TrashCollection.Access, HandleAdminCommand, (_AdminCommand = value));
		}

		[CommandProperty(TrashCollection.Access)]
		public string ProfilesCommand
		{
			get => _ProfilesCommand;
			set => CommandUtility.Replace(_ProfilesCommand, AccessLevel.Player, HandleProfilesCommand, (_ProfilesCommand = value));
		}

		[CommandProperty(TrashCollection.Access)]
		public string DateFormat { get; set; }

		[CommandProperty(TrashCollection.Access)]
		public int DailyLimit { get; set; }

		[CommandProperty(TrashCollection.Access)]
		public bool UseTrashedProps { get; set; }

		[CommandProperty(TrashCollection.Access)]
		public bool GiveBonusTokens { get; set; }

		public override void Clear()
		{
			base.Clear();

			DailyLimit = 0;
			UseTrashedProps = false;
			GiveBonusTokens = false;
		}

		public override void Reset()
		{
			base.Reset();

			AdminCommand = "TrashAdmin";
			ProfilesCommand = "TrashProfiles";

			DailyLimit = 500;
			UseTrashedProps = true;
			GiveBonusTokens = false;
		}

		public void HandleAdminCommand(CommandEventArgs e)
		{
			var m = e.Mobile as PlayerMobile;

			if (m == null || m.Deleted || m.NetState == null)
			{
				return;
			}

			if (ModuleEnabled || m.AccessLevel >= TrashCollection.Access)
			{
				SuperGump.Send(new TrashCollectionAdminGump(m));
			}
			else
			{
				m.SendMessage(0x22, "Trash Collection is currently out of service.");
			}
		}

		public void HandleProfilesCommand(CommandEventArgs e)
		{
			var m = e.Mobile as PlayerMobile;

			if (m == null || m.Deleted || m.NetState == null)
			{
				return;
			}

			if (ModuleEnabled || m.AccessLevel >= TrashCollection.Access)
			{
				SuperGump.Send(new TrashCollectionProfilesGump(m));
			}
			else
			{
				m.SendMessage(0x22, "Trash Collection is currently out of service.");
			}
		}

		public override string ToString()
		{
			return "Trash Options";
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.Write(AdminCommand);
					writer.Write(ProfilesCommand);
					writer.Write(DateFormat);
					writer.Write(DailyLimit);
					writer.Write(UseTrashedProps);
					writer.Write(GiveBonusTokens);
				}
				break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.GetVersion();

			switch (version)
			{
				case 0:
				{
					AdminCommand = reader.ReadString();
					ProfilesCommand = reader.ReadString();
					DateFormat = reader.ReadString();
					DailyLimit = reader.ReadInt();
					UseTrashedProps = reader.ReadBool();
					GiveBonusTokens = reader.ReadBool();
				}
				break;
			}
		}
	}
}