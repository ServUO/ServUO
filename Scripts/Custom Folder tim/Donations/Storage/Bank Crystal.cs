using Server.Items;
using Server.Gumps; 
using Server.Network; 
using Server.Menus; 
using Server.Mobiles;
using Server.Menus.Questions;

namespace Server.Items
{ 
	public class BankCrystal : Item 
	{ 

		[Constructable] 
		public BankCrystal() : base( 7964 ) 
		{ 
			Movable = true; 
			Hue = 0x480; 
			Name = "Bank Crystal";
			LootType = LootType.Blessed;
		} 

		public override void OnDoubleClick( Mobile from ) 
		{ 
			//from.Handled = true;

			BankBox box = from.BankBox;

			if ( box != null )
				box.Open();

     	 	} 
 
		public BankCrystal( Serial serial ) : base( serial ) 
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