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
	public class PvPRewards : PropertyObject
	{
		[CommandProperty(AutoPvP.Access)]
		public virtual PvPReward Loser { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPReward Winner { get; set; }

		public PvPRewards()
		{
			Loser = new PvPReward();
			Winner = new PvPReward();
		}

		public PvPRewards(GenericReader reader)
			: base(reader)
		{ }

		public override string ToString()
		{
			return "Rewards";
		}

		public override void Clear()
		{
			Loser.Clear();
			Winner.Clear();
		}

		public override void Reset()
		{
			Loser.Reset();
			Winner.Reset();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.WriteBlock(w => w.WriteType(Loser, t => Loser.Serialize(w)));
					writer.WriteBlock(w => w.WriteType(Winner, t => Winner.Serialize(w)));
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
					reader.ReadBlock(r => Loser = r.ReadTypeCreate<PvPReward>(r) ?? new PvPReward());
					reader.ReadBlock(r => Winner = r.ReadTypeCreate<PvPReward>(r) ?? new PvPReward());
				}
				break;
			}
		}
	}
}