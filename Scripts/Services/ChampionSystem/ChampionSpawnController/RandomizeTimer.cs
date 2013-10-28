using System;

namespace Server.Engines.CannedEvil
{
    public class RandomizeTimer : Timer
    {
        private readonly ChampionSpawnController m_Controller;
        public RandomizeTimer(ChampionSpawnController controller, TimeSpan delay)
            : base(delay)
        {
            this.m_Controller = controller;
            this.Priority = TimerPriority.FiveSeconds;
        }

        protected override void OnTick()
        {
            this.m_Controller.Slice();
        }
    }
}