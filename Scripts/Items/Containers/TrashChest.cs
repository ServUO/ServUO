using Server.Engines.Points;

using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
	[Flipable(0xE41, 0xE40)]
	public class TrashChest : BaseTrash
	{
		public override TextDefinition EmptyBroadcast => Utility.Random(1042891, 8);

		public override double CavernOfDiscardedChance => 0.01;

		[Constructable]
		public TrashChest()
			: base(0xE41)
		{
			Movable = false;
		}

		public TrashChest(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			reader.ReadInt();
		}
	}
}
