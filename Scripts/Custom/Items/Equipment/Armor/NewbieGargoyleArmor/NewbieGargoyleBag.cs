using System; 
using Server; 
using Server.Items;

namespace Server.Items
{ 
   public class NewbieGargoyleArmor : Bag 
   { 
		[Constructable] 
		public NewbieGargoyleArmor() : this( 1 ) 
		{ 		
			Movable = true;  
			Name = "Gargoyle Newbie Gear";
			Hue = 1150;
		}
		[Constructable]
		public NewbieGargoyleArmor( int amount )
		{
			DropItem( new GargishNewbieShield() );
			DropItem( new GargishNewbieWingArmor() );
			DropItem( new GargishNewbieChest() );
			DropItem( new GargishNewbieLegs() );
			DropItem( new GargishNewbieArms() );
                        DropItem( new GargishNewbieKilt() );
  			//DropItem( new MayTheBowBeWithYou() );
			//DropItem( new Arrow(30) );
		}

      public NewbieGargoyleArmor( Serial serial ) : base( serial ) 
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
