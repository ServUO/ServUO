//Make a weapon One-HAnded, based on the Self Repair deed by SHAMBAMPOW
using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server;

namespace Server.Items
{
	public class OneHandedTarget : Target
	{
		private OneHandedDeed m_Deed;

		public OneHandedTarget( OneHandedDeed deed ) : base( 1, false, TargetFlags.None )
		{
			m_Deed = deed;
		}

		protected override void OnTarget( Mobile from, object target )
		{
			 if ( target is BaseWeapon )
			{
				Item item = (Item)target;

						if( item.RootParent != from )
						{
							from.SendMessage( "You cannot use this deed on that item there!" ); 
						}
						else
						{
							if(((BaseWeapon)item).Layer == Layer.OneHanded)
							{
								from.SendMessage( "That weapon is already One-Handed!");
								return;
							}
							else
							{
								((BaseWeapon)item).Layer = Layer.OneHanded;
								from.SendMessage( "This weapon is now One-Handed.");
								m_Deed.Delete();
							}
						}
					return;
			}
			else
			{
				from.SendMessage( "You cannot use this deed on that!" );
			}
		}
		
	}

	public class OneHandedDeed : Item
	{
		[Constructable]
		public OneHandedDeed() : base( 0x14F0 )
		{
			Weight = 1.0;
			Name = "Make a weapon One-Handed deed";
			LootType = LootType.Blessed;
			Hue = 1150;
		}

		public OneHandedDeed( Serial serial ) : base( serial )
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

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				 from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else
			{
				from.SendMessage("Which weapon would you like to make One-Handed?"  );
				from.Target = new OneHandedTarget( this );
			 }
		}	
	}
}