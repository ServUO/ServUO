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
using System.Linq;

using Server;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;

using VitaNex.SuperGumps;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class AutoPvPCommandOptions : PropertyObject
	{
		private string _BattlesCommand;
		private string _ConfigCommand;
		private string _ProfilesCommand;

		[CommandProperty(AutoPvP.Access)]
		public virtual string ConfigCommand
		{
			get => _ConfigCommand;
			set => CommandUtility.Replace(_ConfigCommand, AutoPvP.Access, OnConfigCommand, (_ConfigCommand = value));
		}

		[CommandProperty(AutoPvP.Access)]
		public virtual string ProfilesCommand
		{
			get => _ProfilesCommand;
			set => CommandUtility.Replace(_ProfilesCommand, AccessLevel.Player, OnProfilesCommand, (_ProfilesCommand = value));
		}

		[CommandProperty(AutoPvP.Access)]
		public virtual string BattlesCommand
		{
			get => _BattlesCommand;
			set => CommandUtility.Replace(_BattlesCommand, AccessLevel.Player, OnBattlesCommand, (_BattlesCommand = value));
		}

		public AutoPvPCommandOptions()
		{
			ConfigCommand = "PvPConfig";
			ProfilesCommand = "PvPRanks";
			BattlesCommand = "Battles";
		}

		public AutoPvPCommandOptions(GenericReader reader)
			: base(reader)
		{ }

		public override string ToString()
		{
			return "Command Options";
		}

		public override void Clear()
		{
			ConfigCommand = null;
			ProfilesCommand = null;
			BattlesCommand = null;
		}

		public override void Reset()
		{
			ConfigCommand = "PvPConfig";
			ProfilesCommand = "PvPRanks";
			BattlesCommand = "Battles";
		}

		public virtual void InvokeConfigCommand(Mobile m)
		{
			if (m != null)
			{
				CommandSystem.Handle(m, CommandSystem.Prefix + ConfigCommand);
			}
		}

		public virtual void InvokeBattlesCommand(Mobile m)
		{
			if (m != null)
			{
				CommandSystem.Handle(m, CommandSystem.Prefix + BattlesCommand);
			}
		}

		public virtual void InvokeProfilesCommand(Mobile m)
		{
			if (m != null)
			{
				CommandSystem.Handle(m, CommandSystem.Prefix + ProfilesCommand);
			}
		}

		protected virtual void OnConfigCommand(CommandEventArgs e)
		{
			if (e.Mobile == null || e.Mobile.Deleted || !(e.Mobile is PlayerMobile))
			{
				return;
			}

			if (e.Mobile.AccessLevel >= AutoPvP.Access)
			{
				e.Mobile.SendGump(new PropertiesGump(e.Mobile, AutoPvP.CMOptions));
			}
		}

		protected virtual void OnProfilesCommand(CommandEventArgs e)
		{
			if (e.Mobile == null || e.Mobile.Deleted || !(e.Mobile is PlayerMobile))
			{
				return;
			}

			if (AutoPvP.CMOptions.ModuleEnabled || e.Mobile.AccessLevel >= AutoPvP.Access)
			{
				SuperGump.Send(new PvPProfilesUI(e.Mobile, null));
			}
		}

		protected virtual void OnBattlesCommand(CommandEventArgs e)
		{
			if (e.Mobile == null || e.Mobile.Deleted || !(e.Mobile is PlayerMobile))
			{
				return;
			}

			if (AutoPvP.CMOptions.ModuleEnabled || e.Mobile.AccessLevel >= AutoPvP.Access)
			{
				PvPBattle battle = null;

				if (e.Arguments != null && e.Arguments.Length > 0)
				{
					var shortcut = e.Arguments[0];

					if (!System.String.IsNullOrWhiteSpace(shortcut))
					{
						battle = AutoPvP.Battles.Values.FirstOrDefault(b => b != null && !b.Deleted && Insensitive.Equals(b.Shortcut, shortcut));
					}
				}

				PvPBattlesUI.DisplayTo(e.Mobile, battle, false);
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.Write(ConfigCommand);
					writer.Write(ProfilesCommand);
					writer.Write(BattlesCommand);
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
					ConfigCommand = reader.ReadString();
					ProfilesCommand = reader.ReadString();
					BattlesCommand = reader.ReadString();
				}
				break;
			}
		}
	}
}