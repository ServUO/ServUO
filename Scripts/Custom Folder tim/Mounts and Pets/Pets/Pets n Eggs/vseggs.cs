using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using System.Collections;

namespace Server.Items
{
   public class vseggs : Item
   {
      [Constructable]
      public vseggs() : base( 2485 )
      {
         Name = "Volt Spider eggs";
         Hue = 0x4fd;

      }

      public override void OnDoubleClick( Mobile from )
      {

	PlayerMobile pm = from as PlayerMobile;

			//if ( !IsChildOf( from.Backpack ) )
			{
				//from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			//else if( from.InRange( this.GetWorldLocation(), 1 ) ) 
		        {
           		from.FixedParticles( 0x373A, 10, 15, 5036, EffectLayer.Head ); 
               		from.PlaySound( 521 );
        		VoltSpider VoltSpider = new VoltSpider();
        		VoltSpider.Controlled = true;
        		VoltSpider.ControlMaster = from;
        		VoltSpider.IsBonded = true;
        		VoltSpider.Location = from.Location;
        		VoltSpider.Map = from.Map;
        		World.AddMobile( VoltSpider );

               		from.SendMessage( "You raised a Volt Spider with loving care." );
      			this.Delete();
		        } 
		        //else 
		        { 
		            from.SendLocalizedMessage( 500446 ); // That is too far away. 
		        }
      }

      public vseggs( Serial serial ) : base( serial )
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
