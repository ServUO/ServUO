using Server.Items;
using Server.Multis;
using System.Collections.Generic;

namespace Server.Engines.Fellowship
{
    public static class FellowshipRewards
    {
        public static List<CollectionItem> Rewards { get; set; }

        public static void Initialize()
        {
            Rewards = new List<CollectionItem>
            {
                new CollectionItem(typeof(TinctureOfSilver), 0x183B, 1155619, 1900, 10000),
                new CollectionItem(typeof(PumpkinCannonDeed), 0x14F2, 1159232, 1922, 50000),
                new CollectionItem(typeof(PumpkinRowBoatDeed), 0x14F2, 1159233, 0, 250000),
                new CollectionItem(typeof(JackOLanternHelm), 0xA3EA, 1125986, 0, 150000),
                new CollectionItem(typeof(AdmiralJacksPumpkinSpiceAle), 0xA40B, 1159230, 1922, 50000),
                new CollectionItem(typeof(ExplodingJackOLantern), 0xA407, 1159220, 1175, 40000),
                new CollectionItem(typeof(CaptainTitleDeed), 0x14EF, 1159216, 0, 80000),
                new CollectionItem(typeof(CommanderTitleDeed), 0x14EF, 1078449, 0, 40000),
                new CollectionItem(typeof(EnsignTitleDeed), 0x14EF, 1159214, 0, 20000),
                new CollectionItem(typeof(RecipeScroll), 0x2831, 1159228, 0, 150000), // cowl of the mace & shield
                new CollectionItem(typeof(RecipeScroll), 0x2831, 1159229, 0, 150000), // mage's hood of scholarly insight
                new CollectionItem(typeof(RecipeScroll), 0x2831, 1159225, 0, 200000), // elegant collar of fortune
                new CollectionItem(typeof(RecipeScroll), 0x2831, 1159213, 0, 75000),  // crimson dagger belt
                new CollectionItem(typeof(RecipeScroll), 0x2831, 1159212, 0, 75000),  // crimson sword belt
                new CollectionItem(typeof(RecipeScroll), 0x2831, 1159211, 0, 75000)   // crimson mace belt
            };
        }
    }
}
