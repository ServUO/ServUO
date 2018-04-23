using System;
using System.Collections;
using Server.Network;

namespace Server.Items
{
	public class LuckyCharmsCereal2007 : Food
	{
		[Constructable]
		public LuckyCharmsCereal2007( ) : base( 0x2254 )
		{
			Name = "Lucky Charms Cereal 2007";
			Hue = 1368;
			Weight = 1;
			FillFactor = 1;
			Stackable = false;
			LootType = LootType.Blessed;
		}

		public LuckyCharmsCereal2007( Serial serial ) : base( serial )
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