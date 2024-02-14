#region AuthorHeader
//
//	EvoSystem version 2.1, by Xanthos
//
//
#endregion AuthorHeader
using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Commands;
using Xanthos.Interfaces;

namespace Xanthos.Evo
{
	public class EvoCommands
	{
		public static void Initialize()
		{
			CommandHandlers.Register( "EP", AccessLevel.Player, new CommandEventHandler( EP_OnCommand ) );
		}

		public static void EP_OnCommand( CommandEventArgs args )
		{
			PlayerMobile from = args.Mobile as PlayerMobile;

			if( from != null )
				from.Target = new InternalTarget( from );
		}

		private class InternalTarget : Target
		{
			public InternalTarget( Mobile from ) : base( 8, false, TargetFlags.None )
			{
				from.SendMessage ( "Target an Evo creature you control to see its experience points." );
			}

			protected override void OnTarget( Mobile from, object obj )
			{
				IEvoCreature evo = obj as IEvoCreature;
				BaseCreature c = obj as BaseCreature;

				if ( !from.Alive )
					from.SendMessage( "You may not do that while dead." );

				else if ( null == evo || null == c )
					from.SendMessage( "That is not an Evo creature!" );

				else if ( from.AccessLevel == AccessLevel.Player && ( c.Controlled == false || c.ControlMaster != from ))
					from.SendMessage( "You do not control that creature!" );

				else
					c.PublicOverheadMessage( MessageType.Regular, c.SpeechHue, true, c.Name +" has "+ evo.Ep +" experience points.", false );
			}
		}
	}
}
