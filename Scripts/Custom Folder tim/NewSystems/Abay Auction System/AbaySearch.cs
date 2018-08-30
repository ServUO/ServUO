#region AuthorHeader
//
//	Abay version 2.1, by Xanthos and Arya
//
//  Based on original ideas and code by Arya
//
#endregion AuthorHeader
using System;
using System.Collections;
using System.Reflection;
using Server;

namespace Arya.Abay
{
	/// <summary>
	/// Provides search methods for the Abay system
	/// </summary>
	public class AbaySearch
	{
		/// <summary>
		/// Merges search results
		/// </summary>
		public static ArrayList Merge( ArrayList first, ArrayList second )
		{
			ArrayList	result = new ArrayList( first );

			foreach( AbayItem item in second )
			{
				if ( ! result.Contains( item ) )
					result.Add( item );
			}

			return result;
		}

		/// <summary>
		/// Performs a text search
		/// </summary>
		/// <param name="items">The items to search</param>
		/// <param name="text">The text search, names should be whitespace separated</param>
		public static ArrayList	SearchForText( ArrayList items, string text )
		{
			string[] split = text.Split( ' ' );
			ArrayList result = new ArrayList();

			foreach( string s in split )
			{
				result = Merge( result, TextSearch( items, s ) );
			}

			return result;
		}

		/// <summary>
		/// Performs a text search
		/// </summary>
		/// <param name="list">The AbayItem objects to search</param>
		/// <param name="name">The text to search for</param>
		private static ArrayList TextSearch( ArrayList list, string name )
		{
			ArrayList results = new ArrayList();

			if ( list == null || name == null )
			{
				return results;
			}

			IEnumerator ie = null;

			try
			{
				name = name.ToLower();

				ie = list.GetEnumerator();

				while ( ie.MoveNext() )
				{
					AbayItem item = ie.Current as AbayItem;

					if ( item != null )
					{
						if ( item.ItemName.ToLower().IndexOf( name ) > -1 )
						{
							results.Add( item );
						}
						else if ( item.Description.ToLower().IndexOf( name ) > -1 )
						{
							results.Add( item );
						}
						else
						{
							// Search individual items
							foreach( AbayItem.ItemInfo info in item.Items )
							{
								if ( info.Name.ToLower().IndexOf( name ) > -1 )
								{
									results.Add( item );
									break;
								}
								else if ( info.Properties.ToLower().IndexOf( name ) > .1 )
								{
									results.Add( item );
									break;
								}
							}
						}
					}
				}
			}
			catch {}
			finally
			{
				IDisposable d = ie as IDisposable;

				if ( d != null )
					d.Dispose();
			}

			return results;
		}

		/// <summary>
		/// Performs a search for types being Abayed
		/// </summary>
		/// <param name="list">The items to search</param>
		/// <param name="types">The list of types to find matches for</param>
		public static ArrayList ForTypes ( ArrayList list, ArrayList types )
		{
			ArrayList results = new ArrayList();

			if ( list == null || types == null )
				return results;

			IEnumerator ie = null;

			try
			{
				ie = list.GetEnumerator();

				while ( ie.MoveNext() )
				{
					AbayItem item = ie.Current as AbayItem;

					if ( item == null )
						continue;

					foreach( Type t in types )
					{
						if ( MatchesType( item, t ) )
						{
							results.Add( item );
							break;
						}
					}
				}
			}
			catch {}
			finally
			{
				IDisposable d = ie as IDisposable;

				if ( d != null )
					d.Dispose();
			}

			return results;
		}

		/// <summary>
		/// Verifies if a specified type is a match to the items sold through an Abay
		/// </summary>
		/// <param name="item">The AbayItem being evaluated</param>
		/// <param name="type">The type looking to match</param>
		/// <returns>True if there's a match</returns>
		private static bool MatchesType( AbayItem item, Type type )
		{
			foreach( AbayItem.ItemInfo info in item.Items )
			{
				if ( info.Item != null )
				{
					if ( type.IsAssignableFrom( info.Item.GetType() ) )
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Performs a search for Artifacts by evaluating the ArtifactRarity property
		/// </summary>
		/// <param name="items">The list of items to search</param>
		/// <returns>An ArrayList of results</returns>
		public static ArrayList ForArtifacts( ArrayList items )
		{
			ArrayList results = new ArrayList();

			foreach( AbayItem Abay in items )
			{
				foreach( AbayItem.ItemInfo info in Abay.Items )
				{
					Item item = info.Item;

					if ( Xanthos.Utilities.Misc.IsArtifact( item ))
					{
						results.Add( Abay );
						break;
					}
				}
			}

			return results;
		}


		/// <summary>
		/// Searches a list of Abays for ICommodities
		/// </summary>
		/// <param name="items">The list to search</param>
		/// <returns>An ArrayList of results</returns>
		public static ArrayList ForCommodities( ArrayList items )
		{
			ArrayList results = new ArrayList();

			foreach( AbayItem Abay in items )
			{
				foreach( AbayItem.ItemInfo info in Abay.Items )
				{
					if ( info.Item == null )
						continue;

					Type t = info.Item.GetType();

					if ( t.GetInterface( "ICommodity" ) != null )
					{
						results.Add( Abay );
						break;
					}
				}
			}

			return results;
		}
	}
}