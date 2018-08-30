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
	public class WoodWellAddon : BaseAddon, IWaterSource
	{
		public int Quantity
		{
			get{ return 500; }
			set{}
		}
		

		[Constructable]
		public WoodWellAddon() : this( false )
		{
		}
		
		[Constructable]
		public WoodWellAddon( bool sandstone )
		{
			AddComponent( new WoodWellPiece(this, 8564 ), 0, 1, 20 );
			AddComponent( new WoodWellPiece(this, 7840 ), 0, 1, 8 );
			AddComponent( new WoodWellPiece(this, 4090 ), 0, 1, 13 );
			AddComponent( new WoodWellPiece(this, 3264 ), 0, 1, 0 );
			AddComponent( new WoodWellPiece(this, 9 ), -1, -1, 0 );
			AddComponent( new WoodWellPiece(this, 3264 ), 1, 1, 0 );
			AddComponent( new WoodWellPiece(this, 3244 ), 1, 1, 0 );
			AddComponent( new WoodWellPiece(this, 8561 ), 1, 1, 20 );
			AddComponent( new WoodWellPiece(this, 9 ), 0, -1, 0 );
			AddComponent( new WoodWellPiece(this, 22 ), 0, -1, 0 );
			AddComponent( new WoodWellPiece(this, 3207 ), -1, 1, 0 );
			AddComponent( new WoodWellPiece(this, 3223 ), -1, 1, 0 );
			AddComponent( new WoodWellPiece(this, 3248 ), 1, 0, 8 );
			AddComponent( new WoodWellPiece(this, 3246 ), 1, 0, 3 );
			AddComponent( new WoodWellPiece(this, 4973 ), 1, 0, 6 );
			AddComponent( new WoodWellPiece(this, 4963 ), 1, 0, 0 );
			AddComponent( new WoodWellPiece(this, 8561 ), 1, 0, 20 );
			AddComponent( new WoodWellPiece(this, 8564 ), 0, 0, 20 );
			AddComponent( new WoodWellPiece(this, 9 ), 0, 0, 0 );
			AddComponent( new WoodWellPiece(this, 6039 ), 0, 0, 1 );
			AddComponent( new WoodWellPiece(this, 20 ), 0, 0, 0 );
			AddComponent( new WoodWellPiece(this, 9 ), -1, 0, 0 );
			AddComponent( new WoodWellPiece(this, 21 ), -1, 0, 0 );
			AddComponent( new WoodWellPiece(this, 3203 ), -1, 0, 0 );
			AddComponent( new WoodWellPiece(this, 3223 ), 1, -1, 0 );
		}
		
		public WoodWellAddon( Serial serial ) : base( serial )
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
	
	public class WoodWellPiece : AddonComponent
	{
		private WoodWellAddon m_woodwell;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public WoodWellAddon woodwell
		{
			get{ return m_woodwell; }
			set{}
		}

		public WoodWellPiece( WoodWellAddon woodwell, int itemid ) : base( itemid )
		{
			m_woodwell = woodwell;
		}
		
		public override void OnDoubleClick( Mobile from )
		{
			if ( from.InRange( m_woodwell.GetWorldLocation(), 4 ) )
			{
				if ( from.Thirst >= 20 )
				{
					from.SendMessage( "You are not thirsty at all." );
				}
				else
				{
					string msg = null;
					
					if ( m_woodwell == null )
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
		
	
		public WoodWellPiece( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int) 0 ); // version
			
			writer.Write( m_woodwell );
			
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();
			
			switch( version )
			{
				case 0: {
					m_woodwell = reader.ReadItem() as WoodWellAddon;
					break;
				}
			}
		}
	}
}
