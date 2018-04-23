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
	public class AbaySortGump : Server.Gumps.Gump
	{
		private bool m_Search;
		private bool m_ReturnToAbay;
		private ArrayList m_List;

		public AbaySortGump( Mobile m, ArrayList items, bool returnToAbay, bool search ) : base( 50, 50 )
		{
			m.CloseGump( typeof( AbaySortGump ) );

			m_List = items;
			m_ReturnToAbay = returnToAbay;
			m_Search = search;

			MakeGump();
		}

		private void MakeGump()
		{
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage(0);

			AddImageTiled(49, 34, 402, 312, 3004);
			AddImageTiled(50, 35, 400, 310, 2624);
			AddAlphaRegion(50, 35, 400, 310);

			AddImage(165, 65, 10452);
			AddImage(0, 20, 10400);
			AddImage(0, 280, 10402);
			AddImage(35, 20, 10420);
			AddImage(421, 20, 10410);
			AddImage(410, 20, 10430);
			AddImageTiled(90, 32, 323, 16, 10254);
			AddLabel(160, 45, AbayLabelHue.kGreenHue, AbaySystem.ST[ 49 ] );

			AddLabel(95, 125, AbayLabelHue.kRedHue, AbaySystem.ST[ 50 ] );
			AddImage(75, 125, 2511);
			
			AddButton(110, 144, 9702, 9703, 1, GumpButtonType.Reply, 0);

			AddLabel(135, 141, AbayLabelHue.kLabelHue, AbaySystem.ST[ 51 ] );

			AddButton(110, 163, 9702, 9703, 2, GumpButtonType.Reply, 0);

			AddLabel(135, 160, AbayLabelHue.kLabelHue, AbaySystem.ST[ 52 ] );

			AddImage(420, 280, 10412);
			AddLabel(95, 185, AbayLabelHue.kRedHue, AbaySystem.ST[ 53 ] );
			AddImage(75, 185, 2511);

			AddButton(110, 204, 9702, 9703, 3, GumpButtonType.Reply, 0);

			AddLabel(135, 201, AbayLabelHue.kLabelHue, AbaySystem.ST[ 54 ] );

			AddButton(110, 223, 9702, 9703, 4, GumpButtonType.Reply, 0);

			AddLabel(135, 220, AbayLabelHue.kLabelHue, AbaySystem.ST[ 55 ] );

			AddLabel(95, 245, AbayLabelHue.kRedHue, AbaySystem.ST[ 56 ] );
			AddImage(75, 245, 2511);

			AddButton(110, 264, 9702, 9703, 5, GumpButtonType.Reply, 0);

			AddLabel(135, 261, AbayLabelHue.kLabelHue, AbaySystem.ST[ 57 ] );

			AddButton(110, 283, 9702, 9703, 6, GumpButtonType.Reply, 0);

			AddLabel(135, 280, AbayLabelHue.kLabelHue, AbaySystem.ST[ 58 ] );

			AddLabel(290, 125, AbayLabelHue.kRedHue, AbaySystem.ST[ 59 ] );
			AddImage(270, 125, 2511);

			AddButton(305, 144, 9702, 9703, 7, GumpButtonType.Reply, 0);

			AddLabel(330, 141, AbayLabelHue.kLabelHue, AbaySystem.ST[ 60 ] );

			AddButton(305, 163, 9702, 9703, 8, GumpButtonType.Reply, 0);

			AddLabel(330, 160, AbayLabelHue.kLabelHue, AbaySystem.ST[ 61 ] );

			AddLabel(290, 185, AbayLabelHue.kRedHue, AbaySystem.ST[ 62 ] );
			AddImage(270, 185, 2511);
			
			AddButton(305, 204, 9702, 9703, 9, GumpButtonType.Reply, 0);

			AddLabel(330, 201, AbayLabelHue.kLabelHue, AbaySystem.ST[ 63 ] );

			AddButton(305, 223, 9702, 9703, 10, GumpButtonType.Reply, 0);

			AddLabel(330, 220, AbayLabelHue.kLabelHue, AbaySystem.ST[ 64 ] );

			AddLabel(290, 245, AbayLabelHue.kRedHue, AbaySystem.ST[ 65 ] );
			AddImage(270, 245, 2511);

			AddButton(305, 264, 9702, 9703, 11, GumpButtonType.Reply, 0);

			AddLabel(330, 261, AbayLabelHue.kLabelHue, AbaySystem.ST[ 63 ] );

			AddButton(305, 283, 9702, 9703, 12, GumpButtonType.Reply, 0);

			AddLabel(330, 280, AbayLabelHue.kLabelHue, AbaySystem.ST[ 64 ] );

			AddLabel(120, 315, AbayLabelHue.kLabelHue, AbaySystem.ST[ 66 ] );
			AddButton(80, 315, 4017, 4018, 0, GumpButtonType.Reply, 0);
		}

		public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
		{
			if ( ! AbaySystem.Running )
			{
				sender.Mobile.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 15 ]  );
				return;
			}

			AbayComparer cmp = null;

			switch( info.ButtonID )
			{
				case 1: // Name

					cmp = new AbayComparer( AbaySorting.Name, true );
					break;

				case 2:

					cmp = new AbayComparer( AbaySorting.Name, false );
					break;

				case 3:

					cmp = new AbayComparer( AbaySorting.Date, true );
					break;

				case 4:

					cmp = new AbayComparer( AbaySorting.Date, false );
					break;

				case 5:

					cmp = new AbayComparer( AbaySorting.TimeLeft, true );
					break;

				case 6:

					cmp = new AbayComparer( AbaySorting.TimeLeft, false );
					break;

				case 7:

					cmp = new AbayComparer( AbaySorting.Bids, true );
					break;

				case 8:

					cmp = new AbayComparer( AbaySorting.Bids, false );
					break;

				case 9:

					cmp = new AbayComparer( AbaySorting.MinimumBid, true );
					break;

				case 10:

					cmp = new AbayComparer( AbaySorting.MinimumBid, false );
					break;

				case 11:

					cmp = new AbayComparer( AbaySorting.HighestBid, true );
					break;

				case 12:

					cmp = new AbayComparer( AbaySorting.HighestBid, false );
					break;
			}

			if ( cmp != null )
			{
				m_List.Sort( cmp );
			}

			sender.Mobile.SendGump( new AbayListing( sender.Mobile, m_List, m_Search, m_ReturnToAbay ) );
		}
	}
}