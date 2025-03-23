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
	[Flipable(0x4910, 0x4911)]
	public class HugeWoodenChest : CustomContainer
	{
		public override int DefaultContainerItemID => 0x4910;

		[Constructable]
		public HugeWoodenChest()
			: base(0x4910, 0x4910)
		{ }

		public HugeWoodenChest(Serial serial)
			: base(serial)
		{ }

		public override void UpdateContainerData()
		{
			if (ItemID == 0x4910 || ItemID == 0x4911)
			{
				ContainerData = new ContainerData(0x3E8, new Rectangle2D(90, 90, 460, 280), 0x42);
				return;
			}

			base.UpdateContainerData();
		}

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