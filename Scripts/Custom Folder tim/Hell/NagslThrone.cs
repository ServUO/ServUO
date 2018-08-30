using System;
using Server.Items;

namespace Server.Items
{
	[Furniture]
	[FlipableAttribute(0x35ED, 0x35EE)]
	public class NagsThrone : Item
	{
		[Constructable]
		public NagsThrone() : base(0x35ED)
		{
			Name = "Nag's Throne";
                        Movable = true;
                        Hue = 1371;
			Weight = 20.0;
		}

		public NagsThrone(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			if ( Weight == 6.0 )
				Weight = 20.0;
		}
	}
}