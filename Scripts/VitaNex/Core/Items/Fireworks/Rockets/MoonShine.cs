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
	public class MoonShineRocket : BaseFireworkRocket
	{
		public override FireworkStars DefStarsEffect => FireworkStars.Ring;

		[Constructable]
		public MoonShineRocket()
			: this(Utility.RandomMetalHue())
		{ }

		[Constructable]
		public MoonShineRocket(int hue)
			: base(18881, hue)
		{
			Name = "Moon Shine";
			Weight = 4.0;
		}

		public MoonShineRocket(Serial serial)
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