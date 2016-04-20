using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System.Collections;
using System.Collections.Generic;


namespace Server.Gumps
{
	public interface IPlayerSelect
	{
		void OnPlayerSelected( PlayerMobile selectedMobile );
		void OnPlayerSelectCanceled( );
	}


	public class PlayerSelect
	{
		public static void SelectOnlinePlayer( Mobile callingPlayer, IPlayerSelect parentScript )
		{
			PlayerSelect.SelectOnlinePlayer( callingPlayer, parentScript, string.Empty );
		}

		public static void SelectOnlinePlayer( Mobile callingPlayer, IPlayerSelect parentScript, string searchString )
		{
			//Build the list of online players
			ArrayList onlinePlayers = BuildOnlineList( callingPlayer, searchString );

			if ( onlinePlayers.Count == 0 )
			{
				callingPlayer.SendMessage( "No player online matches your request" );
				parentScript.OnPlayerSelectCanceled( );
				return;
			}
			else if ( onlinePlayers.Count == 1 )
			{
				parentScript.OnPlayerSelected( (PlayerMobile)onlinePlayers[0] );
				return;
			}

			callingPlayer.SendGump( new PlayerSelectGump( callingPlayer, parentScript, onlinePlayers ) );
		}
	

		//Builds the list of online players.  Hides GMs from players
		public static ArrayList BuildOnlineList( Mobile callingPlayer, string searchString )
		{
			ArrayList list = new ArrayList();
			List<NetState> states = NetState.Instances;

			for ( int i = 0; i < states.Count; ++i )
			{
				PlayerMobile m = (PlayerMobile)((NetState)states[i]).Mobile;
				if ( m != null && !m.Deleted && ( callingPlayer.AccessLevel >= m.AccessLevel || !m.Hidden ) )
				{
					if ( searchString == string.Empty )
						list.Add( m );
					else if ( m.Name.ToLower().IndexOf( searchString.ToLower()) >= 0 )
						list.Add( m );
				}
			}

			return list;
		}


		private class PlayerSelectGump : Gump
		{
			public const int gumpOffsetX = 50;
			public const int gumpOffsetY = 50;
			public const int playersPerPage = 15;

			ArrayList m_OnlinePlayers;
			private int currentPage;
			private IPlayerSelect m_ParentScript;
			private string m_SearchString;
			

			public PlayerSelectGump( Mobile callingPlayer, IPlayerSelect parentScript, ArrayList onlinePlayers ) : this (callingPlayer, parentScript, onlinePlayers, 0 )
			{
			}

			public PlayerSelectGump( Mobile callingPlayer, IPlayerSelect parentScript, ArrayList onlinePlayers, int requestedPage ) : base ( gumpOffsetX, gumpOffsetY )
			{
				currentPage = requestedPage;
				m_ParentScript = parentScript;
				callingPlayer.CloseGump( typeof( PlayerSelectGump ) );
				m_OnlinePlayers = onlinePlayers;
				

				BuildCurrentGumpPage( );
			}

			public void BuildCurrentGumpPage( )
			{
				//Figure out how many players are on this page
				int playersOnPage = m_OnlinePlayers.Count - (currentPage * playersPerPage);
				if ( playersOnPage < 0 )
					playersOnPage = 0;
				else if ( playersOnPage > playersPerPage )
					playersOnPage = playersPerPage;
				
				int totalHeight = 21 * playersOnPage + 22;

				AddPage( 0 );
				AddBackground( 0, 0, 243, totalHeight+20, GumpUtil.Background_PlainGrey );
				AddImageTiled( 10, 10, 223, totalHeight, GumpUtil.Background_PureBlack );
				AddImageTiled( 11, 11, 189, 20, 0xbbc );
				
				AddLabel( 23, 11, 0, String.Format( "Page {0} of {1} ({2})", currentPage+1, (m_OnlinePlayers.Count + playersPerPage - 1) / playersPerPage, m_OnlinePlayers.Count ) );
	
				AddImageTiled( 191, 11, 20, 20, 0x0E14 );
				if (currentPage > 0)
				{
					AddButton( 193, 13, GumpUtil.GoldArrowUp1, GumpUtil.GoldArrowUp2, GumpUtil.BUTTONID_LAST_PAGE, GumpButtonType.Reply, 0 );
				}
	
				AddImageTiled( 212, 11, 20, 20, 0x0E14 );
				if ( (currentPage + 1) * playersOnPage < m_OnlinePlayers.Count )
				{
					AddButton( 214, 13, GumpUtil.GoldArrowDown1, GumpUtil.GoldArrowDown2, GumpUtil.BUTTONID_NEXT_PAGE, GumpButtonType.Reply, 0 );
				}
				
				int y = 11;
				//Go through each player in the list and display their name
				for ( int i = 0; i < playersOnPage; ++i)
				{
					y += 21;
					Mobile m = (Mobile)m_OnlinePlayers[currentPage * playersPerPage + i];
	
					AddImageTiled( 11, y, 221, 20, 0x0BBC );
					AddButton( 15, y+3, GumpUtil.ButtonBlueUp, GumpUtil.ButtonBlueDown, (currentPage * playersPerPage + i + 100), GumpButtonType.Reply, 0 );
					AddLabelCropped( 35, y, 210, 20, GetHueFor( m ), m.Name );
				}
			}

			private static int GetHueFor( Mobile m )
			{
				switch ( m.AccessLevel )
				{
					case AccessLevel.Administrator: return 0x516;
					case AccessLevel.Seer: return 0x144;
					case AccessLevel.GameMaster: return 0x21;
					case AccessLevel.Counselor: return 0x2;
					case AccessLevel.Player: default:
					{
						if ( m.Kills >= 5 )
							return 0x21;
						else if ( m.Criminal )
							return 0x3B1;
	
						return 0x58;
					}
				}
			}

			//Handles button presses
			public override void OnResponse( NetState state, RelayInfo info )
			{
				Mobile player = state.Mobile;
				switch ( info.ButtonID )
				{
					case 0: // Closed
					{
						player.SendMessage( "Finished" );
						return;
					}
					case GumpUtil.BUTTONID_NEXT_PAGE: // Next
					{
						if ( (currentPage + 1) * playersPerPage < m_OnlinePlayers.Count )
							player.SendGump( new PlayerSelectGump( player, m_ParentScript, m_OnlinePlayers, currentPage + 1 ) );
						else
							player.SendMessage( "Error:  invalid gump return" );
	
						break;
					}
					case GumpUtil.BUTTONID_LAST_PAGE: // Previous
					{
						if ( currentPage > 0 )
							player.SendGump( new PlayerSelectGump( player, m_ParentScript, m_OnlinePlayers, currentPage - 1 ) );
						else
							player.SendMessage( "Error:  invalid gump return" );
	
						break;
					}
					default:
					{
						int selectedInt = info.ButtonID - 100;
						if ( selectedInt >= 0 && selectedInt < m_OnlinePlayers.Count )
						{
							try
							{
								PlayerMobile selectedPlayer = (PlayerMobile)m_OnlinePlayers[selectedInt];
								m_ParentScript.OnPlayerSelected( selectedPlayer );
							}
							catch
							{
								player.SendMessage( "Error caught!" );
							}
							return;
						}
						player.SendMessage( "Error:  invalid gump return" );
						break;
					}
				}
			}
		}
	}
}


