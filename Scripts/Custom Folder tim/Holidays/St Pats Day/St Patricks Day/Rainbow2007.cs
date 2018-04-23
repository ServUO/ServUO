using System; 
using Server.Items; 

namespace Server.Items 
{ 
  	public class Rainbow2007 : Item 
   	{ 
      	[Constructable] 
      	public Rainbow2007() : base( 0x1FD1 ) 
      	{ 
     		Name = "Rainbow 2007"; 
			Hue = 1060;
			Weight = 10;
			LootType = LootType.Blessed;
      	}
		
 		public Rainbow2007( Serial serial ) : base( serial ) 
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