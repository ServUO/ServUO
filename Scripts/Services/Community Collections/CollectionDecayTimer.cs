using System;

namespace Server
{
    public class CollectionDecayTimer : Timer
    {
        private readonly IComunityCollection m_Collection;
        public CollectionDecayTimer(IComunityCollection collection, TimeSpan delay)
            : base(delay, TimeSpan.FromDays(1.0))
        {
            this.m_Collection = collection;
            this.Priority = TimerPriority.OneMinute;
        }

        protected override void OnTick()
        { 
            if (this.m_Collection != null && this.m_Collection.DailyDecay > 0)		
                this.m_Collection.Points -= this.m_Collection.DailyDecay;
        }
    }
}