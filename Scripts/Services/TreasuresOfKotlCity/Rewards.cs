using System;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Network;
using System.Linq;
using Server.Engines.Points;

namespace Server.Engines.TreasuresOfKotlCity
{
    public static class KotlCityRewards
    {
        public static List<CollectionItem> Rewards { get; set; }

        public static void Initialize()
        {
            Rewards = new List<CollectionItem>();

            Rewards.Add(new CollectionItem(typeof(SkeletalHangmanAddonDeed), 0x14EF, 1156982, 0, 10));
            Rewards.Add(new CollectionItem(typeof(KotlSacraficialAltarAddonDeed), 0x14EF, 1124311, 0, 10));
            Rewards.Add(new CollectionItem(typeof(BlackrockMoonstone), 0x9CAA, 1156993, 1175, 10));
            Rewards.Add(new CollectionItem(typeof(RecipeScroll), 0x2831, 1156995, 0, 10));
            Rewards.Add(new CollectionItem(typeof(RecipeScroll), 0x2831, 1156996, 0, 10));
            Rewards.Add(new CollectionItem(typeof(RecipeScroll), 0x2831, 1157016, 0, 10));
            Rewards.Add(new CollectionItem(typeof(AutomatonActuator), 0x9CE9, 1156997, 0, 5));
            Rewards.Add(new CollectionItem(typeof(TreasuresOfKotlRewardDeed), 0x14EF, 1156987, 0, 10));
            Rewards.Add(new CollectionItem(typeof(TreasuresOfKotlRewardDeed), 0x14EF, 1156986, 0, 10));
            Rewards.Add(new CollectionItem(typeof(TreasuresOfKotlRewardDeed), 0x14EF, 1156985, 0, 10));

            Rewards.Add(new CollectionItem(typeof(TribalBanner), 0x9D5E, 1124309, 0, 20));
            Rewards.Add(new CollectionItem(typeof(TribalBanner), 0x9D5C, 1124307, 0, 20));
            Rewards.Add(new CollectionItem(typeof(TribalBanner), 0x9D5A, 1124305, 0, 20));
            Rewards.Add(new CollectionItem(typeof(TribalBanner), 0x9D58, 1124303, 0, 20));
            Rewards.Add(new CollectionItem(typeof(TribalBanner), 0x9D56, 1124301, 0, 20));
            Rewards.Add(new CollectionItem(typeof(TribalBanner), 0x9D54, 1124299, 0, 20));

            Rewards.Add(new CollectionItem(typeof(KatalkotlsRing), 0x1F09, 1156989, 2591, 50));
            Rewards.Add(new CollectionItem(typeof(TalonsOfEscaping), 0x41D8, 1155682, 0, 50));
            Rewards.Add(new CollectionItem(typeof(BootsOfEscaping), 0x1711, 1155607, 0, 50));
        }
    }
}