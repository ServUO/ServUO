using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
	public class PetBondTarget : Target
	{
		private PetBondDeed m_Deed;

		public PetBondTarget( PetBondDeed deed ) : base( 1, false, TargetFlags.None )
		{
			m_Deed = deed;
		}

		protected override void OnTarget( Mobile from, object target )
		{
			if ( m_Deed.Deleted )
				return;

			if ( !m_Deed.IsChildOf( from.Backpack ) )
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			else if ( target is BaseCreature )
			{
				BaseCreature pet = (BaseCreature)target;
				if ( pet.ControlMaster != from )
					from.SendMessage( "That's not your pet!" );
				else if ( pet.IsBonded )
					from.SendMessage( "That is already bonded!" );
				else
				{
					m_Deed.Consume();
					pet.IsBonded = true;
					from.SendLocalizedMessage( 1049666 ); // Your pet has bonded with you!
				}
			}
			else
				from.SendMessage( "That's not your pet!" );
		}
	}

	public class PetBondDeed : Item
	{
		public override string DefaultName{ get{ return "a pet bonding deed"; } }

		[Constructable]
		public PetBondDeed() : base( 0x14F0 )
		{
			Weight = 1.0;
			Hue = 1158;
			LootType = LootType.Blessed;
		}

		public PetBondDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}

		public override bool DisplayLootType{ get{ return false; } }

		public override void OnDoubleClick( Mobile from ) // Override double click of the deed to call our target
		{
			if ( !IsChildOf( from.Backpack ) ) // Make sure its in their pack
				 from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			else
			{
				from.SendMessage( "Target your pet to bond it with you." );
				from.Target = new PetBondTarget( this );
			}
		}
	}
}