using System;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;
using System.Collections;
using Server.Commands;

namespace Server.Commands
{
	public class Slap
	{
		public static void Initialize()
		{
			//Register( string command, AccessLevel access, CommandEventHandler handler )
			CommandHandlers.Register( "Slap", AccessLevel.Counselor, new CommandEventHandler( Slap_OnCommand ) );
		}
		[Usage( "Slap" )]
		public static void Slap_OnCommand( CommandEventArgs e )
		{
				Mobile m = e.Mobile;
				e.Mobile.SendMessage("Target the dumb ass");
				e.Mobile.Target = new SlapTarget();

			}

			


		public class SlapTarget : Target
		{
			public SlapTarget() : base(12, false, TargetFlags.None)
			{
			}
			protected override void OnTarget(Mobile from, object target)
			{
				

				if( target is Mobile ) 
				{
						Mobile m = (Mobile)target; 
						if(m.Mounted)
						{
							IMount mount = m.Mount;
							mount.Rider = null;
						}
						m.Animate( 22, 5, 1, true, false, 0 );
						from.PlaySound(948);
						from.Emote("*Slap*");
						m.Emote("*Ouch!*");
									}
							}
						}
					} 
				}