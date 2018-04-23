using System; 
using Server; 
using Server.Gumps; 
using Server.Network; 

    namespace Server.Items 
    { 
    	public class MyLegacyToken : Item 
    	{
    		[Constructable] 
    		public MyLegacyToken() : this( null ) 
   			{ 
    		} 

    		[Constructable]
        	public MyLegacyToken(String name): base(10922)
    		{
        		Name = "A Token For An Ethereal Mount";
        		Stackable = false;
                        Hue = 1195;
        		Weight = 12.0;
        		LootType = LootType.Cursed;
        
    		}

        	public MyLegacyToken(Serial serial)
            	: base(serial) 
    			{ 
    			} 

    		public override void OnDoubleClick( Mobile from ) 
    		{ 
    			if ( !IsChildOf( from.Backpack ) ) 
    			{ 
    				from.SendLocalizedMessage( 1042001 ); 
    			} 
    			else 
    			{
            		from.SendGump( new MyLegacyTokenGump( from, this ) ); 
    			} 
    		} 

    		public override void Serialize ( GenericWriter writer) 
    		{ 
    			base.Serialize ( writer ); 
   				writer.Write ( (int) 0); 
    		} 

    		public override void Deserialize( GenericReader reader ) 
    		{ 
    			base.Deserialize ( reader ); 
    			int version = reader.ReadInt(); 
    		} 
    	} 
    }
