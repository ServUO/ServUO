using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server;

namespace Server.Items
{
	public class SelfRepairTarget : Target // Create our targeting class (which we derive from the base target class)
	{
		private SelfRepairDeed m_Deed;

		public SelfRepairTarget( SelfRepairDeed deed ) : base( 1, false, TargetFlags.None )
		{
			m_Deed = deed;
		}

		protected override void OnTarget( Mobile from, object target ) // Override the protected OnTarget() for our feature
		{
			if ( target is BaseWeapon )
			{
				Item item = (Item)target;

				if ( ((BaseWeapon)item).WeaponAttributes.SelfRepair == 5 )
				{
					from.SendMessage( "That already has self repair!");
				}
				else
				{
					if( item.RootParent != from ) // Make sure its in their pack or they are wearing it
					{
						from.SendMessage( "You cannot put self repair on that there!" ); // You cannot bless that object
					}
					else
					{
						((BaseWeapon)item).WeaponAttributes.SelfRepair = 10;
						from.SendMessage( "You magically add self repair to your weapon...." );

						m_Deed.Delete(); // Delete the deed
					}
				}
			}
			else
			{
				from.SendMessage( "You cannot put self repair on that" );
			}
		}
	}

	public class SelfRepairDeed : Item // Create the item class which is derived from the base item class
	{
		[Constructable]
		public SelfRepairDeed() : base( 0x14F0 )
		{
			Weight = 1.0;
			Name = "Weapon Self Repair deed";
			LootType = LootType.Blessed;
			Hue = 50;
		}

		public SelfRepairDeed( Serial serial ) : base( serial )
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
			LootType = LootType.Blessed;

			int version = reader.ReadInt();
		}

		public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from ) // Override double click of the deed to call our target
		{
			if ( !IsChildOf( from.Backpack ) ) // Make sure its in their pack
			{
				 from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else
			{
				from.SendMessage("What item would you like to add self repair to?"  );
				from.Target = new SelfRepairTarget( this ); // Call our target
			 }
		}	
	}
}


