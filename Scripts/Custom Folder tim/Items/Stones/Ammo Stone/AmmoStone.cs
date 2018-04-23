using System;
using Server.Items;

namespace Server.Items
{
	public class AmmunitionVendStone : Item
	{
		public override string DefaultName
		{
			get { return "Ammunition Vendor Stone"; }
		}

		[Constructable]
		public AmmunitionVendStone() : base( 0xEDc )
		{
			Movable = false;
			Hue = 0x2D5;

		}

		public override void OnDoubleClick( Mobile from )
		{
                  // Bag Cost---2000 Gold
		   	Item[] Token = from.Backpack.FindItemsByType( typeof( Gold ) );
		   	if ( from.Backpack.ConsumeTotal( typeof( Gold ), 2000 ) )
		{
         	   	AmmunitionVendBag AmmunitionVendBag = new AmmunitionVendBag();
		   	from.AddToBackpack( AmmunitionVendBag );
			from.SendMessage( "2000 gold has been removed from your pack." );
		}
		   	else
		   	{
		   		from.SendMessage( "You do not have enough gold." );
		   	}
					
		}

		public AmmunitionVendStone( Serial serial ) : base( serial )
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