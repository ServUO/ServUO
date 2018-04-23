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
	/// Summary description for AbayMessageGump.
	/// </summary>
	public class AbayMessageGump : Gump
	{
		/// <summary>
		/// Sets the message displayed by the gump in the details area
		/// </summary>
		public string Message
		{
			set
			{
				m_HtmlMessage = string.Format( "<basefont color=#111111>{0}", value );
			}
		}

		/// <summary>
		/// Sets the text associated with the OK button
		/// </summary>
		public string OkText
		{
			set { m_OkText = value; }
		}

		/// <summary>
		/// Sets the text associated with the Cancel button
		/// </summary>
		public string CancelText
		{
			set { m_CancelText = value; }
		}

		/// <summary>
		/// Specifies whether this gump carries just information. If true, the gump will only have an OK button.
		/// If false the gump will have both OK and Cancel buttons.
		/// </summary>
		public bool InformationMode
		{
			set { m_InformationMode = value; }
		}

		/// <summary>
		/// Gets or sets the Abay referenced by this message
		/// </summary>
		public AbayItem Abay
		{
			get
			{
				return AbaySystem.Find( m_ID );
			}
			set
			{
				m_ID = value.ID;
			}
		}

		/// <summary>
		/// Specifies if this message is targeted at the Abay owner, rather than at bidder
		/// </summary>
		public bool OwnerTarget
		{
			set { m_OwnerTarget = value; }
		}

		/// <summary>
		/// Specifies whether this message should validate the answer with the Abay
		/// </summary>
		public bool VerifyAbay
		{
			set { m_VerifyAbay = value; }
		}

		/// <summary>
		/// Specifies whether to show the expiration notice at the bottom of the message
		/// </summary>
		public bool ShowExpiration
		{
			set { m_ShowExpiration = value; }
		}

		private Guid m_ID;
		private string m_HtmlMessage;
		private string m_OkText;
		private string m_CancelText;
		private bool m_InformationMode;
		private bool m_OwnerTarget;
		private bool m_VerifyAbay;
		private bool m_ShowExpiration = true;

		public AbayMessageGump( AbayItem abay, bool informationMode, bool ownerTarget, bool verifyAbay ) : base( 50, 50 )
		{
			Abay = abay;
			m_InformationMode = informationMode;
			m_OwnerTarget = ownerTarget;
			m_VerifyAbay = verifyAbay;
		}

		public void SendTo( Mobile m )
		{
			MakeGump();
			m.SendGump( this );
		}

		private void MakeGump()
		{
			Closable = false;
			Disposable = true;
			Dragable = false;
			Resizable = false;

			AddPage(0);
			AddImage(0, 0, 9380);
			AddImageTiled(114, 0, 335, 140, 9381);
			AddImage(449, 0, 9382);
			AddImageTiled(0, 140, 115, 153, 9383);
			AddImageTiled(114, 140, 336, 217, 9384);
			AddImageTiled(449, 140, 102, 217, 9385);
			AddImage(0, 290, 9386);
			AddImageTiled(114, 290, 353, 140, 9387);
			AddImage(450, 290, 9388);
			AddLabel(200, 38, 76, AbaySystem.ST[ 25 ] );
			AddImageTiled(65, 65, 438, 11, 2091);

			AddLabel(65, 85, 0, AbaySystem.ST[ 26 ] );

			AbayItem abay = Abay;

			// BUTTON 0: View Abay details
			if ( abay != null )
			{
				AddLabel(125, 85, AbayLabelHue.kRedHue, abay.ItemName );

				AddButton(65, 112, 9762, 9763, 0, GumpButtonType.Reply, 0);
				AddLabel(85, 110, AbayLabelHue.kLabelHue, AbaySystem.ST[ 27 ] );
			}				
			else
			{
				AddLabel( 125, 85, AbayLabelHue.kRedHue, AbaySystem.ST[ 28 ] );
			}
			
			AddHtml( 75, 170, 413, 120, m_HtmlMessage, (bool)true, (bool)false);
			AddLabel(75, 150, AbayLabelHue.kLabelHue, AbaySystem.ST[ 29 ] );

			// BUTTON 1: OK
			// BUTTON 2: CANCEL

			if ( m_InformationMode )
			{
				// Information mode: show only OK button at bottom with the OK text
				AddButton( 45, 345, 1147, 1149, 1, GumpButtonType.Reply, 0 );
				AddLabel( 125, 355, AbayLabelHue.kLabelHue, m_OkText );
			}
			else
			{
				AddButton(45, 300, 1147, 1149, 1, GumpButtonType.Reply, 0);
				AddLabel(125, 310, AbayLabelHue.kLabelHue, m_OkText);
				AddButton(45, 345, 1144, 1146, 2, GumpButtonType.Reply, 0);
				AddLabel(125, 355, AbayLabelHue.kLabelHue, m_CancelText);
			}

			if ( m_ShowExpiration && abay != null )
			{
				AddLabel( 55, 405, AbayLabelHue.kRedHue, 
					string.Format( AbaySystem.ST[ 30 ] ,
					Abay.PendingTimeLeft.Days, abay.PendingTimeLeft.Hours ) );
			}
		}

		private void ResendMessage( Mobile m )
		{
			m.SendGump( this );
		}

		public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
		{
			if ( ! AbaySystem.Running )
			{
				sender.Mobile.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 15 ] );
				return;
			}

			AbayItem abay = Abay;

			if ( abay == null )
			{
				sender.Mobile.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 31 ] );
				return;
			}

			switch ( info.ButtonID )
			{
				case 0: // View Abay details

					if ( m_InformationMode && ! m_VerifyAbay )
						// This is an outbid message, no need to return after visiting the Abay
						sender.Mobile.SendGump( new AbayViewGump( sender.Mobile, Abay ) );
					else
						sender.Mobile.SendGump( new AbayViewGump( sender.Mobile, Abay, new AbayGumpCallback( ResendMessage ) ) );
					break;

				case 1: // OK

					if ( m_InformationMode )
					{
						if ( m_VerifyAbay )
						{
							abay.ConfirmInformationMessage( m_OwnerTarget );
						}
					}
					else
					{
						abay.ConfirmResponseMessage( m_OwnerTarget, true );
					}
					break;

				case 2: // Cancel

					abay.ConfirmResponseMessage( m_OwnerTarget, false );
					break;
			}
		}
	}
}
