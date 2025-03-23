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
	public abstract class BaseThrowableChair : ThrowableFurniture
	{
		public BaseThrowableChair(int itemID)
			: base(itemID)
		{
			Weight = 20.0;
		}

		public BaseThrowableChair(Serial serial)
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

	[Furniture, Flipable(0xB4F, 0xB4E, 0xB50, 0xB51)]
	public abstract class ThrowableFancyWoodenChairCushion : BaseThrowableChair
	{
		public ThrowableFancyWoodenChairCushion()
			: base(Utility.RandomList(0xB4F, 0xB4E, 0xB50, 0xB51))
		{ }

		public ThrowableFancyWoodenChairCushion(Serial serial)
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

	[Furniture, Flipable(0xB53, 0xB52, 0xB54, 0xB55)]
	public abstract class ThrowableWoodenChairCushion : BaseThrowableChair
	{
		public ThrowableWoodenChairCushion()
			: base(Utility.RandomList(0xB53, 0xB52, 0xB54, 0xB55))
		{ }

		public ThrowableWoodenChairCushion(Serial serial)
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

	[Furniture, Flipable(0xB57, 0xB56, 0xB59, 0xB58)]
	public class ThrowableWoodenChair : BaseThrowableChair
	{
		[Constructable]
		public ThrowableWoodenChair()
			: base(Utility.RandomList(0xB57, 0xB56, 0xB59, 0xB58))
		{ }

		public ThrowableWoodenChair(Serial serial)
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

	[Furniture, Flipable(0xB5B, 0xB5A, 0xB5C, 0xB5D)]
	public class ThrowableBambooChair : BaseThrowableChair
	{
		[Constructable]
		public ThrowableBambooChair()
			: base(Utility.RandomList(0xB5B, 0xB5A, 0xB5C, 0xB5D))
		{ }

		public ThrowableBambooChair(Serial serial)
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

	[Furniture, Flipable(0x1218, 0x1219, 0x121A, 0x121B)]
	public class ThrowableStoneChair : BaseThrowableChair
	{
		[Constructable]
		public ThrowableStoneChair()
			: base(Utility.RandomList(0x1218, 0x1219, 0x121A, 0x121B))
		{ }

		public ThrowableStoneChair(Serial serial)
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

	[Furniture, Flipable(0x2DE3, 0x2DE4, 0x2DE5, 0x2DE6)]
	public class ThrowableOrnateElvenChair : BaseThrowableChair
	{
		[Constructable]
		public ThrowableOrnateElvenChair()
			: base(Utility.RandomList(0x2DE3, 0x2DE4, 0x2DE5, 0x2DE6))
		{ }

		public ThrowableOrnateElvenChair(Serial serial)
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

	[Furniture, Flipable(0x2DEB, 0x2DEC, 0x2DED, 0x2DEE)]
	public class ThrowableBigElvenChair : BaseThrowableChair
	{
		[Constructable]
		public ThrowableBigElvenChair()
			: base(Utility.RandomList(0x2DEB, 0x2DEC, 0x2DED, 0x2DEE))
		{ }

		public ThrowableBigElvenChair(Serial serial)
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

	[Furniture, Flipable(0x2DF5, 0x2DF6)]
	public class ThrowableElvenReadingChair : BaseThrowableChair
	{
		[Constructable]
		public ThrowableElvenReadingChair()
			: base(Utility.RandomList(0x2DF5, 0x2DF6))
		{ }

		public ThrowableElvenReadingChair(Serial serial)
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