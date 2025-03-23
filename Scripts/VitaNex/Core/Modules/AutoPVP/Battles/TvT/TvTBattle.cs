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
	public class TvTBattle : PvPBattle
	{
		public TvTBattle()
		{
			Name = "Team vs Team";
			Category = "Team vs Team";
			Description = "The last team alive wins!";

			Ranked = true;
			RewardTeam = true;

			AddTeam(NameList.RandomName("daemon"), 1, 1, 0x22);
			AddTeam(NameList.RandomName("daemon"), 1, 1, 0x55);

			Schedule.Info.Months = ScheduleMonths.All;
			Schedule.Info.Days = ScheduleDays.All;
			Schedule.Info.Times = ScheduleTimes.EveryQuarterHour;

			Options.Timing.PreparePeriod = TimeSpan.FromMinutes(2.0);
			Options.Timing.RunningPeriod = TimeSpan.FromMinutes(8.0);
			Options.Timing.EndedPeriod = TimeSpan.FromMinutes(1.0);

			Options.Rules.AllowBeneficial = true;
			Options.Rules.AllowHarmful = true;
			Options.Rules.AllowHousing = false;
			Options.Rules.AllowPets = false;
			Options.Rules.AllowSpawn = false;
			Options.Rules.AllowSpeech = true;
			Options.Rules.CanBeDamaged = true;
			Options.Rules.CanDamageEnemyTeam = true;
			Options.Rules.CanDamageOwnTeam = false;
			Options.Rules.CanDie = false;
			Options.Rules.CanHeal = true;
			Options.Rules.CanHealEnemyTeam = false;
			Options.Rules.CanHealOwnTeam = true;
			Options.Rules.CanMount = false;
			Options.Rules.CanFly = false;
			Options.Rules.CanResurrect = false;
			Options.Rules.CanUseStuckMenu = false;
			Options.Rules.CanEquip = true;
		}

		public TvTBattle(GenericReader reader)
			: base(reader)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.SetVersion(1);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var v = reader.GetVersion();

			if (v < 1)
			{
				RewardTeam = true;
			}
		}
	}
}