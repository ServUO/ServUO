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
   	public class PetSkillCapDeed : Item 
   	{     
   		public static int AnimalMaxSkillsCap = 9000;
   		
      	[Constructable] 
      	public PetSkillCapDeed() : base( 0x14F0 )
      	{ 
         	Weight = 1.0;  
         	Movable = true;
         	Name="a pet skill cap increase deed (+10)";
         	LootType = LootType.Blessed;
		} 

		public PetSkillCapDeed( Serial serial ) : base( serial ) 
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
				from.Target = new BondingTarget( this );
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

  		private class BondingTarget : Target 
		{ 
			private Mobile m_Owner; 
			private Item m_Deed; 

			public BondingTarget( Item deed ) : base ( 10, false, TargetFlags.None ) 
			{ 
				m_Deed = deed; 
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
					
					if ( c.ControlMaster != from && c.Controlled == false )
					{
						from.SendMessage( "You may only use this on a pet you own." );
					}
					else if ( c.Summoned )
					{
						from.SendMessage( "You cannot use this on summoned creatures." );
					}
					else if ( c.Controlled == true && c.ControlMaster == from)
					{
						if ( c.SkillsCap < AnimalMaxSkillsCap )
						{
							c.SkillsCap += 10;
							
							if ( c.SkillsCap > AnimalMaxSkillsCap )
								c.SkillsCap = AnimalMaxSkillsCap;
							
							from.SendMessage( "Your pets skill cap has been increased by 10.0%." );
						}
						else
						{
							from.SendMessage( "Your pet is already at the highest skill cap possible." );
						}
					}
				}
			} 
		} 
   	} 
} 
