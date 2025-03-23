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
using Server.Items;
#endregion

namespace VitaNex.Items
{
	public class FireworkStarBronze : BaseFireworkStar
	{
		[Constructable]
		public FireworkStarBronze()
			: this(1)
		{ }

		[Constructable]
		public FireworkStarBronze(int amount)
			: base(CraftResource.Bronze, amount)
		{ }

		public FireworkStarBronze(Serial serial)
			: base(serial)
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