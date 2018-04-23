using System;
using Server;
using Server.Network;
using Server.Prompts;
using Server.Items;
using Server.Misc;
using Server.Mobiles;

namespace Server.Items
{
   public class PetSlotStatue : Item
   {
      [Constructable]
      public PetSlotStatue() : base( 8471 )
      {
         base.Weight = 1.0;
         base.Name = "A Pet Slot +1 Statue";
         base.Hue = 0x8A;
         LootType = LootType.Blessed;
      }

      public PetSlotStatue(Serial serial) : base(serial)
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
      PlayerMobile mobile = from as PlayerMobile;

                if ( !IsChildOf( from.Backpack ) ) 
       { 
         from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
       }
         else if ( mobile.FollowersMax >= 10 )
         {
            mobile.SendMessage( "You are not allowed more than 7 pet slots." );
         }
         else
         {
                         mobile.FollowersMax += 1;
              mobile.SendMessage( "You have gained +1 to your maximum pet slots." );
             

                     Effects.SendLocationParticles( EffectItem.Create( from.Location, from.Map, EffectItem.DefaultDuration ), 0, 0, 0, 0, 0, 5060, 0 );
                     Effects.PlaySound( from.Location, from.Map, 0x243 );
                     Effects.SendMovingParticles( new Entity( Serial.Zero, new Point3D( from.X - 6, from.Y - 6, from.Z + 15 ), from.Map ), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100 );
                     Effects.SendMovingParticles( new Entity( Serial.Zero, new Point3D( from.X - 4, from.Y - 6, from.Z + 15 ), from.Map ), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100 );
                     Effects.SendMovingParticles( new Entity( Serial.Zero, new Point3D( from.X - 6, from.Y - 4, from.Z + 15 ), from.Map ), from, 0x36D4, 7, 0, false, true, 0x497, 0, 9502, 1, 0, (EffectLayer)255, 0x100 );
               Delete(); 
            }
         }
      }
   }