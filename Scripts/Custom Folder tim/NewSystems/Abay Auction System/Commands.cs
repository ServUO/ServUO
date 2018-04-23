#region AuthorHeader
//
//	Abay version 2.1, by Xanthos and Arya
//
//  Based on original ideas and code by Arya
//
#endregion AuthorHeader
using System;
using Server.Commands;
using Server;

namespace Arya.Abay
{
	/// <summary>
	/// Summary description for Commands.
	/// </summary>
	public class AbayCommands
	{
		public static void Initialize()
		{
			CommandHandlers.Register( "InitAbay", AccessLevel.Administrator, new CommandEventHandler( OnInitAbay ) );
			CommandHandlers.Register( "MyAbay", AccessLevel.Player, new CommandEventHandler( OnMyAbay ) );
			CommandHandlers.Register( "Abay", AccessLevel.GameMaster, new CommandEventHandler( OnAbay ) );
			CommandHandlers.Register( "AbayAdmin", AccessLevel.Administrator, new CommandEventHandler( OnAbayAdmin ) );
		}

		#region Placing the control stone

		[ Usage( "InitAbay" ), Description( "Initializes the Abay system by bringing up a target for the creation of the Abay control stone. If the system is already running this command will bring the user to the stone's location" ) ]
		private static void OnInitAbay( CommandEventArgs e )
		{
			if ( AbaySystem.Running )
			{
				e.Mobile.SendMessage( AbayConfig.MessageHue, "The Abay System is already running. You have been teleported to the control stone location" );
				e.Mobile.Location = AbaySystem.ControlStone.Location;
				e.Mobile.Map = AbaySystem.ControlStone.Map;
			}
			else
			{
				e.Mobile.SendMessage( AbayConfig.MessageHue, "Where do you with to place the Abay control stone?" );
				e.Mobile.Target = new AbayTarget( new AbayTargetCallback( PlaceStoneCallback ), -1, true );
			}
		}

		private static void PlaceStoneCallback( Mobile from, object targeted )
		{
			IPoint3D location = targeted as IPoint3D;

			if ( location != null )
			{
				AbayControl stone = new AbayControl();

				stone.MoveToWorld( new Point3D( location ), from.Map );
				AbaySystem.ControlStone = stone;
			}
			else
			{
				from.SendMessage( AbayConfig.MessageHue, "Invalid location" );
			}
		}

		#endregion

		#region MyAbay

		[ Usage( "MyAbay" ), Description( "Displays all the Abays a player has created or has bid on. This command can't be used to access the full system, therefore it cannot be used to create new Abays." ) ]
		private static void OnMyAbay( CommandEventArgs e )
		{
			if ( AbaySystem.Running )
			{
				e.Mobile.SendGump( new MyAbayGump( e.Mobile, null ) );
			}
			else
			{
				e.Mobile.SendMessage( AbayConfig.MessageHue, AbaySystem.ST[ 200 ] );
			}
		}

		#endregion

		#region Abay

		[ Usage( "Abay" ), Description( "Displays the main Abay system gump" ) ]
		private static void OnAbay( CommandEventArgs e )
		{
			if ( AbaySystem.Running )
			{
				e.Mobile.SendGump( new AbayGump( e.Mobile ) );
			}
			else
			{
				e.Mobile.SendMessage( AbayConfig.MessageHue, "The Abay system is currently stopped" );
			}
		}

		#endregion

		#region Abay Admin

		[ Usage( "AbayAdmin" ), Description( "Invokes the Abay system administration gump" ) ]
		private static void OnAbayAdmin( CommandEventArgs e )
		{
			if ( AbaySystem.Running )
				e.Mobile.SendGump( new AbayAdminGump( e.Mobile ) );
			else
				e.Mobile.SendMessage( AbayConfig.MessageHue, "The Abay system is now stopped" );
		}

		#endregion
	}
}