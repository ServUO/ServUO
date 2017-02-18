using System;
using Server;
using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class RandomItemGenerator
    {
        public static bool Enabled { get; private set; }
        public static int FeluccaLuckBonus { get; private set; }
        public static int FeluccaBudgetBonus { get; private set; }

        public static int MaxBaseBudget { get; private set; }
        public static int MinBaseBudget { get; private set; }
        public static int MaxProps { get; private set; }

        public static void Configure()
        {
            Enabled = Config.Get("Loot.Enabled", true);
            FeluccaLuckBonus = Config.Get("Loot.FeluccaLuckBonus", 1000);
            FeluccaBudgetBonus = Config.Get("Loot.FeluccaBudgetBonus", 100);

            MaxBaseBudget = Config.Get("Loot.MaxBaseBudget", 900);
            MinBaseBudget = Config.Get("Loot.MinBaseBudget", 150);
            MaxProps = Config.Get("Loot.MaxProps", 9);
        }

        private RandomItemGenerator()
        {
        }

        /// <summary>
        /// This is called for every item that is dropped to a loot pack.
        /// Change the conditions here to add/remove Random Item Drops with REGULAR loot.
        /// </summary>
        /// <param name="item">item to be mutated</param>
        /// <param name="killer">Mobile.LastKiller</param>
        /// <param name="victim">the victim</param>
        public static bool GenerateRandomItem(Item item, Mobile killer, BaseCreature victim)
        {
            if(Enabled)
                return RunicReforging.GenerateRandomItem(item, killer, victim);
            return false;
        }
    }
}
