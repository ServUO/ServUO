using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using System.Collections;

namespace Server.Items
{
   public class fdeggs : Item
   {
      [Constructable]
      public fdeggs() : base( 2485 )
      {
         Name = "Fire Drake eggs";
         Hue = 0x489;

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
        		FireDrake FireDrake = new FireDrake();
        		FireDrake.Controlled = true;
        		FireDrake.ControlMaster = from;
        		FireDrake.IsBonded = true;
        		FireDrake.Location = from.Location;
        		FireDrake.Map = from.Map;
        		World.AddMobile( FireDrake );

               		from.SendMessage( "You raised a Fire Drake with loving care." );
      			this.Delete();
		        } 
		        //else 
		        { 
		            from.SendLocalizedMessage( 500446 ); // That is too far away. 
		        }
      }

      public fdeggs( Serial serial ) : base( serial )
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
