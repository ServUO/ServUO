using System;

namespace Server
{
    public class CollectionDecayTimer : Timer
    {
        private readonly IComunityCollection m_Collection;
        public CollectionDecayTimer(IComunityCollection collection, TimeSpan delay)
            : base(delay, TimeSpan.FromDays(1.0))
        {
            m_Collection = collection;
            Priority = TimerPriority.OneMinute;
        }

        protected override void OnTick()
        {
            if (m_Collection != null && m_Collection.DailyDecay > 0)
                m_Collection.Points -= m_Collection.DailyDecay;
        }
    }
}