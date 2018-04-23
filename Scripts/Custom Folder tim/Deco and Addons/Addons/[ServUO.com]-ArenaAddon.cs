using System; 
using Server; 

namespace Server.Items 
{ 
   public class ArenaAddon : BaseAddon 
   { 
      public override BaseAddonDeed Deed{ get{ return new ArenaDeed(); } } 

      [Constructable] 
      public ArenaAddon() 
      { 
         AddComponent( new AddonComponent( 0x709 ), -4, 0, 0 ); 
         AddComponent( new AddonComponent( 0x709 ),  -4, 1, 0 ); 
         AddComponent( new AddonComponent( 0x709 ),  -4, 2, 0 ); 
         AddComponent( new AddonComponent( 0x709 ),  -4, 3, 0 ); 
         AddComponent( new AddonComponent( 0x709 ),  -4, 4, 0 ); 
         AddComponent( new AddonComponent( 0x709 ),  -4, 5, 0 ); 
         AddComponent( new AddonComponent( 0x709 ),  -4, 6, 0 ); 
         AddComponent( new AddonComponent( 0x709 ),  -4, 7, 0 ); 
         AddComponent( new AddonComponent( 0x709 ),  -4, 8, 0 ); 
         AddComponent( new AddonComponent( 0x709 ),  -4, 9, 0 );          
         AddComponent( new AddonComponent( 0x709 ),  -3, 0, 0 );          
         AddComponent( new AddonComponent( 0x709 ),  -3, 9, 0 ); 
         AddComponent( new AddonComponent( 0x709 ),  -2, 0, 0 ); 
         AddComponent( new AddonComponent( 0x709 ),  -2, 9, 0 ); 
         AddComponent( new AddonComponent( 0x709 ),  -1,  0, 0 ); 
         AddComponent( new AddonComponent( 0x709 ),  -1,  9, 0 ); 
	 AddComponent( new AddonComponent( 0x709 ),  0, 0, 0 ); 
         AddComponent( new AddonComponent( 0x709 ),  0, 9, 0 ); 
         AddComponent( new AddonComponent( 0x709 ),  1, 0, 0 );          
         AddComponent( new AddonComponent( 0x709 ),  1, 9, 0 );          
         AddComponent( new AddonComponent( 0x709 ),  2, 0, 0 ); 
         AddComponent( new AddonComponent( 0x709 ),  2, 9, 0 ); 
         AddComponent( new AddonComponent( 0x709 ),  3, 0, 0 ); 
         AddComponent( new AddonComponent( 0x709 ),  3,  9, 0 ); 
         AddComponent( new AddonComponent( 0x709 ),  4,  0, 0 ); 
	 AddComponent( new AddonComponent( 0x709 ),  4, 9, 0 ); 
         AddComponent( new AddonComponent( 0x709 ),  5,  0, 0 ); 
         AddComponent( new AddonComponent( 0x709 ),  5,  1, 0 ); 
	 AddComponent( new AddonComponent( 0x709 ),  5, 2, 0 ); 
         AddComponent( new AddonComponent( 0x709 ),  5,  3, 0 ); 
         AddComponent( new AddonComponent( 0x709 ),  5,  4, 0 ); 
	 AddComponent( new AddonComponent( 0x709 ),  5, 5, 0 ); 
         AddComponent( new AddonComponent( 0x709 ),  5, 6, 0 ); 
         AddComponent( new AddonComponent( 0x709 ),  5,  7, 0 ); 
         AddComponent( new AddonComponent( 0x709 ),  5,  8, 0 ); 
	 AddComponent( new AddonComponent( 0x709 ),  5, 9, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  0, 1, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  0, 2, 0 );
         AddComponent( new AddonComponent( 0x511 ),  0, 3, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  0, 4, 0 );
         AddComponent( new AddonComponent( 0x511 ),  0, 5, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  0, 6, 0 );
         AddComponent( new AddonComponent( 0x511 ),  0, 7, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  0, 8, 0 );
	 AddComponent( new AddonComponent( 0x511 ),  1, 1, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  1, 2, 0 );
         AddComponent( new AddonComponent( 0x511 ),  1, 3, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  1, 4, 0 );
         AddComponent( new AddonComponent( 0x511 ),  1, 5, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  1, 6, 0 );
         AddComponent( new AddonComponent( 0x511 ),  1, 7, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  1, 8, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  2, 1, 0 );
         AddComponent( new AddonComponent( 0x511 ),  2, 2, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  2, 3, 0 );
         AddComponent( new AddonComponent( 0x511 ),  2, 4, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  2, 5, 0 );
         AddComponent( new AddonComponent( 0x511 ),  2, 6, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  2, 7, 0 ); 
	 AddComponent( new AddonComponent( 0x511 ),  2, 8, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  3, 1, 0 );
         AddComponent( new AddonComponent( 0x511 ),  3, 2, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  3, 3, 0 );
         AddComponent( new AddonComponent( 0x511 ),  3, 4, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  3, 5, 0 );
         AddComponent( new AddonComponent( 0x511 ),  3, 6, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  3, 7, 0 );
	 AddComponent( new AddonComponent( 0x511 ),  3, 8, 0 );
         AddComponent( new AddonComponent( 0x511 ),  4, 8, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  4, 1, 0 );
         AddComponent( new AddonComponent( 0x511 ),  4, 2, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  4, 3, 0 );
         AddComponent( new AddonComponent( 0x511 ),  4, 4, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  4, 5, 0 );
         AddComponent( new AddonComponent( 0x511 ),  4, 6, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  4, 7, 0 );   
         AddComponent( new AddonComponent( 0x511 ),  4, 8, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  5, 1, 0 );
         AddComponent( new AddonComponent( 0x511 ),  5, 2, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  5, 3, 0 );
         AddComponent( new AddonComponent( 0x511 ),  5, 4, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  5, 5, 0 );
         AddComponent( new AddonComponent( 0x511 ),  5, 6, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  5, 7, 0 );   
         AddComponent( new AddonComponent( 0x511 ),  5, 8, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  -1, 1, 0 );
         AddComponent( new AddonComponent( 0x511 ),  -1, 2, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  -1, 3, 0 );
         AddComponent( new AddonComponent( 0x511 ),  -1, 4, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  -1, 5, 0 );
         AddComponent( new AddonComponent( 0x511 ),  -1, 6, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  -1, 7, 0 );   
         AddComponent( new AddonComponent( 0x511 ),  -1, 8, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  -2, 1, 0 );
         AddComponent( new AddonComponent( 0x511 ),  -2, 2, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  -2, 3, 0 );
         AddComponent( new AddonComponent( 0x511 ),  -2, 4, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  -2, 5, 0 );
         AddComponent( new AddonComponent( 0x511 ),  -2, 6, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  -2, 7, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  -2, 8, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  -3, 1, 0 );
         AddComponent( new AddonComponent( 0x511 ),  -3, 2, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  -3, 3, 0 );
         AddComponent( new AddonComponent( 0x511 ),  -3, 4, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  -3, 5, 0 );
         AddComponent( new AddonComponent( 0x511 ),  -3, 6, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  -3, 7, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  -3, 8, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  -4, 1, 0 );
         AddComponent( new AddonComponent( 0x511 ),  -4, 2, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  -4, 3, 0 );
         AddComponent( new AddonComponent( 0x511 ),  -4, 4, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  -4, 5, 0 );
         AddComponent( new AddonComponent( 0x511 ),  -4, 6, 0 ); 
         AddComponent( new AddonComponent( 0x511 ),  -4, 7, 0 );
         AddComponent( new AddonComponent( 0x511 ),  -4, 8, 0 );
         AddComponent( new AddonComponent( 0x70A ),  0, 10, 0 );
         AddComponent( new AddonComponent( 0x70A ),  1, 10, 0 );
         AddComponent( new AddonComponent( 0x70C ),  0, 8, 0 );
         AddComponent( new AddonComponent( 0x70C ),  1, 8, 0 );
        
                     
      } 

      public ArenaAddon( Serial serial ) : base( serial ) 
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

   public class ArenaDeed : BaseAddonDeed 
   { 
      public override BaseAddon Addon{ get{ return new ArenaAddon(); } } 
      public override int LabelNumber{ get{ return 1026635; } } // fountain 

      [Constructable] 
      public ArenaDeed() 
      { 
      } 

      public ArenaDeed( Serial serial ) : base( serial ) 
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