//Tukaram June2017
using System; 
using Server; 

namespace Server.Items 
{ 

   	public class BrokenLichStaffPart1 : Item 
   	{ 
       
      		[Constructable] 
      		public BrokenLichStaffPart1() 
      		{ 
      		
				Name = "The Arm Of A Broken Lich Staff";          
         		ItemID=3721;
         		Weight= 5.0;
                Hue = 1152;
                       
      		} 

      		public override bool ForceShowProperties{ get{ return ObjectPropertyList.Enabled; } }

     		public BrokenLichStaffPart1( Serial serial ) : base( serial ) 
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