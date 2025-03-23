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

using VitaNex.Schedules;
#endregion

namespace VitaNex.Modules.AutoPvP.Battles
{
	public class FFABattle : PvPBattle
	{
		public FFABattle()
		{
			Name = "Free For All";
			Category = "Free For All";
			Description = "The last participant alive wins!";

			AddTeam(NameList.RandomName("daemon"), 5, 40, 85);

			Schedule.Info.Months = ScheduleMonths.All;
			Schedule.Info.Days = ScheduleDays.All;
			Schedule.Info.Times = ScheduleTimes.EveryHour;

			Options.Timing.PreparePeriod = TimeSpan.FromMinutes(5.0);
			Options.Timing.RunningPeriod = TimeSpan.FromMinutes(15.0);
			Options.Timing.EndedPeriod = TimeSpan.FromMinutes(5.0);

			Options.Rules.AllowBeneficial = true;
			Options.Rules.AllowHarmful = true;
			Options.Rules.AllowHousing = false;
			Options.Rules.AllowPets = false;
			Options.Rules.AllowSpawn = false;
			Options.Rules.AllowSpeech = true;
			Options.Rules.CanBeDamaged = true;
			Options.Rules.CanDamageEnemyTeam = true;
			Options.Rules.CanDamageOwnTeam = true;
			Options.Rules.CanDie = false;
			Options.Rules.CanHeal = true;
			Options.Rules.CanHealEnemyTeam = false;
			Options.Rules.CanHealOwnTeam = false;
			Options.Rules.CanMount = false;
			Options.Rules.CanFly = false;
			Options.Rules.CanResurrect = false;
			Options.Rules.CanUseStuckMenu = false;
		}

		public FFABattle(GenericReader reader)
			: base(reader)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.GetVersion();
		}
	}
}