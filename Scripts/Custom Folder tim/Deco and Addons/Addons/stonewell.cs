// Drinkable well addons, 4/5/2005
// Script by Admin Oak, Graphics design by Seer Anabelle
// SylvanDreams.Com


using System;
using Server;
using Server.Items;
using Server.ContextMenus;
using System.Collections;
using Server.Prompts;
using Server.Mobiles;


namespace Server.Items
{
	public class StoneWellAddon : BaseAddon, IWaterSource
	{
		public int Quantity
		{
			get{ return 500; }
			set{}
		}
		

		[Constructable]
		public StoneWellAddon() : this( false )
		{
		}
		
		[Constructable]
		public StoneWellAddon( bool sandstone )
		{
			AddComponent( new StoneWellPiece(this, 3214 ), 1, 1, 0 );
			AddComponent( new StoneWellPiece(this, 3252 ), 1, 1, 0 );
			AddComponent( new StoneWellPiece(this, 6457 ), 1, 1, 20 );
			AddComponent( new StoneWellPiece(this, 3245 ), 0, 1, 0 );
			AddComponent( new StoneWellPiece(this, 4970 ), 0, 1, 0 );
			AddComponent( new StoneWellPiece(this, 6459 ), 0, 1, 20 );
			AddComponent( new StoneWellPiece(this, 3272 ), 1, -1, 0 );
			AddComponent( new StoneWellPiece(this, 64 ), 0, -1, 5 );
			AddComponent( new StoneWellPiece(this, 66 ), 0, -1, 0 );
			AddComponent( new StoneWellPiece(this, 64 ), -1, -1, 6 );
			AddComponent( new StoneWellPiece(this, 7840 ), 1, 0, 6 );
			AddComponent( new StoneWellPiece(this, 4090 ), 1, 0, 10 );
			AddComponent( new StoneWellPiece(this, 3247 ), 1, 0, 0 );
			AddComponent( new StoneWellPiece(this, 4967 ), 1, 0, 0 );
			AddComponent( new StoneWellPiece(this, 6457 ), 1, 0, 20 );
			AddComponent( new StoneWellPiece(this, 3204 ), -1, 1, 0 );
			AddComponent( new StoneWellPiece(this, 64 ), -1, 0, 5 );
			AddComponent( new StoneWellPiece(this, 67 ), -1, 0, 0 );
			AddComponent( new StoneWellPiece(this, 64 ), 0, 0, 5 );
			AddComponent( new StoneWellPiece(this, 6459 ), 0, 0, 20 );
			AddComponent( new StoneWellPiece(this, 6039 ), 0, 0, 1 );
			AddComponent( new StoneWellPiece(this, 65 ), 0, 0, 0 );
		}
		
		public StoneWellAddon( Serial serial ) : base( serial )
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
	
	public class StoneWellPiece : AddonComponent
	{
		private StoneWellAddon m_stonewell;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public StoneWellAddon stonewell
		{
			get{ return m_stonewell; }
			set{}
		}

		public StoneWellPiece( StoneWellAddon stonewell, int itemid ) : base( itemid )
		{
			m_stonewell = stonewell;
		}
		
		public override void OnDoubleClick( Mobile from )
		{
			if ( from.InRange( m_stonewell.GetWorldLocation(), 4 ) )
			{
				if ( from.Thirst >= 20 )
				{
					from.SendMessage( "You are not thirsty at all." );
				}
				else
				{
					string msg = null;
					
					if ( m_stonewell == null )
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
		
	
		public StoneWellPiece( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int) 0 ); // version
			
			writer.Write( m_stonewell );
			
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();
			
			switch( version )
			{
				case 0: {
					m_stonewell = reader.ReadItem() as StoneWellAddon;
					break;
				}
			}
		}
	}
}
