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
	public class BlackWellAddon : BaseAddon, IWaterSource
	{
		public int Quantity
		{
			get{ return 500; }
			set{}
		}
		

		[Constructable]
		public BlackWellAddon() : this( false )
		{
		}
		
		[Constructable]
		public BlackWellAddon( bool sandstone )
		{
			AddComponent( new BlackWellPiece(this, 9156 ), 2, 1, 15 );
			AddComponent( new BlackWellPiece(this, 3348 ), 0, 1, 3 );
			AddComponent( new BlackWellPiece(this, 9358 ), 0, 0, 0 );
			AddComponent( new BlackWellPiece(this, 9364 ), 0, 0, 5 );
			AddComponent( new BlackWellPiece(this, 6008 ), 2, -1, 0 );
			AddComponent( new BlackWellPiece(this, 3244 ), 2, -1, 0 );
			AddComponent( new BlackWellPiece(this, 9364 ), 0, -1, 5 );
			AddComponent( new BlackWellPiece(this, 9158 ), 1, 1, 15 );
			AddComponent( new BlackWellPiece(this, 3248 ), 1, 1, 0 );
			AddComponent( new BlackWellPiece(this, 9357 ), 1, 0, 0 );
			AddComponent( new BlackWellPiece(this, 9364 ), 1, 0, 5 );
			AddComponent( new BlackWellPiece(this, 6039 ), 1, 0, 0 );
			AddComponent( new BlackWellPiece(this, 9158 ), 1, 0, 15 );
			AddComponent( new BlackWellPiece(this, 9156 ), 2, 0, 15 );
			AddComponent( new BlackWellPiece(this, 6007 ), 2, 0, 0 );
			AddComponent( new BlackWellPiece(this, 4090 ), 2, 0, 9 );
			AddComponent( new BlackWellPiece(this, 7840 ), 2, 0, 4 );
			AddComponent( new BlackWellPiece(this, 3244 ), 2, 0, 0 );
			AddComponent( new BlackWellPiece(this, 3347 ), 2, 0, 3 );
			AddComponent( new BlackWellPiece(this, 7070 ), -1, 0, 0 );
			AddComponent( new BlackWellPiece(this, 9359 ), 1, -1, 0 );
			AddComponent( new BlackWellPiece(this, 9364 ), 1, -1, 5 );
		}
		
		public BlackWellAddon( Serial serial ) : base( serial )
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
	
	public class BlackWellPiece : AddonComponent
	{
		private BlackWellAddon m_Blackwell;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public BlackWellAddon blackwell
		{
			get{ return m_Blackwell; }
			set{}
		}

		public BlackWellPiece( BlackWellAddon blackwell, int itemid ) : base( itemid )
		{
			m_Blackwell = blackwell;
		}
		
		public override void OnDoubleClick( Mobile from )
		{
			if ( from.InRange( m_Blackwell.GetWorldLocation(), 4 ) )
			{
				if ( from.Thirst >= 20 )
				{
					from.SendMessage( "You are not thirsty at all." );
				}
				else
				{
					string msg = null;
					
					if ( m_Blackwell == null )
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
		
		
		public BlackWellPiece( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int) 0 ); // version
			
			writer.Write( m_Blackwell );
			
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();
			
			switch( version )
			{
				case 0: {
					m_Blackwell = reader.ReadItem() as BlackWellAddon;
					break;
				}
			}
		}
	}
}
