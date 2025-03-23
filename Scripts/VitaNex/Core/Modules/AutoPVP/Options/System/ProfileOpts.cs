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
	public class AutoPvPProfileOptions : PropertyObject
	{
		[CommandProperty(AutoPvP.Access)]
		public virtual bool AllowPlayerSearch { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual bool AllowPlayerDelete { get; set; }

		[CommandProperty(AutoPvP.Access)]
		public virtual PvPProfileRankOrder RankingOrder { get; set; }

		public AutoPvPProfileOptions()
		{
			AllowPlayerSearch = true;
			RankingOrder = PvPProfileRankOrder.Points;
		}

		public AutoPvPProfileOptions(GenericReader reader)
			: base(reader)
		{ }

		public override string ToString()
		{
			return "Profile Options";
		}

		public override void Clear()
		{
			AllowPlayerSearch = false;
			AllowPlayerDelete = false;
			RankingOrder = PvPProfileRankOrder.None;
		}

		public override void Reset()
		{
			AllowPlayerSearch = true;
			AllowPlayerDelete = false;
			RankingOrder = PvPProfileRankOrder.Points;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			var version = writer.SetVersion(0);

			switch (version)
			{
				case 0:
				{
					writer.Write(AllowPlayerSearch);
					writer.Write(AllowPlayerDelete);
					writer.WriteFlag(RankingOrder);
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
					AllowPlayerSearch = reader.ReadBool();
					AllowPlayerDelete = reader.ReadBool();
					RankingOrder = reader.ReadFlag<PvPProfileRankOrder>();
				}
				break;
			}
		}
	}
}