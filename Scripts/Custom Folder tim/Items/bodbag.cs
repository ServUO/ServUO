using System; 
using Server; 
using Server.Items;
using Server.Engines.BulkOrders;
using System.Collections;

namespace Server.Items
{ 
   public class SmithBoDBag : Bag 
   { 
      [Constructable] 
      public SmithBoDBag() : this( 50 ) 
      { 
		  Movable = true; 
		  Hue = 0x8AB; 
		  Name = "a Smith BoD Kit";
      } 
	   [Constructable]
	   public SmithBoDBag( int amount )
	   {
		   int i;
		   int l;

		   for ( i = 0; i < 45; i++ ) 
		   {
			   DropItem( new SmallSmithBOD () );
		   }
		   for ( l = 0; l < 10; l++ ) 
		   {
			   DropItem( new LargeSmithBOD () );
		   }
	   }
		

      public SmithBoDBag( Serial serial ) : base( serial ) 
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
 
	public class TailorBoDBag : Bag 
	{ 
		[Constructable] 
		public TailorBoDBag() : this( 50 ) 
		{ 
			Movable = true; 
			Hue = 0x483; 
			Name = "a Tailoring BoD Kit";
		} 
		[Constructable]
		public TailorBoDBag( int amount )
		{
			int i;
			int l;
		   
			for ( i = 0; i < 45; i++ )
			{
				DropItem( new SmallTailorBOD () );
			}
		   
			for ( l = 0; l < 10; l++ ) 
			{
				DropItem( new LargeTailorBOD () );
			}

		}
		

		public TailorBoDBag( Serial serial ) : base( serial ) 
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
    
    #region Taming
    /*
		public class TamingBoDBag : Bag
    {
        [Constructable]
        public TamingBoDBag()
            : this(50)
        {
            Movable = true;
            Hue = 0x483;
            Name = "a Taming BoD Kit";
        }
        [Constructable]
        public TamingBoDBag(int amount)
        {
            int i;
            int l;

            for (i = 0; i < 45; i++)
            {
                DropItem(new SmallTamingBOD());
            }

            for (l = 0; l < 10; l++)
            {
                DropItem(new LargeTamingBOD());
            }

        }


        public TamingBoDBag(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }  
    */
	#endregion
    
} 

