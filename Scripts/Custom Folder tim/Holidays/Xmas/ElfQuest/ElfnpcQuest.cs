using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.ContextMenus;
using Server.Gumps;
using Server.Misc;
using Server.Network;
using Server.Spells;
using System.Collections.Generic;

namespace Server.Mobiles
{
	[CorpseName( "You're on the Nauty List" )]
	public class RalphSantasElf : Mobile
	{
        public virtual bool IsInvulnerable{ get{ return true; } }
		[Constructable]
		public RalphSantasElf()
		{
			Name = "Ralph";
                  Title = "Santa's Helper";      
			Body = 400;
                  Hue = 1002;
                  Blessed = true;


	
                  this.HairItemID = 0x203D;
                  this.HairHue = 0x3EA;

                                             
	
			Item Doublet = new Doublet();
			Doublet.Hue = 1367;
      	     Doublet.Name = "Elf Vest";
			AddItem( Doublet ); 

			Item FancyShirt = new FancyShirt();
			FancyShirt.Hue = 1368;
      	      FancyShirt.Name = "Elf Shirt";
			AddItem( FancyShirt ); 

		      Item LongPants = new LongPants();
			LongPants.Hue = 1368;
      	      LongPants.Name = "Elf Pants";
			AddItem( LongPants ); 

                  Item JesterShoes = new JesterShoes(0x7819);
                  JesterShoes.Hue = 32;
      	      JesterShoes.Name = "Elf Shoes";
			AddItem( JesterShoes ); 

                  Item HalfApron = new HalfApron();
			HalfApron .Hue = 33;
      	      HalfApron .Name = "Elf Apron";
			AddItem( HalfApron  ); 

                  Item JesterHat = new JesterHat ();
			JesterHat  .Hue = 2996;
      	      JesterHat  .Name = "Elf Hat";
			AddItem( JesterHat   ); 

                  
             ShepherdsCrook weapon = new  ShepherdsCrook();
             weapon.Hue = 32;
             weapon.Movable = false;
             AddItem(weapon);

			} 
              

			

public RalphSantasElf( Serial serial ) : base( serial )
		{
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
	    { 
			base.GetContextMenuEntries( from, list ); 
        	list.Add( new RalphSantasElfEntry( from, this ) ); 
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

		public class RalphSantasElfEntry : ContextMenuEntry
		{
			private Mobile m_Mobile;
			private Mobile m_Giver;
			
			public RalphSantasElfEntry( Mobile from, Mobile giver ) : base( 6146, 3 )
			{
				m_Mobile = from;
				m_Giver = giver;
			}

			public override void OnClick()
			{
				if( !( m_Mobile is PlayerMobile ) )
				return;
				
				PlayerMobile mobile = (PlayerMobile) m_Mobile;
				
				{
					if ( ! mobile.HasGump( typeof( ElfQuestGump ) ) )
					{
						mobile.SendGump( new ElfQuestGump( mobile ));			
					} 
				}
			}
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{          		
         	Mobile m = from;
			PlayerMobile mobile = m as PlayerMobile;

			if ( mobile != null)
			{
				if( dropped is ReindeerFood)
         		{
         			if(dropped.Amount!=10)
         			{
					this.PrivateOverheadMessage( MessageType.Regular, 1153, false, "I did not ask for that amount! You're getting coal for Christmas!", mobile.NetState );
         				return false;
         			}

					dropped.Delete(); 
					mobile.AddToBackpack( new ElfGiftBox2017() );
				
					return true;
         		}
				else if ( dropped is ReindeerFood)
				{
				this.PrivateOverheadMessage( MessageType.Regular, 1153, 1054071, mobile.NetState );
         			return false;
				}
         		else
         		{
					this.PrivateOverheadMessage( MessageType.Regular, 1153, false, "That's not what I asked for!  You're getting coal for Christmas!", mobile.NetState );
     			}
			}
			return false;

	     }
       }
}
    


