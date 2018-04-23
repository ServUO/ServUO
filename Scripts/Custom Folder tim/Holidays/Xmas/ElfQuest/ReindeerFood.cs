using System; 
using Server; 

namespace Server.Items 
{ 
   public class ReindeerFood : Item 
   { 
      [Constructable] 
      public ReindeerFood() : this( 1 ) 
      { 
      } 

      [Constructable] 
      public ReindeerFood( int amount ) : base( 0x1039 ) 
      {
	 Name = "Bag Of Reindeer Food";
	 Stackable = true;
	 //Hue = 55;
         Weight = 0.1; 
         Amount = amount; 
      } 

      public ReindeerFood( Serial serial ) : base( serial ) 
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