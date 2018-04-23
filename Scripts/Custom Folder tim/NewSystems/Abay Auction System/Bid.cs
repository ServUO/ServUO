#region AuthorHeader
//
//	Abay version 2.1, by Xanthos and Arya
//
//  Based on original ideas and code by Arya
//
#endregion AuthorHeader
using System;

using Server;

namespace Arya.Abay
{
	/// <summary>
	/// Defines a bid entry
	/// </summary>
	public class Bid
	{
		private Mobile m_Mobile;
		private int m_Amount;
		private DateTime m_Time;

		[ CommandProperty( AccessLevel.Administrator ) ]
		/// <summary>
		/// Gets the mobile who placed the bid
		/// </summary>
		public Mobile Mobile
		{
			get { return m_Mobile; }
		}

		[ CommandProperty( AccessLevel.Administrator ) ]
		/// <summary>
		/// Gets the value of the bid
		/// </summary>
		public int Amount
		{
			get { return m_Amount; }
		}

		[ CommandProperty( AccessLevel.Administrator ) ]
		/// <summary>
		/// Gets the time the bid has been placed at
		/// </summary>
		public DateTime Time
		{
			get { return m_Time; }
		}

		/// <summary>
		/// Creates a new bid
		/// </summary>
		/// <param name="m">The Mobile placing the bid</param>
		/// <param name="amount">The amount of the bid</param>
		public Bid( Mobile m, int amount )
		{
			m_Time = DateTime.UtcNow;
			m_Mobile = m;
			m_Amount = amount;
		}

		/// <summary>
		/// Creates a new bid. Checks if the mobile has enough money to bid
		/// and if so removes the money from the player
		/// </summary>
		/// <param name="from">The mobile bidding</param>
		/// <param name="amount">The amount bid</param>
		/// <returns>A bid object if the mobile has enough money</returns>
		public static Bid CreateBid( Mobile from, int amount )
		{
			if ( Server.Mobiles.Banker.Withdraw( from, amount ) )
			{
				return new Bid( from, amount );
			}
			else
			{
				from.SendMessage( 0x40, AbaySystem.ST[ 199 ] );
				return null;
			}
		}

		private Bid()
		{
		}

		#region Serialization

		public void Serialize( GenericWriter writer )
		{
			// Version 1
			// Version 0
			writer.Write( m_Mobile );
			writer.Write( m_Amount );
			writer.Write( m_Time );
		}

		public static Bid Deserialize( GenericReader reader, int version )
		{
			Bid bid = new Bid();

			switch ( version )
			{
				case 1:
				case 0:
					bid.m_Mobile = reader.ReadMobile();
					bid.m_Amount = reader.ReadInt();
					bid.m_Time = reader.ReadDateTime();
					break;
			}

			return bid;
		}

		#endregion

		/// <summary>
		/// Returns the bid money to the highest bidder because they have been outbid
		/// </summary>
		/// <param name="Abay">The Abay the bid belongs to</param>
		public void Outbid( AbayItem Abay )
		{
			if ( m_Mobile == null || m_Mobile.Account == null )
				return;

			AbayCheck check = new AbayGoldCheck( Abay, AbayResult.Outbid );
			if ( ! this.m_Mobile.Backpack.TryDropItem( m_Mobile, check, false ) )
			{
				m_Mobile.BankBox.DropItem( check );
			}

			// Send notice
			AbayMessaging.SendOutbidMessage( Abay, m_Amount, m_Mobile );
		}

		/// <summary>
		/// Returns the bid money to the bidder because the Abay has been canceled
		/// </summary>
		/// <param name="Abay">The Abay the bid belongs to</param>
		public void AbayCanceled( AbayItem Abay )
		{
			if ( m_Mobile == null )
				return;

			AbayCheck check = new AbayGoldCheck( Abay, AbayResult.SystemStopped );

			if ( m_Mobile.Backpack == null || ! m_Mobile.Backpack.TryDropItem( m_Mobile, check, false ) )
			{
				if ( m_Mobile.BankBox != null )
					m_Mobile.BankBox.DropItem( check );
				else
					check.Delete();
			}
		}

		/// <summary>
		/// Outputs bid information
		/// </summary>
		/// <param name="writer"></param>
		public void Profile( System.IO.StreamWriter writer )
		{
			string owner = null;

			if ( m_Mobile != null && m_Mobile.Account != null )
				owner = string.Format( "{0} [ Account : {1} - Serial {2} ]", m_Mobile.Name, ( m_Mobile.Account as Server.Accounting.Account ).Username, m_Mobile.Serial );
			else
				owner = "None";

			writer.WriteLine( "- {0}\t{1}", m_Amount, owner );
		}
	}
}