using System;
using System.Collections;
using Server;
using Server.Items;

namespace Server.Engines.CannedEvil
{
	public class MCSliceTimer : Timer
	{
		private MiniChamp m_Spawn;

		public MCSliceTimer( MiniChamp spawn ) : base( TimeSpan.FromSeconds( 1.0 ),  TimeSpan.FromSeconds( 1.0 ) )
		{
			m_Spawn = spawn;
			Priority = TimerPriority.OneSecond;
		}

		protected override void OnTick()
		{
			m_Spawn.OnSlice();
		}
	}
}