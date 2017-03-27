using System;
using System.Collections.Generic;
using System.Text;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Regions;
using Server.Engines.CannedEvil;

namespace drNO.ThieveItems
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

        };
        public static void HandleSteal(BaseCreature from, PlayerMobile thieve)
        {
            if (from.HasBeenStolen == true)
            {
                thieve.SendLocalizedMessage(1094948); //That creature has already been stolen from.  There is nothing left to steal.
                return; 
            }
            double stealing = thieve.Skills.Stealing.Value;
            if (stealing < 100)
            {
                return;
            }

            if (!((thieve.Map == Map.Felucca && thieve.Region is DungeonRegion) || thieve.Region is ChampionSpawnRegion || from is ExodusZealot))  
            {
                return; 
            }

            int fame = from.Fame;

            fame = Math.Max(1, fame);
            fame = Math.Min(30000, fame);

            int chance = 0;

            if (stealing == 120)
                chance += 10;
            else if (stealing >= 110.1)
                chance += 8;
            else if (stealing >= 100.1)
                chance += 5;
            else if (stealing == 100)
                chance += 2;

            int level = (int) (40.0/29999.0 * fame - 40.0 / 29999.0); 

            if (level >= 40) 
                chance += 5; 
            else if (level >= 40) 
                chance += 3; 
            else if (level >= 35) 
                chance += 2; 
            else if (level >= 25) 
                chance += 1; 

            if ((Utility.Random(100)+1) <= chance) 
            {
                thieve.SendLocalizedMessage(1094947);//You successfully steal a special item from the creature!

                Item itm;

                if (from is ExodusZealot)
                {
                    itm = Activator.CreateInstance(ExodusChest.RituelItem[Utility.Random(ExodusChest.RituelItem.Length)]) as Item;
                }
                else
                {
                    itm = Activator.CreateInstance(SpecialItemList[Utility.Random(SpecialItemList.Length)]) as Item;
                }                

                thieve.AddToBackpack(itm);
            } 


            from.HasBeenStolen = true;
        }

    }
}
