#region AuthorHeader
//
//	Misc version 2.0 - utilities version 2.0, by Xanthos
//
//
#endregion AuthorHeader
using System;
using System.Reflection;
using System.Security;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Commands;

namespace Xanthos.Utilities
{
	public class Misc
	{
		/// <summary>
		/// The hues used for gumps in the systems
		/// </summary>
		public static int kLabelHue = 0x480;
		public static int kGreenHue = 0x40;
		public static int kRedHue = 0x20;

		public static bool IsArtifact( Item item )
		{
			if ( null == item )
				return false;
		
			Type t = item.GetType();
			PropertyInfo prop = null;

			try { prop = t.GetProperty( "ArtifactRarity" ); }
			catch {}

			if ( null == prop || (int)(prop.GetValue( item, null )) <= 0 )
				return false;			

			return true;
		}

		public static bool IsPlayerConstructed( Item item )
		{
			if ( null == item )
				return false;
		
			Type t = item.GetType();
			PropertyInfo prop = null;

			try { prop = t.GetProperty( "PlayerConstructed" ); }
			catch {}

			if ( null == prop || true != (bool)(prop.GetValue( item, null )))
				return false;			

			return true;
		}

		//
		// Puts spaces before type name inner-caps
		//
		public static string GetFriendlyClassName( string typeName )
		{
			for ( int index = 1; index < typeName.Length; index++ )
			{
				if ( char.IsUpper( typeName, index ) )
				{
					typeName.Insert( index++, " " );
				}
			}

			return typeName;
		}

		public static object InvokeParameterlessMethod( object target, string method )
		{
			object result = null;

			try
			{
				Type objectType = target.GetType();
				MethodInfo methodInfo = objectType.GetMethod( method );

				result = methodInfo.Invoke( target, null );
			}
			catch ( SecurityException exc )
			{
				Console.WriteLine( "SecurityException: " + exc.Message );
			}
			return result;
		}

		public static void SendCommandDetails( Mobile player, string command )
		{
			SendCommandDescription( player, command );
			SendCommandUsage( player, command );
		}

		public static void SendCommandUsage( Mobile player, string command )
		{
			string message;
			CommandEntry entry = CommandSystem.Entries[ command ];

			if ( null != entry )
			{
				MethodInfo mi = entry.Handler.Method;

				object[] attrs = mi.GetCustomAttributes( typeof( UsageAttribute ), false );

				UsageAttribute usage = attrs.Length > 0 ? attrs[ 0 ] as UsageAttribute : null;

				message = "Format: " + ( null == usage ? " - no usage" : usage.Usage );
			}
			else
				message = command + " - unknown command";

			player.SendMessage( kRedHue, message );
		}

		public static void SendCommandDescription( Mobile player, string command )
		{
			string message;
			CommandEntry entry = CommandSystem.Entries[ command ];

			if ( null != entry )
			{
				MethodInfo mi = entry.Handler.Method;

				object[] attrs = mi.GetCustomAttributes( typeof( DescriptionAttribute ), false );

				DescriptionAttribute desc = attrs.Length > 0 ? attrs[ 0 ] as DescriptionAttribute : null;

				message = command + ": " + ( null == desc ? " - no description" : desc.Description );
			}
			else
				message = command + " - unknown command";

			player.SendMessage( kRedHue, message );
		}
	}
}