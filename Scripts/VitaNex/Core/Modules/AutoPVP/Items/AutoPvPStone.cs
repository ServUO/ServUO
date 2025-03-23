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
using System.Drawing;

using Server;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public enum AutoPvPStoneCommand
	{
		ViewBattles,
		ViewProfiles,
		GlobalConfig
	}

	public class AutoPvPStone : Item
	{
		[CommandProperty(AutoPvP.Access)]
		public AutoPvPStoneCommand Command { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public KnownColor UsageColor { get; set; }

		public override bool DisplayLootType => false;
		public override bool DisplayWeight => false;

		[Constructable]
		public AutoPvPStone()
			: this(4963)
		{ }

		[Constructable]
		public AutoPvPStone(int itemID)
			: base(itemID)
		{
			Command = AutoPvPStoneCommand.ViewBattles;
			UsageColor = KnownColor.SkyBlue;

			Name = "PvP Battle Stone";
			LootType = LootType.Blessed;
			Weight = 0;
		}

		public AutoPvPStone(Serial serial)
			: base(serial)
		{ }

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			var color = Color.FromKnownColor(UsageColor).ToRgb();

			switch (Command)
			{
				case AutoPvPStoneCommand.ViewBattles:
				{
					list.Add("<basefont color=#{0:X6}>Opens the PvP battles menu<basefont color=#ffffff>", color);
				}
				break;
				case AutoPvPStoneCommand.ViewProfiles:
				{
					list.Add("<basefont color=#{0:X6}>Opens the PvP profiles menu<basefont color=#ffffff>", color);
				}
				break;
				case AutoPvPStoneCommand.GlobalConfig:
				{
					list.Add("<basefont color=#{0:X6}>Use: Opens the PvP control panel<basefont color=#ffffff>", color);
				}
				break;
			}
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (from == null || from.Deleted)
			{
				return;
			}

			switch (Command)
			{
				case AutoPvPStoneCommand.ViewBattles:
				{
					AutoPvP.CMOptions.Advanced.Commands.InvokeBattlesCommand(from);
				}
				break;
				case AutoPvPStoneCommand.ViewProfiles:
				{
					AutoPvP.CMOptions.Advanced.Commands.InvokeProfilesCommand(from);
				}
				break;
				case AutoPvPStoneCommand.GlobalConfig:
				{
					AutoPvP.CMOptions.Advanced.Commands.InvokeConfigCommand(from);
				}
				break;
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
					writer.WriteFlag(Command);
					writer.WriteFlag(UsageColor);
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
					Command = reader.ReadFlag<AutoPvPStoneCommand>();
					UsageColor = reader.ReadFlag<KnownColor>();
				}
				break;
			}
		}
	}
}