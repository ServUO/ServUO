/*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
~~~~~~~    Ultimate Hue Room Generation System
~~~~~~~~~~          designed by MightyHythloth
~~~~~~~~~~~~~~~~
                with credits to the following:
~~~~~~~~~~~~~~~~~~~~
               Lord_GreyWolf - Equation Coding
          tangentzero - Precursor for dye tubs
Cottonballs (Revision of [hue, author unknown)
~~~~~~~~~~~~~~~~~~~~~~~~
Feel free to modify for your use... but please 
leave all credits if you distribute it further
~~~~~~~~~~~~~~~~~~~~~~~~~~~~
~~~  UltimateDyeTub.cs and HueRoomGen.cs  ~~~~
~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~*/

using System;
using System.Collections;
using System.Collections.Generic;
using Server.Commands;
using Server.Commands.Generic;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server
{
	public class GenerateHueRoom
	{
		public GenerateHueRoom()
		{
		}

		public static void Initialize()
		{
			CommandSystem.Register( "GenHueRoom", AccessLevel.Administrator, new CommandEventHandler( GenerateHueRoom_OnCommand ) );
		}

		[Usage( "GenHueRoom" )]
		[Description( "Generates a 50X60 Grid including spacing with hues ranged from 0-3000." )]
		public static void GenerateHueRoom_OnCommand( CommandEventArgs e )
		{
			e.Mobile.SendMessage( "Please wait for the dye tubs to be generated." );

			Map map = e.Mobile.Map;
			int MX = e.Mobile.X;
			int MY = e.Mobile.Y;
			int MZ = e.Mobile.Z;

			if ( map != null )
			{
				for ( int x = 0; x <= 99; ++x )
				{
					for ( int y = 0; y <= 29; ++y )
					{
						UltimateDyeTub udt = new UltimateDyeTub();
						udt.Hue = ((100 * y) + x);
						udt.Name = Convert.ToString(udt.Hue);
						udt.ItemID = 5162;
						udt.Movable = false;
						if (udt != null) udt.MoveToWorld( new Point3D( (MX + (2* x)), (MY + (2 * y)), MZ ), map );
					}
				}
			}

			e.Mobile.SendMessage( "The Ultimate Hue Room Is Complete." );
		}

	}
}
