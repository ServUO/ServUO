#region AuthorHeader
//
//	Abay version 2.1, by Xanthos and Arya
//
//  Based on original ideas and code by Arya
//
#endregion AuthorHeader
using System;
using System.Collections;

namespace Arya.Abay
{
	/// <summary>
	/// Defines Abay sorting methods
	/// </summary>
	public enum AbaySorting
	{
		/// <summary>
		/// Sorting by item name
		/// </summary>
		Name,
		/// <summary>
		/// Sorting by date of creation
		/// </summary>
		Date,
		/// <summary>
		/// Sorting by time left for the Abay
		/// </summary>
		TimeLeft,
		/// <summary>
		/// Sorting by the number of bids
		/// </summary>
		Bids,
		/// <summary>
		/// Sorting by value of minimum bid
		/// </summary>
		MinimumBid,
		/// <summary>
		/// Sorting by value of the higherst bid
		/// </summary>
		HighestBid
	}

	/// <summary>
	/// Provides sorting for Abay listings
	/// </summary>
	public class AbayComparer : IComparer
	{
		private bool m_Ascending;
		private AbaySorting m_Sorting;

		public AbayComparer( AbaySorting sorting, bool ascending )
		{
			m_Ascending = ascending;
			m_Sorting = sorting;
		}

		#region IComparer Members

		public int Compare(object x, object y)
		{
			AbayItem item1 = null;
			AbayItem item2 = null;

			if ( m_Ascending )
			{
				item1 = x as AbayItem;
				item2 = y as AbayItem;
			}
			else
			{
				// Switch x and y for descending ordering

				item1 = y as AbayItem;
				item2 = x as AbayItem;
			}

			if ( item1 == null || item2 == null )
				return 0;

			switch ( m_Sorting )
			{
				case AbaySorting.Bids:

					return item1.Bids.Count.CompareTo( item2.Bids.Count );

				case AbaySorting.Date:

					return item1.StartTime.CompareTo( item2.StartTime );

				case AbaySorting.HighestBid:

					return item1.MinNewBid.CompareTo( item2.MinNewBid );

				case AbaySorting.MinimumBid:

					return item1.MinBid.CompareTo( item2.MinBid );

				case AbaySorting.Name:

					return item1.ItemName.CompareTo( item2.ItemName );

				case AbaySorting.TimeLeft:

					return item1.TimeLeft.CompareTo( item2.TimeLeft );
			}

			return 0;
		}

		#endregion
	}
}