//Tukaram June2017
using System; 
using Server; 

namespace Server.Items 
{ 

   	public class BrokenLichStaffPart2 : Item 
   	{ 
       
      		[Constructable] 
      		public BrokenLichStaffPart2 () 
      		{ 
      		
				Name = "The Head Of A Broken Lich Staff";          
         		ItemID=3971;
         		Weight= 5.0;
                Hue = 1152;
                  
            } 

      		public override bool ForceShowProperties{ get{ return ObjectPropertyList.Enabled; } }

     		public BrokenLichStaffPart2 ( Serial serial ) : base( serial ) 
     	 	{ 
      		} 

      		public override void Serialize( GenericWriter writer ) 
      		{ 
         		base.Serialize( writer ); 

         		writer.Write( (int) 0 ); 
      		} 
       
      		public override void Deserialize(GenericReader reader) 
      		{ 
         		base.Deserialize( reader ); 

         		int version = reader.ReadInt(); 
      		} 
   	}     
} 