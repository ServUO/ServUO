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

namespace VitaNex.Modules.AutoPvP.Battles
{
	public sealed class PvPCustomBattle : PvPBattle
	{
		public PvPCustomBattle()
		{
			Name = "Generic Battle";
			Description =
				"This generic battle serves as a template to create standard battles with general rules, using no advanced features.";

			AddTeam("Team Alpha", 5, 10, 11);
			AddTeam("Team Bravo", 5, 10, 22);
			AddTeam("Team Gamma", 5, 10, 33);
			AddTeam("Team Omega", 5, 10, 44);
		}

		public PvPCustomBattle(GenericReader reader)
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