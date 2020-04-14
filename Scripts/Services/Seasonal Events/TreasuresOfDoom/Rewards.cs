using Server.Items;
using System.Collections.Generic;

namespace Server.Engines.TreasuresOfDoom
{
    public static class DoomRewards
    {
        public static List<CollectionItem> Rewards { get; set; }

        public static void Initialize()
        {
            Rewards = new List<CollectionItem>();

            Rewards.Add(new CollectionItem(typeof(LanternOfLight), 0xA25, 1155598, 0, 30));
            Rewards.Add(new CollectionItem(typeof(TinctureOfSilver), 0x183B, 1155619, 1900, 10));
            Rewards.Add(new CollectionItem(typeof(AntiqueDocumentsKit), 0x1EBB, 1155630, 0, 10));
            Rewards.Add(new CollectionItem(typeof(TreasuresOfDoomRewardDeed), 0x14EF, 1155603, 0, 20));
            Rewards.Add(new CollectionItem(typeof(TreasuresOfDoomRewardDeed), 0x14EF, 1155600, 0, 30));
            Rewards.Add(new CollectionItem(typeof(TreasuresOfDoomRewardDeed), 0x14EF, 1155602, 0, 50));
            Rewards.Add(new CollectionItem(typeof(CrookOfHumility), 0xE81, 1155624, 0, 50));
            Rewards.Add(new CollectionItem(typeof(ScepterOfPride), 0x26BC, 1155623, 0, 50));
            Rewards.Add(new CollectionItem(typeof(CloakOfLight), 0x1515, 1155608, 0, 50));
            Rewards.Add(new CollectionItem(typeof(BootsOfEscaping), 0x1711, 1155607, 0, 50));
            Rewards.Add(new CollectionItem(typeof(SterlingSilverRing), 0x1F09, 1155606, 0, 50));
        }
    }
}
