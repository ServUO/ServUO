//Author plus
using System;
using System.Collections;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Misc;

namespace Server.Gumps
{
	public class KillGump : Gump
	{
		private const int MAX_KILL_LIST_ENTRIES = 500;

		private KillBook m_Book;
		private ArrayList m_KillList;

		public KillGump( Mobile from, KillBook book, ArrayList list ) : base( 100, 100 )
		{
			m_KillList = list;
			m_Book = book;

			m_KillList.Sort();

			AddPage( 0 );

			AddImage(3, 18, 2200); //Book

			AddLabel(88, 27, 196, "My Kill Book");
			AddButton(73, 32, 2103, 2103, 0, GumpButtonType.Reply, 0); //Kill Menu

			//Seperators
			for (int s = 0; s < 10; s++)
			{
				AddImage( 27, 64, 57);
				AddImage(137, 64, 59);

				if ( s < 5 )
					AddImage( 57 + ((s % 5)  * 16), 64, 58);
				else
					AddImage( 218 + ((s % 5) * 16) , 64, 58);

				AddImage(188, 64, 57);
				AddImage(298, 64, 59);
			}

			//Headings
			AddLabel(041, 47, 0, "Name" );
			AddLabel(121, 47, 0, "Kills");
			AddLabel(201, 47, 0, "Name" );
			AddLabel(281, 47, 0, "Kills");

			if ( m_KillList.Count == 0 )
				AddLabel( 35, 82, 0x25, "No victims killed" );

			int count = Math.Min(m_KillList.Count, MAX_KILL_LIST_ENTRIES);
			for (int i = 0, l = 0; i < count && l <= 18; i++, l++ )
			{
				if ( (i % 18) == 0 )
				{
					if ( i != 0 )
						AddButton( 297, 22, 2206, 2206, 0, GumpButtonType.Page, (i / 18 ) + 1 ); //Next page

					if ( i != 0 )
						l = 0;

					AddPage( (i / 18) + 1 );

					if ( i != 0 )
						AddButton( 25, 22, 2205, 2205, 0, GumpButtonType.Page, (i / 18) ); //Previous Page
				}

				DeathEntry o = (DeathEntry)m_KillList[i];

				if (l <= 8)
				{
					AddLabelCropped( 40,  75 + (( l % 9 ) * 15), 100, 17, 0, o.Name );
					AddLabelCropped( 140, 75 + (( l % 9 ) * 15), 60, 17, 0, o.Deaths.ToString() );
				}
				else
				{
					AddLabelCropped( 204, 75 + (( l % 9 ) * 15),  100, 17, 0, o.Name ); //Right
					AddLabelCropped( 294, 75 + (( l % 9 ) * 15),  60, 17, 0, o.Deaths.ToString() );
				}
			}
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;

			switch ( info.ButtonID )
			{
				case 0:
				{
					from.SendGump( new KillIndex( from, m_Book, m_KillList ));
					break;
				}
			}
		}
	}
}