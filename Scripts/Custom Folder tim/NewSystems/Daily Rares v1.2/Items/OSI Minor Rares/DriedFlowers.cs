using System;

namespace Server.Items
{
	[Flipable( 0xC3B, 0xC3C)]
	public class DriedFlowers : Item
	{
		
		
		
		[Constructable]
		public DriedFlowers() : this( 1 )
		{
		}

		[Constructable]
		public DriedFlowers( int amount ) : base( 0xC3B )
		{
			ItemID = Utility.RandomList( 3131, 3132 );
			Stackable = true;
			Weight = 1.0;
			Amount = amount;
		}

		/*public override Item Dupe( int amount )
		{
			return base.Dupe( new DriedFlowers( amount ), amount );
		}*/

		public DriedFlowers( Serial serial ) : base( serial )
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

	[Flipable( 0xC3D, 0xC3E)]
	public class DriedFlowers2 : Item
	{
		[Constructable]
		public DriedFlowers2() : this( 1 )
		{
		}

		[Constructable]
		public DriedFlowers2( int amount ) : base( 0xC3D )
		{
			ItemID = Utility.RandomList( 3133, 3134 );
			Stackable = true;
			Weight = 1.0;
			Amount = amount;
		}

		/*public override Item Dupe( int amount )
		{
			return base.Dupe( new DriedFlowers2( amount ), amount );
		}*/

		public DriedFlowers2( Serial serial ) : base( serial )
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