using System;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Network;

namespace Server.Gumps
{
	public class MinerBonusGump : Gump
	{
		private MinerSlotStone m_Stone;

		private int m_c1;
		private int m_c2;
		private int m_c3;
		private int m_c4;
		private int m_c5;
		private int m_c6;
		private int m_c7;
		private int m_c8;
		private int m_c9;

		private bool m_b1;
		private bool m_b2;
		private bool m_b3;
		private bool m_b4;
		private bool m_b5;
		private bool m_b6;
		private bool m_b7;
		private bool m_b8;
		private bool m_b9;

		public MinerBonusGump( MinerSlotStone stone, bool b1, bool b2, bool b3, bool b4, bool b5, bool b6, bool b7, bool b8, bool b9, int num1, int num2, int num3, int num4, int num5, int num6, int num7, int num8, int num9 ) : base( 25, 25 )
		{
			m_Stone = stone;


			m_Stone.IsInUse = true;

			int c1 = 0;
			int c2 = 0;
			int c3 = 0;
			int c4 = 0;
			int c5 = 0;
			int c6 = 0;
			int c7 = 0;
			int c8 = 0;
			int c9 = 0;

			if ( b1 == false && b2 == false && b3 == false && b4 == false && b5 == false && b6 == false && b7 == false && b8 == false && b9 == false )
			{
				c1 = Utility.RandomList( 50000, 10000, 10000, 5000, 5000, 5000, 1000, 1000, 1000, 1000, 1000, 500, 500, 500, 500, 500, 500, 500, 500, 100, 100, 100, 100, 100, 100, 100, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 );
				c2 = Utility.RandomList( 50000, 10000, 10000, 5000, 5000, 5000, 1000, 1000, 1000, 1000, 1000, 500, 500, 500, 500, 500, 500, 500, 500, 100, 100, 100, 100, 100, 100, 100, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 );
				c3 = Utility.RandomList( 50000, 10000, 10000, 5000, 5000, 5000, 1000, 1000, 1000, 1000, 1000, 500, 500, 500, 500, 500, 500, 500, 500, 100, 100, 100, 100, 100, 100, 100, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 );
				c4 = Utility.RandomList( 50000, 10000, 10000, 5000, 5000, 5000, 1000, 1000, 1000, 1000, 1000, 500, 500, 500, 500, 500, 500, 500, 500, 100, 100, 100, 100, 100, 100, 100, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 );
				c5 = Utility.RandomList( 50000, 10000, 10000, 5000, 5000, 5000, 1000, 1000, 1000, 1000, 1000, 500, 500, 500, 500, 500, 500, 500, 500, 100, 100, 100, 100, 100, 100, 100, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 );
				c6 = Utility.RandomList( 50000, 10000, 10000, 5000, 5000, 5000, 1000, 1000, 1000, 1000, 1000, 500, 500, 500, 500, 500, 500, 500, 500, 100, 100, 100, 100, 100, 100, 100, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 );
				c7 = Utility.RandomList( 50000, 10000, 10000, 5000, 5000, 5000, 1000, 1000, 1000, 1000, 1000, 500, 500, 500, 500, 500, 500, 500, 500, 100, 100, 100, 100, 100, 100, 100, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 );
				c8 = Utility.RandomList( 50000, 10000, 10000, 5000, 5000, 5000, 1000, 1000, 1000, 1000, 1000, 500, 500, 500, 500, 500, 500, 500, 500, 100, 100, 100, 100, 100, 100, 100, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 );
				c9 = Utility.RandomList( 50000, 10000, 10000, 5000, 5000, 5000, 1000, 1000, 1000, 1000, 1000, 500, 500, 500, 500, 500, 500, 500, 500, 100, 100, 100, 100, 100, 100, 100, 100, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 );
			}
			else
			{
				c1 = num1;
				c2 = num2;
				c3 = num3;
				c4 = num4;
				c5 = num5;
				c6 = num6;
				c7 = num7;
				c8 = num8;
				c9 = num9;
			}

			m_c1 = c1;
			m_c2 = c2;
			m_c3 = c3;
			m_c4 = c4;
			m_c5 = c5;
			m_c6 = c6;
			m_c7 = c7;
			m_c8 = c8;
			m_c9 = c9;

			m_b1 = b1;
			m_b2 = b2;
			m_b3 = b3;
			m_b4 = b4;
			m_b5 = b5;
			m_b6 = b6;
			m_b7 = b7;
			m_b8 = b8;
			m_b9 = b9;



			if ( b1 == true && b2 == true && b2 == true && b3 == true && b4 == true && b5 == true && b6 == true && b7 == true && b8 == true && b9 == true )
			{
				Disposable=true;
				Dragable=true;
				Closable=true;
			}
			else
			{
				Disposable=false;
				Dragable=false;
				Closable=false;
			}

			Resizable=false;
			AddPage(0);
			AddBackground(52, 25, 393, 430, 5120);
			AddImage(59, 63, 5528);
			AddImage(60, 30, 5573);
			AddLabel(123, 26, 1149, @"Pick a location to mine, If you mine up coal you lose.");
			AddLabel(123, 42, 1149, @"You keep mining till you find coal or all are gone.");

			if ( b1 == false )
			{
				AddButton(86, 198, 2117, 2118, 1, GumpButtonType.Reply, 0);
			}
			else
			{
				AddImage(86, 198, 5231);

				if ( c1 != 0 )
					AddLabel(86, 198, 1160, c1.ToString() );
				else
					AddLabel(86, 198, 1160, @"!! Coal !!");
			}

			if ( b2 == false )
			{
				AddButton(156, 158, 2117, 2118, 2, GumpButtonType.Reply, 0);
			}
			else
			{
				AddImage(156, 158, 5231);

				if ( c2 != 0 )
					AddLabel(156, 158, 1160, c2.ToString() );
				else
					AddLabel(156, 158, 1160, @"!! Coal !!" );
			}

			if ( b3 == false )
			{
				AddButton(140, 236, 2117, 2118, 3, GumpButtonType.Reply, 0);
			}
			else
			{
				AddImage(140, 236, 5231);

				if ( c3 != 0 )
					AddLabel(140, 236, 1160, c3.ToString() );
				else
					AddLabel(140, 236, 1160, @"!! Coal !!");
			}

			if ( b4 == false )
			{
				AddButton(205, 85, 2117, 2118, 4, GumpButtonType.Reply, 0);
			}
			else
			{
				AddImage(205, 85, 5231);

				if ( c4 != 0 )
					AddLabel(205, 85, 1160, c4.ToString() );
				else
					AddLabel(205, 85, 1160, @"!! Coal !!");
			}

			if ( b5 == false )
			{
				AddButton(242, 142, 2117, 2118, 5, GumpButtonType.Reply, 0);
			}
			else
			{	
				AddImage(242, 142, 5231);

				if ( c5 != 0 )
					AddLabel(242, 142, 1160, c5.ToString() );
				else
					AddLabel(242, 142, 1160, @"!! Coal !!");
			}

			if ( b6 == false )
			{
				AddButton(359, 98, 2117, 2118, 6, GumpButtonType.Reply, 0);
			}
			else
			{
				AddImage(359, 98, 5231);

				if ( c6 != 0 )
					AddLabel(359, 98, 1160, c6.ToString() );
				else
					AddLabel(359, 98, 1160, @"!! Coal !!");
			}

			if ( b7 == false )
			{
				AddButton(158, 309, 2117, 2118, 7, GumpButtonType.Reply, 0);
			}
			else
			{
				AddImage(158, 309, 5231);

				if ( c7 != 0 )
					AddLabel(158, 309, 1160, c7.ToString() );
				else
					AddLabel(158, 309, 1160, @"!! Coal !!");
			}

			if ( b8 == false )
			{
				AddButton(245, 107, 2117, 2118, 8, GumpButtonType.Reply, 0);
			}
			else
			{
				AddImage(245, 107, 5231);

				if ( c8 != 0 )
					AddLabel(245, 107, 1160, c8.ToString() );
				else
					AddLabel(245, 107, 1160, @"!! Coal !!");
			}

			if ( b9 == false )
			{
				AddButton(399, 363, 2117, 2118, 9, GumpButtonType.Reply, 0);
			}
			else
			{
				AddImage(399, 363, 5231);

				if ( c9 != 0 )
					AddLabel(399, 363, 1160, c9.ToString() );
				else
					AddLabel(399, 363, 1160, @"!! Coal !!");
			}
		}

      		public override void OnResponse( NetState state, RelayInfo info ) 
      		{ 
			Mobile from = state.Mobile; 

			if ( from == null )
				return;

        		if ( info.ButtonID == 0 )
         		{
				from.CloseGump( typeof( MinerMadnessSlotGump ) );
				from.SendGump( new MinerMadnessSlotGump( m_Stone ) );
			}

        		if ( info.ButtonID == 1 )
         		{
				if ( m_c1 == 0 )
				{
					from.SendMessage( "You found coal!" );
					from.SendMessage( "You may now close this menu out to contiune playing." );
					from.SendGump( new MinerBonusGump( m_Stone, true, true, true, true, true, true, true, true, true, m_c1, m_c2, m_c3, m_c4, m_c5, m_c6, m_c7, m_c8, m_c9 ) );
				}
				else
				{
					m_Stone.Won += m_c1;
					from.SendMessage( "You have won {0}, Pick another spot to mine.", m_c1 );
					from.SendGump( new MinerBonusGump( m_Stone, true, m_b2, m_b3, m_b4, m_b5, m_b6, m_b7, m_b8, m_b9, m_c1, m_c2, m_c3, m_c4, m_c5, m_c6, m_c7, m_c8, m_c9 ) );
					m_Stone.LastPay += m_c1;
					m_Stone.TotalCollected -= m_c1;
				}
			}

        		if ( info.ButtonID == 2 )
         		{
				if ( m_c2 == 0 )
				{
					from.SendMessage( "You found coal!" );
					from.SendMessage( "You may now close this menu out to contiune playing." );
					from.SendGump( new MinerBonusGump( m_Stone, true, true, true, true, true, true, true, true, true, m_c1, m_c2, m_c3, m_c4, m_c5, m_c6, m_c7, m_c8, m_c9 ) );
				}
				else
				{
					m_Stone.Won += m_c2;
					from.SendMessage( "You have won {0}, Pick another spot to mine.", m_c2 );
					from.SendGump( new MinerBonusGump( m_Stone, m_b1, true, m_b3, m_b4, m_b5, m_b6, m_b7, m_b8, m_b9, m_c1, m_c2, m_c3, m_c4, m_c5, m_c6, m_c7, m_c8, m_c9 ) );
					m_Stone.LastPay += m_c2;
					m_Stone.TotalCollected -= m_c2;
				}
			}

        		if ( info.ButtonID == 3 )
         		{
				if ( m_c3 == 0 )
				{
					from.SendMessage( "You found coal!" );
					from.SendMessage( "You may now close this menu out to contiune playing." );
					from.SendGump( new MinerBonusGump( m_Stone, true, true, true, true, true, true, true, true, true, m_c1, m_c2, m_c3, m_c4, m_c5, m_c6, m_c7, m_c8, m_c9 ) );
				}
				else
				{
					m_Stone.Won += m_c3;
					from.SendMessage( "You have won {0}, Pick another spot to mine.", m_c3 );
					from.SendGump( new MinerBonusGump( m_Stone, m_b1, m_b2, true, m_b4, m_b5, m_b6, m_b7, m_b8, m_b9, m_c1, m_c2, m_c3, m_c4, m_c5, m_c6, m_c7, m_c8, m_c9 ) );
					m_Stone.LastPay += m_c3;
					m_Stone.TotalCollected -= m_c3;
				}
			}

        		if ( info.ButtonID == 4 )
         		{
				if ( m_c4 == 0 )
				{
					from.SendMessage( "You found coal!" );
					from.SendMessage( "You may now close this menu out to contiune playing." );
					from.SendGump( new MinerBonusGump( m_Stone, true, true, true, true, true, true, true, true, true, m_c1, m_c2, m_c3, m_c4, m_c5, m_c6, m_c7, m_c8, m_c9 ) );
				}
				else
				{
					m_Stone.Won += m_c4;
					from.SendMessage( "You have won {0}, Pick another spot to mine.", m_c4 );
					from.SendGump( new MinerBonusGump( m_Stone, m_b1, m_b2, m_b3, true, m_b5, m_b6, m_b7, m_b8, m_b9, m_c1, m_c2, m_c3, m_c4, m_c5, m_c6, m_c7, m_c8, m_c9 ) );
					m_Stone.LastPay += m_c4;
					m_Stone.TotalCollected -= m_c4;
				}
			}

        		if ( info.ButtonID == 5 )
         		{
				if ( m_c5 == 0 )
				{
					from.SendMessage( "You found coal!" );
					from.SendMessage( "You may now close this menu out to contiune playing." );
					from.SendGump( new MinerBonusGump( m_Stone, true, true, true, true, true, true, true, true, true, m_c1, m_c2, m_c3, m_c4, m_c5, m_c6, m_c7, m_c8, m_c9 ) );
				}
				else
				{
					m_Stone.Won += m_c5;
					from.SendMessage( "You have won {0}, Pick another spot to mine.", m_c5 );
					from.SendGump( new MinerBonusGump( m_Stone, m_b1, m_b2, m_b3, m_b4, true, m_b6, m_b7, m_b8, m_b9, m_c1, m_c2, m_c3, m_c4, m_c5, m_c6, m_c7, m_c8, m_c9 ) );
					m_Stone.LastPay += m_c5;
					m_Stone.TotalCollected -= m_c5;
				}
			}

        		if ( info.ButtonID == 6 )
         		{
				if ( m_c6 == 0 )
				{
					from.SendMessage( "You found coal!" );
					from.SendMessage( "You may now close this menu out to contiune playing." );
					from.SendGump( new MinerBonusGump( m_Stone, true, true, true, true, true, true, true, true, true, m_c1, m_c2, m_c3, m_c4, m_c5, m_c6, m_c7, m_c8, m_c9 ) );
				}
				else
				{
					m_Stone.Won += m_c6;
					from.SendMessage( "You have won {0}, Pick another spot to mine.", m_c6 );
					from.SendGump( new MinerBonusGump( m_Stone, m_b1, m_b2, m_b3, m_b4, m_b5, true, m_b7, m_b8, m_b9, m_c1, m_c2, m_c3, m_c4, m_c5, m_c6, m_c7, m_c8, m_c9 ) );
					m_Stone.LastPay += m_c6;
					m_Stone.TotalCollected -= m_c6;
				}
			}

        		if ( info.ButtonID == 7 )
         		{
				if ( m_c7 == 0 )
				{
					from.SendMessage( "You found coal!" );
					from.SendMessage( "You may now close this menu out to contiune playing." );
					from.SendGump( new MinerBonusGump( m_Stone, true, true, true, true, true, true, true, true, true, m_c1, m_c2, m_c3, m_c4, m_c5, m_c6, m_c7, m_c8, m_c9 ) );
				}
				else
				{
					m_Stone.Won += m_c7;
					from.SendMessage( "You have won {0}, Pick another spot to mine.", m_c7 );
					from.SendGump( new MinerBonusGump( m_Stone, m_b1, m_b2, m_b3, m_b4, m_b5, m_b6, true, m_b8, m_b9, m_c1, m_c2, m_c3, m_c4, m_c5, m_c6, m_c7, m_c8, m_c9 ) );
					m_Stone.LastPay += m_c7;
					m_Stone.TotalCollected -= m_c7;
				}
			}

        		if ( info.ButtonID == 8 )
         		{
				if ( m_c8 == 0 )
				{
					from.SendMessage( "You found coal!" );
					from.SendMessage( "You may now close this menu out to contiune playing." );
					from.SendGump( new MinerBonusGump( m_Stone, true, true, true, true, true, true, true, true, true, m_c1, m_c2, m_c3, m_c4, m_c5, m_c6, m_c7, m_c8, m_c9 ) );
				}
				else
				{
					m_Stone.Won += m_c8;
					from.SendMessage( "You have won {0}, Pick another spot to mine.", m_c8 );
					from.SendGump( new MinerBonusGump( m_Stone, m_b1, m_b2, m_b3, m_b4, m_b5, m_b6, m_b7, true, m_b9, m_c1, m_c2, m_c3, m_c4, m_c5, m_c6, m_c7, m_c8, m_c9 ) );
					m_Stone.LastPay += m_c8;
					m_Stone.TotalCollected -= m_c8;
				}
			}

        		if ( info.ButtonID == 9 )
         		{
				if ( m_c9 == 0 )
				{
					from.SendMessage( "You found coal!" );
					from.SendMessage( "You may now close this menu out to contiune playing." );
					from.SendGump( new MinerBonusGump( m_Stone, true, true, true, true, true, true, true, true, true, m_c1, m_c2, m_c3, m_c4, m_c5, m_c6, m_c7, m_c8, m_c9 ) );
				}
				else
				{
					m_Stone.Won += m_c9;
					from.SendMessage( "You have won {0}, Pick another spot to mine.", m_c9 );
					from.SendGump( new MinerBonusGump( m_Stone, m_b1, m_b2, m_b3, m_b4, m_b5, m_b6, m_b7, m_b8, true, m_c1, m_c2, m_c3, m_c4, m_c5, m_c6, m_c7, m_c8, m_c9 ) );
					m_Stone.LastPay += m_c9;
					m_Stone.TotalCollected -= m_c9;
				}
			}
		}
	}
}