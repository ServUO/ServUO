#define RUNUO_2 //Comment this out to enable RunUO 1.0 Mode

using System;

#if(RUNUO_2)
using Server.Commands;
#else
using Server.Scripts.Commands;
#endif

using Server;
using Server.Mobiles;
using Server.Network;

namespace Server.Touring
{
	public class TourCommand
	{
		public static void Initialize()
		{
#if (RUNUO_2)
			CommandSystem.Register("Tour", AccessLevel.Player, new CommandEventHandler(Tour_OnCommand));
#else
		Commands.Register("Tour", AccessLevel.Player, new CommandEventHandler(Tour_OnCommand));
#endif
		}

		private static void Tour_OnCommand(CommandEventArgs e)
		{
			if (e.Mobile != null && !e.Mobile.Deleted)
			{
				if (TourConfig.YoungOnly && e.Mobile is PlayerMobile && !((PlayerMobile)e.Mobile).Young)
				{
					e.Mobile.SendMessage("Only young players are able to take the tour.");
					return;
				}

				if (!Tour.Stages.ContainsKey(e.Mobile))
					Tour.Start(e.Mobile);
			}
		}
	}
}