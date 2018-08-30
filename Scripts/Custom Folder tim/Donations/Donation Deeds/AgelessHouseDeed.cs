//Ageless House Deed For RunUO SVN
//By DxMonkey - AKA - Tresdni
//Ultima Eclipse - www.ultimaeclipse.com
using System;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Targeting;
using Server.Multis;

namespace Server.Items
{
	public class AgelessHouseTarget : Target 
	{
		private AgelessHouseDeed m_Deed;

		public AgelessHouseTarget( AgelessHouseDeed deed ) : base( 1, false, TargetFlags.None )
		{
			m_Deed = deed;
		}

		protected override void OnTarget( Mobile from, object target ) 
		{
			if ( m_Deed.Deleted || m_Deed.RootParent != from )
				return;

			if ( target is HouseSign )
			{
				HouseSign item = (HouseSign)target;

				if ( item.RestrictDecay == true )
				{
					from.SendMessage ( "This house is already ageless" ); 
				}
				
				else
				{
					item.RestrictDecay = true;
					from.SendMessage ( "The house is now ageless for 30 Days" );
						
					Timer m_timer = new AgelessHouseTimer( item ); 
					m_timer.Start();

					m_Deed.Delete(); // Delete the ageless house deed
				}
			}
			else
			{
				from.SendMessage ( "You must target a house sign!" );
			}
		}
	}
	
	public class AgelessHouseTimer : Timer
	{
		private HouseSign sign;

			public AgelessHouseTimer( HouseSign h ) : base( TimeSpan.FromDays( 30 ) )
     			{
       			 	sign = h;
          			Priority = TimerPriority.OneSecond;
       			}
			protected override void OnTick()
      			{
			
				sign.RestrictDecay = false;
				
				}
	}


	public class AgelessHouseDeed : Item 
	{
		
		[Constructable]
		public AgelessHouseDeed() : base( 0x14F0 )
		{
			Weight = 1.0;
			Hue = 1159;
			LootType = LootType.Blessed;
			Name = "An Ageless House Deed ( 30 Days )";
				
		}

		public AgelessHouseDeed( Serial serial ) : base( serial )
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


		public override void OnDoubleClick( Mobile from ) // Override double click of the deed to call our target
		{
			if ( !IsChildOf( from.Backpack ) ) // Make sure its in their pack
			{
				 from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else
			{
				from.SendMessage ( "Which house would you like to make ageless?" );
				from.Target = new AgelessHouseTarget( this ); // Call our target
			 }
		}	
	}
}