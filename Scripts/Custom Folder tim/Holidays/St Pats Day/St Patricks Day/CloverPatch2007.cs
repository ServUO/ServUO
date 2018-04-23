using System; 
using Server.Items; 

namespace Server.Items 
{ 
  	public class CloverPatch2007 : Item 
   	{ 
      	[Constructable] 
      	public CloverPatch2007() : base( 0xC87 ) 
      	{ 
     		Name = "Clover Patch 2007"; 
			Hue = 68;
			Weight = 2.0;
			LootType = LootType.Blessed;
      	}
		
 		public CloverPatch2007( Serial serial ) : base( serial ) 
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