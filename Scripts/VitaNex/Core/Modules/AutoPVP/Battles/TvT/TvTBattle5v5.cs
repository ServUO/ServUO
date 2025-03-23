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
#endregion

namespace VitaNex.Modules.AutoPvP.Battles
{
	public class TvTBattle5v5 : TvTBattle
	{
		private static readonly TimeSpan[] _Times =
		{
			new TimeSpan(1, 30, 0), new TimeSpan(3, 30, 0), new TimeSpan(5, 30, 0), new TimeSpan(7, 30, 0),
			new TimeSpan(9, 30, 0), new TimeSpan(11, 30, 0), new TimeSpan(14, 30, 0), new TimeSpan(16, 30, 0),
			new TimeSpan(18, 30, 0), new TimeSpan(20, 30, 0)
		};

		public TvTBattle5v5()
		{
			Name = "5 vs 5";

			Teams[0].MinCapacity = 1;
			Teams[0].MaxCapacity = 5;

			Teams[1].MinCapacity = 1;
			Teams[1].MaxCapacity = 5;

			Schedule.Info.Times.Clear();
			Schedule.Info.Times.Add(_Times);

			Options.Broadcasts.World.MessageHue = 891;

			Options.Timing.PreparePeriod = TimeSpan.FromMinutes(3.0);
			Options.Timing.RunningPeriod = TimeSpan.FromMinutes(12.0);
			Options.Timing.EndedPeriod = TimeSpan.FromMinutes(10.0);
		}

		public TvTBattle5v5(GenericReader reader)
			: base(reader)
		{ }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
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
					break;
			}
		}
	}
}