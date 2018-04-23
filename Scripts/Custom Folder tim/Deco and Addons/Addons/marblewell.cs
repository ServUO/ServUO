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
	public class MarbleWellAddon : BaseAddon, IWaterSource
	{
		public int Quantity
		{
			get{ return 500; }
			set{}
		}
		

		[Constructable]
		public MarbleWellAddon() : this( false )
		{
		}
		
		[Constructable]
		public MarbleWellAddon( bool sandstone )
		{
			AddComponent( new MarbleWellPiece(this, 6012 ), -1, 1, 1 );
			AddComponent( new MarbleWellPiece(this, 3248 ), -1, 1, 1 );
			AddComponent( new MarbleWellPiece(this, 272 ), -1, 0, 1 );
			AddComponent( new MarbleWellPiece(this, 269 ), -1, 0, 4 );
			AddComponent( new MarbleWellPiece(this, 3208 ), -1, 0, 1 );
			AddComponent( new MarbleWellPiece(this, 269 ), -1, -1, 4 );
			AddComponent( new MarbleWellPiece(this, 271 ), 0, -1, 1 );
			AddComponent( new MarbleWellPiece(this, 269 ), 0, -1, 4 );
			AddComponent( new MarbleWellPiece(this, 4090 ), 0, -1, 0 );
			AddComponent( new MarbleWellPiece(this, 3250 ), 1, -1, 1 );
			AddComponent( new MarbleWellPiece(this, 3241 ), 1, -1, 1 );
			AddComponent( new MarbleWellPiece(this, 10420 ), 0, 1, 16 );
			AddComponent( new MarbleWellPiece(this, 3245 ), 0, 1, 1 );
			AddComponent( new MarbleWellPiece(this, 10419 ), 1, 1, 16 );
			AddComponent( new MarbleWellPiece(this, 4972 ), 1, 1, 1 );
			AddComponent( new MarbleWellPiece(this, 3246 ), 1, 1, 1 );
			AddComponent( new MarbleWellPiece(this, 10419 ), 1, 0, 16 );
			AddComponent( new MarbleWellPiece(this, 4963 ), 1, 0, 1 );
			AddComponent( new MarbleWellPiece(this, 4090 ), 1, 0, 8 );
			AddComponent( new MarbleWellPiece(this, 7840 ), 1, 0, 3 );
			AddComponent( new MarbleWellPiece(this, 6814 ), 1, 0, 1 );
			AddComponent( new MarbleWellPiece(this, 270 ), 0, 0, 1 );
			AddComponent( new MarbleWellPiece(this, 269 ), 0, 0, 4 );
			AddComponent( new MarbleWellPiece(this, 6039 ), 0, 0, 1 );
			AddComponent( new MarbleWellPiece(this, 10420 ), 0, 0, 16 );
			AddComponent( new MarbleWellPiece(this, 3247 ), 2, 0, 1 );

		}
		
		public MarbleWellAddon( Serial serial ) : base( serial )
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
	
	public class MarbleWellPiece : AddonComponent
	{
		private MarbleWellAddon m_marblewell;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public MarbleWellAddon marblewell
		{
			get{ return m_marblewell; }
			set{}
		}

		public MarbleWellPiece( MarbleWellAddon marblewell, int itemid ) : base( itemid )
		{
			m_marblewell = marblewell;
		}
		
		public override void OnDoubleClick( Mobile from )
		{
			if ( from.InRange( m_marblewell.GetWorldLocation(), 4 ) )
			{
				if ( from.Thirst >= 20 )
				{
					from.SendMessage( "You are not thirsty at all." );
				}
				else
				{
					string msg = null;
					
					if ( m_marblewell == null )
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
		
	
		public MarbleWellPiece( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int) 0 ); // version
			
			writer.Write( m_marblewell );
			
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();
			
			switch( version )
			{
				case 0: {
					m_marblewell = reader.ReadItem() as MarbleWellAddon;
					break;
				}
			}
		}
	}
}
