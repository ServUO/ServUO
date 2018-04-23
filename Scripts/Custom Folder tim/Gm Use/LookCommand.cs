//   ___|========================|___
//   \  |  Written by Felladrin  |  /	This script was released on RunUO Forums under the GPL licensing terms.
//    > |      February 2010     | < 
//   /__|========================|__\	Current version: 1.0 (February 6, 2010)

using System;
using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.Gumps;
using Server.Network;

namespace Server.Commands
{ 
	public class lookCommand
	{ 
		public static void Initialize() 
		{ 
			CommandSystem.Register( "Look", AccessLevel.Player, new CommandEventHandler( look_OnCommand ) );
		}

		[Usage( "Look" )]
		[Description( "Used to look at someone who is near you or to change your's character description." )]
		public static void look_OnCommand( CommandEventArgs e )
		{ 
			if ( e.Mobile is PlayerMobile ) 
			{				
				e.Mobile.SendMessage( "Who would you like to look at?" );
				e.Mobile.Target = new lookTarget();
			}
		}
	}

	public class lookTarget : Target
	{ 
		public lookTarget() : base( -1, false, TargetFlags.None )
		{
		}

		protected override void OnTarget( Mobile from, object targeted ) 
		{ 
			if ( from is PlayerMobile && targeted is PlayerMobile ) 
			{ 
				if(from.Equals(targeted))
				{
					((Mobile)targeted).DisplayPaperdollTo( from );
					from.Send( new DisplayProfile( !from.ProfileLocked, from, "Description of " + from.RawName, from.Profile, "Use the space above to describe your character.") );
				}
				else 
				{
					((Mobile)targeted).SendMessage("You notice that {0} is looking at you.", from.Name );
					((Mobile)targeted).DisplayPaperdollTo( from );
					from.CloseGump( typeof( lookGump ) );
					from.SendGump(new lookGump( from, (Mobile)targeted ));
				}
			}
	 		else
				from.SendMessage("There's nothing special about it, it isn't worth looking...");
		} 
	}
 
	public class lookGump : Gump
	{
		private const int Width = 300;
		private const int Height = 200;

		public lookGump( Mobile m, Mobile target ) : base( 100, 100 )
		{
			AddPage( 0 );

			AddBackground( 0, 0, Width, Height, 0xDAC );

			AddPage( 1 );

			AddHtml( 0, 10, Width, 25, "<CENTER>" + "Observing " + target.Name, false, false );
			AddHtml( 20, 30, Width-40, Height-50, target.Profile, true, true );
		}
	}
}
