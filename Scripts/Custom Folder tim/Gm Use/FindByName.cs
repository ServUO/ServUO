using System;
using Server;
using Server.Commands;

namespace Server.Commands
{
	public class FindByName
	{
		public static void Initialize()
		{
			CommandSystem.Register( "FindByName", AccessLevel.Administrator, new CommandEventHandler( FindByName_OnCommand ) );
		}

		[Usage( "FindByName <name>" )]
		[Description( "Finds an item by name." )]
		public static void FindByName_OnCommand( CommandEventArgs e )
		{
			if ( e.Length == 1 )
			{
				string name = e.GetString( 0 ).ToLower();

				foreach ( Item item in World.Items.Values )
				{
					if ( item.Name != null && item.Name.ToLower().IndexOf( name ) >= 0 )
					{
						object root = item.RootParent;

						if ( root is Mobile )
							e.Mobile.SendMessage( "{0} [{1}]: {2} ({3})", item.GetWorldLocation(), item.Map, root.GetType().Name, ((Mobile)root).Name );
						else
							e.Mobile.SendMessage( "{0} [{1}]: {2}", item.GetWorldLocation(), item.Map, root==null ? "(null)" : root.GetType().Name );
					}
				}
			}
			else
			{
				e.Mobile.SendMessage( "Format: FindByName <name>" );
			}
		}
	}
}