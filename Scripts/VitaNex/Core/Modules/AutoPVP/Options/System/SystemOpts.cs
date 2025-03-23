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
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class AutoPvPOptions : CoreModuleOptions
	{
		[CommandProperty(AutoPvP.Access)]
		public virtual AutoPvPAdvancedOptions Advanced { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual AutoPvPStatistics Statistics { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual AutoPvPExecuteCommands ExecuteCommands { get; set; }

		public AutoPvPOptions()
			: base(typeof(AutoPvP))
		{
			Advanced = new AutoPvPAdvancedOptions();
			Statistics = new AutoPvPStatistics();
			ExecuteCommands = new AutoPvPExecuteCommands();
		}

		public AutoPvPOptions(GenericReader reader)
			: base(reader)
		{ }

		public override string ToString()
		{
			return "System Options";
		}

		public override void Clear()
		{
			Advanced.Clear();
			Statistics.Clear();
			ExecuteCommands.Clear();
		}

		public override void Reset()
		{
			Advanced.Reset();
			Statistics.Reset();
			ExecuteCommands.Reset();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.WriteBlock(w => w.WriteType(Advanced, t => Advanced.Serialize(w)));
					writer.WriteBlock(w => w.WriteType(Statistics, t => Statistics.Serialize(w)));
					writer.WriteBlock(w => w.WriteType(ExecuteCommands, t => ExecuteCommands.Serialize(w)));
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
					reader.ReadBlock(r => Advanced = r.ReadTypeCreate<AutoPvPAdvancedOptions>(r) ?? new AutoPvPAdvancedOptions());
					reader.ReadBlock(r => Statistics = r.ReadTypeCreate<AutoPvPStatistics>(r) ?? new AutoPvPStatistics());
					reader.ReadBlock(
						r => ExecuteCommands = r.ReadTypeCreate<AutoPvPExecuteCommands>(r) ?? new AutoPvPExecuteCommands());
				}
				break;
			}
		}
	}
}