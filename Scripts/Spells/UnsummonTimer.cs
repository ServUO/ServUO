using Server.Mobiles;

using System;
using System.Collections.Generic;

namespace Server.Spells
{
    public class UnsummonTimer : Timer
    {
        public static UnsummonTimer Instance { get; set; }

        public List<BaseCreature> Creatures { get; set; } = new List<BaseCreature>();

        public static void Register(BaseCreature bc)
        {
            if (Instance == null)
            {
                Instance = new UnsummonTimer();
                Instance.Start();
            }

            if (!Instance.Creatures.Contains(bc))
            {
                Instance.Creatures.Add(bc);
            }
        }

        public static void Unregister(BaseCreature bc)
        {
            if (Instance == null)
            {
                return;
            }

            if (Instance.Creatures.Contains(bc))
            {
                Instance.Creatures.Remove(bc);
            }

            if (Instance.Creatures.Count == 0)
            {
                Instance.Stop();
                Instance = null;
            }
        }

        public UnsummonTimer()
            : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
        {
        }

        protected override void OnTick()
        {
            for (int i = Creatures.Count - 1; i >= 0; i--)
            {
                var bc = Creatures[i];

                if (bc.SummonEnd < DateTime.UtcNow)
                {
                    bc.Delete();
                }

                if (bc.Deleted)
                {
                    Unregister(bc);
                }
            }
        }
    }
}
