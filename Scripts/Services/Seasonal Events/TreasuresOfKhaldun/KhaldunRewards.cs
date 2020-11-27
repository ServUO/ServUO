using Server.Items;
using System;
using System.Collections.Generic;

namespace Server.Engines.Khaldun
{
    public static class KhaldunRewards
    {
        public static List<CollectionItem> Rewards { get; set; }

        public static void Initialize()
        {
            Rewards = new List<CollectionItem>();

            Rewards.Add(new CollectionItem(typeof(KhaldunFirstAidBelt), 0xA1F6, 1158681, 0, 30));
            Rewards.Add(new CollectionItem(typeof(MaskOfKhalAnkur), 0xA1C7, 1158701, 0, 50));
            Rewards.Add(new CollectionItem(typeof(PendantOfKhalAnkur), 0xA1C9, 1158731, 0, 50));
            Rewards.Add(new CollectionItem(typeof(SeekerOfTheFallenStarTitleDeed), 5360, 1155604, 0, 20));
            Rewards.Add(new CollectionItem(typeof(ZealotOfKhalAnkurTitleDeed), 5360, 1155604, 0, 30));
            Rewards.Add(new CollectionItem(typeof(ProphetTitleDeed), 5360, 1155604, 0, 50));
            Rewards.Add(new CollectionItem(typeof(CultistsRitualTome), 0xEFA, 1158717, 0, 50));
            Rewards.Add(new CollectionItem(typeof(SterlingSilverRing), 0x1F09, 1155606, 0, 50));
            Rewards.Add(new CollectionItem(typeof(TalonsOfEscaping), 0x41D8, 1155682, 0, 50));
            Rewards.Add(new CollectionItem(typeof(BootsOfEscaping), 0x1711, 1155607, 0, 50));
        }
    }
}
