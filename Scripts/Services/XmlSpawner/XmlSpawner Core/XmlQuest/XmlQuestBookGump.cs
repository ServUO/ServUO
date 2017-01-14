using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Prompts;
using Server.Engines.XmlSpawner2;

//
// XmlQuestBookGump
// modified from RC0 BOBGump.cs
//
namespace Server.Gumps
{
	public class XmlQuestBookGump : Gump
	{
		private PlayerMobile m_From;
		private XmlQuestBook m_Book;
		private ArrayList m_List;

		private int m_Page;

		private const int LabelColor = 0x7FFF;

		public int GetIndexForPage( int page )
		{
			int index = 0;

			while ( page-- > 0 )
				index += GetCountForIndex( index );

			return index;
		}

		public int GetCountForIndex( int index )
		{
			int slots = 0;
			int count = 0;

			ArrayList list = m_List;

			for ( int i = index; i >= 0 && i < list.Count; ++i )
			{
				object obj = list[i];

					int add;

					add = 1;

					if ( (slots + add) > 10 )
						break;

					slots += add;

				++count;
			}

			return count;
		}


		public XmlQuestBookGump( PlayerMobile from, XmlQuestBook book ) : this( from, book, 0, null )
		{
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
            if(info == null || m_From == null) return;

			switch ( info.ButtonID )
			{
				case 0: // EXIT
				{
					break;
				}

				case 2: // Previous page
				{
					if ( m_Page > 0 )
						m_From.SendGump( new XmlQuestBookGump( m_From, m_Book, m_Page - 1, m_List ) );

					return;
				}
				case 3: // Next page
				{
					if ( GetIndexForPage( m_Page + 1 ) < m_List.Count )
						m_From.SendGump( new XmlQuestBookGump( m_From, m_Book, m_Page + 1, m_List ) );

					break;
				}


				default:
				{
				    if ( info.ButtonID >= 2000 )
                    {
                        int index = info.ButtonID - 2000;

                        if ( index < 0 || index >= m_List.Count )
    						break;

                        if(m_List[index] is IXmlQuest)
                        {
                            IXmlQuest o = m_List[index] as IXmlQuest;
    
                            if(o != null && !o.Deleted){
                                m_From.SendGump( new XmlQuestBookGump( m_From, m_Book, m_Page, null ) );
                                m_From.CloseGump( typeof( XmlQuestStatusGump ) );
                                m_From.SendGump( new XmlQuestStatusGump(o, o.TitleString, 320, 0, true) );
                            }
                        } 
                    } else
                    if ( info.ButtonID >= 1000 )
                    {

                        int index = info.ButtonID - 1000;

    					if ( index < 0 || index >= m_List.Count )
    						break;

                        // allow quests to be dropped from books that are either in the world or in the players backpack
  						if ( m_Book.IsChildOf( m_From.Backpack ) || (m_Book.Parent == null))
  						{
  						    // move the item from the book to the players backpack
  							Item item = m_List[index] as Item;
  
  							if ( item != null && !item.Deleted)
  							{
  								m_From.AddToBackpack( item );

  								m_From.SendGump( new XmlQuestBookGump( m_From, m_Book, m_Page, null ) );
  
  							}
  							else
  							{
  								m_From.SendMessage( "Internal error. The quest could not be retrieved." );
  							}
    					}
					}

					break;
				}
			}
		}


		public XmlQuestBookGump( PlayerMobile from, XmlQuestBook book, int page, ArrayList list ) : base( 12, 24 )
		{
			from.CloseGump( typeof( XmlQuestBookGump ) );

			m_From = from;
			m_Book = book;
			m_Page = page;

			if ( list == null )
			{
			    // make a new list based on the number of items in the book
                int nquests = 0;

                Item [] questitems = book.FindItemsByType(typeof(IXmlQuest));

                if(questitems != null)
                    nquests = questitems.Length;

				list = new ArrayList( nquests );

				for ( int i = 0; i < nquests; ++i )
				{
					list.Add( questitems[i] );
				}
			}

			m_List = list;

			int index = GetIndexForPage( page );
			int count = GetCountForIndex( index );

			int tableIndex = 0;

			int width = 600;

			width = 516;

			X = (624 - width) / 2;
			
			int xoffset = 0;
			if(m_Book.Locked)
			 xoffset = 20;

			AddPage( 0 );

			AddBackground( 10, 10, width, 439, 5054 );
			AddImageTiled( 18, 20, width - 17, 420, 2624 );

			AddImageTiled( 58 - xoffset, 64, 36, 352, 200 ); // open
			AddImageTiled( 96 - xoffset, 64, 163, 352, 1416 );  // name
			AddImageTiled( 261 - xoffset, 64, 55, 352, 200 ); // type
			AddImageTiled( 308 - xoffset, 64, 85, 352, 1416 );  // status
			AddImageTiled( 395 - xoffset, 64, 116, 352, 200 );  // expires

			for ( int i = index; i < (index + count) && i >= 0 && i < list.Count; ++i )
			{
				object obj = list[i];

				AddImageTiled( 24, 94 + (tableIndex * 32), 489, 2, 2624 );

				++tableIndex;
			}

			AddAlphaRegion( 18, 20, width - 17, 420 );
			AddImage( 5, 5, 10460 );
			AddImage( width - 15, 5, 10460 );
			AddImage( 5, 424, 10460 );
			AddImage( width - 15, 424, 10460 );

			AddHtmlLocalized( 224, 32, 200, 32, 1046026, LabelColor, false, false ); // Quest Log
			AddHtmlLocalized( 63 - xoffset, 64, 200, 32, 3000362, LabelColor, false, false ); // Open
			AddHtmlLocalized( 147 - xoffset, 64, 200, 32, 3005104, LabelColor, false, false ); // Name
			AddHtmlLocalized( 270 - xoffset, 64, 200, 32, 1062213, LabelColor, false, false ); // Type
			AddHtmlLocalized( 326 - xoffset, 64, 200, 32, 3000132, LabelColor, false, false ); // Status
			AddHtmlLocalized( 429 - xoffset, 64, 200, 32, 1062465, LabelColor, false, false ); // Expires

			AddButton( 375 - xoffset, 416, 4017, 4018, 0, GumpButtonType.Reply, 0 );

			AddHtmlLocalized( 410 - xoffset, 416, 120, 20, 1011441, LabelColor, false, false ); // EXIT
			if(!m_Book.Locked)
                AddHtmlLocalized( 26, 64, 50, 32, 1062212, LabelColor, false, false ); // Drop

			tableIndex = 0;

			if ( page > 0 )
			{
				AddButton( 75, 416, 4014, 4016, 2, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 110, 416, 150, 20, 1011067, LabelColor, false, false ); // Previous page
			}

			if ( GetIndexForPage( page + 1 ) < list.Count )
			{
				AddButton( 225, 416, 4005, 4007, 3, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 260, 416, 150, 20, 1011066, LabelColor, false, false ); // Next page
			}

			for ( int i = index; i < (index + count) && i >= 0 && i < list.Count; ++i )
			{
				object obj = list[i];


				if ( obj is IXmlQuest )
				{
					IXmlQuest e = (IXmlQuest)obj;

					int y = 96 + (tableIndex++ * 32);

                    if(!m_Book.Locked)
                        AddButton( 35, y + 2, 5602, 5606, 1000 + i, GumpButtonType.Reply, 0 ); // drop

                    AddButton( 60 - xoffset, y + 2, 0xFAB, 0xFAD, 2000 + i, GumpButtonType.Reply, 0 ); // open gump


                    int color;

                    if(!e.IsValid)
                    {
                        color = 33;
                    } else
                    if(e.IsCompleted)
                    {
                        color = 67;
                    } else
                    {
                        color = 5;
                    }


					AddLabel( 100 - xoffset, y, color, (string)e.Name );

					//AddHtmlLocalized( 315, y, 200, 32, e.IsCompleted ? 1049071 : 1049072, htmlcolor, false, false ); // Completed/Incomplete
					AddLabel( 315 - xoffset, y, color, e.IsCompleted ? "Completed" : "In Progress" );

					// indicate the expiration time
                    if(e.IsValid){

                        // do a little parsing of the expiration string to fit it in the space
                        string substring = e.ExpirationString;
                        if(e.ExpirationString.IndexOf("Expires in") >= 0)
                        {
                            substring = e.ExpirationString.Substring(11);
                        }
                        AddLabel( 400 - xoffset, y, color, (string)substring );
                    } else
                    {
                    	AddLabel( 400 - xoffset, y, color, "No longer valid" );
                    }

                    if(e.PartyEnabled){

                        AddLabel( 270 - xoffset, y, color, "Party" );
                        //AddHtmlLocalized( 250, y, 200, 32, 3000332, htmlcolor, false, false ); // Party
                    } else {

                        AddLabel( 270 - xoffset, y, color, "Solo" );
                    }
				}
			}
		}
	}
}
