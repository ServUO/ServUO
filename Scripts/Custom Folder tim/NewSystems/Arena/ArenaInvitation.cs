using System;
using System.Net;
using Server;
using Server.Accounting;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Prompts;
using Server.Regions;
using System.Collections;

namespace Server.Items
{

	public class ArenaInvitation : Item
	{

		[Constructable]
		public ArenaInvitation() : this( null )
		{
		}
		
		[Constructable]
		public ArenaInvitation( string name ) : base( 0x2258 )
		{
			Name = "Arena Invitation";
			LootType = LootType.Blessed;
		}
		
		public ArenaInvitation( Serial serial ) : base( serial )
		{
		}
		
		public override void OnDoubleClick( Mobile from )
		{			
			ArrayList list = new ArrayList();
			
			foreach ( Mobile m in World.Mobiles.Values )
			{
				if ( m is BaseHealer )
				{
					BaseHealer bc = (BaseHealer)m;
					
					if ( bc is LordBritish )
						list.Add( bc );
				}
			}
			if ( list.Count > 0 )
				from.SendMessage( "A Warrior Is Already Braving The Arena..." );

			
			else
			{
				from.SendGump( new ArenaGump( from, this ) );
			}
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	
	
		public class ArenaGump : Gump
		{
		private Mobile m_Mobile;
		private Item m_Deed;
		
		public ArenaGump( Mobile from, Item deed ) : base( 30, 20 )
		{
			m_Mobile = from;
			m_Deed = deed;
						
			Account a = from.Account as Account;
			
			AddImageTiled(  54, 33, 369, 400, 2624 );
			AddAlphaRegion( 54, 33, 369, 400 );
			AddImageTiled( 416, 39, 44, 389, 203 );			
			AddImage( 97, 49, 9005 );
			AddImageTiled( 58, 39, 29, 390, 10460 );
			AddImageTiled( 412, 37, 31, 389, 10460 );
			AddLabel( 140, 60, 0x34, "BRAVE THE ARENA...." );
			AddImage( 430, 9, 10441);
			AddImageTiled( 40, 38, 17, 391, 9263 );
			AddImage( 6, 25, 10421 );
			AddImage( 34, 12, 10420 );
			AddImageTiled( 94, 25, 342, 15, 10304 );
			AddImageTiled( 40, 427, 415, 16, 10304 );
			AddImage( -10, 314, 10402 );
			AddImage( 56, 150, 10411 );
			AddImage( 136, 84, 96 );
			AddImage( 215, 150, 5536 );

			AddButton( 225, 390, 0xF7, 0xF8, 1, GumpButtonType.Reply, 0 );
			
		}
		
		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;
			
			switch( info.ButtonID )
			{
				case 0:
					{
						from.CloseGump( typeof( ArenaGump ) );
						break;
					}
				case 1:
					{
						from.MoveToWorld( new Point3D( 2367, 1127, -90 ), Map.Malas );
						m_Deed.Delete(); // Delete the deed

						Item ad = new ArenaDecor();

						break;
					}
				}
			}
		}
	}