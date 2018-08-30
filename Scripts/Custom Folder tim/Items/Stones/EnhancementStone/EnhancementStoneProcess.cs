/*
 * 
 * Equipment Enhancement System
 * Version 1.5
 * Designed for SVN 663 + ML
 * Modified for RunUO 2.2 SVN
 * 
 * Authored by Dougan Ironfist
 * Last Updated on 2/1/2012
 * 
 * The purpose of these scripts is to allow an easier means for shards with a smaller playerbase to be able to enhance their equipment
 * to be more able to handle tougher creatures and spawns.  For shards with a larger playerbase, these scripts can be used as means
 * to eliminate alot of excess gold from the player economy.
 * 
 * These scripts provide a deed for the Equipment Enhancement Stone.  This will allow players to put a stone in their house for easy
 * access and convenience.  The deed can be dispensed in whatever means the shard administrators feel is appropriate.
 * 
 * Alternately, shard administrators could simply place the actual Equipment Enhancement Stones within the cities on their shard
 * and eliminate the need to determine how to distribute deeds.  This could allow the administrators to promote PVP on their
 * shard by placing only a handful of stones in cities in Felucca (if PVP is a desired goal of the shard).
 * 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Gumps;

using Server.Mobiles;

namespace Server.Items
{
    public class EnhancementStoneProcess
    {
        /*
         * Modify the variables below to meet the specific needs of your shard and playerbase.
         * 
         * BaseCost is the base cost in gold to purchase each point of enhancement.  The default value
         * for this variable is 1000.  Lower this value to make enhancements cheaper for a small playerbase with a low gold
         * economy.  For shards with a large playerbase and alot of excess gold laying around, you can increase this
         * value to make enhancements more expensive and reduce the excess gold economy.
         * 
         * Note regarding base cost:
         *      Base cost is used as a multiplier against the maximum value that an attribute can be upgraded with 20 used as
         *      a max upgrade cost.  So for attributes liek Mage Weapon that can only be applied once, the cost would be the
         *      same as if you upgraded Defence Chance 15 times.
         * 
         * AttrCountAffectsCost is used to determine if the cost of enhancements rises as more attributes are
         * added to items.  This variable is set to true by default.  For small shards with a low playerbase, setting
         * this variable to false would make enhancements alot cheaper.  For large shards with a wealthy playerbase, it
         * is highly recommended to leave this value set to true to further reduce excess gold from your
         * shard's economy.
         * 
         * MaxAttrCount is used to limit the maximum number of attributes that can be enhanced on an item
         * by a player.  In this way the shard administrator can make the enhancement stone less powerful.  Please note
         * that attributes that are already on the item when it is found or crafted count towards the the attribute
         * count.  It is highly advised to make the purchase cost more expensive if the maximum enhancements is limited.
         * 
         */

        private int BaseCost = 500;
        private bool AttrCountAffectsCost = false;
        public int MaxAttrCount = 8;

        /*
         * DO NOT EDIT BEYOND THIS POINT
         * 
         * This script specifically does not allow for modifications of skills on items.  It also does not allow the
         * addition of slayer properties on weapons and spellbooks.  These omissions were made intentionally.  This
         * will force the players to find items with those properties BEFORE they can begin to enhance the items.
         * While this script could be modified to allow the addition of these properties, it is not advised and
         * assistance will not be provided to add these properties to the script.
         * 
         */

        public Mobile Owner = null;
        public Item ItemToUpgrade = null;
        public int CurrentAttributeCount = 0;

        public EnhancementStoneProcess(Mobile from, Item target)
        {
            Owner = from;
            ItemToUpgrade = target;
        }

        public void BeginProcess()
        {
            CurrentAttributeCount = 0;

            if (!(ItemToUpgrade is BaseShield || ItemToUpgrade is BaseArmor || ItemToUpgrade is BaseWeapon || ItemToUpgrade is BaseJewel || ItemToUpgrade is Spellbook))
            {
                Owner.SendMessage("This cannot be enhanced.");
            }
            else
            {
                int MaxedAttributes = 0;

                foreach (AttributeHandler handler in AttributeHandler.Definitions)
                {
                    int attr = handler.Upgrade(ItemToUpgrade, true);
                    
                    if (attr > 0)
                        CurrentAttributeCount++;

                    if (attr >= handler.MaxValue)
                        MaxedAttributes++;
                }

                if (CurrentAttributeCount > MaxAttrCount || MaxedAttributes >= MaxAttrCount)
                    Owner.SendMessage("This piece of equipment cannot be enhanced any further.");
                else
                    Owner.SendGump(new EnhancementGump(this));
            }
        }

        public void BeginUpgrade(AttributeHandler handler)
        {
            if (SpendGold(GetCostToUpgrade(handler)))
            {
                handler.Upgrade(ItemToUpgrade, false);
                BeginProcess();
            }
        }

        private bool SpendGold(int amount)
        {
            bool bought = (Owner.AccessLevel >= AccessLevel.GameMaster);
            bool fromBank = false;

           // Container cont = Owner.Backpack;
            if (!bought)
            {
                // if (cont.ConsumeTotal(typeof(Gold), amount))
                    // bought = true;
                if( Banker.GetBalance(Owner) >= amount )
                {
					bought = true;
					Withdraw( Owner, amount);
                    // cont = Owner.FindBankNoCreate();
                    // if (cont != null && cont.ConsumeTotal(typeof(Gold), amount))
                    // {
                        // bought = true;
                        // fromBank = true;
                    // }
                    //else
                    // {
                        // Owner.SendLocalizedMessage(500192);
                    // }
                }
				else
                {
					Owner.SendLocalizedMessage(500192);
                }
            }

            if (bought)
            {
                if (Owner.AccessLevel >= AccessLevel.GameMaster)
                    Owner.SendMessage("{0} gold would have been withdrawn from your bank if you were not a GM.", amount);
                //else if (fromBank)
				else
                    Owner.SendMessage("The total of your purchase is {0} gold, which has been withdrawn from your bank account.  My thanks for your patronage.", amount);
                // else
                    // Owner.SendMessage("The total of your purchase is {0} gold.  My thanks for your patronage.", amount);
            }

            return bought;
        }
		
		public static bool Withdraw(Mobile from, int amount)
		{
			return Banker.Withdraw(from, amount);
		}

        public int GetCostToUpgrade(AttributeHandler handler)
        {
            int attrMultiplier = 1;

            if (AttrCountAffectsCost)
            {
                foreach (AttributeHandler h in AttributeHandler.Definitions)
                    if (h.Name != handler.Name && h.Upgrade(ItemToUpgrade, true) > 0)
                        attrMultiplier++;
            }

            decimal cost = 0;

            int max = handler.MaxValue / handler.IncrementValue;
            int level = handler.Upgrade(ItemToUpgrade, true) / handler.IncrementValue;

            decimal currentLevel = 20M / max * level;
            decimal nextLevel = 20M / max * (level + 1);

            int loopCurrentLevel = (int)currentLevel;
            int loopNextLevel = (int)nextLevel;

            for (int i = loopCurrentLevel; i < loopNextLevel; i++)
            {
                if (i == loopCurrentLevel && loopCurrentLevel != currentLevel)
                {
                    decimal multiplier = i + 1 - (currentLevel - (decimal)loopCurrentLevel);
                    multiplier = multiplier * multiplier;
                    cost += multiplier * BaseCost;
                }
                else if (i == loopNextLevel - 1 && loopNextLevel != nextLevel)
                {
                    decimal multiplier = nextLevel;
                    multiplier = multiplier * multiplier;
                    cost += multiplier * BaseCost;
                }
                else
                    cost += (i + 1) * (i + 1) * BaseCost;
            }

            cost = cost * attrMultiplier;

            return (int)cost;
        }
    }
}