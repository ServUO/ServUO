using System;

namespace Server.Items
{
 	[Flipable( 0x3A5F, 0x3A65 )]
	public class RudolphStatue : Item
	{
		[Constructable]
		public RudolphStatue() : this( 0 )
		{
		}

		[Constructable]
		public RudolphStatue( int hue ) : base( 0x3A5F )
		{
			Weight = 5.0;
			Name = "Rudolph the Red Nosed Reindeer";
			LootType = LootType.Blessed; 
		}

		public RudolphStatue( Serial serial ) : base( serial )
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
