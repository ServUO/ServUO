using Server.Items;
using System.Collections.Generic;

namespace Server.Engines.SorcerersDungeon
{
    public static class SorcerersDungeonRewards
    {
        public static List<CollectionItem> Rewards { get; set; }

        public static void Initialize()
        {
            Rewards = new List<CollectionItem>();

            Rewards.Add(new CollectionItem(typeof(JacksBagOfTricks), 0x9F85, 1157656, 0, 75));
            Rewards.Add(new CollectionItem(typeof(SerpentsJawbone), 0x9F74, 1157654, 0, 50));
            Rewards.Add(new CollectionItem(typeof(DecoJackInTheBox), 0x9F64, 1157655, 0, 20));
            Rewards.Add(new CollectionItem(typeof(ShackledHeartOfThePumpkinKing), 0x4A9C, 1157653, 0, 20)); 
            Rewards.Add(new CollectionItem(typeof(HeroOfTheUnlovedTitleDeed), 0x14F0, 1157649, 0, 20));
            Rewards.Add(new CollectionItem(typeof(SaviorOfTheDementedTitleDeed), 0x14F0, 1157651, 0, 30));
            Rewards.Add(new CollectionItem(typeof(SlayerOfThePumpkinKingTitleDeed), 0x14F0, 1157650, 0, 50));
            Rewards.Add(new CollectionItem(typeof(SterlingSilverRing), 0x1F09, 1155606, 0, 50));
            Rewards.Add(new CollectionItem(typeof(TalonsOfEscaping), 0x41D8, 1155682, 0, 50));
            Rewards.Add(new CollectionItem(typeof(BootsOfEscaping), 0x1711, 1155607, 0, 50));
        }
    }
}
