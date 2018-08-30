using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x8EA, 0x8E9, 0x8E4, 0x8E5, 0x8E0, 0x8E1, 0x8E7, 0x8E3 )]

	public class SnowDrift : Item
	{		
		[Constructable]
		public SnowDrift() : base( 0x8EA )//+ Utility.Random( 5 ))
		{
			Name = "A snow drift";
			Hue = 1151;
			Weight = 2.0;
			LootType = LootType.Blessed;
		}

		public SnowDrift( Serial serial ) : base( serial )
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

