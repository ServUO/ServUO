using System;
namespace Server.Items
{
	[Furniture]
	[Flipable( 0x1945, 0x1946 )]
	public class CarvedWoodenScreen : Item
	{
		[Constructable]
		public CarvedWoodenScreen() : base( 0x1945 )
		{
			LootType = LootType.Blessed;
			Weight = 20.0; // itemdata says 6
		}

		public CarvedWoodenScreen( Serial serial ) : base( serial )
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
