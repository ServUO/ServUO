#region AuthorHeader
//
//	FindMobs version 1.5 - utilities version 2.0, by Xanthos
//	based on the GoByName utility by unknown
//
#endregion AuthorHeaderusing System;
using System;
using System.Collections;
using Server;
using Server.Network;
using Server.Mobiles;
using Server.Accounting;
using Server.Gumps;
using Server.Commands;

namespace Xanthos.Utilities
{
	public class FindMobsGump : Gump
	{
		public const string kCommandName = "FindMobs";
		public const int kGumpOffsetX = 30;
		public const int kGumpOffsetY = 30;

		public const int kTextHue = 0;
		public const int kTextOffsetX = 2;
		public const int kButtonWidth = 20;
		public const int kOffsetSize = 1;
		public const int kEntryHeight = 20;
		public const int kBorderSize = 10;

		public const int kOffsetGumpID = 0x0052;	// Pure black
		public const int kHeaderGumpID = 0x0E14;	// Dark navy blue, textured
		public const int kEntryGumpID = 0x0BBC;		// Light offwhite, textured
		public const int kBackGumpID = 0x13BE;		// Gray slate/stoney
		public const int kButtonGumpID = 0x0E14;	// Dark navy blue, textured

		public const int kGoOffsetX = 2;
		public const int kGoOffsetY = 2;
		public const int kGoButtonID1 = 0x15E1;		// Arrow pointing right
		public const int kGoButtonID2 = 0x15E5;		// " pressed

		public const int kBringOffsetX = 1;
		public const int kBringOffsetY = 1;
		public const int kBringButtonID1 = 0x15E3;	// 'X' Button
		public const int kBringButtonID2 = 0x15E7;	// " pressed

		public const int kPrevWidth = 20;
		public const int kPrevOffsetX = 2;
		public const int kPrevOffsetY = 2;
		public const int kPrevButtonID1 = 0x15E3;	// Arrow pointing left
		public const int kPrevButtonID2 = 0x15E7;	// " pressed

		public const int kNextWidth = 20;
		public const int kNextOffsetX = 2;
		public const int kNextOffsetY = 2;
		public const int kNextButtonID1 = 0x15E1;	// Arrow pointing right
		public const int kNextButtonID2 = 0x15E5;	// " pressed

		private const int kMiniGumpButtonID = 7107;
		private const int kGreenHue = 0x40;
		private const int kRedHue = 0x20;

		private const int kPrevLabelOffsetX = kPrevWidth + 1;
		private const int kPrevLabelOffsetY = 0;

		private const int kNextLabelOffsetX = -29;
		private const int kNextLabelOffsetY = 0;

		private const int kEntryWidth = 500;
		private const int kEntryCount = 15;

		private const int kTotalWidth = kOffsetSize + kEntryWidth + kOffsetSize + (kButtonWidth * 2) + kOffsetSize;
		private const int kTotalHeight = kOffsetSize + ((kEntryHeight + kOffsetSize) * (kEntryCount + 2));

		private const int kBackWidth = kBorderSize + kTotalWidth + kBorderSize;
		private const int kBackHeight = kBorderSize + kTotalHeight + kBorderSize;

		private static bool s_PrevLabel = false;
		private static bool s_NextLabel = false;

		private string m_SearchValue;
		private int m_Options;
		private Mobile m_Owner;
		private ArrayList m_Names;
		private int m_Page;
		private string m_Args;
		private bool m_MiniGump;

		public static void Initialize()
		{
			CommandSystem.Register( kCommandName, AccessLevel.GameMaster, new CommandEventHandler( Find_OnCommand ) );
		}

		[Usage( kCommandName + " [-i] [-p|-t] <name>" )]
		[Description( "Finds all mobiles by name, -p for players, -t searches by mobiles type name, -i specifies the internal map" )]
		private static void Find_OnCommand( CommandEventArgs e )
		{
			string str;

			try
			{
				int options = GetOptions( e, out str );
				e.Mobile.SendGump( new FindMobsGump( e.Mobile, str, options ) );
			}
			catch
			{
				Misc.SendCommandDetails( e.Mobile, kCommandName );
			}
		}

		public enum FindOptions
		{
			None		= 0x00,
			Player		= 0x01,
			Internal	= 0x02,
			Type		= 0x04,
		}

		private static int GetOptions( CommandEventArgs e, out string searchValue )
		{
			int options = (int)FindOptions.None;

			searchValue = "";

			for ( int i = 0; i < e.Length; i++ )
			{
				string str = e.GetString( i ).ToLower();

				if ( str.Equals( "-?" ) )
					throw new Exception();

				else if ( str.Equals( "-i" ) )
					options |= (int)FindOptions.Internal;

				else if ( str.Equals( "-p" ) )
				{
					options |= (int)FindOptions.Player;
					options &= ~( (int)FindOptions.Type );		// mutually exclusive, turn off the bit
				}
				else if ( str.Equals( "-t" ) )
				{
					options |= (int)FindOptions.Type;
					options &= ~( (int)FindOptions.Player );	// mutually exclusive, turn off the bit
				}
				else
					searchValue = str;
			}

			return options;
		}

		public FindMobsGump( Mobile owner, string searchValue, int options ) : this( owner, BuildList( owner, searchValue, options ), 0, searchValue, options, false )
		{
			m_SearchValue = searchValue;
			m_Options = options;
			m_MiniGump = false;
			GenerateArgString();
		}

		public FindMobsGump( Mobile owner, ArrayList list, int page, string searchValue, int options, bool miniGump ) : base( kGumpOffsetX, kGumpOffsetY )
		{
			m_Owner = owner;
			m_Names = list;
			m_SearchValue = searchValue;
			m_Options = options;
			m_MiniGump = miniGump;
			owner.CloseGump( typeof( FindMobsGump ) );
			GenerateArgString();
			Initialize( page );
		}

		public static ArrayList BuildList( Mobile owner, string searchValue, int options )
		{
			ArrayList list = new ArrayList();

			bool searchInternal = ((options & (int)FindOptions.Internal) == (int)FindOptions.Internal);
			bool findPlayers = ((options & (int)FindOptions.Player) == (int)FindOptions.Player);
			bool findTypes = ((options & (int)FindOptions.Type) == (int)FindOptions.Type);

			foreach ( Mobile mob in World.Mobiles.Values )
			{
				if ( searchInternal != (Map.Internal == mob.Map) )
					continue;
				else if ( findPlayers && !(mob is PlayerMobile) )
					continue;

				if ( findTypes && searchValue != "" && mob.GetType().Name.ToLower().StartsWith( searchValue ) )
					list.Add( mob );
				if ( !findTypes && (searchValue == "" || mob.Name.ToLower().StartsWith( searchValue )) )
					list.Add( mob );
			}
			return list;
		}

		private void GenerateArgString()
		{
			bool m_SearchInternal = ( ( m_Options & (int)FindOptions.Internal ) == (int)FindOptions.Internal );
			bool m_FindPlayers = ( ( m_Options & (int)FindOptions.Player ) == (int)FindOptions.Player );
			bool m_FindTypes = ( ( m_Options & (int)FindOptions.Type ) == (int)FindOptions.Type );

			if ( m_SearchValue == "" )
				m_Args = "all ";

			m_Args += m_FindPlayers ? "players" : "mobiles";
			m_Args += m_SearchInternal ? " (internal)" : "";

			if ( m_SearchValue != "" )
			{
				m_Args += m_FindTypes ? " of type " : " named ";
				m_Args += m_SearchValue;
			}
		}

		public void Initialize( int page )
		{
			m_Page = page;

			int count = m_Names.Count - ( page * kEntryCount );

			if ( count < 0 )
				count = 0;
			else if ( count > kEntryCount )
				count = kEntryCount;

			if ( m_MiniGump )
			{
				AddPage( 0 );
				AddBackground( 0, 0, 40, 40, kBackGumpID );
				AddItem( 10, 10, kMiniGumpButtonID, kGreenHue );
			}
			else
			{
				int kTotalHeight = kOffsetSize + ( ( kEntryHeight + kOffsetSize ) * ( count + 2 ) );
				int x = kBorderSize + kOffsetSize;
				int y = kBorderSize + kOffsetSize;
				int emptyWidth = kEntryWidth;

				AddPage( 0 );

				AddBackground( 0, 0, kBackWidth, kBorderSize + kTotalHeight + kBorderSize, kBackGumpID );
				AddImageTiled( kBorderSize, kBorderSize, kTotalWidth, kTotalHeight, kOffsetGumpID );
				AddImageTiled( x, y, emptyWidth, kEntryHeight, kEntryGumpID );
				AddLabel( x + kTextOffsetX, y, kTextHue, string.Format( "Page {0} of {1} ({2}) - Matches for: {3}", page + 1, ( m_Names.Count + kEntryCount - 1 ) / kEntryCount, m_Names.Count, m_Args ) );

				x += emptyWidth + kOffsetSize;

				AddImageTiled( x, y, kPrevWidth, kEntryHeight, kHeaderGumpID );

				if ( page > 0 )
				{
					AddButton( x + kPrevOffsetX, y + kPrevOffsetY, kPrevButtonID1, kPrevButtonID2, 1, GumpButtonType.Reply, 0 );

					if ( s_PrevLabel )
						AddLabel( x + kPrevLabelOffsetX, y + kPrevLabelOffsetY, kTextHue, "Previous" );
				}

				x += kPrevWidth + kOffsetSize;

				AddImageTiled( x, y, kNextWidth, kEntryHeight, kHeaderGumpID );

				if ( ( page + 1 ) * kEntryCount < m_Names.Count )
				{
					AddButton( x + kNextOffsetX, y + kNextOffsetY, kNextButtonID1, kNextButtonID2, 2, GumpButtonType.Reply, 1 );

					if ( s_NextLabel )
						AddLabel( x + kNextLabelOffsetX, y + kNextLabelOffsetY, kTextHue, "Next" );
				}

				for ( int i = 0, index = page * kEntryCount; i < kEntryCount && index < m_Names.Count; ++i, ++index )
				{
					x = kBorderSize + kOffsetSize;
					y += kEntryHeight + kOffsetSize;

					Mobile item = (Mobile)m_Names[ index ];

					AddImageTiled( x, y, kEntryWidth, kEntryHeight, kEntryGumpID );
					AddLabelCropped( x + kTextOffsetX, y, kEntryWidth - kTextOffsetX, kEntryHeight, 0x58,
						item.Deleted ? "(deleted)" : ( string.Format( "{0} : {1} : {2} {3}", item.Name, item is PlayerMobile ? ( (Account)item.Account ).Username : "Null", item.Map, item.Location ) ) );

					if ( !item.Deleted )
					{
						x += kEntryWidth + kOffsetSize;
						AddImageTiled( x, y, kButtonWidth, kEntryHeight, kButtonGumpID );
						AddButton( x + kBringOffsetX, y + kBringOffsetY, kBringButtonID1, kBringButtonID2, i + 4, GumpButtonType.Reply, 0 );

						x += kButtonWidth + kOffsetSize;
						AddImageTiled( x, y, kButtonWidth, kEntryHeight, kButtonGumpID );
						AddButton( x + kGoOffsetX, y + kGoOffsetY, kGoButtonID1, kGoButtonID2, i + 1004, GumpButtonType.Reply, 0 );
					}
				}
//				AddItem( kBackWidth - 55, ((kEntryHeight + kOffsetSize) * (count + 1)) + 15, kMiniGumpButtonID, kRedHue );
//				AddButton( kBackWidth - 55, ( ( kEntryHeight + kOffsetSize ) * ( count + 1 ) ) + 15, 0, 0, 3, GumpButtonType.Reply, 0 );
			}
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;

			switch ( info.ButtonID )
			{
				case 0: // Close
				{
					return;
				}
				case 1: // Previous
				{
					if ( m_Page > 0 )
						from.SendGump( new FindMobsGump( from, m_Names, m_Page - 1, m_SearchValue, m_Options, false ) );

					break;
				}
				case 2: // Next
					{
						if ( ( m_Page + 1 ) * kEntryCount < m_Names.Count )
							from.SendGump( new FindMobsGump( from, m_Names, m_Page + 1, m_SearchValue, m_Options, false ) );

						break;
					}
				case 3: // MiniGump toggle
					{
						from.SendGump( new FindMobsGump( from, m_Names, m_Page, m_SearchValue, m_Options, !m_MiniGump ) );
						break;
					}
				default:
				{
					int index = (m_Page * kEntryCount) + (info.ButtonID - 4);
					bool bringing = index < 1000;
					if ( !bringing )
						index -= 1000;

					if ( index >= 0 && index < m_Names.Count )
					{
						Mobile sought = (Mobile)m_Names[index];

						if ( sought.Deleted )
						{
							from.SendMessage( "That mobile no longer exists." );
							from.SendGump( new FindMobsGump( from, m_Names, m_Page, m_SearchValue, m_Options, false ) );
						}
						else if ( bringing )
						{
							from.SendMessage( "Bringing {0} to you.", sought.Name );
							sought.Map = from.Map;
							sought.SetLocation( from.Location, true );
							from.SendGump( new FindMobsGump( from, m_Names, m_Page, m_SearchValue, m_Options, false ) );
						}
						else
						{
							from.SendMessage( "Going to {0}.", sought.Name );
							if ( sought.Map == Map.Internal)
							{
								from.Map = sought.LogoutMap;
							}
							else
							{
								from.Map = sought.Map;
							}
							from.SetLocation( sought.Location, true );
							from.SendGump( new FindMobsGump( from, m_Names, m_Page, m_SearchValue, m_Options, false ) );
						}
					}
					break;
				}
			}
		}
	}
}