using System;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Gumps
{
	public class MinerMadnessSlotGump : Gump
	{
		private MinerSlotStone m_Stone;

		public MinerMadnessSlotGump( MinerSlotStone stone ) : base( 25, 25 )
		{
			m_Stone = stone;

			m_Stone.IsInUse = true;

			// Gold Bars = 7147
			// Silver Bars = 7159
			// Bronze Bars = 7141
			// Iron Bars = 7153
			// Sledge Hammer = 4020
			// Smith Hammer = 5091
			// Iron Wire = 6262
			// Pickaxe = 3717

			Closable=true;
			Disposable=true;
			Dragable=true;
			Resizable=false;
			AddPage(0);
			AddBackground(18, 64, 371, 278, 5120);
			AddBackground(32, 193, 345, 100, 2620);
			AddImageTiled(23, 298, 363, 10, 5121);
			AddHtml( 26, 70, 356, 23, @"<BASEFONT COLOR=#FFFFFF><CENTER>Miner Madness</CENTER></BASEFONT>", (bool)false, (bool)false);
			AddImageTiled(37, 200, 335, 86, 9354);
			AddImageTiled(150, 202, 2, 83, 9353);
			AddLabel(30, 100, 1160, @"Cost: " + m_Stone.Cost.ToString() );
			AddLabel(30, 120, 1160, @"Credits: " + m_Stone.Won.ToString() );
			AddLabel(30, 140, 1160, @"Last Pay: " + m_Stone.LastPay.ToString() );
			AddButton(30, 163, 4026, 4027, 1, GumpButtonType.Reply, 0);
			AddLabel(65, 164, 1149, @"View Pay Table");
			AddButton(30, 307, 4020, 4021, 2, GumpButtonType.Reply, 0);
			AddLabel(65, 307, 1149, @"Spin");
			AddButton(118, 307, 4029, 4030, 3, GumpButtonType.Reply, 0);
			AddLabel(153, 307, 1149, @"Cash Out");
			AddImageTiled(60, 205, 75, 75, 9304);
			AddImageTiled(167, 205, 75, 75, 9304);
			AddImageTiled(274, 205, 75, 75, 9304);
			AddImageTiled(258, 202, 2, 83, 9353);

			if ( m_Stone != null )
			{
				if ( m_Stone.ReelOne != 0 )
					AddItem(75, 220, m_Stone.ReelOne );
				else
					AddItem(75, 220, 7147 );

				if ( m_Stone.ReelTwo != 0 )
					AddItem(183, 220, m_Stone.ReelTwo );
				else
					AddItem(183, 220, 7147 );

				if ( m_Stone.ReelThree != 0 )
					AddItem(290, 220, m_Stone.ReelThree );
				else
					AddItem(290, 220, 7147);
			}
		}

      		public override void OnResponse( NetState state, RelayInfo info ) 
      		{ 
			Mobile from = state.Mobile; 

			if ( from == null )
				return;

        		if ( info.ButtonID == 0 ) // Close
         		{
				m_Stone.IsInUse = false;
			}

        		if ( info.ButtonID == 1 ) // Pay Table
         		{
				from.SendGump( new MinerMadnessSlotGump( m_Stone ) );
				from.SendGump( new MinerPayTableGump( m_Stone ) );
			}

        		if ( info.ButtonID == 2 ) // Spin
         		{
		   		Item[] Gold = from.Backpack.FindItemsByType( typeof( Gold ) );
				int amount = m_Stone.Cost * 10000;

		   		if ( from.Backpack.ConsumeTotal( typeof( Gold ), m_Stone.Cost ) )
				{
					m_Stone.DoSpin( from );
					m_Stone.LastPay = 0;
					m_Stone.TotalCollected += m_Stone.Cost;
				}
				else if ( m_Stone.Won >= m_Stone.Cost )
				{
					m_Stone.Won -= m_Stone.Cost;
					m_Stone.DoSpin( from );
					m_Stone.LastPay = 0;
					m_Stone.TotalCollected += m_Stone.Cost;
				}
				else
				{
					from.SendMessage( "You must have at least {0} gold, or credits on the machine to play.", m_Stone.Cost );
					from.SendGump( new MinerMadnessSlotGump( m_Stone ) );
				}
			}

        		if ( info.ButtonID == 3 ) // Cash Out
         		{
				if ( m_Stone.Won != 0 )
					m_Stone.DoCashOut( from );

				from.SendGump( new MinerMadnessSlotGump( m_Stone ) );
			}
		}
	}
}