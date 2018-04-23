namespace Server.Items 
{ 
   public class KarmaBong : Item 
   { 
      [Constructable] 
      public KarmaBong() : base( 0x183A ) 
      { 
         Weight = 0.5;
	 Name = "Karma Bong";
	 Hue = 77; 
         Stackable = false;
	 LootType = LootType.Blessed; 
      } 

      public KarmaBong( Serial serial ) : base( serial ) 
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
         Effects.PlaySound( from.Location, from.Map, 33 ); 
         from.SendMessage( 0, "Your karma is {0}, your fame is {1}, and you have {2} murder counts.", from.Karma,  
                       from.Fame, from.Kills); 
      } 
   } 
}
// created on 11/3/2003 at 10:31 AM
