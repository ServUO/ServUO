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
	public class RedWellAddon : BaseAddon, IWaterSource
	{
		public int Quantity
		{
			get{ return 500; }
			set{}
		}
		

		[Constructable]
		public RedWellAddon() : this( false )
		{
		}
		
		[Constructable]
		public RedWellAddon( bool sandstone )
		{
			AddComponent( new RedWellPiece(this, 10562 ), -1, -1, 5 );
			AddComponent( new RedWellPiece(this, 10558 ), 0, -1, 0 );
			AddComponent( new RedWellPiece(this, 10562 ), 0, -1, 5 );
			AddComponent( new RedWellPiece(this, 3232 ), -1, 1, 0 );
			AddComponent( new RedWellPiece(this, 10555 ), -1, 0, 0 );
			AddComponent( new RedWellPiece(this, 10562 ), -1, 0, 5 );
			AddComponent( new RedWellPiece(this, 3247 ), -1, 0, 0 );
			AddComponent( new RedWellPiece(this, 9181 ), 1, 1, 15 );
			AddComponent( new RedWellPiece(this, 3245 ), 1, 1, 0 );
			AddComponent( new RedWellPiece(this, 9181 ), 1, 0, 15 );
			AddComponent( new RedWellPiece(this, 4973 ), 1, 0, 0 );
			AddComponent( new RedWellPiece(this, 4967 ), 1, 0, 2 );
			AddComponent( new RedWellPiece(this, 6814 ), 1, 0, 0 );
			AddComponent( new RedWellPiece(this, 3262 ), 1, 0, 2 );
			AddComponent( new RedWellPiece(this, 4970 ), 1, -1, 0 );
			AddComponent( new RedWellPiece(this, 10555 ), 0, 0, 0 );
			AddComponent( new RedWellPiece(this, 10558 ), 0, 0, 0 );
			AddComponent( new RedWellPiece(this, 10562 ), 0, 0, 5 );
			AddComponent( new RedWellPiece(this, 6039 ), 0, 0, 0 );
			AddComponent( new RedWellPiece(this, 9183 ), 0, 0, 15 );
			AddComponent( new RedWellPiece(this, 9183 ), 0, 1, 15 );
			AddComponent( new RedWellPiece(this, 4090 ), 0, 1, 8 );
			AddComponent( new RedWellPiece(this, 7840 ), 0, 1, 3 );
			AddComponent( new RedWellPiece(this, 3245 ), 0, 1, 0 );
			AddComponent( new RedWellPiece(this, 3247 ), 2, 0, 0 );
		}
		
		public RedWellAddon( Serial serial ) : base( serial )
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
	
	public class RedWellPiece : AddonComponent
	{
		private RedWellAddon m_redwell;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public RedWellAddon redwell
		{
			get{ return m_redwell; }
			set{}
		}

		public RedWellPiece( RedWellAddon redwell, int itemid ) : base( itemid )
		{
			m_redwell = redwell;
		}
		
		public override void OnDoubleClick( Mobile from )
		{
			if ( from.InRange( m_redwell.GetWorldLocation(), 4 ) )
			{
				if ( from.Thirst >= 20 )
				{
					from.SendMessage( "You are not thirsty at all." );
				}
				else
				{
					string msg = null;
					
					if ( m_redwell == null )
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
		
	
		public RedWellPiece( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int) 0 ); // version
			
			writer.Write( m_redwell );
			
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();
			
			switch( version )
			{
				case 0: {
					m_redwell = reader.ReadItem() as RedWellAddon;
					break;
				}
			}
		}
	}
}
