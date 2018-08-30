using System;
using System.Collections;
using Server.Network;

namespace Server.Items
{
	public class BowlOfPotatoes2007 : Food
	{
		[Constructable]
		public BowlOfPotatoes2007( ) : base( 0x1602 )
		{
			Name = "Bowl Of Potatoes 2007";
			Weight = 1;
			FillFactor = 1;
			Stackable = false;
			LootType = LootType.Blessed;
		}

		public BowlOfPotatoes2007( Serial serial ) : base( serial )
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