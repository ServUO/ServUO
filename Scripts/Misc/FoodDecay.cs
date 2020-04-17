using Server.Network;
using System;

namespace Server.Misc
{
    public class FoodDecayTimer : Timer
    {
        public FoodDecayTimer()
            : base(TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5))
        {
            Priority = TimerPriority.OneMinute;
        }

        public static void Initialize()
        {
            new FoodDecayTimer().Start();
        }

        public static void FoodDecay()
        {
            foreach (NetState state in NetState.Instances)
            {
                HungerDecay(state.Mobile);
                ThirstDecay(state.Mobile);
            }
        }

        public static void HungerDecay(Mobile m)
        {
            if (m != null && m.Hunger >= 1)
                m.Hunger -= 1;
        }

        public static void ThirstDecay(Mobile m)
        {
            if (m != null && m.Thirst >= 1)
                m.Thirst -= 1;
        }

        protected override void OnTick()
        {
            FoodDecay();
        }
    }
}