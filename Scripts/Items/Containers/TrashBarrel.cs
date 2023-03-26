using Server.Engines.Points;
using Server.Multis;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Items
{
	public class TrashBarrel : BaseTrash, IChopable
	{
		public override int LabelNumber => 1041064; // a trash barrel

		public override TextDefinition EmptyBroadcast => 501479; // Emptying the trashcan!

		public override double CavernOfDiscardedChance => 0.01;

		[Constructable]
		public TrashBarrel()
			: base(0xE77)
		{
			Hue = 0x3B2;
			Movable = false;
		}

		public TrashBarrel(Serial serial)
			: base(serial)
		{
		}

		public void OnChop(Mobile from)
		{
			var house = BaseHouse.FindHouseAt(from);

			if (house != null && house.IsCoOwner(from))
			{
				Effects.PlaySound(Location, Map, 0x3B3);

				from.SendLocalizedMessage(500461); // You destroy the item.

				Destroy();
			}
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
