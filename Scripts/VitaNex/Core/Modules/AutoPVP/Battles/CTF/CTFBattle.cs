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
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Mobiles;

using VitaNex.Schedules;
#endregion

namespace VitaNex.Modules.AutoPvP.Battles
{
	public class CTFBattle : PvPBattle
	{
		[CommandProperty(AutoPvP.Access)]
		public virtual double FlagDamageInc { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual double FlagDamageIncMax { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int FlagCapturePoints { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int FlagReturnPoints { get; set; }

		public CTFBattle()
		{
			Name = "Capture The Flag";
			Category = "Capture The Flag";
			Description = "Capture the enemy flag and return it to your podium to score points!" +
						  "\nDefend your flag from the enemy, you can only capture their flag when your flag is on your podium.";

			AddTeam(NameList.RandomName("daemon"), 1, 5, 0x22);
			AddTeam(NameList.RandomName("daemon"), 1, 5, 0x55);

			Options.Missions.Enabled = true;
			Options.Missions.Team = new CTFBattleObjectives
			{
				FlagsCaptured = 5
			};

			Schedule.Info.Months = ScheduleMonths.All;
			Schedule.Info.Days = ScheduleDays.All;
			Schedule.Info.Times = ScheduleTimes.EveryQuarterHour;

			Options.Timing.QueuePeriod = TimeSpan.FromMinutes(5.0);
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
			Options.Rules.CanDamageOwnTeam = false;
			Options.Rules.CanDie = false;
			Options.Rules.CanHeal = true;
			Options.Rules.CanHealEnemyTeam = false;
			Options.Rules.CanHealOwnTeam = true;
			Options.Rules.CanMount = false;
			Options.Rules.CanFly = false;
			Options.Rules.CanResurrect = true;
			Options.Rules.CanUseStuckMenu = false;
			Options.Rules.CanEquip = true;
		}

		public CTFBattle(GenericReader reader)
			: base(reader)
		{ }

		public override bool Validate(Mobile viewer, List<string> errors, bool pop = true)
		{
			if (!base.Validate(viewer, errors, pop) && pop)
			{
				return false;
			}

			if (!Teams.All(t => t is CTFTeam))
			{
				errors.Add("One or more teams are not of the CTFTeam type.");
				errors.Add("[Options] -> [View Teams]");

				if (pop)
				{
					return false;
				}
			}

			return true;
		}

		public override bool AddTeam(string name, int minCapacity, int capacity, int color)
		{
			return AddTeam(new CTFTeam(this, name, minCapacity, capacity, color));
		}

		public override bool AddTeam(PvPTeam team)
		{
			if (team == null || team.Deleted)
			{
				return false;
			}

			if (team is CTFTeam)
			{
				return base.AddTeam(team);
			}

			var added = AddTeam(team.Name, team.MinCapacity, team.MinCapacity, team.Color);

			team.Delete();

			return added;
		}

		public virtual void OnFlagDropped(CTFFlag flag, PlayerMobile attacker, CTFTeam enemyTeam)
		{
			UpdateStatistics(enemyTeam, attacker, s => ++s["Flags Dropped"]);

			PlaySound(746);

			LocalBroadcast("[{0}]: {1} has dropped the flag of {2}!", enemyTeam.Name, attacker.Name, flag.Team.Name);
		}

		public virtual void OnFlagCaptured(CTFFlag flag, PlayerMobile attacker, CTFTeam enemyTeam)
		{
			UpdateStatistics(enemyTeam, attacker, s => ++s["Flags Captured"]);

			if (FlagCapturePoints > 0)
			{
				AwardPoints(attacker, FlagCapturePoints);
			}

			PlaySound(747);

			LocalBroadcast("[{0}]: {1} has captured the flag of {2}!", enemyTeam.Name, attacker.Name, flag.Team.Name);
		}

		public virtual void OnFlagStolen(CTFFlag flag, PlayerMobile attacker, CTFTeam enemyTeam)
		{
			UpdateStatistics(enemyTeam, attacker, s => ++s["Flags Stolen"]);

			PlaySound(748);

			LocalBroadcast("[{0}]: {1} has stolen the flag of {2}!", enemyTeam.Name, attacker.Name, flag.Team.Name);
		}

		public virtual void OnFlagReturned(CTFFlag flag, PlayerMobile defender)
		{
			UpdateStatistics(flag.Team, defender, s => ++s["Flags Returned"]);

			if (FlagReturnPoints > 0)
			{
				AwardPoints(defender, FlagReturnPoints);
			}

			PlaySound(749);

			LocalBroadcast("[{0}]: {1} has returned the flag of {0}!", flag.Team.Name, defender.Name);
		}

		public virtual void OnFlagTimeout(CTFFlag flag)
		{
			PlaySound(749);

			LocalBroadcast("[{0}]: Flag has been returned to the base!", flag.Team.Name);
		}

		public override bool CheckDamage(Mobile damaged, ref int damage)
		{
			if (!base.CheckDamage(damaged, ref damage))
			{
				return false;
			}

			if (damaged != null && damaged.Player && damaged.Backpack != null && damage > 0)
			{
				var flag = damaged.Backpack.FindItemByType<CTFFlag>();

				if (flag != null)
				{
					damage += (int)(damage * flag.DamageInc);
				}
			}

			return true;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(3);

			switch (version)
			{
				case 3:
				case 2:
				{
					writer.Write(FlagDamageInc);
					writer.Write(FlagDamageIncMax);
				}
				goto case 1;
				case 1:
				{
					writer.Write(FlagCapturePoints);
					writer.Write(FlagReturnPoints);
				}
				goto case 0;
				case 0:
					writer.Write(-1); // CapsToWin
					break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			if (!(Options.Missions.Team is CTFBattleObjectives))
			{
				Options.Missions.Enabled = true;

				Options.Missions.Team = new CTFBattleObjectives
				{
					FlagsCaptured = 5
				};
			}

			var version = reader.ReadInt();

			switch (version)
			{
				case 3:
				case 2:
				{
					FlagDamageInc = reader.ReadDouble();
					FlagDamageIncMax = reader.ReadDouble();
				}
				goto case 1;
				case 1:
				{
					FlagCapturePoints = reader.ReadInt();
					FlagReturnPoints = reader.ReadInt();
				}
				goto case 0;
				case 0:
				{
					var capsToWin = reader.ReadInt();

					if (capsToWin >= 0)
					{
						((CTFBattleObjectives)Options.Missions.Team).FlagsCaptured = capsToWin;
					}
				}
				break;
			}

			if (version < 3)
			{
				RewardTeam = true;
			}
		}
	}
}