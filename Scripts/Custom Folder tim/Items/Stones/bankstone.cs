using System; 
using Server.Items; 
using Server.Targeting;
using Server.Targets;
using Server.Multis;
using System.Collections;
using Server;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.ContextMenus;

namespace Server.Items 
{ 
   public class BankStone : Item
   { 
      [Constructable] 
      public BankStone() : base( 0xED4 ) 
      { 
         Name = "Bank Stone"; 
         Movable = true;
	 Hue = 0x480;
         Weight = 100;
      }
                
      public override void OnDoubleClick( Mobile from )
      {
      if ( this.Movable == true )
      {
       from.SendMessage ("This must be locked down to use!");
       return;
      }
      	 BankBox box = from.BankBox;

      	 if ( box != null )
      	 box.Open(); 
      }
      public BankStone( Serial serial ) : base( serial )
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

