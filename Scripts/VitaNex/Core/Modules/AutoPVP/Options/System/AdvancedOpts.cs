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
	public class AutoPvPAdvancedOptions : PropertyObject
	{
		[CommandProperty(AutoPvP.Access)]
		public virtual AutoPvPCommandOptions Commands { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual AutoPvPProfileOptions Profiles { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual AutoPvPSeasonOptions Seasons { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual AutoPvPMiscOptions Misc { get; set; }

		public AutoPvPAdvancedOptions()
		{
			Commands = new AutoPvPCommandOptions();
			Profiles = new AutoPvPProfileOptions();
			Seasons = new AutoPvPSeasonOptions();
			Misc = new AutoPvPMiscOptions();
		}

		public AutoPvPAdvancedOptions(GenericReader reader)
			: base(reader)
		{ }

		public override void Clear()
		{
			Commands.Clear();
			Profiles.Clear();
			Seasons.Clear();
			Misc.Clear();
		}

		public override void Reset()
		{
			Commands.Reset();
			Profiles.Reset();
			Seasons.Reset();
			Misc.Reset();
		}

		public override string ToString()
		{
			return "Advanced Options";
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
					writer.WriteBlock(w => w.WriteType(Misc, t => Misc.Serialize(w)));
					goto case 0;
				case 0:
				{
					writer.WriteBlock(w => w.WriteType(Commands, t => Commands.Serialize(w)));
					writer.WriteBlock(w => w.WriteType(Profiles, t => Profiles.Serialize(w)));
					writer.WriteBlock(w => w.WriteType(Seasons, t => Seasons.Serialize(w)));
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
				case 1:
					reader.ReadBlock(r => Misc = r.ReadTypeCreate<AutoPvPMiscOptions>(r) ?? new AutoPvPMiscOptions());
					goto case 0;
				case 0:
				{
					reader.ReadBlock(r => Commands = r.ReadTypeCreate<AutoPvPCommandOptions>(r) ?? new AutoPvPCommandOptions());
					reader.ReadBlock(r => Profiles = r.ReadTypeCreate<AutoPvPProfileOptions>(r) ?? new AutoPvPProfileOptions());
					reader.ReadBlock(r => Seasons = r.ReadTypeCreate<AutoPvPSeasonOptions>(r) ?? new AutoPvPSeasonOptions());
				}
				break;
			}

			if (Misc == null)
			{
				Misc = new AutoPvPMiscOptions();
			}
		}
	}
}