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

using VitaNex.Schedules;
#endregion

namespace VitaNex.Modules.AutoPvP
{
	public class AutoPvPSeasonOptions : PropertyObject
	{
		[CommandProperty(AutoPvP.Access)]
		public ScheduleInfo ScheduleInfo
		{
			get => AutoPvP.SeasonSchedule.Info;
			set => AutoPvP.SeasonSchedule.Info = value;
		}

		[CommandProperty(AutoPvP.Access)]
		public virtual int CurrentSeason { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int TopListCount { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int RunnersUpCount { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPRewards Rewards { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int SkipTicks { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual int SkippedTicks { get; set; }

		public AutoPvPSeasonOptions()
		{
			CurrentSeason = 1;
			TopListCount = 3;
			RunnersUpCount = 7;

			Rewards = new PvPRewards();
		}

		public AutoPvPSeasonOptions(GenericReader reader)
			: base(reader)
		{ }

		public override string ToString()
		{
			return "Season Options";
		}

		public override void Clear()
		{
			ScheduleInfo.Clear();

			TopListCount = 0;
			RunnersUpCount = 0;
			SkipTicks = 0;
			SkippedTicks = 0;

			Rewards.Clear();
		}

		public override void Reset()
		{
			ScheduleInfo.Clear();

			TopListCount = 3;
			RunnersUpCount = 7;
			SkipTicks = 0;
			SkippedTicks = 0;

			Rewards.Reset();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(1);

			switch (version)
			{
				case 1:
				{
					writer.Write(SkipTicks);
					writer.Write(SkippedTicks);
				}
				goto case 0;
				case 0:
				{
					writer.WriteBlock(
						w =>
						{
							w.Write(CurrentSeason);
							w.Write(TopListCount);
							w.Write(RunnersUpCount);

							w.WriteType(ScheduleInfo, t => ScheduleInfo.Serialize(w));

							w.Write(AutoPvP.SeasonSchedule.Enabled);
						});

					writer.WriteBlock(w => w.WriteType(Rewards, t => Rewards.Serialize(w)));
				}
				break;
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			var version = reader.GetVersion();

			var scheduled = AutoPvP.SeasonSchedule.Enabled;

			switch (version)
			{
				case 1:
				{
					SkipTicks = reader.ReadInt();
					SkippedTicks = reader.ReadInt();
				}
				goto case 0;
				case 0:
				{
					reader.ReadBlock(
						r =>
						{
							CurrentSeason = r.ReadInt();
							TopListCount = r.ReadInt();
							RunnersUpCount = r.ReadInt();

							ScheduleInfo = r.ReadTypeCreate<ScheduleInfo>(r) ?? new ScheduleInfo();

							scheduled = r.ReadBool();
						});

					reader.ReadBlock(r => Rewards = r.ReadTypeCreate<PvPRewards>(r) ?? new PvPRewards());
				}
				break;
			}

			AutoPvP.SeasonSchedule.Enabled = scheduled;
		}
	}
}