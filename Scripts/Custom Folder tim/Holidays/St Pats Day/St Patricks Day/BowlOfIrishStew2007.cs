using System;
using System.Collections;
using Server.Network;

namespace Server.Items
{
	public class BowlOfIrishStew2007 : Food
	{
		[Constructable]
		public BowlOfIrishStew2007( ) : base( 0x15F9 )
		{
			Name = "Bowl Of Irish Stew 2007";
			Weight = 1;
			FillFactor = 1;
			Stackable = false;
			LootType = LootType.Blessed;
		}

		public BowlOfIrishStew2007( Serial serial ) : base( serial )
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