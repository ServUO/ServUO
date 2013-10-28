using System;

namespace Server.Engines.CannedEvil
{
    public class SliceTimer : Timer
    {
        private readonly ChampionSpawn m_Spawn;
        public SliceTimer(ChampionSpawn spawn)
            : base(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(1.0))
        {
            this.m_Spawn = spawn;
            this.Priority = TimerPriority.OneSecond;
        }

        protected override void OnTick()
        {
            this.m_Spawn.OnSlice();
        }
    }
}