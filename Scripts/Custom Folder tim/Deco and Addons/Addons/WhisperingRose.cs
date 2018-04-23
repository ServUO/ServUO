using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	public class WhisperingRose : Item
	{
		[Constructable]
		public WhisperingRose() : base( 0x18E9 )
		{
                  Name = "A Whispering Rose";
			Weight = 1.0;
			LootType = LootType.Blessed;
		}

		public WhisperingRose( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}