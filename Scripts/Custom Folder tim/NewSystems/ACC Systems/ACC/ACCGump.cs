using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Network;

namespace Server.ACC
{
	public class ACCGump : Gump
	{
		public static void Initialize()
		{
			CommandSystem.Register( "ACC", ACC.GlobalMinimumAccessLevel, new CommandEventHandler( OnCommand ) );
		}

		[Usage( "ACC" )]
		[Description( "Sends the ACC Gump" )]
		private static void OnCommand( CommandEventArgs e )
		{
			e.Mobile.SendGump( new ACCGump( e.Mobile, null, null ) );
		}

		private List<string> m_List;
        private string m_SysString;
		private ACCSystem m_Syst;
		private ACCGumpParams  m_SubP;

		public ACCGump( Mobile from, string system, ACCGumpParams subParams ) : base( 0, 0 )
		{
			if( from.AccessLevel < ACC.GlobalMinimumAccessLevel )
				return;

			m_List = new List<string>();
            m_SysString = system;
			m_SubP = subParams;

			foreach( KeyValuePair<string,bool> kvp in ACC.RegisteredSystems )
			{
				m_List.Add( kvp.Key );
			}

			Closable   = true;
			Disposable = true;
			Dragable   = true;
			Resizable  = false;

			AddPage(0);

			AddBackground( 0,   0, 630, 360,  5120 ); //Top BG
			AddBackground( 0, 360, 630, 113,  5120 ); //Bottom BG
			AddImageTiled( 0, 446, 620,  50, 10452 ); //Bottom

			if( system == null )
			{
				AddHtml( 175, 40, 375,  30, "<basefont size=7 color=#33CC33><center>A_Li_N - Completely Custom</center></basefont>", false, false );
				AddHtml( 175, 80, 420, 265, "<basefont size=4 color=white>Thanks for choosing to test A_Li_N - Completely Custom.  With this gump, you will be able to control every aspect of my systems that are changable in-game.  Currently, there are only a couple test bases that I have made to do minimal testing.  If you have any questions, concerns, requests, bugs or anything else having to do with my systems, please email me at anAliengmail.com with the topic of 'ACC'.  Enjoy!</basefont>", false, false );
			}

			for( int i = 0; i < m_List.Count; i++ )
			{
				Type t = Type.GetType( m_List[i] );
				if( t == null )
					continue;

				ACCSystem sys = (ACCSystem)Activator.CreateInstance( t );
				if( sys == null )
					continue;

				AddButton( (i<3?35:(i<6?225:415)), (i%3==0?372:(i%3==1?397:422)), 1122, 1124, i+1, GumpButtonType.Reply, 0 );
				AddHtml( (i<3?35:(i<6?225:415)), (i%3==0?370:(i%3==1?395:420)), 184, 20, String.Format( "<basefont color=white><center>{0}</center></basefont>", sys.Name() ), false, false );

				if( system == m_List[i] )
					m_Syst = sys;
			}

			if( m_Syst != null )
			{
				AddButton( 560, 0, 1417, 1417, 10, GumpButtonType.Reply, 0 );
				if( m_Syst.Enabled )
					AddLabel( 592, 45, 66, "On" );
				else
					AddLabel( 588, 45, 36, "Off" );

//				AddButton( 15, 340, 22153, 22155, 11, GumpButtonType.Reply, 0 ); //Help
			}

			AddImage(   0,   0,  9002); //Border
			AddImage( 580, 350, 10410); //Dragon

			if( m_Syst != null )
			{
				AddPage(1);
				m_Syst.Gump( from, this, m_SubP );
			}
		}
/*
Reserved ButtonIDs
1-9	= System Buttons
10	= Dis/Enable System
11	= System Help
 */

		public override void OnResponse( NetState state, RelayInfo info )
		{
			if( info.ButtonID == 0 || state.Mobile.AccessLevel < ACC.GlobalMinimumAccessLevel )
				return;

			if( info.ButtonID >= 1 && info.ButtonID < 10 )
			{
				int page = info.ButtonID-1;
				if( m_SysString == m_List[page] )
					state.Mobile.SendGump( new ACCGump( state.Mobile, null, null ) );
				else if( page >= 0 && page <= m_List.Count )
					state.Mobile.SendGump( new ACCGump( state.Mobile, m_List[page], null ) );
				return;
			}

			if( info.ButtonID == 10 && m_Syst != null )
			{
				state.Mobile.SendMessage( "{0} {1}", (m_Syst.Enabled?"Disabling":"Enabling"), m_Syst.Name() );
				if( m_Syst.Enabled )
					ACC.DisableSystem( m_Syst.ToString() );
				else
					ACC.EnableSystem( m_Syst.ToString() );

				state.Mobile.SendGump( new ACCGump( state.Mobile, m_SysString, m_SubP ) );

				return;
			}

/*			if( info.ButtonID == 11 && m_Syst != null )
			{
				m_Syst.Help();
			}
*/
			if( m_Syst != null )
				m_Syst.OnResponse( state, info, m_SubP );
		}
	}
}