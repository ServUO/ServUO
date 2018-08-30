using System; 
using Server.Items; 

namespace Server.Items 
{ 
  	public class PotOfGold2007 : Item 
   	{ 
      	[Constructable] 
      	public PotOfGold2007() : base( 0x9ED ) 
      	{ 
     		Name = "Pot O' Gold 2007"; 
//			Hue = 2654;
			Hue = 1161;
			Weight = 5.0;
			LootType = LootType.Blessed;
      	}
		
 		public PotOfGold2007( Serial serial ) : base( serial ) 
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