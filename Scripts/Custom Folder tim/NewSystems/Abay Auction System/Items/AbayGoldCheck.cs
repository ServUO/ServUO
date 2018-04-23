#region AuthorHeader
//
//	Abay version 2.1, by Xanthos and Arya
//
//  Based on original ideas and code by Arya
//
#endregion AuthorHeader
using System;
using Server;
using Server.Items;
using Arya.Savings;

namespace Arya.Abay
{
	/// <summary>
	/// Summary description for AbayGoldCheck.
	/// </summary>
	public class AbayGoldCheck : AbayCheck
	{
		private static int OutbidHue = 2107;
		private static int SoldHue = 2125;

		private int m_GoldAmount;

		/// <summary>
		/// Creates a check delivering gold for the Abay system
		/// </summary>
		/// <param name="Abay">The Abay originating this check</param>
		/// <param name="result">Specifies the reason for the creation of this check</param>
		public AbayGoldCheck( AbayItem Abay, AbayResult result )
		{
			Name = AbaySystem.ST[ 122 ];
			m_Abay = Abay.ID;
			m_ItemName = Abay.ItemName;

			if ( result != AbayResult.BuyNow )
				m_GoldAmount = Abay.HighestBid.Amount;
			else
				m_GoldAmount = Abay.BuyNow;

			switch ( result )
			{
				case AbayResult.Outbid:
				case AbayResult.SystemStopped:
				case AbayResult.PendingRefused:
				case AbayResult.ReserveNotMet:
				case AbayResult.PendingTimedOut:
				case AbayResult.StaffRemoved:
				case AbayResult.ItemDeleted:

					m_Owner = Abay.HighestBid.Mobile;
					Hue = OutbidHue;

					switch ( result )
					{
						case AbayResult.Outbid :
							m_Message = string.Format( AbaySystem.ST[ 123 ] , m_ItemName, m_GoldAmount.ToString( "#,0" ));
							break;

						case AbayResult.SystemStopped:
							m_Message = string.Format( AbaySystem.ST[ 124 ] , m_ItemName, m_GoldAmount.ToString( "#,0" ) );
							break;

						case AbayResult.PendingRefused:
							m_Message = string.Format( AbaySystem.ST[ 125 ] , m_ItemName ) ;
							break;

						case AbayResult.ReserveNotMet:
							m_Message = string.Format( AbaySystem.ST[ 126 ] , m_GoldAmount.ToString( "#,0" ), m_ItemName );
							break;

						case AbayResult.PendingTimedOut:
							m_Message = AbaySystem.ST[ 127 ] ;
							break;

						case AbayResult.ItemDeleted:
							m_Message = AbaySystem.ST[ 128 ] ;
							break;

						case AbayResult.StaffRemoved:
							m_Message = AbaySystem.ST[ 202 ];
							break;
					}
					break;

				case AbayResult.PendingAccepted:
				case AbayResult.Succesful:
				case AbayResult.BuyNow:

					m_Owner = Abay.Owner;
					Hue = SoldHue;
					m_Message = string.Format( AbaySystem.ST[ 129 ] , m_ItemName, m_GoldAmount.ToString( "#,0" ) );
					break;

				default:

					throw new Exception( string.Format( "{0} is not a valid reason for an Abay gold check", result.ToString() ) );
			}
		}

		public AbayGoldCheck( Serial serial ) : base( serial )
		{
		}

		public override string ItemName
		{
			get
			{
				return string.Format( "{0} Gold Coins", m_GoldAmount.ToString( "#,0" ));
			}
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1060659, "Gold\t{0}", m_GoldAmount.ToString( "#,0" ));
		}

		public override bool Deliver( Mobile to )
		{
			if ( Delivered )
				return true;

			if ( !SavingsAccount.DepositGold( m_Owner, m_GoldAmount ) && !Server.Mobiles.Banker.Deposit( m_Owner, m_GoldAmount ) )
			{
				m_Owner.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 212 ] );
				return false;
			}
			else	// Success
			{				
				DeliveryComplete();
				Delete();
				m_Owner.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 117 ] );
				return true;
			}
		}

		#region Serialization

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize (writer);

			writer.Write( 0 ); // Version
			
			writer.Write( m_GoldAmount );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize (reader);

			int version = reader.ReadInt();

			m_GoldAmount = reader.ReadInt();
		}

		#endregion
	}
}
