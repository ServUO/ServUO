#region AuthorHeader
//
//	wipemobs version 1.1 - utilities version 2.0, by Xanthos
//
//
#endregion AuthorHeader      
using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Accounting;
using Server.Commands;

namespace Xanthos.Utilities
{
	public class WipeMobsGump : Gump
	{
		public const string kCommandName = "WipeMobs";

		public static void Initialize()
		{
			CommandHandlers.Register( kCommandName, AccessLevel.Administrator, new CommandEventHandler( WipeMobs_OnCommand ) );
		}
		public static string m_args;
		[Usage( kCommandName + " <mobname>" )]
		[Description( "Finds and wipes mobiles by name" )]
		private static void WipeMobs_OnCommand( CommandEventArgs e )
		{
			m_args = e.ArgString;

			if ( 1 != e.Length )
				Misc.SendCommandDetails( e.Mobile, kCommandName );
			else
				e.Mobile.SendGump( new WipeMobsGump( e.Mobile, e.ArgString ) );
		}
		public const int GumpOffsetX = 30;
		public const int GumpOffsetY = 30;

		public const int TextHue = 0;
		public const int TextOffsetX = 2;

		public const int OffsetGumpID = 0x0052; // Pure black
		public const int HeaderGumpID = 0x0E14; // Dark navy blue, textured
		public const int EntryGumpID = 0x0BBC; // Light offwhite, textured
		public const int BackGumpID = 0x13BE; // Gray slate/stoney
		public const int ButtonGumpID = 0x0E14; // Dark navy blue, textured

		public const int ButtonWidth = 20;

		public const int GoOffsetX = 2, GoOffsetY = 2;
		public const int GoButtonID1 = 0x15E1; // Arrow pointing right
		public const int GoButtonID2 = 0x15E5; // " pressed

		public const int DeleteOffsetX = 1, DeleteOffsetY = 1;
		public const int DeleteButtonID1 = 0x0A94; // 'X' Button
		public const int DeleteButtonID2 = 0x0A95; // " pressed

		public const int PrevWidth = 20;
		public const int PrevOffsetX = 2, PrevOffsetY = 2;
		public const int PrevButtonID1 = 0x15E3; // Arrow pointing left
		public const int PrevButtonID2 = 0x15E7; // " pressed

		public const int NextWidth = 20;
		public const int NextOffsetX = 2, NextOffsetY = 2;
		public const int NextButtonID1 = 0x15E1; // Arrow pointing right
		public const int NextButtonID2 = 0x15E5; // " pressed

		public const int WipeWidth = 20;
		public const int WipeOffsetX = 0, WipeOffsetY = 1;
		public const int WipeButtonID1 = 0x0A94; // 'X' Button
		public const int WipeButtonID2 = 0x0A95; // " pressed

		public const int OffsetSize = 1;

		public const int EntryHeight = 20;
		public const int BorderSize = 10;

		private static bool PrevLabel = false, NextLabel = false, WipeLabel = true;

		private const int PrevLabelOffsetX = PrevWidth + 1;
		private const int PrevLabelOffsetY = 0;

		private const int NextLabelOffsetX = -29;
		private const int NextLabelOffsetY = 0;

		private const int WipeLabelOffsetX = WipeWidth + 1;
		private const int WipeLabelOffsetY = 0;

		private const int EntryWidth = 500;
		private const int EntryCount = 15;

		private const int TotalWidth = OffsetSize + EntryWidth + OffsetSize + (ButtonWidth * 2) + OffsetSize;
		private const int TotalHeight = OffsetSize + ((EntryHeight + OffsetSize) * (EntryCount + 2));

		private const int BackWidth = BorderSize + TotalWidth + BorderSize;
		private const int BackHeight = BorderSize + TotalHeight + BorderSize;

		private Mobile m_Owner;
		private ArrayList m_Names;
		private int m_Page;
		public WipeMobsGump( Mobile owner, string args ) : this( owner, BuildList( owner, args ), 0 )
		{
		}

		public WipeMobsGump( Mobile owner, ArrayList list, int page ) : base( GumpOffsetX, GumpOffsetY )
		{
			owner.CloseGump( typeof( WipeMobsGump ) );

			m_Owner = owner;
			m_Names = list;

			Initialize( page );
		}

		public static ArrayList BuildList( Mobile owner, string args )
		{
			ArrayList list = new ArrayList();
			foreach ( Mobile i in World.Mobiles.Values )
			{
				if (args.Length == 0)
					break;
				else if (i.Name == null)
					continue;
				else if (i.Name.ToLower().IndexOf( args.ToLower() ) >= 0)
					list.Add(i);
			}
			return list;
		}

		public void Initialize( int page )
		{
			m_Page = page;

			int count = m_Names.Count - (page * EntryCount);

			if ( count < 0 )
				count = 0;
			else if ( count > EntryCount )
				count = EntryCount;

			int totalHeight = OffsetSize + ((EntryHeight + OffsetSize) * (count + 2));

			AddPage( 0 );

			AddBackground( 0, 0, BackWidth, BorderSize + totalHeight + BorderSize, BackGumpID );
			AddImageTiled( BorderSize, BorderSize, TotalWidth, totalHeight, OffsetGumpID );

			int x = BorderSize + OffsetSize;
			int y = BorderSize + OffsetSize;

			//int emptyWidth = TotalWidth - PrevWidth - NextWidth - (OffsetSize * 4);
			int emptyWidth = EntryWidth;

			AddImageTiled( x, y, emptyWidth, EntryHeight, EntryGumpID );

			AddLabel( x + TextOffsetX, y, TextHue, string.Format( "Page {0} of {1} ({2}) - Matches for: {3}", page+1, (m_Names.Count + EntryCount - 1) / EntryCount, m_Names.Count, m_args) );

			x += emptyWidth + OffsetSize;

			AddImageTiled( x, y, PrevWidth, EntryHeight, HeaderGumpID );

			if ( page > 0 )
			{
				AddButton( x + PrevOffsetX, y + PrevOffsetY, PrevButtonID1, PrevButtonID2, 1, GumpButtonType.Reply, 0 );

				if ( PrevLabel )
					AddLabel( x + PrevLabelOffsetX, y + PrevLabelOffsetY, TextHue, "Previous" );
			}

			x += PrevWidth + OffsetSize;

			AddImageTiled( x, y, NextWidth, EntryHeight, HeaderGumpID );

			if ( (page + 1) * EntryCount < m_Names.Count )
			{
				AddButton( x + NextOffsetX, y + NextOffsetY, NextButtonID1, NextButtonID2, 2, GumpButtonType.Reply, 1 );

				if ( NextLabel )
					AddLabel( x + NextLabelOffsetX, y + NextLabelOffsetY, TextHue, "Next" );
			}

			for ( int i = 0, index = page * EntryCount; i < EntryCount && index < m_Names.Count; ++i, ++index )
			{
				x = BorderSize + OffsetSize;
				y += EntryHeight + OffsetSize;

				Mobile item = (Mobile)m_Names[index];

				AddImageTiled( x, y, EntryWidth, EntryHeight, EntryGumpID );
				AddLabelCropped( x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, 0x58 /*hue*/,
					item.Deleted ? "(deleted)" : (string.Format("{0} : {1} : {2} {3}", item.Name, item is PlayerMobile? ((Account)item.Account).Username : "Null", item.Map, item.Location)) );

				if ( !item.Deleted )
				{
					x += EntryWidth + OffsetSize;
					AddImageTiled( x, y, ButtonWidth, EntryHeight, ButtonGumpID );
					AddButton( x + DeleteOffsetX, y + DeleteOffsetY, DeleteButtonID1, DeleteButtonID2, i + 4, GumpButtonType.Reply, 0 );

					x += ButtonWidth + OffsetSize;
					AddImageTiled( x, y, ButtonWidth, EntryHeight, ButtonGumpID );
					AddButton( x + GoOffsetX, y + GoOffsetY, GoButtonID1, GoButtonID2, i + 1004, GumpButtonType.Reply, 0 );
				}
			}

			x = BorderSize + OffsetSize;
			y += EntryHeight + OffsetSize;

			AddImageTiled( x, y, TotalWidth, EntryHeight, EntryGumpID );

			AddImageTiled( x, y, WipeWidth, EntryHeight, HeaderGumpID );

			AddButton( x + WipeOffsetX, y + WipeOffsetY, WipeButtonID1, WipeButtonID2, 3, GumpButtonType.Reply, 1 );

			if ( WipeLabel )
				AddLabel( x + WipeLabelOffsetX, y + WipeLabelOffsetY, TextHue, "Wipe All Listed" );
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile;

			switch ( info.ButtonID )
			{
				case 0: // Closed
				{
					return;
				}
				case 1: // Previous
				{
					if ( m_Page > 0 )
						from.SendGump( new WipeMobsGump( from, m_Names, m_Page - 1 ) );

					break;
				}
				case 2: // Next
				{
					if ( (m_Page + 1) * EntryCount < m_Names.Count )
						from.SendGump( new WipeMobsGump( from, m_Names, m_Page + 1 ) );

					break;
				}
				case 3: // Wipe All Listed
				{
					from.SendGump( new WipeAllGump(from, m_Names) );
					break;
				}
				default:
				{
					int index = (m_Page * EntryCount) + (info.ButtonID - 4);
					bool deleting = index < 1000;
					if (!deleting)
						index -= 1000;

					if ( index >= 0 && index < m_Names.Count )
					{
						Mobile s = (Mobile)m_Names[index];

						if ( s.Deleted )
						{
							from.SendMessage( "That Mobile no longer exists." );
							from.SendGump( new WipeMobsGump( from, m_Names, m_Page ) );
						}
						else
						{
							if (deleting)
								from.SendGump( new DeleteGump(from, m_Names, index) );
							else
							{
								from.SendMessage( "Going to {0}.", s.Name );
								if ( s.Map == Map.Internal )
								{
									from.Map = s.LogoutMap;
								}
								else
								{
									from.Map = s.Map;
								}
								from.SetLocation( s.Location, true );
								from.SendGump( new WipeMobsGump( from, m_Names, m_Page ) );
							}
						}
					}

					break;
				}
			}
		}

		private class DeleteGump : Gump
		{
			private Mobile m_From;
			private ArrayList m_Names;
			private int m_Index;

			public DeleteGump( Mobile from, ArrayList spawners, int index ) : base( 50, 50 )
			{
				m_From = from;
				m_Names = spawners;
				m_Index = index;

				AddPage( 0 );

				AddBackground( 0, 0, 270, 120, 5054 );
				AddBackground( 10, 10, 250, 100, 3000 );

				AddHtml( 20, 15, 230, 60, "Are you sure you wish to delete this Mobile?", true, true );

				AddButton( 20, 80, 4005, 4007, 2, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 55, 80, 75, 20, 1011011, false, false ); // CONTINUE

				AddButton( 135, 80, 4005, 4007, 1, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 170, 80, 75, 20, 1011012, false, false ); // CANCEL
			}

			public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
			{
				if ( info.ButtonID == 2 )
				{
					((Mobile)m_Names[m_Index]).Delete();
					m_Names.RemoveAt(m_Index);
					m_From.SendLocalizedMessage( 1010303 ); // deleted object
				}
				m_From.SendGump( new WipeMobsGump( m_From, m_Names, 0 ) );
			}
		}

		private class WipeAllGump : Gump
		{
			private Mobile m_From;
			private ArrayList m_Names;

			public WipeAllGump( Mobile from, ArrayList spawners ) : base( 50, 50 )
			{
				m_From = from;
				m_Names = spawners;

				AddPage( 0 );

				AddBackground( 0, 0, 270, 120, 5054 );
				AddBackground( 10, 10, 250, 100, 3000 );

				AddHtml( 20, 15, 230, 60, "Are you sure you wish to delete all " + m_Names.Count + " of the listed Mobiles?", true, true );

				AddButton( 20, 80, 4005, 4007, 2, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 55, 80, 75, 20, 1011011, false, false ); // CONTINUE

				AddButton( 135, 80, 4005, 4007, 1, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 170, 80, 75, 20, 1011012, false, false ); // CANCEL
			}

			public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
			{
				if ( info.ButtonID == 2 )
				{
					foreach (Mobile item in m_Names)
					{
						item.Delete();
					}

					m_From.SendMessage( "Deleted {0} Mobiles.", m_Names.Count );
				}
			}
		}
	}
}
