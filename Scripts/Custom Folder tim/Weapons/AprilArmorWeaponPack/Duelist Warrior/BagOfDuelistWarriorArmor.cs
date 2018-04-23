using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class BagofDuelistWarriorArmor : Bag
	{
		[Constructable]
		public BagofDuelistWarriorArmor() : this( 1 )
		{
		Name = " Bag of Duelist Warrior Armor ";
		}
		[Constructable]
		public BagofDuelistWarriorArmor( int amount )
		{
			DropItem( new DuelistWarriorLegs() );
			DropItem( new DuelistWarriorArms() );
			DropItem( new DuelistWarriorChest() );
			DropItem( new DuelistWarriorHelm() );
			DropItem( new DuelistWarriorGloves() );
			DropItem( new DuelistWarriorGorget() );
		}

		public BagofDuelistWarriorArmor( Serial serial ) : base( serial )
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