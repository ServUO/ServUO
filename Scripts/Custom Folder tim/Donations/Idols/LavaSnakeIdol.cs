

      





   
       

using System; 
using Server.Network; 
using Server.Items; 
using Server.Mobiles; 

namespace Server.Items 
{ 
   public class LavaSnakeIdol : Item 
   { 
      [Constructable] 
      public LavaSnakeIdol() : base( 9666 ) 
      { 
         Movable = true; 
         Name = "Lava Snake Idol"; 
         Hue = 1643;
      } 

      public LavaSnakeIdol( Serial serial ) : base( serial ) 
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
         if ( !from.InRange( GetWorldLocation(),0 ) ) 
         { 
            from.SendLocalizedMessage( 500446 ); // That is too far away. 
         } 
         else 
         { 
            if ( from.BodyValue == 0x190 || from.BodyValue == 0x191 ) 
            { 
               from.BodyValue = 52; 
               from.PlaySound( 219 ); 
               from.HueMod = 1643;
              Effects.SendLocationParticles( EffectItem.Create( from.Location, from.Map, EffectItem.DefaultDuration ), 0x376A, 1, 29, 0x47D, 2, 9962, 0 ); 
              Effects.SendLocationParticles( EffectItem.Create( new Point3D( from.X, from.Y, from.Z - 7 ), from.Map, EffectItem.DefaultDuration ), 0x37C4, 1, 29, 0x47D, 2, 9502, 0 ); 

            } 
            else 
            { 
               if (from.Female == true ) 
                { 
                  from.BodyValue = 0x191; 
                  from.PlaySound( 219 ); 
                  from.HueMod = -1;
                 Effects.SendLocationParticles( EffectItem.Create( from.Location, from.Map, EffectItem.DefaultDuration ), 0x376A, 1, 29, 0x47D, 2, 9962, 0 ); 
                 Effects.SendLocationParticles( EffectItem.Create( new Point3D( from.X, from.Y, from.Z - 7 ), from.Map, EffectItem.DefaultDuration ), 0x37C4, 1, 29, 0x47D, 2, 9502, 0 ); 
    
                } 
               else 
                { 
                  from.BodyValue = 0x190; 
                  from.PlaySound( 219 ); 
                  from.HueMod = -1;
                 Effects.SendLocationParticles( EffectItem.Create( from.Location, from.Map, EffectItem.DefaultDuration ), 0x376A, 1, 29, 0x47D, 2, 9962, 0 ); 
                 Effects.SendLocationParticles( EffectItem.Create( new Point3D( from.X, from.Y, from.Z - 7 ), from.Map, EffectItem.DefaultDuration ), 0x37C4, 1, 29, 0x47D, 2, 9502, 0 ); 

              } 
            } 
         } 
      } 
   } 
} 
