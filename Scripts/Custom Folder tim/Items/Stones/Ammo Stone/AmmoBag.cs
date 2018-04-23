using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class AmmunitionVendBag : Bag
	{


		public AmmunitionVendBag() : this( 500 )
		{
		}

		[Constructable]

		public AmmunitionVendBag( int amount )
		{
			Movable = true;
			Hue = 0x2D5;
			Name = "Ammunition Bag";

			DropItem( new Arrow   ( amount ) );
                        DropItem( new Bolt   ( amount ) );
			
		}

		public AmmunitionVendBag( Serial serial ) : base( serial )
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