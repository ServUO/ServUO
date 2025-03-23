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
	public class TvTBattle2v2 : TvTBattle
	{
		private static readonly TimeSpan[] _Times =
		{
			new TimeSpan(0, 30, 0), new TimeSpan(2, 30, 0), new TimeSpan(4, 30, 0), new TimeSpan(6, 30, 0),
			new TimeSpan(8, 30, 0), new TimeSpan(10, 30, 0), new TimeSpan(13, 30, 0), new TimeSpan(15, 30, 0),
			new TimeSpan(17, 30, 0), new TimeSpan(19, 30, 0), new TimeSpan(21, 30, 0), new TimeSpan(23, 45, 0)
		};

		public TvTBattle2v2()
		{
			Name = "2 vs 2";

			Teams[0].MinCapacity = 1;
			Teams[0].MaxCapacity = 2;

			Teams[1].MinCapacity = 1;
			Teams[1].MaxCapacity = 2;

			Schedule.Info.Times.Clear();
			Schedule.Info.Times.Add(_Times);

			Options.Timing.PreparePeriod = TimeSpan.FromMinutes(4.0);
			Options.Timing.RunningPeriod = TimeSpan.FromMinutes(8.0);
			Options.Timing.EndedPeriod = TimeSpan.FromMinutes(10.0);
		}

		public TvTBattle2v2(GenericReader reader)
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