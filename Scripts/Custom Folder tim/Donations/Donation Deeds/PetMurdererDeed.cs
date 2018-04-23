using Server.Targeting; 
using System; 
using Server; 
using Server.Gumps; 
using Server.Network; 
using Server.Menus; 
using Server.Menus.Questions; 
using Server.Mobiles; 
using System.Collections; 

namespace Server.Items 
{ 
   	public class PetMurdererDeed : Item 
   	{     
      	[Constructable] 
      	public PetMurdererDeed() : base( 0x14F0 )
      	{ 
         	Weight = 1.0;  
         	Movable = true;
         	Name="a pet murder deed (Changes pets name hue red)"; 
		} 

		public PetMurdererDeed( Serial serial ) : base( serial ) 
		{ 
		}
		
		public override void OnDoubleClick( Mobile from ) 
		{ 
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else if( from.InRange( this.GetWorldLocation(), 1 ) ) 
			{
				from.SendMessage( "Which animal would you like to use this on?" ); 
				from.Target = new PetDyeTarget( this );
			} 
			else 
			{ 
				from.SendLocalizedMessage( 500446 ); // That is too far away. 
			}
		} 

		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 
			writer.Write( (int) 0 ); 
		} 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 
			int version = reader.ReadInt(); 
		} 

  		private class PetDyeTarget : Target 
		{ 
			private Mobile m_Owner; 
			private Item m_Dye; 

			public PetDyeTarget( Item dye ) : base ( 10, false, TargetFlags.None ) 
			{ 
				m_Dye = dye; 
			} 
          
			protected override void OnTarget( Mobile from, object target ) 
			{ 
				if ( target == from ) 
					from.SendMessage( "This can only be used on pets." );

				else if ( target is PlayerMobile )
					from.SendMessage( "You cannont use this on them." );

				else if ( target is Item )
					from.SendMessage( "This does not work on that." );

				else if ( target is BaseCreature ) 
				{ 
					BaseCreature c = (BaseCreature)target;
					
					if ( c.BodyValue == 400 || c.BodyValue == 401 && c.Controlled == false )
					{
						from.SendMessage( "You cannont use this on humans." );
					}
					else if ( c.ControlMaster != from && c.Controlled == false )
					{
						from.SendMessage( "You may only use this on a pet you own." );
					}
					else if ( c.Summoned )
					{
						from.SendMessage( "You cannot use this on summoned creatures." );
					}
					else if ( c.Controlled == true && c.ControlMaster == from)
					{
						c.Kills = 5;
						from.SendMessage( 53, "You have made your pet and murderer." );
						m_Dye.Delete();
					}
				}
			} 
		} 
   	} 
} 
