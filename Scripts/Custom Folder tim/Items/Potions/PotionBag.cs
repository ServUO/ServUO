using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class PotionBag : Bag
	{
		public override string DefaultName
		{
			get { return "a Potion Bag"; }
		}

		[Constructable]
		public PotionBag() : this( 1 )
		{
			Movable = true;
			Hue = 0x250;
		}

		[Constructable]
		public PotionBag( int amount )
		{
			DropItem( new GreaterHealPotion() );
			DropItem( new GreaterHealPotion() );
			DropItem( new GreaterHealPotion() );
			DropItem( new GreaterHealPotion() );
			DropItem( new GreaterHealPotion() );
			DropItem( new GreaterHealPotion() );
			DropItem( new GreaterHealPotion() );
			DropItem( new GreaterHealPotion() );
			DropItem( new GreaterHealPotion() );
			DropItem( new GreaterHealPotion() );
			DropItem( new GreaterCurePotion() );
			DropItem( new GreaterCurePotion() );
			DropItem( new GreaterCurePotion() );
			DropItem( new GreaterCurePotion() );
			DropItem( new GreaterCurePotion() );
			DropItem( new GreaterCurePotion() );
			DropItem( new GreaterCurePotion() );
			DropItem( new GreaterCurePotion() );
			DropItem( new GreaterCurePotion() );
			DropItem( new GreaterCurePotion() );
			DropItem( new TotalRefreshPotion() );
			DropItem( new TotalRefreshPotion() );
			DropItem( new TotalRefreshPotion() );
			DropItem( new TotalRefreshPotion() );
			DropItem( new TotalRefreshPotion() );
			DropItem( new TotalRefreshPotion() );
			DropItem( new TotalRefreshPotion() );
			DropItem( new TotalRefreshPotion() );
			DropItem( new TotalRefreshPotion() );
			DropItem( new TotalRefreshPotion() );
			DropItem( new Bandage( 100 ) );
		}

		public PotionBag( Serial serial ) : base( serial )
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