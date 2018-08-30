#region AuthorHeader
//
//	Abay version 2.1, by Xanthos and Arya
//
//  Based on original ideas and code by Arya
//
#endregion AuthorHeader
using System;
using System.Collections;

using Server;
using Server.Gumps;
using Xanthos.Utilities;

namespace Arya.Abay
{
	/// <summary>
	/// Summary description for BidViewGump.
	/// </summary>
	public class BidViewGump : Gump
	{
		private AbayGumpCallback m_Callback;
		private int m_Page;
		private ArrayList m_Bids;

		public BidViewGump( Mobile m, ArrayList bids, AbayGumpCallback callback ) : this ( m, bids, callback, 0 )
		{
		}

		public BidViewGump( Mobile m, ArrayList bids, AbayGumpCallback callback, int page ) : base( 100, 100 )
		{
			m.CloseGump( typeof( BidViewGump ) );
			m_Callback = callback;
			m_Page = page;
			m_Bids = new ArrayList( bids );
			MakeGump();
		}

		private void MakeGump()
		{
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			int numOfPages = ( m_Bids.Count - 1 ) / 10 + 1;

			if ( m_Bids.Count == 0 )
				numOfPages = 0;

			AddPage(0);
			AddImageTiled(0, 0, 297, 282, 5174);
			AddImageTiled(1, 1, 295, 280, 2702);
			AddAlphaRegion(1, 1, 295, 280);
			AddLabel(12, 5, AbayLabelHue.kRedHue, AbaySystem.ST[ 86 ] );

			AddLabel(160, 5, AbayLabelHue.kGreenHue, string.Format( AbaySystem.ST[ 18 ] , m_Page + 1, numOfPages ) );
			AddImageTiled(10, 30, 277, 221, 5174);
			AddImageTiled(11, 31, 39, 19, 9274);
			AddAlphaRegion(11, 31, 39, 19);
			AddImageTiled(51, 31, 104, 19, 9274);
			AddAlphaRegion(51, 31, 104, 19);
			AddLabel(55, 30, AbayLabelHue.kGreenHue, AbaySystem.ST[ 87 ] );
			AddImageTiled(156, 31, 129, 19, 9274);
			AddAlphaRegion(156, 31, 129, 19);
			AddLabel(160, 30, AbayLabelHue.kGreenHue, AbaySystem.ST[ 88 ] );

			for ( int i = 0; i < 10; i++ )
			{
				AddImageTiled(11, 51 + i * 20, 39, 19, 9264);
				AddAlphaRegion(11, 51 + i * 20, 39, 19);
				AddImageTiled(51, 51 + i * 20, 104, 19, 9264);
				AddAlphaRegion(51, 51 + i * 20, 104, 19);
				AddImageTiled(156, 51 + i * 20, 129, 19, 9264);
				AddAlphaRegion(156, 51 + i * 20, 129, 19);

				if ( m_Page * 10 + i < m_Bids.Count )
				{
					Bid bid = m_Bids[ m_Page * 10 + i ] as Bid;
					AddLabel(15, 50 + i * 20, AbayLabelHue.kLabelHue, ( m_Page * 10 + i + 1 ).ToString() );
					AddLabelCropped( 55, 50 + i * 20, 100, 19, AbayLabelHue.kLabelHue, bid.Mobile != null ? bid.Mobile.Name : AbaySystem.ST[ 78 ] );
					AddLabel(160, 50 + i * 20, AbayLabelHue.kLabelHue, bid.Amount.ToString("#,0" ));
				}
			}

			AddButton(10, 255, 4011, 4012, 0, GumpButtonType.Reply, 0);
			AddLabel(48, 257, AbayLabelHue.kLabelHue, AbaySystem.ST[ 89 ] );
			
			// PREV PAGE: 1
			if ( m_Page > 0 )
			{
				AddButton(250, 8, 9706, 9707, 1, GumpButtonType.Reply, 0);
			}

			// NEXT PAGE: 2
			if ( m_Page < numOfPages - 1 )
			{
				AddButton(270, 8, 9702, 9703, 2, GumpButtonType.Reply, 0);
			}
		}

		public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
		{
			if ( ! AbaySystem.Running )
			{
				sender.Mobile.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 15 ] );
				return;
			}

			switch ( info.ButtonID )
			{
				case 0:

					if ( m_Callback != null )
					{
						try { m_Callback.DynamicInvoke( new object[] { sender.Mobile } ); }
						catch {}
					}
					break;

				case 1:

					sender.Mobile.SendGump( new BidViewGump( sender.Mobile, m_Bids, m_Callback, m_Page - 1 ) );
					break;

				case 2:

					sender.Mobile.SendGump( new BidViewGump( sender.Mobile, m_Bids, m_Callback, m_Page + 1 ) );
					break;
			}
		}
	}
}
