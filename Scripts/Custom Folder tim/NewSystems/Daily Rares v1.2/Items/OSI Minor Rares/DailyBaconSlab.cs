using System;

namespace Server.Items
{
	[Flipable( 0x976, 0x977)]
	public class DailyBaconSlab : Food
	{
		
		
		[Constructable]
		public DailyBaconSlab() : this( 1 )
		{
		}

		[Constructable]
		public DailyBaconSlab( int amount ) : base( 0x976 )
		{
			Name = "Bacon Slab";
			ItemID = Utility.RandomList( 2422, 2423 );
			Stackable = true;
			Weight = 1.0;
			Amount = amount;
		}

		/*public override Item Dupe( int amount )
		{
			return base.Dupe( new BaconSlab( amount ), amount );
		}*/

		public DailyBaconSlab( Serial serial ) : base( serial )
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