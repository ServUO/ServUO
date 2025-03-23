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
	public abstract class BaseThrowableBench : ThrowableFurniture
	{
		public BaseThrowableBench(int itemID)
			: base(itemID)
		{
			Weight = 1.0;
		}

		public BaseThrowableBench(Serial serial)
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

	[Furniture, Flipable(0xB2D, 0xB2C)]
	public class ThrowableWoodenBench : BaseThrowableBench
	{
		[Constructable]
		public ThrowableWoodenBench()
			: base(Utility.RandomList(0xB2D, 0xB2C))
		{ }

		public ThrowableWoodenBench(Serial serial)
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