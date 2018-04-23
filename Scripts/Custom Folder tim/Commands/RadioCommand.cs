using System;
using System.Reflection;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Commands
{
	public class Radio
	{
		public static void Initialize()
		{
			CommandSystem.Register( "Radio", AccessLevel.Player, new CommandEventHandler( Radio_OnCommand ) );
		}

		[Usage( "Radio" )]
		[Description( "Visit the Defiance Radio page and listen to staff interviews and lots of music!" )]
		public static void Radio_OnCommand( CommandEventArgs e )
		{
			string url = "http://defianceuo.com/radio";

			Mobile m = e.Mobile;
			m.LaunchBrowser( url );
		}
	}
}