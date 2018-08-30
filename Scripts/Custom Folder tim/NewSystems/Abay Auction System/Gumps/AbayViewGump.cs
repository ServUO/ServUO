#region AuthorHeader
//
//	Abay version 2.1, by Xanthos and Arya
//
//  Based on original ideas and code by Arya
//
#endregion AuthorHeader
using System;
using System.Collections;
using System.Globalization;
using Server;
using Server.Gumps;
using Xanthos.Utilities;

namespace Arya.Abay
{
	/// <summary>
	/// This gump displays the general information about an Abay
	/// </summary>
	public class AbayViewGump : Gump
	{
		private const int kHueExampleID = 7107;
		private const int kBeigeBorderOuter = 2524;
		private const int kBeigeBorderInner = 2624;

		private Mobile m_User;
		private AbayItem m_Abay;
		private int m_Page = 0;
		private AbayGumpCallback m_Callback;

		public AbayViewGump( Mobile user, AbayItem Abay ) : this( user, Abay, null )
		{
		}

		public AbayViewGump( Mobile user, AbayItem Abay, AbayGumpCallback callback ) : this( user, Abay, callback, 0 )
		{
		}

		public AbayViewGump( Mobile user, AbayItem Abay, AbayGumpCallback callback, int page ) : base( 50, 50 )
		{
			m_Page = page;
			m_User = user;
			m_Abay = Abay;
			m_Callback = callback;

			MakeGump();
		}

		/// <summary>
		/// Gets the item hue
		/// </summary>
		/// <param name="item">The item to get the hue of</param>
		/// <returns>A positive hue value</returns>
		private int GetItemHue( Item item )
		{
			if ( null == item )
				return 0;

			int hue = item.Hue == 1 ? AbayConfig.BlackHue : item.Hue;

			hue &= 0x7FFF;	// Some hues are | 0x8000 for some reason, but it leads to the same hue

			// Validate in case the hue was shifted by some other value

			return ( hue < 0 || hue >= 3000 ) ? 0 : hue;
		}

		private void MakeGump()
		{
			AbayItem.ItemInfo item = m_Abay[ m_Page ];

			if ( item == null )
				return;

			int itemHue = GetItemHue( item.Item );

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage( 0 );

			// The page and background
			AddBackground( 0, 0, 502, 370, 9270 );
			AddImageTiled( 4, 4, 492, 362, kBeigeBorderOuter );
			AddImageTiled( 5, 5, 490, 360, kBeigeBorderInner );
			AddAlphaRegion( 5, 5, 490, 360);

			//
			// The item display area
			//
			AddImageTiled( 4, 4, 156, 170, kBeigeBorderOuter );
			AddImageTiled( 5, 5, 154, 168, kBeigeBorderInner );
			AddAlphaRegion( 5, 5, 154, 168);

			// Item image goes here
			if ( item.Item != null )
			{
                NewAbayGump.AddItemCentered(5, 5, 155, 140, item.Item.ItemID, item.Item.Hue, this);
				AddItemProperty( item.Item.Serial);
			}
			// Hue preview image goes here if the item has a hue
			if ( item.Item != null && 0 != itemHue )
			{
				AddImageTiled( 30, 140, 107, 24, 3004 );
				AddImageTiled( 31, 141, 105, 22, kBeigeBorderInner );
				AddAlphaRegion( 31, 141, 105, 22 );
				AddLabel( 37, 142, AbayLabelHue.kLabelHue, AbaySystem.ST[ 82 ]  );
				AddItem( 90, 141, kHueExampleID, itemHue );
			}

			//
			// The Abay info area
			//
			AddImageTiled( 4, 169, 156, 196, kBeigeBorderOuter );
			AddImageTiled( 5, 170, 154, 195, kBeigeBorderInner );
			AddAlphaRegion( 5, 170, 154, 195);

			// Reserve and bids
			AddLabel( 10, 175, AbayLabelHue.kLabelHue, AbaySystem.ST[ 68 ] );
			AddLabel( 45, 190, AbayLabelHue.kGreenHue, m_Abay.MinBid.ToString( "#,0" ) );

			AddLabel( 10, 280, AbayLabelHue.kLabelHue, AbaySystem.ST[ 69 ] );
			AddLabel( 45, 295, m_Abay.ReserveMet ? AbayLabelHue.kGreenHue : AbayLabelHue.kRedHue, m_Abay.ReserveMet ? "Met" : "Not Met" );

			AddLabel( 10, 210, AbayLabelHue.kLabelHue, AbaySystem.ST[ 70 ] );

			if ( m_Abay.HasBids )
				AddLabel( 45, 225, m_Abay.ReserveMet ? AbayLabelHue.kGreenHue : AbayLabelHue.kRedHue, m_Abay.HighestBid.Amount.ToString("#,0" ));
			else
				AddLabel( 45, 225, AbayLabelHue.kRedHue, AbaySystem.ST[ 71 ]  );

			// Time remaining
			string timeleft = null;

			AddLabel( 10, 245, AbayLabelHue.kLabelHue, AbaySystem.ST[ 56 ] );

			if ( ! m_Abay.Expired )
			{
				if ( m_Abay.TimeLeft >= TimeSpan.FromDays( 1 ) )
					timeleft = string.Format( AbaySystem.ST[ 73 ] , m_Abay.TimeLeft.Days, m_Abay.TimeLeft.Hours );
				else if ( m_Abay.TimeLeft >= TimeSpan.FromMinutes( 60 ) )
					timeleft = string.Format( AbaySystem.ST[ 74 ] , m_Abay.TimeLeft.Hours );
				else if ( m_Abay.TimeLeft >= TimeSpan.FromSeconds( 60 ) )
					timeleft = string.Format( AbaySystem.ST[ 75 ] , m_Abay.TimeLeft.Minutes );
				else
					timeleft = string.Format( AbaySystem.ST[ 76 ] , m_Abay.TimeLeft.Seconds );
			}
			else if ( m_Abay.Pending )
			{
				timeleft = AbaySystem.ST[ 77 ] ;
			}
			else
			{
				timeleft = AbaySystem.ST[ 78 ] ;
			}
			AddLabel( 45, 260, AbayLabelHue.kGreenHue, timeleft );
			
			// Bidding
			if ( m_Abay.CanBid( m_User ) && ! m_Abay.Expired )
			{
				AddLabel( 10, 318, AbayLabelHue.kLabelHue, AbaySystem.ST[ 79 ] );
				AddImageTiled( 9, 338, 112, 22, kBeigeBorderOuter );
				AddImageTiled( 10, 339, 110, 20, kBeigeBorderInner );
				AddAlphaRegion( 10, 339, 110, 20 );
				
				// Bid text: 0
				AddTextEntry( 10, 339, 110, 20, AbayLabelHue.kGreenHue, 0, @"" );
   
				// Bid button: 4
				AddButton( 125, 338, 4011, 4012, 4, GumpButtonType.Reply, 0 );
			}
			else if ( m_Abay.IsOwner( m_User ) )
			{
				// View bids: button 5
				AddLabel( 10, 338, AbayLabelHue.kLabelHue, AbaySystem.ST[ 80 ]  );
				AddButton( 125, 338, 4011, 4012, 5, GumpButtonType.Reply, 0 );
			}

			//
			// Item properties area
			//
			AddImageTiled( 169, 29, 317, 142, kBeigeBorderOuter );
			AddImageTiled( 170, 30, 315, 140, kBeigeBorderInner );
			AddAlphaRegion( 170, 30, 315, 140 );

			// If it is a container make room for the arrows to navigate to each of the items
			if ( m_Abay.ItemCount > 1 )
			{
				AddLabel( 170, 10, AbayLabelHue.kGreenHue, string.Format( AbaySystem.ST[ 231 ] , m_Abay.ItemName ));

				AddImageTiled( 169, 29, 317, 27, kBeigeBorderOuter );
				AddImageTiled( 170, 30, 315, 25, kBeigeBorderInner );
				AddAlphaRegion( 170, 30, 315, 25 );
				AddLabel( 185, 35, AbayLabelHue.kGreenHue, string.Format( AbaySystem.ST[ 67 ] , m_Page + 1, m_Abay.ItemCount ) );

				// Prev Item button: 1
				if ( m_Page > 0 )
				{
					AddButton( 415, 31, 4014, 4015, 1, GumpButtonType.Reply, 0 );
				}

				// Next Item button: 2
				if ( m_Page < m_Abay.ItemCount - 1 )
				{
					AddButton( 450, 31, 4005, 4006, 2, GumpButtonType.Reply, 0 );
				}

				//AddHtml( 173, 56, 312, 114, m_Abay[ m_Page ].Properties, (bool)false, (bool)true );
				AddHtml( 173, 56, 312, 114, "Hover your mouse over the item to the left to see this item's properties.", (bool)false, (bool)true );
			}
			else
			{
				AddLabel( 170, 10, AbayLabelHue.kGreenHue, m_Abay.ItemName );
				//AddHtml( 173, 30, 312, 140, m_Abay[ m_Page ].Properties, (bool)false, (bool)true );
				AddHtml( 173, 30, 312, 140,  "Hover your mouse over the item to the left to see this item's properties.", (bool)false, (bool)true );
			}
			
			//
			// Owner description area
			//
			AddImageTiled( 169, 194, 317, 112, kBeigeBorderOuter );
			AddLabel( 170, 175, AbayLabelHue.kLabelHue, AbaySystem.ST[ 81 ] );
			AddImageTiled( 170, 195, 315, 110, kBeigeBorderInner );
			AddAlphaRegion( 170, 195, 315, 110 );
			AddHtml( 173, 195, 312, 110, string.Format( "<basefont color=#FFFFFF>{0}", m_Abay.Description ), (bool)false, (bool)true);
			
			// Web link button: 3
			if ( m_Abay.WebLink != null && m_Abay.WebLink.Length > 0 )
			{
				AddLabel( 350, 175, AbayLabelHue.kLabelHue, AbaySystem.ST[ 72 ] );
				AddButton( 415, 177, 5601, 5605, 3, GumpButtonType.Reply, 0 );
			}

			//
			// Abay controls
			//

			// Buy now : Button 8
			if ( m_Abay.AllowBuyNow && m_Abay.CanBid( m_User ) && !m_Abay.Expired )
			{
				AddButton( 170, 310, 4029, 4030, 8, GumpButtonType.Reply, 0 );
				AddLabel( 205, 312, AbayLabelHue.kGreenHue, string.Format( AbaySystem.ST[ 210 ], m_Abay.BuyNow.ToString( "#,0" )));
			}

			// Button 6 : Admin Abay Panel
			if ( m_User.AccessLevel >= AbayConfig.AbayAdminAcessLevel )
			{
				AddButton( 170, 338, 4011, 4012, 6, GumpButtonType.Reply, 0 );
				AddLabel( 205, 340, AbayLabelHue.kLabelHue, AbaySystem.ST[ 227 ] );
			}

			// Close button: 0
			AddButton( 455, 338, 4017, 4018, 0, GumpButtonType.Reply, 0 );
			AddLabel( 415, 340, AbayLabelHue.kLabelHue, AbaySystem.ST[ 7 ] );
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			if ( ! AbaySystem.Running )
			{
				sender.Mobile.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 15 ] );
				return;
			}

			if ( !AbaySystem.Abays.Contains( m_Abay ) )
			{
				sender.Mobile.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 207 ] );
				
				if ( m_Callback != null )
				{
					try { m_Callback.DynamicInvoke( new object[] { m_User } ) ; }
					catch {}
				}

				return;
			}

			switch ( info.ButtonID )
			{
				case 0: // Close

					if ( m_Callback != null )
					{
						try { m_Callback.DynamicInvoke( new object[] { m_User } ); }
						catch {}
					}
					break;

				case 1: // Prev item

					m_User.SendGump( new AbayViewGump( m_User, m_Abay, m_Callback, m_Page - 1 ) );
					break;

				case 2: // Next item

					m_User.SendGump( new AbayViewGump( m_User, m_Abay, m_Callback, m_Page + 1 ) );
					break;

				case 3: // Web link

					m_User.SendGump( new AbayViewGump( m_User, m_Abay, m_Callback, m_Page ) );
					m_Abay.SendLinkTo( m_User );
					break;

				case 4: // Bid

					uint bid = 0;

					try { bid = uint.Parse( info.TextEntries[ 0 ].Text, NumberStyles.AllowThousands ); }
					catch {}

					if ( m_Abay.Expired )
					{
						m_User.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 84 ] );
					}
					else if ( bid == 0 )
					{
						m_User.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 85 ]  );
					}
					else
					{
						if ( m_Abay.AllowBuyNow && bid >= m_Abay.BuyNow )
						{
							// Do buy now instead
							goto case 8;
						}
						else
						{
							m_Abay.PlaceBid( m_User, (int) bid );
						}
					}

					m_User.SendGump( new AbayViewGump( m_User, m_Abay, m_Callback, m_Page ) );
					break;

				case 5: // View bids

					m_User.SendGump( new BidViewGump( m_User, m_Abay.Bids, new AbayGumpCallback( BidViewCallback ) ) );
					break;

				case 6: // Staff Panel

					m_User.SendGump( new AbayControlGump( m_User, m_Abay, this ) );
					break;

				case 8: // Buy Now

					if ( m_Abay.DoBuyNow( sender.Mobile ) )
					{
						goto case 0; // Close the gump
					}
					else
					{
						sender.Mobile.SendGump( new AbayViewGump( sender.Mobile, m_Abay, m_Callback, m_Page ) );
					}
					break;
			}
		}

		private void BidViewCallback( Mobile m )
		{
			m.SendGump( new AbayViewGump( m, m_Abay, m_Callback, m_Page ) );
		}
	}
}