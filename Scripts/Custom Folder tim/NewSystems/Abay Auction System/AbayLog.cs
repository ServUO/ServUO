#region AuthorHeader
//
//	Abay version 2.1, by Xanthos and Arya
//
//  Based on original ideas and code by Arya
//
#endregion AuthorHeader
using System;
using System.IO;

using Server;

namespace Arya.Abay
{
	/// <summary>
	/// Summary description for AbayLog.
	/// </summary>
	public class AbayLog
	{
		private static StreamWriter m_Writer;
		private static bool m_Enabled = false;

		public static void Initialize()
		{
			if ( AbaySystem.Running && AbayConfig.EnableLogging )
			{
				// Create the log writer
				try
				{
					string folder = Path.Combine( Core.BaseDirectory, @"Logs\Abay" );

					if ( ! Directory.Exists( folder ) )
						Directory.CreateDirectory( folder );

					string name = string.Format( "{0}.txt", DateTime.UtcNow.ToLongDateString() );
					string file = Path.Combine( folder, name );

					m_Writer = new StreamWriter( file, true );
					m_Writer.AutoFlush = true;

					m_Writer.WriteLine( "###############################" );
					m_Writer.WriteLine( "# {0} - {1}", DateTime.UtcNow.ToShortDateString(), DateTime.UtcNow.ToShortTimeString() );
					m_Writer.WriteLine();
					
					m_Enabled = true;
				}
				catch ( Exception err )
				{
					Console.WriteLine( "Couldn't initialize Abay system log. Reason:" );
					Console.WriteLine( err.ToString() );
					m_Enabled = false;
				}
			}
		}

		/// <summary>
		/// Records the creation of a new Abay item
		/// </summary>
		/// <param name="Abay">The new Abay</param>
		public static void WriteNewAbay( AbayItem Abay )
		{
			if ( !m_Enabled || m_Writer == null )
				return;

			try
			{
				m_Writer.WriteLine( "## New Abay : {0}", Abay.ID );
				m_Writer.WriteLine( "# {0}", Abay.ItemName );
				m_Writer.WriteLine( "# Created on {0} at {1}", DateTime.UtcNow.ToShortDateString(), DateTime.UtcNow.ToShortTimeString() );
				m_Writer.WriteLine( "# Owner : {0} [{1}] Account: {2}", Abay.Owner.Name, Abay.Owner.Serial.ToString(), Abay.Account.Username );
				m_Writer.WriteLine( "# Expires on {0} at {1}", Abay.EndTime.ToShortDateString(), Abay.EndTime.ToShortTimeString() );
				m_Writer.WriteLine( "# Starting Bid: {0}. Reserve: {1}. Buy Now: {2}",
					Abay.MinBid, Abay.Reserve, Abay.AllowBuyNow ? Abay.BuyNow.ToString() : "Disabled" );
				m_Writer.WriteLine( "# Owner Description : {0}", Abay.Description );
				m_Writer.WriteLine( "# Web Link : {0}", Abay.WebLink != null ? Abay.WebLink : "N/A" );
			
				if ( Abay.Creature )
				{
					// Selling a pet
					m_Writer.WriteLine( "#### Selling 1 Creature" );
					m_Writer.WriteLine( "# Type : {0}. Serial : {1}. Name : {2} Hue : {3}", Abay.Pet.GetType().Name, Abay.Pet.Serial.ToString(), Abay.Pet.Name != null ? Abay.Pet.Name : "Unnamed", Abay.Pet.Hue );
					m_Writer.WriteLine( "# Statuette Serial : {0}", Abay.Item.Serial.ToString() );
					m_Writer.WriteLine( "# Properties: {0}", Abay.Items[ 0 ].Properties );
				}
				else
				{
					// Selling items
					m_Writer.WriteLine( "#### Selling {0} Items", Abay.ItemCount );

					for ( int i = 0; i < Abay.ItemCount; i++ )
					{
						AbayItem.ItemInfo info = Abay.Items[ i ];
						m_Writer.WriteLine( "# {0}. {1} [{2}] Type {3} Hue {4}", i, info.Name, info.Item.Serial, info.Item.GetType().Name, info.Item.Hue );
						m_Writer.WriteLine( "Properties: {0}", info.Properties );
					}
				}

				m_Writer.WriteLine();
			}
			catch {}
		}

		/// <summary>
		/// Writes the current highest bid in an Abay
		/// </summary>
		/// <param name="Abay">The Abay corresponding to the bid</param>
		public static void WriteBid( AbayItem Abay )
		{
			if ( !m_Enabled || m_Writer == null )
				return;

			try
			{
				m_Writer.WriteLine( "> [{0}] Bid Amount : {1}, Mobile : {2} [{3}] Account : {4}",
					Abay.ID.ToString(),
					Abay.HighestBidValue.ToString("#,0" ),
					Abay.HighestBidder.Name,
					Abay.HighestBidder.Serial.ToString(),
					( Abay.HighestBidder.Account as Server.Accounting.Account ).Username );
			}
			catch {}
		}

		/// <summary>
		/// Changes the
		/// </summary>
		/// <param name="Abay">The Abay switching to pending</param>
		/// <param name="reason">The reason why the Abay is set to pending</param>
		public static void WritePending( AbayItem Abay, string reason )
		{
			if ( !m_Enabled || m_Writer == null )
				return;

			try
			{
				m_Writer.WriteLine( "] [{0}] Becoming Pending on {1} at {2}. Reason : {3}",
					Abay.ID.ToString(),
					DateTime.UtcNow.ToShortDateString(),
					DateTime.UtcNow.ToShortTimeString(),
					reason );
			}
			catch {}
		}

		/// <summary>
		/// Writes the end of the Abay to the log
		/// </summary>
		/// <param name="Abay">The Abay ending</param>
		/// <param name="reason">The AbayResult stating why the Abay is ending</param>
		/// <param name="m">The Mobile forcing the end of the Abay (can be null)</param>
		/// <param name="comments">Additional comments on the ending (can be null)</param>
		public static void WriteEnd( AbayItem Abay, AbayResult reason, Mobile m, string comments )
		{
			if ( !m_Enabled || m_Writer == null )
				return;

			try
			{
				m_Writer	.WriteLine( "## Ending Abay {0}", Abay.ID.ToString() );
				m_Writer	.WriteLine( "# Status : {0}", reason.ToString() );

				if ( m != null )
					m_Writer	.WriteLine( "# Ended by {0} [{1}], {2}, Account : {3}",
						m.Name, m.Serial.ToString(), m.AccessLevel.ToString(), ( m.Account as Server.Accounting.Account ).Username );

				if ( comments != null )
					m_Writer	.WriteLine( "# Comments : {0}", comments );

				m_Writer	.WriteLine();
			}
			catch {}
		}

		/// <summary>
		/// Records a staff member viewing an item
		/// </summary>
		/// <param name="Abay">The Abay item</param>
		/// <param name="m">The mobile viewing the item</param>
		public static void WriteViewItem( AbayItem Abay, Mobile m )
		{
			if ( !m_Enabled || m_Writer == null )
				return;

			try
			{
				m_Writer	.WriteLine( "} Vieweing item [{0}] Mobile: {1} [2], {3}, Account : {4}",
					Abay.ID.ToString(),
					m.Name,
					m.Serial.ToString(),
					m.AccessLevel.ToString(),
					( m.Account as Server.Accounting.Account ).Username );
			}
			catch {}
		}

		/// <summary>
		/// Records a staff member returning an item
		/// </summary>
		/// <param name="Abay">The Abay</param>
		/// <param name="m">The mobile returning the item</param>
		public static void WriteReturnItem( AbayItem Abay, Mobile m )
		{
			if ( !m_Enabled || m_Writer == null )
				return;

			try
			{
				m_Writer	.WriteLine( "} Returning item [{0}] Mobile: {1} [2], {3}, Account : {4}",
					Abay.ID.ToString(),
					m.Name,
					m.Serial.ToString(),
					m.AccessLevel.ToString(),
					( m.Account as Server.Accounting.Account ).Username );
			}
			catch {}
		}
	}
}