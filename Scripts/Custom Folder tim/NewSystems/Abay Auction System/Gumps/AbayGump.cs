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
	/// The main gump for the Abay house
	/// </summary>
	public class AbayGump : Gump
	{
		public AbayGump( Mobile user ) : base( 50, 50 )
		{
			user.CloseGump( typeof( AbayGump ) );
			MakeGump();
		}

		private void MakeGump()
		{
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage(0);
			AddImageTiled(49, 39, 402, 197, 3004);
			AddImageTiled(50, 40, 400, 195, 2624);
			AddAlphaRegion(50, 40, 400, 195);
			AddImage(165, 65, 10452);
			AddImage(-1, 20, 10400);
			AddImage(-1, 185, 10402);
			AddImage(35, 20, 10420);
			AddImage(421, 20, 10410);
			AddImage(410, 20, 10430);
			AddImageTiled(90, 32, 323, 16, 10254);
			AddImage(420, 185, 10412);

			AddLabel(160, 45, 151, AbaySystem.ST[ 8 ] );

			// Create new Abay: B1
			AddLabel(100, 130, AbayLabelHue.kLabelHue, AbaySystem.ST[ 9 ] );
			AddButton(60, 130, 4005, 4006, 1, GumpButtonType.Reply, 0);

			// View all Abays: B2
			AddLabel(285, 130, AbayLabelHue.kLabelHue, AbaySystem.ST[ 10 ] );
			AddButton(245, 130, 4005, 4006, 2, GumpButtonType.Reply, 0);

			// View your Abays: B3
			AddLabel(100, 165, AbayLabelHue.kLabelHue, AbaySystem.ST[ 11 ] );
			AddButton(60, 165, 4005, 4006, 3, GumpButtonType.Reply, 0);

			// View your bids: B4
			AddLabel(285, 165, AbayLabelHue.kLabelHue, AbaySystem.ST[ 12 ] );
			AddButton(245, 165, 4005, 4006, 4, GumpButtonType.Reply, 0);

			// View pendencies: B5
			AddButton( 60, 200, 4005, 4006, 5, GumpButtonType.Reply, 0 );
			AddLabel( 100, 200, AbayLabelHue.kLabelHue, AbaySystem.ST[ 13 ] );

			// Exit: B0
			AddLabel(285, 205, AbayLabelHue.kLabelHue, AbaySystem.ST[ 14 ] );
			AddButton(245, 200, 4017, 4018, 0, GumpButtonType.Reply, 0);
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
				case 0: // Exit
					break;

				case 1: // Create Abay
					AbaySystem.AbayRequest( sender.Mobile );
					break;

				case 2: // View all Abays
					sender.Mobile.SendGump( new AbayListing( sender.Mobile, AbaySystem.Abays, true, true ) );
					break;

				case 3: // View your Abays

					sender.Mobile.SendGump( new AbayListing( sender.Mobile, AbaySystem.GetAbays( sender.Mobile ), true, true ) );
					break;

				case 4: // View your bids

					sender.Mobile.SendGump( new AbayListing( sender.Mobile, AbaySystem.GetBids( sender.Mobile ), true, true ) );
					break;

				case 5: // View pendencies

					sender.Mobile.SendGump( new AbayListing( sender.Mobile, AbaySystem.GetPendencies( sender.Mobile ), true, true ) );
					break;
			}
		}

	}
}