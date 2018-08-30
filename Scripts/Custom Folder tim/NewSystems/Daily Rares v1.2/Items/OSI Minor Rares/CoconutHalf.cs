using System;

namespace Server.Items
{
	[Flipable( 0x1723, 0x1724)]
	public class CoconutHalf : Food
	{
		
		
		[Constructable]
		public CoconutHalf() : base( 0x1723 )
		{
			ItemID = Utility.RandomList( 5923, 5924 );
			Weight = 1.0;
		}

		public CoconutHalf( Serial serial ) : base( serial )
		{
		}

		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );
			list.Add( 1049644, "Daily Rare" );
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