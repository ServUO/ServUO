using System;
using System.Collections.Generic;
using System.Text;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Regions;
using Server.Engines.CannedEvil;

namespace Server.Engines.CreatureStealing
{
    class StealingHandler
    {
        private static Type[] SpecialItemList = 
        { 
            typeof(SeedOflife), 
            typeof(BalmOfStrength), 
            typeof(BalmOfWisdom),
            typeof(BalmOfSwiftness), 
            typeof(ManaDraught), 
            typeof(BalmOfProtection), 
            typeof(StoneSkinLotion), 
            typeof(GemOfSalvation), 
            typeof(LifeShieldLotion),
            typeof(SmugglersLantern),
            typeof(SmugglersToolBox)
        };

        public static void HandleSteal(BaseCreature from, PlayerMobile thief, bool smugglersEdge)
        {
            if (from.HasBeenStolen)
            {
                thief.SendLocalizedMessage(1094948); //That creature has already been stolen from.  There is nothing left to steal.
                return; 
            }

            double stealing = thief.Skills.Stealing.Value;

            if (stealing < 100)
            {
                return;
            }

            if (!((thief.Map == Map.Felucca && thief.Region is DungeonRegion) || thief.Region is ChampionSpawnRegion || from is ExodusZealot))  
            {
                return; 
            }

            int fame = from.Fame;

            fame = Math.Max(1, fame);
            fame = Math.Min(30000, fame);

            int chance = 0;

            if (smugglersEdge)
            {
                chance = 0.05;
            }
            else
            {
                if (stealing == 120)
                    chance += 10;
                else if (stealing >= 110.1)
                    chance += 8;
                else if (stealing >= 100.1)
                    chance += 5;
                else if (stealing == 100)
                    chance += 2;

                int level = (int)(40.0 / 29999.0 * fame - 40.0 / 29999.0);

                if (level >= 40)
                    chance += 5;
                else if (level >= 40)
                    chance += 3;
                else if (level >= 35)
                    chance += 2;
                else if (level >= 25)
                    chance += 1;
            }

            if ((Utility.Random(100)+1) <= chance) 
            {
                thief.SendLocalizedMessage(1094947);//You successfully steal a special item from the creature!

                Item item;

                if (from is ExodusZealot)
                {
                    item = Activator.CreateInstance(ExodusChest.RituelItem[Utility.Random(ExodusChest.RituelItem.Length)]) as Item;
                }
                else
                {
                    if(smugglersEdge)
                        item = Activator.CreateInstance(SpecialItemList[Utility.Random(SpecialItemList.Length)]) as Item;
                    else
                        item = Activator.CreateInstance(SpecialItemList[Utility.Random(SpecialItemList.Length - 2)]) as Item;
                }

                thief.AddToBackpack(item);
            } 

            from.HasBeenStolen = true;
        }

    }
}
