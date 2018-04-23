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
using Server.Items;
using Server.Gumps;
using Xanthos.Utilities;

namespace Arya.Abay
{
	/// <summary>
	/// Configuration for a new Abay
	/// </summary>
	public class NewAbayGump : Gump
	{
		private Mobile m_User;
		private AbayItem m_Abay;

		public NewAbayGump( Mobile user, AbayItem Abay ) : base( 100, 100 )
		{
			user.CloseGump( typeof( NewAbayGump ) );
			m_User = user;
			m_Abay = Abay;
			MakeGump();
		}

        public static void AddItemCentered(int x, int y, int w, int h, int itemID, int itemHue, Gump gump)
        {
            Rectangle2D r = ItemBounds.Table[itemID];
            gump.AddItem(x + ((w - r.Width) / 2) - r.X, y + ((h - r.Height) / 2) - r.Y, itemID, itemHue );
        }

		private void MakeGump()
		{			
			Closable = false;
			Disposable = true;
			Dragable = true;
			Resizable = false;

			AddPage(0);
			AddBackground( 0, 0, 502, 370, 9270 );
			AddImageTiled(4, 4, 492, 362, 2524);
			AddImageTiled(5, 5, 490, 360, 2624);
			AddAlphaRegion(5, 5, 490, 360);

			// Abay item goes here
			AddItemCentered( 5, 5, 155, 155, m_Abay.Item.ItemID, m_Abay.Item.Hue, this );

			AddImageTiled(159, 5, 20, 184, 2524);
			AddImageTiled(5, 169, 255, 20, 2524);
			AddImageTiled(160, 5, 335, 355, 2624);
			AddImageTiled(5, 170, 155, 195, 2624);
			AddAlphaRegion(160, 5, 335, 360);
			AddAlphaRegion(5, 170, 155, 195);

			AddLabel(250, 10, AbayLabelHue.kRedHue, AbaySystem.ST[ 100 ] );

			// Starting bid: text 0
			AddLabel(170, 35, AbayLabelHue.kLabelHue, AbaySystem.ST[ 68 ] );
			AddImageTiled(254, 34, 72, 22, 2524);
			AddImageTiled(255, 35, 70, 20, 2624);
			AddAlphaRegion(255, 35, 70, 20);
			AddTextEntry(255, 35, 70, 20, AbayLabelHue.kGreenHue, 0, m_Abay.MinBid.ToString( "#,0" ) );

			// Reserve: text 1
			AddLabel(345, 35, AbayLabelHue.kLabelHue, AbaySystem.ST[ 69 ] );
			AddImageTiled(414, 34, 72, 22, 2524);
			AddImageTiled(415, 35, 70, 20, 2624);
			AddAlphaRegion(415, 35, 70, 20);
			AddTextEntry(415, 35, 70, 20, AbayLabelHue.kGreenHue, 1, m_Abay.Reserve.ToString( "#,0" ) );

			// Days duration: text 2
			AddLabel(170, 60, AbayLabelHue.kLabelHue, AbaySystem.ST[ 101 ] );
			AddImageTiled(254, 59, 32, 22, 2524);
			AddImageTiled(255, 60, 30, 20, 2624);
			AddAlphaRegion(255, 60, 30, 20);
			AddTextEntry(255, 60, 30, 20, AbayLabelHue.kGreenHue, 2, m_Abay.Duration.TotalDays.ToString() );
			AddLabel(290, 60, AbayLabelHue.kLabelHue, AbaySystem.ST[ 102 ] );

			// Item name: text 3
			AddLabel(170, 85, AbayLabelHue.kLabelHue, AbaySystem.ST[ 50 ] );
			AddImageTiled(254, 84, 232, 22, 2524);
			AddImageTiled(255, 85, 230, 20, 2624);
			AddAlphaRegion(255, 85, 230, 20);
			AddTextEntry(255, 85, 230, 20, AbayLabelHue.kGreenHue, 3, m_Abay.ItemName );

			// Buy now: Check 0, Text 6
			AddCheck( 165, 110, 2152, 2153, false, 0 );
			AddLabel( 200, 115, AbayLabelHue.kLabelHue, AbaySystem.ST[ 208 ] );
			AddImageTiled( 329, 114, 157, 22, 2524 );
			AddImageTiled( 330, 115, 155, 20, 2624 );
			AddAlphaRegion( 330, 115, 155, 20 );
			AddTextEntry( 330, 115, 155, 20, AbayLabelHue.kGreenHue, 6, "" );

			// Description: text 4
			AddLabel(170, 140, AbayLabelHue.kLabelHue, AbaySystem.ST[ 103 ] );
			AddImageTiled(169, 159, 317, 92, 2524);
			AddImageTiled(170, 160, 315, 90, 2624);
			AddAlphaRegion(170, 160, 315, 90);
			AddTextEntry(170, 160, 315, 90, AbayLabelHue.kGreenHue, 4, m_Abay.Description);

			// Web link: text 5
			AddLabel(170, 255, AbayLabelHue.kLabelHue, AbaySystem.ST[ 104 ] );
			AddImageTiled(224, 274, 262, 22, 2524);
			AddLabel(170, 275, AbayLabelHue.kLabelHue, @"http://");
			AddImageTiled(225, 275, 260, 20, 2624);
			AddAlphaRegion(225, 275, 260, 20);
			AddTextEntry(225, 275, 260, 20, AbayLabelHue.kGreenHue, 5, m_Abay.WebLink );
			
			// Help area
			AddImageTiled(9, 174, 152, 187, 2524);
			AddImageTiled(10, 175, 150, 185, 2624);
			AddAlphaRegion(10, 175, 150, 185);
			AddHtml( 10, 175, 150, 185, AbaySystem.ST[ 105 ] , (bool)false, (bool)true);

			// OK Button: button 1
			AddButton(170, 305, 4023, 4024, 1, GumpButtonType.Reply, 0);
			AddLabel(210, 300, AbayLabelHue.kRedHue, AbaySystem.ST[ 106 ] );
			AddLabel(210, 315, AbayLabelHue.kRedHue, AbaySystem.ST[ 107 ] );

			// Cancel button: button 0
			AddButton(170, 335, 4020, 4020, 0, GumpButtonType.Reply, 0);
			AddLabel(210, 335, AbayLabelHue.kLabelHue, AbaySystem.ST[ 108 ] );
		}

		public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
		{
			if ( ! AbaySystem.Running )
			{
				sender.Mobile.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 15 ] );

				m_Abay.Cancel();

				return;
			}

			bool allowBuyNow = info.Switches.Length > 0; // Just one switch

			switch ( info.ButtonID )
			{
				case 0: // Cancel the Abay

					m_Abay.Cancel();
					m_User.SendGump( new AbayGump( m_User ) );
					break;

				case 1: // Commit the Abay

					// Collect information
					int minbid = 0; // text 0
					int reserve = 0; // text 1
					int days = 0; // text 2
					string name = ""; // text 3
					string description = ""; // text 4
					string weblink = ""; // text 5
					int buynow = 0; // text 6

					// The 3D client sucks

					string[] tr = new string[ 7 ];

					foreach( TextRelay t in info.TextEntries )
					{
						tr[ t.EntryID ] = t.Text;
					}

					try { minbid = (int) uint.Parse( tr[ 0 ], NumberStyles.AllowThousands ); } 
					catch {}

					try { reserve = (int) uint.Parse( tr[ 1 ], NumberStyles.AllowThousands ); }
					catch {}

					try { days = (int) uint.Parse( tr[ 2 ] ); }
					catch {}

					try { buynow = (int) uint.Parse( tr[ 6 ], NumberStyles.AllowThousands ); }
					catch {}

					if ( tr[ 3 ] != null )
					{
						name = tr[ 3 ];
					}

					if ( tr[ 4 ] != null )
					{
						description = tr[ 4 ];
					}

					if ( tr[ 5 ] != null )
					{
						weblink = tr[ 5 ];
					}

					bool ok = true;

					if ( minbid < 1 )
					{
						m_User.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 109 ] );
						ok = false;
					}

					if ( reserve < 1 || reserve < minbid )
					{
						m_User.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 110 ] );
						ok = false;
					}

					if ( days < AbaySystem.MinAbayDays && m_User.AccessLevel < AccessLevel.GameMaster || days < 1 )
					{
						m_User.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 111 ] , AbaySystem.MinAbayDays );
						ok = false;
					}

					if ( days > AbaySystem.MaxAbayDays && m_User.AccessLevel < AccessLevel.GameMaster )
					{
						m_User.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 112 ] , AbaySystem.MaxAbayDays );
						ok = false;
					}

					if ( name.Length == 0 )
					{
						m_User.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 113 ] );
						ok = false;
					}

					if ( minbid * AbayConfig.MaxReserveMultiplier < reserve && m_User.AccessLevel < AccessLevel.GameMaster )
					{
						m_User.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 114 ] );
						ok = false;
					}

					if ( allowBuyNow && buynow <= reserve )
					{
						m_User.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 209 ] );
						ok = false;
					}

					if ( ok && AbayConfig.CostOfAbay > 0.0 )
					{
						int toPay = 0;

						if ( AbayConfig.CostOfAbay <= 1.0 )
							toPay = (int) ( Math.Max( minbid, reserve ) * AbayConfig.CostOfAbay );
						else
							toPay = (int) AbayConfig.CostOfAbay;

						if ( toPay > 0 )
						{
							if ( Server.Mobiles.Banker.Withdraw( m_User, toPay ) )
								m_User.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 228 ], toPay );
							else
							{
								m_User.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 229 ], toPay );
								goto case 0; // Pretty much cancel the Abay
							}
						}
					}

					m_Abay.MinBid = minbid;
					m_Abay.Reserve = reserve;
					m_Abay.ItemName = name;
					m_Abay.Duration = TimeSpan.FromDays( days );					
					m_Abay.Description = description;
					m_Abay.WebLink = weblink;
					m_Abay.BuyNow = allowBuyNow ? buynow : 0;

					if ( ok && AbaySystem.Running )
					{
						m_Abay.Confirm();
						m_User.SendGump( new AbayViewGump( m_User, m_Abay, new AbayGumpCallback( AbayCallback ) ) );
					}
					else if ( AbaySystem.Running )
					{
						m_User.SendGump( new NewAbayGump( m_User, m_Abay ) );
					}
					else
					{
						m_User.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 115 ] );
					}

					break;
			}
		}

		private static void AbayCallback( Mobile user )
		{
			user.SendGump( new AbayGump( user ) );
		}
	}
}
