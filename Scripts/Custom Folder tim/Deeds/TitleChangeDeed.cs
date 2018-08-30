using System;
using Server;
using Server.Commands; 
using Server.Network; 
using Server.Prompts; 
using Server.Mobiles; 
using Server.Misc; 
using Server.Items; 

namespace Server.Items 
{ 
   public class TitleChangeDeed : Item 
   { 
      [Constructable] 
      public TitleChangeDeed() : base( 0x14F0 ) 
      { 
         base.Weight = 1.0; 
         base.Name = "a tile change deed"; 
      } 

      public TitleChangeDeed( Serial serial ) : base( serial ) 
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
         if ( !IsChildOf( from.Backpack ) ) 
         { 
            from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it. 
         } 
         else 
         { 
            from.SendMessage( "Enter your desired Title." ); 
            from.Prompt = new RetitlePrompt( from ); 
            this.Delete(); 
         } 
      } 

      private class RetitlePrompt : Prompt 
      { 
         private Mobile m_from; 

         public RetitlePrompt( Mobile from ) 
         { 
            m_from = from; 
         } 

         public override void OnResponse( Mobile from, string text ) 
         { 
                                PlayerMobile pm = (PlayerMobile) from; 
            text = text.Trim(); 
            if ( !NameVerification.Validate( text, 2, 16, true, true, true, 1, NameVerification.SpaceDashPeriodQuote ))
               return; 

	    pm.Title = text + "" ;                                
            pm.SendMessage( "Your Title be hence forth know as {0}", text ); 
         } 
      } 
   } 
}