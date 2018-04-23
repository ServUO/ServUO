using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class BagofDuelistMagerArmor : Bag
	{
		[Constructable]
		public BagofDuelistMagerArmor() : this( 1 )
		{
		Name = " Bag of Duelist Mager Armor ";
		}
		[Constructable]
		public BagofDuelistMagerArmor( int amount )
		{
			DropItem( new DuelistMagersLegs() );
			DropItem( new DuelistMagerArms() );
			DropItem( new DuelistMagersBreast() );
			DropItem( new DuelistMagersCap() );
			DropItem( new DuelistMagersGloves() );
			DropItem( new DuelistMagerGorget() );
		}

		public BagofDuelistMagerArmor( Serial serial ) : base( serial )
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