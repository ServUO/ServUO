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

namespace Arya.Abay
{
	/// <summary>
	/// Manages the delivery of messages between players involved in an Abay
	/// </summary>
	public class AbayMessaging
	{
		public static void Initialize()
		{
			EventSink.Login += new LoginEventHandler( OnPlayerLogin );
		}

		private static void OnPlayerLogin( LoginEventArgs e )
		{
			if ( ! AbaySystem.Running )
				return;

			foreach( AbayItem Abay in AbaySystem.Pending )
			{
				Abay.SendMessage( e.Mobile );
			}
		}

		/// <summary>
		/// Sends a message to a mobile to notify them that they have been outbid during an Abay.
		/// 
		/// </summary>
		/// <param name="Abay">The Abay generating the message</param>
		/// <param name="amount">The value of the mobile's bid</param>
		/// <param name="to">The mobile sending to. This can be null or offline. If offline, nothing will be sent.</param>
		public static void SendOutbidMessage( AbayItem Abay, int amount, Mobile to )
		{
			if ( to == null || to.Account == null || to.NetState == null )
				return;

			AbayMessageGump gump = new AbayMessageGump( Abay, true, false, false );
			gump.Message = string.Format( AbaySystem.ST[ 179 ] , amount.ToString("#,0" ) );
			gump.OkText = "Close this message";
			gump.ShowExpiration = false;

			to.SendGump( new AbayNoticeGump( gump ) );
		}

		/// <summary>
		/// Sends the confirmation request for the reserve not met to the Abay owner
		/// </summary>
		/// <param name="item">The Abay</param>
		public static void SendReserveMessageToOwner( AbayItem item )
		{
			if ( item.Owner == null || item.Owner.Account == null || item.Owner.NetState == null )
				return;

			AbayMessageGump gump = new AbayMessageGump( item, false, true, true );
			string msg = string.Format(
				AbaySystem.ST[ 180 ],
				item.HighestBid.Amount, item.Reserve.ToString("#,0" ) );

			if ( ! item.IsValid() )
			{
				msg += AbaySystem.ST[ 181 ];
			}

			gump.Message = msg;
			gump.OkText = AbaySystem.ST[ 182 ];
			gump.CancelText = AbaySystem.ST[ 183 ];

			item.Owner.SendGump( new AbayNoticeGump( gump ) );
		}

		/// <summary>
		/// Sends the information message about the reserve not met to the buyer
		/// </summary>
		public static void SendReserveMessageToBuyer( AbayItem item )
		{
			if ( item.HighestBid.Mobile == null || item.HighestBid.Mobile.Account == null || item.HighestBid.Mobile.NetState == null )
				return;

			AbayMessageGump gump = new AbayMessageGump( item, true, false, true );
			gump.Message = string.Format( AbaySystem.ST[ 184 ],
				AbayConfig.DaysForConfirmation, item.HighestBid.Amount, item.Reserve );

			gump.OkText = AbaySystem.ST[ 185 ];

			item.HighestBid.Mobile.SendGump( new AbayNoticeGump( gump ) );
		}

		/// <summary>
		/// Informs the buyer that some of the items Abayed have been deleted.
		/// </summary>
		public static void SendInvalidMessageToBuyer ( AbayItem item )
		{
			Mobile m = item.HighestBid.Mobile;

			if ( m == null || m.Account == null || m.NetState == null )
				return;

			AbayMessageGump gump = new AbayMessageGump( item, false, false, true );
			string msg = string.Format( AbaySystem.ST[ 186 ], item.HighestBid.Amount.ToString("#,0" ) );

			if ( ! item.ReserveMet )
			{
				msg += AbaySystem.ST[ 187 ];
			}

			gump.Message = msg;
			gump.OkText = AbaySystem.ST[ 188 ];
			gump.CancelText = AbaySystem.ST[ 189 ];

			m.SendGump( new AbayNoticeGump( gump ) );
		}

		/// <summary>
		/// Sends the invalid message to the owner.
		/// </summary>
		/// <param name="gump"></param>
		public static void SendInvalidMessageToOwner( AbayItem item )
		{
			Mobile m = item.Owner;

			if ( m == null || m.Account == null || m.NetState == null )
				return;

			AbayMessageGump gump = new AbayMessageGump( item, true, true, true );
			gump.Message = AbaySystem.ST[ 190 ];
			gump.OkText = AbaySystem.ST[ 185 ];

			m.SendGump( new AbayNoticeGump( gump ) );
		}
	}
}