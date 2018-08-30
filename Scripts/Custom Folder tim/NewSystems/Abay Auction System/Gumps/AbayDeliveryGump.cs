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
	/// This gump is used to deliver the Abay checks
	/// </summary>
	public class AbayDeliveryGump : Gump
	{
		private AbayCheck m_Check;

		public AbayDeliveryGump( AbayCheck check ) : base( 100, 100 )
		{
			m_Check = check;

			MakeGump();
		}

		private void MakeGump()
		{
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage(0);
			AddImage(0, 0, 2080);
			AddImageTiled(18, 37, 263, 245, 2081);
			AddImage(20, 280, 2083);
			AddLabel(75, 5, 210, AbaySystem.ST[ 0 ] );
			AddLabel(45, 35, 0, AbaySystem.ST[ 1 ] );

			int goldHue = 0;
			int itemHue = 0;

			if ( m_Check is AbayGoldCheck )
			{
				// Delivering gold
				goldHue = 143;
				itemHue = 730;
				AddImage(200, 39, 2530);
				AddLabel(70, 220, AbayLabelHue.kLabelHue, AbaySystem.ST[ 2 ] );
			}
			else
			{
				// Delivering an item
				goldHue = 730;
				itemHue = 143;
				AddImage(135, 39, 2530);
				AddLabel(70, 220, AbayLabelHue.kLabelHue, AbaySystem.ST[ 3 ] );
			}

			AddLabel(145, 35, itemHue, AbaySystem.ST[ 4 ] );
			AddLabel(210, 35, goldHue, AbaySystem.ST[ 5 ] );

			AddImage(45, 60, 2091);
			AddImage(45, 100, 2091);

			// Item name
			AddLabelCropped( 55, 75, 200, 20, AbayLabelHue.kLabelHue, m_Check.ItemName );

			AddHtml( 45, 115, 215, 100, m_Check.HtmlDetails, (bool)false, (bool)false);

			// Button 1 : Place in bank
			AddButton(45, 223, 5601, 5605, 1, GumpButtonType.Reply, 0);

			// Button 2 : View Abay
			if ( m_Check.Abay != null )
			{
				AddButton(45, 243, 5601, 5605, 2, GumpButtonType.Reply, 0);
				AddLabel(70, 240, AbayLabelHue.kLabelHue, AbaySystem.ST[ 6 ] );
			}

			// Button 0 : Close
			AddButton(45, 263, 5601, 5605, 0, GumpButtonType.Reply, 0);
			AddLabel(70, 260, AbayLabelHue.kLabelHue, AbaySystem.ST[ 7 ] );

			AddImage(225, 240, 9004);
		}

		public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
		{
			switch ( info.ButtonID )
			{
				case 1: // Place in bank

					if ( !m_Check.Deliver( sender.Mobile ))
					{
						sender.Mobile.SendGump( new AbayDeliveryGump( m_Check ));
					}
					break;

				case 2: // View Abay

					if ( m_Check.Abay != null )
					{
						sender.Mobile.SendGump( new AbayViewGump( sender.Mobile, m_Check.Abay, null ));
					}
					else
					{
						sender.Mobile.SendGump( new AbayDeliveryGump( m_Check ));
					}
					break;
			}
		}
	}
}