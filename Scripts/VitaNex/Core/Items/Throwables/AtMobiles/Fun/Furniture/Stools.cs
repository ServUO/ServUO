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
	[Furniture]
	public abstract class BaseThrowableStool : ThrowableFurniture
	{
		public BaseThrowableStool(int itemID)
			: base(itemID)
		{
			Weight = 10.0;
		}

		public BaseThrowableStool(Serial serial)
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
	public class ThrowableStool : BaseThrowableStool
	{
		[Constructable]
		public ThrowableStool()
			: base(0xA2A)
		{ }

		public ThrowableStool(Serial serial)
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
	public class ThrowableFootStool : BaseThrowableStool
	{
		[Constructable]
		public ThrowableFootStool()
			: base(0xB5E)
		{ }

		public ThrowableFootStool(Serial serial)
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