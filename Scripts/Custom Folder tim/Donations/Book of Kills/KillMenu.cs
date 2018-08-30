//Author plus
using System;
using Server;
using Server.Network;
using Server.Gumps;
using Server.Items;
using System.Collections;
using Server.Mobiles;

namespace Server.Gumps
{
	public class KillIndex : Gump
	{
		private KillBook m_Book;

		private ArrayList m_KillList;

		public KillIndex (Mobile from, KillBook book, ArrayList list ): base( 100, 100 )
		{
			m_Book = book;
			m_KillList = list;

			PlayerMobile m = from as PlayerMobile;

			from.CloseGump( typeof( KillIndex ));

			AddPage(0);

			AddImage(3, 18, 2200); //Book
			AddImage(63, 89, 9804); //Skull
			AddLabel(70, 132, 136, "Kill Book");

			AddLabel(204, 32, 0, "Kill List");
			AddButton(271, 34, 5601, 5605, 1, GumpButtonType.Reply, 0);

			//Seperators
			for (int s = 0; s < 5; s++)
			{
				AddImage ( 216 + (s * 16), 53, 58);
				AddImage(186, 53, 57);
				AddImage(296, 53, 59);
			}

			if ( from == m_Book.BookOwner && m_Book.BookOwner != null )
				Owner( m );
			else
				Spectator( m_Book.BookOwner, m_Book.TotKills, m_Book.TotDeaths );
		}

		private void Owner( PlayerMobile m )
		{
			int pointpos = 145;

			AddImage(186, 75, 2103);
			AddLabel(206, 70, 0, "A Book Of Kills For");
			AddLabel(206, 87, 0, m_Book.BookOwner.Name);

			AddImage(186, 111, 2103);
			AddLabel(204, 105, 0, (String.Format( "You Have {0} Kill{1}", m_Book.TotKills, m_Book.TotKills == 1 ? "" : "s")));

			AddImage(186, 131, 2103);
			AddLabel(204, 125, 0, (String.Format( "You Have {0} Death{1}", m_Book.TotDeaths, m_Book.TotDeaths == 1 ? "" : "s")));
			if ( KillBook.Pointsys )
			{
				//int points = m.Points; //Comment out this line if you don't have nox's point system.
				AddImage(186, 151, 2103);
				//AddLabel(204, 145, 0, (String.Format( "You Have {0} Point{1}", points, points == 1 ? "" : "s")));//And this line
			}

			AddImage(186, (KillBook.Pointsys ? 172 : 151), 2103);
			AddLabel(205, (KillBook.Pointsys ? 167 : pointpos), 0, "You Have Killed "+m_KillList.Count);
			AddLabel(205, (KillBook.Pointsys ? 185 : pointpos + 18), 0, (String.Format( "Player{0}", m_KillList.Count == 1 ? "" : "s")));
		}

		private void Spectator( Mobile from, int kills, int deaths )
		{
			PlayerMobile m = (PlayerMobile)from;

			int pointpos = 145;

			AddImage(186, 75, 2103);
			AddLabel(206, 70, 0, "A Book Of Kills For");
			AddLabel(206, 87, 0, from.Name);

			string gender = from.Female ? "She" : "He";

			AddImage(186, 111, 2103);
			AddLabel(204, 105, 0, (String.Format( "{0} Has {1} Kill{2}", gender, kills, kills == 1 ? "" : "s")));

			AddImage(186, 131, 2103);
			AddLabel( 204, 125, 0, (String.Format( "{0} Has {1} Death{2}", gender, deaths, deaths == 1 ? "" : "s")));

			if ( KillBook.Pointsys )
			{
				//int points = m.Points;//Comment out this line if you don't have nox's point system.
				AddImage(186, 151, 2103);
				//AddLabel( 204, 145, 0, (String.Format( "{0} Has {1} Point{2}", gender, points, points == 1 ? "" : "s")));//And this line
			}

			AddImage(186, (KillBook.Pointsys ? 172 : 151), 2103);
			AddLabel(205, (KillBook.Pointsys ? 167 : pointpos), 0, gender+" Has Killed "+m_KillList.Count);
			AddLabel(205, (KillBook.Pointsys ? 185 : pointpos + 18), 0, (String.Format( "Player{0}", m_KillList.Count == 1 ? "" : "s")));
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;

			switch ( info.ButtonID )
			{
				case 1:
				{
					from.CloseGump( typeof(KillIndex) );
					from.SendGump( new KillGump( from, m_Book, m_KillList ) );
					break;
				}
			}
		}
	}
}