using System;

namespace Server.Engines.CannedEvil
{
    public class RestartTimer : Timer
    {
        private readonly ChampionSpawn m_Spawn;
        public RestartTimer(ChampionSpawn spawn, TimeSpan delay)
            : base(delay)
        {
            this.m_Spawn = spawn;
            this.Priority = TimerPriority.FiveSeconds;
        }

        protected override void OnTick()
        {
            this.m_Spawn.EndRestart();
        }
    }
}