using System;

namespace Server.Items
{
 	[Flipable( 0x3A5F, 0x3A65 )]
	public class OliveTheOtherReindeer : Item
	{
		[Constructable]
		public OliveTheOtherReindeer() : this( 0 )
		{
		}

		[Constructable]
		public OliveTheOtherReindeer( int hue ) : base( 0x3A5F )
		{
			Weight = 10.0;
			Name = "Olive The Other Reindeer";
			LootType = LootType.Blessed; 
		}

		public OliveTheOtherReindeer( Serial serial ) : base( serial )
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