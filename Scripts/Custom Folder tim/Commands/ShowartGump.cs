  //
 //  Written by Haazen Feb 2011
//
using System;
using System.Collections;
using System.IO;
using Server;
using Server.Commands;
using Server.Items;
using Server.Network;
using Server.Prompts;
using Server.Multis;
using Server.Targeting;

namespace Server.Gumps
{
	public class ShowartGump : Gump
	{

		public static void Initialize()
		{
			CommandSystem.Register( "Showart", AccessLevel.GameMaster, new CommandEventHandler( Showart_OnCommand ) );
		}

		[Usage( "Showart" )]
		[Description( "Show art" )]
		public static void Showart_OnCommand( CommandEventArgs e )
		{

			e.Mobile.SendGump( new ShowartGump( e.Mobile, 16383 ) );
		}

		private Mobile m_From;
		private int m_newstart;

		public void AddBlackAlpha( int x, int y, int width, int height )
		{
			AddImageTiled( x, y, width, height, 2624 );
			AddAlphaRegion( x, y, width, height );
		}

		public ShowartGump( Mobile from, int index ) : base( 50, 40 )
		{
			from.CloseGump( typeof( ShowartGump ) );

			int start = index;

			m_From = from;
			m_newstart = index;

			AddPage( 0 );

			AddBackground( 0, 0, 522, 475, 9270 );
			AddBackground( 11, 35, 500, 430, 3000 );


			AddLabelCropped( 32, 13, 150, 20, 1152, "Haazen's ShowartGump" );
			AddBackground( 355, 13, 60, 20, 3000 );
			AddTextEntry( 360, 13, 60, 20, 10, 20, "" );
			AddButton( 425, 13, 0x15E2, 0x15E6, 3, GumpButtonType.Reply, 0 );

			AddButton( 465, 14, 0x15E3, 0x15E7, 1, GumpButtonType.Reply, 0 );
			AddButton( 482, 14, 0x15E1, 0x15E5, 2, GumpButtonType.Reply, 0 );


			AddDetails( start );

		}

		private void AddDetails( int index )
		{
		  try{

			for ( int i = 0; i < 5; ++ i )
			{
				AddLabel( 13, 70 + (i * 75), 1152, String.Format( "{0}", index + (i * 6) ) );
				for ( int j = 0; j < 6; ++ j )
				{

			//		AddButton( 70 + (j * 75), 50 + (i * 75), 0x24b2, 0x24b2, index + (i * 6) + (j), GumpButtonType.Reply, 0 );
					AddItem( 70 + (j * 75), 50 + (i * 75), index + (i * 6) + (j) );
				}
			}

		  }catch{}
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;
			int newstart = 0;
			int buttonID = info.ButtonID;

			if ( buttonID == 1 )
			{
				newstart = m_newstart - 30;
				if ( newstart < 1 )
					newstart = 1;
				from.CloseGump( typeof( ShowartGump ) );
				from.SendGump( new ShowartGump( from, newstart ) );
			}
			else if ( buttonID == 2 )
			{
				newstart = m_newstart + 30;
				from.CloseGump( typeof( ShowartGump ) );
				from.SendGump( new ShowartGump( from, newstart ) );
			}
			else if ( buttonID == 3 )
			{
				from.CloseGump( typeof( ShowartGump ) );
				TextRelay entry = info.GetTextEntry( 20 );

				try { newstart = Convert.ToInt32(entry.Text); } 
				catch {}
				if ( newstart < 1 )
					newstart = 1;

				from.SendGump( new ShowartGump( from, newstart ) );
			}
			else
			{
			//	from.CloseGump( typeof( ShowartGump ) );
			//	from.SendMessage("{0}", buttonID);
			//	from.SendGump( new ShowartGump( from, m_newstart ) );
			}
		}
	}
}