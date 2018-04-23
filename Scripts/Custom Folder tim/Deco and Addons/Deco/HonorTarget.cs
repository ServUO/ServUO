using System;
using Server;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Targets
{
	public class HonorTarget : Target
	{
		public static void Initialize()
		{
			EventSink.Speech += new SpeechEventHandler( OnSpeech );
		}

		private static void OnSpeech( SpeechEventArgs e )
		{
			Mobile from = e.Mobile;
			if( from is PlayerMobile && e.HasKeyword( 0x0031 ) && from.Karma >= 50 )
				from.Target = new HonorTarget();
		}

		public HonorTarget() : base( 12, false, TargetFlags.None )
		{
		}

		protected override void OnTarget( Mobile from, object o )
		{
			if( !from.Alive || !(o is Mobile) || !(o as Mobile).Alive )
				return;

			if( from.Karma < 50 )
				from.SendLocalizedMessage( 502129 ); // no grant occurs--you are at less than 50 karma
			else if( !(o is PlayerMobile) )
				from.SendLocalizedMessage( 502127 ); // You can only honor players.
			else if( o == from )
				from.SendLocalizedMessage( 502128 ); // You flatter yourself.
			else
			{
				PlayerMobile honored = o as PlayerMobile;

				int maxKarma = Server.Misc.Titles.MaxKarma;

				if( honored.Karma < maxKarma )
				{
					Direction dir = from.GetDirectionTo( honored.Location );
					from.Direction = dir;

					if ( from.Body.IsHuman )
						from.Animate( 32, 5, 1, true, false, 0 );

					from.Karma -= 50;
					from.SendLocalizedMessage( 1019064 ); // You have lost some karma.

					honored.Karma = Math.Min( maxKarma, honored.Karma + 10 );
					honored.SendLocalizedMessage( 1019059 ); // You have gained a little karma.
				}
			}
		}
	}
}