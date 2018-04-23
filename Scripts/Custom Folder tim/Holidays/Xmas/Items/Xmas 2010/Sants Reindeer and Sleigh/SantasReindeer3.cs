using System;

namespace Server.Items
{
 	[Flipable( 0x3A6F, 0x3A72 )]
	public class SantasReindeer3 : Item
	{
		[Constructable]
		public SantasReindeer3() : this( 0 )
		{
		}

		[Constructable]
		public SantasReindeer3( int hue ) : base( 0x3A6F )
		{
			Weight = 5.0;
			Name = NameList.RandomName( "reindeer" );
			LootType = LootType.Blessed; 
		}

		public SantasReindeer3( Serial serial ) : base( serial )
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
