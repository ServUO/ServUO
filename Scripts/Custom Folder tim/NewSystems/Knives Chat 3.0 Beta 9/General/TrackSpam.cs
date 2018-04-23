using System;
using System.Collections;
using Server;

namespace Knives.Chat3
{
	public class TrackSpam
	{
		private static Hashtable s_Log = new Hashtable();

		public static bool LogSpam( Mobile m, string type, TimeSpan limit )
		{
			if ( s_Log.Contains( m ) )
			{
				Hashtable table = (Hashtable)s_Log[m];

				if ( table.Contains( type ) )
				{
					if ( (DateTime)table[type] > DateTime.Now-limit )
						return false;

					table[type] = DateTime.Now;
				}
			}
			else
			{
				Hashtable table = new Hashtable();
				table[type] = DateTime.Now;
				s_Log[m] = table;
			}

			return true;
        }

		public static TimeSpan NextAllowedIn( Mobile m, string type, TimeSpan limit )
		{
			if ( s_Log[m] == null )
				return TimeSpan.FromSeconds( 1 );

			Hashtable table = (Hashtable)s_Log[m];

			if ( table[type] == null || (DateTime)table[type]+limit < DateTime.Now )
				return TimeSpan.FromSeconds( 1 );

			return (DateTime)table[type]+limit-DateTime.Now;
		}
	}
}