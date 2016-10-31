using System;
using Server;

namespace Server.Items
{
	public class BootsOfTheIceWyrm : Boots
	{
		public override int LabelNumber { get { return 1151208; } } // Boots of the Ice Wyrm

		[Constructable]
		public BootsOfTheIceWyrm()
		{
			Hue = 0x482;
			Resistances.Cold = 2;
		}

		public BootsOfTheIceWyrm( Serial serial )
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