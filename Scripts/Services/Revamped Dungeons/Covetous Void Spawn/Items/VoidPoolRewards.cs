using Server;
using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using System.Linq;
using Server.Targeting;
using Server.Engines.Quests;
using Server.Multis;

namespace Server.Engines.VoidPool
{
    public static class VoidPoolRewards
    {
        public static List<CollectionItem> Rewards { get; set; }

        public static void Initialize()
        {
            Rewards = new List<CollectionItem>();

            Rewards.Add(new CollectionItem(typeof(CauldronOfTransmutationDeed), 0x9ED, 1152656, (int)CraftResource.Bronze, 100));	//Bronze
            Rewards.Add(new CollectionItem(typeof(CauldronOfTransmutationDeed), 0x9ED, 1152657, (int)CraftResource.Gold, 200));	    //Gold
            Rewards.Add(new CollectionItem(typeof(CauldronOfTransmutationDeed), 0x9ED, 1152658, (int)CraftResource.Agapite, 500));	//Agapite
            Rewards.Add(new CollectionItem(typeof(CauldronOfTransmutationDeed), 0x9ED, 1152659, (int)CraftResource.Verite, 1000));	//Verite

            Rewards.Add(new CollectionItem(typeof(CauldronOfTransmutationDeed), 0x9ED, 1152660, (int)CraftResource.AshWood, 100));	//Ash
            Rewards.Add(new CollectionItem(typeof(CauldronOfTransmutationDeed), 0x9ED, 1152661, (int)CraftResource.YewWood, 200));	//Yew
            Rewards.Add(new CollectionItem(typeof(CauldronOfTransmutationDeed), 0x9ED, 1152662, (int)CraftResource.Heartwood, 500));  // Heartwood
            Rewards.Add(new CollectionItem(typeof(CauldronOfTransmutationDeed), 0x9ED, 1152663, (int)CraftResource.Bloodwood, 1000)); // Bloodwood

            Rewards.Add(new CollectionItem(typeof(HarvestMap), 0x14EC, 1152665, (int)CraftResource.Gold, 250));	//Gold
            Rewards.Add(new CollectionItem(typeof(HarvestMap), 0x14EC, 1152666, (int)CraftResource.Agapite, 500));	//Agapite
            Rewards.Add(new CollectionItem(typeof(HarvestMap), 0x14EC, 1152667, (int)CraftResource.Verite, 1000));	//Verite
            Rewards.Add(new CollectionItem(typeof(HarvestMap), 0x14EC, 1152668, (int)CraftResource.Valorite, 2000));	//Valorite

            Rewards.Add(new CollectionItem(typeof(HarvestMap), 0x14EC, 1152669, (int)CraftResource.YewWood, 250));	//Yew
            Rewards.Add(new CollectionItem(typeof(HarvestMap), 0x14EC, 1152670, (int)CraftResource.Heartwood, 500));	//Heartwood
            Rewards.Add(new CollectionItem(typeof(HarvestMap), 0x14EC, 1152671, (int)CraftResource.Bloodwood, 1000));	//Bloodwood
            Rewards.Add(new CollectionItem(typeof(HarvestMap), 0x14EC, 1152672, (int)CraftResource.Frostwood, 2000));	//Frostwood

            Rewards.Add(new CollectionItem(typeof(SmeltersTalisman), 0x2F5B, 1152674, (int)CraftResource.Gold, 50)); 	// Gold
            Rewards.Add(new CollectionItem(typeof(SmeltersTalisman), 0x2F5B, 1152675, (int)CraftResource.Agapite, 100));	// Agapite
            Rewards.Add(new CollectionItem(typeof(SmeltersTalisman), 0x2F5B, 1152676, (int)CraftResource.Verite, 200));	// Verite
            Rewards.Add(new CollectionItem(typeof(SmeltersTalisman), 0x2F5B, 1152677, (int)CraftResource.Valorite, 500));	// Valorite

            Rewards.Add(new CollectionItem(typeof(WoodsmansTalisman), 0x2F5A, 1152678, (int)CraftResource.YewWood, 50)); // Yew
            Rewards.Add(new CollectionItem(typeof(WoodsmansTalisman), 0x2F5A, 1152679, (int)CraftResource.Heartwood, 100));// Heartwood
            Rewards.Add(new CollectionItem(typeof(WoodsmansTalisman), 0x2F5A, 1152680, (int)CraftResource.Bloodwood, 200));// Bloodwood
            Rewards.Add(new CollectionItem(typeof(WoodsmansTalisman), 0x2F5A, 1152681, (int)CraftResource.Frostwood, 500));// Frostwood

            Rewards.Add(new CollectionItem(typeof(TemporaryForgeDeed), 0xFB1,  1152682, 0, 250));
            Rewards.Add(new CollectionItem(typeof(MagicalFishFinder),  0x14F6, 1152683, 2500, 250));

            //TODO: Hues for below
            Rewards.Add(new CollectionItem(typeof(BraceletOfProtection), 0x1086, 1152730, 1157, 1840));
            Rewards.Add(new CollectionItem(typeof(Hephaestus), 0x1B76, 1152909, 1910, 2000));
            Rewards.Add(new CollectionItem(typeof(GargishHephaestus), 0x4204, 1152909, 1910, 2000));
            Rewards.Add(new CollectionItem(typeof(BlightOfTheTundra), 0x26C2, 1152910, 1165, 2515));
            Rewards.Add(new CollectionItem(typeof(GargishBlightOfTheTundra), 0x090A, 1152910, 1165, 2515));
        }

        public static Item DropRandomArtifact()
        {
            switch (Utility.Random(9))
            {
                case 0: return new PrismaticLenses();
                case 1: return new GargishPrismaticLenses();
                case 2: return new Brightblade();
                case 3: return new GargishBrightblade();
                case 4: return new BraceletOfProtection(false);
                case 5: return new BlightOfTheTundra(false);
                case 6: return new GargishBlightOfTheTundra(false);
                case 7: return new Hephaestus(false);
                case 8: return new GargishHephaestus(false);
            }

            return null;
        }
    }
}