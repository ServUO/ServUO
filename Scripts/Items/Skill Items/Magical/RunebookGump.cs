using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Items;
using Server.Network;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Seventh;
using Server.Spells.Chivalry;
using Server.Prompts;

namespace Server.Gumps
{
	public class RunebookGump : Gump
	{
		public override int TypeID { get { return 0x59; } }

		// button IDs:
		//1 - rename book
		//200 - drop rune
		//10 - recall (charge)
		//50 - recall (spell)
		//100 - gate travel
		//75 - sacred journey
		//300 - set default
		//0 - close gump

		public void PrecompileStringTable()
		{
			// here's the order on OSI:
			// first two strings are "old" unlocalized version:
			Intern( "Charges", true );              // 0
			Intern( "Max Charges", true );          // 1
			// Next 16 entries are Location Values
			for ( int i = 0; i < 16; ++i )
			{
				string desc;
				if ( i < m_Book.Entries.Count )
					desc = GetName( ( (RunebookEntry) m_Book.Entries[i] ).Description );
				else
					desc = "Empty";

				Intern( desc, false );
			}

			// Next 2 entries are charge / max charge
			Intern( m_Book.CurCharges.ToString(), false );
			Intern( m_Book.MaxCharges.ToString(), false );
			// then old unused "drop rune" etc entries
			Intern( "Drop Rune", true );
			Intern( "Rename Book", true );
			Intern( "Set Default", true );          // 22

			// then location values, one entry has two values

			for ( int i = 0; i < 16; ++i )
			{
				if ( i < m_Book.Entries.Count )
				{
					RunebookEntry e = (RunebookEntry) m_Book.Entries[i];

					// Location labels
					int xLong = 0, yLat = 0;
					int xMins = 0, yMins = 0;
					bool xEast = false, ySouth = false;

					if ( Sextant.Format( e.Location, e.Map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth ) )
					{
						Intern( String.Format( "{0}° {1}'{2}", yLat, yMins, ySouth ? "S" : "N" ), false );
						Intern( String.Format( "{0}° {1}'{2}", xLong, xMins, xEast ? "E" : "W" ), false );
					}
					else
					{
						Intern( "Nowhere", false );
						Intern( "Nowhere", false );
					}
				}
				else
				{
					Intern( "Nowhere", false );
					Intern( "Nowhere", false );
				}
			}
		}

		private Runebook m_Book;

		public Runebook Book { get { return m_Book; } }

		public int GetMapHue( Map map )
		{
			if ( map == Map.Trammel )
				return 10;
			else if ( map == Map.Felucca )
				return 81;
			else if ( map == Map.Ilshenar )
				return 1102;
			else if ( map == Map.Malas )
				return 1102;
			else if ( map == Map.Tokuno )
				return 1154;
			else if ( map == Map.TerMur )
				return 1645;

			return 0;
		}

		public string GetName( string name )
		{
			if ( name == null || ( name = name.Trim() ).Length <= 0 )
				return "(indescript)";

			return name;
		}

		private void AddBackground()
		{
			AddPage( 0 );

			// Background image
			AddImage( 100, 10, 2200 );

			// Two seperators
			for ( int i = 0; i < 2; ++i )
			{
				int xOffset = 125 + ( i * 165 );

				AddImage( xOffset, 50, 57 );
				xOffset += 20;

				for ( int j = 0; j < 6; ++j, xOffset += 15 )
					AddImage( xOffset, 50, 58 );

				AddImage( xOffset - 5, 50, 59 );
			}

			// First four page buttons
			for ( int i = 0, xOffset = 130, gumpID = 2225; i < 4; ++i, xOffset += 35, ++gumpID )
				AddButton( xOffset, 187, gumpID, gumpID, 0, GumpButtonType.Page, 2 + i );

			// Next four page buttons
			for ( int i = 0, xOffset = 300, gumpID = 2229; i < 4; ++i, xOffset += 35, ++gumpID )
				AddButton( xOffset, 187, gumpID, gumpID, 0, GumpButtonType.Page, 6 + i );

			// Charges
			AddHtmlIntern( 140, 40, 80, 18, 0, false, false );						// Charges:	
			AddHtmlIntern( 300, 40, 100, 18, 1, false, false );						// Max Charges:	

			AddHtmlIntern( 220, 40, 30, 18, 18, false, false );						// Charges
			AddHtmlIntern( 400, 40, 30, 18, 19, false, false ); 						// Max charges
		}

		private void AddIndex()
		{
			// Index
			AddPage( 1 );

			// Rename button
			AddButton( 125, 15, 2472, 2473, 1, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 158, 22, 100, 18, 1011299, false, false ); // Rename book

			// List of entries
			List<RunebookEntry> entries = m_Book.Entries;

			for ( int i = 0; i < 16; ++i )
			{
				string desc;
				int hue;

				if ( i < entries.Count )
				{
					desc = GetName( ( (RunebookEntry) entries[i] ).Description );
					hue = GetMapHue( ( (RunebookEntry) entries[i] ).Map );
				}
				else
				{
					desc = "Empty";
					hue = 0;
				}

				// Use charge button
				AddButton( 130 + ( ( i / 8 ) * 160 ), 65 + ( ( i % 8 ) * 15 ), 2103, 2104, 10 + i, GumpButtonType.Reply, 0 );
			}

			for ( int i = 0; i < 16; ++i )
			{
				string desc;
				int hue;

				if ( i < entries.Count )
				{
					desc = GetName( ( (RunebookEntry) entries[i] ).Description );
					hue = GetMapHue( ( (RunebookEntry) entries[i] ).Map );
				}
				else
				{
					desc = "Empty";
					hue = 0;
				}

				// Description label
				AddLabelIntern( 145 + ( ( i / 8 ) * 160 ), 60 + ( ( i % 8 ) * 15 ), hue, i + 2 );
			}

			// Turn page button
			AddButton( 393, 14, 2206, 2206, 0, GumpButtonType.Page, 2 );
		}

		private void AddDetails( int index, int half )
		{
			// Use charge button
			AddButton( 130 + ( half * 160 ), 65, 2103, 2104, 10 + index, GumpButtonType.Reply, 0 );

			string desc;
			int hue;

			if ( index < m_Book.Entries.Count )
			{
				RunebookEntry e = (RunebookEntry) m_Book.Entries[index];

				desc = GetName( e.Description );
				hue = GetMapHue( e.Map );

				// Description label
				AddLabelIntern( 145 + ( half * 160 ), 60, hue, index + 2 );

				// Location labels
				int xLong = 0, yLat = 0;
				int xMins = 0, yMins = 0;
				bool xEast = false, ySouth = false;

				if ( Sextant.Format( e.Location, e.Map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth ) )
				{
					AddLabelIntern( 135 + ( half * 160 ), 80, 0, index + 23 );
					AddLabelIntern( 135 + ( half * 160 ), 95, 0, index + 24 );
				}

				// Drop rune button
				AddButton( 135 + ( half * 160 ), 115, 2437, 2438, 200 + index, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 150 + ( half * 160 ), 115, 100, 18, 1011298, false, false ); // Drop rune

				if ( e != m_Book.Default )
				{
					// Set as default button
					AddButton( 160 + ( half * 140 ), 20, 2361, 2361, 300 + index, GumpButtonType.Reply, 0 );
					AddHtmlLocalized( 175 + ( half * 140 ), 15, 100, 18, 1011300, false, false ); // Set default
				}

				AddButton( 135 + ( half * 160 ), 140, 2103, 2104, 50 + index, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 150 + ( half * 160 ), 136, 110, 20, 1062722, false, false ); // Recall

				AddButton( 135 + ( half * 160 ), 158, 2103, 2104, 100 + index, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 150 + ( half * 160 ), 154, 110, 20, 1062723, false, false ); // Gate Travel

				AddButton( 135 + ( half * 160 ), 176, 2103, 2104, 75 + index, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 150 + ( half * 160 ), 172, 110, 20, 1062724, false, false ); // Sacred Journey
			}
			else
			{
				desc = "Empty";
				hue = 0;
			}
		}

		public RunebookGump( Mobile from, Runebook book )
			: base( 150, 200 )
		{
			m_Book = book;

			PrecompileStringTable();
			AddBackground();
			AddIndex();

			for ( int page = 0; page < 8; ++page )
			{
				AddPage( 2 + page );

				AddButton( 125, 14, 2205, 2205, 0, GumpButtonType.Page, 1 + page );

				if ( page < 7 )
					AddButton( 393, 14, 2206, 2206, 0, GumpButtonType.Page, 3 + page );

				for ( int half = 0; half < 2; ++half )
					AddDetails( ( page * 2 ) + half, half );
			}
		}

		public static bool HasSpell( Mobile from, int spellID )
		{
			Spellbook book = Spellbook.Find( from, spellID );

			return ( book != null && book.HasSpell( spellID ) );
		}

		private class InternalPrompt : Prompt
		{
			// Please enter a title for the runebook:
			public override int MessageCliloc { get { return 502414; } }

			private Runebook m_Book;

			public InternalPrompt( Runebook book )
				: base( book )
			{
				m_Book = book;
			}

			public override void OnResponse( Mobile from, string text )
			{
				if ( m_Book.Deleted || !from.InRange( m_Book.GetWorldLocation(), 1 ) )
					return;

				if ( m_Book.CheckAccess( from ) )
				{
					m_Book.Description = Utility.FixHtml( text.Trim() );

					from.CloseGump( typeof( RunebookGump ) );
					from.SendGump( new RunebookGump( from, m_Book ) );

					from.SendMessage( "The book's title has been changed." );
				}
				else
				{
					from.SendLocalizedMessage( 502416 ); // That cannot be done while the book is locked down.
				}
			}

			public override void OnCancel( Mobile from )
			{
				from.SendLocalizedMessage( 502415 ); // Request cancelled.

				if ( !m_Book.Deleted && from.InRange( m_Book.GetWorldLocation(), 1 ) )
				{
					from.CloseGump( typeof( RunebookGump ) );
					from.SendGump( new RunebookGump( from, m_Book ) );
				}
			}
		}

        public override void OnResponse( NetState state, RelayInfo info)
		{
			Mobile from = state.Mobile;

			if ( m_Book.Deleted || !from.InRange( m_Book.GetWorldLocation(), 1 ) || !DesignContext.Check( from ) )
				return;

			int buttonID = info.ButtonID;

			if ( buttonID == 1 ) // Rename book
			{
				if ( m_Book.CheckAccess( from ) )
					from.Prompt = new InternalPrompt( m_Book );
				else
					from.SendLocalizedMessage( 502413 ); // That cannot be done while the book is locked down.
			}
			else
			{
				int index = buttonID % 25;
				int type = buttonID / 25;

				if ( type == 0 || type == 1 )
					index = buttonID - 10;

				if ( index >= 0 && index < m_Book.Entries.Count )
				{
					RunebookEntry e = (RunebookEntry) m_Book.Entries[index];

					switch ( type )
					{
						case 0:
						case 1: // Use charges
							{
								if ( m_Book.CurCharges <= 0 )
								{
									from.CloseGump( typeof( RunebookGump ) );
									from.SendGump( new RunebookGump( from, m_Book ) );

									from.SendLocalizedMessage( 502412 ); // There are no charges left on that item.
								}
								else
								{
									int xLong = 0, yLat = 0;
									int xMins = 0, yMins = 0;
									bool xEast = false, ySouth = false;

									if ( Sextant.Format( e.Location, e.Map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth ) )
									{
										string location = String.Format( "{0}° {1}'{2}, {3}° {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W" );
										from.SendMessage( location );
									}

									//m_Book.OnTravel();

									new RecallSpell( from, m_Book, e, m_Book ).Cast();
								}

								break;
							}
						case 8: // Drop rune
							{
								if ( m_Book.CheckAccess( from ) )
								{
									m_Book.DropRune( from, e, index );

									from.CloseGump( typeof( RunebookGump ) );
									from.SendGump( new RunebookGump( from, m_Book ) );
								}
								else
								{
									from.SendLocalizedMessage( 502413 ); // That cannot be done while the book is locked down.
								}

								break;
							}
						case 12: // Set default
							{
								if ( m_Book.CheckAccess( from ) )
								{
									m_Book.Default = e;

									from.CloseGump( typeof( RunebookGump ) );
									from.SendGump( new RunebookGump( from, m_Book ) );

									from.SendLocalizedMessage( 502417 ); // New default location set.
								}
								else
								{
									from.SendLocalizedMessage( 502413 ); // That cannot be done while the book is locked down.
								}

								break;
							}
						case 2: // Recall
							{
								if ( HasSpell( from, 31 ) )
								{
									int xLong = 0, yLat = 0;
									int xMins = 0, yMins = 0;
									bool xEast = false, ySouth = false;

									if ( Sextant.Format( e.Location, e.Map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth ) )
									{
										string location = String.Format( "{0}° {1}'{2}, {3}° {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W" );
										from.SendMessage( location );
									}

									//m_Book.OnTravel();
									new RecallSpell( from, null, e, null ).Cast();
								}
								else
								{
									from.SendLocalizedMessage( 500015 ); // You do not have that spell!
								}

								break;
							}
						case 4: // Gate
							{
								if ( HasSpell( from, 51 ) )
								{
									int xLong = 0, yLat = 0;
									int xMins = 0, yMins = 0;
									bool xEast = false, ySouth = false;

									if ( Sextant.Format( e.Location, e.Map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth ) )
									{
										string location = String.Format( "{0}° {1}'{2}, {3}° {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W" );
										from.SendMessage( location );
									}

									//m_Book.OnTravel();
									new GateTravelSpell( from, null, e ).Cast();
								}
								else
								{
									from.SendLocalizedMessage( 500015 ); // You do not have that spell!
								}

								break;
							}
						case 3: // Sacred Journey
							{
								if ( HasSpell( from, 209 ) )
								{
									int xLong = 0, yLat = 0;
									int xMins = 0, yMins = 0;
									bool xEast = false, ySouth = false;

									if ( Sextant.Format( e.Location, e.Map, ref xLong, ref yLat, ref xMins, ref yMins, ref xEast, ref ySouth ) )
									{
										string location = String.Format( "{0}° {1}'{2}, {3}° {4}'{5}", yLat, yMins, ySouth ? "S" : "N", xLong, xMins, xEast ? "E" : "W" );
										from.SendMessage( location );
									}

									//m_Book.OnTravel();
									new SacredJourneySpell( from, null, e, null ).Cast();
								}
								else
								{
									from.SendLocalizedMessage( 500015 ); // You do not have that spell!
								}

								break;
							}

						default:
							break;
					}
				}
			}
		}
	}
}