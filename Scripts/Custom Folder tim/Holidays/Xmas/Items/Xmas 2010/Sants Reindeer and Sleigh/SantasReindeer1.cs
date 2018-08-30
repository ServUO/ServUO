using System;

namespace Server.Items
{
 	[Flipable( 0x3A55, 0x3A56 )]
	public class SantasReindeer1 : Item
	{
		[Constructable]
		public SantasReindeer1() : this( 0 )
		{
		}

		[Constructable]
		public SantasReindeer1( int hue ) : base( 0x3A55 )
		{
			Weight = 5.0;
			Name = NameList.RandomName( "reindeer" );
			LootType = LootType.Blessed; 
		}

		public SantasReindeer1( Serial serial ) : base( serial )
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
