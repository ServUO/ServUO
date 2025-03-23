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

namespace VitaNex.Items
{
	public class LittleBoyRocket : BaseFireworkRocket
	{
		public override FireworkStars DefStarsEffect => FireworkStars.Dahlia;

		[Constructable]
		public LittleBoyRocket()
			: this(Utility.RandomMetalHue())
		{ }

		[Constructable]
		public LittleBoyRocket(int hue)
			: base(2581, hue)
		{
			Name = "Little Boy";
			Weight = 2.0;
		}

		public LittleBoyRocket(Serial serial)
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