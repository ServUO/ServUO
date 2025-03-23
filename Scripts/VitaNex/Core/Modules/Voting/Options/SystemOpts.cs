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
using System;

using Server;
using Server.Commands;
using Server.Mobiles;

using VitaNex.SuperGumps;
#endregion

namespace VitaNex.Modules.Voting
{
	public sealed class VotingOptions : CoreModuleOptions
	{
		private string _AdminCommand;
		private string _ProfilesCommand;
		private string _VoteCommand;

		[CommandProperty(Voting.Access)]
		public string AdminCommand
		{
			get => _AdminCommand;
			set => CommandUtility.Replace(_AdminCommand, Voting.Access, HandleAdminCommand, (_AdminCommand = value));
		}

		[CommandProperty(Voting.Access)]
		public string ProfilesCommand
		{
			get => _ProfilesCommand;
			set => CommandUtility.Replace(_ProfilesCommand, AccessLevel.Player, HandleProfilesCommand, (_ProfilesCommand = value));
		}

		[CommandProperty(Voting.Access)]
		public string VoteCommand
		{
			get => _VoteCommand;
			set => CommandUtility.Replace(_VoteCommand, AccessLevel.Player, HandleVoteCommand, (_VoteCommand = value));
		}

		[CommandProperty(Voting.Access)]
		public string DateFormat { get; set; }

		[CommandProperty(Voting.Access)]
		public int DailyLimit { get; set; }

		[CommandProperty(Voting.Access)]
		public bool GiveBonusTokens { get; set; }

		[CommandProperty(Voting.Access)]
		public bool RestrictByIP { get; set; }

		[CommandProperty(Voting.Access)]
		public TimeSpan BrowserDelay { get; set; }

		public VotingOptions()
			: base(typeof(Voting))
		{
			AdminCommand = "VoteConfig";
			ProfilesCommand = "VoteProfiles";
			VoteCommand = "Vote";
			DateFormat = "m/d/y";
			DailyLimit = 0;
			GiveBonusTokens = false;
			BrowserDelay = TimeSpan.FromSeconds(2.0);
		}

		public VotingOptions(GenericReader reader)
			: base(reader)
		{ }

		public void HandleAdminCommand(CommandEventArgs e)
		{
			var m = e.Mobile as PlayerMobile;

			if (m == null || m.Deleted || m.NetState == null)
			{
				return;
			}

			if (ModuleEnabled && m.AccessLevel >= Voting.Access)
			{
				SuperGump.Send(new VoteAdminGump(m));
			}
			else
			{
				m.SendMessage(0x22, "Voting is currently out of service.");
			}
		}

		public void HandleProfilesCommand(CommandEventArgs e)
		{
			var m = e.Mobile as PlayerMobile;

			if (m == null || m.Deleted || m.NetState == null)
			{
				return;
			}

			if (ModuleEnabled || m.AccessLevel >= Voting.Access)
			{
				SuperGump.Send(new VoteProfilesGump(m));
			}
			else
			{
				m.SendMessage(0x22, "Voting is currently out of service.");
			}
		}

		public void HandleVoteCommand(CommandEventArgs e)
		{
			var m = e.Mobile as PlayerMobile;

			if (m == null || m.Deleted || m.NetState == null)
			{
				return;
			}

			if (ModuleEnabled || m.AccessLevel >= Voting.Access)
			{
				SuperGump.Send(new VoteGump(m));
			}
			else
			{
				m.SendMessage(0x22, "Voting is currently out of service.");
			}
		}

		public override void Clear()
		{
			base.Clear();

			DailyLimit = 0;
			GiveBonusTokens = false;
			RestrictByIP = false;
			BrowserDelay = TimeSpan.Zero;
		}

		public override void Reset()
		{
			base.Reset();

			AdminCommand = "VoteConfig";
			ProfilesCommand = "VoteProfiles";
			VoteCommand = "Vote";

			DailyLimit = 0;
			GiveBonusTokens = false;
			RestrictByIP = false;
			BrowserDelay = TimeSpan.FromSeconds(2.0);
		}

		public override string ToString()
		{
			return "Voting Config";
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
					writer.Write(VoteCommand);
					writer.Write(DateFormat);
					writer.Write(DailyLimit);
					writer.Write(GiveBonusTokens);
					writer.Write(BrowserDelay);
					writer.Write(RestrictByIP);
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
					VoteCommand = reader.ReadString();
					DateFormat = reader.ReadString();
					DailyLimit = reader.ReadInt();
					GiveBonusTokens = reader.ReadBool();
					BrowserDelay = reader.ReadTimeSpan();
					RestrictByIP = reader.ReadBool();
				}
				break;
			}
		}
	}
}