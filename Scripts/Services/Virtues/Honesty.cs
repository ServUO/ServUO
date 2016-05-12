using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Services.Virtues
{
	class Honesty
	{
		public static void Initialize()
		{
			VirtueGump.Register(106, OnVirtueUsed);
		}

		public static void OnVirtueUsed(Mobile from)
		{
			from.SendLocalizedMessage(1053001); // This virtue is not activated through the virtue menu.
		}
	}
}
