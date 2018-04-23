using System;
using System.IO;
using System.Diagnostics;
using Server;

namespace Server.Misc
{
	public class SpawnTimer : Timer
	{
		// This is the time of day you wish for your rares to spawn. (Default 4:00am)
		// this should match your autorestart time.
		private static TimeSpan SpawnTime = TimeSpan.FromHours( 4.00 );

		private static DateTime m_SpawnTime;
		private static bool m_HasSpawned;

		public static void Initialize()
		{
			new SpawnTimer().Start();
		}

		public SpawnTimer() : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
		{
			Priority = TimerPriority.FiveSeconds;

			m_SpawnTime = DateTime.Now.Date + SpawnTime;
		}

		protected override void OnTick()
		{
			if ( DateTime.Now < m_SpawnTime )
				return;

			if ( DateTime.Now > m_SpawnTime && m_HasSpawned == false )
			{
				DailyRaresSystem.StartRareSpawn( true );
				m_HasSpawned = true;
			}
		}
	}
}