using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server;

namespace Server.Items
{
	public class BalancingTarget : Target
	{
		private BalancingDeed m_Deed;

		public BalancingTarget( BalancingDeed Deed ) : base( 1, false, TargetFlags.None )
		{
			m_Deed = Deed;
		}

		protected override void OnTarget( Mobile from, object target )
		{
			if ( target is BaseRanged )
			{
				Item item = (Item)target;
				if ( ((BaseRanged)item).Balanced == true ) from.SendMessage( "That is already balanced!");
					else if( item.RootParent != from ) from.SendMessage( "You must have the weapon in your backpack!" );
					else
					{
							((BaseRanged)item).Balanced = true;
							from.SendMessage( "You successfully make your weapon balanced." );
							m_Deed.Delete();
					
					}
			}
			else from.SendMessage( "You can only balance archery weapons!" );
		}
//			else from.SendMessage( "You can not balance that item!" );
	}
}

	public class BalancingDeed : Item
	{
		[Constructable]
		public BalancingDeed() : base( 0x14F0 )
		{
			Weight = 0;
			LootType = LootType.Blessed;
			Name = "A Weapon Balancing Deed";
			Hue = 1266;
		}

		public BalancingDeed(Serial serial) : base(serial){}
		public override void Serialize( GenericWriter writer ) {base.Serialize( writer ); writer.Write( (int) 0 );}
		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt();}

		public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) ) from.SendLocalizedMessage( 1042001 );
			else
			{
				from.SendMessage("What item would you like to balance?"  );
				from.Target = new BalancingTarget( this );
			 }
		}
	}
// }