using System;
using System.Reflection;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Commands
{
	public class Donate
	{
		public static void Initialize()
		{
			CommandSystem.Register( "Donate", AccessLevel.Player, new CommandEventHandler( Donate_OnCommand ) );
		}

		[Usage( "Donate" )]
		[Description( "Defiance Network needs your aid to continue, every little bit you can spare counts alot for our future. Opens webbrowser to Donation Centre." )]
		public static void Donate_OnCommand( CommandEventArgs e )
		{
			string url = "http://www.defianceuo.com/donate/";

			Mobile m = e.Mobile;
			m.LaunchBrowser( url );
		}
	}
}