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

        private static Dictionary<Type, List<DropEntry>> m_Table = new Dictionary<Type, List<DropEntry>>();

        public static void Configure()
        {
            Enabled = Config.Get("Loot.Enabled", true);
            FeluccaLuckBonus = Config.Get("Loot.FeluccaLuckBonus", 1000);
            FeluccaBudgetBonus = Config.Get("Loot.FeluccaBudgetBonus", 100);
        }

        public static void Initialize()
        {
            m_Table[typeof(BaseCreature)] = new List<DropEntry>();

            m_Table[typeof(Server.Engines.Despise.AndrosTheDreadLord)] = new List<DropEntry>();
            m_Table[typeof(Server.Engines.Despise.AndrosTheDreadLord)].Add(new DropEntry(null, 50, 15));

            m_Table[typeof(Server.Engines.Despise.AdrianTheGloriousLord)] = new List<DropEntry>();
            m_Table[typeof(Server.Engines.Despise.AdrianTheGloriousLord)].Add(new DropEntry(null, 50, 15));
        }

        public static void OnDeath(BaseCreature bc, Container corpse)
        {
            if (corpse == null || corpse.Map == Map.Internal || corpse.Map == null)
                return;

            Type type = bc.GetType();

            foreach(KeyValuePair<Type, List<DropEntry>> kvp in m_Table)
            {
                if (type == kvp.Key || type.IsSubclassOf(kvp.Key))
                {
                    Region r = Region.Find(corpse.Location, corpse.Map);
                    Map map = corpse.Map;

                    if (r != null)
                    {
                        foreach (DropEntry entry in kvp.Value)
                        {
                            if (entry.Region == null || entry.Region == r.Name || entry.Region == map.ToString())
                            {
                                for (int i = 0; i < entry.Amount; i++)
                                {
                                    if (Utility.Random(100) < entry.Probability)
                                    {
                                        Item item = RunicReforging.GenerateRandomItem(bc.LastKiller, bc);

                                        if (item != null)
                                            corpse.DropItem(item);
                                    }
                                }
                            }
                        }
                    }
                }
            }
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

        private class DropEntry
        {
            private string m_Region;
            private int m_Probability;
            private int m_Amount;

            public string Region { get { return m_Region; } }           // mark null to omit region/map restriction. can use map name instead of region
            public int Probability { get { return m_Probability; } }
            public int Amount { get { return m_Amount; } }

            public DropEntry(string region, int probability, int amount)
            {
                m_Region = region;
                m_Probability = probability;
                m_Amount = amount;
            }
        }
    }
}
