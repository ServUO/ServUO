using System;
using System.Collections;
using Server;
using Server.Items;

namespace Server.Engines.CannedEvil
{
	public class MCRestartTimer : Timer
	{
		private MiniChamp m_Spawn;

		public MCRestartTimer( MiniChamp spawn, TimeSpan delay ) : base( delay )
		{
			m_Spawn = spawn;
			Priority = TimerPriority.FiveSeconds;
		}

		protected override void OnTick()
		{
			m_Spawn.EndRestart();
		}
	}
}