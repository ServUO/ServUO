using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;

namespace Server.Commands
{
	public class TeleGenCommand
	{
		private static Teleporter m_Tele;
		
		public static void Initialize()
		{
			CommandSystem.Register( "TeleGen", AccessLevel.GameMaster, new CommandEventHandler( TeleGen_OnCommand ) );
		}
		
		[Usage( "TeleGen" )]
		[Description( "Generate teleporters quickly and easily." )]
		public static void TeleGen_OnCommand( CommandEventArgs e )
		{
			Teleporter tele = new Teleporter();
			tele.PointDest = e.Mobile.Location;
			tele.MapDest = e.Mobile.Map;
			
			e.Mobile.SendMessage("You have generated a teleporter for {0} on map {1}.", tele.PointDest.ToString(), tele.MapDest.ToString());
			
			e.Mobile.AddToBackpack( tele );
		}
	}
}
