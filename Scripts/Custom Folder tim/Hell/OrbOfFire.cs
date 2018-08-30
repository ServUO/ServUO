using System; 
using Server.Network; 
using Server.Items; 
using Server.Mobiles; 

namespace Server.Items 
{ 
   public class OrbOfFire : Item 
   { 
      [Constructable] 
      public OrbOfFire() : base( 15284 ) 
      { 
         Movable = true; 
	     Hue = 1360;
         Name = "Orb Of Fire";
      }

       public OrbOfFire(Serial serial)
           : base(serial) 
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

      
      public override void OnDoubleClick( Mobile from ) 
      { 
         if ( !from.InRange( GetWorldLocation(), 2 ) ) 
         { 
            from.SendLocalizedMessage( 500446 ); // That is too far away. 
         } 
         else 
         { 
            if ( from.BodyMod == 0x0 ) 
            { 
               from.BodyMod = 130; 
		   from.HueMod = -1;
           from.PlaySound (0x174); 
              Effects.SendLocationParticles( EffectItem.Create( from.Location, from.Map, EffectItem.DefaultDuration ), 0x376A, 1, 29, 0x47D, 2, 9962, 0 ); 
              Effects.SendLocationParticles( EffectItem.Create( new Point3D( from.X, from.Y, from.Z - 7 ), from.Map, EffectItem.DefaultDuration ), 0x37C4, 1, 29, 0x47D, 2, 9502, 0 ); 

            }
               else 
                { 
                  from.BodyMod = 0x0;
                  from.PlaySound (0x174); 
                 Effects.SendLocationParticles( EffectItem.Create( from.Location, from.Map, EffectItem.DefaultDuration ), 0x376A, 1, 29, 0x47D, 2, 9962, 0 ); 
                 Effects.SendLocationParticles( EffectItem.Create( new Point3D( from.X, from.Y, from.Z - 7 ), from.Map, EffectItem.DefaultDuration ), 0x37C4, 1, 29, 0x47D, 2, 9502, 0 ); 

              } 
            } 
         } 
   } 
} 
