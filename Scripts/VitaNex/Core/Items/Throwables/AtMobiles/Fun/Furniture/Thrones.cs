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
	public abstract class BaseThrowableThrone : ThrowableFurniture
	{
		public BaseThrowableThrone(int itemID)
			: base(itemID)
		{
			Weight = 10.0;
		}

		public BaseThrowableThrone(Serial serial)
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

	[Furniture, Flipable(0xB32, 0xB33)]
	public class ThrowableThrone : BaseThrowableThrone
	{
		[Constructable]
		public ThrowableThrone()
			: base(Utility.RandomList(0xB32, 0xB33))
		{ }

		public ThrowableThrone(Serial serial)
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

	[Furniture, Flipable(0xB2E, 0xB2F, 0xB31, 0xB30)]
	public class ThrowableWoodenThrone : BaseThrowableThrone
	{
		[Constructable]
		public ThrowableWoodenThrone()
			: base(Utility.RandomList(0xB2E, 0xB2F, 0xB31, 0xB30))
		{ }

		public ThrowableWoodenThrone(Serial serial)
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