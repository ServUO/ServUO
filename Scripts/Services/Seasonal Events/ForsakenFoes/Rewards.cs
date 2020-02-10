using System;
using Server;
using Server.Items;
using System.Collections.Generic;
using Server.Multis;

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
                new CollectionItem(typeof(PumpkinDeed), 0x14F2, 1159232, 1922, 50000),
                new CollectionItem(typeof(PumpkinRowBoatDeed), 0x14F2, 1159233, 0, 250000),
                new CollectionItem(typeof(JackOLanternHelm), 0xA3EA, 1125986, 0, 150000),
                new CollectionItem(typeof(AdmiralJacksPumpkinSpiceAle), 0xA40B, 1159230, 1922, 50000),
                new CollectionItem(typeof(ExplodingJackOLantern), 0xA407, 1159220, 1175, 40000),
                new CollectionItem(typeof(CaptainTitleDeed), 0x14EF, 1156995, 0, 80000),
                new CollectionItem(typeof(CommanderTitleDeed), 0x14EF, 1156995, 0, 40000),
                new CollectionItem(typeof(EnsignTitleDeed), 0x14EF, 1156995, 0, 20000),
                new CollectionItem(typeof(RecipeScroll), 0x2831, 1074560, 0, 150000),
                new CollectionItem(typeof(RecipeScroll), 0x2831, 1074560, 0, 150000),
                new CollectionItem(typeof(RecipeScroll), 0x2831, 1074560, 0, 200000),
                new CollectionItem(typeof(RecipeScroll), 0x2831, 1074560, 0, 75000),
                new CollectionItem(typeof(RecipeScroll), 0x2831, 1074560, 0, 75000),
                new CollectionItem(typeof(RecipeScroll), 0x2831, 1074560, 0, 75000)
            };
        }
    }
}
