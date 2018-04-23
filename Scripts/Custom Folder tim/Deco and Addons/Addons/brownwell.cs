// Drinkable well addons, 4/5/2005
// Script by Admin Oak, Graphics design by Seer Anabelle
// SylvanDreams.Com

using System;
using Server;
using Server.Items;
using System.Collections;
using Server.Prompts;
using Server.Mobiles;


namespace Server.Items
{
	public class BrownWellAddon : BaseAddon, IWaterSource
	{
		public int Quantity
		{
			get{ return 500; }
			set{}
		}
		

		[Constructable]
		public BrownWellAddon() : this( false )
		{
		}
		
		[Constructable]
		public BrownWellAddon( bool sandstone )
		{
			AddComponent( new BrownWellPiece(this, 10489 ), 1, 1, 15 );
			AddComponent( new BrownWellPiece(this, 4973 ), 1, 1, 4 );
			AddComponent( new BrownWellPiece(this, 3212 ), 2, 0, 3 );
			AddComponent( new BrownWellPiece(this, 10491 ), 0, 1, 15 );
			AddComponent( new BrownWellPiece(this, 4090 ), 0, 1, 9 );
			AddComponent( new BrownWellPiece(this, 7840 ), 0, 1, 4 );
			AddComponent( new BrownWellPiece(this, 3248 ), 0, 1, 0 );
			AddComponent( new BrownWellPiece(this, 3250 ), 0, 1, 0 );
			AddComponent( new BrownWellPiece(this, 9343 ), 0, 0, 0 );
			AddComponent( new BrownWellPiece(this, 9353 ), 0, 0, 5 );
			AddComponent( new BrownWellPiece(this, 6039 ), 0, 0, 0 );
			AddComponent( new BrownWellPiece(this, 10491 ), 0, 0, 15 );
			AddComponent( new BrownWellPiece(this, 9344 ), -1, 0, 0 );
			AddComponent( new BrownWellPiece(this, 9353 ), -1, 0, 5 );
			AddComponent( new BrownWellPiece(this, 3234 ), -1, 1, 0 );
			AddComponent( new BrownWellPiece(this, 9353 ), -1, -1, 5 );
			AddComponent( new BrownWellPiece(this, 9352 ), -1, -1, 0 );
			AddComponent( new BrownWellPiece(this, 9345 ), 0, -1, 0 );
			AddComponent( new BrownWellPiece(this, 9353 ), 0, -1, 5 );
			AddComponent( new BrownWellPiece(this, 3248 ), 1, -1, 0 );
			AddComponent( new BrownWellPiece(this, 9327 ), 1, -1, 0 );
			AddComponent( new BrownWellPiece(this, 10489 ), 1, 0, 15 );
			AddComponent( new BrownWellPiece(this, 4969 ), 1, 0, 2 );
			AddComponent( new BrownWellPiece(this, 4968 ), 1, 0, 4 );
			AddComponent( new BrownWellPiece(this, 3244 ), 1, 0, 0 );
			AddComponent( new BrownWellPiece(this, 3251 ), 1, 0, 0 );

		}
		
		public BrownWellAddon( Serial serial ) : base( serial )
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

	// component
	
	public class BrownWellPiece : AddonComponent
	{
		private BrownWellAddon m_brownwell;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public BrownWellAddon brownwell
		{
			get{ return m_brownwell; }
			set{}
		}

		public BrownWellPiece( BrownWellAddon brownwell, int itemid ) : base( itemid )
		{
			m_brownwell = brownwell;
		}
		
		public override void OnDoubleClick( Mobile from )
		{
			if ( from.InRange( m_brownwell.GetWorldLocation(), 4 ) )
			{
				if ( from.Thirst >= 20 )
				{
					from.SendMessage( "You are not thirsty at all." );
				}
				else
				{
					string msg = null;
					
					if ( m_brownwell == null )
					{
						from.SendMessage( "Debug: Parent was null" );
						return;
					}
					
					switch( Utility.RandomMinMax( 1, 5 ) )
					{
						case 1:  
							msg = "You drink your fill of the cool well water. The quiet sounds of splashing water are softly musical.";
							break;
						case 2:  
							msg = "The well's invigorating water refreshes you and sets your mind at ease. You drink your fill.";	
							break;
						case 3:  
							msg = "You drink deeply of the clean well water. The shimmering reflections on the surface stir your thoughts.";
							break;
						case 4:  
							msg = "As you drink from the water, an tantalizing scent reminds you of memories long forgotten.";
							break;
						case 5:  
							msg = "You drink from the pure well and quiet dreams of sylvan delight pass through your mind.";
							break;
					}
						
					from.SendMessage( msg );
					
					from.Thirst = 20;
				}
			}
			else
			{
				from.SendMessage( "Get closer." );
			}
		}
		
	
		public BrownWellPiece( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int) 0 ); // version
			
			writer.Write( m_brownwell );
			
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();
			
			switch( version )
			{
				case 0: {
					m_brownwell = reader.ReadItem() as BrownWellAddon;
					break;
				}
			}
		}
	}
}
