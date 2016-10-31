using System;
using Server;

namespace Server.Items
{
	public class BootsOfTheThrasher : Boots
	{
		public override int LabelNumber { get { return 1151210; } } // Snake Skin Boots

		[Constructable]
		public BootsOfTheThrasher()
		{
			Hue = 0x497;
			Resistances.Physical = 2;
		}

		public BootsOfTheThrasher( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}