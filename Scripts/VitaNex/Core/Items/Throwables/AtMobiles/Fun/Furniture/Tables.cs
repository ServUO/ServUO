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
	public abstract class BaseThrowableTable : ThrowableFurniture
	{
		public BaseThrowableTable(int itemID)
			: base(itemID)
		{
			Weight = 10.0;
		}

		public BaseThrowableTable(Serial serial)
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

	[Furniture]
	public class ThrowableElegantLowTable : BaseThrowableTable
	{
		[Constructable]
		public ThrowableElegantLowTable()
			: base(0x2819)
		{ }

		public ThrowableElegantLowTable(Serial serial)
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

	[Furniture]
	public class ThrowablePlainLowTable : BaseThrowableTable
	{
		[Constructable]
		public ThrowablePlainLowTable()
			: base(0x281A)
		{ }

		public ThrowablePlainLowTable(Serial serial)
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

	[Furniture, Flipable(0xB90, 0xB7D)]
	public class ThrowableLargeTable : BaseThrowableTable
	{
		[Constructable]
		public ThrowableLargeTable()
			: base(Utility.RandomList(0xB90, 0xB7D))
		{ }

		public ThrowableLargeTable(Serial serial)
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

	[Furniture, Flipable(0xB35, 0xB34)]
	public class ThrowableNightstand : BaseThrowableTable
	{
		[Constructable]
		public ThrowableNightstand()
			: base(Utility.RandomList(0xB35, 0xB34))
		{ }

		public ThrowableNightstand(Serial serial)
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

	[Furniture, Flipable(0xB8F, 0xB7C)]
	public class ThrowableYewWoodTable : BaseThrowableTable
	{
		[Constructable]
		public ThrowableYewWoodTable()
			: base(Utility.RandomList(0xB8F, 0xB7C))
		{ }

		public ThrowableYewWoodTable(Serial serial)
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